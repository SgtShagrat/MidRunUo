using System;
using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public class VeteranFlamingHead : StoneFaceTrapNoDamage, IAddon, IRewardItem
	{
		private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get { return m_IsRewardItem; }
			set { m_IsRewardItem = value; }
		}

		public override int LabelNumber { get { return 1065538; } } // flaming head
		
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
        public VeteranFlamingHead()
        {
        }

        public VeteranFlamingHead(Serial serial) : base(serial)
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
				VeteranFlamingSkullDeed deed = new VeteranFlamingSkullDeed();
				deed.IsRewardItem = IsRewardItem;
				return deed;
			}
        }

		public bool CouldFit( IPoint3D p, Map map )
		{
			if (!map.CanFit( p.X, p.Y, p.Z, ItemData.Height ))
				return false;

			if (Type == StoneFaceTrapType.NorthWall)
				return BaseAddon.IsWall( p.X, p.Y - 1, p.Z, map ); // North wall
			else if (Type == StoneFaceTrapType.WestWall)
				return BaseAddon.IsWall( p.X - 1, p.Y, p.Z, map ); // West wall
			else
				return BaseAddon.IsWall( p.X, p.Y - 1, p.Z, map ) && BaseAddon.IsWall( p.X - 1, p.Y, p.Z, map );
		} 

        public override void OnDoubleClick(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && house.IsOwner(from))
            {
                if (from.InRange(this.GetWorldLocation(), 3))
                {
                    from.CloseGump(typeof(DecoRedeedGump));
                    from.SendGump(new DecoRedeedGump(this));
                }
                else
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                }
            }
        }
    }

	public class VeteranFlamingSkullDeed : Item, IAddonTargetDeed, IRewardItem
	{
		private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get { return m_IsRewardItem; }
			set { m_IsRewardItem = value; }
		}

        public override int LabelNumber { get { return 1065539; } } // deed for a flaming head

        [Constructable]
        public VeteranFlamingSkullDeed() : base( 0x14F0 )
        {
            LootType = LootType.Blessed;
        }

        public VeteranFlamingSkullDeed(Serial serial) : base(serial)
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

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                BaseHouse house = BaseHouse.FindHouseAt(from);

                if (house != null && house.IsOwner(from))
                {
					from.CloseGump( typeof( FlamingHeadFacingGump ) );
					from.SendGump( new FlamingHeadFacingGump( this ) );
                }
                else
                {
                    from.SendLocalizedMessage(502092); // You must be in your house to do this.
                }
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

		public int TargetLocalized { get { return 1042264; } } // Where would you like to place this head?

        public void Placement_OnTarget(Mobile from, object targeted, object state)
        {
            if (!IsChildOf(from.Backpack))
                return;

            IPoint3D p = targeted as IPoint3D;

            if (p == null)
                return;

            Point3D loc = new Point3D(p);
			if (!from.Map.CanFit( loc.X, loc.Y, loc.Z, 1 ))
				return;

            BaseHouse house = BaseHouse.FindHouseAt(loc, from.Map, 16);

            if (house != null && house.IsOwner(from))
            {
				int itemID = (int)state;
				if ((itemID & 0x1) == 0x0 ? BaseAddon.IsWall( loc.X, loc.Y - 1, loc.Z, from.Map ) : BaseAddon.IsWall( p.X - 1, p.Y, p.Z, from.Map ))
				{
                    VeteranFlamingHead head = new VeteranFlamingHead();
					head.ItemID = itemID;
					head.IsRewardItem = IsRewardItem;
                    head.MoveToWorld(loc, from.Map);
                    house.Addons.Add(head);
                    Delete();
                }
                else
                    from.SendLocalizedMessage(1042266); // The head must be placed next to a wall.
            }
            else
            {
                from.SendLocalizedMessage(1042036); // That location is not in your house.
            }
		}

		private class FlamingHeadFacingGump : Gump
		{
			private IAddonTargetDeed m_Deed;

			public FlamingHeadFacingGump( IAddonTargetDeed deed ) : base( 150, 50 )
			{
				m_Deed = deed;
				Closable = false;

				AddBackground( 0, 2, 300, 150, 2600 );
				AddButton( 50, 40, 2151, 2153, 0x110F, GumpButtonType.Reply, 0 );
				AddItem( 90, 35, 0x110F );
				AddButton( 150, 40, 2151, 2153, 0x10FC, GumpButtonType.Reply, 0 );
				AddItem( 180, 35, 0x10FC );
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				if (info.ButtonID != 0x10FC && info.ButtonID != 0x110F)
					return;

				sender.Mobile.SendLocalizedMessage( m_Deed.TargetLocalized );
				sender.Mobile.BeginTarget( -1, true, TargetFlags.None, new TargetStateCallback( m_Deed.Placement_OnTarget ), info.ButtonID );
			}
		}
    }
}
