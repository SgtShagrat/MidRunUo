/***************************************************************************
 *                                  RankInfoGump.cs
 *                            		-------------------
 *  begin                	: Maggio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public class RankInfoGump : TownGump
    {
        public enum Buttons
        {
            Close
        }

        public RankInfoGump( TownSystem system, Mobile citizen, Mobile owner )
            : base( system, owner, citizen, 50, 50 )
        {
            owner.CloseGump( typeof( RankInfoGump ) );

            Design();

            base.RegisterUse( typeof( RankInfoGump ) );
        }

        private static void Design()
        {
        }

        public override void OnResponse( NetState sender, RelayInfo info )
        {
        }
    }
}