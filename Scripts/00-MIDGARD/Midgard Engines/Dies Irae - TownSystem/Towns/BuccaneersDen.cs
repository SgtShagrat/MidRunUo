/***************************************************************************
 *                                  BuccaneersDen.cs
 *                            		----------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Menus.Questions;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public sealed class BuccaneersDen : TownSystem
    {
        public BuccaneersDen()
        {
            m_Definition = new TownDefinition( "Buccaneer's Den",
                                                MidgardTowns.BuccaneersDen,
                                                "Buccaneer's Den",
                                                new CityInfo( "Buccaneer's Den", "Center", 0, 0, 0, Map.Felucca ),
                                                TownBanFlag.BuccaneersDen,
                                                new Point3D( 2708, 2227, 5 )
                                                );

            FieldDefinitions = null;

            m_VendorScalars.Add( typeof( Thief ), 0.9 );
            m_VendorScalars.Add( typeof( Fisherman ), 0.95 );
        }

        public override int AccessCost
        {
            get { return 1000; }
        }

        public override bool AcceptCitizens
        {
            get { return true; }
        }

        private static Dictionary<Type, double> m_VendorScalars = new Dictionary<Type, double>();

        public override double GetDefaultScalar( Type vendorType )
        {
            double scalar = 1.0;
            m_VendorScalars.TryGetValue( vendorType, out scalar );

            return scalar;
        }

        public override bool IsMurdererTown
        {
            get { return true; }
        }

        public override bool IsUnderNoHousingCriteria( Point3D p )
        {
            return false;
        }

        public override bool AllowMurderResurrection
        {
            get { return true; }
        }

        public override void DressTownVendor( Mobile mobile )
        {
            for( int i = 0; i < mobile.Items.Count; i++ )
            {
                Item item = mobile.Items[ i ];

                if( !item.Deleted )
                    item.Delete();
            }

            if( mobile.Female )
            {
                mobile.EquipItem( Immovable( new Sandals( 0 ) ) );
                mobile.EquipItem( Immovable( new Kilt( Utility.RandomDyedHue() ) ) );
                mobile.EquipItem( Immovable( new Shirt( 0 ) ) );
                mobile.EquipItem( Immovable( new GoldEarrings() ) );
                mobile.EquipItem( Immovable( new SkullCap( Utility.RandomRedHue() ) ) );
            }
            else
            {
                mobile.EquipItem( Immovable( new Shoes( 0 ) ) );
                mobile.EquipItem( Immovable( new ShortPants( Utility.RandomBlueHue() ) ) );
                mobile.EquipItem( Immovable( new Shirt( 0 ) ) );
                mobile.EquipItem( Immovable( new SilverBeadNecklace() ) );
                mobile.EquipItem( Immovable( new SilverEarrings() ) );
                mobile.EquipItem( Immovable( new SkullCap( Utility.RandomRedHue() ) ) );
                mobile.EquipItem( Immovable( new SilverBracelet() ) );
            }
        }

        public override bool CanAccessTownServices( Mobile citizenDealer, Mobile from, bool message, bool dropAccessCost )
        {
            return true;
        }

        private static StuckMenuEntry[] m_StuckEntries = new StuckMenuEntry[]
		{
			// Buccanner's Den
			new StuckMenuEntry( 1019001, new Point3D[]
				{
					new Point3D( 2700, 2233, 0 ),
					new Point3D( 2683, 2174, 0 ),
					new Point3D( 2728, 2116, 0 ),
					new Point3D( 2717, 2178, 0 ),
					new Point3D( 2716, 2218, 0 )
				} )
        };

        public override StuckMenuEntry[] GetStuckEntries()
        {
            return m_StuckEntries;
        }
    }
}