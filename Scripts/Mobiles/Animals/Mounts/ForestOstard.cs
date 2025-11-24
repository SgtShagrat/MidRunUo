using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "an ostard corpse" )]
	public class ForestOstard : BaseMount
	{
		[Constructable]
		public ForestOstard() : this( "a forest ostard" )
		{
		}

		[Constructable]
		public ForestOstard( string name ) : base( name, 0xDB, 0x3EA5, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
            #region mod by Dies Irae
            if( !Core.AOS )
            {
                SetOldTemplate();
                return;
            }
            #endregion

			Hue = Utility.RandomSlimeHue() | 0x8000;

			BaseSoundID = 0x270;

			SetStr( 94, 170 );
			SetDex( 56, 75 );
			SetInt( 6, 10 );

			SetHits( 71, 88 );
			SetMana( 0 );

			SetDamage( 8, 14 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 15, 20 );

			SetSkill( SkillName.MagicResist, 27.1, 32.0 );
			SetSkill( SkillName.Tactics, 29.3, 44.0 );
			SetSkill( SkillName.Wrestling, 29.3, 44.0 );

			Fame = 450;
			Karma = 0;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 75.0;
		}

	    private const int ForestOstardHue = 1850;

	    private void SetOldTemplate()
	    {
            /*
                npctemplate forestostard
                {
                    name a forest ostard
                    color 1850
                    str 100
                    int 25
                    dex 85
                    hits 100
                    mana 25
                    stam 85
                    parry 60
                    magicresistance 60
                    tactics 70
                    wrestling 85
                    attackspeed 35
                    attackdamage 1d6+3
                    attackskillid wrestling
                    attackhitsound 0x254
                    attackmisssound 0x256
                    ar 35
                    tameskill 75
                    food meat
                    karma -600 -800
                    fame 300 400
                }
            */

	        Hue = ForestOstardHue;

			BaseSoundID = 0x270;

			SetStr( 90, 110 );
			SetDex( 75, 95 );
			SetInt( 20, 30 );

			SetHits( Str );
			SetMana( 0 );
	        SetStam(Dex);

			SetDamage( "1d6+3" );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 15, 20 );

            SetSkill( SkillName.Parry, 50.0, 70.0 );
            SetSkill( SkillName.MagicResist, 50.0, 70.0 );
			SetSkill( SkillName.Tactics, 60.0, 70.0 );
			SetSkill( SkillName.Wrestling, 75.0, 85.0 );

			Fame = Utility.RandomMinMax( 300, 400 );
			Karma = Utility.RandomMinMax( -600, -800 );

            VirtualArmor = 35;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 29.1;
        }

        public override double GetControlChance( Mobile m, bool useBaseSkill )
        {
            return 1.0;
        }

        public override int CustomWeaponSpeed { get { return 35; } } // mod by Dies Irae

	    public override int Meat{ get{ return 3; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Ostard; } }

		public ForestOstard( Serial serial ) : base( serial )
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
            if( Hue != ForestOstardHue )
                Hue = ForestOstardHue;

            if( version < 1 )
                Timer.DelayCall( TimeSpan.Zero, delegate { if( InternalItem != null ) { InternalItem.Hue = Hue; } } );
            #endregion
		}
	}
}