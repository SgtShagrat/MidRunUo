using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3369 Pirates Jacket - ( craft del sarto, colorabile da hue )
    /// </summary>
    public class PiratesJacket : BaseMiddleTorso
    {
        public override string DefaultName { get { return "pirates jacket"; } }

        [Constructable]
        public PiratesJacket()
            : this( 0 )
        {
        }

        [Constructable]
        public PiratesJacket( int hue )
            : base( 0x3369, hue )
        {
            Weight = 1.0;
        }

        #region serialization
        public PiratesJacket( Serial serial )
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