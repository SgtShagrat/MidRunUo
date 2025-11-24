using Midgard.Engines.Races;

using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x256B Bones Necklace - ( no craft, nei loot di scheletri, orchi, liche )
    /// </summary>
    [RaceAllowance( typeof( MountainDwarf ) )]
    public class BonesNecklace : Necklace
    {
        public override string DefaultName { get { return "bones necklace"; } }

        [Constructable]
        public BonesNecklace()
        {
            ItemID = 0x256B;
        }

        #region serialization
        public BonesNecklace( Serial serial )
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