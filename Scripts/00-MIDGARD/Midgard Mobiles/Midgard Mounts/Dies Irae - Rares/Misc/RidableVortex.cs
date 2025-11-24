using Server;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    [CorpseName( "an vortex corpse" )]
    public class RidableVortex : BaseMount
    {
        public override int Meat { get { return 2; } }
        public override int Hides { get { return 16; } }
        public override FoodType FavoriteFood { get { return FoodType.Fish | FoodType.FruitsAndVegies | FoodType.Meat; } }
        public override PackInstinct PackInstinct { get { return PackInstinct.Bear; } }

                
        [Constructable( AccessLevel.Administrator )]
        public RidableVortex()
            : base( "a ridable vortex", 573, 16144, AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
        {
            BaseSoundID = 655;

            SetDamageType( ResistanceType.Physical, 100 );

            SetSkill( SkillName.MagicResist, 80.1, 85.0 );
            SetSkill( SkillName.Tactics, 60.1, 70.0 );
            SetSkill( SkillName.Wrestling, 45.1, 60.0 );

            Fame = 1500;
            Karma = 0;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 20.1;
        }

        public RidableVortex( Serial serial )
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