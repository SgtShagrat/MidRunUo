using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class LightFungicidePotion : BaseFungicidePotion
    {
        public override PlantPotionLevel Level { get { return PlantPotionLevel.Light; } }
        public override double MinUseSkill { get { return 0.0; } }

        [Constructable]
        public LightFungicidePotion( int amount )
            : base( PotionEffect.FungicideLight, amount )
        {
            Hue = 0x8C5;
        }

        [Constructable]
        public LightFungicidePotion()
            : this( 1 )
        {
        }

        public LightFungicidePotion( Serial serial )
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
