using System;
using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
	public class VeteranHangingSkeleton : Item, IAddon, IRewardItem
	{
		private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get { return m_IsRewardItem; }
			set { m_IsRewardItem = value; }
		}
		
		public override int LabelNumber { get { return 1065534; } } // hanging skeleton
		
		public override void AddNameProperty( ObjectPropertyList list )
		{
			if( m_IsRewardItem )
				list.Add( 1065529, GetNameString() ); // ~1_NAME~ (Midgard Veteran Reward)
			else if( Name == null )
				list.Add( LabelNumber );
			else
				list.Add( Name );
		}
		
		private string GetNameString()
		{
			string name = Name;

			if( name == null )
				name = String.Format( "#{0}", LabelNumber );

			return name;
		}

		[Constructable]
		public VeteranHangingSkeleton() : this( 0x1A02 )
		{
		}

		[Constructable]
		public VeteranHangingSkeleton( int itemID ) : base( itemID )
		{
			Movable = false;
		}

		public VeteranHangingSkeleton( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version

			writer.Write( m_IsRewardItem );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_IsRewardItem = reader.ReadBool();
			
			if( m_IsRewardItem )
				Hue = Utility.RandomNondyedHue();
		}

		public Item Deed
		{
			get
			{
				VeteranHangingSkeletonDeed deed = new VeteranHangingSkeletonDeed();
				deed.IsRewardItem = IsRewardItem;
				return deed;
			}
		}

		public bool CouldFit( IPoint3D p, Map map )
		{
			if (!map.CanFit( p.X, p.Y, p.Z, ItemData.Height ))
				return false;

			if (ItemID == 0x1A02 || ItemID == 0x1A03 || ItemID == 0x1A05)
				return BaseAddon.IsWall( p.X, p.Y - 1, p.Z, map ); // North wall
			else
				return BaseAddon.IsWall( p.X - 1, p.Y, p.Z, map ); // West wall
		} 

		public override void OnDoubleClick( Mobile from )
		{
			BaseHouse house = BaseHouse.FindHouseAt( this );

			if (house != null && house.IsOwner( from ))
			{
				if (from.InRange( GetWorldLocation(), 3 ))
				{
					from.CloseGump( typeof( DecoRedeedGump ) );
					from.SendGump( new DecoRedeedGump( this ) );
				}
				else
				{
					from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
				}
			}
		}
	}

	public class VeteranHangingSkeletonDeed : Item, IAddonTargetDeed, IRewardItem
	{
		private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get { return m_IsRewardItem; }
			set { m_IsRewardItem = value; }
		}

		public override int LabelNumber { get { return 1065535; } } // deed for a hanging skeleton

		[Constructable]
		public VeteranHangingSkeletonDeed() : base( 0x14F0 )
		{
			LootType = LootType.Blessed;
		}

		public VeteranHangingSkeletonDeed( Serial serial ) : base( serial )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			
			if( m_IsRewardItem )
				list.Add( 1065528 ); // Midgard Veteran Reward
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version

			writer.Write( m_IsRewardItem );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_IsRewardItem = reader.ReadBool();
			
			if( m_IsRewardItem )
				Hue = Utility.RandomNondyedHue();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if (IsChildOf( from.Backpack ))
			{
				BaseHouse house = BaseHouse.FindHouseAt( from );

				if (house != null && house.IsOwner( from ))
				{
					from.CloseGump( typeof( ChooseDecoGump ) );
					from.SendGump( new ChooseDecoGump( this, 0, 0x1A01, 0x1A05, "Hanging Skeleton" ) );
				}
				else
				{
					from.SendLocalizedMessage( 502092 ); // You must be in your house to do this.
				}
			}
			else
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
		}

		public int TargetLocalized { get { return 1049780; } } // Where would you like to place this decoration?

		public void Placement_OnTarget( Mobile from, object targeted, object state )
		{
			if (!IsChildOf( from.Backpack ))
				return;

			IPoint3D p = targeted as IPoint3D;

			if (p == null)
				return;

			Point3D loc = new Point3D( p );
			if (!from.Map.CanFit( loc.X, loc.Y, loc.Z, 1 ))
				return;

			BaseHouse house = BaseHouse.FindHouseAt( loc, from.Map, 16 );

			if (house != null && house.IsOwner( from ))
			{
				int itemID = (int)state;
				if ((itemID == 0x1A02 || itemID == 0x1A03 || itemID == 0x1A05) ? BaseAddon.IsWall( loc.X, loc.Y - 1, loc.Z, from.Map ) : BaseAddon.IsWall( p.X - 1, p.Y, p.Z, from.Map ))
				{
					VeteranHangingSkeleton skeleton = new VeteranHangingSkeleton( itemID );
					skeleton.IsRewardItem = IsRewardItem;
					skeleton.MoveToWorld( loc, from.Map );
					house.Addons.Add( skeleton );
					Delete();
				}
				else
					from.SendLocalizedMessage( 1062840 ); // The decoration must be placed next to a wall.
			}
			else
			{
				from.SendLocalizedMessage( 1042036 ); // That location is not in your house.
			}
		}
	}
}
