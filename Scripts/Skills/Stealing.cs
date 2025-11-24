using System;
using System.Collections;
using System.Collections.Generic;

using Midgard.Engines.Classes;
using Midgard.Engines.MidgardTownSystem;
using Server;
using Server.Misc;
using Server.Mobiles;
using Server.Targeting;
using Server.Items;
using Server.Network;
using Server.Factions;
using Server.Spells.Seventh;
using Server.Spells.Fifth;
using Server.Spells.Necromancy;
using Server.Spells;
using Server.Spells.Ninjitsu;
using System.IO;

namespace Server.SkillHandlers
{
	public class Stealing
	{
		public static void Initialize()
		{
			SkillInfo.Table[33].Callback = new SkillUseCallback( OnUse );
		}

		public static readonly bool ClassicMode = true; // mod by Dies Irae
		public static readonly bool SuspendOnMurder = false; // mod by Dies Irae

		public static bool IsInGuild( Mobile m )
		{
			#region mod by Dies Irae
			if( ClassSystem.IsThief( m ) )
				return true;
			#endregion

			return ( m is PlayerMobile && ((PlayerMobile)m).NpcGuild == NpcGuild.ThievesGuild );
		}

		public static bool IsInnocentTo( Mobile from, Mobile to )
		{
			#region modifica by Dies Irae
			int noto = Notoriety.Compute( from, to );
			return ( noto == Notoriety.Innocent || noto == Notoriety.Ally );
			// return ( Notoriety.Compute( from, (Mobile)to ) == Notoriety.Innocent );
			#endregion
		}

		private class StealingTarget : Target
		{
			private Mobile m_Thief;

			public StealingTarget( Mobile thief ) : base ( 1, false, TargetFlags.None )
			{
				m_Thief = thief;

				AllowNonlocal = true;
			}

