using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    [CorpseName( "an order human corpse" )]
    public class OrderWarrior : BaseCreature, IVirtueCreature
    {
        [Constructable]
        public OrderWarrior()
            : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.15, 0.4 )
        {
            Hue = 0;
            Criminal = true;

            if( Female = Utility.RandomBool() )
            {
                Body = 0x191;
                Female = true;
                Name = NameList.RandomName( "female" );
                AddItem( new Skirt( Utility.RandomNeutralHue() ) );
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName( "male" );
                AddItem( new Kilt( Utility.RandomNeutralHue() ) );
            }

            Title = "the ghost of an Order Warrior";

            SetStr( 90, 125 );
            SetDex( 100, 125 );
            SetInt( 90, 125 );

            SetHits( 175, 225 );

            SetDamage( 30, 50 ); // 10,23

            SetSkill( SkillName.Archery, 95.0, 105.5 );
            SetSkill( SkillName.Macing, 85.0, 105.5 );
            SetSkill( SkillName.MagicResist, 165.0, 195.5 );
            SetSkill( SkillName.Swords, 85.0, 105.5 );
            SetSkill( SkillName.Tactics, 115.0, 119.5 );
            SetSkill( SkillName.Wrestling, 100.1, 105.3 );
            SetSkill( SkillName.Parry, 105.0, 110.0 );
            SetSkill( SkillName.Lumberjacking, 100.0, 120.0 );
            SetSkill( SkillName.Fencing, 85.0, 120.0 );

            Fame = -100;
            Karma = -10000;

            VirtualArmor = 50;

            PackItem( new Bandage( Utility.RandomMinMax( 1, 15 ) ) );
            AddItem( new Boots( Utility.RandomNeutralHue() ) );
            AddItem( new PlateChest() );
            AddItem( new PlateLegs() );
            AddItem( new PlateArms() );
            AddItem( new PlateGloves() );
            AddItem( new PlateGorget() );
            AddItem( new PlateHelm() );

            Item shield = new OrderShield();
            shield.Movable = false;
            AddItem( shield );

            AddItem( new Cloak() );

            switch( Utility.Random( 3 ) )
            {
                case 0:
                    AddItem( new ShortSpear() );
                    break;
                case 1:
                    AddItem( new Longsword() );
                    break;
                case 2:
                    AddItem( new VikingSword() );
                    break;
            }

            var Hair = new Item( 0x203C );
            Hair.Hue = Utility.RandomHairHue();
            Hair.Layer = Layer.Hair;
            Hair.Movable = false;
            AddItem( Hair );
        }

        public OrderWarrior( Serial serial )
            : base( serial )
        {
        }

        public override int Meat
        {
            get { return 1; }
        }

        public override bool ShowFameTitle
        {
            get { return false; }
        }

        public override OppositionGroup OppositionGroup
        {
            get { return OppositionGroup.OrderAndChaos; }
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Poor );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }
}