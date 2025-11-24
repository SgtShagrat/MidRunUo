using Server;
using Server.Items;

namespace Midgard.Items
{
    public class EvilPlateHelm : PlateHelm
    {
        [Constructable]
        public EvilPlateHelm()
            : this( 0 )
        {
        }

        [Constructable]
        public EvilPlateHelm( int hue )
        {
            ItemID = 0x2c6;
            Hue = hue;
        }

        public override string DefaultName
        {
            get { return "evil helm"; }
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

        public EvilPlateHelm( Serial serial )
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