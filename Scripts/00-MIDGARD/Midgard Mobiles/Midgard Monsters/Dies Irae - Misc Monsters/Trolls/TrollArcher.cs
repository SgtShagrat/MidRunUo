/***************************************************************************
 *                               TrollArcher.cs
 *
 *   begin                : 17 November, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    [CorpseName( "a troll corpse" )]
    public class TrollArcher : BaseTroll
    {
        public override int TreasureMapLevel { get { return 1; } }
        public override int Meat { get { return 2; } }

        [Constructable]
        public TrollArcher()
            : base( AIType.AI_Archer )
        {
            Name = "a troll archer";
            Body = 54;
            BaseSoundID = 461;

            SetStr( 176, 205 );
            SetDex( 46, 65 );
            SetInt( 46, 70 );

            SetHits( 106, 123 );

            SetDamage( 8, 14 );

            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 35, 45 );
            SetResistance( ResistanceType.Fire, 25, 35 );
            SetResistance( ResistanceType.Cold, 15, 25 );
            SetResistance( ResistanceType.Poison, 5, 15 );
            SetResistance( ResistanceType.Energy, 5, 15 );

            SetSkill( SkillName.MagicResist, 45.1, 60.0 );
            SetSkill( SkillName.Tactics, 50.1, 70.0 );
            SetSkill( SkillName.Wrestling, 50.1, 70.0 );

            Fame = 3500;
            Karma = -3500;

            VirtualArmor = 40;

            AddItem( new Bow() );
            PackItem( new Arrow( Utility.Random( 50, 120 ) ) );
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Average );
        }

        #region serial-deserial
        public TrollArcher( Serial serial )
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