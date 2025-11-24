/***************************************************************************
 *                               AmazonHighPriestess.cs
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
    public class AmazonHighPriestess : BaseAmazon
    {
        [Constructable]
        public AmazonHighPriestess()
            : base( AIType.AI_Mage, "the amazon high priestess" )
        {
            SetStr( 100, 175 );
            SetDex( 100, 135 );
            SetInt( 100, 125 );

            SetHits( 250, 300 );

            SetDamage( 10, 23 );

            SetSkill( SkillName.EvalInt, 100.0, 115.5 );
            SetSkill( SkillName.Magery, 100.0, 120.5 );
            SetSkill( SkillName.MagicResist, 175.0, 215.5 );
            SetSkill( SkillName.Meditation, 95.6, 105.4 );

            VirtualArmor = 45;

            AddItem( new ThighBoots( 0x78B ) );
            AddItem( Rehued( new StuddedBustierArms(), 0x78B ) );
            AddItem( Rehued( new LeatherSkirt(), 0x78B ) );
            AddItem( Rehued( new Necklace(), 0x78B ) );
            AddItem( new Cloak( 0x78B ) );

            PackSlayer();
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Rich );
            AddLoot( LootPack.Gems );
        }

        public override bool AlwaysMurderer { get { return true; } }

        #region serialization
        public AmazonHighPriestess( Serial serial )
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