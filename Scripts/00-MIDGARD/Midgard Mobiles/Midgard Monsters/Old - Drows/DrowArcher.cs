/***************************************************************************
 *                               DrowArcher.cs
 *
 *   begin                : 03 luglio 2010
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
    public class DrowArcher : BaseDrow
    {
        [Constructable]
        public DrowArcher()
            : base( AIType.AI_Archer, FightMode.Closest, 10, 1, 0.15, 0.4 )
        {
            Title = ", the drow archer";

            SetStr( 160, 205 );
            SetDex( 90, 135 );
            SetInt( 90, 125 );

            SetHits( 250, 300 );

            SetDamage( 8, 14 );

            SetSkill( SkillName.Archery, 95.0, 115.5 );
            SetSkill( SkillName.MagicResist, 165.0, 195.5 );
            SetSkill( SkillName.Tactics, 95.0, 100.5 );
            SetSkill( SkillName.Wrestling, 100.1, 105.3 );
            SetSkill( SkillName.Parry, 105.0, 110.0 );
            SetSkill( SkillName.Anatomy, 100.0, 120.0 );

            Fame = 10000;
            Karma = -10000;

            VirtualArmor = 50;

            if( Female )
                AddItem( new Skirt( Utility.RandomNeutralHue() ) );
            else
                AddItem( new Kilt( Utility.RandomNeutralHue() ) );

            AddItem( new Boots( Utility.RandomNeutralHue() ) );
            AddItem( new ChainChest() );
            AddItem( new ChainLegs() );
            AddItem( new Cloak() );

            if( 0.5 >= Utility.RandomDouble() )
                PackItem( new ExecutionersCap( 3 ) );

            switch( Utility.Random( 2 ) )
            {
                case 0: AddItem( new Bow() ); break;
                case 1: AddItem( new Crossbow() ); break;
                case 2: AddItem( new HeavyCrossbow() ); break;
            }

            PackItem( new Arrow( 25 ) );
            PackItem( new Bolt( 25 ) );
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Meager );
            AddLoot( LootPack.Potions );
        }

        #region serialization
        public DrowArcher( Serial serial )
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