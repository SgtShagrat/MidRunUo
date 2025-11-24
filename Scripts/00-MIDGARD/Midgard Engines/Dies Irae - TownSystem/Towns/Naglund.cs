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
    public sealed class Naglund : TownSystem
    {
        public Naglund()
        {
            m_Definition = new TownDefinition( "Naglund",
                                                MidgardTowns.Naglund,
                                                "Naglund",
                                                new CityInfo( "Naglund", "Center", 0, 0, 0, Map.Felucca ),
                                                TownBanFlag.Naglund,
                                                new Point3D( 6611, 1460, 5 )
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

            guard.Race = Races.Core.Naglor;
            guard.Title = "the Naglund guard";

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

            guard.AddItem( Immovable( Rehued( new Cloak(), 0x0541 ) ) );
            guard.AddItem( Immovable( Rehued( new SeaChainChest(), 0x0896 ) ) );
            guard.AddItem( Immovable( Rehued( new SeaChainLegs(), 0x0896 ) ) );
            guard.AddItem( Immovable( Rehued( new SeaChainCoif(), 0x0896 ) ) );
            guard.AddItem( Immovable( Rehued( new Sandals(), 0x0896 ) ) );

            Pitchfork weapon = new Pitchfork();
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