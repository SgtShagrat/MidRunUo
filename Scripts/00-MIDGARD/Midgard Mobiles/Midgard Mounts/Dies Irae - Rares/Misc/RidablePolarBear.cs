using Server;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    [CorpseName( "an polar bear corpse" )]
    public class RidablePolarBear : BaseMount
    {
        public override int Meat { get { return 2; } }
        public override int Hides { get { return 16; } }
        public override FoodType FavoriteFood { get { return FoodType.Fish | FoodType.FruitsAndVegies | FoodType.Meat; } }
        public override PackInstinct PackInstinct { get { return PackInstinct.Bear; } }

                
        [Constructable( AccessLevel.Administrator )]
        public RidablePolarBear()
            : base( "a ridable polar bear", 213, 16143, AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
        {
            BaseSoundID = 0xA3;

            SetDamageType( ResistanceType.Physical, 100 );

            SetSkill( SkillName.MagicResist, 80.1, 85.0 );
            SetSkill( SkillName.Tactics, 60.1, 70.0 );
            SetSkill( SkillName.Wrestling, 45.1, 60.0 );

            Fame = 1500;
            Karma = 0;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 35.1;
        }

        public RidablePolarBear( Serial serial )
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