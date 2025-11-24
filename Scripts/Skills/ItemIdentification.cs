using System;

using Server.Targeting;
using Server.Mobiles;
using Midgard.Engines.StoneEnchantSystem;
using Midgard.Engines;

namespace Server.Items
{
	public class ItemIdentification
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.ItemID].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile from )
		{
			from.SendLocalizedMessage( 500343 ); // What do you wish to appraise and identify?
			from.Target = new InternalTarget();

			return TimeSpan.FromSeconds( Core.AOS ? 1.0 : 10.0 );
		}

		[PlayerVendorTarget]
		private class InternalTarget : Target
		{
			public InternalTarget() :  base ( 8, false, TargetFlags.None )
			{
				AllowNonlocal = true;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Item )
				{
					if( Utility.Random( 101 ) < from.Skills[SkillName.ItemID].Value )
					{
						from.CheckSkill( SkillName.ItemID, 0.01 );

						// mod by Dies Irae
						// http://forum.uosecondage.com/viewtopic.php?f=7&t=3403
						// All Weapons, Armor, Wands, Jewelry and Clothing now identified per mobile (50 max)
						/*
                        if( Core.AOS )
						{
							if ( o is BaseWeapon )
								((BaseWeapon)o).Identified = true;
							else if ( o is BaseArmor )
								((BaseArmor)o).Identified = true;
						}
                        */

						#region modifica by Dies Irae
						if( !Core.AOS )
						{
							if( o is KeyRing )
							{
								( (KeyRing)o ).DisplayInfoTo( from );
							}
							else if( o is IIdentificable )
							{
								IIdentificable identificable = (IIdentificable)o;

								identificable.InvalidateSecondAgeNames();
								identificable.AddIdentifier( from );
								identificable.DisplayItemInfo( from );

								from.SendMessage( from.Language == "ITA" ? "Analizzi con cura questo oggetto." : "You analyze carefully this item." );
							}
							else if( o is EnchantStone )
							{
								EnchantStone stone = ( (EnchantStone)o );

								if( !stone.Identified )
									stone.OnIdentification( from, Utility.Random( 3 ) == 1, false );//Utility.Random( 20 ) == 1 );
								else
									stone.DisplayInfo( from );
							}

							StoneEnchantItem state = StoneEnchantItem.Find( (Item)o );
							if( state != null )
								state.DisplayInfo( from );

							if( o is IDurability && ( (IDurability)o ).MaxHitPoints != 0 )
							{
								SendDurabilityMessage( from, (IDurability)o );
							}

							if( o is IUsesRemaining )
							{
								IUsesRemaining usesRemaining = ( (IUsesRemaining)o );
								if (from.Language == "ITA")
									from.SendMessage( "Questo oggetto ha {0} us{1} rimast{1}.", usesRemaining.UsesRemaining, usesRemaining.UsesRemaining > 1 ? "i" : "o" );
								else
									from.SendMessage( "This item has {0} use{1} remaining.", usesRemaining.UsesRemaining, usesRemaining.UsesRemaining > 1 ? "s" : "" );
							}

							if( !( o is BaseWeapon || o is BaseArmor ) )
							{
								Item item = (Item)o;
								if( item.CustomQuality != Quality.Undefined )
									from.SendMessage( from.Language == "ITA" ? "La qualità di questo oggetto è: {0}" : "The quality of this item is: {0}", Quality.GetQualityName( item ) );
							}
						}
						else
						{
							if( XmlForceIdentify.IsUnidentified( (Item)o ) )
								XmlForceIdentify.HandleIdentification( from, (Item)o );
							else if( o is BaseJewel )
							{
							    int level = (int)Math.Floor( XmlForceIdentify.GetItemRarity( (BaseJewel)o ) );

							    from.SendMessage( level <= 5 ? String.Format( from.Language == "ITA" ? "Questo gioiello si classifica {0}" : "This jewel could be classified as {0}",
							         Enum.GetName( typeof( JewelIntensity ), level ) ) : (from.Language == "ITA" ? "Questo oggetto è Leggendario" : "This jewel could be classified as Legendary") );
							}
						}
						#endregion

						if ( !Core.AOS )
							((Item)o).OnSingleClick( from );

						// ARTEGORDONMOD XML allows the identify skill to reveal attachments
						Engines.XmlSpawner2.XmlAttach.RevealAttachments( from, o );
					}
					else
					{
						from.CheckSkill( SkillName.ItemID, 0.95 );
						from.SendLocalizedMessage( 500353 ); // You are not certain...
					}
				}
				else if ( o is Mobile )
				{
					((Mobile)o).OnSingleClick( from );
				}
				else
				{
					from.SendLocalizedMessage( 500353 ); // You are not certain...
				}
			}
		}

		private static void SendDurabilityMessage( Mobile from, IDurability durability )
		{
			if( durability.MaxHitPoints != 0 )
			{
				int hp = (int)( ( durability.HitPoints / (double)durability.MaxHitPoints ) * 10 );

				if( hp < 0 )
					hp = 0;
				else if( hp > 9 )
					hp = 9;

				from.SendLocalizedMessage( 1038285 + hp );
			}
		}
	}
}