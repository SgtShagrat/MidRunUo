using System;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.WelcomeSystem
{
    internal class ChangeMotdGump : Gump
    {
        public ChangeMotdGump()
            : base( 50, 50 )
        {
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage( 0 );

            AddBackground( 0, 0, 342, 336, 3500 );
            AddLabel( 20, 20, 0, "Change Motd:" );
            AddTextEntry( 20, 50, 300, 230, 0, 1, @"" );
            AddAlphaRegion( 20, 49, 300, 230 );
            AddButton( 220, 290, 1153, 1154, 2, GumpButtonType.Reply, 0 );
            AddLabel( 260, 290, 0, "Add" );
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
            if( info.ButtonID == 0 )
                return;

            TextRelay relay = info.GetTextEntry( 1 );
            string text = ( relay == null ? null : relay.Text.Trim() );

            Console.WriteLine( "text " + text );
            if( String.IsNullOrEmpty( text ) )
            {
                Console.WriteLine( "Failed writing {0}", Config.NewsFilePath );
                return;
            }

            if( !Core.WriteMotd( text, sender.Mobile ) )
                Console.WriteLine( "Failed writing {0}", Config.NewsFilePath );

            Core.Motd = text;
        }
    }
}