using System;
using System.Collections;
using System.Collections.Generic;

using Midgard.Engines.MidgardTownSystem;

using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;

namespace Midgard.Engines.TownHouses
{
	public enum Intu
	{
		Neither,
		No,
		Yes
	}

	[Flipable( 0xBC5, 0xBC6 )]
	public class TownHouseSign : Item
	{
		private Point3D m_BanLoc;
		private List<DecoreItemInfo> m_DecoreItemInfos;
		private bool m_ForcePrivate, m_ForcePublic;
		private bool m_Free;
		private TownHouse m_House;
		private int m_ItemsPrice;
		private bool m_KeepItems, m_LeaveItems;
		private int m_Locks;
		private int m_MaxTotalSkill;
		private int m_MaxZ, m_MinTotalSkill;
		private int m_MinZ;
		private Intu m_Murderers;
		private bool m_NoBanning;
		private TimeSpan m_OriginalRentTime;
		private List<Item> m_PreviewItems;
		private Timer m_PreviewTimer;
		private int m_Price;
		private int m_RtoPayments;
		private bool m_RecurRent;
		private TimeSpan m_RentByTime;
		private Timer m_RentTimer;
		private bool m_RentToOwn;
		private int m_Secures;
		private Point3D m_SignLoc;
		private string m_Skill;
		private double m_SkillReq;
		private bool m_YoungOnly;

		[Constructable]
		public TownHouseSign() : base( 0xBC5 )
		{
			Name = "This building is for sale or rent!";
			Movable = false;

			m_BanLoc = Point3D.Zero;
			m_SignLoc = Point3D.Zero;
			m_Skill = "";
			Blocks = new List<Rectangle2D>();
			m_DecoreItemInfos = new List<DecoreItemInfo>();
			m_PreviewItems = new List<Item>();
			DemolishTime = DateTime.Now;
			RentTime = DateTime.Now;
			m_RentByTime = TimeSpan.Zero;
			m_RecurRent = true;

			m_MinZ = short.MinValue;
			m_MaxZ = short.MaxValue;

			AllSigns.Add( this );
		}

