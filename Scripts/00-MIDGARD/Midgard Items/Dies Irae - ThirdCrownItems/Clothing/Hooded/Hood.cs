using Midgard.Engines.Races;

using Server;
using Server.Items;
using Server.Network;

namespace Midgard.Items
{
    [RaceAllowance( typeof( MountainDwarf ) )]
    public class Hood : BaseHat
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public Item HoodedItem { get; private set; }

        [Constructable]
        public Hood()
            : base( 0x3356, 0 )
        {
        }

        [Constructable]
        public Hood( int hue )
            : base( 0x3356, hue )
        {
        }

        [Constructable]
        public Hood( Item item, int itemID, int hue )
            : base( itemID, hue )
        {
            HoodedItem = item;
        }

        public override bool CheckLift( Mobile from, Item item, ref LRReason reject )
        {
            reject = LRReason.CannotLift;

            return false;
        }

        #region serialization
        public Hood( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( HoodedItem );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            HoodedItem = reader.ReadItem();
        }
        #endregion
    }
}