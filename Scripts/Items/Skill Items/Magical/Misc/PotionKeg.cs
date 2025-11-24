using System;
using Server;
using Server.Engines.Craft;
using Server.Items;

using Midgard.Engines.PlantSystem;
using Midgard.Items;
using System.Collections.Generic;

namespace Server.Items
{
	public class PotionKeg : Item, ITasteIdentificable
	{
		private PotionEffect m_Type;
		private int m_Held;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Held
		{
			get
			{
				return m_Held;
			}
			set
			{
				if ( m_Held != value )
				{
					m_Held = value;
					UpdateWeight();
					InvalidateProperties();
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public PotionEffect Type
		{
			get
			{
				return m_Type;
			}
			set
			{
				m_Type = value;
				InvalidateProperties();
			}
		}

		[Constructable]
		public PotionKeg() : base( 0x1940 )
		{
			UpdateWeight();

            m_Owner = null;
            m_IdentifiersList = new List<Mobile>(); // mod by Dies Irae
		}

		public virtual void UpdateWeight()
		{
			int held = Math.Max( 0, Math.Min( m_Held, 100 ) );

			this.Weight = 20 + ((held * 80) / 100);
		}

		public PotionKeg( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version

            // version 2 : mod by Dies Irae
            writer.Write( m_Owner );
            writer.Write( (bool) ( m_IdentifiersList != null ) );
            if ( m_IdentifiersList != null )
                writer.Write( m_IdentifiersList, true );

			writer.Write( (int) m_Type );
			writer.Write( (int) m_Held );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
                case 2:
                    m_Owner = reader.ReadMobile();
					if ( reader.ReadBool() )
						m_IdentifiersList = reader.ReadStrongMobileList();
                    goto case 1;
				case 1:
				case 0:
				{
					m_Type = (PotionEffect)reader.ReadInt();
					m_Held = reader.ReadInt();

					break;
				}
			}

			if ( version < 1 )
				Timer.DelayCall( TimeSpan.Zero, new TimerCallback( UpdateWeight ) );
		}

		#region modifica by Dies Irae per il nome del keg
		public override void AddNameProperty(ObjectPropertyList list)
		{
			if ( m_Held < 1 )
				list.Add( 1041641 );
			else
			{
				int label;
				switch( m_Type )
				{
					case PotionEffect.Conflagration: 			label = 1065901; break;
					case PotionEffect.ConflagrationGreater: 	label = 1065902; break;
					
					case PotionEffect.ConfusionBlast: 			label = 1065903; break;
					case PotionEffect.ConfusionBlastGreater: 	label = 1065904; break;
					
					case PotionEffect.Parasitic: 				label = 1065905; break;
					case PotionEffect.Darkglow: 				label = 1065906; break;
					
					case PotionEffect.Invisibility: 			label = 1065907; break;
					
					case PotionEffect.IntelligenceLesser: 		label = 1065908; break;
					case PotionEffect.Intelligence: 			label = 1065909; break;
					case PotionEffect.IntelligenceGreater: 		label = 1065910; break;
					
					case PotionEffect.ResistancesLesser: 		label = 1065911; break;
					case PotionEffect.Resistances: 				label = 1065912; break;
					case PotionEffect.ResistancesGreater: 		label = 1065913; break;
					
					case PotionEffect.Bless: 					label = 1065914; break;
					case PotionEffect.BlessGreater: 			label = 1065915; break;
					
					case PotionEffect.ManaRefreshLesser: 		label = 1065916; break;
					case PotionEffect.ManaRefresh: 				label = 1065917; break;
					case PotionEffect.ManaRefreshGreater: 		label = 1065918; break;
					
					case PotionEffect.Tamla: 					label = 1065919; break;
					
					case PotionEffect.TrasmutationLesser: 		label = 1065920; break;
					case PotionEffect.TrasmutationGreater: 		label = 1065921; break;
					
					case PotionEffect.Totem: 					label = 1065922; break;
					case PotionEffect.Elixir: 					label = 1065923; break;
					
					case PotionEffect.FlashBang: 				label = 1065924; break;
					
					case PotionEffect.ColdResistanceLesser: 	label = 1065925; break;
					case PotionEffect.ColdResistance: 			label = 1065926; break;
					case PotionEffect.ColdResistanceGreater: 	label = 1065927; break;
					
					case PotionEffect.EnergyResistanceLesser: 	label = 1065928; break;
					case PotionEffect.EnergyResistance: 		label = 1065929; break;
					case PotionEffect.EnergyResistanceGreater: 	label = 1065930; break;
					
					case PotionEffect.FireResistanceLesser: 	label = 1065931; break;
					case PotionEffect.FireResistance: 			label = 1065932; break;
					case PotionEffect.FireResistanceGreater: 	label = 1065933; break;
					
					case PotionEffect.PoisonResistanceLesser: 	label = 1065934; break;
					case PotionEffect.PoisonResistance: 		label = 1065935; break;
					case PotionEffect.PoisonResistanceGreater: 	label = 1065936; break;
					
					case PotionEffect.PetResurrection: 			label = 1065937; break;
					case PotionEffect.PetHeal: 					label = 1065938; break;
					case PotionEffect.PetHealGreater: 			label = 1065939; break;
					case PotionEffect.PetCure: 					label = 1065940; break;
					case PotionEffect.PetCureGreater: 			label = 1065941; break;
					case PotionEffect.PetShrink: 				label = 1065942; break;
					case PotionEffect.PetDyeBlack: 				label = 1065943; break;
					
					case PotionEffect.NarcoticLight: 			label = 1065944; break;
					case PotionEffect.NarcoticRegular: 			label = 1065945; break;
					case PotionEffect.NarcoticMedium: 			label = 1065946; break;
					case PotionEffect.NarcoticHeavy: 			label = 1065947; break;
					
					case PotionEffect.PesticideLight: 			label = 1065948; break;
					case PotionEffect.PesticideMedium: 			label = 1065949; break;
					case PotionEffect.PesticideHeavy: 			label = 1065950; break;
					
					case PotionEffect.FungicideLight: 			label = 1065951; break;
					case PotionEffect.FungicideMedium: 			label = 1065952; break;
					case PotionEffect.FungicideHeavy: 			label = 1065953; break;
					
					case PotionEffect.FertilizerLight: 			label = 1065954; break;
					case PotionEffect.FertilizerMedium: 		label = 1065955; break;
					case PotionEffect.FertilizerHeavy: 			label = 1065956; break;
					
					default: 									label = 0;		 break;
				}
				
				if( label != 0 )
					list.Add( string.Format( "A keg of {0} potions", StringList.Localization[ label ]  ) ); // A keg of ~1_TYPE~ potions
				else
					list.Add( 1041620 + (int)m_Type );
			}
		}

		//public override int LabelNumber{ get{ return (m_Held > 0 ? 1041620 + (int)m_Type : ); } }
		#endregion

        #region IIdentificable members : mod by Dies Irae
        private Mobile m_Owner;

        private List<Mobile> m_IdentifiersList;

        private const int MaxIdentifiers = 50;

        public void CopyIdentifiersTo( ITasteIdentificable tasteIdentificable )
        {
            if( m_IdentifiersList != null && m_IdentifiersList.Count > 0 )
            {
                foreach( Mobile mobile in m_IdentifiersList )
                {
                    tasteIdentificable.AddIdentifier( mobile );
                }
            }
        }

        public void ClearIdentifiers()
        {
            if( m_IdentifiersList != null && m_IdentifiersList.Count > 0 )
                m_IdentifiersList.Clear();
        }

        public void AddIdentifier( Mobile from )
        {
            if( m_IdentifiersList == null )
                m_IdentifiersList = new List<Mobile>();

            if( m_IdentifiersList.Contains( from ) )
            {
                SpamIdentifiers();
                return;
            }

            m_IdentifiersList.Add( from );

            if( m_IdentifiersList.Count > MaxIdentifiers )
                m_IdentifiersList.RemoveAt( 0 );
        }

        private void SpamIdentifiers()
        {
            if( !Core.Debug )
                return;

            Utility.Log( "PotionKegSpamIdentifiers.log", "IdentifiersList for potion keg {0}", Serial.ToString() );
            foreach( Mobile m in m_IdentifiersList )
                Utility.Log( "PotionKegSpamIdentifiers.log", m.Name );
        }

	    public string AlreadyIdentifiedMessage
	    {
            get { return "You are already familiar with this keg's contents."; }
	    }

	    public bool IsIdentifiedFor( Mobile from )
        {
            if( from == m_Owner )
                return true;

            if( BasePotion.IsStandardEffect( m_Type ) )
                return true;

            if( m_IdentifiersList == null )
                return false;

            return m_IdentifiersList.Contains( from );
        }

        public void DisplayItemInfo( Mobile from )
        {
            if( !IsIdentifiedFor( from ) )
                return;

            OnSingleClick( from );
        }

        public string GetUnidentifiedName()
        {
            int label = 0;

			switch( m_Type )
			{
                case PotionEffect.Nightsight: 		label = 1041611; break; // A keg of black liquid.
	            case PotionEffect.CureLesser:
	            case PotionEffect.Cure:
	            case PotionEffect.CureGreater: 		label = 1041612; break; // A keg of orange liquid.
	            case PotionEffect.Agility:
	            case PotionEffect.AgilityGreater: 	label = 1041613; break; // A keg of blue liquid.
	            case PotionEffect.Strength:
	            case PotionEffect.StrengthGreater: 	label = 1041614; break; // A keg of white liquid.
	            case PotionEffect.PoisonLesser:
	            case PotionEffect.Poison:
	            case PotionEffect.PoisonGreater:
	            case PotionEffect.PoisonDeadly: 	label = 1041615; break; // A keg of green liquid.
	            case PotionEffect.Refresh:
	            case PotionEffect.RefreshTotal: 	label = 1041616; break; // A keg of red liquid.
	            case PotionEffect.HealLesser:
	            case PotionEffect.Heal:
	            case PotionEffect.HealGreater: 		label = 1041617; break; // A keg of yellow liquid.
	            case PotionEffect.ExplosionLesser:
	            case PotionEffect.Explosion:
	            case PotionEffect.ExplosionGreater: label = 1041618; break; // A keg of purple liquid.

                default: label = 1041610;break; // A keg of strange liquid.
            }

            return StringList.Localization[ label ];
        }

        public string GetName()
        {
			int label = 0;

			switch( m_Type )
			{
				case PotionEffect.Conflagration: 			label = 1065901; break;
				case PotionEffect.ConflagrationGreater: 	label = 1065902; break;
				
				case PotionEffect.ConfusionBlast: 			label = 1065903; break;
				case PotionEffect.ConfusionBlastGreater: 	label = 1065904; break;
				
				case PotionEffect.Parasitic: 				label = 1065905; break;
				case PotionEffect.Darkglow: 				label = 1065906; break;
				
				case PotionEffect.Invisibility: 			label = 1065907; break;
				
				case PotionEffect.IntelligenceLesser: 		label = 1065908; break;
				case PotionEffect.Intelligence: 			label = 1065909; break;
				case PotionEffect.IntelligenceGreater: 		label = 1065910; break;
				
				case PotionEffect.ResistancesLesser: 		label = 1065911; break;
				case PotionEffect.Resistances: 				label = 1065912; break;
				case PotionEffect.ResistancesGreater: 		label = 1065913; break;
					
				case PotionEffect.Bless: 					label = 1065914; break;
				case PotionEffect.BlessGreater: 			label = 1065915; break;
				
				case PotionEffect.ManaRefreshLesser: 		label = 1065916; break;
				case PotionEffect.ManaRefresh: 				label = 1065917; break;
				case PotionEffect.ManaRefreshGreater: 		label = 1065918; break;
				
				case PotionEffect.Tamla: 					label = 1065919; break;
				
				case PotionEffect.TrasmutationLesser: 		label = 1065920; break;
				case PotionEffect.TrasmutationGreater: 		label = 1065921; break;
				
				case PotionEffect.Totem: 					label = 1065922; break;
				case PotionEffect.Elixir: 					label = 1065923; break;
				
				case PotionEffect.FlashBang: 				label = 1065924; break;
				
				case PotionEffect.ColdResistanceLesser: 	label = 1065925; break;
				case PotionEffect.ColdResistance: 			label = 1065926; break;
				case PotionEffect.ColdResistanceGreater: 	label = 1065927; break;
				
				case PotionEffect.EnergyResistanceLesser: 	label = 1065928; break;
				case PotionEffect.EnergyResistance: 		label = 1065929; break;
				case PotionEffect.EnergyResistanceGreater: 	label = 1065930; break;
				
				case PotionEffect.FireResistanceLesser: 	label = 1065931; break;
				case PotionEffect.FireResistance: 			label = 1065932; break;
				case PotionEffect.FireResistanceGreater: 	label = 1065933; break;
				
				case PotionEffect.PoisonResistanceLesser: 	label = 1065934; break;
				case PotionEffect.PoisonResistance: 		label = 1065935; break;
				case PotionEffect.PoisonResistanceGreater: 	label = 1065936; break;
				
				case PotionEffect.PetResurrection: 			label = 1065937; break;
				case PotionEffect.PetHeal: 					label = 1065938; break;
				case PotionEffect.PetHealGreater: 			label = 1065939; break;
				case PotionEffect.PetCure: 					label = 1065940; break;
				case PotionEffect.PetCureGreater: 			label = 1065941; break;
				case PotionEffect.PetShrink: 				label = 1065942; break;
				case PotionEffect.PetDyeBlack: 				label = 1065943; break;
				
				case PotionEffect.NarcoticLight: 			label = 1065944; break;
				case PotionEffect.NarcoticRegular: 			label = 1065945; break;
				case PotionEffect.NarcoticMedium: 			label = 1065946; break;
				case PotionEffect.NarcoticHeavy: 			label = 1065947; break;
				
				case PotionEffect.PesticideLight: 			label = 1065948; break;
				case PotionEffect.PesticideMedium: 			label = 1065949; break;
				case PotionEffect.PesticideHeavy: 			label = 1065950; break;
				
				case PotionEffect.FungicideLight: 			label = 1065951; break;
				case PotionEffect.FungicideMedium: 			label = 1065952; break;
				case PotionEffect.FungicideHeavy: 			label = 1065953; break;
				
				case PotionEffect.FertilizerLight: 			label = 1065954; break;
				case PotionEffect.FertilizerMedium: 		label = 1065955; break;
				case PotionEffect.FertilizerHeavy: 			label = 1065956; break;
					
				default: 									label = 0;		 break;
		    }

            // A keg of ~1_TYPE~ potions
            if( label != 0 )
                return string.Format( "A keg of {0} potions", StringList.Localization[ label ] );
            else
                return StringList.Localization[ 1041620 + (int)m_Type ];
        }
        #endregion

        public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			int number;

			if ( m_Held <= 0 )
				number = 502246; // The keg is empty.
			else if ( m_Held < 5 )
				number = 502248; // The keg is nearly empty.
			else if ( m_Held < 20 )
				number = 502249; // The keg is not very full.
			else if ( m_Held < 30 )
				number = 502250; // The keg is about one quarter full.
			else if ( m_Held < 40 )
				number = 502251; // The keg is about one third full.
			else if ( m_Held < 47 )
				number = 502252; // The keg is almost half full.
			else if ( m_Held < 54 )
				number = 502254; // The keg is approximately half full.
			else if ( m_Held < 70 )
				number = 502253; // The keg is more than half full.
			else if ( m_Held < 80 )
				number = 502255; // The keg is about three quarters full.
			else if ( m_Held < 96 )
				number = 502256; // The keg is very full.
			else if ( m_Held < 100 )
				number = 502257; // The liquid is almost to the top of the keg.
			else
				number = 502258; // The keg is completely full.

			list.Add( number );
		}

		public override void OnSingleClick( Mobile from )
		{
            // mod by Dies Irae
            // http://update.uo.com/design_42.html
            // When empty, single clicking on a potion keg will 
            // return the message "a specially lined keg for holding potions".
            // When filled, single clicking on a potion keg will return:
            // "A keg of (color) liquid" for non-owners and anyone else that has not used taste-id on the barrel.
            // "A keg of (potion type)" for the owner or those who have used the Taste Id skill on the keg (up to 5 at a time).

            if( m_Held <= 0 )
            {
                LabelTo( from, "a specially lined keg for holding potions" );
            }
            else if( IsIdentifiedFor( from  ) )
            {
                LabelTo( from, GetName() );
            }
		    else
		    {
		        LabelTo( from, GetUnidentifiedName() );
		    }

			int number;

			if ( m_Held <= 0 )
				number = 502246; // The keg is empty.
			else if ( m_Held < 5 )
				number = 502248; // The keg is nearly empty.
			else if ( m_Held < 20 )
				number = 502249; // The keg is not very full.
			else if ( m_Held < 30 )
				number = 502250; // The keg is about one quarter full.
			else if ( m_Held < 40 )
				number = 502251; // The keg is about one third full.
			else if ( m_Held < 47 )
				number = 502252; // The keg is almost half full.
			else if ( m_Held < 54 )
				number = 502254; // The keg is approximately half full.
			else if ( m_Held < 70 )
				number = 502253; // The keg is more than half full.
			else if ( m_Held < 80 )
				number = 502255; // The keg is about three quarters full.
			else if ( m_Held < 96 )
				number = 502256; // The keg is very full.
			else if ( m_Held < 100 )
				number = 502257; // The liquid is almost to the top of the keg.
			else
				number = 502258; // The keg is completely full.

			this.LabelTo( from, number );

			// base.OnSingleClick( from );
		}

		public override void OnDoubleClick( Mobile from )
		{
            // mod by Dies Irae
            // http://forums.uosecondage.com/viewtopic.php?f=7&t=1169&sid=df224cd1f1c4de80a5fff7dcd66ef498
            // You will no longer be able to use potion kegs or eat food though walls.
			if ( from.InRange( GetWorldLocation(), 2 ) && from.InLOS( this ) )
			{
				if ( m_Held > 0 )
				{
					Container pack = from.Backpack;

					if ( pack != null && pack.ConsumeTotal( typeof( Bottle ), 1 ) )
					{
						from.SendLocalizedMessage( 502242 ); // You pour some of the keg's contents into an empty bottle...

						BasePotion pot = FillBottle();

                        #region mod by Dies Irae
                        /*
                        The keg will fill one empty bottle in the user's backpack.
                        * If the type of potion in the keg is known to the user, 
                        * the new potion will already be identified.
                        * If the type of the potion in the keg is unknown to the user, 
                        * the new potion is identified by color only.
                        */
                        if( IsIdentifiedFor( from ) )
                            pot.AddIdentifier( from );
                        #endregion

                        if ( pack.TryDropItem( from, pot, false ) )
						{
							from.SendLocalizedMessage( 502243 ); // ...and place it into your backpack.
							from.PlaySound( 0x240 );

							if ( --Held == 0 )
								from.SendLocalizedMessage( 502245 ); // The keg is now empty.

						    #region mod by Dies Irae
                            if( Held == 0 )
                                m_Owner = null;
						    #endregion
						}
						else
						{
							from.SendLocalizedMessage( 502244 ); // ...but there is no room for the bottle in your backpack.
							pot.Delete();
						}
					}
					else
					{
						from.SendLocalizedMessage( 1044558 ); // You don't have any empty bottles.
					}
				}
				else
				{
					from.SendLocalizedMessage( 502246 ); // The keg is empty.
				}
			}
			else
			{
				from.LocalOverheadMessage( Network.MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
			}
		}

		public override bool OnDragDrop( Mobile from, Item item )
		{
			if ( item is BasePotion )
			{
			    #region mod by Dies Irae
			    if( m_Held > 0 )
			    {
			        if( m_Owner == null )
			            m_Owner = from;

			        if( !IsIdentifiedFor( from ) )
			        {
			            from.SendMessage( "In order to place a potion in a potion keg you must be the owner," +
                           "or have identified the keg with the Taste Id skill" );
			            return false;
			        }
			    }
			    #endregion
                
				BasePotion pot = (BasePotion)item;
                int toHold = Math.Min( 100 - m_Held, pot.Amount );

                
				if ( toHold <= 0 )
				{
					from.SendLocalizedMessage( 502233 ); // The keg will not hold any more!
					return false;
				}
				else if ( m_Held == 0 )
				{
					/*
					#region Mondain's Legacy
					if ( (int) pot.PotionEffect >= (int) PotionEffect.Invisibility )
					{
						from.SendLocalizedMessage( 502232 ); // The keg is not designed to hold that type of object.
						return false;
					}
					#endregion
					*/
					
					if ( GiveBottle( from, toHold ) )
					{
					    #region mod by Dies Irae
					    // Placing a potion in an empty potion keg will make you the owner. 
					    from.SendMessage( "You are now the owner of this keg." );
					    m_Owner = from;
					    #endregion

						m_Type = pot.PotionEffect;
						Held = toHold;

						from.PlaySound( 0x240 );

						from.SendLocalizedMessage( 502237 ); // You place the empty bottle in your backpack.

                        item.Consume( toHold );

						if( !item.Deleted )
							item.Bounce( from );

						return true;
					}
					else
					{
						from.SendLocalizedMessage( 502238 ); // You don't have room for the empty bottle in your backpack.
						return false;
					}
				}
				else if ( pot.PotionEffect != m_Type )
				{
					from.SendLocalizedMessage( 502236 ); // You decide that it would be a bad idea to mix different types of potions.
					return false;
				}
				else
				{
					if ( GiveBottle( from, toHold ) )
					{
						Held += toHold;

						from.PlaySound( 0x240 );

						from.SendLocalizedMessage( 502237 ); // You place the empty bottle in your backpack.

						item.Consume( toHold );

						if( !item.Deleted )
							item.Bounce( from );

						return true;
					}
					else
					{
						from.SendLocalizedMessage( 502238 ); // You don't have room for the empty bottle in your backpack.
						return false;
					}
				}
			}
			else
			{
				from.SendLocalizedMessage( 502232 ); // The keg is not designed to hold that type of object.
				return false;
			}
		}

		public bool GiveBottle( Mobile m )
		{
			#region Modifica by Dies Irae per le pozioni Stackable
			return GiveBottle( m, 1 );
			#endregion
		}

		public bool GiveBottle( Mobile m, int amount )
		{
			Container pack = m.Backpack;

			Bottle bottle = new Bottle( amount );

			if ( pack == null || !pack.TryDropItem( m, bottle, false ) )
			{
				bottle.Delete();
				return false;
			}

			return true;
		}

		public BasePotion FillBottle()
		{
			switch ( m_Type )
			{
				default:
				case PotionEffect.Nightsight:			return new NightSightPotion();

				case PotionEffect.CureLesser:			return new LesserCurePotion();
				case PotionEffect.Cure:				return new CurePotion();
				case PotionEffect.CureGreater:			return new GreaterCurePotion();

				case PotionEffect.Agility:			return new AgilityPotion();
				case PotionEffect.AgilityGreater:		return new GreaterAgilityPotion();

				case PotionEffect.Strength:			return new StrengthPotion();
				case PotionEffect.StrengthGreater:		return new GreaterStrengthPotion();

				case PotionEffect.PoisonLesser:			return new LesserPoisonPotion();
				case PotionEffect.Poison:			return new PoisonPotion();
				case PotionEffect.PoisonGreater:		return new GreaterPoisonPotion();
				case PotionEffect.PoisonDeadly:			return new DeadlyPoisonPotion();

				case PotionEffect.Refresh:			return new RefreshPotion();
				case PotionEffect.RefreshTotal:			return new TotalRefreshPotion();

				case PotionEffect.HealLesser:			return new LesserHealPotion();
				case PotionEffect.Heal:				return new HealPotion();
				case PotionEffect.HealGreater:			return new GreaterHealPotion();

				case PotionEffect.ExplosionLesser:		return new LesserExplosionPotion();
				case PotionEffect.Explosion:			return new ExplosionPotion();
				case PotionEffect.ExplosionGreater:		return new GreaterExplosionPotion();

				case PotionEffect.Invisibility: 			return new InvisibilityPotion();
				
				case PotionEffect.IntelligenceLesser: 		return new PhandelsFineIntellectPotion();
				case PotionEffect.Intelligence: 			return new PhandelsFantasticIntellectPotion();
				case PotionEffect.IntelligenceGreater: 		return new PhandelsFabulousIntellectPotion();
				
				case PotionEffect.ResistancesLesser: 		return new MegoInvulnerabilityPotionLesser();
				case PotionEffect.Resistances: 				return new MegoInvulnerabilityPotion();
				case PotionEffect.ResistancesGreater: 		return new MegoInvulnerabilityPotionGreater();
				
				case PotionEffect.Bless: 					return new HomericMightPotion();
				case PotionEffect.BlessGreater: 			return new HomericMightPotionGreater();
				
				case PotionEffect.ManaRefreshLesser: 		return new GrandMageRefreshElixirLesser();
				case PotionEffect.ManaRefresh: 				return new GrandMageRefreshElixir();
				case PotionEffect.ManaRefreshGreater: 		return new GrandMageRefreshElixirGreater();
				
				case PotionEffect.Tamla: 					return new TamlaPotion();
				
				case PotionEffect.TrasmutationLesser: 		return new TaintsMinorTransmutationPotion();
				case PotionEffect.TrasmutationGreater: 		return new TaintsMajorTransmutationPotion();
				
				case PotionEffect.Totem: 					return new Totem();
				case PotionEffect.Elixir: 					return new Elixir();
				
				case PotionEffect.FlashBang: 				return new FlashBangPotion();
				
				case PotionEffect.ColdResistanceLesser: 	return new ColdResistancePotionLesser();
				case PotionEffect.ColdResistance: 			return new ColdResistancePotion();
				case PotionEffect.ColdResistanceGreater: 	return new ColdResistancePotionGreater();
				case PotionEffect.EnergyResistanceLesser: 	return new EnergyResistancePotionLesser();
				case PotionEffect.EnergyResistance: 		return new EnergyResistancePotion();
				case PotionEffect.EnergyResistanceGreater: 	return new EnergyResistancePotionGreater();
				case PotionEffect.FireResistanceLesser: 	return new FireResistancePotionLesser();
				case PotionEffect.FireResistance: 			return new FireResistancePotion();
				case PotionEffect.FireResistanceGreater: 	return new FireResistancePotionGreater();
				case PotionEffect.PoisonResistanceLesser: 	return new PoisonResistancePotionLesser();
				case PotionEffect.PoisonResistance: 		return new PoisonResistancePotion();
				case PotionEffect.PoisonResistanceGreater: 	return new PoisonResistancePotionGreater();
				
				case PotionEffect.PetResurrection: 			return new PetResurrectionPotion();
				case PotionEffect.PetHeal: 					return new PetHealPotion();
				case PotionEffect.PetHealGreater: 			return new PetGreaterHealPotion();
				case PotionEffect.PetCure: 					return new PetCurePotion();
				case PotionEffect.PetCureGreater: 			return new PetGreaterCurePotion();
				case PotionEffect.PetShrink: 				return new PetShrinkPotion();
				
				case PotionEffect.NarcoticLight: 			return new LightNarcoticPotion();
				case PotionEffect.NarcoticRegular: 			return new RegularNarcoticPotion();
				case PotionEffect.NarcoticMedium: 			return new MediumNarcoticPotion();
				case PotionEffect.NarcoticHeavy: 			return new HeavyNarcoticPotion();
				
				case PotionEffect.PesticideLight: 			return new LightPesticidePotion();
				case PotionEffect.PesticideMedium: 			return new MediumPesticidePotion();
				case PotionEffect.PesticideHeavy: 			return new HeavyPesticidePotion();
				
				case PotionEffect.FungicideLight: 			return new LightFungicidePotion();
				case PotionEffect.FungicideMedium: 			return new MediumFungicidePotion();
				case PotionEffect.FungicideHeavy: 			return new HeavyFungicidePotion();
				
				case PotionEffect.FertilizerLight: 			return new LightFertilizerPotion();
				case PotionEffect.FertilizerMedium: 		return new MediumFertilizerPotion();
				case PotionEffect.FertilizerHeavy: 			return new HeavyFertilizerPotion();
				#region Mondain's Legacy
				case PotionEffect.Conflagration:			return new ConflagrationPotion();
				case PotionEffect.ConflagrationGreater:		return new GreaterConflagrationPotion();
				//case PotionEffect.MaskOfDeath:			return new MaskOfDeathPotion();
				//case PotionEffect.MaskOfDeathGreater:		return new MaskOfDeathGreaterPotion();
				case PotionEffect.ConfusionBlast:			return new ConfusionBlastPotion();
				case PotionEffect.ConfusionBlastGreater:	return new GreaterConfusionBlastPotion();
				#endregion
			}
		}

		public static void Initialize()
		{
			TileData.ItemTable[0x1940].Height = 4;
		}
	}
}