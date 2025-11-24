using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x2B5A Shadow Shield - ( craftabile solo da karma negativo , non subisce il colore del metallo ) 
    /// </summary>
    public class OldShadowShield : HeaterShield // da verificare!
    {
        public override string DefaultName { get { return "shadow shield"; } }

        public override bool CannotBeHuedOnCraft { get { return true; } }

        public override int OldDexBonus { get { return -7; } }

        public override int OldStrReq { get { return 30; } }

        [Constructable]
        public OldShadowShield()
        {
            ItemID = 0x2B5A;
        }

        public override bool CanBeCraftedBy( Mobile from )
        {
            return from.Karma < 0;
        }

        #region serialization
        public OldShadowShield( Serial serial )
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