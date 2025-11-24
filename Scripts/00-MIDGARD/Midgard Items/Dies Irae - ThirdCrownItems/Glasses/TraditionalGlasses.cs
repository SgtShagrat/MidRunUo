using Server;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3367 Glasses trad. Occhiali - ( craftabile esclusivamente dal pg tinker , colorabile da hue )
    /// </summary>
    public class TraditionalGlasses : BaseGlasses
    {
        public override string DefaultName { get { return "traditional glasses"; } }

        [Constructable]
        public TraditionalGlasses()
            : base( 0x3367 )
        {
        }

        #region serial-deserial
        public TraditionalGlasses( Serial serial )
            : base( serial )
        {
        }

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