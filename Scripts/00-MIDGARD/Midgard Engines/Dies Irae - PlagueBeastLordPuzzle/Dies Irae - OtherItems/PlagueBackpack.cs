using Server;
using Server.Items;

namespace Midgard.Engines.PlagueBeastLordPuzzle
{
    public class PlagueBackpack : Container
    {
        #region properties
        public override int DefaultMaxWeight { get { return 0x0; } }
        public override int DefaultGumpID { get { return 0x2A63; } }
        public override int DefaultDropSound { get { return 0x39F; } }
        public override Rectangle2D Bounds { get { return new Rectangle2D( 10, 10, 300, 300 ); } }
        #endregion

        public PlagueBackpack()
            : base( 0x261B )
        {
            Layer = Layer.Backpack;
            Movable = false;
        }

        public PlagueBackpack( Serial serial )
            : base( serial )
        {
        }

        public override bool IsAccessibleTo( Mobile m )
        {
            if( RootParent is PuzzlePlagueBeastLord )
                return true;
            else
                return base.IsAccessibleTo( m );
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