/***************************************************************************
 *                                     Gardener.cs
 *                            		------------------
 *  begin                	: Giugno, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			NPC Gardener
 * 
 ***************************************************************************/

using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Midgard.Engines.WineCrafting
{
    public class Gardener : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

        [Constructable]
        public Gardener()
            : base( "the Gardener" )
        {
            SetSkill( SkillName.Alchemy, 100.0, 120.0 );
            SetSkill( SkillName.Camping, 80.0, 100.0 );
            SetSkill( SkillName.Lumberjacking, 80.0, 100.0 );
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add( new SBGardener() );
        }

        public override VendorShoeType ShoeType
        {
            get { return VendorShoeType.Boots; }
        }

        public override int GetShoeHue()
        {
            return 0;
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            AddItem( new Server.Items.WideBrimHat( Utility.RandomNeutralHue() ) );
        }

        public Gardener( Serial serial )
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