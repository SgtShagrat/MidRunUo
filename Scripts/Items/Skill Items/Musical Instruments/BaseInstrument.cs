// #define FixNames

using System;
using System.Collections;

using Midgard.Engines.Classes;
using Midgard.Engines.MidgardTownSystem;
using Midgard.Items;
using Midgard.Misc;
using Server.Engines.Craft;
using Server.Guilds;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using System.Text;
using System.Collections.Generic;

namespace Server.Items
{
	public delegate void InstrumentPickedCallback( Mobile from, BaseInstrument instrument );

	public enum InstrumentQuality
	{
		Low,
		Regular,
		Exceptional
	}

	public abstract class BaseInstrument : Item, ICraftable, ISlayer, IIdentificable, IResourceItem
	{
		private int m_WellSound, m_BadlySound;
		private SlayerName m_Slayer, m_Slayer2;
		private InstrumentQuality m_Quality;
		private Mobile m_Crafter;
		private int m_UsesRemaining;

		[CommandProperty( AccessLevel.GameMaster )]
		public int SuccessSound
		{
			get{ return m_WellSound; }
			set{ m_WellSound = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int FailureSound
		{
			get{ return m_BadlySound; }
			set{ m_BadlySound = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public SlayerName Slayer
		{
			get{ return m_Slayer; }
			set{ m_Slayer = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public SlayerName Slayer2
		{
			get{ return m_Slayer2; }
			set{ m_Slayer2 = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public InstrumentQuality Quality
		{
			get
			{
                #region mod by Dies Irae
                if( CustomQuality >= Server.Quality.Exceptional )
                    return InstrumentQuality.Exceptional;
                else if( CustomQuality <= Server.Quality.Low )
                    return InstrumentQuality.Low;
                #endregion
                
                return m_Quality;
			}
			set{ UnscaleUses(); m_Quality = value; InvalidateProperties(); ScaleUses(); }
		}

        /*
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Crafter
		{
			get{ return m_Crafter; }
			set{ m_Crafter = value; InvalidateProperties(); }
		}
        */

		public virtual int InitMinUses{ get{ return 350; } }
		public virtual int InitMaxUses{ get{ return 450; } }

		public virtual TimeSpan ChargeReplenishRate { get { return TimeSpan.FromMinutes( 5.0 ); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int UsesRemaining
		{
			get{ CheckReplenishUses(); return m_UsesRemaining; }
			set{ m_UsesRemaining = value; InvalidateProperties(); }
		}

		private DateTime m_LastReplenished;

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime LastReplenished
		{
			get { return m_LastReplenished; }
			set { m_LastReplenished = value; CheckReplenishUses(); }
		}

		private bool m_ReplenishesCharges;
		[CommandProperty( AccessLevel.GameMaster )]
		public bool ReplenishesCharges
		{
			get { return m_ReplenishesCharges; }
			set 
			{
				if( value != m_ReplenishesCharges && value )
					m_LastReplenished = DateTime.Now;

				m_ReplenishesCharges = value; 
			}
		}

		public void CheckReplenishUses()
		{
			CheckReplenishUses( true );
		}

		public void CheckReplenishUses( bool invalidate )
		{
			if( !m_ReplenishesCharges || m_UsesRemaining >= InitMaxUses )
				return;

			if( m_LastReplenished + ChargeReplenishRate < DateTime.Now )
			{
				TimeSpan timeDifference = DateTime.Now - m_LastReplenished;

				m_UsesRemaining = Math.Min( m_UsesRemaining + (int)( timeDifference.Ticks / ChargeReplenishRate.Ticks), InitMaxUses );	//How rude of TimeSpan to not allow timespan division.
				m_LastReplenished = DateTime.Now;

				if( invalidate )
					InvalidateProperties();

			}
		}

		public void ScaleUses()
		{
			UsesRemaining = (UsesRemaining * GetUsesScalar()) / 100;
			//InvalidateProperties();
		}

		public void UnscaleUses()
		{
			UsesRemaining = (UsesRemaining * 100) / GetUsesScalar();
		}

		public int GetUsesScalar()
		{
			if ( m_Quality == InstrumentQuality.Exceptional )
				return 200;

			return 100;
		}

		public void ConsumeUse( Mobile from )
		{
			// TODO: Confirm what must happen here?

			if ( UsesRemaining > 1 )
			{
				--UsesRemaining;
			}
			else
			{
				if ( from != null )
					from.SendLocalizedMessage( 502079 ); // The instrument played its last tune.

				Delete();
			}
		}

		private static Hashtable m_Instruments = new Hashtable();

		public static BaseInstrument GetInstrument( Mobile from )
		{
			BaseInstrument item = m_Instruments[from] as BaseInstrument;

			if ( item == null )
				return null;

			if ( !item.IsChildOf( from.Backpack ) )
			{
				m_Instruments.Remove( from );
				return null;
			}

			return item;
		}

		public static int GetBardRange( Mobile bard, SkillName skill )
		{
			return 8 + (int)(bard.Skills[skill].Value / 15);
		}

		public static void PickInstrument( Mobile from, InstrumentPickedCallback callback )
		{
			BaseInstrument instrument = GetInstrument( from );

			if ( instrument != null )
			{
				if ( callback != null )
					callback( from, instrument );
			}
			else
			{
				from.SendLocalizedMessage( 500617 ); // What instrument shall you play?
				from.BeginTarget( 1, false, TargetFlags.None, new TargetStateCallback( OnPickedInstrument ), callback );
			}
		}

		public static void OnPickedInstrument( Mobile from, object targeted, object state )
		{
			BaseInstrument instrument = targeted as BaseInstrument;

			if ( instrument == null )
			{
				from.SendLocalizedMessage( 500619 ); // That is not a musical instrument.
			}
			else
			{
				SetInstrument( from, instrument );

				InstrumentPickedCallback callback = state as InstrumentPickedCallback;

				if ( callback != null )
					callback( from, instrument );
			}
		}

		public static bool IsMageryCreature( BaseCreature bc )
		{
			return ( bc != null && bc.AI == AIType.AI_Mage && bc.Skills[SkillName.Magery].Base > 5.0 );
		}

		public static bool IsFireBreathingCreature( BaseCreature bc )
		{
			if ( bc == null )
				return false;

			return bc.HasBreath;
		}

		public static bool IsPoisonImmune( BaseCreature bc )
		{
			return ( bc != null && bc.PoisonImmune != null );
		}

		public static int GetPoisonLevel( BaseCreature bc )
		{
			if ( bc == null )
				return 0;

			Poison p = bc.HitPoison;

			if ( p == null )
				return 0;

			#region Mondain's Legacy mod
			return p.RealLevel + 1;
			#endregion
		}

		public static double GetBaseDifficulty( Mobile targ )
		{
			/* Difficulty TODO: Add another 100 points for each of the following abilities:
				- Radiation or Aura Damage (Heat, Cold etc.)
				- Summoning Undead
			*/

			double val = (targ.HitsMax * 1.6) + targ.StamMax + targ.ManaMax;

			val += targ.SkillsTotal / 10;

			if ( val > 700 )
				val = 700 + (int)((val - 700) * (3.0 / 11));

			BaseCreature bc = targ as BaseCreature;

			if ( IsMageryCreature( bc ) )
				val += 100;

			if ( IsFireBreathingCreature( bc ) )
				val += 100;

			if ( IsPoisonImmune( bc ) )
				val += 100;

			if ( targ is VampireBat || targ is VampireBatFamiliar )
				val += 100;

			val += GetPoisonLevel( bc ) * 20;

			val /= 10;

			if ( bc != null && bc.IsParagon )
				val += 40.0;

			if ( Core.SE && val > 160.0 )
				val = 160.0;

			return val;
		}

		public double GetDifficultyFor( Mobile targ )
		{
			double val = GetBaseDifficulty( targ );

			if ( m_Quality == InstrumentQuality.Exceptional )
				val -= 5.0; // 10%

			if ( m_Slayer != SlayerName.None )
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName( m_Slayer );

				if ( entry != null )
				{
					if ( entry.Slays( targ ) )
						val -= 10.0; // 20%
					else if ( entry.Group.OppositionSuperSlays( targ ) )
						val += 10.0; // -20%
				}
			}

			if ( m_Slayer2 != SlayerName.None )
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName( m_Slayer2 );

				if ( entry != null )
				{
					if ( entry.Slays( targ ) )
						val -= 10.0; // 20%
					else if ( entry.Group.OppositionSuperSlays( targ ) )
						val += 10.0; // -20%
				}
			}

			return val;
		}

		public static void SetInstrument( Mobile from, BaseInstrument item )
		{
			m_Instruments[from] = item;
		}

		public BaseInstrument( int itemID, int wellSound, int badlySound ) : base( itemID )
		{
			m_WellSound = wellSound;
			m_BadlySound = badlySound;
			UsesRemaining = Utility.RandomMinMax( InitMinUses, InitMaxUses );

            m_IdentifiersList = new List<Mobile>(); // mod by Dies Irae
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			int oldUses = m_UsesRemaining;
			CheckReplenishUses( false );

			base.GetProperties( list );

			if ( m_Crafter != null )
				list.Add( 1050043, m_Crafter.Name ); // crafted by ~1_NAME~

			if ( m_Quality == InstrumentQuality.Exceptional )
				list.Add( 1060636 ); // exceptional

			list.Add( 1060584, m_UsesRemaining.ToString() ); // uses remaining: ~1_val~

			if( m_ReplenishesCharges )
				list.Add( 1070928 ); // Replenish Charges

			if( m_Slayer != SlayerName.None )
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName( m_Slayer );
				if( entry != null )
					list.Add( entry.Title );
			}

			if( m_Slayer2 != SlayerName.None )
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName( m_Slayer2 );
				if( entry != null )
					list.Add( entry.Title );
			}

			if( m_UsesRemaining != oldUses )
				Timer.DelayCall( TimeSpan.Zero, new TimerCallback( InvalidateProperties ) );
		}

		public override void OnSingleClick( Mobile from )
		{
            #region mod by Dies Irae
            OnOldSingleClick( from );
            return;
            #endregion

			ArrayList attrs = new ArrayList();

			if ( DisplayLootType )
			{
				if ( LootType == LootType.Blessed )
					attrs.Add( new EquipInfoAttribute( 1038021 ) ); // blessed
				else if ( LootType == LootType.Cursed )
					attrs.Add( new EquipInfoAttribute( 1049643 ) ); // cursed
			}

			if ( m_Quality == InstrumentQuality.Exceptional )
				attrs.Add( new EquipInfoAttribute( 1018305 - (int)m_Quality ) );

			if( m_ReplenishesCharges )
				attrs.Add( new EquipInfoAttribute( 1070928 ) ); // Replenish Charges

			// TODO: Must this support item identification?
			if( m_Slayer != SlayerName.None )
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName( m_Slayer );
				if( entry != null )
					attrs.Add( new EquipInfoAttribute( entry.Title ) );
			}

			if( m_Slayer2 != SlayerName.None )
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName( m_Slayer2 );
				if( entry != null )
					attrs.Add( new EquipInfoAttribute( entry.Title ) );
			}

			int number;

			if ( Name == null )
			{
				number = LabelNumber;
			}
			else
			{
				this.LabelTo( from, Name );
				number = 1041000;
			}

			if ( attrs.Count == 0 && Crafter == null && Name != null )
				return;

			EquipmentInfo eqInfo = new EquipmentInfo( number, m_Crafter, false, (EquipInfoAttribute[])attrs.ToArray( typeof( EquipInfoAttribute ) ) );

			from.Send( new DisplayEquipmentInfo( this, eqInfo ) );
		}

        public BaseInstrument( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 7 ); // version

            MidgardSerialize( writer ); // mod by Dies Irae

			writer.Write( m_ReplenishesCharges );
			if( m_ReplenishesCharges )
				writer.Write( m_LastReplenished );

			writer.Write( m_Crafter );

			writer.WriteEncodedInt( (int) m_Quality );
			writer.WriteEncodedInt( (int) m_Slayer );
			writer.WriteEncodedInt( (int) m_Slayer2 );

			writer.WriteEncodedInt( (int)UsesRemaining );

			writer.WriteEncodedInt( (int) m_WellSound );
			writer.WriteEncodedInt( (int) m_BadlySound );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				#region modifica by Dies Irae
                case 7:
			        goto case 6;
                case 6:
                {
                    MidgardDeserialize( reader, version ); // mod by Dies Irae
                    goto case 5;
                }
                case 5:
                {
                    if( version < 6 )
                        reader.ReadBool();

                    goto case 4;
                }
				case 4:
				{
                    if( version < 6 )
                    {
					    m_EngravedText = reader.ReadString();
					    m_Resource = (CraftResource) reader.ReadInt();
                    }
					goto case 3;
				}
				#endregion
				case 3:
				{
					m_ReplenishesCharges = reader.ReadBool();

					if( m_ReplenishesCharges )
						m_LastReplenished = reader.ReadDateTime();

					goto case 2;
				}
				case 2:
				{
					m_Crafter = reader.ReadMobile();

					m_Quality = (InstrumentQuality)reader.ReadEncodedInt();
					m_Slayer = (SlayerName)reader.ReadEncodedInt();
					m_Slayer2 = (SlayerName)reader.ReadEncodedInt();

					UsesRemaining = reader.ReadEncodedInt();

					m_WellSound = reader.ReadEncodedInt();
					m_BadlySound = reader.ReadEncodedInt();
					
					break;
				}
				case 1:
				{
					m_Crafter = reader.ReadMobile();

					m_Quality = (InstrumentQuality)reader.ReadEncodedInt();
					m_Slayer = (SlayerName)reader.ReadEncodedInt();

					UsesRemaining = reader.ReadEncodedInt();

					m_WellSound = reader.ReadEncodedInt();
					m_BadlySound = reader.ReadEncodedInt();

					break;
				}
				case 0:
				{
					m_WellSound = reader.ReadInt();
					m_BadlySound = reader.ReadInt();
					UsesRemaining = Utility.RandomMinMax( InitMinUses, InitMaxUses );

					break;
				}
			}

#if FixNames
			Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Midgard.CraftHelper.VerifyInstrumentName_Callback ), this );
#endif

			CheckReplenishUses();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !from.InRange( GetWorldLocation(), 1 ) )
			{
				from.SendLocalizedMessage( 500446 ); // That is too far away.
			}
			else if ( from.BeginAction( typeof( BaseInstrument ) ) )
			{
				SetInstrument( from, this );

				// Delay of 7 second before beign able to play another instrument again
				new InternalTimer( from ).Start();

				if ( CheckMusicianship( from ) )
					PlayInstrumentWell( from );
				else
					PlayInstrumentBadly( from );
			}
			else
			{
				from.SendLocalizedMessage( 500119 ); // You must wait to perform another action
			}
		}

