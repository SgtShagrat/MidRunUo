using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    /*
     npctemplate                  trollking
    {
        name                     a troll chieftan
        objtype                  0x35
        truecolor                0x0465
        str                      350
        int                      160
        dex                      70
        hits                     350
        mana                     160
        stam                     70
        tactics                  120
        swordsmanship            140
        magicresistance          100
        attackspeed              35
        attackdamage             6d8+5
        ar                       4d6
        karma                    -3500    -4500
        fame                     1750     2250
    }
    */

    [CorpseName( "a troll corpse" )]
    public class TrollKing : BaseCreature
    {
        [Constructable]
        public TrollKing()
            : base( AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.25, 1.0 )
        {
            Name = "a troll chieftan";
            Body = 0x35;
            BaseSoundID = 0x1CD;

            Hue = 0x0465;

            SetStr( 350 );
            SetDex( 70 );
            SetInt( 160 );

            SetHits( 350 );
            SetMana( 160 );
            SetStam( 70 );

            SetDamage( "6d8+5" );

            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 35, 45 );
            SetResistance( ResistanceType.Fire, 25, 35 );
            SetResistance( ResistanceType.Cold, 15, 25 );
            SetResistance( ResistanceType.Poison, 5, 15 );
            SetResistance( ResistanceType.Energy, 5, 15 );

            SetSkill( SkillName.Tactics, 120.0 );
            SetSkill( SkillName.Swords, 140.0 );
            SetSkill( SkillName.MagicResist, 100.0 );
            SetSkill( SkillName.Wrestling, 100.0 );

            SetArmor( "8d6" );

            Karma = Utility.RandomMinMax( -3500, -4500 );
            Fame = Utility.RandomMinMax( 1750, 2250 );
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
        public TrollKing( Serial serial )
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
