using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "an ostard corpse" )]
	public class FrenziedOstard : BaseMount
	{
		[Constructable]
		public FrenziedOstard() : this( "a frenzied ostard" )
		{
		}

		[Constructable]
		public FrenziedOstard( string name ) : base( name, 0xDA, 0x3EA4, AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            #region mod by Dies Irae
            if( !Core.AOS )
            {
                SetOldTemplate();
                return;
            }
            #endregion

			Hue = Utility.RandomHairHue() | 0x8000;

			BaseSoundID = 0x275;

			SetStr( 94, 170 );
			SetDex( 96, 115 );
			SetInt( 6, 10 );

			SetHits( 71, 110 );
			SetMana( 0 );

			SetDamage( 11, 17 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 25, 30 );
			SetResistance( ResistanceType.Fire, 10, 15 );
			SetResistance( ResistanceType.Poison, 20, 25 );
			SetResistance( ResistanceType.Energy, 20, 25 );

			SetSkill( SkillName.MagicResist, 75.1, 80.0 );
			SetSkill( SkillName.Tactics, 79.3, 94.0 );
			SetSkill( SkillName.Wrestling, 79.3, 94.0 );

			Fame = 1500;
			Karma = -1500;

			Tamable = true;
			ControlSlots = 1;
			// MinTameSkill = 77.1;
            MinTameSkill = 29.1;
		}

        private const int FrenziedOstardHue = 1435;

	    private void SetOldTemplate()
	    {
            /*
            npctemplate                  frenziedostard
            {
                name                     a frenzied ostard
                objtype                  0xda
                color                    1435
                str                      120
                int                      15
                dex                      90
                hits                     120
                mana                     15
                stam                     90
                parry                    80
                magicresistance          70
                tactics                  100
                wrestling                115
                attackspeed              35
                attackdamage             2d6+6
                ar                       35
                tameskill                85
                karma                    -600    -800
                fame                     300     400
            }
            */

	        Hue = FrenziedOstardHue;

			BaseSoundID = 0x270;

            SetStr( 110, 130 );
            SetDex( 75, 95 );
            SetInt( 10, 20 );

            SetHits( Str );
            SetMana( Int );
            SetStam( Dex );

			SetDamage( "2d6+6" );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 15, 20 );

            SetSkill( SkillName.Parry, 80.0 );
            SetSkill( SkillName.MagicResist, 70.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 115.0 );

			Fame = Utility.RandomMinMax( 300, 400 );
			Karma = Utility.RandomMinMax( -600, -800 );

            VirtualArmor = 35;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 85.0;
        }

        public override double GetControlChance( Mobile m, bool useBaseSkill )
        {
            return 1.0;
        }

        public override int CustomWeaponSpeed { get { return 35; } } // mod by Dies Irae
		public override int Meat{ get{ return 3; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Fish | FoodType.Eggs | FoodType.FruitsAndVegies; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Ostard; } }

		public FrenziedOstard( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            #region mod by Dies Irae
            if( Hue != FrenziedOstardHue )
                Hue = FrenziedOstardHue;

            if( version < 1 )
                Timer.DelayCall( TimeSpan.Zero, delegate { if( InternalItem != null ) { InternalItem.Hue = Hue; } } );
            #endregion
		}
	}
}