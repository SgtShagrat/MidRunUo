/***************************************************************************
 *                               Papua.cs
 *                            --------------------
 *   begin                : 22 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public sealed class Papua : TownSystem
    {
        public Papua()
        {
            m_Definition = new TownDefinition(  "Papua",
                                                MidgardTowns.Papua,
                                                "Papua",
                                                new CityInfo( "Papua", "Center", 0, 0, 0, Map.Felucca ),
                                                TownBanFlag.Papua,
                                                new Point3D( 5708, 3220, 3 )
                                               );
        }
    }
}