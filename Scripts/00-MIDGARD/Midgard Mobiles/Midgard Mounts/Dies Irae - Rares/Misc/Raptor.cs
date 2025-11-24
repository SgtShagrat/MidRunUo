using Server;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    [CorpseName( "an raptor corpse" )]
    public class Raptor : BaseMount
    {
        public override int Meat { get { return 5; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }
        public override PackInstinct PackInstinct { get { return PackInstinct.Ostard; } }
        public override bool SubdueBeforeTame { get { return false; } } // Must be beaten into submission
   
        [Constructable]
        public Raptor()
            : base( "a raptor", 0xD2, 0x3EA3, AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Hue = Utility.RandomList( 1175, 1108, 1109, 1918 );

            BaseSoundID = 0x275;

            SetStr( 114, 190 );
            SetDex( 101, 120 );
            SetInt( 6, 10 );

            SetHits( 91, 130 );
            SetMana( 0 );

            SetDamage( 14, 20 );

            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 30, 45 );
            SetResistance( ResistanceType.Fire, 15, 20 );
            SetResistance( ResistanceType.Poison, 25, 30 );
            SetResistance( ResistanceType.Energy, 25, 30 );

            SetSkill( SkillName.MagicResist, 80.1, 85.0 );
            SetSkill( SkillName.Tactics, 85.3, 100.0 );
            SetSkill( SkillName.Wrestling, 85.3, 100.0 );

            Fame = 2000;
            Karma = -2000;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 29.1;
        }

        public Raptor( Serial serial )
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

            if( Hits == 1 )
            {
                SetStr( 114, 190 );
                SetDex( 101, 120 );
                SetInt( 6, 10 );

                SetHits( 91, 130 );
                SetMana( 0 );

                SetDamage( 14, 20 );

                SetResistance( ResistanceType.Physical, 30, 45 );
                SetResistance( ResistanceType.Fire, 15, 20 );
                SetResistance( ResistanceType.Poison, 25, 30 );
                SetResistance( ResistanceType.Energy, 25, 30 );
            }
        }
        #endregion
    }
}