using Server;

namespace Midgard.Engines.PlagueBeastLordPuzzle
{
    public class DiseasedGland : Item
    {
        public override int LabelNumber { get { return 1066116; } } // a diseased gland
        public override bool DisplayWeight { get { return false; } }

        [Constructable]
        public DiseasedGland( bool visible, bool movable )
            : base( 0x1CEF )
        {
            Hue = 0x4F7;
            Weight = 1;

            Visible = visible;
            Movable = movable;
        }

        public DiseasedGland( Serial serial )
            : base( serial )
        {
        }

        public void OnAftetCut( Mobile from )
        {
            from.SendLocalizedMessage( 1066150 ); // You carefully removed the deseased gland!
            Delete();
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