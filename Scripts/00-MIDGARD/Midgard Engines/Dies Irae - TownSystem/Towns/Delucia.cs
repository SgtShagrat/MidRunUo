/***************************************************************************
 *                               Delucia.cs
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
    public sealed class Delucia : TownSystem
    {
        public Delucia()
        {
            m_Definition = new TownDefinition(  "Delucia",
                                                MidgardTowns.Delucia,
                                                "Delucia",
                                                new CityInfo( "Delucia", "Center", 0, 0, 0, Map.Felucca ),
                                                TownBanFlag.Delucia,
                                                new Point3D( 5209, 4043, 47 )
                                               );
        }
    }
}