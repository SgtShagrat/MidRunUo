/***************************************************************************
 *                               FishingGump.cs
 *
 *   begin                : 19 settembre 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.AdvancedFishing
{
    public delegate void FishingGumpCallback( Mobile from, Actions action, SpecialFishingPole pole );

    public class FishingGump : Gump
    {
        private readonly FishingGumpCallback m_Callback;
        private readonly SpecialFishingPole m_Pole;

        public FishingGump( SpecialFishingPole pole, FishingGumpCallback callback )
            : base( 50, 50 )
        {
            m_Callback = callback;
            m_Pole = pole;

            Closable = true;

            AddPage( 0 );
            AddBackground( 0, 0, 200, 100, 0x18A );

            AddButton(  40, 40, 116, 117, (int)Actions.Jump, GumpButtonType.Reply, 0 );
            AddButton(  90, 40, 118, 119, (int)Actions.War,  GumpButtonType.Reply, 0 );
            AddButton( 140, 40, 114, 115, (int)Actions.Down, GumpButtonType.Reply, 0 );
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            if( m_Callback == null )
                return;

            switch ( info.ButtonID )
            {
                case (int)Actions.Jump:
                    m_Callback( sender.Mobile, Actions.Jump, m_Pole );
                    break;
                case (int)Actions.War:
                    m_Callback( sender.Mobile, Actions.War, m_Pole );
                    break;
                case (int)Actions.Down:
                    m_Callback( sender.Mobile, Actions.Down, m_Pole );
                    break;
                default:
                    m_Callback( sender.Mobile, Actions.None, m_Pole );
                    break;
            }
        }
    }
}