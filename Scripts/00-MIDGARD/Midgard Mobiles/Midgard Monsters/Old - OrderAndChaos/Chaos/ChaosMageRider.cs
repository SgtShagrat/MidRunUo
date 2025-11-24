using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    [CorpseName( "an order human corpse" )]
    public class ChaosMageRider : BaseCreature, IVirtueCreature
    {
        [Constructable]
        public ChaosMageRider()
            : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.15, 0.4 )
        {
            Hue = 0;
            Criminal = true;
            if( Female = Utility.RandomBool() )
            {
                Body = 0x191;
                Female = true;
                Name = NameList.RandomName( "female" );
                AddItem( new Skirt( Utility.RandomNeutralHue() ) );
                Title = "the ghost of a Chaos Sorceress Rider";
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName( "male" );
                AddItem( new Kilt( Utility.RandomNeutralHue() ) );
                Title = "the ghost of a Chaos Mage Rider";
            }

            SetStr( 155, 170 );
            SetDex( 95, 135 );
            SetInt( 130, 200 );

            SetHits( 250, 300 );

            SetDamage( 10, 23 );

            SetSkill( SkillName.EvalInt, 140.0, 155.5 );
            SetSkill( SkillName.Magery, 120.0, 140.5 );
            SetSkill( SkillName.MagicResist, 175.0, 215.5 );
            SetSkill( SkillName.Anatomy, 85.0, 95.5 );
            SetSkill( SkillName.Tactics, 95.0, 107.5 );
            SetSkill( SkillName.Wrestling, 105.0, 117.5 );
            SetSkill( SkillName.Meditation, 95.6, 105.4 );

            Fame = -100;
            Karma = -10000;

            VirtualArmor = 45;

            AddItem( new Robe( Utility.RandomNeutralHue() ) );
            AddItem( new Cloak( Utility.RandomNeutralHue() ) );
            AddItem( new Boots( Utility.RandomNeutralHue() ) );

            var Hair = new Item( 0x203C );
            Hair.Hue = Utility.RandomHairHue();
            Hair.Layer = Layer.Hair;
            Hair.Movable = false;
            AddItem( Hair );

            new BlackHorse().Rider = this;
        }

        public ChaosMageRider( Serial serial )
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

        public override bool OnBeforeDeath()
        {
            IMount mount = Mount;

            if( mount != null )
            {
                mount.Rider = null;
            }

            if( mount is Mobile )
            {
                ( (Mobile)mount ).Delete();
            }

            return base.OnBeforeDeath();
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