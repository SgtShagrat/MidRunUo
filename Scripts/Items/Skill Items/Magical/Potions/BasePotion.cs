using System;

using Midgard.Engines.Races;

using Server;
using Server.Engines.Craft;
using System.Collections.Generic;

namespace Server.Items
{
	public enum PotionEffect
	{
		Nightsight,
		CureLesser,
		Cure,
		CureGreater,
		Agility,
		AgilityGreater,
		Strength,
		StrengthGreater,
		PoisonLesser,
		Poison,
		PoisonGreater,
		PoisonDeadly,
		Refresh,
		RefreshTotal,
		HealLesser,
		Heal,
		HealGreater,
		ExplosionLesser,
		Explosion,
		ExplosionGreater,
		
		#region Mondain's Legacy
		Conflagration,
		ConflagrationGreater,
		MaskOfDeath,
		MaskOfDeathGreater,
		ConfusionBlast,
		ConfusionBlastGreater,		
//		Invisibility,
		Parasitic,
		Darkglow,
		#endregion

		#region PaganPotions
		Invisibility = 100,

		IntelligenceLesser,
		Intelligence,
		IntelligenceGreater,

		ResistancesLesser,
		Resistances,
		ResistancesGreater,

		Bless,
		BlessGreater,

		ManaRefreshLesser,
		ManaRefresh,
		ManaRefreshGreater,

		Tamla,

		TrasmutationLesser,
		TrasmutationGreater,

		Totem,
		Elixir,

		FlashBang,
		#endregion

		#region ElementalResistance
		ColdResistanceLesser = 150,
		ColdResistance,
		ColdResistanceGreater,

		EnergyResistanceLesser,
		EnergyResistance,
		EnergyResistanceGreater,

		FireResistanceLesser,
		FireResistance,
		FireResistanceGreater,

		PoisonResistanceLesser,
		PoisonResistance,
		PoisonResistanceGreater,
		#endregion

		#region PetHealth
		PetResurrection = 165,
		PetHeal,
		PetHealGreater,
		PetCure,
		PetCureGreater,
		PetShrink,
		#endregion

		#region petdyes
		PetDyeBlack = 200,
		#endregion

		#region narcotics
		NarcoticLight = 300,
		NarcoticRegular,
		NarcoticMedium,
		NarcoticHeavy,
		#endregion

		#region plant care
		PesticideLight = 400,
		PesticideMedium,
		PesticideHeavy,
		FungicideLight,
		FungicideMedium,
		FungicideHeavy,
		FertilizerLight,
		FertilizerMedium,
		FertilizerHeavy,
		#endregion

		#region New Poisons
		BloccoPoisonLesser = 410,
		BloccoPoison,
		BloccoPoisonGreater,
		BloccoPoisonDeadly,
		LentezzaPoisonLesser,
		LentezzaPoison,
		LentezzaPoisonGreater,
		LentezzaPoisonDeadly,
		MagiaPoisonLesser,
		MagiaPoison,
		MagiaPoisonGreater,
		MagiaPoisonDeadly,
		ParalisiPoisonLesser,
		ParalisiPoison,
		ParalisiPoisonGreater,
		ParalisiPoisonDeadly,
		StanchezzaPoisonLesser,
		StanchezzaPoison,
		StanchezzaPoisonGreater,
		StanchezzaPoisonDeadly
		#endregion
	}

	public abstract class BasePotion : Item, ICraftable, ITasteIdentificable
	{
		private PotionEffect m_PotionEffect;

        [CommandProperty( AccessLevel.GameMaster )]
		public PotionEffect PotionEffect
		{
			get
			{
				return m_PotionEffect;
			}
			set
			{
				m_PotionEffect = value;
				InvalidateProperties();
			}
		}

        [CommandProperty( AccessLevel.GameMaster )]
		public override int LabelNumber{ get{ return 1041314 + (int)m_PotionEffect; } }

		public BasePotion( int itemID, PotionEffect effect, int amount ) : base( itemID )
		{
			m_PotionEffect = effect;

			#region Modifica Stackable
			Stackable = true;
			Amount = amount;
			#endregion

			Weight = 1.0;

            m_IdentifiersList = new List<Mobile>(); // mod by Dies Irae
		}
		
		public BasePotion( int itemID, PotionEffect effect ) : this( itemID, effect, 1)
		{
		}

		public BasePotion( Serial serial ) : base( serial )
		{
		}

