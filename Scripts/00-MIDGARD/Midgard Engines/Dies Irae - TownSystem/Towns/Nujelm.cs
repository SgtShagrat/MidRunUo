/***************************************************************************
 *                               Nujelm.cs
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
    public sealed class Nujelm : TownSystem
    {
        public Nujelm()
        {
            m_Definition = new TownDefinition( "Nujel'm",
                                                MidgardTowns.Nujelm,
                                                "Nujel'm",
                                                new CityInfo( "Nujel'm", "Center", 1475, 1626, 20, Map.Felucca ),
                                                TownBanFlag.Nujelm,
                                                new Point3D( 3728, 1279, 5 )
                                               );

            m_VendorScalars.Add( typeof( Jeweler ), 0.85 );
            m_VendorScalars.Add( typeof( InnKeeper ), 0.85 );
            m_VendorScalars.Add( typeof( Weaponsmith ), 1.25 );
            m_VendorScalars.Add( typeof( Armorer ), 1.25 );
            m_VendorScalars.Add( typeof( Tailor ), 1.25 );
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