//#define PrePassaggioMondain
// #define FixNames
// #define MLSVN

using System;
using System.Collections.Generic;
using System.Text;

using Midgard.Items;

using Server.Multis;
using Server.Mobiles;
using Server.Network;
using Server.ContextMenus;
using Server.Engines.Craft;
using Midgard.Engines.Races;

namespace Server.Items
{
	public enum ContainerQuality
	{
		Low,
		Regular,
		Exceptional
	}

    public abstract class BaseContainer : Container, ICraftable, IEngravable, IResourceItem
	{
		public override int DefaultMaxWeight
		{
			get
			{
                if ( Core.ML ) {
					Mobile m = ParentEntity as Mobile;
					if ( m != null && m.Player && m.Backpack == this ) {
						return 550;
					} else {
						return base.DefaultMaxWeight;
					}
                } else {
                // mod by Dies Irae
                // A secure container can hold a maximum of 125 items and 400 stones.
                // http://wiki.uosecondage.com/index.php?title=House
				if ( IsSecure )
					return BaseHouse.OldSchoolRules ? 400 : 0;

                // a locked down container can have 125 items, unlimited stones
                if ( IsLockedDown )
                    return BaseHouse.OldSchoolRules ? 0 : base.DefaultMaxWeight;

				return base.DefaultMaxWeight;
                }
			}
		}

        public override int DefaultMaxItems
        {
            get
            {
                // mod by Dies Irae
                // A secure container can hold a maximum of 125 items and 400 stones.
                // A locked down container can have 125 items, unlimited stones
                // http://wiki.uosecondage.com/index.php?title=House
				if ( IsSecure || IsLockedDown )
					return BaseHouse.OldSchoolRules ? 125 : base.DefaultMaxItems;
                
                return base.DefaultMaxItems;
            }
        }

		#region Mondain's Legacy
		private ContainerQuality m_Quality;
		private CraftResource m_Resource;
		private Mobile m_Crafter;
		private string m_EngravedText;

