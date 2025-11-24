/***************************************************************************
 *                               Cove.cs
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
    public sealed class Cove : TownSystem
    {
        public Cove()
        {
            m_Definition = new TownDefinition( "Cove",
                                                MidgardTowns.Cove,
                                                "Cove",
                                                new CityInfo( "Cove", "Center", 0, 0, 0, Map.Felucca ),
                                                TownBanFlag.Cove,
                                                new Point3D( 2265, 1201, 6 )
                                                );

            m_VendorScalars.Add( typeof( Weaponsmith ), 0.90 );
            m_VendorScalars.Add( typeof( Armorer ), 0.90 );
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

        public override bool CanAccessTownServices( Mobile citizenDealer, Mobile from, bool message, bool dropAccessCost )
        {
            return true;
        }
    }
}