		public static bool CheckMusicianship( Mobile m )
		{
			m.CheckSkill( SkillName.Musicianship, 0.0, Core.AOS ? 120.0 : 100.0 ); // mod by Dies Irae

			return ( (m.Skills[SkillName.Musicianship].Value / 100) > Utility.RandomDouble() );
		}

		public void PlayInstrumentWell( Mobile from )
		{
			from.PlaySound( m_WellSound );
		}

		public void PlayInstrumentBadly( Mobile from )
		{
			from.PlaySound( m_BadlySound );
		}

		private class InternalTimer : Timer
		{
			private Mobile m_From;

			public InternalTimer( Mobile from ) : base( TimeSpan.FromSeconds( 6.0 ) )
			{
				m_From = from;
				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
				m_From.EndAction( typeof( BaseInstrument ) );
			}
		}
		#region ICraftable Members
		public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
			Quality = (InstrumentQuality)quality;

			if ( makersMark )
				Crafter = from;

			#region modifica by Dies Irae
            PlayerConstructed = true;
            CrafterSkill = from.Skills[ craftSystem.MainSkill ].Value;

			Type resourceType = typeRes;

			if ( resourceType == null )
				resourceType = craftItem.Resources.GetAt( 0 ).ItemType;

			Resource = CraftResources.GetFromType( resourceType );