		[CommandProperty( AccessLevel.GameMaster )]
		public ContainerQuality Quality
		{
			get
			{
               #region mod by Dies Irae
                if( CustomQuality >= Server.Quality.Exceptional )
                    return ContainerQuality.Exceptional;
                else if( CustomQuality <= Server.Quality.Low )
                    return ContainerQuality.Low;
                #endregion

                return m_Quality;
			}
			set{ m_Quality = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public CraftResource Resource
		{
			get{ return m_Resource;	}
			set
			{
				if ( m_Resource != value )
				{
					m_Resource = value;
					Hue = CraftResources.GetHue( m_Resource );
					
					InvalidateProperties();
				}
			}
		}

        /*
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Crafter
		{
			get{ return m_Crafter; }
			set{ m_Crafter = value; InvalidateProperties(); }
		}
		*/

		[CommandProperty( AccessLevel.GameMaster )]		
		public string EngravedText
		{
			get{ return m_EngravedText; }
			set{ m_EngravedText = value; InvalidateProperties(); }
		}
		#endregion

		public BaseContainer( int itemID ) : base( itemID )
		{
		}

		public override bool IsAccessibleTo( Mobile m )
		{
			if ( !BaseHouse.CheckAccessible( m, this ) )
				return false;

			return base.IsAccessibleTo( m );
		}

		public override bool CheckHold( Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight )
		{
			if ( this.IsSecure && !BaseHouse.CheckHold( m, this, item, message, checkItems, plusItems, plusWeight ) )
				return false;

			return base.CheckHold( m, item, message, checkItems, plusItems, plusWeight );
		}

		public override bool CheckItemUse( Mobile from, Item item )
		{
			if ( IsDecoContainer && item is BaseBook )
				return true;

			return base.CheckItemUse( from, item );
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );
			SetSecureLevelEntry.AddTo( from, this, list );
		}

		public override bool TryDropItem( Mobile from, Item dropped, bool sendFullMessage )
		{
			if ( !CheckHold( from, dropped, sendFullMessage, true ) )
				return false;

			BaseHouse house = BaseHouse.FindHouseAt( this );

			if ( house != null && house.IsLockedDown( this ) )
			{
				if ( dropped is VendorRentalContract || ( dropped is Container && ((Container)dropped).FindItemByType( typeof( VendorRentalContract ) ) != null ) )
				{
					from.SendLocalizedMessage( 1062492 ); // You cannot place a rental contract in a locked down container.
					return false;
				}

                // mod by Dies Irae
                // http://wiki.uosecondage.com/index.php?title=House
                // you can't lock an item down inside a container
				if ( !BaseHouse.OldSchoolRules && !house.LockDown( from, dropped, false ) )
					return false;
			}

			List<Item> list = this.Items;

			for ( int i = 0; i < list.Count; ++i )
			{
				Item item = list[i];

				if ( !(item is Container) && item.StackWith( from, dropped, false ) )
					return true;
			}

			DropItem( dropped );

            #region ARTEGORDONMOD
            // Begin mod for spawner release of items
            // set flag to have item taken off spawner list at next defrag
            ItemFlags.SetTaken( dropped, true );
            #endregion

			return true;
		}

		public override bool OnDragDropInto( Mobile from, Item item, Point3D p )
		{
			if ( !CheckHold( from, item, true, true ) )
				return false;

			BaseHouse house = BaseHouse.FindHouseAt( this );

			if ( house != null && house.IsLockedDown( this ) )
			{
				if ( item is VendorRentalContract || ( item is Container && ((Container)item).FindItemByType( typeof( VendorRentalContract ) ) != null ) )
				{
					from.SendLocalizedMessage( 1062492 ); // You cannot place a rental contract in a locked down container.
					return false;
				}

                // mod by Dies Irae
                // http://wiki.uosecondage.com/index.php?title=House
                // you can't lock an item down inside a container
				if ( !BaseHouse.OldSchoolRules && !house.LockDown( from, item, false ) )
					return false;
			}

			item.Location = new Point3D( p.X, p.Y, 0 );
			AddItem( item );

			from.SendSound( GetDroppedSound( item ), GetWorldLocation() );

			#region ARTEGORDONMOD
			// Begin mod for spawner release of items
			// set flag to have item taken off spawner list at next defrag
            ItemFlags.SetTaken(item,true);
            #endregion

			return true;
		}

		public override void UpdateTotal( Item sender, TotalType type, int delta )
		{
			base.UpdateTotal( sender, type, delta );

			if ( type == TotalType.Weight && RootParent is Mobile )
				((Mobile) RootParent).InvalidateProperties();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.AccessLevel > AccessLevel.Player || from.InRange( this.GetWorldLocation(), 2 ) || this.RootParent is PlayerVendor )
				Open( from );
			else
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
		}
		
		public override void AddWeightProperty( ObjectPropertyList list )
		{
			base.AddWeightProperty( list );

			/*
			#region Mondain's Legacy
			if ( m_EngravedText != null )
				list.Add( 1072305, m_EngravedText ); // Engraved: ~1_INSCRIPTION~

			if ( m_Crafter != null )
				list.Add( 1050043, m_Crafter.Name ); // crafted by ~1_NAME~

			if ( m_Quality == ClothingQuality.Exceptional )
				list.Add( 1060636 ); // exceptional
			#endregion
			*/
		}

		#region modifica by Dies Irae
        public override bool CanEquip( Mobile from )
        {
            if( !RaceAllowanceAttribute.IsAllowed( this, from, Midgard.Engines.Races.Core.MountainDwarf, true ) )
                return false;

            return base.CanEquip( from );
        }

