/***************************************************************************
 *                               TrollMage.cs
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
    public class TrollMage : BaseTroll
    {
        public override int TreasureMapLevel { get { return 1; } }
        public override int Meat { get { return 2; } }

        [Constructable]
        public TrollMage()
            : base( AIType.AI_Mage )
        {
            Name = "a troll mage";
            Body = 54;
            BaseSoundID = 461;

            SetStr( 176, 205 );
            SetDex( 46, 65 );
            SetInt( 200, 300 );

            SetHits( 300, 350 );

            SetDamage( 10, 15 );

            SetDamageType( ResistanceType.Physical, 100 );

            SetResistance( ResistanceType.Physical, 65, 75 );
            SetResistance( ResistanceType.Fire, 45, 55 );
            SetResistance( ResistanceType.Cold, 45, 55 );
            SetResistance( ResistanceType.Poison, 40, 45 );
            SetResistance( ResistanceType.Energy, 55, 65 );

            SetSkill( SkillName.MagicResist, 45.1, 60.0 );
            SetSkill( SkillName.Magery, 70.1, 80.0 );
            SetSkill( SkillName.Wrestling, 50.1, 70.0 );

            Fame = 3500;
            Karma = -3500;

            VirtualArmor = 40;

            PackReg( 3 );
            PackReg( 3 );
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Average );
            AddLoot( LootPack.MedScrolls, 2 );
        }

        #region serial-deserial
        public TrollMage( Serial serial )
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