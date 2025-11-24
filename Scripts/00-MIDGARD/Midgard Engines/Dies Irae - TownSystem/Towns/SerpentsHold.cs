/***************************************************************************
 *                                  SerpentsHold.cs
 *                            		---------------
 *  begin                	: Gennaio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Midgard.Items;

using Server;
using Server.Items;
using Server.Menus.Questions;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public sealed class SerpentsHold : TownSystem
    {
        public SerpentsHold()
        {
            m_Definition = new TownDefinition( "Serpent's Hold",
                                                MidgardTowns.SerpentsHold,
                                                "Serpent's Hold",
                                                new string[] { "Serpent's Hold Mine" },
                                                new CityInfo( "Serpent's Hold", "Center", 3032, 3419, 15, Map.Felucca ),
                                                TownBanFlag.SerpentsHold,
                                                new Point3D( 3031, 3414, 15 )
                                                );

            FieldDefinitions = null;

            m_VendorScalars.Add( typeof( Weaponsmith ), 1.15 );
            m_VendorScalars.Add( typeof( Armorer ), 1.15 );
            m_VendorScalars.Add( typeof( AnimalTrainer ), 1.20 );
            m_VendorScalars.Add( typeof( LeatherWorker ), 1.10 );
            m_VendorScalars.Add( typeof( Butcher ), 0.90 );
            m_VendorScalars.Add( typeof( Baker ), 0.90 );
            m_VendorScalars.Add( typeof( Farmer ), 0.90 );
            m_VendorScalars.Add( typeof( Tailor ), 1.15 );
            m_VendorScalars.Add( typeof( Alchemist ), 1.18 );
            m_VendorScalars.Add( typeof( Mage ), 1.18 );
            m_VendorScalars.Add( typeof( Scribe ), 3.00 );
        }

        public override int AccessCost
        {
            get { return 1000; }
        }

        public override bool AcceptCitizens
        {
            get { return true; }
        }

        public override bool IsUnderNoHousingCriteria( Point3D p )
        {
            if( p.X >= 2720 && p.Y >= 3240 && p.X <= 2720 + 480 && p.Y <= 3240 + 440 )
                return true;
            else
                return false;
        }

        public override void DressTownVendor( Mobile mobile )
        {
            /*
            // Items that will be removed
            Item toDelete;

            // Remove robe
            toDelete = mobile.FindItemOnLayer( Layer.OuterTorso );
            if( toDelete != null && !toDelete.Deleted )
                toDelete.Delete();

            string name = String.Format( "citizen of {0}", Definition.TownName );

            // Add robe with hue and name ot vendor city            
            mobile.EquipItem( Newbied( Rehued( Renamed( new Robe(), name ), 0x819 ) ) );

            // Remove cloak
            toDelete = mobile.FindItemOnLayer( Layer.Cloak );
            if( toDelete != null && !toDelete.Deleted )
                toDelete.Delete();

            // Add cloak with hue and name ot vendor city
            mobile.EquipItem( Newbied( Rehued( Renamed( new Cloak(), name ), 0x99A ) ) );
        
             */
        }

        public override void DressTownGuard( BaseCreature guard )
        {
            guard.InitStats( Utility.Dice( 1, 25, 200 ), Utility.Dice( 1, 25, 160 ), Utility.Dice( 1, 15, 180 ) );

            guard.Title = "the Serpent's Hold guard";
            guard.SpeechHue = Utility.RandomDyedHue();

            guard.Hue = Utility.RandomSkinHue();
            guard.Female = Utility.RandomBool();

            if( guard.Female )
            {
                guard.Body = 401;
                guard.Name = NameList.RandomName( "female" );
            }
            else
            {
                guard.Body = 400;
                guard.Name = NameList.RandomName( "male" );
            }

            Utility.AssignRandomHair( guard );
            Utility.AssignRandomFacialHair( guard, guard.HairHue );

            guard.VirtualArmor = 32;

            guard.SetSkill( SkillName.Swords, 110.0, 120.0 );
            guard.SetSkill( SkillName.Macing, 110.0, 120.0 );
            guard.SetSkill( SkillName.Wrestling, 110.0, 120.0 );
            guard.SetSkill( SkillName.Tactics, 110.0, 120.0 );
            guard.SetSkill( SkillName.MagicResist, 110.0, 120.0 );
            guard.SetSkill( SkillName.Healing, 110.0, 120.0 );
            guard.SetSkill( SkillName.Anatomy, 110.0, 120.0 );
            guard.SetSkill( SkillName.DetectHidden, 110.0, 120.0 );

            guard.AddItem( Immovable( Rehued( new Cloak(), 0x0556 ) ) );
            guard.AddItem( Immovable( new OrderPlatemail() ) );
            guard.AddItem( Immovable( new PlateGloves() ) );
            guard.AddItem( Immovable( new PlateLegs() ) );
            guard.AddItem( Immovable( new PlateArms() ) );
            guard.AddItem( Immovable( new PlateGorget() ) );
            guard.AddItem( Immovable( new CloseHelm() ) );

            Halberd weapon = new Halberd();
            weapon.Movable = false;
            weapon.Crafter = guard;
            weapon.Quality = WeaponQuality.Exceptional;
            weapon.CustomQuality = Quality.Exceptional;
            guard.AddItem( weapon );

            if( guard.Backpack == null )
            {
                Container pack = new Backpack();
                pack.Movable = false;
                guard.AddItem( pack );
            }

            guard.PackItem( new Bandage( Utility.RandomMinMax( 30, 40 ) ) );
        }

        private string[] m_Arenas = new string[] { "Arena di Serpent's Hold" };

        public override string[] GetTownArenas()
        {
            return m_Arenas;
        }

        private static StuckMenuEntry[] m_StuckEntries = new StuckMenuEntry[]
		{
			// Serpent's Hold
			new StuckMenuEntry( 1011348, new Point3D[]
				{
					new Point3D( 3031, 3446, 15 ),
					new Point3D( 2958, 3447, 15 ),
					new Point3D( 2958, 3488, 15 ),
					new Point3D( 2916, 3509, 10 ),
					new Point3D( 2983, 3430, 15 )
				} )
        };

        public override StuckMenuEntry[] GetStuckEntries()
        {
            return m_StuckEntries;
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