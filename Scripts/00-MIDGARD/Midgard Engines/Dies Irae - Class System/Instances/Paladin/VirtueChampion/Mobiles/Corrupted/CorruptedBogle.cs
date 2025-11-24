/***************************************************************************
 *                               CorruptedBogle.cs
 *
 *   begin                : 13 giugno 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Engines.Classes.VirtueChampion
{
    [CorpseName( "a ghostly corpse" )]
    public class CorruptedBogle : HowlingSoulOfCorruption
    {
        [Constructable]
        public CorruptedBogle()
            : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Name = "a corrupted soul";
            Body = 153;
            BaseSoundID = 0x482;

            SetStr( 76, 100 );
            SetDex( 76, 95 );
            SetInt( 36, 60 );

            SetHits( 46, 60 );

            SetDamage( 7, 11 );

            SetSkill( SkillName.EvalInt, 55.1, 70.0 );
            SetSkill( SkillName.Magery, 55.1, 70.0 );
            SetSkill( SkillName.MagicResist, 55.1, 70.0 );
            SetSkill( SkillName.Tactics, 45.1, 60.0 );
            SetSkill( SkillName.Wrestling, 45.1, 55.0 );

            Fame = 4000;
            Karma = -4000;

            VirtualArmor = 28;
            PackItem( Loot.RandomWeapon() );
            PackItem( new Bone() );
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Meager );
        }

        #region serialization
        public CorruptedBogle( Serial serial )
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