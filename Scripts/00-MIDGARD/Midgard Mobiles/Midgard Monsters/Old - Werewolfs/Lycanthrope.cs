using Server.Items;

namespace Server.Mobiles
{
    [CorpseName( "a lycanthrope corpse" )]
    public class Lycanthrope : BaseCreature
    {
        public string Form;
        public bool Transformed = false;

        public override bool ShowFameTitle
        {
            get { return false; }
        }

        public override bool AlwaysMurderer
        {
            get { return false; }
        }

        public override bool BardImmune
        {
            get { return true; }
        }

        public override Poison PoisonImmune
        {
            get { return Poison.Regular; }
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.BleedAttack;
        }

        [Constructable]
        public Lycanthrope()
            : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Female = Utility.RandomBool();

            HumanForm();
        } //end constructable

        public void HumanForm()
        {
            // human form
            if( Female )
            {
                Name = NameList.RandomName( "female" );
                Body = 401;
            }
            else
            {
                Name = NameList.RandomName( "male" );
                Body = 400;
            }

            switch( Utility.Random( 4 ) )
            {
                case 0:
                    AddItem( new ShortHair( 1 ) );
                    break;
                case 1:
                    AddItem( new TwoPigTails( 1 ) );
                    break;
                case 2:
                    AddItem( new PonyTail( 1 ) );
                    break;
                case 3:
                    AddItem( new LongHair( 1 ) );
                    break;
            }

            AddItem( new FancyShirt( Utility.RandomNeutralHue() ) );
            AddItem( new ShortPants( Utility.RandomNeutralHue() ) );
            AddItem( new Boots( Utility.RandomNeutralHue() ) );

            Hue = Utility.RandomSkinHue();

            SetStr( 196, 250 );
            SetDex( 76, 95 );
            SetInt( 36, 60 );

            SetHits( 118, 150 );

            SetDamage( 8, 18 );

            SetDamageType( ResistanceType.Physical, 40 );
            SetDamageType( ResistanceType.Cold, 60 );

            SetResistance( ResistanceType.Physical, 35, 45 );
            SetResistance( ResistanceType.Fire, 20, 30 );
            SetResistance( ResistanceType.Cold, 50, 60 );
            SetResistance( ResistanceType.Poison, 20, 30 );
            SetResistance( ResistanceType.Energy, 30, 40 );

            SetSkill( SkillName.MagicResist, 65.1, 80.0 );
            SetSkill( SkillName.Tactics, 85.1, 100.0 );
            SetSkill( SkillName.Wrestling, 85.1, 95.0 );

            Fame = 3000;
            Karma = 3000;

            VirtualArmor = 40;
            ControlSlots = 1;

            Form = "Human";
            Transformed = false;
        } //fine human form

        public void WolfForm()
        {
            //werewolf form
            Name = "a Lycanthrope";
            Body = 250;
            BaseSoundID = 0xE5;

            SetStr( 400, 450 );
            SetDex( 150, 200 );
            SetInt( 65, 75 );

            SetHits( 380, 440 );
            SetMana( 40, 40 );
            SetStam( 150, 200 );

            SetDamage( 48, 73 );

            SetDamageType( ResistanceType.Physical, 90 );
            SetDamageType( ResistanceType.Cold, 5 );
            SetDamageType( ResistanceType.Energy, 5 );

            SetResistance( ResistanceType.Physical, 40, 60 );
            SetResistance( ResistanceType.Fire, 50, 70 );
            SetResistance( ResistanceType.Cold, 50, 70 );
            SetResistance( ResistanceType.Poison, 50, 70 );
            SetResistance( ResistanceType.Energy, 50, 70 );

            SetSkill( SkillName.MagicResist, 65.0, 70.0 );
            SetSkill( SkillName.Tactics, 95.0, 110.0 );
            SetSkill( SkillName.Wrestling, 98.0, 108.0 );
            SetSkill( SkillName.Anatomy, 65.0, 75.0 );

            Fame = -3000;
            Karma = -3000;

            VirtualArmor = 60;
            ControlSlots = 1;

            Form = "Wolf";
            Transformed = true;
        } //end WerewolfForm

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Poor );
        }

        public Lycanthrope( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }

        //****
        public override bool OnBeforeDeath()
        {
            if( Form == "Human" )
            {
                WolfForm();
                return false;
            }

            AddLoot( LootPack.Poor );
            return true;
        }

        public override void OnGotMeleeAttack( Mobile from )
        {
            if( Form == "Human" )
            {
                WolfForm();
            }
        }

        public override void OnDamagedBySpell( Mobile from )
        {
            if( Form == "Human" )
            {
                WolfForm();
            }
        }

        public override void OnThink()
        {
            //di giorno umani
            if( !Transformed )
                if( IsDay( this ) == true && Form != "Human" )
                {
                    Hidden = false;
                    HumanForm();
                }
                else if( IsDay( this ) == false && Form != "Wolf" )
                {
                    Hidden = false;
                    WolfForm();
                }
        }

        public static bool IsDay( Mobile from )
        {
            Region reg = Region.Find( new Point3D( from.X, from.Y, from.Z ), from.Map );

            int hours, minutes;
            Clock.GetTime( from.Map, from.X, from.Y, out hours, out minutes );

            if( hours >= 4 && hours < 22 ) // ( !(reg is DungeonRegion) && (hours >= 4 && hours < 22) )
                return true;
            else
                return false;
        }

        //***
    }
}