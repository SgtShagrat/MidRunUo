using Server.Gumps;

namespace Midgard.Engines.WelcomeSystem
{
    internal class WelcomeGump : Gump
    {
        public WelcomeGump()
            : base( 0, 0 )
        {
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage( 0 );
            AddBackground( 128, 205, 532, 384, 9250 );
            AddImage( 243, -24, 1418 );
            AddBackground( 144, 220, 501, 353, 9350 );
            AddBackground( 166, 295, 457, 264, 2620 );
            AddAlphaRegion( 172, 302, 444, 249 );
            AddImage( 76, 174, 10440 );
            AddImage( 627, 174, 10441 );
            AddImageTiled( 280, 242, 226, 31, 87 );

            AddHtml( 172, 302, 444, 249, Core.News, false, true );

            AddImage( 268, 228, 83 );
            AddImageTiled( 283, 226, 222, 20, 84 );

            AddLabel( 310, 236, 1154, @"Welcome on Midgard Shard!" );

            AddImage( 305, 259, 96 );
            AddImage( 484, 250, 97 );
            AddImage( 296, 250, 95 );
            AddImage( 505, 228, 85 );
            AddImageTiled( 266, 244, 14, 31, 86 );
            AddImageTiled( 506, 243, 14, 26, 88 );
            AddImage( 268, 267, 89 );
            AddImage( 505, 267, 91 );
            AddImageTiled( 284, 269, 222, 12, 90 );
        }
    }
}