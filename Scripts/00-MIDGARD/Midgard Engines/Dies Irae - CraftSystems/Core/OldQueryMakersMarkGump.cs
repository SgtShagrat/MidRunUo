using System;

using Midgard.Gumps;

using Server;
using Server.Commands;
using Server.Engines.Craft;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.OldCraftSystem
{
    public class MarkCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register( "Marchio", AccessLevel.Player, new CommandEventHandler( Mark_OnCommand ) );
        }

        [Usage( "Marchio" )]
        [Description( "Decide cosa succedera' nel caso si crei un oggetto di qualità eccezionale (marcare, non marcare, chiedere se marcare)" )]
        private static void Mark_OnCommand( CommandEventArgs e )
        {
            Midgard2PlayerMobile m2Pm = e.Mobile as Midgard2PlayerMobile;
            if( m2Pm == null )
                return;

            string msg = "";
            switch( m2Pm.CraftMark )
            {
                case CraftMarkOption.MarkItem:
                    m2Pm.CraftMark = CraftMarkOption.DoNotMark;
                    msg = m2Pm.Language == "ITA" ? "di non marchiare" : "not to mark";
                    break;
                case CraftMarkOption.DoNotMark:
                    m2Pm.CraftMark = CraftMarkOption.PromptForMark;
                    msg = m2Pm.Language == "ITA" ? "di richedere di marchiare" : "to be prompted to mark";
                    break;
                case CraftMarkOption.PromptForMark:
                    m2Pm.CraftMark = CraftMarkOption.MarkItem;
                    msg = m2Pm.Language == "ITA" ? "di marchiare" : "to to mark";
                    break;
                default:
                    break;
            }

            m2Pm.LocalOverheadMessage( MessageType.Regular, 0x3B2, true, string.Format( m2Pm.Language == "ITA" ? "Hai scelto {0} qualsiasi creazione eccezionale." : "Thou have chosen {0} any exceptional craft", msg ) );
        }
    }

    public class OldQueryMakersMarkGump : SmallConfirmGump
    {
        private int m_Quality;
        private Mobile m_From;
        private CraftItem m_CraftItem;
        private CraftSystem m_CraftSystem;
        private Type m_TypeRes;
        private BaseTool m_Tool;

        public OldQueryMakersMarkGump( int quality, Mobile from, CraftItem craftItem, CraftSystem craftSystem, Type typeRes, BaseTool tool )
            : base( from.Language == "ITA" ? "Vuoi marchiare l'oggetto?" : "Do you wish to mark the item?", null )
        {
            from.CloseGump( typeof( OldQueryMakersMarkGump ) );

            m_Quality = quality;
            m_From = from;
            m_CraftItem = craftItem;
            m_CraftSystem = craftSystem;
            m_TypeRes = typeRes;
            m_Tool = tool;
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            bool makersMark = ( info.ButtonID == 1 );

            if( makersMark )
                m_From.SendLocalizedMessage( 501808 ); // You mark the item.
            else
                m_From.SendLocalizedMessage( 501809 ); // Cancelled mark.

		if (m_CraftItem!=null && m_Tool != null )
			m_CraftItem.CompleteCraft( m_Quality, makersMark, m_From, m_CraftSystem, m_TypeRes, m_Tool, null );
        }
    }
}