		public override void AddNameProperty( ObjectPropertyList list )
		{
			int oreType;

			switch ( m_Resource )
			{
				case CraftResource.DullCopper:		oreType = 1053108; break; // dull copper
				case CraftResource.ShadowIron:		oreType = 1053107; break; // shadow iron
				case CraftResource.Copper:			oreType = 1053106; break; // copper
				case CraftResource.Bronze:			oreType = 1053105; break; // bronze
				case CraftResource.Gold:			oreType = 1053104; break; // golden
				case CraftResource.Agapite:			oreType = 1053103; break; // agapite
				case CraftResource.Verite:			oreType = 1053102; break; // verite
				case CraftResource.Valorite:		oreType = 1053101; break; // valorite

				case CraftResource.Platinum:		oreType = 1064690; break; // platinum
				case CraftResource.Titanium:		oreType = 1064691; break; // titanium
				case CraftResource.Obsidian:		oreType = 1064692; break; // obsidian
				case CraftResource.DarkRuby:		oreType = 1064693; break; // darkRuby
				case CraftResource.EbonSapphire:	oreType = 1064694; break; // ebon sapphire
				case CraftResource.RadiantDiamond:	oreType = 1064695; break; // radiant diamond
				case CraftResource.Eldar:			oreType = 1064696; break; // eldar
				case CraftResource.Crystaline:		oreType = 1064697; break; // crystaline
				case CraftResource.Vulcan:			oreType = 1064698; break; // vulcan
				case CraftResource.Aqua:			oreType = 1064699; break; // aqua

				case CraftResource.Oak:				oreType = 1065471; break; // oak
				case CraftResource.Walnut:			oreType = 1065472; break; // walnut
				case CraftResource.Ohii:			oreType = 1065473; break; // ohii
				case CraftResource.Cedar:			oreType = 1065474; break; // cedar
				case CraftResource.Willow:			oreType = 1065475; break; // willow
				case CraftResource.Cypress:			oreType = 1065476; break; // cypress
				case CraftResource.Yew:				oreType = 1065477; break; // yew
				case CraftResource.Apple:			oreType = 1065478; break; // apple
				case CraftResource.Pear:			oreType = 1065479; break; // pear
				case CraftResource.Peach:			oreType = 1065480; break; // peach
				case CraftResource.Banana:			oreType = 1065481; break; // banana
				case CraftResource.Stonewood:		oreType = 1065482; break; // stonewood
				case CraftResource.Silver:			oreType = 1065483; break; // silver
				case CraftResource.Blood:			oreType = 1065484; break; // blood
				case CraftResource.Swamp:			oreType = 1065485; break; // swamp
				case CraftResource.Crystal:			oreType = 1065486; break; // crystal
				case CraftResource.Elven:			oreType = 1065487; break; // elven
				case CraftResource.Elder:			oreType = 1065488; break; // elder
				case CraftResource.Enchanted:		oreType = 1065489; break; // enchanted
				case CraftResource.Death:			oreType = 1065490; break; // death	

				case CraftResource.SpinedLeather:	oreType = 1061118; break; // spined
				case CraftResource.HornedLeather:	oreType = 1061117; break; // horned
				case CraftResource.BarbedLeather:	oreType = 1061116; break; // barbed
				case CraftResource.RedScales:		oreType = 1060814; break; // red
				case CraftResource.YellowScales:	oreType = 1060818; break; // yellow
				case CraftResource.BlackScales:		oreType = 1060820; break; // black
				case CraftResource.GreenScales:		oreType = 1060819; break; // green
				case CraftResource.WhiteScales:		oreType = 1060821; break; // white
				case CraftResource.BlueScales:		oreType = 1060815; break; // blue
				default: oreType = 0; break;
			}

			if( m_Quality == ContainerQuality.Exceptional )
			{
				if ( oreType != 0 )
					list.Add( 1053100, "#{0}\t{1}", oreType, GetNameString() ); // exceptional ~1_oretype~ ~2_armortype~
				else
					list.Add( 1050040, GetNameString() ); // exceptional ~1_ITEMNAME~
			}
			else
			{
				if ( oreType != 0 )
					list.Add( 1053099, "#{0}\t{1}", oreType, GetNameString() ); // ~1_oretype~ ~2_armortype~
				else if ( Name == null )
					list.Add( LabelNumber );
				else
					list.Add( Name );
			}

			if( m_EngravedText != null )
				list.Add( 1062613, m_EngravedText ); // "~1_NAME~"
		}

		private string GetNameString()
		{
			string name = this.Name;

			if ( name == null )
				name = String.Format( "#{0}", LabelNumber );

			return name;
		}

