using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    /*
    npctemplate                  trollwarlord
    {
        name                     a troll warlord
        objtype                  0x35
        color                    0x0455
        str                      350
        int                      100
        dex                      90
        hits                     350
        mana                     100
        stam                     90
        tactics                  110
        swordsmanship            130
        magicresistance          100
        attackspeed              35
        attackdamage             6d8
        ar                       22
        karma                    -3000    -3500
        fame                     1500     1750
    }
    */

    [CorpseName( "a troll corpse" )]
    public class TrollWarLord : BaseTroll
    {
        [Constructable]
        public TrollWarLord()
            : base( AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.25, 1.0 )
        {
            Name = "a troll warlord";
            Body = 0x35;
            BaseSoundID = 0x1CD;

            Hue = 0x0455;

            SetStr( 350 );
            SetDex( 90 );
            SetInt( 100 );

            SetHits( 350 );
            SetMana( 100 );
            SetStam( 90 );

            SetDamage( "6d8" );

            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 35, 45 );
            SetResistance( ResistanceType.Fire, 25, 35 );
            SetResistance( ResistanceType.Cold, 15, 25 );
            SetResistance( ResistanceType.Poison, 5, 15 );
            SetResistance( ResistanceType.Energy, 5, 15 );

            SetSkill( SkillName.Tactics, 110.0 );
            SetSkill( SkillName.Swords, 130.0 );
            SetSkill( SkillName.MagicResist, 100.0 );
            SetSkill( SkillName.Wrestling, 100.0 );

            VirtualArmor = 22 * 2;

            Karma = Utility.RandomMinMax( -3000, -3500 );
            Fame = Utility.RandomMinMax( 1500, 1750 );
        }

        public override int CustomWeaponSpeed
        {
            get { return 35; }
        }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average, 2 );
		}

        #region serialization
        public TrollWarLord( Serial serial )
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
