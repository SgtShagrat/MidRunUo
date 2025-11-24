using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Druid : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

        public override NpcGuild NpcGuild { get { return NpcGuild.MagesGuild; } }

        [Constructable]
        public Druid()
            : base( "the Druid" )
        {
            // Stats
            SetStr( 304, 400 );
            SetDex( 102, 150 );
            SetInt( 204, 300 );

            SetHits( 66, 125 );
            SetDamage( 30, 50 );

            // Resistances
            SetResistance( ResistanceType.Physical, 90, 100 );
            SetResistance( ResistanceType.Fire, 90, 100 );
            SetResistance( ResistanceType.Cold, 90, 100 );
            SetResistance( ResistanceType.Poison, 90, 100 );
            SetResistance( ResistanceType.Energy, 90, 100 );

            // Skills
            SetSkill( SkillName.EvalInt, 90.0, 100.0 );
            SetSkill( SkillName.AnimalTaming, 90.0, 100.0 );
            SetSkill( SkillName.Magery, 90.0, 100.0 );
            SetSkill( SkillName.Meditation, 90.0, 100.0 );
            SetSkill( SkillName.AnimalLore, 90.0, 100.0 );
            SetSkill( SkillName.Macing, 90.0, 100.0 );

            // Fama e Karma
            Fame = 10000;
            Karma = 0;

            // Hair
            HairItemID = 0x203B;
            HairHue = Utility.RandomHairHue();

            // Speech & Skin hues
            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();

            // Genre
            if( Female = Utility.RandomBool() )
            {
                Name = NameList.RandomName( "female" );
                Title = ", the wise druid";
                Body = 0x191;
            }
            else
            {
                Name = NameList.RandomName( "male" );
                Title = ", the wise druid";
                Body = 0x190;
            }

            // Equip:
            // 	Robe
            /*
            Item robe = new Item( 0x3DB9 );
            robe.Name = " a robe of nature";
            robe.Hue = 2025;
            robe.Layer = Layer.OuterTorso;
            robe.LootType = LootType.Blessed;
            robe.Movable = false;
            AddItem( robe );

            // Staff
            WildStaff staff = new WildStaff();
            staff.LootType = LootType.Blessed;
            staff.Name = "Staff of Nature";
            staff.Hue = 2025;
            AddItem( staff );
            */
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add( new SBDruid() );
        }

        public override VendorShoeType ShoeType
        {
            get { return VendorShoeType.Sandals; }
        }

        public Druid( Serial serial )
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