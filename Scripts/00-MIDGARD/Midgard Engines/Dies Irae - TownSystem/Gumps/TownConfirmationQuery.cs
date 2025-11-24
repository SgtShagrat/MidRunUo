using Server;
using Server.Network;
using Server.StringQueries;

namespace Midgard.Engines.MidgardTownSystem
{
    public delegate void TownConfirmationQueryCallback( Mobile from, bool okay, string text, object state );

    public class TownConfirmationQuery : StringQuery
    {
        private readonly TownConfirmationQueryCallback m_Callback;
        private readonly object m_State;

        public TownConfirmationQuery( string topText, string entryText, bool numerical, int max, TownConfirmationQueryCallback callback, object state ) :
            base( topText, true, numerical ? StringQueryStyle.Numerical : StringQueryStyle.Normal, max, entryText )
        {
            m_Callback = callback;
            m_State = state;
        }

        public override void OnResponse( NetState sender, bool okay, string text )
        {
            if( m_Callback != null )
            {
                m_Callback( sender.Mobile, okay, text, m_State );
            }
        }
    }
}