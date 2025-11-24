/***************************************************************************
 *                               AmazonArcher.cs
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
    public class AmazonArcher : BaseAmazon
    {
        [Constructable]
        public AmazonArcher()
            : base( AIType.AI_Archer, "the amazon archer" )
        {
            SetStr( 100, 175 );
            SetDex( 100, 135 );
            SetInt( 90, 125 );

            SetHits( 250, 300 );

            SetDamage( 10, 23 );

            SetSkill( SkillName.Archery, 95.0, 115.5 );

            AddItem( new Sandals( 1654 ) );
            AddItem( Rehued( new FemaleStuddedChest(), 1654 ) );
            AddItem( Rehued( new Necklace(), 1931 ) );
            AddItem( new Cloak( 1654 ) );

            switch( Utility.Random( 2 ) )
            {
                case 0: AddItem( new Bow() ); break;
                case 1: AddItem( new Crossbow() ); break;
                case 2: AddItem( new HeavyCrossbow() ); break;
            }

            PackSlayer();
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Poor );
        }

        #region serialization
        public AmazonArcher( Serial serial )
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