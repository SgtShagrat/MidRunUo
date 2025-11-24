using Server;
using Server.Multis;

namespace Midgard.Multis
{
    public class BoatFixture : Item
    {
        public BaseBoat Boat { get; set; }

        public BoatFixture( BaseBoat boat, int itemID, int hue )
            : base( itemID )
        {
            Boat = boat;
            Hue = hue;

            Movable = false;
        }

        #region serialization
        public BoatFixture( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );//version

            writer.Write( Boat );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        Boat = reader.ReadItem() as BaseBoat;
                        if( Boat == null )
                            Delete();

                        break;
                    }
            }
        }
        #endregion

        public void OffsetFixture( int x, int y )
        {
            if( Boat != null && !Deleted )
                Location.Offset( x, y, 0 );
        }
    }
}