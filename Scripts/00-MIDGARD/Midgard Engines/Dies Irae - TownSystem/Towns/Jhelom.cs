/***************************************************************************
 *                               Jhelom.cs
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
    public sealed class Jhelom : TownSystem
    {
        public Jhelom()
        {
            m_Definition = new TownDefinition( "Jhelom",
                                                MidgardTowns.Jhelom,
                                                "Jhelom",
                                                new CityInfo( "Jhelom", "Center", 0, 0, 0, Map.Felucca ),
                                                TownBanFlag.Jhelom,
                                                new Point3D( 1396, 3789, 5 )
                                               );

            FieldDefinitions = null;

            m_VendorScalars.Add( typeof( Tailor ), 0.95 );
            m_VendorScalars.Add( typeof( Butcher ), 0.90 );
            m_VendorScalars.Add( typeof( Baker ), 0.90 );
            m_VendorScalars.Add( typeof( Farmer ), 0.90 );
            m_VendorScalars.Add( typeof( Carpenter ), 0.85 );
            m_VendorScalars.Add( typeof( LeatherWorker ), 1.05 );
            m_VendorScalars.Add( typeof( Alchemist ), 1.18 );
            m_VendorScalars.Add( typeof( Mage ), 1.18 );
            m_VendorScalars.Add( typeof( Scribe ), 1.18 );
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