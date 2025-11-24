using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3C52 Evening Gown trad. vestito da sera - ( craftabile dai sarti )
    /// </summary>
    public class EveningGown : BaseOuterTorso
    {
        [Constructable]
        public EveningGown()
            : this( 0 )
        {
        }

        [Constructable]
        public EveningGown( int hue )
            : base( 0x3C52, hue )
        {
            Weight = 1.0;
        }

        #region serialization

        public EveningGown( Serial serial )
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

        public override string DefaultName
        {
            get { return "evening gown"; }
        }

        public override bool AllowMaleWearer
        {
            get { return false; }
        }
    }
}