using System;
using System.Text;
using Midgard;
using Midgard.Engines.OldCraftSystem;

using Server.Engines.Harvest;
using Server.Network;
using Server.Engines.Craft;

using Server.Regions;

namespace Server.Items
{
	public enum ToolQuality
	{
		Low,
		Regular,
		Exceptional
	}

	public abstract class BaseTool : Item, IUsesRemaining, ICraftable, IResourceItem
	{
		private Mobile m_Crafter;
		private ToolQuality m_Quality;
		private int m_UsesRemaining;

        /*
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Crafter
		{
			get{ return m_Crafter; }
			set{ m_Crafter = value; InvalidateProperties(); InvalidateSecondAgeNames(); }
		}
        */

		[CommandProperty( AccessLevel.GameMaster )]
		public ToolQuality Quality
		{
            get
            {
                #region mod by Dies Irae
                if( CustomQuality >= Server.Quality.Exceptional )
                    return ToolQuality.Exceptional;
                else if( CustomQuality <= Server.Quality.Low )
                    return ToolQuality.Low;
                #endregion

                return m_Quality;
            }
			set{ UnscaleUses(); m_Quality = value; InvalidateProperties(); ScaleUses(); InvalidateSecondAgeNames(); }
		}

        #region mod by Dies Irae
        private CraftResource m_Resource;

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual CraftResource Resource
        {
            get { return m_Resource; }
            set
            {
                UnscaleUses();
                m_Resource = value;
                Hue = CraftResources.GetHue( m_Resource ); 
                ScaleUses();
                InvalidateSecondAgeNames();
            }
        }

		public virtual void UnscaleUses()
		{
			int scale = 100 + GetDurabilityBonus();
            m_UsesRemaining = ( ( m_UsesRemaining * 100 ) + ( scale - 1 ) ) / scale;
		}

		public virtual void ScaleUses()
		{
			int scale = 100 + GetDurabilityBonus();
            m_UsesRemaining = ( ( m_UsesRemaining * scale ) + 99 ) / 100;
		}

		public int GetDurabilityBonus()
		{
			int bonus = OldGetQualityDurabilityBonus();

            if( ResourceInfo != null )
				bonus += ResourceInfo.OldToolDurability;

			return bonus;
		}

	    public int OldGetQualityDurabilityBonus()
	    {
            if( Quality == ToolQuality.Exceptional )
                return +20;

	        if( CustomQuality == Server.Quality.Undefined )
	            return 0;
	        else if( CustomQuality <= Server.Quality.VeryLow )
	            return -20;
	        else if( CustomQuality <= Server.Quality.Low )
	            return -10;
	        else if( CustomQuality <= Server.Quality.Decent )
	            return -4;
	        else if( CustomQuality <= Server.Quality.BelowNormal )
	            return 0;
	        else if( CustomQuality <= Server.Quality.Standard )
	            return +8;
	        else if( CustomQuality <= Server.Quality.Superior )
	            return +12;
	        else if( CustomQuality <= Server.Quality.Great )
	            return +16;
	        else
	            return +20;
	    }

	    [CommandProperty( AccessLevel.GameMaster )]
	    public string MaterialName
	    {
	        get { return CraftResources.IsStandard( m_Resource ) ? string.Empty : CraftResources.GetName( m_Resource ); }
	    }

	    private CraftAttributeInfo ResourceInfo
	    {
	        get
	        {
	            CraftResourceInfo resInfo = CraftResources.GetInfo( m_Resource );
	            if( resInfo != null )
	            {
	                CraftAttributeInfo attrInfo = resInfo.AttributeInfo;
	                if( attrInfo != null )
	                    return attrInfo;
	            }

	            return null;
	        }
	    }

        private static readonly DiceRoll m_DefaultUsesDice = new DiceRoll( 1, 10, 50 ); // 1d10+50

        public static int GetDefaultUses()
        {
            return m_DefaultUsesDice.Roll();
        }
        #endregion

		[CommandProperty( AccessLevel.GameMaster )]
		public int UsesRemaining
		{
			get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
		}