		public override void OnSingleClick( Mobile from )
		{
			StringBuilder info = new StringBuilder();

			CraftResourceInfo materialName = CraftResources.IsStandard( m_Resource ) ? null : CraftResources.GetInfo( m_Resource );

			if (from.Language == "ITA")
			{
				info.Append( string.IsNullOrEmpty( Name ) ? StringList.LocalizationIta[ LabelNumber ] +" " : Name+" " ); // 'box'

				if( Quality == ContainerQuality.Exceptional )
					info.Append( "eccezionale " ); // 'exceptional '

				if( materialName != null && materialName.Name != null )
					info.AppendFormat( "{0} ", materialName.Name.ToLower() ); // 'oak '

				if( Crafter != null && !String.IsNullOrEmpty( Crafter.Name ) )
					info.AppendFormat( " (creato da {0})", Crafter.Name ); // ' crafted by Dies Irae'
			}
			else
			{
				if( Quality == ContainerQuality.Exceptional )
					info.Append( "exceptional " ); // 'exceptional '

				if( materialName != null && materialName.Name != null )
					info.AppendFormat( "{0} ", materialName.Name.ToLower() ); // 'oak '

				info.Append( string.IsNullOrEmpty( Name ) ?  StringList.Localization[ LabelNumber ] : Name ); // 'box'

				if( Crafter != null && !String.IsNullOrEmpty( Crafter.Name ) )
					info.AppendFormat( " (crafted by {0})", Crafter.Name ); // ' crafted by Dies Irae'
			}
			LabelTo( from, info.ToString() );

			if( !String.IsNullOrEmpty( m_EngravedText ) )
				LabelTo( from, "* {0} *", m_EngravedText );
		}
		#endregion

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			#region modifica by Dies Irae
			if ( m_Crafter != null )
				list.Add( 1050043, m_Crafter.Name ); // crafted by ~1_NAME~
			#endregion

			/*
			#region Mondain's Legacy
			CraftResourceInfo info = CraftResources.IsStandard( m_Resource ) ? null : CraftResources.GetInfo( m_Resource );

			if ( info != null && info.Number > 0 )
				list.Add( info.Number );
			#endregion
			*/		
		}

		public virtual void Open( Mobile from )
		{
			DisplayTo( from );
		}

		public BaseContainer( Serial serial ) : base( serial )
		{
		}

		/* Note: base class insertion; we cannot serialize anything here */
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			#region Mondain's Legacy
			writer.Write( 2 ); // version

		    writer.Write( PlayerConstructed ); // mod by Dies Irae

			writer.Write( (string) m_EngravedText );
			writer.Write( (int) m_Quality );
			writer.Write( (int) m_Resource );
			writer.Write( (Mobile) m_Crafter );
			#endregion
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
#if MLSVN
            if( GetType().IsSubclassOf( typeof(BaseAddonContainer) ) )
            {
                Console.WriteLine( ">>> Deserialize.BaseAddonContainer rilevato..." );
                return;
            }
#endif

			#region Mondain's Legacy
#if PrePassaggioMondain
			int version = 0;
#else
			int version = reader.ReadInt();
#endif
			switch( version )
			{
                case 2:
			    {
			        PlayerConstructed = reader.ReadBool();
			        goto case 1;
			    }
				case 1:
				{
					m_EngravedText = reader.ReadString();
					m_Quality = (ContainerQuality) reader.ReadInt();
					m_Resource = (CraftResource) reader.ReadInt();
					m_Crafter = reader.ReadMobile();
					goto case 0;
				}
				case 0:
				{
					break;
				}
			}			
			#endregion

#if FixNames
			Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( Midgard.CraftHelper.VerifyContainerName_Callback ), this );
