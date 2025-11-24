/***************************************************************************
 *                                  Trinsic.cs
 *                            		----------
 *  begin                	: Marzo, 2008
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
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public sealed class Trinsic : TownSystem
    {
        public Trinsic()
        {
            m_Definition = new TownDefinition( "Trinsic",
                                                MidgardTowns.Trinsic,
                                                "Trinsic",
                                                new CityInfo( "Trinsic", "Center", 0, 0, 0, Map.Felucca ),
                                                TownBanFlag.Trinsic,
                                                new Point3D( 1949, 2786, 7 )
                                                );

            FieldDefinitions = null;

            m_VendorScalars.Add( typeof( AnimalTrainer ), 0.90 );
            m_VendorScalars.Add( typeof( LeatherWorker ), 0.95 );
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

        public override bool IsUnderNoHousingCriteria( Point3D p )
        {
            return false;
        }

        public override void DressTownGuard( BaseCreature guard )
        {
            guard.InitStats( Utility.Dice( 1, 25, 200 ), Utility.Dice( 1, 25, 160 ), Utility.Dice( 1, 15, 180 ) );

            guard.Title = "the Trinsic guard";

            guard.SpeechHue = Utility.RandomDyedHue();

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
            guard.AddItem( Immovable( new PlumedHelm() ) );

            Halberd weapon = new Halberd();
            weapon.Movable = false;
            weapon.Crafter = guard;
            weapon.Quality = WeaponQuality.Exceptional;
            weapon.CustomQuality = Quality.Exceptional;
            guard.AddItem( weapon );

            guard.AddItem( Immovable( new Backpack() ) );

            guard.PackItem( new Bandage( Utility.RandomMinMax( 30, 40 ) ) );
        }
    }
}