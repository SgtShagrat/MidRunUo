using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Midgard.Items
{
    [Flipable( 0x234E, 0x234F )]
    public class TapestryOfMidgard : Item
    {
        public override string DefaultName { get { return "the tapestry of Midgard"; } }

        [Constructable]
        public TapestryOfMidgard()
            : base( 0x234E )
        {
            Weight = 10.0;
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( from.InRange( GetWorldLocation(), 2 ) )
            {
                from.CloseGump( typeof( InternalGump ) );
                from.SendGump( new InternalGump() );
            }
            else
            {
                from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
            }
        }

        private class InternalGump : Gump
        {
            public InternalGump()
                : base( 50, 50 )
            {
                AddImage( 0, 0, 0xb8 );
            }
        }

        public TapestryOfMidgard( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();
        }
    }
}