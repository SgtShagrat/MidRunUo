/***************************************************************************
 *                               Ahnor.cs
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
    public sealed class Ahnor : TownSystem
    {
        public Ahnor()
        {
            m_Definition = new TownDefinition(  "Ahnor", 
                                                MidgardTowns.Ahnor,
                                                "Ahnor",
                                                new CityInfo( "Ahnor", "Center", 6044, 3820, 22, Map.Felucca ),
                                                TownBanFlag.Ahnor,
                                                new Point3D( 6061, 3701, 21 )
                                                );
        }
    }
}