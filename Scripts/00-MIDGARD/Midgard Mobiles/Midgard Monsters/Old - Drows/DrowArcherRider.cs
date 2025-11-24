/***************************************************************************
 *                               DrowArcherRider.cs
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
    public class DrowArcherRider : BaseDrow
    {
        [Constructable]
        public DrowArcherRider()
            : base( AIType.AI_Archer, FightMode.Closest, 10, 1, 0.15, 0.4 )
        {
            Title = ", the drow archer";

            SetStr( 160, 205 );
            SetDex( 90, 135 );
            SetInt( 90, 125 );

            SetHits( 250, 300 );

            SetDamage( 10, 16 );

            SetSkill( SkillName.Archery, 95.0, 115.5 );
            SetSkill( SkillName.MagicResist, 165.0, 195.5 );
            SetSkill( SkillName.Tactics, 95.0, 100.5 );
            SetSkill( SkillName.Wrestling, 100.1, 105.3 );
            SetSkill( SkillName.Parry, 105.0, 110.0 );
            SetSkill( SkillName.Anatomy, 100.0, 120.0 );

            Fame = 10000;
            Karma = -10000;

            VirtualArmor = 50;

            if( 0.5 >= Utility.RandomDouble() )
                PackItem( new ExecutionersCap( 3 ) );

            AddItem( new Boots( Utility.RandomNeutralHue() ) );
            AddItem( new ChainChest() );
            AddItem( new ChainLegs() );
            AddItem( new Cloak() );

            switch( Utility.Random( 2 ) )
            {
                case 0: AddItem( new Bow() ); break;
                case 1: AddItem( new Crossbow() ); break;
                case 2: AddItem( new HeavyCrossbow() ); break;
            }

            PackItem( new Arrow( 75 ) );
            PackItem( new Bolt( 75 ) );

            new BlackHorse().Rider = this;
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Meager );
            AddLoot( LootPack.Potions );
        }

        public override bool OnBeforeDeath()
        {
            IMount mount = Mount;

            if( mount != null )
                mount.Rider = null;

            if( mount is Mobile && Utility.Random( 20 ) != 1 ) // mod by Dies Irae
                ( (Mobile)mount ).Delete();

            return base.OnBeforeDeath();
        }

        #region serialization
        public DrowArcherRider( Serial serial )
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