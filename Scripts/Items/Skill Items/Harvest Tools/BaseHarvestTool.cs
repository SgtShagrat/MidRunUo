using System;
using System.Collections.Generic;
using System.Text;
using Midgard;
using Server.Mobiles;
using Server.Network;
using Server.Engines.Craft;
using Server.Engines.Harvest;
using Server.ContextMenus;
using Server.Regions;

namespace Server.Items
{
	interface IUsesRemaining
	{
		int UsesRemaining{ get; set; }
		bool ShowUsesRemaining{ get; set; }
	}

	public abstract class BaseHarvestTool : Item, IUsesRemaining, ICraftable, IResourceItem
	{
		private Mobile m_Crafter;
		private ToolQuality m_Quality;
		private int m_UsesRemaining;

        /*
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Crafter
		{
			get{ return m_Crafter; }
            set { m_Crafter = value; InvalidateProperties(); InvalidateSecondAgeNames(); }
		}
        */

		[CommandProperty( AccessLevel.GameMaster )]
		public ToolQuality Quality
		{
			get{ return m_Quality; }
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

        /*
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

		public abstract HarvestSystem HarvestSystem{ get; }

		public BaseHarvestTool( int itemID ) : this( /* 50 * 2 */ GetDefaultUses(), itemID ) // mod by Dies Irae
		{
		}

		public BaseHarvestTool( int usesRemaining, int itemID ) : base( itemID )
		{
			m_UsesRemaining = usesRemaining;
			m_Quality = ToolQuality.Regular;

            m_Resource = CraftResource.None; // mod by Dies Irae
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			// Makers mark not displayed on OSI
			//if ( m_Crafter != null )
			//	list.Add( 1050043, m_Crafter.Name ); // crafted by ~1_NAME~

			if ( m_Quality == ToolQuality.Exceptional )
				list.Add( 1060636 ); // exceptional

			list.Add( 1060584, m_UsesRemaining.ToString() ); // uses remaining: ~1_val~
		}

		public virtual void DisplayDurabilityTo( Mobile m )
		{
			LabelToAffix( m, 1017323, AffixType.Append, ": " + m_UsesRemaining.ToString() ); // Durability
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

            // 'shovel'
            builder.Append( rawName.ToLower() );

            //  ' crafted by Dies Irae'
            if( Crafter != null && !string.IsNullOrEmpty( Crafter.Name ) )
                builder.AppendFormat( language == "ITA" ? " (creato da {0})" : " (crafted by {0})", Crafter.Name );

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
                LabelTo( from, StringUtility.ConvertItemName( m_SecondAgeFullName, from.Language ) );

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

			    HarvestSystem.BeginHarvesting( from, this );
			}
			else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			AddContextMenuEntries( from, this, list, HarvestSystem );
		}

		public static void AddContextMenuEntries( Mobile from, Item item, List<ContextMenuEntry> list, HarvestSystem system )
		{
			if ( system != Mining.System )
				return;

			if ( !item.IsChildOf( from.Backpack ) && item.Parent != from )
				return;

			PlayerMobile pm = from as PlayerMobile;

			if ( pm == null )
				return;

			ContextMenuEntry miningEntry = new ContextMenuEntry( pm.ToggleMiningStone ? 6179 : 6178 );
			miningEntry.Color = 0x421F;
			list.Add( miningEntry );

			list.Add( new ToggleMiningStoneEntry( pm, false, 6176 ) );
			list.Add( new ToggleMiningStoneEntry( pm, true, 6177 ) );
		}

		private class ToggleMiningStoneEntry : ContextMenuEntry
		{
			private PlayerMobile m_Mobile;
			private bool m_Value;

			public ToggleMiningStoneEntry( PlayerMobile mobile, bool value, int number ) : base( number )
			{
				m_Mobile = mobile;
				m_Value = value;

				bool stoneMining = ( mobile.StoneMining /* && mobile.Skills[SkillName.Mining].Base >= 100.0 */ );

				if ( mobile.ToggleMiningStone == value || ( value && !stoneMining ) )
					this.Flags |= CMEFlags.Disabled;
			}

			public override void OnClick()
			{
				bool oldValue = m_Mobile.ToggleMiningStone;

				if ( m_Value )
				{
					if ( oldValue )
					{
						m_Mobile.SendLocalizedMessage( 1054023 ); // You are already set to mine both ore and stone!
					}
					else if ( !m_Mobile.StoneMining /* || m_Mobile.Skills[SkillName.Mining].Base < 100.0 */ )
					{
						m_Mobile.SendLocalizedMessage( 1054024 ); // You have not learned how to mine stone or you do not have enough skill!
					}
					else
					{
						m_Mobile.ToggleMiningStone = true;
						m_Mobile.SendLocalizedMessage( 1054022 ); // You are now set to mine both ore and stone.
					}
				}
				else
				{
					if ( oldValue )
					{
						m_Mobile.ToggleMiningStone = false;
						m_Mobile.SendLocalizedMessage( 1054020 ); // You are now set to mine only ore.
					}
					else
					{
						m_Mobile.SendLocalizedMessage( 1054021 ); // You are already set to mine only ore!
					}
				}
			}
		}

		public BaseHarvestTool( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version

            writer.Write( PlayerConstructed ); // mod by Dies Irae

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
                case 2:
                {
                    PlayerConstructed = reader.ReadBool();
                    goto case 1;
                }
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
		}

		#region ICraftable Members
		public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
			Quality = (ToolQuality)quality;

			if ( makersMark )
				Crafter = from;

            PlayerConstructed = true;
            CrafterSkill = from.Skills[ craftSystem.MainSkill ].Value;

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