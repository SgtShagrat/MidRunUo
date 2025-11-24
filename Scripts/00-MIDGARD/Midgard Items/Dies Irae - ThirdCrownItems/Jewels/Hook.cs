using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x2635 Hook ( craft del tinker, magari trovare uncini nei loot dei dungeon con bassa frequenza )
    /// </summary>
    public class Hook : Necklace
    {
        public override string DefaultName { get { return "hook"; } }

        [Constructable]
        public Hook()
        {
            ItemID = 0x2635;
        }

        #region serialization
        public Hook( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}