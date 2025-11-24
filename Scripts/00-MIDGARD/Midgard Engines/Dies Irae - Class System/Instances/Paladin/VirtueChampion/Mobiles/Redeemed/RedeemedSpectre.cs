/***************************************************************************
 *                               RedeemedSpectre.cs
 *
 *   begin                : 01 luglio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Mobiles;

namespace Midgard.Engines.Classes.VirtueChampion
{
    [CorpseName( "a redeemed soul corpse" )]
    public class RedeemedSpectre : HowlingSoulOfRedemption
    {
        [Constructable]
        public RedeemedSpectre()
            : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Name = "a damned soul";
            Body = 26;
            Hue = 0x4001;
            BaseSoundID = 0x482;

            SetStr( 76, 100 );
            SetDex( 76, 95 );
            SetInt( 36, 60 );

            SetHits( 560, 640 );

            SetDamage( 9, 13 );

            SetDamageType( ResistanceType.Physical, 50 );
            SetDamageType( ResistanceType.Cold, 50 );

            SetResistance( ResistanceType.Physical, 25, 30 );
            SetResistance( ResistanceType.Cold, 15, 25 );
            SetResistance( ResistanceType.Poison, 10, 20 );

            SetSkill( SkillName.EvalInt, 55.1, 70.0 );
            SetSkill( SkillName.Magery, 55.1, 70.0 );
            SetSkill( SkillName.MagicResist, 55.1, 70.0 );
            SetSkill( SkillName.Tactics, 45.1, 60.0 );
            SetSkill( SkillName.Wrestling, 45.1, 55.0 );

            Fame = 4000;
            Karma = -4000;

            VirtualArmor = 30;

            PackReg( 10 );
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Meager );
        }

        public override bool BleedImmune { get { return true; } }

        public override OppositionGroup OppositionGroup
        {
            get { return OppositionGroup.FeyAndUndead; }
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        #region serialization
        public RedeemedSpectre( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}