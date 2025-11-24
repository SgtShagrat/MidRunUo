/***************************************************************************
 *                               Moonglow.cs
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
    public sealed class Moonglow : TownSystem
    {
        public Moonglow()
        {
            m_Definition = new TownDefinition( "Moonglow",
                                                MidgardTowns.Moonglow,
                                                "Moonglow",
                                                new CityInfo( "Moonglow", "Center", 0, 0, 0, Map.Felucca ),
                                                TownBanFlag.Moonglow,
                                                new Point3D( 4479, 1122, 5 )
                                               );

            FieldDefinitions = null;

            m_VendorScalars.Add( typeof( Alchemist ), 0.80 );
            m_VendorScalars.Add( typeof( Mage ), 0.80 );
            m_VendorScalars.Add( typeof( Scribe ), 0.80 );
            m_VendorScalars.Add( typeof( Jeweler ), 0.95 );
            m_VendorScalars.Add( typeof( InnKeeper ), 0.90 );
            m_VendorScalars.Add( typeof( Weaponsmith ), 1.25 );
            m_VendorScalars.Add( typeof( Armorer ), 1.25 );
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