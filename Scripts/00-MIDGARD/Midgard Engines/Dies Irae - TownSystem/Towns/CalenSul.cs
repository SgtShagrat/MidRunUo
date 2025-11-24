/***************************************************************************
 *                               CalenSul.cs
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
    public sealed class CalenSul : TownSystem
    {
        public CalenSul()
        {
            m_Definition = new TownDefinition( "Calen Sul",
                                                MidgardTowns.CalenSul,
                                               "Calen Sul",
                                               new CityInfo( "Calen Sul", "Center", 0, 0, 0, Map.Felucca ),
                                               TownBanFlag.CalenSul,
                                               new Point3D( 771, 1979, 5 )
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
	/* Mod by Arlas: FIX TEMPORANEO BUG ARCHERS
        public override void DressTownGuard( BaseCreature guard )
        {
            guard.InitStats( Utility.Dice( 1, 25, 200 ), Utility.Dice( 1, 25, 160 ), Utility.Dice( 1, 15, 180 ) );

            guard.Race = Races.Core.HighElf;
            guard.Title = "the Calen Sul guard";

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

            guard.AddItem( Immovable( Rehued( new HoodedCloak(), 0x056F ) ) );
            guard.AddItem( Immovable( Rehued( new Hood(), 0x056F ) ) );
            guard.AddItem( Immovable( new LeafChest() ) );
            guard.AddItem( Immovable( new ElvenBoots() ) );
            guard.AddItem( Immovable( new LeafLegs() ) );
            guard.AddItem( Immovable( new LeafArms() ) );
            guard.AddItem( Immovable( new LeafGloves() ) );
            guard.AddItem( Immovable( new Quiver() ) );
            guard.AddItem( Immovable( new Belt() ) );
            guard.AddItem( Immovable( Rehued( new Muzzle(), 0x0873 ) ) );

            ElvenCompositeLongbow weapon = new ElvenCompositeLongbow();
            weapon.Movable = false;
            weapon.Crafter = guard;
            weapon.Quality = WeaponQuality.Exceptional;
            weapon.MinDamage = 40;
            weapon.MaxDamage = 60;
            weapon.CustomQuality = Quality.Exceptional;
            guard.AddItem( weapon );

            guard.AddItem( Immovable( new Backpack() ) );

            guard.PackItem( new Bandage( Utility.RandomMinMax( 30, 40 ) ) );
        }
	*/
        public override void DressTownGuard( BaseCreature guard )
        {
            guard.InitStats( Utility.Dice( 1, 25, 200 ), Utility.Dice( 1, 25, 160 ), Utility.Dice( 1, 15, 180 ) );

            guard.Race = Races.Core.HighElf;
            guard.Title = "the Calen Sul guard";

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

            guard.AddItem( Immovable( Rehued( new HoodedCloak(), 0x056F ) ) );
            guard.AddItem( Immovable( Rehued( new Hood(), 0x056F ) ) );
            guard.AddItem( Immovable( new LeafChest() ) );
            guard.AddItem( Immovable( new ElvenBoots() ) );
            guard.AddItem( Immovable( new LeafLegs() ) );
            guard.AddItem( Immovable( new LeafArms() ) );
            guard.AddItem( Immovable( new LeafGloves() ) );
            //guard.AddItem( Immovable( new Quiver() ) );
            guard.AddItem( Immovable( new Belt() ) );
            guard.AddItem( Immovable( Rehued( new Muzzle(), 0x0873 ) ) );

            Bardiche weapon = new Bardiche();
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