			private Item TryStealItem( Item toSteal, ref bool caught )
			{
				Item stolen = null;

				object root = toSteal.RootParent;

				StealableArtifactsSpawner.StealableInstance si = null;
				if ( toSteal.Parent == null || !toSteal.Movable )
					si = StealableArtifactsSpawner.GetStealableInstance( toSteal );

				if ( !IsEmptyHanded( m_Thief ) )
				{
					m_Thief.SendLocalizedMessage( 1005584 ); // Both hands must be free to steal.
				}
				/* else if ( root is Mobile && ((Mobile)root).Player && IsInnocentTo( m_Thief, (Mobile)root ) && !IsInGuild( m_Thief ) )
				{
					m_Thief.SendLocalizedMessage( 1005596 ); // You must be in the thieves guild to steal from other players.
				}*/ // mod by Dies Irae
				else if ( SuspendOnMurder && root is Mobile && ((Mobile)root).Player && IsInGuild( m_Thief ) && m_Thief.Kills > 0 )
				{
					m_Thief.SendLocalizedMessage( 502706 ); // You are currently suspended from the thieves guild.
				}
				else if ( root is BaseVendor && ((BaseVendor)root).IsInvulnerable )
				{
					m_Thief.SendLocalizedMessage( 1005598 ); // You can't steal from shopkeepers.
				}
				else if ( root is PlayerVendor )
				{
					m_Thief.SendLocalizedMessage( 502709 ); // You can't steal from vendors.
				}
				else if ( !m_Thief.CanSee( toSteal ) )
				{
					m_Thief.SendLocalizedMessage( 500237 ); // Target can not be seen.
				}
				else if ( m_Thief.Backpack == null || !m_Thief.Backpack.CheckHold( m_Thief, toSteal, false, true ) )
				{
					m_Thief.SendLocalizedMessage( 1048147 ); // Your backpack can't hold anything else.
				}
				#region Sigils
				else if ( toSteal is Sigil )
				{						
					PlayerState pl = PlayerState.Find( m_Thief );
					Faction faction = ( pl == null ? null : pl.Faction );

					Sigil sig = (Sigil) toSteal;

					if ( !m_Thief.InRange( toSteal.GetWorldLocation(), 1 ) )
					{
						m_Thief.SendLocalizedMessage( 502703 ); // You must be standing next to an item to steal it.
					}
					else if ( root != null ) // not on the ground
					{
						m_Thief.SendLocalizedMessage( 502710 ); // You can't steal that!
					}
					else if ( faction != null )
					{
						if ( !m_Thief.CanBeginAction( typeof( IncognitoSpell ) ) )
						{
							m_Thief.SendLocalizedMessage( 1010581 ); //	You cannot steal the sigil when you are incognito
						}
						else if ( DisguiseTimers.IsDisguised( m_Thief ) )
						{
							m_Thief.SendLocalizedMessage( 1010583 ); //	You cannot steal the sigil while disguised
						}
						else if ( !m_Thief.CanBeginAction( typeof( PolymorphSpell ) ) )
						{
							m_Thief.SendLocalizedMessage( 1010582 ); //	You cannot steal the sigil while polymorphed				
						}
						else if( TransformationSpellHelper.UnderTransformation( m_Thief ) )
						{
							m_Thief.SendLocalizedMessage( 1061622 ); // You cannot steal the sigil while in that form.
						}
						else if ( AnimalForm.UnderTransformation( m_Thief ) )
						{
							m_Thief.SendLocalizedMessage( 1063222 ); // You cannot steal the sigil while mimicking an animal.
						}
						else if ( pl.IsLeaving )
						{
							m_Thief.SendLocalizedMessage( 1005589 ); // You are currently quitting a faction and cannot steal the town sigil
						}
						else if ( sig.IsBeingCorrupted && sig.LastMonolith.Faction == faction )
						{
							m_Thief.SendLocalizedMessage( 1005590 ); //	You cannot steal your own sigil
						}
						else if ( sig.IsPurifying )
						{
							m_Thief.SendLocalizedMessage( 1005592 ); // You cannot steal this sigil until it has been purified
						}
						else if ( m_Thief.CheckTargetSkill( SkillName.Stealing, toSteal, 80.0, 80.0 ) )
						{
							if ( Sigil.ExistsOn( m_Thief ) )
							{
								m_Thief.SendLocalizedMessage( 1010258 ); //	The sigil has gone back to its home location because you already have a sigil.
							}
							else if ( m_Thief.Backpack == null || !m_Thief.Backpack.CheckHold( m_Thief, sig, false, true ) )
							{
								m_Thief.SendLocalizedMessage( 1010259 ); //	The sigil has gone home because your backpack is full
							}
							else
							{
								if ( sig.IsBeingCorrupted )
									sig.GraceStart = DateTime.Now; // begin grace period

								m_Thief.SendLocalizedMessage( 1010586 ); // YOU STOLE THE SIGIL!!!   (woah, calm down now)

								if ( sig.LastMonolith != null && sig.LastMonolith.Sigil != null ) {
									sig.LastMonolith.Sigil = null;
									sig.LastStolen = DateTime.Now;
								}

								return sig;
							}
						}
						else
						{
							m_Thief.SendLocalizedMessage( 1005594 ); //	You do not have enough skill to steal the sigil
						}
					}
					else
					{
						m_Thief.SendLocalizedMessage( 1005588 ); //	You must join a faction to do that
					}
				}
				#endregion
				
				#region ARTEGORDONMOD
				// allow stealing of STEALABLE items on the ground or in containers
				else if ( si == null && ( toSteal.Parent == null || !toSteal.Movable ) && !ItemFlags.GetStealable(toSteal))
				{
					m_Thief.SendLocalizedMessage( 502710 ); // You can't steal that!
				}
				else if ( (toSteal.LootType == LootType.Newbied || toSteal.CheckBlessed( root ) )&& !ItemFlags.GetStealable(toSteal) && !toSteal.Insured )
				{
					m_Thief.SendLocalizedMessage( 502710 ); // You can't steal that!
				}
				else if ( Core.AOS && si == null && toSteal is Container && !ItemFlags.GetStealable(toSteal))
				{
					m_Thief.SendLocalizedMessage( 502710 ); // You can't steal that!
				}
				#endregion
				
				else if ( !m_Thief.InRange( toSteal.GetWorldLocation(), 1 ) )
				{
					m_Thief.SendLocalizedMessage( 502703 ); // You must be standing next to an item to steal it.
				}
				else if ( si != null && m_Thief.Skills[SkillName.Stealing].Value < 100.0 )
				{
					m_Thief.SendLocalizedMessage( 1060025, "", 0x66D ); // You're not skilled enough to attempt the theft of this item.
				}
				else if ( toSteal.Parent is Mobile )
				{
					m_Thief.SendLocalizedMessage( 1005585 ); // You cannot steal items which are equiped.
				}
				else if ( root == m_Thief )
				{
					m_Thief.SendLocalizedMessage( 502704 ); // You catch yourself red-handed.
				}
				else if ( root is Mobile && ((Mobile)root).AccessLevel > AccessLevel.Player )
				{
					m_Thief.SendLocalizedMessage( 502710 ); // You can't steal that!
				}
				else if ( root is Mobile && !m_Thief.CanBeHarmful( (Mobile)root ) )
				{
				}
				else if ( root is Corpse )
				{
					m_Thief.SendLocalizedMessage( 502710 ); // You can't steal that!
				}
				else if( root is Midgard2PlayerMobile )
				{
					return TryStealItemToMidgard2PlayerMobile( toSteal, (Mobile)root, ref caught );
				}
				else
				{
					double w = toSteal.Weight + toSteal.TotalWeight;

					if ( w > 10 )
					{
						m_Thief.SendMessage( m_Thief.Language == "ITA" ? "E' troppo pesante da rubare." : "That is too heavy to steal." );
					}
					else
					{
						if ( toSteal.Stackable && toSteal.Amount > 1 )
						{
							#region ARTEGORDON fix for zero-weight stackables
							int maxAmount = toSteal.Amount;
							if(toSteal.Weight > 0)
								maxAmount = (int)((m_Thief.Skills[SkillName.Stealing].Value / 10.0) / toSteal.Weight);
							#endregion
							
							if ( maxAmount < 1 )
								maxAmount = 1;
							//edit by arlas, stack piccoli buggati
							//else if ( maxAmount > toSteal.Amount )
							//	maxAmount = toSteal.Amount;

							int amount = Utility.RandomMinMax( 1, maxAmount );

							if ( amount >= toSteal.Amount )
							{
								int pileWeight = (int)Math.Ceiling( toSteal.Weight * toSteal.Amount );
								pileWeight *= 10;

								if ( m_Thief.CheckTargetSkill( SkillName.Stealing, toSteal, pileWeight - 22.5, pileWeight + 27.5 ) )
									stolen = toSteal;
							}
							else
							{
								int pileWeight = (int)Math.Ceiling( toSteal.Weight * amount );
								pileWeight *= 10;

								if ( m_Thief.CheckTargetSkill( SkillName.Stealing, toSteal, pileWeight - 22.5, pileWeight + 27.5 ) )
								{
									stolen = Mobile.LiftItemDupe( toSteal, toSteal.Amount - amount );

									if ( stolen == null )
										stolen = toSteal;
								}
							}
						}
						else
						{
							int iw = (int)Math.Ceiling( w );
							iw *= 10;

							#region modifica by ARTEGORDONMOD
							int rarityScaled = ScaleWeightFromRarity( toSteal );
							if( rarityScaled > 0 )
								iw = rarityScaled;
							#endregion

							if ( m_Thief.CheckTargetSkill( SkillName.Stealing, toSteal, iw - 22.5, iw + 27.5 ) )
								stolen = toSteal;
						}

						if ( stolen != null )
						{
							m_Thief.SendLocalizedMessage( 502724 ); // You succesfully steal the item.

							#region ARTEGORDONMOD
							try
							{
								TextWriter tw = File.AppendText( "Logs/StealableItemsLog.log" );
								tw.WriteLine( String.Format( "ItemType {0} - Serial {1} - Location {2} - DateTime {3} - Thief {4} - Account {5}",
								stolen.GetType().Name, stolen.Serial, stolen.Location, DateTime.Now, m_Thief.Name, m_Thief.Account.Username ) );
								tw.Close();
							}
							catch( Exception ex )
							{
								Console.Write( "Log failed: {0}", ex );
							}

							ItemFlags.SetTaken(stolen, true); // set the taken flag to trigger release from any controlling spawner
							ItemFlags.SetStealable(stolen, false); // clear the stealable flag so that the item can only be stolen once if it is later locked down.
							stolen.Movable = true; // release it if it was locked down
							#endregion

							if ( si != null )
							{
								toSteal.Movable = true;
								si.Item = null;
							}
						}
						else
						{
							m_Thief.SendLocalizedMessage( 502723 ); // You fail to steal the item.
						}

						caught = ( m_Thief.Skills[SkillName.Stealing].Value < Utility.Random( 150 ) );
					}
				}

				return stolen;
			}

