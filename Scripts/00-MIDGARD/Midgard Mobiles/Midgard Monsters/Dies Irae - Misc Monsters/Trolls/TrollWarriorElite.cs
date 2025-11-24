using Server;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    /*
    npctemplate                  trollelitew
    {
        name                     an elite troll warrior
        objtype                  0x37
        color                    33784
        str                      270
        int                      110
        dex                      75
        hits                     270
        mana                     110
        stam                     75
        magicresistance          80
        tactics                  120
        swordsmanship            135
        attackspeed              35
        attackdamage             4d5
        ar                       5d5+4
        karma                    -2000    -2500
        fame                     1000     1250
    }
     */

    [CorpseName( "a troll corpse" )]
    public class TrollWarriorElite : BaseTroll
    {
        [Constructable]
        public TrollWarriorElite()
            : base( AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.25, 1.0 )
        {
            Name = "an elite troll warrior";
            Body = 0x37;
            BaseSoundID = 0x1CD;

            Hue = 33784;

            SetStr( 270 );
            SetDex( 75 );
            SetInt( 110 );

            SetHits( 270 );
            SetMana( 110 );
            SetStam( 75 );

            SetDamage( "4d5" );

            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 35, 45 );
            SetResistance( ResistanceType.Fire, 25, 35 );
            SetResistance( ResistanceType.Cold, 15, 25 );
            SetResistance( ResistanceType.Poison, 5, 15 );
            SetResistance( ResistanceType.Energy, 5, 15 );

            SetSkill( SkillName.Tactics, 120.0 );
            SetSkill( SkillName.Swords, 135.0 );
            SetSkill( SkillName.MagicResist, 80.0 );
            SetSkill( SkillName.Wrestling, 100.0 );

            SetArmor( "10d5+8" );

            Karma = Utility.RandomMinMax( -2000, -2500 );
            Fame = Utility.RandomMinMax( 1000, 1250 );
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
        public TrollWarriorElite( Serial serial )
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