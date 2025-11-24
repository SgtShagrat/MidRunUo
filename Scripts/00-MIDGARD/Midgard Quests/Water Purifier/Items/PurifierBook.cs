using System;
using System.Collections;
using Server.ContextMenus;
using Server.Gumps;

namespace Server.Items
{
    public class PurifierBook : Item
    {
        [Constructable]
        public PurifierBook()
            : base( 0x0FF4 )
        {
            Movable = false;
        }

        public override void OnDoubleClick( Mobile from )
        {
            from.CloseGump( typeof( PurifierGump ) );
            from.SendGump( new PurifierGump() );
        }

        #region serialization
        public PurifierBook( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}