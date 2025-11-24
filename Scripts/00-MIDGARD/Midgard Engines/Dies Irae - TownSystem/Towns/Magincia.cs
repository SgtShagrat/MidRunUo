/***************************************************************************
 *                               Magincia.cs
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
using Server.Items;

namespace Midgard.Engines.MidgardTownSystem
{
    public sealed class Magincia : TownSystem
    {
        public Magincia()
        {
            m_Definition = new TownDefinition( "Magincia",
                                                MidgardTowns.Magincia,
                                                "Magincia",
                                                new CityInfo( "Magincia", "Center", 0, 0, 0, Map.Felucca ),
                                                TownBanFlag.Magincia,
                                                new Point3D( 3707, 2115, 25 )
                                               );

            FieldDefinitions = null;

            m_VendorScalars.Add( typeof( Shipwright ), 0.85 );
            m_VendorScalars.Add( typeof( Alchemist ), 0.85 );
            m_VendorScalars.Add( typeof( Mage ), 0.85 );
            m_VendorScalars.Add( typeof( Scribe ), 0.90 );
            m_VendorScalars.Add( typeof( Jeweler ), 0.90 );
            m_VendorScalars.Add( typeof( Tailor ), 1.30 );
            m_VendorScalars.Add( typeof( Armorer ), 1.30 );
            m_VendorScalars.Add( typeof( AnimalTrainer ), 1.10 );
        }

        private static Dictionary<Type, double> m_VendorScalars = new Dictionary<Type, double>();

        public override double GetDefaultScalar( Type vendorType )
        {
            double scalar = 1.0;
            m_VendorScalars.TryGetValue( vendorType, out scalar );

            return scalar;
        }

        public override void DressTownGuard( BaseCreature guard )
        {
            guard.InitStats( Utility.Dice( 1, 25, 200 ), Utility.Dice( 1, 25, 160 ), Utility.Dice( 1, 15, 180 ) );

            //guard.Race = Races.Core.Naglor;
            guard.Title = ", Guardia Draconica di Magincia";

            guard.SpeechHue = Utility.RandomYellowHue();
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

            guard.AddItem( Immovable( Rehued( new DragonHelm(), 2216 ) ) );
            guard.AddItem( Immovable( Rehued( new DragonChest(), 2216 ) ) );
            guard.AddItem( Immovable( Rehued( new DragonArms(), 2216 ) ) );
            guard.AddItem( Immovable( Rehued( new DragonGloves(), 2216 ) ) );
            guard.AddItem( Immovable( Rehued( new DragonLegs(), 2216 ) ) );

            Bardiche weapon = new Bardiche();
            weapon.Movable = false;
            weapon.Crafter = guard;
            weapon.Hue = 2216;
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