/***************************************************************************
 *                               ScoutThrowingHatchet.cs
 *
 *   begin                : 08 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Items;

namespace Midgard.Items
{
    [Flipable( 0xF43, 0xF44 )]
    public class ScoutThrowingHatchet : BaseScoutThrowingWeapon
    {
        public override string DefaultName { get { return "a throwing hatchet"; } }

        public override int OldStrengthReq { get { return 15; } }

        public override int OldMinDamage { get { return 2; } }

        public override int OldMaxDamage { get { return 17; } }

        public override int OldSpeed { get { return 40; } }

        public override int InitMinHits { get { return 31; } }

        public override int InitMaxHits { get { return 80; } }

        public override int NumDice { get { return 1; } }

        public override int NumSides { get { return 8; } }

        public override int DiceBonus { get { return 1; } }

        [Constructable]
        public ScoutThrowingHatchet()
            : base( 0xF43 )
        {
            Weight = 4.0;
            Layer = Layer.OneHanded;
        }

        #region serialization
        public ScoutThrowingHatchet( Serial serial )
            : base( serial )
        {
        }

        public override int AnimID { get { return 0x3B7B; } }

        public override int AnimHue { get { return 0x481; } }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}