using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Network;

namespace Midgard.Items
{
    public class TournamentEquipmentStone : Item
    {
        public override string DefaultName
        {
            get { return "an Tournament Equipment Stone"; }
        }

        [Constructable]
        public TournamentEquipmentStone()
            : base( 0xED4 )
        {
            Movable = false;
            Hue = 2025;
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( from.InRange( GetWorldLocation(), 2 ) )
            {
                DoEquipmentClear( from );
            }
            else
            {
                from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
            }
        }

        private static void DoEquipmentClear( Mobile from )
        {
            Backpack bag = new Backpack();

            List<Item> equipitems = new List<Item>( from.Items );
            for( int i = 0; i < equipitems.Count; i++ )
            {
                if( equipitems[ i ].Movable )
                {
                    if( ( equipitems[ i ].Layer != Layer.Bank ) && ( equipitems[ i ].Layer != Layer.Mount ) && ( equipitems[ i ].Layer != Layer.Backpack ) )
                    {
                        bag.DropItem( equipitems[ i ] );
                    }
                }
            }

            List<Item> packitems = new List<Item>( from.Backpack.Items );
            for( int i = 0; i < packitems.Count; i++ )
            {
                if( packitems[ i ].Movable )
                {
                    bag.DropItem( packitems[ i ] );
                }
            }

            from.BankBox.DropItem( bag );

            from.LocalOverheadMessage( MessageType.Regular, 0x3B2, true, "Your equipment has been moved to your bankbox." );
        }

        #region serialization
        public TournamentEquipmentStone( Serial serial )
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
        #endregion
    }
}