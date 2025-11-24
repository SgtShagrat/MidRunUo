using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    /*
    npctemplate                  trollgeneral
    {
        name                     a troll general
        objtype                  0x37
        color                    33784
        gender                   0
        str                      290
        int                      140
        dex                      80
        hits                     290
        mana                     140
        stam                     80
        tactics                  130
        macefighting             130
        magicresistance          100
        attackspeed              30
        attackdamage             4d8+5
        ar                       4d6+5
        karma                    -3000    -3500
        fame                     1500     1750
    }
    */
    [CorpseName( "a troll corpse" )]
    public class TrollGeneral : BaseTroll
    {
        [Constructable]
        public TrollGeneral()
            : base( AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.25, 1.0 )
        {
            Name = "a troll general";
            Body = 0x37;
            BaseSoundID = 0x1CD;

            Hue = 33784;

            SetStr( 290 );
            SetDex( 80 );
            SetInt( 140 );

            SetHits( 290 );
            SetMana( 140 );
            SetStam( 80 );

            SetDamage( "4d8+5" );

            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 35, 45 );
            SetResistance( ResistanceType.Fire, 25, 35 );
            SetResistance( ResistanceType.Cold, 15, 25 );
            SetResistance( ResistanceType.Poison, 5, 15 );
            SetResistance( ResistanceType.Energy, 5, 15 );

            SetSkill( SkillName.Tactics, 130.0 );
            SetSkill( SkillName.Macing, 130.0 );
            SetSkill( SkillName.MagicResist, 100.0 );
            SetSkill( SkillName.Wrestling, 100.0 );

            SetArmor( "8d6+10" );

            Karma = Utility.RandomMinMax( -3000, -3500 );
            Fame = Utility.RandomMinMax( 1500, 1750 );
        }

        public override int CustomWeaponSpeed
        {
            get { return 30; }
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Average, 3 );
        }

        #region serialization
        public TrollGeneral( Serial serial )
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
