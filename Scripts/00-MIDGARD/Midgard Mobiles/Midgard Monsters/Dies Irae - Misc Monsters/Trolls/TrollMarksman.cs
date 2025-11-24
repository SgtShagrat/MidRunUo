using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    /*
    npctemplate                  trollmarksman
    {
        name                     a troll marksman
        objtype                  0x36
        color                    33784
        str                      140
        int                      60
        dex                      95
        hits                     140
        mana                     60
        stam                     95
        tactics                  105
        wrestling                115
        archery                  125
        magicresistance          60
        attackspeed              33
        attackdamage             3d4+5
        ar                       3d4+2
        karma                    -1000    -1300
        fame                     500     650
    }
    */

    [CorpseName( "a troll corpse" )]
    public class TrollMarksman : BaseCreature
    {
        [Constructable]
        public TrollMarksman()
            : base( AIType.AI_Archer, FightMode.Aggressor, 10, 1, 0.25, 1.0 )
        {
            Name = "a troll marksman";
            Body = 0x36;
            BaseSoundID = 0x1CD;

            Hue = 33784;

            SetStr( 140 );
            SetDex( 95 );
            SetInt( 60 );

            SetHits( 140 );
            SetMana( 60 );
            SetStam( 95 );

            SetDamage( "3d4+5" );

            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 35, 45 );
            SetResistance( ResistanceType.Fire, 25, 35 );
            SetResistance( ResistanceType.Cold, 15, 25 );
            SetResistance( ResistanceType.Poison, 5, 15 );
            SetResistance( ResistanceType.Energy, 5, 15 );

            SetSkill( SkillName.MagicResist, 60.0 );
            SetSkill( SkillName.Tactics, 105.0 );
            SetSkill( SkillName.Wrestling, 115.0 );
            SetSkill( SkillName.Archery, 125.0 );

            SetArmor( "6d4+4" );

            Karma = Utility.RandomMinMax( -1000, -1300 );
            Fame = Utility.RandomMinMax( 500, 650 );

            AddItem( new Bow() );
            PackItem( new Arrow( Utility.Random( 50, 120 ) ) );
        }

        public override int CustomWeaponSpeed
        {
            get { return 33; }
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Average );
            AddLoot( LootPack.Potions );
        }

        #region serialization
        public TrollMarksman( Serial serial )
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
