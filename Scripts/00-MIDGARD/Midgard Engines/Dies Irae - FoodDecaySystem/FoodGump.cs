using Server;
using Server.Gumps;

namespace Midgard.Engines.FoodDecaySystem
{
    public class FoodGump : Gump
    {
        private Mobile m_From;

        public FoodGump( Mobile from )
            : base( 50, 50 )
        {
            m_From = from;

            m_From.CloseGump( typeof( FoodGump ) );

            int t = m_From.Hunger + m_From.Thirst;
            int percStatLoss = ( -2 * t + 60 < 0 ) ? 0 : ( -2 * t + 60 );

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage( 0 );
            AddBackground( 0, 0, 384, 148, 2600 );

            AddLabel( 110, 30, 0, @"Midgard Hunger:" );
            AddItem( 80, 56, 2505 );
            AddItem( 80, 80, 2477 );
            AddLabel( 128, 56, 0, @"La tua fame è di:" );
            AddLabel( 128, 80, 0, @"La tua sete è di:" );
            AddLabel( 32, 104, 0, @"Il tuo stato attuale comporta uno statloss del " );
            AddLabel( 248, 56, 0, m_From.Hunger + " su 20" );
            AddLabel( 248, 80, 0, m_From.Thirst + " su 20" );
            AddLabel( 315, 104, 0, percStatLoss + "%." );
        }
    }
}