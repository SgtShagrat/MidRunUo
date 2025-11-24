using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
	public class VeteranDecoAnkh : BaseAddon, IRewardItem
	{
		private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get { return m_IsRewardItem; }
			set { m_IsRewardItem = value; }
		}

		public override BaseAddonDeed Deed
		{
			get
			{
				VeteranDecoAnkhDeed deed = new VeteranDecoAnkhDeed();
				deed.IsRewardItem = IsRewardItem;
				return deed;
			}
		}

		[Constructable]
		public VeteranDecoAnkh( bool east, bool isRewardItem )
		{
			if ( east )
			{
				if( isRewardItem )
				{
					AddComponent( new LocalizedAddonComponent( 0x2, 1065530 ), 0, 0, 0 ); 	// ankh (Midgard Veteran Reward)
					AddComponent( new LocalizedAddonComponent( 0x3, 1065530 ), 0, -1, 0 ); 	// ankh (Midgard Veteran Reward)
				}
				else
				{
					AddComponent( new AddonComponent( 0x2 ), 0, 0, 0 ); 	// ankh
					AddComponent( new AddonComponent( 0x3 ), 0, -1, 0 ); 	// ankh				
				}
			}
			else
			{
				if( isRewardItem )
				{
					AddComponent( new LocalizedAddonComponent( 0x5, 1065530 ), 0, 0, 0 );	// ankh (Midgard Veteran Reward)
					AddComponent( new LocalizedAddonComponent( 0x4, 1065530 ), -1, 0, 0 );	// ankh (Midgard Veteran Reward)
				}
				else
				{
					AddComponent( new AddonComponent( 0x5 ), 0, 0, 0 );		// ankh
					AddComponent( new AddonComponent( 0x4 ), -1, 0, 0 );	// ankh				
				}
			}
		}

		public VeteranDecoAnkh( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version

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
	}

	public class VeteranDecoAnkhDeed : BaseAddonDeed, IRewardItem
	{
		private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get { return m_IsRewardItem; }
			set { m_IsRewardItem = value; }
		}

		public override int LabelNumber { get { return 1065531; } } // deed for a stone ankh

		public bool m_East;

		public override BaseAddon Addon
		{
			get
			{
				VeteranDecoAnkh ankh = new VeteranDecoAnkh( m_East, IsRewardItem );
				ankh.IsRewardItem = IsRewardItem;
				return ankh;
			}
		}

		[Constructable]
		public VeteranDecoAnkhDeed()
		{
			LootType = LootType.Blessed;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			
			if( m_IsRewardItem )
				list.Add( 1065528 ); // Midgard Veteran Reward
		}
		
		public override void OnDoubleClick( Mobile from )
		{
			if (IsChildOf( from.Backpack ))
			{
				BaseHouse house = BaseHouse.FindHouseAt( from );

				if (house != null && house.IsOwner( from ))
				{
					from.CloseGump( typeof( InternalGump ) );
					from.SendGump( new InternalGump( this ) );
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

		private void SendTarget( Mobile m )
		{
			base.OnDoubleClick( m );
		}

		private class InternalGump : Gump
		{
			private VeteranDecoAnkhDeed m_Deed;

			public InternalGump( VeteranDecoAnkhDeed deed ) : base( 150, 50 )
			{
				Closable = false;

				m_Deed = deed;

				AddBackground( 0, 0, 350, 250, 0xA28 );

				AddItem( 90, 52, 0x4 );
				AddItem( 112, 52, 0x5 );
				AddButton( 70, 35, 0x868, 0x869, 1, GumpButtonType.Reply, 0 ); // South

				AddItem( 220, 52, 0x2 );
				AddItem( 242, 52, 0x3 );
				AddButton( 185, 35, 0x868, 0x869, 2, GumpButtonType.Reply, 0 ); // East
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				if ( m_Deed.Deleted )
					return;

				m_Deed.m_East = (info.ButtonID == 2);
				m_Deed.SendTarget( sender.Mobile );
			}
		}

		public VeteranDecoAnkhDeed( Serial serial ) : base( serial )
		{
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
	}
}