			#region modifica by Dies Irae
			private Item TryStealItemToMidgard2PlayerMobile( Item toSteal, Mobile target, ref bool caught )
			{
				Item stolen = null;

				//peso totale dell'obiettivo (se contenitore considera gli item che contiene)
				double weight = toSteal.Weight + toSteal.TotalWeight;

				int str = m_Thief.Str;
				int dex = m_Thief.Dex;
				int stealing = m_Thief.Skills[SkillName.Stealing].Fixed;
				int hiding = m_Thief.Skills[SkillName.Hiding].Fixed;
				int stealth = m_Thief.Skills[SkillName.Stealth].Fixed;

				int detectHidden = target.Skills[SkillName.DetectHidden].Fixed;

				int victimMaxStat = target.Str;
				if( target.Int > victimMaxStat )
					victimMaxStat = target.Int;
				if( target.Dex > victimMaxStat )
					victimMaxStat = target.Dex;

				int maxWeight = stealing / 100;

				if( str <= 80 )
					maxWeight -= (int)( ( 80 - str ) / 10.0 );
				else
					maxWeight += (int)( ( str - 80 ) / 20.0 );

				// m_Thief.SendMessage( "Debug Stealing. maxWeight: {0}", maxWeight );

				double thiefChance = stealing + dex + ( stealth * 0.1 );

				double victimChance = ( weight * 10.0 ) + ( victimMaxStat * 5.0 );

				if( m_Thief.Hidden )
				{
					thiefChance += hiding * 0.5;
					victimChance += detectHidden * 0.25;
				}

				double chance = thiefChance - victimChance;

				// m_Thief.SendMessage( "Debug Stealing. Chance step 1: {0}/1000", chance.ToString( "F0" ) );

				if( !m_Thief.Hidden )
					chance = chance * 0.66;

				if( stealing >= 1200 )
					chance += 50;

				if( dex > 120 )
					chance += ( dex - 120 ) * 10;

				if( toSteal.Insured || toSteal.LootType == LootType.Blessed )
					chance = chance * 0.5;

				//m_Thief.SendMessage( "Debug Stealing. Chance step 2: {0}/1000", chance.ToString( "F0" ) );

				m_Thief.CheckSkill( SkillName.Stealing, 21.1, 120.0 );

				if( chance > 1000.0 )
					chance = 1000.0;

				bool success = Utility.Random( 1000 ) < chance;

				//m_Thief.SendMessage( "Debug Stealing. Success: {0}", success.ToString() );

				if( !success )
					success = ( Utility.Dice( 1, 20, 0 ) == 1 );
				else
					success = ( Utility.Dice( 1, 20, 0 ) != 20 );

				//m_Thief.SendMessage( "Debug Stealing. Success FINAL: {0}", success.ToString() );

				if( weight > maxWeight )
				{
					m_Thief.SendMessage( "That is too heavy to steal." );
				}
				else
				{
					if( toSteal.Stackable && toSteal.Amount > 1 )
					{
						int maxAmount = toSteal.Amount;
						if( toSteal.Weight > 0 )
							maxAmount = (int)( ( m_Thief.Skills[SkillName.Stealing].Value / 10.0 ) / toSteal.Weight );

						if( maxAmount < 1 )
							maxAmount = 1;
						
						/*
						else if( maxAmount > toSteal.Amount )
							maxAmount = toSteal.Amount;
						*/

						int bonusScalar = (int)( ( m_Thief.Skills[ SkillName.Stealing ].Value / 10 ) * 3 );
						int minAmount = ( maxAmount * bonusScalar ) / 100;
						if( minAmount > maxAmount )
							minAmount = maxAmount;

						int amount = Utility.RandomMinMax( minAmount, maxAmount );

						if( amount >= toSteal.Amount )
						{
							if( success )
								stolen = toSteal;
						}
						else
						{
							if( success )
							{
								stolen = Mobile.LiftItemDupe( toSteal, toSteal.Amount - amount );

								if( stolen == null )
									stolen = toSteal;
							}
						}
					}
					else
					{
						if( success )
							stolen = toSteal;
					}

					if( stolen != null )
					{
						m_Thief.SendLocalizedMessage( 502724 ); // You succesfully steal the item.
					}
					else
					{
						m_Thief.SendLocalizedMessage( 502723 ); // You fail to steal the item.
					}

					caught = !success && ( m_Thief.Skills[SkillName.Stealing].Value < Utility.Random( 150 ) );
				}
				
				return stolen;
			}

