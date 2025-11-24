/***************************************************************************
 *                               DalBaraz.cs
 *                            --------------------
 *   begin                : 22 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Midgard.Items;
using Server.Items;

namespace Midgard.Engines.MidgardTownSystem
{
    public sealed class DalBaraz : TownSystem
    {
        public DalBaraz()
        {
            m_Definition = new TownDefinition( "Dal-Baraz",
                                                MidgardTowns.DalBaraz,
                                                "Dal-Baraz",
                                                new string[] { "Dorog Dum" },
                                                new CityInfo( "Dal-Baraz", "Center", 0, 0, 0, Map.Felucca ),
                                                TownBanFlag.DalBaraz,
                                                new Point3D( 1918, 244, 19 )
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

            guard.Race = Races.Core.MountainDwarf;
            guard.Title = "the Dal-Baraz guard";

            guard.SpeechHue = Utility.RandomDyedHue();
            guard.Female = false;
            guard.Name = NameList.RandomName( "male" );

            guard.SetSkill( SkillName.Swords, 110.0, 120.0 );
            guard.SetSkill( SkillName.Macing, 110.0, 120.0 );
            guard.SetSkill( SkillName.Wrestling, 110.0, 120.0 );
            guard.SetSkill( SkillName.Tactics, 110.0, 120.0 );
            guard.SetSkill( SkillName.MagicResist, 110.0, 120.0 );
            guard.SetSkill( SkillName.Healing, 110.0, 120.0 );
            guard.SetSkill( SkillName.Anatomy, 110.0, 120.0 );
            guard.SetSkill( SkillName.DetectHidden, 110.0, 120.0 );

            guard.AddItem( Immovable( new PlateChest() ) );
            guard.AddItem( Immovable( new PlateLegs() ) );
            guard.AddItem( Immovable( new PlateArms() ) );
            guard.AddItem( Immovable( new PlateGloves() ) );
            guard.AddItem( Immovable( new PlateHelm() ) );

            DwarvenAxe weapon = new DwarvenAxe();
            weapon.Movable = false;
            weapon.Crafter = guard;
            weapon.Quality = WeaponQuality.Exceptional;
            weapon.CustomQuality = Quality.Exceptional;
            guard.AddItem( weapon );

            if( guard.Backpack == null )
                guard.AddItem( Immovable( new Backpack() ) );

            guard.PackItem( new Bandage( Utility.RandomMinMax( 30, 40 ) ) );
        }

        public override void DressTownVendor( Mobile mobile )
        {
            BaseVendor vendor = mobile as BaseVendor;
            if( vendor == null )
                return;

            vendor.Female = false;

            vendor.Race = Races.Core.MountainDwarf;

            vendor.SpeechHue = Utility.RandomDyedHue();

            vendor.Name = NameList.RandomName( "male" );

            for( int i = 0; i < mobile.Items.Count; ++i )
            {
                Item item = mobile.Items[ i ];

                if( item is BaseClothing || item is Hair || item is Beard )
                    item.Delete();
            }

            switch( Utility.Random( 3 ) )
            {
                case 0: vendor.AddItem( new FancyShirt( vendor.GetRandomHue() ) ); break;
                case 1: vendor.AddItem( new Doublet( vendor.GetRandomHue() ) ); break;
                case 2: vendor.AddItem( new Shirt( vendor.GetRandomHue() ) ); break;
            }

            switch( vendor.ShoeType )
            {
                case VendorShoeType.Shoes: vendor.AddItem( new Shoes( vendor.GetShoeHue() ) ); break;
                case VendorShoeType.Boots: vendor.AddItem( new Boots( vendor.GetShoeHue() ) ); break;
            }

            int hairHue = vendor.GetHairHue();

            Utility.AssignRandomHair( vendor, hairHue );
            Utility.AssignRandomFacialHair( vendor, hairHue );

            if( vendor.Female )
            {
                switch( Utility.Random( 6 ) )
                {
                    case 0: vendor.AddItem( new ShortPants( vendor.GetRandomHue() ) ); break;
                    case 1:
                    case 2: vendor.AddItem( new Kilt( vendor.GetRandomHue() ) ); break;
                    case 3:
                    case 4:
                    case 5: vendor.AddItem( new LongPants( vendor.GetRandomHue() ) ); break;
                }
            }
            else
            {
                switch( Utility.Random( 2 ) )
                {
                    case 0: vendor.AddItem( new LongPants( vendor.GetRandomHue() ) ); break;
                    case 1: vendor.AddItem( new ShortPants( vendor.GetRandomHue() ) ); break;
                }
            }
        }
    }
}