		public virtual bool RequireFreeHand{ get{ return true; } }

		public static bool HasFreeHand( Mobile m )
		{
			Item handOne = m.FindItemOnLayer( Layer.OneHanded );
			Item handTwo = m.FindItemOnLayer( Layer.TwoHanded );

			if ( handTwo is BaseWeapon )
				handOne = handTwo;
			
			if ( handOne is BaseRanged )
			{
				BaseRanged ranged = (BaseRanged) handOne;

				if ( ranged.Balanced )
					return true;
			}

			return ( handOne == null || handTwo == null );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !Movable )
				return;

            #region mod by Dies Irae
            if( !Morph.CheckItemAllowed( this, from, true ) )
                return;

            if( !IsAccessibleTo( from ) )
                return;
            #endregion

			if ( from.InRange( this.GetWorldLocation(), 1 ) )
			{
				if ( !RequireFreeHand || HasFreeHand( from ) )
				{
                    if( CanDrink( from, true ) )
                        Drink( from );
                }
				else
					from.SendLocalizedMessage( 502172 ); // You must have a free hand to drink a potion.
			}
			else
			{
				from.SendLocalizedMessage( 502138 ); // That is too far away for you to use
			}
		}

        #region mod by Dies Irae : pre-aos stuff
        public virtual int DelayUse
        {
            get { return 15; }
        }

        public virtual int BonusOnDelayAtHundred
        {
            get { return 6; }
        }

        public virtual bool CanDrink( Mobile from, bool message )
        {
            bool canDrink = from.CanBeginAction( typeof( BasePotion ) );

            if( !canDrink && message )
                from.SendMessage( from.Language == "ITA" ? "Devi aspettare prima di poter bere un'altra pozione!" : "You must wait until you can drink another potion!" );

            return canDrink;
        }

        public virtual void LockBasePotionUse( Mobile from )
        {
            from.BeginAction( typeof( BasePotion ) );
            Timer.DelayCall( GetDelayOfReuse( from ), new TimerStateCallback( ReleaseBasePotionLock ), from );
        }

        public virtual TimeSpan GetDelayOfReuse( Mobile from )
        {
            int offset = (int)( ( from.Skills[ SkillName.Alchemy ].Value / 100.0 ) * BonusOnDelayAtHundred );

            return DelayUse - offset > 0 ? TimeSpan.FromSeconds( DelayUse - offset ) : TimeSpan.Zero;
        }

	    private static void ReleaseBasePotionLock( object state )
        {
            ( (Mobile)state ).EndAction( typeof( BasePotion ) );
        }

        public override void OnSingleClick( Mobile from )
        {
            LabelTo( from, StringUtility.ConvertItemName( ( IsIdentifiedFor( from ) ? GetName(from.Language) : GetUnidentifiedName( from.Language ) ), Amount, true, from.Language ) );
        }

        public static bool IsStandardEffect( PotionEffect effect )
        {
            return effect >= PotionEffect.Nightsight && effect <= PotionEffect.ExplosionGreater;
        }

        public string GetUnidentifiedName(string language)
        {
            int label = 0;

			switch( m_PotionEffect )
			{
                case PotionEffect.Nightsight: 		label = 1041306; break; // an unknown black potion
	            case PotionEffect.CureLesser:
	            case PotionEffect.Cure:
	            case PotionEffect.CureGreater: 		label = 1041307; break; // an unknown orange potion.
	            case PotionEffect.Agility:
	            case PotionEffect.AgilityGreater: 	label = 1041308; break; // an unknown blue potion
	            case PotionEffect.Strength:
	            case PotionEffect.StrengthGreater: 	label = 1041309; break; // an unknown white potion
	            case PotionEffect.PoisonLesser:
	            case PotionEffect.Poison:
	            case PotionEffect.PoisonGreater:
	            case PotionEffect.PoisonDeadly: 	label = 1041310; break; // an unknown green potion
	            case PotionEffect.Refresh:
	            case PotionEffect.RefreshTotal: 	label = 1041311; break; // an unknown red potion
	            case PotionEffect.HealLesser:
	            case PotionEffect.Heal:
	            case PotionEffect.HealGreater: 		label = 1041312; break; // an unknown yellow potion
	            case PotionEffect.ExplosionLesser:
	            case PotionEffect.Explosion:
	            case PotionEffect.ExplosionGreater: label = 1041313; break; // an unknown potion

                default: label = 1041305; break; // an unknown potion
            }

            return language == "ITA" ? StringList.LocalizationIta[ label ] : StringList.Localization[ label ];
        }

