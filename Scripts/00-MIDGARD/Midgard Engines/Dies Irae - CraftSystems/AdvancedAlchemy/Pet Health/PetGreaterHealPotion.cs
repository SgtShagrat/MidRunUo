using Server.Mobiles;

namespace Server.Items
{
    public class PetGreaterHealPotion : BasePetHealthPotion
    {
        #region campi
        public const double PercMinHeal = 0.30;
        public const double PercMaxHeal = 0.40;
        public override double PercProperFun { get { return 0.75; } }
        public override int DelayUse { get { return 10; } }
        #endregion

        #region costruttori
        [Constructable]
        public PetGreaterHealPotion( int amount )
            : base( PotionEffect.PetHealGreater, amount )
        {
            // Name = "Pet Greater Heal Potion";
            Hue = 2466;
        }

        [Constructable]
        public PetGreaterHealPotion()
            : this( 1 )
        {
        }

        public PetGreaterHealPotion( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override void DoPetHealthEffect( Mobile user, BaseCreature pet )
        {
            if( pet.Hits < pet.HitsMax )
            {
                if( pet.Poisoned || MortalStrike.IsWounded( pet ) )
                {
                    user.SendMessage( "Il tuo animale non puo' essere curato mentre è in questo stato!" );
                }
                else
                {
                    int min = Scale( user, (int)( pet.HitsMax * PercMinHeal ) );
                    int max = Scale( user, (int)( pet.HitsMax * PercMaxHeal ) );
                    pet.Heal( Utility.RandomMinMax( min, max ) );
                    Consume();
                }
            }
            else
            {
                user.SendMessage( "Il tuo animale non è ferito!" );
            }
        }
        #endregion

        #region serial deserial
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