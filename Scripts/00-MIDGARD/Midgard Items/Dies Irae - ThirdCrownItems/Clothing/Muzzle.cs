using Server;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3B7F Muzzle - Bavaglio, da inserire nel sistema craft del taylor, colorabile. ( layer 10, neck )
    /// </summary>
    public class Muzzle : BaseOuterNeck
    {
        public override string DefaultName { get { return "muzzle"; } }

        [Constructable]
        public Muzzle()
            : this( 0 )
        {
        }

        [Constructable]
        public Muzzle( int hue )
            : base( 0x3B7F, hue )
        {
            Weight = 1.0;
        }

        #region serialization
        public Muzzle( Serial serial )
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