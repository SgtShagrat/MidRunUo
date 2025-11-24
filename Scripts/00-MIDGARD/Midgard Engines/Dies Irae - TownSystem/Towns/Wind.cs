/***************************************************************************
 *                               Wind.cs
 *                            --------------------
 *   begin                : 22 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public sealed class Wind : TownSystem
    {
        public Wind()
        {
            m_Definition = new TownDefinition( "Wind",
                                                MidgardTowns.Wind,
                                                "Wind",
                                                new CityInfo( "Wind", "Center", 0, 0, 0, Map.Felucca ),
                                                TownBanFlag.Wind,
                                                new Point3D( 5205, 141, 5 )
                                                );

            m_VendorScalars.Add( typeof( Mage ), 0.85 );
            m_VendorScalars.Add( typeof( Scribe ), 0.85 );
            m_VendorScalars.Add( typeof( Alchemist ), 0.85 );
            m_VendorScalars.Add( typeof( Provisioner ), 1.10 );
        }

        private static Dictionary<Type, double> m_VendorScalars = new Dictionary<Type, double>();

        public override double GetDefaultScalar( Type vendorType )
        {
            double scalar = 1.0;
            m_VendorScalars.TryGetValue( vendorType, out scalar );

            return scalar;
        }
    }
}