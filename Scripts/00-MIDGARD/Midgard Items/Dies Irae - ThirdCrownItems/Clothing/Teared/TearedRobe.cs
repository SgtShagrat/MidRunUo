using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x2670 Teared Robe trad. tuncia stracciata - ( solo random nei loot di zombie, scheletri e liche , colorabile )
    /// </summary>
    public class TearedRobe : BaseOuterTorso
    {
        public override string DefaultName { get { return "teared robe"; } }

        [Constructable]
        public TearedRobe()
            : this( 0 )
        {
        }

        [Constructable]
        public TearedRobe( int hue )
            : base( 0x2670, hue )
        {
            Weight = 1.0;
        }

        #region serialization
        public TearedRobe( Serial serial )
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