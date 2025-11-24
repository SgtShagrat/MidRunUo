/***************************************************************************
 *                                      Librarian.cs
 *                            		--------------------
 *  begin                	: Marzo, 2007
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 *  		Venditore di libri per spell, comuni e rari.
 *  
 ***************************************************************************/

using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class Librarian : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

        [Constructable]
        public Librarian()
            : base( "the Librarian" )
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
            SetSkill( SkillName.Inscribe, 90.0, 100.0 );
            SetSkill( SkillName.Magery, 90.0, 100.0 );
            SetSkill( SkillName.Meditation, 90.0, 100.0 );
            SetSkill( SkillName.MagicResist, 90.0, 100.0 );
            SetSkill( SkillName.Wrestling, 90.0, 100.0 );

            // Fama e Karma
            Fame = 10000;
            Karma = 10000;

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
                Title = ", the pretty librarian";
                Body = 0x191;
            }
            else
            {
                Name = NameList.RandomName( "male" );
                Title = ", the librarian";
                Body = 0x190;
            }

            // Equip:
            // 	Robe
            if( FindItemOnLayer( Layer.OuterTorso ) == null )
            {
                Item robe = new Item( 0x3DBA );
                robe.Name = " a threadbare robe";
                robe.Hue = 2569;
                robe.Layer = Layer.OuterTorso;
                robe.LootType = LootType.Blessed;
                robe.Movable = false;
                AddItem( robe );
            }

            // 	Sandals
            if( FindItemOnLayer( Layer.Shoes ) == null )
            {
                Sandals sandals = new Sandals();
                sandals.Hue = 1;
                sandals.LootType = LootType.Blessed;
                sandals.Movable = false;
                AddItem( sandals );
            }

            // Staff
            PhantomStaff staff = new PhantomStaff();
            staff.LootType = LootType.Blessed;
            staff.Name = "Staff of Wisdom";
            staff.ItemID = 0x3D92;
            AddItem( staff );
        }

        public override void InitSBInfo()
        {
            if( Map != null && Map == Map.Tokuno )
            {
                m_SBInfos.Add( new SBTokunoLibrarian() );
            }
            else
            {
                m_SBInfos.Add( new SBLibrarian() );
            }
        }

        public override void OnAfterSpawn()
        {
            base.OnAfterSpawn();

            if( Map == Map.Tokuno )
            {
                SetSkill( SkillName.Bushido, 90.0, 100.0 );
                SetSkill( SkillName.Ninjitsu, 90.0, 100.0 );
            }
        }

        public Librarian( Serial serial )
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
