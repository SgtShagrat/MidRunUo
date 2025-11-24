using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    /*
     npctemplate                  trollwarrior
    {
        name                     a troll warrior
        objtype                  0x36
        color                    33784
        str                      250
        int                      60
        dex                      70
        hits                     250
        mana                     60
        stam                     70
        magicresistance          80
        tactics                  115
        wrestling                115
        attackspeed              25
        attackdamage             3d5+5
        ar                       3d5
        karma                    -1600    -2000
        fame                     800     1000
    }
    */

    [CorpseName( "a troll corpse" )]
    public class TrollWarrior : BaseTroll
    {
        [Constructable]
        public TrollWarrior()
            : base( AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.25, 1.0 )
        {
            Name = "a troll warrior";
            Body = 0x36;
            BaseSoundID = 0x1CD;

            Hue = 33784;

            SetStr( 250 );
            SetDex( 70 );
            SetInt( 60 );

            SetHits( 250 );
            SetMana( 60 );
            SetStam( 70 );

            SetDamage( "3d5+5" );

            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 35, 45 );
            SetResistance( ResistanceType.Fire, 25, 35 );
            SetResistance( ResistanceType.Cold, 15, 25 );
            SetResistance( ResistanceType.Poison, 5, 15 );
            SetResistance( ResistanceType.Energy, 5, 15 );

            SetSkill( SkillName.Tactics, 115.0 );
            SetSkill( SkillName.MagicResist, 80.0 );
            SetSkill( SkillName.Wrestling, 115.0 );

            SetArmor( "6d5" );

            Karma = Utility.RandomMinMax( -1600, -2000 );
            Fame = Utility.RandomMinMax( 800, 1000 );
        }

        public override int CustomWeaponSpeed
        {
            get { return 25; }
        }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
		}

        #region serialization
        public TrollWarrior( Serial serial )
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
