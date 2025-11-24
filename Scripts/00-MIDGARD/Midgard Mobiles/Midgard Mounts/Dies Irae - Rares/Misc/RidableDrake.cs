using Server;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    [CorpseName( "a drake corpse" )]
    public class RidableDrake : BaseMount
    {
        public override int TreasureMapLevel { get { return 2; } }
        public override int Meat { get { return 10; } }
        public override int Hides { get { return 20; } }
        public override HideType HideType { get { return HideType.Horned; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat | FoodType.Fish; } }

                
        [Constructable( AccessLevel.Administrator )]
        public RidableDrake()
            : base( "a ridable drake", 60, 0x3f0e, AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            BaseSoundID = 362;

            SetDamageType( ResistanceType.Physical, 80 );
            SetDamageType( ResistanceType.Fire, 20 );

            SetSkill( SkillName.MagicResist, 65.1, 80.0 );
            SetSkill( SkillName.Tactics, 65.1, 90.0 );
            SetSkill( SkillName.Wrestling, 65.1, 80.0 );

            Fame = 5500;
            Karma = -5500;

            VirtualArmor = 46;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 84.3;
        }

        public RidableDrake( Serial serial )
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