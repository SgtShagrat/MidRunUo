/***************************************************************************
 *                               Vesper.cs
 *                            --------------------
 *   begin                : 22 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Midgard.Items;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public sealed class Vesper : TownSystem
    {
        public Vesper()
        {
            m_Definition = new TownDefinition( "Vesper",
                                                MidgardTowns.Vesper,
                                                "Vesper",
                                                new CityInfo( "Vesper", "Center", 0, 0, 0, Map.Felucca ),
                                                TownBanFlag.Vesper,
                                                new Point3D( 2897, 688, 5 )
                                                );

            m_VendorScalars.Add( typeof( Weaponsmith ), 0.85 );
            m_VendorScalars.Add( typeof( Armorer ), 0.85 );
            m_VendorScalars.Add( typeof( Tailor ), 0.80 );
            m_VendorScalars.Add( typeof( LeatherWorker ), 0.90 );
            m_VendorScalars.Add( typeof( Alchemist ), 1.15 );
            m_VendorScalars.Add( typeof( Mage ), 1.15 );
            m_VendorScalars.Add( typeof( Scribe ), 1.15 );
            m_VendorScalars.Add( typeof( Jeweler ), 1.15 );
            m_VendorScalars.Add( typeof( InnKeeper ), 1.20 );
        }

        private static Dictionary<Type, double> m_VendorScalars = new Dictionary<Type, double>();

        public override double GetDefaultScalar( Type vendorType )
        {
            double scalar = 1.0;
            m_VendorScalars.TryGetValue( vendorType, out scalar );

            return scalar;
        }

        public override int AccessCost
        {
            get { return 1000; }
        }

        public override bool AcceptCitizens
        {
            get { return true; }
        }

        public override void DressTownGuard( BaseCreature guard )
        {
            guard.InitStats( Utility.Dice( 1, 25, 200 ), Utility.Dice( 1, 25, 160 ), Utility.Dice( 1, 15, 180 ) );

            guard.Title = "the Vesper guard";
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

            /*
            guard.AddItem( Immovable( new TwoColouredPants() ) );
            guard.AddItem( Immovable( Rehued( new ReinforcedBoots(), 0x0878 ) ) );
            guard.AddItem( Immovable( Rehued( new LeatherArms(), 0x0427 ) ) );
            guard.AddItem( Immovable( Rehued( new FemaleStuddedChest(), 0x0878 ) ) );
            guard.AddItem( Immovable( Rehued( new Bonnet(), 0x0151 ) ) );
            guard.AddItem( Immovable( Rehued( new Cloak(), 0x0845 ) ) );
            */

            guard.AddItem( Immovable( new PlateChest() ) );
            guard.AddItem( Immovable( new PlateGloves() ) );
            guard.AddItem( Immovable( new PlateGorget() ) );
            guard.AddItem( Immovable( new NobleShirt() ) );
            guard.AddItem( Immovable( Rehued( new Cap(), 0x0020 ) ) );
            guard.AddItem( Immovable( Rehued( new TwoColouredPants(), 0x0021 ) ) );
            guard.AddItem( Immovable( Rehued( new Cloak(), 0x0021 ) ) );
            guard.AddItem( Immovable( new ReinforcedBoots() ) );

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
    }
}