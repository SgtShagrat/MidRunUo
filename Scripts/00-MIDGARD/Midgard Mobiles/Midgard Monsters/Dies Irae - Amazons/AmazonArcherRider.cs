/***************************************************************************
 *                               AmazonArcherRider.cs
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
    public class AmazonArcherRider : BaseAmazon
    {
        [Constructable]
        public AmazonArcherRider()
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

            new BlackHorse().Rider = this;
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Average );
        }

        public override bool AlwaysMurderer { get { return true; } }

        public override bool OnBeforeDeath()
        {
            IMount mount = this.Mount;

            if( mount != null )
                mount.Rider = null;

            if( mount is Mobile && Utility.Random( 20 ) != 1 )
                ( (Mobile)mount ).Delete();

            return base.OnBeforeDeath();
        }

        #region serialization
        public AmazonArcherRider( Serial serial )
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