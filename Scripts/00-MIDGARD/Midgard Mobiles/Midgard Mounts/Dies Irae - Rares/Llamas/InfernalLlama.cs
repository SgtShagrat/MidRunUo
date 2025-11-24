using Midgard.Engines.PetSystem;
using Server;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    [CorpseName( "an infernal llama corpse" )]
    public class InfernalLlama : BaseMount
    {
        public override bool HasBreath { get { return true; } }
        public override int Meat { get { return 5; } }
        public override int Hides { get { return 10; } }
        public override HideType HideType { get { return HideType.Barbed; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }
        
        [Constructable( AccessLevel.Administrator )]
        public InfernalLlama()
            : base( "an infernal llama", 0xDC, 0x3EA6, AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            BaseSoundID = 0xA8;
            Hue = 2265;

            SetDamageType( ResistanceType.Physical, 40 );
            SetDamageType( ResistanceType.Fire, 40 );
            SetDamageType( ResistanceType.Energy, 20 );

            SetSkill( SkillName.EvalInt, 10.4, 50.0 );
            SetSkill( SkillName.Magery, 10.4, 50.0 );
            SetSkill( SkillName.MagicResist, 85.3, 100.0 );
            SetSkill( SkillName.Tactics, 97.6, 100.0 );
            SetSkill( SkillName.Wrestling, 80.5, 92.5 );

            Fame = 14000;
            Karma = -14000;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 95.1;
        }

        public InfernalLlama( Serial serial )
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