			private static int ScaleWeightFromRarity( Item toSteal )
			{
				// Begin mod for ArtifactRarity difficulty scaling
				// add in an additional difficulty factor for objects with ArtifactRarity
				// with rarity=1 requiring a minimum of 100 stealing, and rarity 12 requiring a minimum of 118
				// Note, this is completely independent of weight

				Type propertyType;
				string intensity = BaseXmlSpawner.GetBasicPropertyValue( toSteal, "ArtifactRarity", out propertyType );

				if( propertyType == typeof( int ) && intensity != null )
				{
					int rarity = 0;
					try
					{
						rarity = int.Parse( intensity );
					}
					catch( Exception ex )
					{
						Console.WriteLine( ex.ToString() );
					}

					// rarity difficulty scaling
					return rarity > 0 ? (int)Math.Ceiling( 120 + rarity * 1.5 ) : 0;
				}
				else
					return 0;
			}
			#endregion

			protected override void OnTarget( Mobile from, object target )
			{	
				//from.RevealingAction(); // Modifica by Dies Irae

				Item stolen = null;
				object root = null;
				bool caught = false;

				if ( target is Item )
				{
					root = ((Item)target).RootParent;
					stolen = TryStealItem( (Item)target, ref caught );
				} 
				else if ( target is Mobile )
				{
					Container pack = ((Mobile)target).Backpack;

					if ( pack != null && pack.Items.Count > 0 )
					{
						int randomIndex = Utility.Random( pack.Items.Count );

						root = target;

						if( pack.Items[randomIndex] != null && pack.Items[randomIndex].InstanceOwner != target ) // mod by Dies Irae
							stolen = TryStealItem( pack.Items[randomIndex], ref caught );
					}
					#region mod by Dies Irae
					else
					{
						m_Thief.SendMessage( m_Thief.Language == "ITA" ? "Non sembra esser rimasto nulla da rubare." : "It seems that there is nothing left to steal." );
					}
					#endregion
				} 
				else 
				{
					m_Thief.SendLocalizedMessage( 502710 ); // You can't steal that!
				}

				if ( stolen != null )
				{
					#region modifica by Dies Irae
					if( stolen.Insured )
					{
						Mobile mobRoot = (Mobile)root;

						bool canPayInsuranceCost =  ( Banker.GetBalance( mobRoot ) >= 600 );

						if( canPayInsuranceCost )
						{
							Banker.Withdraw( mobRoot, 600 );
							Banker.Deposit( m_Thief, 600 );

							mobRoot.SendMessage( 37, "Hey, one of your item was stolen. Because of its insurance status it was placed in your bankbox!" );
							m_Thief.SendMessage( 37, "Item you have stolen has been placed in its owner bank. You have been refunded with gold pieces!" );
							mobRoot.BankBox.DropItem( stolen );
						}
						else
						{
							mobRoot.SendMessage( 37, "Hey, one of your insured items was successfully stolen." );
							m_Thief.SendMessage( 37, "You have successfully stolen an insured item." );						
							from.AddToBackpack( stolen );
						}

						try
						{
							TextWriter tw = File.AppendText( "Logs/MidgardStolenItems.log" );
							tw.WriteLine( String.Format( "Item {0} - Serial {1} - Owner {2} (account {3}) - Thief {4} (account {5}) - DateTime {6} - canPayInsuranceCost {7}",
														stolen.GetType().Name, stolen.Serial, 
														mobRoot.Name, mobRoot.Account.Username,
														m_Thief.Name, m_Thief.Account.Username,
														DateTime.Now, canPayInsuranceCost ) );
							tw.Close();
						}
						catch( Exception ex )
						{
							Console.WriteLine( ex.ToString() );
						}
					}
					else
					{
						from.AddToBackpack( stolen );
					}
					#endregion

					if( !ClassicMode ) // mod by Dies Irae
						StolenItem.Add( stolen, m_Thief, root as Mobile );

					/*
					var karma:=GetKarma(who);
					var lossKarma:=0;
					if (karma>-625)
						lossKarma:=-RandomInt(300)+1;
					elseif (karma>-2500)
						lossKarma:=-RandomInt(100)+1;
					elseif (karma>-5000)
						lossKarma:=-RandomInt(20)+1;
					 */

					int karma = from.Karma;
					int loss = 0;
					if( karma > -625 )
						loss = -( Utility.Random( 30 ) + 1 );
					else if( karma > -1250 )
						loss = -( Utility.Random( 10 ) + 1 );
					else if( karma > -2500 )
						loss = -( Utility.Random( 5 ) + 1 );

					Titles.AwardKarma( from, loss, true );
				}

				if ( caught )
				{
					if ( root == null )
					{
						m_Thief.CriminalAction( false );
					}
					else if ( root is Corpse && ((Corpse)root).IsCriminalAction( m_Thief ) )
					{
						m_Thief.CriminalAction( false );
					}
					else if ( root is Mobile )
					{
						Mobile mobRoot = (Mobile)root;

						if ( !IsInGuild( mobRoot ) && IsInnocentTo( m_Thief, mobRoot ) )
							m_Thief.CriminalAction( false );

						string message = String.Format( m_Thief.Language == "ITA" ? "Noti che {0} cerca di derubare {1}." : "You notice {0} trying to steal from {1}.", m_Thief.Name, mobRoot.Name );

						foreach ( NetState ns in m_Thief.GetClientsInRange( 8 ) )
						{
							if ( ns.Mobile != m_Thief )
								ns.Mobile.SendMessage( message );
						}

						#region mod by Dies Irae
						TownSystem sys = TownSystem.Find( m_Thief.Location, m_Thief.Map );
						if( sys != null )
							sys.RegisterCriminal( m_Thief, CrimeType.StealAction );
						#endregion
					}
				}
				else if ( root is Corpse && ((Corpse)root).IsCriminalAction( m_Thief ) )
				{
					m_Thief.CriminalAction( false );
				}

				if ( caught && root is Mobile && ((Mobile)root).Player && m_Thief is PlayerMobile && IsInnocentTo( m_Thief, (Mobile)root ) && !IsInGuild( (Mobile)root ) )
				{
					PlayerMobile pm = (PlayerMobile)m_Thief;

					pm.PermaFlags.Add( (Mobile)root );
					pm.Delta( MobileDelta.Noto );
				}
			}
		}

