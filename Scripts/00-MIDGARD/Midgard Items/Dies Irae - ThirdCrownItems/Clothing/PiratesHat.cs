using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x336A Pirates Hat - ( craft del sarto, colorabile da hue )
    /// </summary>
    public class PiratesHat : BaseHat
    {
        public override string DefaultName { get { return "pirates hat"; } }

        [Constructable]
        public PiratesHat()
            : this( 0 )
        {
        }

        [Constructable]
        public PiratesHat( int hue )
            : base( 0x336A, hue )
        {
            Weight = 1.0;
        }

        #region serialization
        public PiratesHat( Serial serial )
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