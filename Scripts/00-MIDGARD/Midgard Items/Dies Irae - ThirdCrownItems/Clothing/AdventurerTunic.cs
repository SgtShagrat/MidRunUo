using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3C57 Adventurer Tunic - ( craftabile dai sarti )
    /// </summary>
    public class AdventurerTunic : BaseMiddleTorso
    {
        public override string DefaultName { get { return "adventurer tunic"; } }

        [Constructable]
        public AdventurerTunic()
            : this( 0 )
        {
        }

        [Constructable]
        public AdventurerTunic( int hue )
            : base( 0x3C57, hue )
        {
            Weight = 1.0;
        }

        #region serialization
        public AdventurerTunic( Serial serial )
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