		public static bool IsEmptyHanded( Mobile from )
		{
			if ( from.FindItemOnLayer( Layer.OneHanded ) != null )
				return false;

			if ( from.FindItemOnLayer( Layer.TwoHanded ) != null )
				return false;

			return true;
		}

		#region mod by Dies Irae
		public static bool IsHeavyEquipped( Mobile from )
		{
			if( from.AccessLevel > AccessLevel.Player )
				return false;

			List<Item> items = from.Items;

			for( int i = 0; i < items.Count; ++i )
			{
				Item obj = items[ i ];

				if( obj.BlockCircle != -1 && obj.BlockCircle <= 6 )
					return true;
			}

			return false;
		}
		#endregion

		public static TimeSpan OnUse( Mobile m )
		{
			if ( !IsEmptyHanded( m ) )
			{
				m.SendLocalizedMessage( 1005584 ); // Both hands must be free to steal.
			}
			#region mod by Dies Irae
			else if( IsHeavyEquipped( m ) )
			{
				m.SendMessage( m.Language == "ITA" ? "Solo un pazzo ruberebbe con addosso vestiti tanto rumorosi!" : "Only a fool would steal in noisey armor!" );
			}
			#endregion
			else
			{
				m.Target = new StealingTarget( m );
				// m.RevealingAction(); // modifica by Dies Irae per non rivelare OnSteal

				m.SendLocalizedMessage( 502698 ); // Which item do you want to steal?
			}

			return TimeSpan.FromSeconds( 10.0 );
		}
	}

