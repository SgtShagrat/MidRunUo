using Server;
using Server.Items;

namespace Midgard.Items
{
    public class FarmableZoogiFungus : FarmableCrop
    {
        public static int GetCropID()
        {
            return 0x26B7;
        }

        public override Item GetCropObject()
        {
            return new ZoogiFungus();
        }

        public override int GetPickedID()
        {
            return 3254;
        }

        public override void OnPicked( Mobile from, Point3D loc, Map map )
        {
            Visible = false;

            base.OnPicked( from, loc, map );
        }

        [Constructable]
        public FarmableZoogiFungus()
            : base( GetCropID() )
        {
        }

        #region serialization
        public FarmableZoogiFungus( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();
        }
        #endregion
    }
}