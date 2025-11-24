/***************************************************************************
 *                               Marble.cs
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
    public sealed class Marble : TownSystem
    {
        public Marble()
        {
            m_Definition = new TownDefinition(  "Marble",
                                                MidgardTowns.Marble,
                                                "Marble",
                                                new string[] { "Marble Tree Houses" },
                                                new CityInfo( "Marble", "Center", 0, 0, 0, Map.Felucca ),
                                                TownBanFlag.Marble,
                                                new Point3D( 1903, 2082, 5 )
                                               );
        }
    }
}