		public static List<TownHouseSign> AllSigns { get; private set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D BanLoc
		{
			get { return m_BanLoc; }
			set
			{
				m_BanLoc = value;
				InvalidateProperties();
				if( Owned )
					m_House.Region.GoLocation = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D SignLoc
		{
			get { return m_SignLoc; }
			set
			{
				m_SignLoc = value;
				InvalidateProperties();

				if( Owned )
				{
					m_House.Sign.Location = value;
					m_House.Hanger.Location = value;
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Locks
		{
			get { return m_Locks; }
			set
			{
				m_Locks = value;
				InvalidateProperties();
				if( Owned )
					m_House.MaxLockDowns = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Secures
		{
			get { return m_Secures; }
			set
			{
				m_Secures = value;
				InvalidateProperties();
				if( Owned )
					m_House.MaxSecures = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Price
		{
			get { return m_Price; }
			set
			{
				m_Price = value;
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MinZ
		{
			get { return m_MinZ; }
			set
			{
				if( value > m_MaxZ )
					m_MaxZ = value + 1;

				m_MinZ = value;
				if( Owned )
					RegionHelper.UpdateRegion( this );
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxZ
		{
			get { return m_MaxZ; }
			set
			{
				if( value < m_MinZ )
					value = m_MinZ;

				m_MaxZ = value;
				if( Owned )
					RegionHelper.UpdateRegion( this );
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MinTotalSkill
		{
			get { return m_MinTotalSkill; }
			set
			{
				if( value > m_MaxTotalSkill )
					value = m_MaxTotalSkill;

				m_MinTotalSkill = value;
				ValidateOwnership();
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxTotalSkill
		{
			get { return m_MaxTotalSkill; }
			set
			{
				if( value < m_MinTotalSkill )
					value = m_MinTotalSkill;

				m_MaxTotalSkill = value;
				ValidateOwnership();
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool YoungOnly
		{
			get { return m_YoungOnly; }
			set
			{
				m_YoungOnly = value;

				if( m_YoungOnly )
					m_Murderers = Intu.Neither;

				ValidateOwnership();
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan RentByTime
		{
			get { return m_RentByTime; }
			set
			{
				m_RentByTime = value;
				m_OriginalRentTime = value;

				if( value == TimeSpan.Zero )
					ClearRentTimer();
				else
				{
					ClearRentTimer();
					BeginRentTimer( value );
				}

				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool RecurRent
		{
			get { return m_RecurRent; }
			set
			{
				m_RecurRent = value;

				if( !value )
					m_RentToOwn = false;

				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool KeepItems
		{
			get { return m_KeepItems; }
			set
			{
				m_LeaveItems = false;
				m_KeepItems = value;
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Free
		{
			get { return m_Free; }
			set
			{
				m_Free = value;
				m_Price = 1;
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Intu Murderers
		{
			get { return m_Murderers; }
			set
			{
				m_Murderers = value;

				ValidateOwnership();
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool ForcePrivate
		{
			get { return m_ForcePrivate; }
			set
			{
				m_ForcePrivate = value;

				if( value )
				{
					m_ForcePublic = false;

					if( m_House != null )
						m_House.Public = false;
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool ForcePublic
		{
			get { return m_ForcePublic; }
			set
			{
				m_ForcePublic = value;

				if( value )
				{
					m_ForcePrivate = false;

					if( m_House != null )
						m_House.Public = true;
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool NoBanning
		{
			get { return m_NoBanning; }
			set
			{
				m_NoBanning = value;

				if( value && m_House != null )
					m_House.Bans.Clear();
			}
		}

		public List<Rectangle2D> Blocks { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public string Skill
		{
			get { return m_Skill; }
			set
			{
				m_Skill = value;
				ValidateOwnership();
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public double SkillReq
		{
			get { return m_SkillReq; }
			set
			{
				m_SkillReq = value;
				ValidateOwnership();
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool LeaveItems
		{
			get { return m_LeaveItems; }
			set
			{
				m_LeaveItems = value;
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool RentToOwn
		{
			get { return m_RentToOwn; }
			set
			{
				m_RentToOwn = value;
				InvalidateProperties();
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Relock { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool NoTrade { get; set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public int ItemsPrice
		{
			get { return m_ItemsPrice; }
			set
			{
				m_ItemsPrice = value;
				InvalidateProperties();
			}
		}

		public TownHouse House
		{
			get { return m_House; }
			set { m_House = value; }
		}

		public Timer DemolishTimer { get; private set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime DemolishTime { get; private set; }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Owned
		{
			get { return m_House != null && !m_House.Deleted; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Floors
		{
			get { return ( m_MaxZ - m_MinZ ) / 20 + 1; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool BlocksReady
		{
			get { return Blocks.Count != 0; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool FloorsReady
		{
			get { return ( BlocksReady && MinZ != short.MinValue ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool SignReady
		{
			get { return ( FloorsReady && SignLoc != Point3D.Zero ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool BanReady
		{
			get { return ( SignReady && BanLoc != Point3D.Zero ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool LocSecReady
		{
			get { return ( BanReady && Locks != 0 && Secures != 0 ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool ItemsReady
		{
			get { return LocSecReady; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool LengthReady
		{
			get { return ItemsReady; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool PriceReady
		{
			get { return ( LengthReady && Price != 0 ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string PriceType
		{
			get
			{
				if( m_RentByTime == TimeSpan.Zero )
					return "Sale";
				if( m_RentByTime == TimeSpan.FromDays( 1 ) )
					return "Daily";
				if( m_RentByTime == TimeSpan.FromDays( 7 ) )
					return "Weekly";
				if( m_RentByTime == TimeSpan.FromDays( 30 ) )
					return "Monthly";

				return "Sale";
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string PriceTypeShort
		{
			get
			{
				if( m_RentByTime == TimeSpan.Zero )
					return "Sale";
				if( m_RentByTime == TimeSpan.FromDays( 1 ) )
					return "Day";
				if( m_RentByTime == TimeSpan.FromDays( 7 ) )
					return "Week";
				if( m_RentByTime == TimeSpan.FromDays( 30 ) )
					return "Month";

				return "Sale";
			}
		}

		#region modifica by Dies Irae
		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime RentTime { get; private set; }

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
		public TownSystem System { get; set; }
		#endregion

		public void UpdateBlocks()
		{
			if( !Owned )
				return;

			if( Blocks.Count == 0 )
				UnconvertDoors();

			RegionHelper.UpdateRegion( this );
			ConvertItems( false );
			m_House.InitSectorDefinition();
		}

		public void ShowAreaPreview( Mobile m )
		{
			ClearPreview();

			List<Point2D> blocks = new List<Point2D>();

			foreach( Rectangle2D rect in Blocks )
			{
				for( int x = rect.Start.X; x < rect.End.X; ++x )
				{
					for( int y = rect.Start.Y; y < rect.End.Y; ++y )
					{
						Point2D point = new Point2D( x, y );
						if( !blocks.Contains( point ) )
							blocks.Add( point );
					}
				}
			}

			if( blocks.Count > 500 )
			{
				m.SendMessage( m.Language == "ITA" ? "Annullo la preview per via delle dimensioni." : "Due to size of the area, skipping the preview." );
				return;
			}

			Item item;
			foreach( Point2D p in blocks )
			{
				int avgz = Map.GetAverageZ( p.X, p.Y );

				item = new Item( 0x1766 );
				item.Name = "Area Preview";
				item.Movable = false;
				item.Location = new Point3D( p.X, p.Y, ( avgz <= m.Z ? m.Z + 2 : avgz + 2 ) );
				item.Map = Map;

				m_PreviewItems.Add( item );
			}

			m_PreviewTimer = Timer.DelayCall( TimeSpan.FromSeconds( 100 ), new TimerCallback( ClearPreview ) );
		}

		public void ShowSignPreview()
		{
			ClearPreview();

			Item sign = new Item( 0xBD2 );
			sign.Name = "Sign Preview";
			sign.Movable = false;
			sign.Location = SignLoc;
			sign.Map = Map;

			m_PreviewItems.Add( sign );

			sign = new Item( 0xB98 );
			sign.Name = "Sign Preview";
			sign.Movable = false;
			sign.Location = SignLoc;
			sign.Map = Map;

			m_PreviewItems.Add( sign );

			m_PreviewTimer = Timer.DelayCall( TimeSpan.FromSeconds( 100 ), new TimerCallback( ClearPreview ) );
		}

		public void ShowBanPreview()
		{
			ClearPreview();

			Item ban = new Item( 0x17EE );
			ban.Name = "Ban Loc Preview";
			ban.Movable = false;
			ban.Location = BanLoc;
			ban.Map = Map;

			m_PreviewItems.Add( ban );

			m_PreviewTimer = Timer.DelayCall( TimeSpan.FromSeconds( 100 ), new TimerCallback( ClearPreview ) );
		}

		public void ShowFloorsPreview( Mobile m )
		{
			ClearPreview();

			Item item = new Item( 0x7BD );
			item.Name = "Bottom Floor Preview";
			item.Movable = false;
			item.Location = m.Location;
			item.Z = m_MinZ;
			item.Map = Map;

			m_PreviewItems.Add( item );

			item = new Item( 0x7BD );
			item.Name = "Top Floor Preview";
			item.Movable = false;
			item.Location = m.Location;
			item.Z = m_MaxZ;
			item.Map = Map;

			m_PreviewItems.Add( item );

			m_PreviewTimer = Timer.DelayCall( TimeSpan.FromSeconds( 100 ), new TimerCallback( ClearPreview ) );
		}

		public void ClearPreview()
		{
			try
			{
				if( m_PreviewItems != null )
				{
					for( int i = m_PreviewItems.Count - 1; i >= 0; i-- )
					{
						Item item = m_PreviewItems[ i ];

						if( item != null )
							item.Delete();
					}
				}
			}
			catch( Exception ex )
			{
				Console.WriteLine( ex.ToString() );
				return;
			}

			if( m_PreviewTimer != null )
				m_PreviewTimer.Stop();

			m_PreviewTimer = null;
		}

		public void Purchase( Mobile m )
		{
			Purchase( m, false );
		}

		public void Purchase( Mobile m, bool sellitems )
		{
			try
			{
				if( Owned )
				{
					m.SendMessage( m.Language == "ITA" ? "Qualcuno già possiede questa proprietà!" : "Someone already owns this property!" );
					return;
				}

				if( !PriceReady )
				{
					m.SendMessage( m.Language == "ITA" ? "Il setup di questa proprietà non è completo." : "The setup for this property is not yet complete." );
					return;
				}

				int price = m_Price + ( sellitems ? m_ItemsPrice : 0 );

				if( m_Free )
					price = 0;

				if( m.AccessLevel == AccessLevel.Player && !Banker.Withdraw( m, price ) )
				{
					m.SendMessage( m.Language == "ITA" ? "Non ti puoi permettere questa proprietà." : "You cannot afford this property." );
					return;
				}

				if( m.AccessLevel == AccessLevel.Player )
					m.SendLocalizedMessage( 1060398, price.ToString() ); // ~1_AMOUNT~ gold has been withdrawn from your bank box.

				#region Midgard Town System
				if( System != null )
					System.RegisterTransaction( m_Price );
				#endregion

				Visible = false;

				int minX = Blocks[ 0 ].Start.X;
				int minY = Blocks[ 0 ].Start.Y;
				int maxX = Blocks[ 0 ].End.X;
				int maxY = Blocks[ 0 ].End.Y;

				foreach( Rectangle2D rect in Blocks )
				{
					if( rect.Start.X < minX )
						minX = rect.Start.X;
					if( rect.Start.Y < minY )
						minY = rect.Start.Y;
					if( rect.End.X > maxX )
						maxX = rect.End.X;
					if( rect.End.Y > maxY )
						maxY = rect.End.Y;
				}

				m_House = new TownHouse( m, this, m_Locks, m_Secures );

				m_House.Components.Resize( maxX - minX, maxY - minY );
				m_House.Components.Add( 0x520, m_House.Components.Width - 1, m_House.Components.Height - 1, -5 );

				m_House.Location = new Point3D( minX, minY, Map.GetAverageZ( minX, minY ) );
				m_House.Map = Map;
				m_House.Region.GoLocation = m_BanLoc;
				m_House.Sign.Location = m_SignLoc;
				m_House.Hanger = new Item( 0xB98 );
				m_House.Hanger.Location = m_SignLoc;
				m_House.Hanger.Map = Map;
				m_House.Hanger.Movable = false;

				if( m_ForcePublic )
					m_House.Public = true;

				m_House.Price = ( RentByTime == TimeSpan.FromDays( 0 ) ? m_Price : 1 );

				RegionHelper.UpdateRegion( this );

				if( m_House.Price == 0 )
					m_House.Price = 1;

				if( m_RentByTime != TimeSpan.Zero )
					BeginRentTimer( m_RentByTime );

				m_RtoPayments = 1;

				HideOtherSigns();

				m_DecoreItemInfos = new List<DecoreItemInfo>();

				ConvertItems( sellitems );

				m_House.ChangeLocks( m );
			}
			catch( Exception e )
			{
				Errors.Report( String.Format( "An error occurred during home purchasing.  More information available on the console." ) );
				Console.WriteLine( e.Message );
				Console.WriteLine( e.Source );
				Console.WriteLine( e.StackTrace );
			}
		}

		private void HideOtherSigns()
		{
			foreach( Item item in m_House.Sign.GetItemsInRange( 0 ) )
			{
				if( !( item is HouseSign ) )
				{
					if( item.ItemID == 0xB95 || item.ItemID == 0xB96 || item.ItemID == 0xC43 || item.ItemID == 0xC44 || ( item.ItemID > 0xBA3 && item.ItemID < 0xC0E ) )
						item.Visible = false;
				}
			}
		}

		public virtual void ConvertItems( bool keep )
		{
			if( m_House == null )
				return;

			List<Item> items = new List<Item>();
			foreach( Rectangle2D rect in Blocks )
			{
				foreach( Item item in Map.GetItemsInBounds( rect ) )
				{
					if( m_House.Region.Contains( item.Location ) && item.RootParent == null && !items.Contains( item ) )
						items.Add( item );
				}
			}

			foreach( Item item in items )
			{
				if( item is HouseSign || item is BaseMulti || item is BaseAddon || item is AddonComponent || item == m_House.Hanger || !item.Visible || item.IsLockedDown || item.IsSecure || item.Movable || m_PreviewItems.Contains( item ) )
					continue;

				if( item is BaseDoor )
					ConvertDoor( (BaseDoor)item );
				else if( !m_LeaveItems )
				{
					m_DecoreItemInfos.Add( new DecoreItemInfo( item.GetType().ToString(), item.Name, item.ItemID, item.Hue, item.Location, item.Map ) );

					if( !m_KeepItems || !keep )
						item.Delete();
					else
					{
						item.Movable = true;
						m_House.LockDown( m_House.Owner, item, false );
					}
				}
			}
		}

		protected void ConvertDoor( BaseDoor door )
		{
			if( !Owned )
				return;

			if( door is ISecurable )
			{
				door.Locked = false;
				m_House.Doors.Add( door );
				return;
			}

			door.Open = false;

			GenericHouseDoor newdoor = new GenericHouseDoor( 0, door.ClosedID, door.OpenedSound, door.ClosedSound );
			newdoor.Offset = door.Offset;
			newdoor.ClosedID = door.ClosedID;
			newdoor.OpenedID = door.OpenedID;
			newdoor.Location = door.Location;
			newdoor.Map = door.Map;

			door.Delete();

			foreach( Item inneritem in newdoor.GetItemsInRange( 1 ) )
			{
				if( inneritem is BaseDoor && inneritem != newdoor && inneritem.Z == newdoor.Z )
				{
					( (BaseDoor)inneritem ).Link = newdoor;
					newdoor.Link = (BaseDoor)inneritem;
				}
			}

			m_House.Doors.Add( newdoor );
		}

		public virtual void UnconvertDoors()
		{
			if( m_House == null )
				return;

			BaseDoor newdoor;

			foreach( BaseDoor door in new ArrayList( m_House.Doors ) )
			{
				door.Open = false;

				if( Relock )
					door.Locked = true;

				newdoor = new StrongWoodDoor( (DoorFacing)0 );
				newdoor.ItemID = door.ItemID;
				newdoor.ClosedID = door.ClosedID;
				newdoor.OpenedID = door.OpenedID;
				newdoor.OpenedSound = door.OpenedSound;
				newdoor.ClosedSound = door.ClosedSound;
				newdoor.Offset = door.Offset;
				newdoor.Location = door.Location;
				newdoor.Map = door.Map;

				door.Delete();

				foreach( Item inneritem in newdoor.GetItemsInRange( 1 ) )
				{
					if( inneritem is BaseDoor && inneritem != newdoor && inneritem.Z == newdoor.Z )
					{
						( (BaseDoor)inneritem ).Link = newdoor;
						newdoor.Link = (BaseDoor)inneritem;
					}
				}

				m_House.Doors.Remove( door );
			}
		}

		public void RecreateItems()
		{
			Item item;
			foreach( DecoreItemInfo info in m_DecoreItemInfos )
			{
				if( info.TypeString.ToLower().IndexOf( "static" ) != -1 )
					item = new Static( info.ItemID );
				else
				{
					try
					{
						item = Activator.CreateInstance( ScriptCompiler.FindTypeByFullName( info.TypeString ) ) as Item;
					}
					catch
					{
						continue;
					}
				}

				if( item == null )
					continue;

				item.ItemID = info.ItemID;
				item.Name = info.Name;
				item.Hue = info.Hue;
				item.Location = info.Location;
				item.Map = info.Map;
				item.Movable = false;
			}
		}

		public virtual void ClearHouse()
		{
			UnconvertDoors();
			ClearDemolishTimer();
			ClearRentTimer();
			PackUpItems();
			RecreateItems();
			m_House = null;
			Visible = true;

			if( m_RentToOwn )
				m_RentByTime = m_OriginalRentTime;
		}

		public virtual void ValidateOwnership()
		{
			if( !Owned )
				return;

			if( m_House.Owner == null && BaseHouse.DecayEnabled )
			{
				m_House.Delete();
				return;
			}

			if( m_House.Owner.AccessLevel != AccessLevel.Player )
				return;

			if( !CanBuyHouse( m_House.Owner ) && DemolishTimer == null && BaseHouse.DecayEnabled )
				BeginDemolishTimer();
			else
				ClearDemolishTimer();
		}

		public int CalcVolume()
		{
			int floors = 1;
			if( m_MaxZ - m_MinZ < 100 )
				floors = 1 + Math.Abs( ( m_MaxZ - m_MinZ ) / 20 );

			List<Point3D> blocks = new List<Point3D>();

			foreach( Rectangle2D rect in Blocks )
			{
				for( int x = rect.Start.X; x < rect.End.X; ++x )
				{
					for( int y = rect.Start.Y; y < rect.End.Y; ++y )
					{
						for( int z = 0; z < floors; z++ )
						{
							Point3D point = new Point3D( x, y, z );
							if( !blocks.Contains( point ) )
								blocks.Add( point );
						}
					}
				}
			}
			return blocks.Count;
		}

		/*private void StartTimers(bool startRentTimerToo)
		{
			if( DemolishTime > DateTime.Now )
				BeginDemolishTimer( DemolishTime - DateTime.Now );
			else if(startRentTimerToo && m_RentByTime != TimeSpan.Zero )
				BeginRentTimer( m_RentByTime );
		}*/

		public virtual bool CanBuyHouse( Mobile m )
		{
			if( m_Skill != "" )
			{
				try
				{
					SkillName index = (SkillName)Enum.Parse( typeof( SkillName ), m_Skill, true );
					if( m.Skills[ index ].Value < m_SkillReq )
						return false;
				}
				catch
				{
					return false;
				}
			}

			if( m_MinTotalSkill != 0 && m.SkillsTotal / 10 < m_MinTotalSkill )
				return false;

			if( m_MaxTotalSkill != 0 && m.SkillsTotal / 10 > m_MaxTotalSkill )
				return false;

			if( m_YoungOnly && m.Player && !( (PlayerMobile)m ).Young )
				return false;

			if( m_Murderers == Intu.Yes && m.Kills < 5 )
				return false;

			if( m_Murderers == Intu.No && m.Kills >= 5 )
				return false;

			return true;
		}

		public override void OnDoubleClick( Mobile m )
		{
			if( m.AccessLevel != AccessLevel.Player )
				new TownHouseSetupGump( m, this );
			else if( !Visible ) // se non e' visible allora e' gia' owned
				return;
			#region midgard town system
			else if( !TownSystem.CanBuyTownHouse( m, this ) )
				return;
			#endregion

			else if( CanBuyHouse( m ) /*&& !BaseHouse.HasAccountHouse( m )*/)
				new TownHouseConfirmGump( m, this );
			else
				m.SendMessage( m.Language == "ITA" ? "Non puoi comprare questa casa." : "You cannot purchase this house." );
		}

		public override void Delete()
		{
			if( m_House == null || m_House.Deleted )
				base.Delete();
			else
				PublicOverheadMessage( MessageType.Regular, 0x0, true, "You cannot delete this while the home is owned." );

			if( Deleted )
				AllSigns.Remove( this );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if( m_Free )
				list.Add( 1060658, "Price\tFree" );
			else if( m_RentByTime == TimeSpan.Zero )
				list.Add( 1060658, "Price\t{0}{1}", m_Price, m_KeepItems ? " (+" + m_ItemsPrice + " for the items)" : "" );
			else if( m_RecurRent )
				list.Add( 1060658, "{0}\t{1}\r{2}", PriceType + ( m_RentToOwn ? " Rent-to-Own" : " Recurring" ), m_Price, m_KeepItems ? " (+" + m_ItemsPrice + " for the items)" : "" );
			else
				list.Add( 1060658, "One {0}\t{1}{2}", PriceTypeShort, m_Price, m_KeepItems ? " (+" + m_ItemsPrice + " for the items)" : "" );

			list.Add( 1060659, "Lockdowns\t{0}", m_Locks );
			list.Add( 1060660, "Secures\t{0}", m_Secures );

			if( m_SkillReq != 0.0 )
				list.Add( 1060661, "Requires\t{0}", m_SkillReq + " in " + m_Skill );
			if( m_MinTotalSkill != 0 )
				list.Add( 1060662, "Requires more than\t{0} total skills", m_MinTotalSkill );
			if( m_MaxTotalSkill != 0 )
				list.Add( 1060663, "Requires less than\t{0} total skills", m_MaxTotalSkill );

			if( m_YoungOnly )
				list.Add( 1063483, "Must be\tYoung" );
			else if( m_Murderers == Intu.Yes )
				list.Add( 1063483, "Must be\ta murderer" );
			else if( m_Murderers == Intu.No )
				list.Add( 1063483, "Must be\tinnocent" );
		}

		#region serialization
		static TownHouseSign()
		{
			AllSigns = new List<TownHouseSign>();
		}

		public TownHouseSign( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 14 );

			// Version 14
			if( System != null )
				writer.Write( (int)System.Definition.Town );
			else
				writer.Write( (int)MidgardTowns.None );

			// Version 13

			writer.Write( m_ForcePrivate );
			writer.Write( m_ForcePublic );
			writer.Write( NoTrade );

			// Version 12

			writer.Write( m_Free );

			// Version 11

			writer.Write( (int)m_Murderers );

			// Version 10

			writer.Write( m_LeaveItems );

			// Version 9
			writer.Write( m_RentToOwn );
			writer.Write( m_OriginalRentTime );
			writer.Write( m_RtoPayments );

			// Version 7
			writer.WriteItemList( m_PreviewItems, true );

			// Version 6
			writer.Write( m_ItemsPrice );
			writer.Write( m_KeepItems );

			// Version 5
			writer.Write( m_DecoreItemInfos.Count );
			foreach( DecoreItemInfo info in m_DecoreItemInfos )
				info.Serialize( writer );

			writer.Write( Relock );

			// Version 4
			writer.Write( m_RecurRent );
			writer.Write( m_RentByTime );
			writer.Write( RentTime );
			writer.Write( DemolishTime );
			writer.Write( m_YoungOnly );
			writer.Write( m_MinTotalSkill );
			writer.Write( m_MaxTotalSkill );

			// Version 3
			writer.Write( m_MinZ );
			writer.Write( m_MaxZ );

			// Version 2
			writer.Write( m_House );

			// Version 1
			writer.Write( m_Price );
			writer.Write( m_Locks );
			writer.Write( m_Secures );
			writer.Write( m_BanLoc );
			writer.Write( m_SignLoc );
			writer.Write( m_Skill );
			writer.Write( m_SkillReq );
			writer.Write( Blocks.Count );
			foreach( Rectangle2D rect in Blocks )
				writer.Write( rect );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if( version >= 14 )
				System = TownSystem.Find( (MidgardTowns)reader.ReadInt() );

			if( version >= 13 )
			{
				m_ForcePrivate = reader.ReadBool();
				m_ForcePublic = reader.ReadBool();
				NoTrade = reader.ReadBool();
			}

			if( version >= 12 )
				m_Free = reader.ReadBool();

			if( version >= 11 )
				m_Murderers = (Intu)reader.ReadInt();

			if( version >= 10 )
				m_LeaveItems = reader.ReadBool();

			if( version >= 9 )
			{
				m_RentToOwn = reader.ReadBool();
				m_OriginalRentTime = reader.ReadTimeSpan();
				m_RtoPayments = reader.ReadInt();
			}

			m_PreviewItems = new List<Item>();
			if( version >= 7 )
				m_PreviewItems = reader.ReadStrongItemList();

			if( version >= 6 )
			{
				m_ItemsPrice = reader.ReadInt();
				m_KeepItems = reader.ReadBool();
			}

			m_DecoreItemInfos = new List<DecoreItemInfo>();
			if( version >= 5 )
			{
				int decorecount = reader.ReadInt();
				DecoreItemInfo info;
				for( int i = 0; i < decorecount; ++i )
				{
					info = new DecoreItemInfo();
					info.Deserialize( reader );
					m_DecoreItemInfos.Add( info );
				}

				Relock = reader.ReadBool();
			}

			if( version >= 4 )
			{
				m_RecurRent = reader.ReadBool();
				m_RentByTime = reader.ReadTimeSpan();
				RentTime = reader.ReadDateTime();
				DemolishTime = reader.ReadDateTime();
				m_YoungOnly = reader.ReadBool();
				m_MinTotalSkill = reader.ReadInt();
				m_MaxTotalSkill = reader.ReadInt();
			}

			if( version >= 3 )
			{
				m_MinZ = reader.ReadInt();
				m_MaxZ = reader.ReadInt();
			}

			if( version >= 2 )
				m_House = (TownHouse)reader.ReadItem();

			m_Price = reader.ReadInt();
			m_Locks = reader.ReadInt();
			m_Secures = reader.ReadInt();
			m_BanLoc = reader.ReadPoint3D();
			m_SignLoc = reader.ReadPoint3D();
			m_Skill = reader.ReadString();
			m_SkillReq = reader.ReadDouble();

			Blocks = new List<Rectangle2D>();
			int count = reader.ReadInt();
			for( int i = 0; i < count; ++i )
				Blocks.Add( reader.ReadRect2D() );
			
#region mod by Magius(CHE)
			// I timer vengono avviati in maniera corretta, tuttavia subito dopo veniva riavviato nuovamente
			// il timer e settato come nuovo (cio e' a un mese se il rectcycle era di un mese o ad un giorno se era giornaliero)
			// La modifica ora fa sì che la funzione StartTimers avvii solo il DemolishTimer e non il RentTimer se 
			// nel salvataggio vi e' gia' un certo renttime settato (sono passati tot giorni per esempio).		
			Timer.DelayCall( TimeSpan.FromSeconds( 30 ),
			delegate
			{
				bool isPartialTime = RentTime > DateTime.Now;

				if( isPartialTime )
					BeginRentTimer( RentTime - DateTime.Now ); //il prelievo viene fatto considerando il tempo salvato
				else
				{
					if( Owned && !m_Free )
						m_RentTimer = Timer.DelayCall( TimeSpan.Zero, new TimerCallback( RentDue ) ); // il prelievo viene fatto immediatamente.
					//BeginRentTimer( TimeSpan.Zero ); 
				}

				/*
				if( BaseHouse.DecayEnabled && DemolishTime > DateTime.Now )
					BeginDemolishTimer( DemolishTime - DateTime.Now );
				*/

				//StartTimers( false ); // non parte ma
			});
#endregion

			ClearPreview();

			AllSigns.Add( this );

			#region midgard town system
			if( ItemID == 0xC0B )
				ItemID = 0xBC5;
			else if( ItemID == 0xC0C )
				ItemID = 0xBC6;

			TownSystem.CheckTownHouse( this );
			#endregion
		}
		#endregion

		#region Demolish
		public void ClearDemolishTimer()
		{
			if( DemolishTimer == null )
				return;

			DemolishTimer.Stop();
			DemolishTimer = null;
			DemolishTime = DateTime.Now;

			if( !m_House.Deleted && Owned )
				m_House.Owner.SendMessage( m_House.Owner.Language == "ITA" ? "Demolizione annullata." : "Demolition canceled." );
		}

		public void CheckDemolishTimer()
		{
			if( DemolishTimer == null || !Owned )
				return;

			// DemolishAlert();
		}

		protected void BeginDemolishTimer()
		{
			BeginDemolishTimer( TimeSpan.FromHours( 24 ) );
		}

		protected void BeginDemolishTimer( TimeSpan time )
		{
			if( !Owned || !BaseHouse.DecayEnabled )
				return;

			DemolishTime = DateTime.Now + time;
			DemolishTimer = Timer.DelayCall( time, new TimerCallback( PackUpHouse ) );

			// DemolishAlert();
		}

		protected virtual void DemolishAlert()
		{
			m_House.Owner.SendMessage( m_House.Owner.Language == "ITA" ? "Non possiedi più i requisiti necessari per la tua casa, sarà demolita automaticamente in {0}:{1}:{2}." : "You no longer meet the requirements for your town house, which will be demolished automatically in {0}:{1}:{2}.", ( DemolishTime - DateTime.Now ).Hours, ( DemolishTime - DateTime.Now ).Minutes, ( DemolishTime - DateTime.Now ).Seconds );
		}

		public void PackUpHouse()
		{
			if( !Owned || m_House.Deleted )
				return;

			PackUpItems();

			m_House.Owner.BankBox.DropItem( new BankCheck( m_House.Price ) );

			try
			{
				m_House.Delete();
			}
			catch
			{
				Errors.Report( "The infamous SVN bug has occured." );
			}
		}

		protected void PackUpItems()
		{
			if( m_House == null )
				return;

			Container bag = new Bag();
			bag.Name = "Town House Belongings";

			foreach( Item item in new ArrayList( m_House.LockDowns ) )
			{
				item.IsLockedDown = false;
				item.Movable = true;
				m_House.LockDowns.Remove( item );
				bag.DropItem( item );
			}

			foreach( SecureInfo info in new ArrayList( m_House.Secures ) )
			{
				info.Item.IsLockedDown = false;
				info.Item.IsSecure = false;
				info.Item.Movable = true;
				info.Item.SetLastMoved();
				m_House.Secures.Remove( info );
				bag.DropItem( info.Item );
			}

			foreach( Rectangle2D rect in Blocks )
			{
				List<Item> l = new List<Item>();
				foreach( Item item in Map.GetItemsInBounds( rect ) )
					l.Add( item );

				foreach( Item item in l )
				{
					if( item is HouseSign || item is BaseDoor || item is BaseMulti || item is BaseAddon || item is AddonComponent || !item.Visible || item.IsLockedDown || item.IsSecure || !item.Movable || item.Map != m_House.Map || !m_House.Region.Contains( item.Location ) )
						continue;

					bag.DropItem( item );
				}
			}

			if( bag.Items.Count == 0 )
			{
				bag.Delete();
				return;
			}

			m_House.Owner.BankBox.DropItem( bag );
		}
		#endregion

		#region Rent
		public void ClearRentTimer()
		{
			if( m_RentTimer != null )
			{
				m_RentTimer.Stop();
				m_RentTimer = null;
			}

			RentTime = DateTime.Now;
		}

		private static string RentLog = "rentStatus.log";

		private void BeginRentTimer( TimeSpan time )
		{
			if( !Owned )
				return;

			m_RentTimer = Timer.DelayCall( time, new TimerCallback( RentDue ) );
			RentTime = DateTime.Now + time;

			string owner = ( m_House != null && m_House.Owner != null ) ? m_House.Owner.Name : "none";

			Utility.Log( RentLog, "Sign: {3}, Owner: {0} - NextCheck: {1}, New RentTime {2}", owner, time.ToString() , RentTime.ToString(), this );
		}

		public void CheckRentTimer()
		{
			if( m_RentTimer == null || !Owned )
				return;

			m_House.Owner.SendMessage( m_House.Owner.Language == "ITA" ? "L'affitto verrà riscosso in {0} giorni, {1}:{2}:{3}." : "This rent cycle ends in {0} days, {1}:{2}:{3}.", ( RentTime - DateTime.Now ).Days, ( RentTime - DateTime.Now ).Hours, ( RentTime - DateTime.Now ).Minutes, ( RentTime - DateTime.Now ).Seconds );
		}

		private void RentDue()
		{
			if( !Owned || m_House.Owner == null )//|| !BaseHouse.DecayEnabled ) edit by arlas
				return;			 

			string owner = m_House != null ? m_House.Owner.Name : "none";
			Utility.Log( RentLog, " {0} - RentDue per Owner: {1} - RentTime: {2}", DateTime.Now.ToString(), owner, RentTime.ToString() );

			if( !m_RecurRent )
			{

				if( m_House != null )
					m_House.Owner.SendMessage( m_House.Owner.Language == "ITA" ? "Il tuo contratto di affitto è scaduto, la banca ne ha preso il possesso." : "Your town house rental contract has expired, and the bank has once again taken possession." );

				PackUpHouse();

				Utility.Log( RentLog, "\t{0}", "Il tuo contratto di affitto è scaduto, la banca ne ha preso il possesso.");
				return;
			}

			if( m_House != null )
			{
				if( !m_Free && m_House.Owner.AccessLevel == AccessLevel.Player && !Banker.Withdraw( m_House.Owner, m_Price ) )
				{
					Utility.Log( RentLog, "\tNon hai abbastanza fondi per sostenere l'affitto, la banca ha reclamato la tua proprietà.");
					Console.WriteLine( "Notice: packed {0} house.", owner );

					if( m_House.Owner != null )
						m_House.Owner.SendMessage( m_House.Owner.Language == "ITA" ? "Non hai abbastanza fondi per sostenere l'affitto, la banca ha reclamato la tua proprietà." : "Since you can not afford the rent, the bank has reclaimed your town house." );
					PackUpHouse();

					return;
				}
				else//edit per soldi recursivo
				{
					RentTime += m_RentByTime;

					Utility.Log( RentLog, "\tLa banca ha rinnovato la casa.");

					if( RentTime < DateTime.Now && Owned && !m_Free )
					{
						if( m_House.Owner != null )
							m_House.Owner.SendMessage( m_House.Owner.Language == "ITA" ? "La banca ha rinnovato il vostro contratto ricorsivamente." : "The bank has renowed your town house recursively.");

						Utility.Log( RentLog, "\tLa banca ha rinnovato la casa di {0} ricorsivamente: {1}, {2}.", owner, RentTime.ToString(), m_Price);

						m_RentTimer = Timer.DelayCall( TimeSpan.Zero, new TimerCallback( RentDue ) ); // il prelievo viene fatto immediatamente.

						return;
					}
				}

				if( !m_Free )
				{
					if( m_House.Owner != null )
						m_House.Owner.SendMessage( m_House.Owner.Language == "ITA" ? "\tLa banca ha riscosso {0} oro di affitto per la tua proprietà cittadina.": "The bank has withdrawn {0} gold rent for your town house.", m_Price );

					Utility.Log( RentLog, "\tLa banca ha riscosso {0} oro di affitto per la tua proprietà cittadina.", m_Price.ToString() );
				}
			}

			OnRentPaid();

			if( m_RentToOwn )
			{
				m_RtoPayments++;

				bool complete = false;

				if( m_RentByTime == TimeSpan.FromDays( 1 ) && m_RtoPayments >= 60 )
				{
					complete = true;
					if( m_House != null )
						m_House.Price = m_Price * 60;
				}

				if( m_RentByTime == TimeSpan.FromDays( 7 ) && m_RtoPayments >= 9 )
				{
					complete = true;
					if( m_House != null )
						m_House.Price = m_Price * 9;
				}

				if( m_RentByTime == TimeSpan.FromDays( 30 ) && m_RtoPayments >= 2 )
				{
					complete = true;
					if( m_House != null )
						m_House.Price = m_Price * 2;
				}

				if( complete )
				{
					if( m_House != null )
						m_House.Owner.SendMessage( "You now own your rental home." );
					m_RentByTime = TimeSpan.FromDays( 0 );
					return;
				}
			}

			BeginRentTimer( m_RentByTime );
		}

		protected virtual void OnRentPaid()
		{
			#region Midgard Town System
			if( System != null )
				System.RegisterTransaction( m_Price );
			#endregion
		}

		public void NextPriceType()
		{
			if( m_RentByTime == TimeSpan.Zero )
				RentByTime = TimeSpan.FromDays( 1 );
			else if( m_RentByTime == TimeSpan.FromDays( 1 ) )
				RentByTime = TimeSpan.FromDays( 7 );
			else if( m_RentByTime == TimeSpan.FromDays( 7 ) )
				RentByTime = TimeSpan.FromDays( 30 );
			else
				RentByTime = TimeSpan.Zero;
		}

		public void PrevPriceType()
		{
			if( m_RentByTime == TimeSpan.Zero )
				RentByTime = TimeSpan.FromDays( 30 );
			else if( m_RentByTime == TimeSpan.FromDays( 30 ) )
				RentByTime = TimeSpan.FromDays( 7 );
			else if( m_RentByTime == TimeSpan.FromDays( 7 ) )
				RentByTime = TimeSpan.FromDays( 1 );
			else
				RentByTime = TimeSpan.Zero;
		}
		#endregion
	}
}