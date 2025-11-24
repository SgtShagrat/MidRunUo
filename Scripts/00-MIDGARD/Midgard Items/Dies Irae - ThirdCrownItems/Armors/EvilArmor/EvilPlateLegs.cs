using Server;
using Server.Items;

namespace Midgard.Items
{
    public class EvilPlateLegs : PlateLegs
    {
        [Constructable]
        public EvilPlateLegs()
            : this( 0 )
        {
        }

        [Constructable]
        public EvilPlateLegs( int hue )
        {
            ItemID = 0x2c3;
            Hue = hue;
        }

        public override string DefaultName
        {
            get { return "evil legs"; }
        }

        public override bool CanBeCraftedBy( Mobile from )
        {
            return from.AccessLevel > AccessLevel.Counselor || from.Karma < 0;
        }

        public override bool CanEquip( Mobile from )
        {
            return from.Karma < 0 && base.CanEquip( from );
        }

        #region serialization

        public EvilPlateLegs( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }

        #endregion
    }
}