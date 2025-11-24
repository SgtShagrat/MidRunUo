using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    /*
    npctemplate                  trollarcherelite
    {
        name                     an elite troll archer
        objtype                  0x36
        color                    33784
        str                      200
        int                      100
        dex                      110
        hits                     200
        mana                     100
        stam                     110
        magicresistance          60
        tactics                  95
        wrestling                120
        archery                  105
        attackspeed              35
        attackdamage             3d5+5
        ar                       4d5+5
        karma                    -1600    -2000
        fame                     800     1000
    }
    */

    [CorpseName( "a troll corpse" )]
    public class TrollArcherElite : BaseTroll
    {
        [Constructable]
        public TrollArcherElite()
            : base( AIType.AI_Archer )
        {
            Name = "an elite troll archer";
            Body = 0x36;
            BaseSoundID = 0x1CD;

            Hue = 0x83f8;

            SetStr( 200 );
            SetDex( 110 );
            SetInt( 100 );

            SetHits( 200 );
            SetMana( 100 );
            SetStam( 110 );

            SetDamage( "3d5+5" );

            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 35, 45 );
            SetResistance( ResistanceType.Fire, 25, 35 );
            SetResistance( ResistanceType.Cold, 15, 25 );
            SetResistance( ResistanceType.Poison, 5, 15 );
            SetResistance( ResistanceType.Energy, 5, 15 );

            SetSkill( SkillName.MagicResist, 60.0 );
            SetSkill( SkillName.Tactics, 95.0 );
            SetSkill( SkillName.Wrestling, 120.0 );
            SetSkill( SkillName.Archery, 105.0 );

            SetArmor( "8d5+10" );

            Karma = Utility.RandomMinMax( -1600, -2000 );
            Fame = Utility.RandomMinMax( 800, 1000 );

            AddItem( new Bow() );
            PackItem( new Arrow( Utility.Random( 50, 120 ) ) );
        }

        public override int CustomWeaponSpeed
        {
            get { return 35; }
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Average );
        }

        #region serialization
        public TrollArcherElite( Serial serial )
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