/***************************************************************************
 *                               Vinyamar.cs
 *                            --------------------
 *   begin                : 22 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public sealed class Vinyamar : TownSystem
    {
        public Vinyamar()
        {
            m_Definition = new TownDefinition( "Vinyamar",
                                                MidgardTowns.Vinyamar,
                                                "Vinyamar",
                                                new CityInfo( "Vinyamar", "Center", 0, 0, 0, Map.Felucca ),
                                                TownBanFlag.Vinyamar,
                                                new Point3D( 3926, 402, 5 )
                                                );
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

            guard.Race = Races.Core.NorthernElf;
            guard.Title = "the Vinyamar guard";

            guard.SpeechHue = Utility.RandomDyedHue();

            if( Utility.RandomBool() )
            {
                guard.Female = true;
                guard.Name = NameList.RandomName( "female" );
            }
            else
            {
                guard.Female = false;
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

            guard.AddItem( Immovable( Rehued( new FurCape(), 0x0427 ) ) );
            guard.AddItem( Immovable( new ElvenDarkShirt() ) );
            guard.AddItem( Immovable( Rehued( new ElvenBoots(), 0x0427 ) ) );
            guard.AddItem( Immovable( Rehued( new ElvenPants(), 0x0427 ) ) );
            guard.AddItem( Immovable( new VultureHelm() ) );

            Scimitar weapon = new Scimitar();
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