/***************************************************************************
 *                               AmazonPriestess.cs
 *
 *   begin                : 15 November, 2009
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
    public class AmazonPriestess : BaseAmazon
    {
        [Constructable]
        public AmazonPriestess()
            : base( AIType.AI_Mage, "the amazon priestess" )
        {
            SetStr( 100, 175 );
            SetDex( 100, 135 );
            SetInt( 90, 125 );

            SetHits( 250, 300 );

            SetDamage( 10, 23 );

            SetSkill( SkillName.EvalInt, 95.0, 110.5 );
            SetSkill( SkillName.Magery, 95.0, 110.5 );
            SetSkill( SkillName.MagicResist, 175.0, 215.5 );
            SetSkill( SkillName.Anatomy, 85.0, 95.5 );
            SetSkill( SkillName.Tactics, 95.0, 102.5 );
            SetSkill( SkillName.Wrestling, 100.0, 112.5 );
            SetSkill( SkillName.Meditation, 95.6, 105.4 );

            VirtualArmor = 45;

            AddItem( new Boots( 0x78B ) );
            AddItem( Rehued( new FemaleStuddedChest(), 0x78B ) );
            AddItem( Rehued( new Necklace(), 0x78B ) );
            AddItem( Rehued( new Cloak(), 0x78B ) );
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Average );
        }

        public override bool AlwaysMurderer { get { return true; } }

        #region serialization
        public AmazonPriestess( Serial serial )
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