using System;
using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
	public class VeteranHangingShield : Item, IAddon, IRewardItem
	{
		private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get { return m_IsRewardItem; }
			set { m_IsRewardItem = value; }
		}

		public override int LabelNumber { get { return 1065536; } } // decorative shield wall hanging
		
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
		public VeteranHangingShield() : this( 0x156C )
		{
		}

		[Constructable]
		public VeteranHangingShield( int itemID ) : base( itemID )
		{
			Movable = false;
		}

		public VeteranHangingShield( Serial serial ) : base( serial )
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
				VeteranHangingShieldDeed deed = new VeteranHangingShieldDeed();
				deed.IsRewardItem = IsRewardItem;
				return deed;
			}
		}

		public bool CouldFit( IPoint3D p, Map map )
		{
			if (!map.CanFit( p.X, p.Y, p.Z, ItemData.Height ))
				return false;

			if ((ItemID & 0x1) == 0x0)
				return (BaseAddon.IsWall( p.X, p.Y - 1, p.Z, map ) && BaseAddon.IsWall( p.X - 1, p.Y - 1, p.Z, map )); // North wall + Upper wall
			else
				return (BaseAddon.IsWall( p.X - 1, p.Y, p.Z, map ) && BaseAddon.IsWall( p.X - 1, p.Y - 1, p.Z, map )); // West wall + Upper wall
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

	public class VeteranHangingShieldDeed : Item, IAddonTargetDeed, IRewardItem
	{
		private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get { return m_IsRewardItem; }
			set { m_IsRewardItem = value; }
		}

		public override int LabelNumber { get { return 1065537; } } // deed for a decorative shield wall hanging

		[Constructable]
		public VeteranHangingShieldDeed()	: base( 0x14F0 )
		{
			LootType = LootType.Blessed;
		}

		public VeteranHangingShieldDeed( Serial serial ) : base( serial )
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

			writer.WriteEncodedInt( (int)0 ); // version

			writer.Write( m_IsRewardItem );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_IsRewardItem = reader.ReadBool();
		}

		public override void OnDoubleClick( Mobile from )
		{
			if (IsChildOf( from.Backpack ))
			{
				BaseHouse house = BaseHouse.FindHouseAt( from );

				if (house != null && house.IsOwner( from ))
				{
					from.CloseGump( typeof( ChooseDecoGump ) );
					from.SendGump( new ChooseDecoGump( this, 0, 0x156C, 0x1580, "Decorative Shield Wall Hanging" ) );
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
			if (!from.Map.CanFit( loc.X, loc.Y, loc.Z, 1 )) // TODO: Height
				return;

			BaseHouse house = BaseHouse.FindHouseAt( loc, from.Map, 16 );

			if (house != null && house.IsOwner( from ))
			{
				int itemID = (int)state;
				if (BaseAddon.IsWall( loc.X - 1, loc.Y - 1, loc.Z, from.Map ) && ((itemID & 0x1) == 0x0 ? BaseAddon.IsWall( loc.X, loc.Y - 1, loc.Z, from.Map ) : BaseAddon.IsWall( p.X - 1, p.Y, p.Z, from.Map )))
				{
					VeteranHangingShield shield = new VeteranHangingShield( itemID );
					shield.IsRewardItem = IsRewardItem;
					shield.MoveToWorld( loc, from.Map );
					house.Addons.Add( shield );
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
