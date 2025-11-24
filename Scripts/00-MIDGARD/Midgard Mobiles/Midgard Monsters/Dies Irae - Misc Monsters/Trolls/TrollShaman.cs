using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    /*
    npctemplate                  trollshaman
    {
        name                     a troll shaman
        objtype                  0x36
        color                    0x0220
        str                      250
        int                      120
        dex                      70
        hits                     250
        mana                     120
        stam                     70
        magicresistance          100
        tactics                  60
        wrestling                90
        evaluatingintelligence   85
        magery                   90
        attackspeed              30
        attackdamage             4d5
        ar                       5d6+3
        karma                    -2000    -2500
        fame                     1000     1250
    }
    */

    [CorpseName( "a troll corpse" )]
    public class TrollShaman : BaseTroll
    {
        [Constructable]
        public TrollShaman()
            : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Name = "a troll shaman";
            Body = 0x36;
            BaseSoundID = 0x1CD;

            Hue = 0x0220;

            SetStr( 250 );
            SetDex( 70 );
            SetInt( 120 );

            SetHits( 250 );
            SetMana( 120 );
            SetStam( 70 );

            SetDamage( "4d5" );

            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 35, 45 );
            SetResistance( ResistanceType.Fire, 25, 35 );
            SetResistance( ResistanceType.Cold, 15, 25 );
            SetResistance( ResistanceType.Poison, 5, 15 );
            SetResistance( ResistanceType.Energy, 5, 15 );

            SetSkill( SkillName.MagicResist, 120.0 );
            SetSkill( SkillName.Tactics, 60.0 );
            SetSkill( SkillName.Wrestling, 90.0 );
            SetSkill( SkillName.EvalInt, 85.0 );
            SetSkill( SkillName.Magery, 90.0 );

            SetArmor( "10d6+6" );

            Karma = Utility.RandomMinMax( -2000, -2500 );
            Fame = Utility.RandomMinMax( 1000, 1250 );
        }

        public override bool AlwaysMurderer { get { return true; } }
        public override bool ShowFameTitle { get { return false; } }

        public override int CustomWeaponSpeed
        {
            get { return 30; }
        }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average, 2 );
            AddLoot( LootPack.MedScrolls );
		}

        #region serialization
        public TrollShaman( Serial serial )
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
        #endregion
    }
}