        /* mod by Dies Irae
		public void ScaleUses()
		{
			m_UsesRemaining = (m_UsesRemaining * GetUsesScalar()) / 100;
			InvalidateProperties();
		}

		public void UnscaleUses()
		{
			m_UsesRemaining = (m_UsesRemaining * 100) / GetUsesScalar();
		}
        
		public int GetUsesScalar()
		{
			if ( m_Quality == ToolQuality.Exceptional )
				return 200;

			return 100;
		}
        */

		public bool ShowUsesRemaining{ get{ return true; } set{} }

		public abstract CraftSystem CraftSystem{ get; }

        public BaseTool( int itemID )
            : this( /* Utility.RandomMinMax( 25, 75 ) * 2 */ GetDefaultUses(), itemID )
		{
		}

		public BaseTool( int uses, int itemID ) : base( itemID )
		{
			m_UsesRemaining = uses;
			m_Quality = ToolQuality.Regular;

		    m_Resource = CraftResource.None; // mod by Dies Irae
		}

		public BaseTool( Serial serial ) : base( serial )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			// Makers mark not displayed on OSI
			if ( m_Crafter != null )
				list.Add( 1050043, m_Crafter.Name ); // crafted by ~1_NAME~

			if ( m_Quality == ToolQuality.Exceptional )
				list.Add( 1060636 ); // exceptional

			list.Add( 1060584, m_UsesRemaining.ToString() ); // uses remaining: ~1_val~
		}

		public virtual void DisplayDurabilityTo( Mobile m )
		{
			LabelToAffix( m, 1017323, AffixType.Append, ": " + m_UsesRemaining.ToString() ); // Durability
		}

		public static bool CheckAccessible( Item tool, Mobile m )
		{
			return ( tool.IsChildOf( m ) || tool.Parent == m );
		}

		public static bool CheckTool( Item tool, Mobile m )
		{
			Item check = m.FindItemOnLayer( Layer.OneHanded );

			if ( check is BaseTool && check != tool && !(check is AncientSmithyHammer) )
				return false;

			check = m.FindItemOnLayer( Layer.TwoHanded );

			if ( check is BaseTool && check != tool && !(check is AncientSmithyHammer) )
				return false;

			return true;
        }

        #region mod by Dies Irae
        private string m_SecondAgeFullName;

        private void InvalidateSecondAgeNames()
        {
            BuildSecondAgeNames( string.IsNullOrEmpty( Name ) ? StringList.Localization[ LabelNumber ] : Name, "ITA" );
        }

        private void BuildStandardSecondAgeNames( string rawName, string language )
        {
            StringBuilder builder = new StringBuilder();

            // 'exceptional '
            if( Quality == ToolQuality.Exceptional )
                builder.Append( "exceptional " );

            // 'radiand diamond '
            if( !CraftResources.IsStandard( m_Resource ) )
                builder.AppendFormat( "{0} ", MaterialName.ToLower() );

            // 'hammer'
            builder.Append( rawName.ToLower() );

            //  ' crafted by Dies Irae'
            if( Crafter != null && !string.IsNullOrEmpty( Crafter.Name ) )
                builder.AppendFormat( language == "ITA" ? "(creato da {0})" : " (crafted by {0})", Crafter.Name );

            if( DisplayLootType )
            {
                if( LootType == LootType.Blessed )
                    builder.Append( language == "ITA" ? " [Benedetto]" : " [Blessed]" );
                else if( LootType == LootType.Cursed )
                    builder.Append( language == "ITA" ? " [Maledetto]" : " [Cursed]" );
            }

            m_SecondAgeFullName = builder.ToString();
        }

        private void BuildSecondAgeNames( string rawName, string language )
        {
            BuildStandardSecondAgeNames( rawName, language );
        }

        public void OnOldSingleClick( Mobile from )
        {
            if( Deleted || !from.CanSee( this ) )
                return;

            string name = StringList.GetClilocString( Name, LabelNumber, from.Language );

            if( m_SecondAgeFullName == null )
                BuildSecondAgeNames( name, from.Language );

            if( !String.IsNullOrEmpty( Name ) && Name != DefaultName )
                LabelTo( from, Name );
            else
                LabelTo( from, StringUtility.ConvertItemName( m_SecondAgeFullName, false, from.Language ) );

            if( IsPortingItem )
                LabelOwnageTo( from );
        }
        #endregion

