using Server;
using Server.Items;

using Core = Midgard.Engines.Races.Core;

namespace Midgard.Items
{
    public class NaglorPlateArms : ChainChest
    {
        [Constructable]
        public NaglorPlateArms()
        {
            ItemID = 0x1160;
        }

        public override string DefaultName
        {
            get { return "naglor plate arms"; }
        }

        public override Race RequiredRace
        {
            get { return Core.Naglor; }
        }

        #region serialization

        public NaglorPlateArms( Serial serial )
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

        public override bool CanBeCraftedBy( Mobile from )
        {
            return from.Race == Core.Naglor;
        }
    }
}