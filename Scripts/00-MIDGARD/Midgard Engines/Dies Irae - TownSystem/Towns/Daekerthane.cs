/***************************************************************************
 *                               Daekerthane.cs
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
    public sealed class Daekerthane : TownSystem
    {
        public Daekerthane()
        {
            m_Definition = new TownDefinition(  "Daekerthane",
                                                MidgardTowns.Daekerthane,
                                                "Daekerthane",
                                                new CityInfo( "Daekerthane", "Center", 0, 0, 0, Map.Felucca ),
                                                TownBanFlag.Daekerthane,
                                                new Point3D( 1133, 1990, 50 )
                                                );
        }
    }
}