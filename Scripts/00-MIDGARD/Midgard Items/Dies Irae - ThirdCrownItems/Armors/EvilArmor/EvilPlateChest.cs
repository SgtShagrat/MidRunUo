using Server;
using Server.Items;

namespace Midgard.Items
{
    public class EvilPlateChest : PlateChest
    {
        [Constructable]
        public EvilPlateChest()
            : this( 0 )
        {
        }

        [Constructable]
        public EvilPlateChest( int hue )
        {
            ItemID = 0x2c4;
            Hue = hue;
        }

        public override string DefaultName
        {
            get { return "evil platemail"; }
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

        public EvilPlateChest( Serial serial )
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