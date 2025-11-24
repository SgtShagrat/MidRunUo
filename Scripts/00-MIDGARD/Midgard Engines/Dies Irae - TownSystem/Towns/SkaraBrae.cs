/***************************************************************************
 *                               SkaraBrae.cs
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
    public sealed class SkaraBrae : TownSystem
    {
        public SkaraBrae()
        {
            m_Definition = new TownDefinition( "Skara Brae",
                                                MidgardTowns.SkaraBrae,
                                                "Skara Brae",
                                                new CityInfo( "Skara Brae", "Center", 0, 0, 0, Map.Felucca ),
                                                TownBanFlag.SkaraBrae,
                                                new Point3D( 580, 2158, 5 )
                                               );
            FieldDefinitions = null;

            m_VendorScalars.Add( typeof( Carpenter ), 0.80 );
            m_VendorScalars.Add( typeof( AnimalTrainer ), 1.10 );
            m_VendorScalars.Add( typeof( Shipwright ), 1.10 );
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