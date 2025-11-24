using Midgard.Engines.PetSystem;

using Server;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    [CorpseName( "a horse corpse" )]
    public class BlackHorse : BaseMount
    {
        [Constructable]
        public BlackHorse()
            : this( "a horse" )
        {
        }

        [Constructable]
        public BlackHorse( string name )
            : base( name, 0xE2, 0x3EA0, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
        {
            Hue = Utility.RandomList( new int[] { 1175, 1108, 1109, 1997 } );

            BaseSoundID = 0xA8;

            SetStr( 22, 98 );
            SetDex( 56, 75 );
            SetInt( 6, 10 );

            SetHits( 28, 45 );
            SetMana( 0 );

            SetDamage( 3, 4 );

            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 15, 20 );

            SetSkill( SkillName.MagicResist, 25.1, 30.0 );
            SetSkill( SkillName.Tactics, 29.3, 44.0 );
            SetSkill( SkillName.Wrestling, 29.3, 44.0 );

            Fame = 300;
            Karma = 300;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 29.1;

            #region modifica by Dies Irae per lo scalamento dei pet
            PetUtility.ScalePet( this );
            PetUtility.PetNormalize( this );
            #endregion
        }

        public override int Meat
        {
            get { return 3; }
        }

        public override int Hides
        {
            get { return 10; }
        }

        public override FoodType FavoriteFood
        {
            get { return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; }
        }

        #region serialization
        public BlackHorse( Serial serial )
            : base( serial )
        {
        }

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