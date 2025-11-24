using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class HeavyFungicidePotion : BaseFungicidePotion
    {
        public override PlantPotionLevel Level { get { return PlantPotionLevel.Heavy; } }
        public override double MinUseSkill { get { return 90.0; } }

        [Constructable]
        public HeavyFungicidePotion( int amount )
            : base( PotionEffect.FungicideHeavy, amount )
        {
            Hue = 0x8CA;
        }

        [Constructable]
        public HeavyFungicidePotion()
            : this( 1 )
        {
        }

        public HeavyFungicidePotion( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}
