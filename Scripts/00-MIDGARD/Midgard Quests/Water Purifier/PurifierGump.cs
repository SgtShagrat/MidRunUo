using Server.Network;

namespace Server.Gumps
{
    public class PurifierGump : Gump
    {
        public PurifierGump()
            : base( 35, 70 )
        {
            AddImage( 0, 0, 0x1F4 );
            AddHtmlLocalized( 40, 17, 150, 220, 1055141, false, false );
            // This device is used to purify water. 
            // For it to work, three key ingredients are required: 
            // a plague beast core, 
            // some moonfire brew, 
            // and a fragment of obsidian.
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
        }
    }
}