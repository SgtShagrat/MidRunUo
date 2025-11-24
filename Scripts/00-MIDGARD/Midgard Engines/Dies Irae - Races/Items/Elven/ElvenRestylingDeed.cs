/***************************************************************************
 *                                  ElvenHairRestylingDeed.cs
 *                            		-------------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info: 
 * 			Deed per i capelli da elfo.
 * 
 ***************************************************************************/

using Midgard.Engines.Races;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public class ElvenHairRestylingDeed : Item
    {
        [Constructable]
        public ElvenHairRestylingDeed()
            : base( 0x14F0 )
        {
            Name = "elven hair restyling deed";
            Weight = 1.0;
            LootType = Core.AOS ? LootType.Blessed : LootType.Regular; // mod by Dies Irae
        }

        public ElvenHairRestylingDeed( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( from.Race != Midgard.Engines.Races.Core.HighElf &&
                from.Race != Midgard.Engines.Races.Core.NorthernElf &&
                from.Race != Midgard.Engines.Races.Core.HalfElf )
            {
                from.SendMessage( "This deed is only for the Elven kind." );
                return;
            }

            if( !IsChildOf( from.Backpack ) )
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
            else
                from.SendGump( new InternalGump( from, this ) );
        }

        private class InternalGump : Gump
        {
            private Mobile m_From;
            private ElvenHairRestylingDeed m_Deed;

            public InternalGump( Mobile from, ElvenHairRestylingDeed deed )
                : base( 50, 50 )
            {
                m_From = from;
                m_Deed = deed;

                bool female = m_From.Female;

                from.CloseGump( typeof( InternalGump ) );

                AddBackground( 100, 10, 400, 385, 0xA28 );

                AddHtmlLocalized( 100, 25, 400, 35, 1013008, false, false );
                AddButton( 175, 340, 0xFA5, 0xFA7, 0x0, GumpButtonType.Reply, 0 ); // CANCEL

                AddHtmlLocalized( 210, 342, 90, 35, 1011012, false, false );// <CENTER>HAIRSTYLE SELECTION MENU</center>

                AddBackground( 220, 60, 50, 50, 0xA3C );
                AddBackground( 220, 115, 50, 50, 0xA3C );
                AddBackground( 220, 170, 50, 50, 0xA3C );
                AddBackground( 220, 225, 50, 50, 0xA3C );
                AddBackground( 425, 60, 50, 50, 0xA3C );
                AddBackground( 425, 115, 50, 50, 0xA3C );
                AddBackground( 425, 170, 50, 50, 0xA3C );
                AddBackground( 425, 225, 50, 50, 0xA3C );
                AddBackground( 425, 280, 50, 50, 0xA3C );

                AddHtml( 150, 75, 80, 35, "Long Feather", false, false );
                AddHtml( 150, 130, 80, 35, "Short", false, false );
                AddHtml( 150, 185, 80, 35, "Mullet", false, false );
                AddHtml( 150, 240, 80, 35, "Knob", false, false );
                AddHtml( 355, 75, 80, 35, "Braided", false, false );
                AddHtml( 355, 130, 80, 35, "Spiked", false, false );
                AddHtml( 355, 185, 80, 35, "Flower", false, false );	//Flower or Mid-long
                AddHtml( 355, 240, 80, 35, "Bun", false, false );	//Bun or Long
                AddHtml( 355, 295, 80, 35, "Bald", false, false ); // Bald

                AddImage( 153, 20, 0xEDF5 );	// Long Feather
                AddImage( 153, 65, 0xEDF6 );	// Short
                AddImage( 153, 120, 0xEDF7 );	// Mullet
                AddImage( 153, 185, 0xEDDC );	// Knob
                AddImage( 358, 18, 0xEDDD );    // Braided
                AddImage( 358, 75, 0xEDDF );    // Spiked
                AddImage( 358, 120, ( female ? 0xEDDA : 0xEDF4 ) );
                AddImage( 362, 190, ( female ? 0xEDDE : 0xEDDB ) );

                AddButton( 118, 73, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0 );
                AddButton( 118, 128, 0xFA5, 0xFA7, 3, GumpButtonType.Reply, 0 );
                AddButton( 118, 183, 0xFA5, 0xFA7, 4, GumpButtonType.Reply, 0 );
                AddButton( 118, 238, 0xFA5, 0xFA7, 5, GumpButtonType.Reply, 0 );
                AddButton( 323, 73, 0xFA5, 0xFA7, 6, GumpButtonType.Reply, 0 );
                AddButton( 323, 128, 0xFA5, 0xFA7, 7, GumpButtonType.Reply, 0 );
                AddButton( 323, 183, 0xFA5, 0xFA7, 8, GumpButtonType.Reply, 0 );
                AddButton( 323, 238, 0xFA5, 0xFA7, 9, GumpButtonType.Reply, 0 );
                AddButton( 323, 292, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0 );
            }

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                if( m_Deed.Deleted )
                    return;

                if( info.ButtonID > 0 )
                {
                    int itemID = 0;

                    switch( info.ButtonID )
                    {
                        case 2: itemID = 0x2FC0; break;	//Long Feather 
                        case 3: itemID = 0x2FC1; break;	//Short
                        case 4: itemID = 0x2FC2; break;	//Mullet
                        case 5: itemID = 0x2FCE; break;	//Knob
                        case 6: itemID = 0x2FCF; break;	//Braided
                        case 7: itemID = 0x2FD1; break;	//Spiked
                        case 8: itemID = m_From.Female ? 0x2FCC : 0x2FBF; break;	//Flower or Mid-long
                        case 9: itemID = m_From.Female ? 0x2FD0 : 0x2FCD; break;	//Bun or Long
                    }

                    if( m_From is PlayerMobile )
                    {
                        PlayerMobile pm = (PlayerMobile)m_From;

                        pm.SetHairMods( -1, -1 ); // clear any hairmods (disguise kit, incognito)
                    }

                    m_From.HairItemID = itemID;

                    m_Deed.Delete();
                }
            }
        }
    }
}