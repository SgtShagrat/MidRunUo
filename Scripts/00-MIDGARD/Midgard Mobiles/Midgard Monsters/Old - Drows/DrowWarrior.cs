using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    public class DrowWarrior : BaseDrow
    {
        [Constructable]
        public DrowWarrior()
            : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.15, 0.4 )
        {
            Title = ", a drow Warrior";

            if( Female )
                AddItem( new Skirt( Utility.RandomNeutralHue() ) );
            else
                AddItem( new Kilt( Utility.RandomNeutralHue() ) );

            SetStr( 90, 125 );
            SetDex( 100, 125 );
            SetInt( 90, 125 );

            SetHits( 175, 225 );

            SetDamage( 10, 16 );

            SetSkill( SkillName.Archery, 95.0, 105.5 );
            SetSkill( SkillName.Macing, 85.0, 105.5 );
            SetSkill( SkillName.MagicResist, 165.0, 195.5 );
            SetSkill( SkillName.Swords, 85.0, 105.5 );
            SetSkill( SkillName.Tactics, 115.0, 119.5 );
            SetSkill( SkillName.Wrestling, 100.1, 105.3 );
            SetSkill( SkillName.Parry, 105.0, 110.0 );
            SetSkill( SkillName.Lumberjacking, 100.0, 120.0 );
            SetSkill( SkillName.Fencing, 85.0, 120.0 );

            Fame = 10000;
            Karma = -10000;

            VirtualArmor = 50;

            AddItem( new Boots( Utility.RandomNeutralHue() ) );
            AddItem( new ChainChest() );
            AddItem( new ChainLegs() );
            AddItem( new Cloak() );

            switch( Utility.Random( 4 ) )
            {
                case 0: AddItem( new Longsword() ); break;
                case 1:
                case 2: AddItem( new ExecutionersAxe() ); break;
                case 3: AddItem( new Spear() ); break;
                case 4: AddItem( new WarHammer() ); break;
                case 5: AddItem( new Kryss() ); break;
                case 6: AddItem( new ShortSpear() ); break;
            }
            if( 0.5 >= Utility.RandomDouble() )
                PackItem( new ExecutionersCap( 3 ) );

            HairItemID = Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x203C );
            HairHue = Race.RandomHairHue();
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Meager );
            AddLoot( LootPack.Gems, Utility.Random( 1, 5 ) );
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

        public DrowWarrior( Serial serial )
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
    }
}
