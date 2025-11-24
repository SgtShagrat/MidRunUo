using Server.Items;

namespace Server.Mobiles
{
    [CorpseName( "an ore elemental corpse" )]
    public class XenianElemental : BaseCreature
    {
        [Constructable]
        public XenianElemental()
            : this( 2 )
        {
        }

        [Constructable]
        public XenianElemental( int oreAmount )
            : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Name = "a xenian elemental";
            Body = 108;
            Hue = CraftResources.GetHue( CraftResource.OldXenian );
            BaseSoundID = 268;

            SetStr( 226, 255 );
            SetDex( 126, 145 );
            SetInt( 71, 92 );

            SetHits( 136, 153 );

            SetDamage( 15, 25 );

            SetDamageType( ResistanceType.Physical, 25 );
            SetDamageType( ResistanceType.Fire, 25 );
            SetDamageType( ResistanceType.Cold, 25 );
            SetDamageType( ResistanceType.Energy, 25 );

            SetResistance( ResistanceType.Physical, 65, 75 );
            SetResistance( ResistanceType.Fire, 50, 60 );
            SetResistance( ResistanceType.Cold, 50, 60 );
            SetResistance( ResistanceType.Poison, 50, 60 );
            SetResistance( ResistanceType.Energy, 40, 50 );

            SetSkill( SkillName.MagicResist, 50.1, 95.0 );
            SetSkill( SkillName.Tactics, 60.1, 100.0 );
            SetSkill( SkillName.Wrestling, 60.1, 100.0 );

            Fame = 3500;
            Karma = -3500;

            VirtualArmor = 38;

            PackItem( new XenianOre( oreAmount ) );

            Engines.Harvest.Mining.BoostElemental( this, CraftResource.OldXenian );
        }

        public XenianElemental( Serial serial )
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
            AddLoot( LootPack.Gems, 2 );
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