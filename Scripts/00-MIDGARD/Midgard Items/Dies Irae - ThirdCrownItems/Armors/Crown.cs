using Midgard.Engines.Races;

using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3d89 Crown - ( item blessed, per seer, si utilizza solo per i due regnanti )
    /// </summary>
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
    public class Crown : Circlet
    {
        public override string DefaultName { get { return "crown"; } }

        [Constructable]
        public Crown()
        {
            ItemID = 0x3d89;
            LootType = LootType.Blessed;
        }

        #region serialization
        public Crown( Serial serial )
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