using Server.Items;

namespace Server.Mobiles
{
    [CorpseName( "an ore elemental corpse" )]
    public class DarkRubyElemental : BaseCreature
    {
        [Constructable]
        public DarkRubyElemental()
            : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            // TODO: Gas attack
            Name = "a darkRuby elemental";
            Body = 108;
            BaseSoundID = 268;
            Hue = 0x78D;
            SetStr( 226, 255 );
            SetDex( 126, 145 );
            SetInt( 71, 92 );

            SetHits( 136, 153 );

            SetDamage( 9, 16 );

            SetDamageType( ResistanceType.Physical, 30 );
            SetDamageType( ResistanceType.Fire, 70 );

            SetResistance( ResistanceType.Physical, 30, 40 );
            SetResistance( ResistanceType.Fire, 30, 40 );
            SetResistance( ResistanceType.Cold, 10, 20 );
            SetResistance( ResistanceType.Poison, 70, 80 );
            SetResistance( ResistanceType.Energy, 20, 30 );

            SetSkill( SkillName.MagicResist, 50.1, 95.0 );
            SetSkill( SkillName.Tactics, 60.1, 100.0 );
            SetSkill( SkillName.Wrestling, 60.1, 100.0 );

            Fame = 5000;
            Karma = -5000;

            VirtualArmor = 29;

            PackItem( new DarkRubyOre( 1 ) );

            Engines.Harvest.Mining.BoostElemental( this, CraftResource.DarkRuby );
        }

        public DarkRubyElemental( Serial serial )
            : base( serial )
        {
        }

        public override bool AutoDispel
        {
            get { return true; }
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Average );
            AddLoot( LootPack.Gems, 3 );
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
    }
}