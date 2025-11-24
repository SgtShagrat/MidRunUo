/***************************************************************************
 *                               AmazonLeader.cs
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
    public class AmazonLeader : BaseAmazon
    {
        [Constructable]
        public AmazonLeader()
            : base( AIType.AI_Melee, "the amazon leader" )
        {
            SetStr( 130, 175 );
            SetDex( 120, 135 );
            SetInt( 90, 125 );

            SetHits( 275, 300 );

            SetDamage( 15, 23 );

            SetSkill( SkillName.Fencing, 90.0, 105.5 );
            SetSkill( SkillName.Macing, 90.0, 105.5 );
            SetSkill( SkillName.MagicResist, 165.0, 195.5 );
            SetSkill( SkillName.Swords, 90.0, 105.5 );
            SetSkill( SkillName.Tactics, 95.0, 100.5 );
            SetSkill( SkillName.Wrestling, 100.1, 105.3 );
            SetSkill( SkillName.Parry, 105.0, 110.0 );
            SetSkill( SkillName.Lumberjacking, 100.0, 120.0 );
            SetSkill( SkillName.Poisoning, 70.1, 100.0 );

            AddItem( new ThighBoots( 1654 ) );
            AddItem( Rehued( new StuddedBustierArms(), 1654 ) );
            AddItem( Rehued( new LeatherSkirt(), 1654 ) );
            AddItem( Rehued( new Necklace(), 1654 ) );
            AddItem( new Cloak( 1654 ) );

            switch( Utility.Random( 7 ) )
            {
                case 0: AddItem( new Spear() ); break;
                case 1:
                case 2: AddItem( new DoubleAxe() ); break;
                case 3: AddItem( new Axe() ); break;
                case 4: AddItem( new WarHammer() ); break;
                case 5:
                case 6: AddItem( new ShortSpear() ); break;
            }
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Average );
        }

        public override Poison HitPoison { get { return Poison.Regular; } }
        public override double HitPoisonChance { get { return 50.0; } }

        public override bool AlwaysMurderer { get { return true; } }

        #region serialization
        public AmazonLeader( Serial serial )
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