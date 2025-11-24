/***************************************************************************
 *                               Sshamath.cs
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
    public sealed class Sshamath : TownSystem
    {
        public Sshamath()
        {
            m_Definition = new TownDefinition( "Sshamath",
                                                MidgardTowns.Sshamath,
                                                "Sshamath",
                                                new CityInfo( "Sshamath", "Center", 0, 0, 0, Map.Felucca ),
                                                TownBanFlag.Sshamath,
                                                new Point3D( 6401, 1132, 5 )
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

            guard.Race = Races.Core.Drow;
            guard.Title = "the Sshamath guard";

            guard.SpeechHue = Utility.RandomBlueHue();

            guard.Female = false;
            guard.Name = NameList.RandomName( "male" );

            guard.VirtualArmor = 32;

            guard.SetSkill( SkillName.Swords, 110.0, 120.0 );
            guard.SetSkill( SkillName.Macing, 110.0, 120.0 );
            guard.SetSkill( SkillName.Wrestling, 110.0, 120.0 );
            guard.SetSkill( SkillName.Tactics, 110.0, 120.0 );
            guard.SetSkill( SkillName.MagicResist, 110.0, 120.0 );
            guard.SetSkill( SkillName.Healing, 110.0, 120.0 );
            guard.SetSkill( SkillName.Anatomy, 110.0, 120.0 );
            guard.SetSkill( SkillName.DetectHidden, 110.0, 120.0 );

            guard.AddItem( Immovable( Rehued( new HoodedCloak(), 0x09CE ) ) );
            guard.AddItem( Immovable( Rehued( new Hood(), 0x09CE ) ) );
            guard.AddItem( Immovable( Rehued( new Shirt(), 0x0497 ) ) );
            guard.AddItem( Immovable( Rehued( new LongPants(), 0x0497 ) ) );
            guard.AddItem( Immovable( Rehued( new StuddedArms(), 0x04DF ) ) );
            guard.AddItem( Immovable( Rehued( new Belt(), 0x09CE ) ) );
            guard.AddItem( Immovable( Rehued( new StuddedGloves(), 0x04DF ) ) );
            guard.AddItem( Immovable( Rehued( new Muzzle(), 0x09CE ) ) );
            guard.AddItem( Immovable( new ThighBoots() ) );

            DrowPike weapon = new DrowPike();
            weapon.Movable = false;
            weapon.Crafter = guard;
            weapon.Quality = WeaponQuality.Exceptional;
            weapon.CustomQuality = Quality.Exceptional;
            guard.AddItem( weapon );

            guard.AddItem( Immovable( new Backpack() ) );

            guard.PackItem( new Bandage( Utility.RandomMinMax( 30, 40 ) ) );
        }

        public override bool AllowMurderResurrection
        {
            get { return true; }
        }
    }
}