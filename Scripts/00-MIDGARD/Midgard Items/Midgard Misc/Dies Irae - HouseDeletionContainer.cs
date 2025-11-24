using System;
using System.Collections.Generic;
using System.Text;
using Server;
using Server.Items;
using Server.Multis;
using Server.Network;

namespace Midgard.Items
{
	public class HouseDeletionContainer : Container
	{
		private Mobile m_HouseOwner;

		public override int DefaultMaxItems{ get{ return 0; } }
		public override int DefaultMaxWeight{ get{ return 0; } }

		public HouseDeletionContainer( Mobile owner ) : base( 0xE3D )
		{
			Hue = Utility.RandomMetalHue();
		    m_HouseOwner = owner;
		}

		public HouseDeletionContainer( Serial serial ) : base( serial )
		{
		}

		public override bool IsDecoContainer
		{
			get{ return false; }
		}

		public override bool CheckHold( Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight )
		{
			if ( m.AccessLevel < AccessLevel.GameMaster )
			{
				m.SendMessage( "You cannot place items into a house deletion crate." );
				return false;
			}

			return base.CheckHold( m, item, message, checkItems, plusItems, plusWeight );
		}

		public override bool CheckLift( Mobile from, Item item, ref LRReason reject )
		{
			return base.CheckLift( from, item, ref reject ) && ( m_HouseOwner != null && m_HouseOwner == from );
		}

		public override bool CheckItemUse( Mobile from, Item item )
		{
			return base.CheckItemUse( from, item ) && ( m_HouseOwner != null && m_HouseOwner == from );
		}

        public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 );

			writer.WriteMobile( m_HouseOwner );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_HouseOwner = reader.ReadMobile() as Mobile;
		}
	}
}
