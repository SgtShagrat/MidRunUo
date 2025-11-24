/***************************************************************************
 *                               Yew.cs
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
    public sealed class Yew : TownSystem
    {
        public Yew()
        {
            m_Definition = new TownDefinition( "Yew",
                                                MidgardTowns.Yew,
                                                "Yew",
                                                new CityInfo( "Yew", "Center", 0, 0, 0, Map.Felucca ),
                                                TownBanFlag.Yew,
                                                new Point3D( 535, 992, 15 )
                                               );

            FieldDefinitions = null;

            m_VendorScalars.Add( typeof( Bowyer ), 0.75 );
            m_VendorScalars.Add( typeof( LeatherWorker ), 0.85 );
            m_VendorScalars.Add( typeof( Weaponsmith ), 1.10 );
            m_VendorScalars.Add( typeof( Armorer ), 1.20 );
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