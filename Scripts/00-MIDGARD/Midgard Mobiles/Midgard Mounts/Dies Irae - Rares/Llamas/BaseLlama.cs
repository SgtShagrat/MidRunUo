using Server;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    public abstract class BaseLlama : BaseMount
    {
        public override int Meat { get { return 1; } }
        public override int Hides { get { return 12; } }
        public override FoodType FavoriteFood { get { return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

        public BaseLlama( string name, int hue )
            : base( name, 0xDC, 0x3EA6, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
        {
            Hue = hue;

            BaseSoundID = 0x3F3;

            SetStr( 51, 89 );
            SetDex( 56, 95 );
            SetInt( 26, 40 );

            SetHits( 25, 77 );
            SetMana( 0 );

            SetDamage( 8, 15 );

            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 10, 15 );
            SetResistance( ResistanceType.Fire, 5, 10 );
            SetResistance( ResistanceType.Cold, 5, 10 );
            SetResistance( ResistanceType.Poison, 5, 10 );
            SetResistance( ResistanceType.Energy, 5, 10 );

            SetSkill( SkillName.MagicResist, 15.1, 20.0 );
            SetSkill( SkillName.Tactics, 19.2, 29.0 );
            SetSkill( SkillName.Wrestling, 19.2, 29.0 );

            Fame = 300;
            Karma = 0;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 29.1;
        }

        #region serialization
        public BaseLlama( Serial serial )
            : base( serial )
        {
        }

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