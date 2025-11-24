/***************************************************************************
 *                               TrollPoisoner.cs
 *
 *   begin                : 17 November, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    [CorpseName( "a troll corpse" )]
    public class TrollPoisoner : BaseTroll
    {
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override Poison HitPoison { get { return ( 0.8 >= Utility.RandomDouble() ? Poison.Greater : Poison.Deadly ); } }
        public override int TreasureMapLevel { get { return 1; } }
        public override int Meat { get { return 2; } }

        [Constructable]
        public TrollPoisoner()
            : base( AIType.AI_Melee )
        {
            Name = "a troll poisoner";
            Body = 53;
            BaseSoundID = 461;

            SetStr( 176, 205 );
            SetDex( 46, 65 );
            SetInt( 46, 70 );

            SetHits( 300, 350 );

            SetDamage( 10, 15 );

            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 65, 75 );
            SetResistance( ResistanceType.Fire, 45, 55 );
            SetResistance( ResistanceType.Cold, 45, 55 );
            SetResistance( ResistanceType.Poison, 40, 45 );
            SetResistance( ResistanceType.Energy, 55, 65 );

            SetSkill( SkillName.MagicResist, 45.1, 60.0 );
            SetSkill( SkillName.Tactics, 50.1, 70.0 );
            SetSkill( SkillName.Wrestling, 50.1, 70.0 );

            Fame = 3500;
            Karma = -3500;

            VirtualArmor = 40;
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Average );
            AddLoot( LootPack.Potions );
        }

        #region serial-deserial
        public TrollPoisoner( Serial serial )
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