/***************************************************************************
 *                               DrowMage.cs
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
    public class DrowMage : BaseDrow
    {
        [Constructable]
        public DrowMage()
            : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Title = Female ? ", the drow priestess" : ", the drow mage";

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

            Fame = 10000;
            Karma = -10000;

            VirtualArmor = 45;

            if( 0.5 >= Utility.RandomDouble() )
                PackItem( new ExecutionersCap( 3 ) );

            AddItem( new Robe( Utility.RandomNeutralHue() ) );
            AddItem( new Boots( Utility.RandomNeutralHue() ) );
            AddItem( new Cloak() );
        }

        public override void GenerateLoot()
        {
            AddLoot( LootPack.Meager );
            AddLoot( LootPack.Potions );
        }

        #region serialization
        public DrowMage( Serial serial )
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