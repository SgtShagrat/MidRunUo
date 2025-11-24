using System;
using Server;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles 
{
	[CorpseName( "an evil mage corpse" )] 
	public class EvilMage : BaseCreature 
	{ 
		[Constructable] 
		public EvilMage() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 ) 
		{
            if( Core.AOS )
            {
                Name = NameList.RandomName( "evil mage" );
                Title = "the evil mage";
                Body = 124;

                SetStr( 81, 105 );
                SetDex( 91, 115 );
                SetInt( 96, 120 );

                SetHits( 49, 63 );

                SetDamage( 5, 10 );

                SetDamageType( ResistanceType.Physical, 100 );

                SetResistance( ResistanceType.Physical, 15, 20 );
                SetResistance( ResistanceType.Fire, 5, 10 );
                SetResistance( ResistanceType.Poison, 5, 10 );
                SetResistance( ResistanceType.Energy, 5, 10 );

                SetSkill( SkillName.EvalInt, 75.1, 100.0 );
                SetSkill( SkillName.Magery, 75.1, 100.0 );
                SetSkill( SkillName.MagicResist, 75.0, 97.5 );
                SetSkill( SkillName.Tactics, 65.0, 87.5 );
                SetSkill( SkillName.Wrestling, 20.2, 60.0 );

                Fame = 2500;
                Karma = -2500;

                VirtualArmor = 16;
                PackReg( 6 );
                PackItem( new Robe( Utility.RandomNeutralHue() ) ); // TODO: Proper hue
                PackItem( new Sandals() );
            }

            #region mod by Dies Irae [Second Age Template]
            // <name 549>
            Name = NameList.RandomName( "evil mage" );

            Title = "the evil mage";

            // <type NORMAL 400>
            Body = 400;

            // <sex MALE>
            Female = false;

            // <partialhue all_skin_colors>
            Hue = Race.RandomSkinHue();

            // <strength 1d25+80>
            SetStr( "1d25+80" );

            // <dexterity 1d25+90>
            SetDex( "1d25+90" );

            // <intelligence 1d25+95>
            SetInt( "1d25+95" );

            // <hp STR>
            SetHits( Str );

            // <mana INT>
            SetMana( Int );

            // <stamina DEX>
            SetStam( Dex );

            // <naturalwc 3d4>
            SetDamage( "3d4" );

            // <naturalwc 3d4>

            /*
            <sk skill_melee 25d10+625>
            <sk skill_magic_defense 25d10+725>
            <sk skill_battle_defense 25d10+625>
            <sk skill_evaluate 25d10+525>
            <sk skill_magic 1d150+850>
            <sk skill_inscribe 1d150+750>
            <sk	kill_weapon_hand 2d200+200>
             */

            SetSkillF( SkillName.Wrestling, "25d10+625" );
            SetSkillF( SkillName.MagicResist, "25d10+725" );
            SetSkillF( SkillName.Parry, "25d10+625" );
            SetSkillF( SkillName.EvalInt, "25d10+525" );
            SetSkillF( SkillName.Magery, "1d150+850" );
            SetSkillF( SkillName.Inscribe, "1d150+750" );
            SetSkillF( SkillName.Tactics, "2d200+200" );

            /*
            <eq hair all_hair_colors 1>
            <eq facial_hair match_hair 1>
            <eq robe red_colors 1>
            <eq sandals 0 0 1>
            <eq random_coin_purse 0 0 1>
            <eq random_upper_scroll 0 0 1 SELFCONTAINED>
            */

            // <naturalac 8>
            VirtualArmor = 8 * 2; // 2 is scalar to equiparate runuo and the demo

            // <notoriety -125>
            Fame = 125 * 20; // is 20 the demo scalar?
            Karma = -125 * 20;

            HairItemID = Race.RandomHair( false );
            HairHue = Race.RandomHairHue();

            FacialHairItemID = Race.RandomFacialHair( false );
            FacialHairHue = HairHue;

            AddItem( new Robe( Utility.RandomRedHue() ) );
            AddItem( new Sandals() );

            Container pack = Backpack;
            if( pack != null )
                pack.Delete();

            pack = new Backpack();
            pack.Movable = false;

            AddItem( pack );

            PackReg( 6 );
            #endregion
        }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.MedScrolls );
		}

		public override bool CanRummageCorpses{ get{ return true; } } // <script loot>
		public override bool AlwaysMurderer{ get{ return true; } }
		public override int Meat{ get{ return 1; } }
		public override int TreasureMapLevel{ get{ return Core.AOS ? 1 : 0; } }

		public EvilMage( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}