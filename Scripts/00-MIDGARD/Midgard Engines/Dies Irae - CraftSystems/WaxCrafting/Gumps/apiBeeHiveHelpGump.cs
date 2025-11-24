using Server;
using Server.Gumps;

namespace Midgard.Engines.Apiculture
{
    public class ApiBeeHiveHelpGump : Gump
    {
        public ApiBeeHiveHelpGump( Mobile from, int type ) : base( 20, 20 )
        {
            Closable=true;
            Disposable=true;
            Dragable=true;
            Resizable=false;

            AddPage(0);
            AddBackground(37, 25, 386, 353, 3600);
            AddLabel(177, 42, 92, @"Apiculture Help");

            AddItem(32, 277, 3311);
            AddItem(30, 193, 3311);
            AddItem(29, 107, 3311);
            AddItem(28, 24, 3311);
            AddItem(386, 277, 3307);
            AddItem(387, 191, 3307);
            AddItem(388, 108, 3307);
            AddItem(385, 26, 3307);

            if( type == 0 )
                AddHtmlLocalized( 59, 67, 342, 257, 1065394, true, true);
            else
                AddHtmlLocalized( 59, 67, 342, 257, 1065395, true, true);

            AddButton(202, 333, 247, 248, 0, GumpButtonType.Reply, 0);
        }
    }
}