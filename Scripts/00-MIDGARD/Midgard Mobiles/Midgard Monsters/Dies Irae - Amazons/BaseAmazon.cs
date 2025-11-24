/***************************************************************************
 *                               BaseAmazon.cs
 *
 *   begin                : 15 November, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    public abstract class BaseAmazon : BaseCreature
    {
        protected BaseAmazon( AIType ai, string title )
            : base( ai, FightMode.Closest, 10, 1, 0.15, 0.4 )
        {
            SpeechHue = Utility.RandomDyedHue();
            Title = title;

            Hue = Utility.RandomSkinHue();
            Body = 0x191;
            Female = true;
            Name = NameList.RandomName( "female" );

            SetSkill( SkillName.MagicResist, 165.0, 195.5 );
            SetSkill( SkillName.Tactics, 95.0, 100.5 );
            SetSkill( SkillName.Wrestling, 100.1, 105.3 );
            SetSkill( SkillName.Parry, 105.0, 110.0 );
            SetSkill( SkillName.Anatomy, 100.0, 120.0 );

            Fame = 10000;
            Karma = -10000;

            VirtualArmor = 50;

            HairItemID = Utility.RandomList( 0x203D, 0x2049, 0x2044, 0x203C );
        }

        public static Item Rehued( Item item, int hue )
        {
            item.Hue = hue;
            return item;
        }

        public override bool AlwaysMurderer { get { return true; } }

        #region serialization
        public BaseAmazon( Serial serial )
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