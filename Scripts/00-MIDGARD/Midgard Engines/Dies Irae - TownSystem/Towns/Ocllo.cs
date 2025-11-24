/***************************************************************************
 *                               Ocllo.cs
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
    public sealed class Ocllo : TownSystem
    {
        public Ocllo()
        {
            m_Definition = new TownDefinition( "Ocllo",
                                                MidgardTowns.Ocllo,
                                                "Ocllo",
                                                new CityInfo( "Ocllo", "Center", 0, 0, 0, Map.Felucca ),
                                                TownBanFlag.Ocllo,
                                                new Point3D( 3660, 2548, 5 )
                                               );

            FieldDefinitions = null;

            m_VendorScalars.Add( typeof( Alchemist ), 0.90 );
            m_VendorScalars.Add( typeof( Mage ), 0.90 );
            m_VendorScalars.Add( typeof( Scribe ), 0.90 );
            m_VendorScalars.Add( typeof( Weaponsmith ), 1.20 );
            m_VendorScalars.Add( typeof( Armorer ), 1.20 );
            m_VendorScalars.Add( typeof( Tailor ), 1.35 );
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