using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3355 Dark Scythe - ( craftabile solo da karma negativo, due mani )
    /// </summary>
    public class DarkScythe : Scythe
    {
        public override string DefaultName { get { return "dark scythe"; } }

        [Constructable]
        public DarkScythe()
        {
            ItemID = 0x3355;
            Layer = Layer.TwoHanded;
        }

        public override bool CanBeCraftedBy( Mobile from )
        {
            return from.AccessLevel > AccessLevel.Counselor || from.Karma < 0;
        }

        #region serialization
        public DarkScythe( Serial serial )
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