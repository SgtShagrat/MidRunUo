using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3823 Superior Kilt - ( nel vendor tailor e nel menu del craft, colorabile da hue )
    /// </summary>
    public class SuperiorKilt : BaseOuterLegs
    {
        public override string DefaultName { get { return "superior kilt"; } }

        [Constructable]
        public SuperiorKilt()
            : this( 0 )
        {
        }

        [Constructable]
        public SuperiorKilt( int hue )
            : base( 0x3823, hue )
        {
            Weight = 1.0;
        }

        #region serialization
        public SuperiorKilt( Serial serial )
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