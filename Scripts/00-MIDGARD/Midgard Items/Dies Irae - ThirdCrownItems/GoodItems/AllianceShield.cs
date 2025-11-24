using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x2B59 Alliance Shield - ( craftabile solo da karma positivo, non subisce il colore del metallo )
    /// </summary>
    public class AllianceShield : HeaterShield // da verificare!
    {
        public override string DefaultName { get { return "alliance shield"; } }

        public override bool CannotBeHuedOnCraft { get { return true; } }

        public override int OldDexBonus { get { return -7; } }

        public override int OldStrReq { get { return 30; } }

        [Constructable]
        public AllianceShield()
        {
            ItemID = 0x2B59;
        }

        public override bool CanBeCraftedBy( Mobile from )
        {
            return from.Karma > 0;
        }

        #region serialization
        public AllianceShield( Serial serial )
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