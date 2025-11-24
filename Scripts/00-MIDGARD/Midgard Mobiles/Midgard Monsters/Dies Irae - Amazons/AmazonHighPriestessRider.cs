/***************************************************************************
 *                               AmazonHighPriestessRider.cs
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
    public class AmazonHighPriestessRider : BaseAmazon
    {
        [Constructable]
        public AmazonHighPriestessRider()
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
            SetSkill( SkillName.Anatomy, 85.0, 95.5 );
            SetSkill( SkillName.Tactics, 95.0, 107.5 );
            SetSkill( SkillName.Wrestling, 110.0, 120.5 );
            SetSkill( SkillName.Meditation, 95.6, 105.4 );

            AddItem( new ThighBoots( 0x78B ) );
            AddItem( Rehued( new StuddedBustierArms(), 0x78B ) );
            AddItem( Rehued( new LeatherSkirt(), 0x78B ) );
            AddItem( Rehued( new Necklace(), 0x78B ) );
            AddItem( new Cloak( 0x78B ) );

            new BlackHorse().Rider = this;
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Rich );
        }

        public override bool AlwaysMurderer { get { return true; } }

        public override bool OnBeforeDeath()
        {
            IMount mount = Mount;

            if( mount != null )
                mount.Rider = null;

            if( mount is Mobile && Utility.Random( 20 ) != 1 ) 
                ( (Mobile)mount ).Delete();

            return base.OnBeforeDeath();
        }

        #region serialization
        public AmazonHighPriestessRider( Serial serial )
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