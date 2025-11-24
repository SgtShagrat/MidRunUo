/***************************************************************************
 *                               DrowMageRider.cs
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
    public class DrowMageRider : BaseDrow
    {
        [Constructable]
        public DrowMageRider()
            : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Title = Female ? ", the drow priestess" : ", the drow mage";

            SetStr( 155, 170 );
            SetDex( 95, 135 );
            SetInt( 130, 200 );

            SetHits( 250, 300 );

            SetDamage( 10, 23 );

            SetSkill( SkillName.EvalInt, 95.0, 115.5 );
            SetSkill( SkillName.Magery, 100.0, 120.5 );
            SetSkill( SkillName.MagicResist, 175.0, 215.5 );
            SetSkill( SkillName.Anatomy, 85.0, 95.5 );
            SetSkill( SkillName.Tactics, 95.0, 107.5 );
            SetSkill( SkillName.Wrestling, 105.0, 117.5 );
            SetSkill( SkillName.Meditation, 95.6, 105.4 );

            Fame = 10000;
            Karma = -10000;

            VirtualArmor = 45;

            if( 0.5 >= Utility.RandomDouble() )
                PackItem( new ExecutionersCap( 3 ) );

            if( Female )
                AddItem( new Skirt( Utility.RandomNeutralHue() ) );
            else
                AddItem( new Kilt( Utility.RandomNeutralHue() ) );

            AddItem( new Robe( Utility.RandomNeutralHue() ) );
            AddItem( new Boots( Utility.RandomNeutralHue() ) );
            AddItem( new Cloak() );

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
        public DrowMageRider( Serial serial )
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