			CraftContext context = craftSystem.GetContext( from );

			if ( context != null && context.DoNotColor )
				Hue = 0;
			#endregion

			return quality;
		}

        [CommandProperty( AccessLevel.GameMaster )]
        public bool PlayerConstructed { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Crafter
        {
            get { return m_Crafter; }
            set { m_Crafter = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public double CrafterSkill { get; set; }
		#endregion

        #region mod by Dies Irae [Third Crown]
        private CraftResource m_Resource;

        [CommandProperty( AccessLevel.GameMaster )]
        public CraftResource Resource
        {
            get { return m_Resource; }
            set
            {
                if( m_Resource != value )
                {
                    m_Resource = value;
                    Hue = CraftResources.GetHue( m_Resource );

                    InvalidateSecondAgeNames();
                    InvalidateProperties();
                }
            }
        }

        private string m_EngravedText;

        [CommandProperty( AccessLevel.GameMaster )]
        public string EngravedText
        {
            get { return m_EngravedText; }
            set { m_EngravedText = value; InvalidateSecondAgeNames(); }
        }

        public virtual string OldInitHits { get { return "1d0+0"; } }

        public virtual TownSystem RequiredTownSystem { get { return null; } }

        public virtual Classes RequiredClass { get { return Classes.None; } }

        #region [guilds]
        [CommandProperty( AccessLevel.Administrator )]
        public bool IsGuildItem { get; set; }

        private BaseGuild m_OwnerGuild;

        [CommandProperty( AccessLevel.Administrator )]
        public BaseGuild OwnerGuild
        {
            get { return m_OwnerGuild; }
            set
            {
                m_OwnerGuild = value;

                LootType = ( m_OwnerGuild == null ? LootType.Regular : LootType.Blessed );
            }
        }

        [CommandProperty( AccessLevel.Administrator )]
        public Mobile GuildedOwner { get; set; }

	    public void VerifyGuildItem()
        {
            if( IsGuildItem && OwnerGuild != null && GuildedOwner == null && OwnerGuild is Guild )
            {
                GuildsHelper.StringToLog( string.Format( "Warning: Guilditem for {0} with serial {1} without owner... deleting.", OwnerGuild.Name, Serial ) );
                GuildsHelper.UnRegisterUniform( null, (Guild)OwnerGuild, this );
                Delete();
            }
        }

        public override void OnAfterDelete()
        {
            if( IsGuildItem && m_OwnerGuild != null && m_OwnerGuild is Guild )
                GuildsHelper.UnRegisterUniform( null, (Guild)m_OwnerGuild, this );

            base.OnAfterDelete();
        }
        #endregion

        private string m_SecondAgeFullName;
        private string m_SecondAgeUnIdentifiedName;

        public void InvalidateSecondAgeNames()
        {
            BuildSecondAgeNames( string.IsNullOrEmpty( Name ) ? StringList.Localization[ LabelNumber ] : Name, "ITA" );
        }

        private void BuildMagicalSecondAgeNames( string rawName, string language )
        {
            StringBuilder builder = new StringBuilder();

            // 'exceptional '
            builder.Append( Quality == InstrumentQuality.Exceptional ? "exceptional " : string.Empty );

            // 'oak '
            string material = MaterialName;
            if( material.Length > 0 )
                builder.AppendFormat( "{0} ", material );

            // 'tamburine'
            builder.Append( rawName );

            //  ' crafted by Dies Irae'
            if( Crafter != null && !string.IsNullOrEmpty( Crafter.Name ) )
                builder.AppendFormat( language == "ITA" ? " (creato da {0})" : " (crafted by {0})", StringUtility.Capitalize( Crafter.Name ) );

            // [silver]
            if( m_Slayer != SlayerName.None )
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName( m_Slayer );
                if( entry != null )
                    builder.AppendFormat( " [{0}]", language == "ITA" ? StringList.LocalizationIta[ entry.Title ] : StringList.Localization[ entry.Title ] );
            }

            // [exorcism]
            if( m_Slayer2 != SlayerName.None )
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName( m_Slayer2 );
                if( entry != null )
                    builder.AppendFormat( " [{0}]", language == "ITA" ? StringList.LocalizationIta[ entry.Title ] : StringList.Localization[ entry.Title ] );
            }

            if( DisplayLootType )
            {
                if( LootType == LootType.Blessed )
                    builder.Append( language == "ITA" ? " [Benedetto]" : " [Blessed]" );
                else if( LootType == LootType.Cursed )
                    builder.Append( language == "ITA" ? " [Maledetto]" : " [Cursed]" );
            }

            m_SecondAgeFullName = builder.ToString();
            m_SecondAgeUnIdentifiedName = string.Format( "magic {0}", rawName );
        }

        private void BuildStandardSecondAgeNames( string rawName, string language )
        {
            StringBuilder builder = new StringBuilder();

            // 'exceptional '
            if( Quality == InstrumentQuality.Exceptional )
                builder.Append( "exceptional " );

            // 'oak '
            if( m_Resource != CraftResource.None )
                builder.AppendFormat( "{0} ", MaterialName );

            // 'tamburine'
            builder.Append( rawName );

            //  ' crafted by Dies Irae'
            if( Crafter != null && !string.IsNullOrEmpty( Crafter.Name ) )
                builder.AppendFormat( language == "ITA" ? " (creato da {0})" : " (crafted by {0})", StringUtility.Capitalize( Crafter.Name ) );


            if( DisplayLootType )
            {
                if( LootType == LootType.Blessed )
                    builder.Append( language == "ITA" ? " [Benedetto]" : " [Blessed]" );
                else if( LootType == LootType.Cursed )
                    builder.Append( language == "ITA" ? " [Maledetto]" : " [Cursed]" );
            }

            m_SecondAgeFullName = builder.ToString();
            m_SecondAgeUnIdentifiedName = builder.ToString();
        }

        private void BuildSecondAgeNames( string rawName, string language )
        {
            if( IsMagical )
                BuildMagicalSecondAgeNames( rawName, language );
            else
                BuildStandardSecondAgeNames( rawName, language );
        }

        public void OnOldSingleClick( Mobile from )
        {
            if( Deleted || !from.CanSee( this ) )
                return;

            string name = string.IsNullOrEmpty( Name ) ? ( from.Language == "ITA" ? StringList.LocalizationIta[ LabelNumber ] : StringList.Localization[ LabelNumber ]) : Name;

            if( m_SecondAgeFullName == null || m_SecondAgeUnIdentifiedName == null )
                BuildSecondAgeNames( name, from.Language );

            if( this is ITreasureOfMidgard )
                XmlBlessedCursedAttach.OnSingleClick( from, this );

            if( this is ITreasureOfMidgard )
                LabelTo( from, name );
            else if( !String.IsNullOrEmpty( Name ) && Name != DefaultName )
                LabelTo( from, Name );
            else if( IsMagical )
            {
                if( IsIdentifiedFor( from ) )
                    LabelTo( from, StringUtility.ConvertItemName( m_SecondAgeFullName, from.Language ) );
                else
                    LabelTo( from, StringUtility.ConvertItemName( m_SecondAgeUnIdentifiedName, from.Language ) );
            }
            else
                LabelTo( from, StringUtility.ConvertItemName( m_SecondAgeFullName, from.Language ) );

            if( IsGuildItem )
            {
                if( m_OwnerGuild != null && GuildedOwner != null && GuildedOwner.Name != null )
                    LabelTo( from, from.Language == "ITA" ? "[gilda : {0} ({1})]" : "[guild : {0} ({1})]", m_OwnerGuild.Name, GuildedOwner.Name );
                else if( m_OwnerGuild != null )
                    LabelTo( from, from.Language == "ITA" ? "[gilda : {0}]" : "[guild : {0}]", m_OwnerGuild.Name );
            }

            if( !String.IsNullOrEmpty( m_EngravedText ) )
                LabelTo( from, "* {0} *", m_EngravedText );

            if( IsPortingItem )
                LabelOwnageTo( from );
        }

        [CommandProperty( AccessLevel.Developer )]
        public bool IsMagical
        {
            get { return Slayer != SlayerName.None || Slayer2 != SlayerName.None; }
        }

        [CommandProperty( AccessLevel.Developer )]
        public string MaterialName
        {
            get
            {
                if( CraftResources.IsStandard( m_Resource ) )
                    return string.Empty;

                return CraftResources.GetName( m_Resource );
            }
        }

        #region IIdentificable members
        private List<Mobile> m_IdentifiersList;

        private const int MaxIdentifiers = 50;

        public void CopyIdentifiersTo( IIdentificable identificable )
        {
            if( m_IdentifiersList != null && m_IdentifiersList.Count > 0 )
            {
                foreach( Mobile mobile in m_IdentifiersList )
                {
                    identificable.AddIdentifier( mobile );
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

            if( !m_IdentifiersList.Contains( from ) )
                m_IdentifiersList.Add( from );

            if( m_IdentifiersList.Count > MaxIdentifiers )
                m_IdentifiersList.RemoveAt( 0 );

            // debug
            if( Core.Debug )
            {
                Utility.Log( "BaseInsturmentAddIdentifier.log", "IdentifiersList for insturment {0}", Serial.ToString() );
                foreach( Mobile m in m_IdentifiersList )
                    Utility.Log( "BaseInsturmentAddIdentifier.log", m.Name );
            }
        }

        public bool IsIdentifiedFor( Mobile from )
        {
            if( Crafter != null && from == Crafter )
                return true;

            if( !IsMagical )
                return true;

            if( m_IdentifiersList == null )
                return false;

            return m_IdentifiersList.Contains( from );
        }

        public void DisplayItemInfo( Mobile from )
        {
        }
        #endregion

        #region [serialization]
        private static void SetSaveFlag( ref MidgardFlag flags, MidgardFlag toSet, bool setIf )
        {
            if( setIf )
                flags |= toSet;
        }

        private static bool GetSaveFlag( MidgardFlag flags, MidgardFlag toGet )
        {
            return ( ( flags & toGet ) != 0 );
        }

        [Flags]
        private enum MidgardFlag
        {
            None = 0x00000000,

            EngravedText = 0x00000001,
            Resource = 0x00000002,
            ItemIDList = 0x00000004,
            IsGuildUniform = 0x00000008,

            Guild = 0x00000001,
            GuildedOwner = 0x00000002
        }

        private void MidgardSerialize( GenericWriter writer )
        {
            MidgardFlag flags = MidgardFlag.None;

            SetSaveFlag( ref flags, MidgardFlag.EngravedText, m_EngravedText != null );
            SetSaveFlag( ref flags, MidgardFlag.Resource, m_Resource != CraftResource.None );
            SetSaveFlag( ref flags, MidgardFlag.ItemIDList, m_IdentifiersList != null && m_IdentifiersList.Count > 0 );

            writer.WriteEncodedInt( (int)flags );

            if( GetSaveFlag( flags, MidgardFlag.EngravedText ) )
                writer.Write( (string)m_EngravedText );

            if( GetSaveFlag( flags, MidgardFlag.Resource ) )
                writer.Write( (int)m_Resource );

            if( GetSaveFlag( flags, MidgardFlag.ItemIDList ) )
                writer.Write( m_IdentifiersList, true );

            if( GetSaveFlag( flags, MidgardFlag.IsGuildUniform ) )
                writer.Write( IsGuildItem );

            if( GetSaveFlag( flags, MidgardFlag.Guild ) )
                writer.Write( (BaseGuild)m_OwnerGuild );

            if( GetSaveFlag( flags, MidgardFlag.GuildedOwner ) )
                writer.Write( (Mobile)GuildedOwner );
        }

        private void MidgardDeserialize( GenericReader reader, int version )
        {
            MidgardFlag flags = (MidgardFlag)reader.ReadEncodedInt();

            if( GetSaveFlag( flags, MidgardFlag.EngravedText ) )
                m_EngravedText = reader.ReadString();

            if( GetSaveFlag( flags, MidgardFlag.Resource ) )
                m_Resource = (CraftResource)reader.ReadInt();

            if( GetSaveFlag( flags, MidgardFlag.ItemIDList ) )
                m_IdentifiersList = reader.ReadStrongMobileList();

            if( GetSaveFlag( flags, MidgardFlag.IsGuildUniform ) )
                IsGuildItem = reader.ReadBool();

            if( GetSaveFlag( flags, MidgardFlag.Guild ) )
                m_OwnerGuild = reader.ReadGuild();

            if( GetSaveFlag( flags, MidgardFlag.GuildedOwner ) )
                GuildedOwner = reader.ReadMobile();
        }
        #endregion

        #endregion
    }
}