#endif
		}
		
		#region ICraftable
		public virtual int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
			Quality = (ContainerQuality) quality;

			if ( makersMark )
				Crafter = from;

            PlayerConstructed = true;
            CrafterSkill = from.Skills[ craftSystem.MainSkill ].Value;

			Type resourceType = typeRes;

			if ( resourceType == null )
				resourceType = craftItem.Resources.GetAt( 0 ).ItemType;

			Resource = CraftResources.GetFromType( resourceType );

			CraftContext context = craftSystem.GetContext( from );

			if ( context != null && context.DoNotColor )
				Hue = 0;

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
	}

	public class StrongBackpack : Backpack	//Used on Pack animals
	{
		[Constructable]
		public StrongBackpack()
		{
			Layer = Layer.Backpack;
			Weight = 13.0;
		}

		public override bool CheckHold( Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight )
		{
			return base.CheckHold( m, item, false, checkItems, plusItems, plusWeight );
		}

	    #region mod by Dies Irae
	    // public override int DefaultMaxWeight{ get{ return 1600; } }

	    public override int DefaultMaxWeight
	    {
	        get
	        {
                BaseCreature bc = ParentEntity as BaseCreature;

	            if( bc != null && bc.Backpack == this )
	                return bc.StoneMaxWeight;
	            else
	                return 1600;
	        }
	    }
	    #endregion


		public override bool CheckContentDisplay( Mobile from )
		{
			object root = this.RootParent;

			if ( root is BaseCreature && ((BaseCreature)root).Controlled && ((BaseCreature)root).ControlMaster == from )
				return true;

			return base.CheckContentDisplay( from );
		}

		public StrongBackpack( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 )
				Weight = 13.0;
		}
	}

	public class Backpack : BaseContainer, IDyable
	{
		[Constructable]
		public Backpack() : base( 0xE75 )
		{
			Layer = Layer.Backpack;
			Weight = 3.0;
		}

		public override int DefaultMaxWeight {
			get {
				if ( Core.ML ) {
					Mobile m = ParentEntity as Mobile;
					if ( m != null && m.Player && m.Backpack == this ) {
						return 550;
					} else {
						return base.DefaultMaxWeight;
					}
				} else {
					return base.DefaultMaxWeight;
				}
			}
		}

		public Backpack( Serial serial ) : base( serial )
		{
		}

		public bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted ) return false;

			Hue = sender.DyedHue;

			return true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 && ItemID == 0x9B2 )
				ItemID = 0xE75;
		}
	}

	public class Pouch : TrapableContainer, IDyable // mod by Dies Irae
	{
		[Constructable]
		public Pouch() : base( 0xE79 )
		{
			Weight = 1.0;
		}

		public Pouch( Serial serial ) : base( serial )
		{
		}

		#region modifica by Dies Irae
		public bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted ) return false;

			Hue = sender.DyedHue;

			return true;
		}
		#endregion

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public abstract class BaseBagBall : BaseContainer, IDyable
	{
		public BaseBagBall( int itemID ) : base( itemID )
		{
			Weight = 1.0;
		}

		public BaseBagBall( Serial serial ) : base( serial )
		{
		}

		public bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted )
				return false;

			Hue = sender.DyedHue;

			return true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class SmallBagBall : BaseBagBall
	{
		[Constructable]
		public SmallBagBall() : base( 0x2256 )
		{
		}

		public SmallBagBall( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class LargeBagBall : BaseBagBall
	{
		[Constructable]
		public LargeBagBall() : base( 0x2257 )
		{
		}

		public LargeBagBall( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class Bag : BaseContainer, IDyable
	{
		[Constructable]
		public Bag() : base( 0xE76 )
		{
			Weight = 2.0;
		}

		public Bag( Serial serial ) : base( serial )
		{
		}

		public bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted ) return false;

			Hue = sender.DyedHue;

			return true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class Barrel : BaseContainer
	{
		[Constructable]
		public Barrel() : base( 0xE77 )
		{
			Weight = 25.0;
		}

		public Barrel( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( Weight == 0.0 )
				Weight = 25.0;
		}
	}

	public class Keg : BaseContainer
	{
		[Constructable]
		public Keg() : base( 0xE7F )
		{
			Weight = 15.0;
		}

		public Keg( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class PicnicBasket : BaseContainer
	{
		[Constructable]
		public PicnicBasket() : base( 0xE7A )
		{
			Weight = 2.0; // Stratics doesn't know weight
		}

		public PicnicBasket( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class Basket : BaseContainer
	{
		[Constructable]
		public Basket() : base( 0x990 )
		{
			Weight = 1.0; // Stratics doesn't know weight
		}

		public Basket( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	[Furniture]
	[Flipable( 0x9AA, 0xE7D )]
	public class WoodenBox : LockableContainer
	{
		[Constructable]
		public WoodenBox() : base( 0x9AA )
		{
			Weight = 4.0;
		}

		public WoodenBox( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	[Furniture]
	[Flipable( 0x9AA, 0xE7D )]
	public class TrapableExplosionWoodenBox : TrapableContainer
	{
		[Constructable]
		public TrapableExplosionWoodenBox() : base( 0x9AA )
		{
			Name = "trapable wooden box";
			Weight = 4.0;
		}

		public TrapableExplosionWoodenBox( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	[Furniture]
	[Flipable( 0x9A9, 0xE7E )]
	public class SmallCrate : LockableContainer
	{
		[Constructable]
		public SmallCrate() : base( 0x9A9 )
		{
			Weight = 2.0;
		}

		public SmallCrate( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( Weight == 4.0 )
				Weight = 2.0;
		}
	}

	[Furniture]
	[Flipable( 0xE3F, 0xE3E )]
	public class MediumCrate : LockableContainer
	{
		[Constructable]
		public MediumCrate() : base( 0xE3F )
		{
			Weight = 2.0;
		}

		public MediumCrate( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( Weight == 6.0 )
				Weight = 2.0;
		}
	}

	[Furniture]
	[Flipable( 0xE3D, 0xE3C )]
	public class LargeCrate : LockableContainer
	{
		[Constructable]
		public LargeCrate() : base( 0xE3D )
		{
			Weight = 1.0;
		}

		public LargeCrate( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( Weight == 8.0 )
				Weight = 1.0;
		}
	}

	[DynamicFliping]
	[Flipable( 0x9A8, 0xE80 )]
	public class MetalBox : LockableContainer
	{
		[Constructable]
		public MetalBox() : base( 0x9A8 )
		{
		}

		public MetalBox( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 && Weight == 3 )
				Weight = -1;
		}
	}

	[DynamicFliping]
	[Flipable( 0x9AB, 0xE7C )]
	public class MetalChest : LockableContainer
	{
		[Constructable]
		public MetalChest() : base( 0x9AB )
		{
		}

		public MetalChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 && Weight == 25 )
				Weight = -1;
		}
	}

	[DynamicFliping]
	[Flipable( 0xE41, 0xE40 )]
	public class MetalGoldenChest : LockableContainer
	{
		[Constructable]
		public MetalGoldenChest() : base( 0xE41 )
		{
		}

		public MetalGoldenChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 && Weight == 25 )
				Weight = -1;
		}
	}

	[Furniture]
	[Flipable( 0xe43, 0xe42 )]
	public class WoodenChest : LockableContainer
	{
		[Constructable]
		public WoodenChest() : base( 0xe43 )
		{
			Weight = 2.0;
		}

		public WoodenChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( Weight == 15.0 )
				Weight = 2.0;
		}
	}

	[Furniture]
	[Flipable( 0x280B, 0x280C )]
	public class PlainWoodenChest : LockableContainer
	{
		[Constructable]
		public PlainWoodenChest() : base( 0x280B )
		{
		}

		public PlainWoodenChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 && Weight == 15 )
				Weight = -1;
		}
	}

	[Furniture]
	[Flipable( 0x280D, 0x280E )]
	public class OrnateWoodenChest : LockableContainer
	{
		[Constructable]
		public OrnateWoodenChest() : base( 0x280D )
		{
		}

		public OrnateWoodenChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 && Weight == 15 )
				Weight = -1;
		}
	}

	[Furniture]
	[Flipable( 0x280F, 0x2810 )]
	public class GildedWoodenChest : LockableContainer
	{
		[Constructable]
		public GildedWoodenChest() : base( 0x280F )
		{
		}

		public GildedWoodenChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 && Weight == 15 )
				Weight = -1;
		}
	}

	[Furniture]
	[Flipable( 0x2811, 0x2812 )]
	public class WoodenFootLocker : LockableContainer
	{
		[Constructable]
		public WoodenFootLocker() : base( 0x2811 )
		{
			GumpID = 0x10B;
		}

		public WoodenFootLocker( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 && Weight == 15 )
				Weight = -1;
			
			if ( version < 2 )
				GumpID = 0x10B;
		}
	}

	[Furniture]
	[Flipable( 0x2813, 0x2814 )]
	public class FinishedWoodenChest : LockableContainer
	{
		[Constructable]
		public FinishedWoodenChest() : base( 0x2813 )
		{
		}

		public FinishedWoodenChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 && Weight == 15 )
				Weight = -1;
		}
	}
}