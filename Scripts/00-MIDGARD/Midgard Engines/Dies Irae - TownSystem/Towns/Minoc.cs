/***************************************************************************
 *                               Minoc.cs
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
    public sealed class Minoc : TownSystem
    {
        public Minoc()
        {
            m_Definition = new TownDefinition( "Minoc",
                                                MidgardTowns.Minoc,
                                                "Minoc",
                                                new CityInfo( "Minoc", "Center", 0, 0, 0, Map.Felucca ),
                                                TownBanFlag.Minoc,
                                                new Point3D( 2521, 558, 5 )
                                               );

            m_VendorScalars.Add( typeof( Weaponsmith ), 0.80 );
            m_VendorScalars.Add( typeof( Armorer ), 0.80 );
            m_VendorScalars.Add( typeof( Carpenter ), 0.90 );
            m_VendorScalars.Add( typeof( Tailor ), 0.90 );
            m_VendorScalars.Add( typeof( LeatherWorker ), 0.90 );
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