        public override void OnSingleClick( Mobile from )
		{
            #region mod by Dies Irae
            OnOldSingleClick( from );
            // base.OnSingleClick( from );
            return;
            #endregion

			DisplayDurabilityTo( from );

			base.OnSingleClick( from );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) || Parent == from )
			{
                #region mod by Dies Irae
                if( HarvestSystem.IsHarvesting( from ) )
                {
                    from.SendMessage( "Thou are busy at this time." );
                    return;
                }
                else if( from.Region.IsPartOf( typeof( Jail ) ) )
                {
                    from.SendMessage( "Are thou a foolish player?" );
                    return;
                }
                #endregion

				CraftSystem system = this.CraftSystem;

				int num = system.CanCraft( from, this, null );

				if ( num > 0 && ( num != 1044267 || !Core.SE ) ) // Blacksmithing shows the gump regardless of proximity of an anvil and forge after SE
				{
					from.SendLocalizedMessage( num );
				}
				else
				{
					CraftContext context = system.GetContext( from );

                    if( !Core.AOS && system.SupportOldMenu )
                    {
                        if( from.HasMenu( typeof( OldCraftMenu ) ) )
                        {
                            from.SendMessage( "Plase, close all other craft menu before proceeding." );
                            return;
                        }

                        system.ResetContext( from );
                        OldCraftMenu.DisplayTo( from, system, this );
                    }
                    else
					    from.SendGump( new CraftGump( from, system, this, null ) );
				}
			}
			else
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 3 ); // version

            writer.Write( (int) m_Resource ); // mod by Dies Irae

			writer.Write( (Mobile) m_Crafter );
			writer.Write( (int) m_Quality );

			writer.Write( (int) m_UsesRemaining );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
                #region mod by Dies Irae
                case 3:
			        {
			            goto case 2;
			        }
                case 2:
                    {
                        m_Resource = (CraftResource)reader.ReadInt();
                        goto case 1;
                    }
                #endregion
				case 1:
				{
					m_Crafter = reader.ReadMobile();
					m_Quality = (ToolQuality) reader.ReadInt();
					goto case 0;
				}
				case 0:
				{
					m_UsesRemaining = reader.ReadInt();
					break;
				}
			}

            #region mod by Dies Irae
            if( version < 2 )
                m_Resource = CraftResource.None;

            if( version < 3 )
            {
                if( CustomQuality == Server.Quality.Undefined )
                    CustomQuality = Server.Quality.VeryLow;

                int oldUses = m_UsesRemaining;
                m_UsesRemaining = GetDefaultUses();
                ScaleUses();

                Utility.Log( "baseToolUsesFix.log", "serial {0:X4} - type {1} - quality {2} - resource {3} - oldUses {4} - newUses {5}.",
                    Serial.ToString(), GetType().Name, Server.Quality.GetQualityName( this ), m_Resource.ToString(), oldUses, m_UsesRemaining );

                if( oldUses < m_UsesRemaining )
                    m_UsesRemaining = oldUses;
            }
            #endregion
		}

		#region ICraftable Members
		public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
			Quality = (ToolQuality)quality;

			if ( makersMark )
				Crafter = from;

            PlayerConstructed = true;
            CrafterSkill = from.Skills[ craftSystem.MainSkill ].Value;

            #region mod by Dies Irae
            Type resourceType = typeRes ?? craftItem.Resources.GetAt( 0 ).ItemType;

		    Resource = CraftResources.GetFromType( resourceType );
            #endregion

			return quality;
		}

        [CommandProperty( AccessLevel.GameMaster )]
        public bool PlayerConstructed { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Crafter
        {
            get { return m_Crafter; }
            set { m_Crafter = value; InvalidateSecondAgeNames(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public double CrafterSkill { get; set; }
		#endregion
	}
}