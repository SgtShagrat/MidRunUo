using Server;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    /*
    npctemplate                  trollassassin
    {
        name                     a troll assassin
        script                   poisonkillpcs
        objtype                  0x36
        color                    33784
        truecolor                33784
        gender                   0
        str                      240
        int                      50
        dex                      60
        hits                     240
        mana                     50
        stam                     60
        parry                    75
        tactics                  100
        poisoning                80
        wrestling                100
        magicresistance          50
        attackspeed              30
        attackdamage             5d4
        ar                       20
        poisondamagelvl          2d2
        karma                    -1300    -1600
        fame                     650     800
    }
    */

    [CorpseName( "a troll corpse" )]
    public class TrollAssassin : BaseTroll
    {
        [Constructable]
        public TrollAssassin()
            : base( AIType.AI_Melee )
        {
            Name = "a troll assassin";
            Body = 0x36;
            BaseSoundID = 0x1CD;

            Hue = 0x83f8;

            SetStr( 240 );
            SetDex( 60 );
            SetInt( 50 );

            SetHits( 240 );
            SetMana( 50 );
            SetStam( 60 );

            SetDamage( "5d4" );

            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 35, 45 );
            SetResistance( ResistanceType.Fire, 25, 35 );
            SetResistance( ResistanceType.Cold, 15, 25 );
            SetResistance( ResistanceType.Poison, 5, 15 );
            SetResistance( ResistanceType.Energy, 5, 15 );

            SetSkill( SkillName.Parry, 75.0 );
            SetSkill( SkillName.Tactics, 95.0 );
            SetSkill( SkillName.Poisoning, 80.0 );
            SetSkill( SkillName.Wrestling, 100.0 );
            SetSkill( SkillName.MagicResist, 50.0 );

            VirtualArmor = 20 * 2;

            Karma = Utility.RandomMinMax( -1300, -1600 );
            Fame = Utility.RandomMinMax( 650, 800 );
        }

        public override int CustomWeaponSpeed
        {
            get { return 30; }
        }

        public override Poison PoisonImmune
        {
            get { return Poison.Lethal; }
        }

        public override Poison HitPoison
        {
            get { return ( 0.8 >= Utility.RandomDouble() ? Poison.Deadly : Poison.Greater ); }
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Average, 3 );
        }

        #region serialization
        public TrollAssassin( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}