/***************************************************************************
 *                               CraftableTrashChest.cs
 *
 *   begin                : 21 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Items;
using Server.Network;

namespace Midgard.Items
{
    [Furniture]
    [Flipable( 0xE41, 0xE40 )]
    public class CraftableTrashChest : Container
    {
        public override int DefaultMaxWeight { get { return 0; } }
        public override bool IsDecoContainer { get { return false; } }

        public override int LabelNumber
        {
            get { return 1064924; } // trashchest
        }

        [Constructable]
        public CraftableTrashChest()
            : base( 0xE41 )
        {
            Weight = 10;
        }

        public CraftableTrashChest( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion

        public override bool OnDragDrop( Mobile from, Item dropped )
        {
            if( !base.OnDragDrop( from, dropped ) )
                return false;

            PublicOverheadMessage( MessageType.Regular, 0x3B2, Utility.Random( 1042891, 8 ) );
            dropped.Delete();

            return true;
        }

        public override bool OnDragDropInto( Mobile from, Item item, Point3D p )
        {
            if( !base.OnDragDropInto( from, item, p ) )
                return false;

            PublicOverheadMessage( MessageType.Regular, 0x3B2, Utility.Random( 1042891, 8 ) );
            item.Delete();

            return true;
        }
    }
}