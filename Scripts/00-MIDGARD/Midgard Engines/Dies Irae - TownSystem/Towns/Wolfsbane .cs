/***************************************************************************
 *                               Ahnor.cs
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
    public sealed class Wolfsbane : TownSystem
    {
        public Wolfsbane()
        {
            m_Definition = new TownDefinition(  "Wolfsbane", 
                                                MidgardTowns.Wolfsbane,
                                                "Wolfsbane",
                                                new CityInfo( "Wolfsbane", "Center", 0, 0, 0, Map.Felucca ),
                                                TownBanFlag.Wolfsbane,
                                                new Point3D( 1045, 1413, 6 )
                                                );
        }

        public override bool AcceptCitizens
        {
            get { return true; }
        }

        public override void DressTownGuard( BaseCreature guard )
        {
            guard.InitStats( Utility.Dice( 1, 25, 200 ), Utility.Dice( 1, 25, 160 ), Utility.Dice( 1, 15, 180 ) );

            guard.Title = "the Wolfsbane guard";
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

            guard.AddItem( Immovable( Rehued( new FurCape(), 0x0427 ) ) );
            guard.AddItem( Immovable( new PlateChest() ) );
            guard.AddItem( Immovable( new PlateLegs() ) );
            guard.AddItem( Immovable( new PlateArms() ) );
            guard.AddItem( Immovable( new PlateGorget() ) );
            guard.AddItem( Immovable( new PlateGloves() ) );
            guard.AddItem( Immovable( new BarbarianPlateHelm() ) );

            Bardiche weapon = new Bardiche();
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