        public string GetName(string language)
        {
			int label = 0;

			switch( m_PotionEffect )
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

				case PotionEffect.BloccoPoisonLesser:		return language == "ITA" ? "pozione di blocco minore" : "lesser block potion";
				case PotionEffect.BloccoPoison:			return language == "ITA" ? "pozione di blocco" : "block potion";
				case PotionEffect.BloccoPoisonGreater:		return language == "ITA" ? "pozione di blocco maggiore" : "greater block potion";
				case PotionEffect.BloccoPoisonDeadly:		return language == "ITA" ? "pozione di blocco estrema" : "deadly block potion";
				case PotionEffect.LentezzaPoisonLesser:		return language == "ITA" ? "pozione di lentezza minore" : "lesser slow potion";
				case PotionEffect.LentezzaPoison:		return language == "ITA" ? "pozione di lentezza" : "slow potion";
				case PotionEffect.LentezzaPoisonGreater:	return language == "ITA" ? "pozione di lentezza maggiore" : "greater slow potion";
				case PotionEffect.LentezzaPoisonDeadly:		return language == "ITA" ? "pozione di lentezza estrema" : "deadly slow potion";
				case PotionEffect.MagiaPoisonLesser:		return language == "ITA" ? "pozione di magia minore" : "lesser magic potion";
				case PotionEffect.MagiaPoison:			return language == "ITA" ? "pozione di magia" : "magic potion";
				case PotionEffect.MagiaPoisonGreater:		return language == "ITA" ? "pozione di magia maggiore" : "greater magic potion";
				case PotionEffect.MagiaPoisonDeadly:		return language == "ITA" ? "pozione di magia estrema" : "deadly magic potion";
				case PotionEffect.ParalisiPoisonLesser:		return language == "ITA" ? "pozione di paralisi minore" : "lesser paralysis potion";
				case PotionEffect.ParalisiPoison:		return language == "ITA" ? "pozione di paralisi" : "paralysis potion" ;
				case PotionEffect.ParalisiPoisonGreater:	return language == "ITA" ? "pozione di paralisi maggiore" : "greater paralysis potion";
				case PotionEffect.ParalisiPoisonDeadly:		return language == "ITA" ? "pozione di paralisi estrema" : "deadly paralysis potion";
				case PotionEffect.StanchezzaPoisonLesser:	return language == "ITA" ? "pozione di stanchezza minore" : "lesser fatigue potion";
				case PotionEffect.StanchezzaPoison:		return language == "ITA" ? "pozione di stanchezza" : "fatigue potion";
				case PotionEffect.StanchezzaPoisonGreater:	return language == "ITA" ? "pozione di stanchezza maggiore" : "greater fatigue potion";
				case PotionEffect.StanchezzaPoisonDeadly:	return language == "ITA" ? "pozione di stanchezza estrema" : "deadly fatigue potion";
				default: 									label = 0;		 break;
		    }
		string plur = Amount > 1 ? (language == "ITA" ? "i" : "s") : (language == "ITA" ? "e" : "") ;

