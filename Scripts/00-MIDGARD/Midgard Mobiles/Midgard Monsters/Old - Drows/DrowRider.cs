/***************************************************************************
 *                               DrowRider.cs
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
    public class DrowRider : BaseDrow
    {
        [Constructable]
        public DrowRider()
            : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.15, 0.4 )
        {
            Title = ", the drow rider";

            SetStr( 100, 175 );
            SetDex( 100, 135 );
            SetInt( 90, 125 );

            SetHits( 250, 300 );

            SetDamage( 10, 23 );

            SetSkill( SkillName.Fencing, 85.0, 105.5 );
            SetSkill( SkillName.Macing, 85.0, 105.5 );
            SetSkill( SkillName.MagicResist, 165.0, 195.5 );
            SetSkill( SkillName.Swords, 85.0, 105.5 );
            SetSkill( SkillName.Tactics, 95.0, 100.5 );
            SetSkill( SkillName.Wrestling, 100.1, 105.3 );
            SetSkill( SkillName.Parry, 105.0, 110.0 );
            SetSkill( SkillName.Lumberjacking, 100.0, 120.0 );

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
            AddItem( new Cloak( Utility.RandomNeutralHue() ) );

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

            if( 0.5 >= Utility.RandomDouble() )
                PackItem( new ExecutionersCap( 3 ) );

            new BlackHorse().Rider = this;
        }
        public override void GenerateLoot()
        {
            AddLoot( LootPack.Average );
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
        public DrowRider( Serial serial )
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