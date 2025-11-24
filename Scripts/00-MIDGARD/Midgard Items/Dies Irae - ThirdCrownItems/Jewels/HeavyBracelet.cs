using Midgard.Engines.Races;

using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3368 Heavy Bracelet - ( craftabile esclusivamente dal pg fabbro, loot di orchi e npc di assalto,)
    /// </summary>
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
    public class HeavyBracelet : GoldBracelet
    {
        public override string DefaultName { get { return "heavy bracelet"; } }

        [Constructable]
        public HeavyBracelet()
        {
            ItemID = 0x3368;
        }

        #region serialization
        public HeavyBracelet( Serial serial )
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