            if( label != 0 )
                return string.Format( (language == "ITA" ? "pozion"+plur+" {0}" : "{0} potion"+plur), StringList.GetClilocString( null, label, language ) );
            else
                return language == "ITA" ? StringList.LocalizationIta[ LabelNumber ] : StringList.Localization[ LabelNumber ];
        }

        public override void OnAfterDuped( Item newitem )
        {
            if( newitem is ITasteIdentificable )
                CopyIdentifiersTo( (ITasteIdentificable)newitem );

            base.OnAfterDuped( newitem );
        }

        #region IIdentificable members
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

            Utility.Log( "BasePotionSpamIdentifiers.log", "IdentifiersList for potion {0}", Serial.ToString() );
            foreach( Mobile m in m_IdentifiersList )
                Utility.Log( "BasePotionSpamIdentifiers.log", m.Name );
        }

		public string AlreadyIdentifiedMessage
		{
			get { return "You already know what kind of potion that is."; }
		}
		public string AlreadyIdentifiedMessageIta
		{
			get { return "Conosci già il liquido contenuto."; }
		}

	    public bool IsIdentifiedFor( Mobile from )
        {
            if( IsStandardEffect( m_PotionEffect ) )
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
        #endregion
        #endregion

		#region serialization
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 3 ); // version

            // version 3
            writer.Write( m_IdentifiersList != null );
            if ( m_IdentifiersList != null )
                writer.Write( m_IdentifiersList, true );

			writer.Write( (int) m_PotionEffect );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
                case 3:
					if ( reader.ReadBool() )
					{
                        if( m_IdentifiersList == null )
                            m_IdentifiersList = new List<Mobile>();

					    m_IdentifiersList = reader.ReadStrongMobileList();
					}
                    goto case 2;
                case 2:
                    if( version < 3 )
			            reader.ReadBool();
			        goto case 1;
				case 1:
				case 0:
				{
					m_PotionEffect = (PotionEffect)reader.ReadInt();
					break;
				}
			}

			if( version ==  0 )
				Stackable = Core.ML;
		}
		#endregion

		public abstract void Drink( Mobile from );

		public static void PlayDrinkEffect( Mobile m )
		{
			m.RevealingAction();

			m.PlaySound( 0x2D6 );

			m.AddToBackpack( new Bottle() );

			if ( m.Body.IsHuman && !m.Mounted )
				m.Animate( 34, 5, 1, true, false, 0 );

            #region mod by Dies Irae
            if( m.Target != null )
                Targeting.Target.Cancel( m );
            #endregion
        }

		public static int EnhancePotions( Mobile m )
		{
			int EP = AosAttributes.GetValue( m, AosAttribute.EnhancePotions );
			if ( Core.ML && EP > 50 )
				EP = 50;
			return EP;
		}

		public static TimeSpan Scale( Mobile m, TimeSpan v )
		{
			if ( !Core.AOS )
				return v;

			double scalar = 1.0 + ( 0.01 * EnhancePotions( m ) );

			return TimeSpan.FromSeconds( v.TotalSeconds * scalar );
		}

		public static double Scale( Mobile m, double v )
		{
			if ( !Core.AOS )
				return v;

			double scalar = 1.0 + ( 0.01 * EnhancePotions( m ) );

			return v * scalar;
		}

		public static int Scale( Mobile m, int v )
		{
			if ( !Core.AOS )
				return v;

			return AOS.Scale( v, 100 + EnhancePotions( m ) );
		}

        #region mod by Dies Irae : Publish 46
        public static double GetAlchemyBonusScalar( Mobile from )
        {
            double alchemy = from.Skills[ SkillName.Alchemy ].Value;
            if( alchemy < 33.4 )
                return 0.0;
            else if( alchemy < 66.7 )
                return 0.1;
            else if( alchemy < 99.9 )
                return 0.2;
            else
                return 0.3;
        }
        #endregion

		public override bool StackWith( Mobile from, Item dropped, bool playSound )
		{
            #region mod by Dies Irae
            if( dropped is BasePotion && !( (BasePotion)dropped ).IsIdentifiedFor( from ) )
                return false;

            if( dropped is BasePotion && ( (BasePotion)dropped ).IsIdentifiedFor( from ) && !IsIdentifiedFor( from ) )
                return false;
            #endregion

			if( dropped is BasePotion && ((BasePotion)dropped).m_PotionEffect == m_PotionEffect )
				return base.StackWith( from, dropped, playSound );

			return false;
		}

		#region ICraftable Members

		public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
            AddIdentifier( from ); // mod by Dies Irae

			if ( craftSystem is DefAlchemy )
			{
				Container pack = from.Backpack;

				if ( pack != null )
				{
					List<PotionKeg> kegs = pack.FindItemsByType<PotionKeg>();

					for ( int i = 0; i < kegs.Count; ++i )
					{
						PotionKeg keg = kegs[i];

						if ( keg == null )
							continue;

						if ( keg.Held <= 0 || keg.Held >= 100 )
							continue;

						if ( keg.Type != PotionEffect )
							continue;

						++keg.Held;

						Consume();
						from.AddToBackpack( new Bottle() );

						return -1; // signal placed in keg
					}
				}
			}

            if( makersMark )
                Crafter = from;

            PlayerConstructed = true;
            CrafterSkill = from.Skills[ craftSystem.MainSkill ].Value;

			return 1;
		}

	    [CommandProperty( AccessLevel.GameMaster )]
        public bool PlayerConstructed { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Crafter { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public double CrafterSkill { get; set; }

	    #endregion
	}
}