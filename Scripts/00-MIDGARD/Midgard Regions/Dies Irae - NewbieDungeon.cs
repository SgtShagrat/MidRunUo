/***************************************************************************
 *                                  NewbieDungeon.cs
 *                            		-------------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System.Xml;
using Server.Mobiles;

namespace Server.Regions
{
    public class NewbieDungeon : DungeonRegion
    {
        public NewbieDungeon( XmlElement xml, Map map, Region parent )
            : base( xml, map, parent )
        {
        }

        public override bool AllowHarmful( Mobile from, Mobile target )
        {
            if( from != null && target != null && from.Player && from.AccessLevel == AccessLevel.Player && target is PlayerMobile )
            {
                from.SendMessage( "You cannot perform harmful acts on your target." );
                return false;
            }

            return base.AllowHarmful( from, target );
        }
    }
}