/***************************************************************************
 *                                  Britain.cs
 *                            		----------
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
    public sealed class Britain : TownSystem
    {
        public Britain()
        {
            m_Definition = new TownDefinition( "Britain",
                                                MidgardTowns.Britain,
                                                "Britain",
                                                new CityInfo( "Britain", "Center", 0, 0, 0, Map.Felucca ),
                                                TownBanFlag.Britain,
                                                new Point3D( 1476, 1655, 15 )
                                                );

            FieldDefinitions = null;

            // Essendo La Capitale Ha I Prezzi Generalmente + Bassi Tranne 
            // Tailor Per Sfregio :)
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
            m_VendorScalars.Add( typeof( Scribe ), 1.18 );
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
            if( p.X >= 1100 && p.Y >= 1235 && p.X <= 1100 + 1010 && p.Y <= 1235 + 350 )
                return true;
            else if( p.X >= 1085 && p.Y >= 1585 && p.X <= 1085 + 695 && p.Y <= 1585 + 300 )
                return true;
            else if( p.X >= 1165 && p.Y >= 1885 && p.X <= 1165 + 445 && p.Y <= 1885 + 100 )
                return true;
            else if( p.X >= 1570 && p.Y >= 1160 && p.X <= 1570 + 550 && p.Y <= 1160 + 75 )
                return true;
            else
                return false;
        }

        private string[] m_Arenas = new string[] { "Arena di Britain" };

        public override string[] GetTownArenas()
        {
            return m_Arenas;
        }

        public override void DressTownGuard( BaseCreature guard )
        {
            guard.InitStats( Utility.Dice( 1, 25, 200 ), Utility.Dice( 1, 25, 160 ), Utility.Dice( 1, 15, 180 ) );

            guard.Title = "the Britain guard";

            guard.SpeechHue = Utility.RandomDyedHue();

            guard.Hue = Utility.RandomSkinHue();

            if( Utility.RandomBool() )
            {
                guard.Female = true;
                guard.Body = 401;
                guard.Name = NameList.RandomName( "female" );
            }
            else
            {
                guard.Female = false;
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

            guard.AddItem( Immovable( Rehued( new Cloak(), 0x078D ) ) );
            guard.AddItem( Immovable( new ChaosPlatemail() ) );
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

            guard.AddItem( Immovable( new Backpack() ) );

            guard.PackItem( new Bandage( Utility.RandomMinMax( 30, 40 ) ) );
        }

        private static StuckMenuEntry[] m_StuckEntries = new StuckMenuEntry[]
		{
			// Britain
			new StuckMenuEntry( 1011028, new Point3D[]
				{
					new Point3D( 1522, 1757, 28 ),
					new Point3D( 1519, 1619, 10 ),
					new Point3D( 1457, 1538, 30 ),
					new Point3D( 1607, 1568, 20 ),
					new Point3D( 1643, 1680, 18 )
				} ),
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