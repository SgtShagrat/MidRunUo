using Server;

namespace Midgard.Engines.PlagueBeastLordPuzzle
{
    public class HealtyGland : Item
    {
        public override int LabelNumber { get { return 1066129; } } // a healthy gland
        public override bool DisplayWeight { get { return false; } }

        [Constructable]
        public HealtyGland()
            : base( 0x1CEF )
        {
            Hue = 0x26B;
            Weight = 1;
        }

        public HealtyGland( Serial serial )
            : base( serial )
        {
        }

        public override bool DropToItem( Mobile from, Item target, Point3D p )
        {
            if( target is PlagueBackpack )
            {
                PuzzlePlagueBeastLord pbl = target.RootParent as PuzzlePlagueBeastLord;

                if( pbl != null && !pbl.Deleted && pbl.Map != Map.Internal )
                    pbl.HealtyGlandPlaceCheck( p, this, from );
            }

            return base.DropToItem( from, target, p );
        }

        #region serial-deserial
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