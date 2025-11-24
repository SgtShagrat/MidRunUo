using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class MediumFungicidePotion : BaseFungicidePotion
    {
        public override PlantPotionLevel Level { get { return PlantPotionLevel.Medium; } }
        public override double MinUseSkill { get { return 60.0; } }

        [Constructable]
        public MediumFungicidePotion( int amount )
            : base( PotionEffect.FungicideMedium, amount )
        {
            Hue = 0x8C7;
        }

        [Constructable]
        public MediumFungicidePotion()
            : this( 1 )
        {
        }

        public MediumFungicidePotion( Serial serial )
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
