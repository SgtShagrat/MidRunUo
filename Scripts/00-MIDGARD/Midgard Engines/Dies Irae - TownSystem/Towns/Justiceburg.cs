/***************************************************************************
 *                               Justiceburg.cs
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
    public sealed class Justiceburg : TownSystem
    {
        public Justiceburg()
        {
            m_Definition = new TownDefinition(  "Justiceburg",
                                                MidgardTowns.Justiceburg,
                                                "Justiceburg",
                                                new CityInfo( "Justiceburg", "Center", 0, 0, 0, Map.Felucca ),
                                                TownBanFlag.Justiceburg,
                                                new Point3D( 1308, 663, 7 )
                                               );
        }
    }
}