	public class StolenItem
	{
		public static readonly TimeSpan StealTime = TimeSpan.FromMinutes( 2.0 );

		private Item m_Stolen;
		private Mobile m_Thief;
		private Mobile m_Victim;
		private DateTime m_Expires;

		public Item Stolen{ get{ return m_Stolen; } }
		public Mobile Thief{ get{ return m_Thief; } }
		public Mobile Victim{ get{ return m_Victim; } }
		public DateTime Expires{ get{ return m_Expires; } }

		public bool IsExpired{ get{ return ( DateTime.Now >= m_Expires ); } }

		public StolenItem( Item stolen, Mobile thief, Mobile victim )
		{
			m_Stolen = stolen;
			m_Thief = thief;
			m_Victim = victim;

			m_Expires = DateTime.Now + StealTime;
		}

		private static Queue m_Queue = new Queue();

		public static void Add( Item item, Mobile thief, Mobile victim )
		{
			Clean();

			m_Queue.Enqueue( new StolenItem( item, thief, victim ) );
		}

		public static bool IsStolen( Item item )
		{
			Mobile victim = null;

			return IsStolen( item, ref victim );
		}

		public static bool IsStolen( Item item, ref Mobile victim )
		{
			Clean();

			foreach ( StolenItem si in m_Queue )
			{
				if ( si.m_Stolen == item && !si.IsExpired )
				{
					victim = si.m_Victim;
					return true;
				}
			}

			return false;
		}

		public static void ReturnOnDeath( Mobile killed, Container corpse )
		{
			Clean();

			foreach ( StolenItem si in m_Queue )
			{
				if ( si.m_Stolen.RootParent == corpse && si.m_Victim != null && !si.IsExpired )
				{
					if ( si.m_Victim.AddToBackpack( si.m_Stolen ) )
						si.m_Victim.SendLocalizedMessage( 1010464 ); // the item that was stolen is returned to you.
					else
						si.m_Victim.SendLocalizedMessage( 1010463 ); // the item that was stolen from you falls to the ground.

					si.m_Expires = DateTime.Now; // such a hack
				}
			}
		}

		public static void Clean()
		{
			while ( m_Queue.Count > 0 )
			{
				StolenItem si = (StolenItem) m_Queue.Peek();

				if ( si.IsExpired )
					m_Queue.Dequeue();
				else
					break;
			}
		}
	}
}