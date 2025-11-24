using Server.Items;

namespace Server.Mobiles
{
    /// <summary>
    /// Creatura della foresta, se attaccata c'e' una percentuale che spawni 
    /// delle spore funghifere che generano altri esemplari suoi simili
    /// </summary>
    [CorpseName( "a Fungis corpse" )]
    public class Fungis : BaseCreature
    {
        [Constructable]
        public Fungis()
            : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Name = "a Fungis";
            Body = 789;
            Hue = 1155;
            ActiveSpeed = 0.175;

            SetStr( 400, 450 );
            SetDex( 150, 185 );
            SetInt( 71, 95 );

            SetHits( 500, 550 );

            SetDamage( 15, 17 );

            SetDamageType( ResistanceType.Physical, 20 );
            SetDamageType( ResistanceType.Poison, 80 );

            SetResistance( ResistanceType.Physical, 75, 85 );
            SetResistance( ResistanceType.Fire, 40, 55 );
            SetResistance( ResistanceType.Cold, 50, 60 );
            SetResistance( ResistanceType.Poison, 700, 800 );
            SetResistance( ResistanceType.Energy, 50, 60 );

            SetSkill( SkillName.MagicResist, 165.0 );
            SetSkill( SkillName.Tactics, 100.0 );
            SetSkill( SkillName.Wrestling, 100.0 );
            SetSkill( SkillName.Anatomy, 100.0 );


            Fame = 5000;
            Karma = -5000;

            VirtualArmor = 50;
        }
        public override void GenerateLoot()
        {
            AddLoot( LootPack.Average );
            AddLoot( LootPack.Potions );
        }

        public override bool BardImmune { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override Poison HitPoison { get { return Poison.Lethal; } }

        public override void OnThink()
        {
            if( 0.1 >= Utility.RandomDouble() && Combatant != null && 0.1 >= Utility.RandomDouble() )
            {
                new SporaFungis().MoveToWorld( Location, Map );
                Say( "*Go,my friend...*" );
            }

            base.OnThink();
        }

        public Fungis( Serial serial )
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
    }
}