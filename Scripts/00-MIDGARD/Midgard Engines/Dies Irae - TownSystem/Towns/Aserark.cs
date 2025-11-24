/***************************************************************************
 *                               Aserark.cs
 *                            --------------------
 *   begin                : 22 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Midgard.Items;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public sealed class Aserark : TownSystem
    {
        public Aserark()
        {
            m_Definition = new TownDefinition( "Aserark",
                                                MidgardTowns.Aserark,
                                                "Aserark",
                                                new CityInfo( "Aserark", "Center", 0, 0, 0, Map.Felucca ),
                                                TownBanFlag.Aserark,
                                                new Point3D( 2104, 2062, 5 )
                                                );
        }

        public override void DressTownGuard( BaseCreature guard )
        {
            guard.InitStats( Utility.Dice( 1, 25, 200 ), Utility.Dice( 1, 25, 160 ), Utility.Dice( 1, 15, 180 ) );

            guard.Name = NameList.RandomName( "male" );
            guard.Title = "the Aserark guard";
            guard.SpeechHue = Utility.RandomDyedHue();
            guard.Hue = Utility.RandomSkinHue();

            guard.SetSkill( SkillName.Swords, 110.0, 120.0 );
            guard.SetSkill( SkillName.Macing, 110.0, 120.0 );
            guard.SetSkill( SkillName.Wrestling, 110.0, 120.0 );
            guard.SetSkill( SkillName.Tactics, 110.0, 120.0 );
            guard.SetSkill( SkillName.MagicResist, 110.0, 120.0 );
            guard.SetSkill( SkillName.Healing, 110.0, 120.0 );
            guard.SetSkill( SkillName.Anatomy, 110.0, 120.0 );
            guard.SetSkill( SkillName.DetectHidden, 110.0, 120.0 );

            guard.AddItem( Immovable( Rehued( new Cloak(), 0x078D ) ) );
            guard.AddItem( Immovable( new ChaosShield() ) );
            guard.AddItem( Immovable( new ChaosPlatemail() ) );
            guard.AddItem( Immovable( new PlateGloves() ) );
            guard.AddItem( Immovable( new PlateLegs() ) );
            guard.AddItem( Immovable( new PlateArms() ) );
            guard.AddItem( Immovable( new PlateGorget() ) );
            guard.AddItem( Immovable( new CloseHelm() ) );

            Utility.AssignRandomHair( guard );
            if( Utility.RandomBool() )
                Utility.AssignRandomFacialHair( guard, guard.HairHue );

            Longsword weapon = new Longsword();
            weapon.Movable = false;
            weapon.Crafter = guard;
            weapon.Quality = WeaponQuality.Exceptional;
            weapon.CustomQuality = Quality.Exceptional;
            guard.AddItem( weapon );

            Container pack = new Backpack();
            pack.Movable = false;
            guard.AddItem( pack );

            guard.PackItem( new Bandage( Utility.RandomMinMax( 30, 40 ) ) );
        }
    }
}