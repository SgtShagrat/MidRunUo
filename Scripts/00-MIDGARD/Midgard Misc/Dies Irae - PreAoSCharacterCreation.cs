using System;
using System.Collections.Generic;

using Midgard.Items;
using Server;
using Server.Items;

namespace Midgard.Commands
{
    public class PreAoSCharacterCreation
    {
        public static void Dress( Mobile m )
        {
            AddBackpack( m );

            AddShirt( m, Utility.RandomDyedHue() );
            AddPants( m, Utility.RandomDyedHue() );
            AddShoes( m );

            List<Skill> skills = GetTopSkills( m );

            for( int i = 0; i < 3; ++i )
            {
                if( skills[ i ].Value > 0 )
                    AddSkillItems( skills[ i ].SkillName, m );
            }
        }

        private static void AddBackpack( Mobile m )
        {
            Container pack = m.Backpack;
            if( pack == null )
            {
                pack = new Backpack();
                pack.Movable = false;

                m.AddItem( pack );
            }

            // Regardless of skills chosen, all characters begin with a Book (blank), Candle, Dagger, Pants/Skirt, Shirt, Shoes and 1000 Gold.
            PackItem( new RedBook( "a book", m.Name, 20, true ), pack );
            PackItem( new Candle(), pack );
            PackItem( new PracticeWoodenDagger(), pack );
            PackItem( new Gold( 1000 ), pack );
        }

        private static void AddShirt( Mobile m, int shirtHue )
        {
            switch( Utility.Random( 3 ) )
            {
                case 0: EquipItem( m, new Shirt( shirtHue ), true ); break;
                case 1: EquipItem( m, new FancyShirt( shirtHue ), true ); break;
                case 2: EquipItem( m, new Doublet( shirtHue ), true ); break;
            }
        }

        private static void AddPants( Mobile m, int pantsHue )
        {
            if( m.Female )
            {
                switch( Utility.Random( 2 ) )
                {
                    case 0: EquipItem( m, new Skirt( pantsHue ), true ); break;
                    case 1: EquipItem( m, new Kilt( pantsHue ), true ); break;
                }
            }
            else
            {
                switch( Utility.Random( 2 ) )
                {
                    case 0: EquipItem( m, new LongPants( pantsHue ), true ); break;
                    case 1: EquipItem( m, new ShortPants( pantsHue ), true ); break;
                }
            }
        }

        private static void AddShoes( Mobile m )
        {
            EquipItem( m, new Shoes( Utility.RandomYellowHue() ), true );
        }

        private static void EquipItem( Mobile m, Item item )
        {
            EquipItem( m, item, true );
        }

        private static void EquipItem( Mobile m, Item item, bool mustEquip )
        {
            item.LootType = LootType.Newbied;

            if( m != null && m.EquipItem( item ) )
                return;
            else if( m == null )
                return;

            Container pack = m.Backpack;

            if( !mustEquip && pack != null )
                pack.DropItem( item );
            else
                item.Delete();
        }

        private static void PackItem( Item item, Container pack )
        {
            item.LootType = LootType.Newbied;

            if( pack != null )
                pack.DropItem( item );
            else
            {
                Console.WriteLine( "Warning: null backpack during player import." );
                item.Delete();
            }
        }

        private static void PackInstrument( Container pack )
        {
            switch( Utility.Random( 6 ) )
            {
                case 0: PackItem( new Drums(), pack ); break;
                case 1: PackItem( new Harp(), pack ); break;
                case 2: PackItem( new LapHarp(), pack ); break;
                case 3: PackItem( new Lute(), pack ); break;
                case 4: PackItem( new Tambourine(), pack ); break;
                case 5: PackItem( new TambourineTassel(), pack ); break;
            }
        }

        private static void PackScroll( int circle, Container pack )
        {
            switch( Utility.Random( 8 ) * ( circle * 8 ) )
            {
                case 0: PackItem( new ClumsyScroll(), pack ); break;
                case 1: PackItem( new CreateFoodScroll(), pack ); break;
                case 2: PackItem( new FeeblemindScroll(), pack ); break;
                case 3: PackItem( new HealScroll(), pack ); break;
                case 4: PackItem( new MagicArrowScroll(), pack ); break;
                case 5: PackItem( new NightSightScroll(), pack ); break;
                case 6: PackItem( new ReactiveArmorScroll(), pack ); break;
                case 7: PackItem( new WeakenScroll(), pack ); break;
                case 8: PackItem( new AgilityScroll(), pack ); break;
                case 9: PackItem( new CunningScroll(), pack ); break;
                case 10: PackItem( new CureScroll(), pack ); break;
                case 11: PackItem( new HarmScroll(), pack ); break;
                case 12: PackItem( new MagicTrapScroll(), pack ); break;
                case 13: PackItem( new MagicUnTrapScroll(), pack ); break;
                case 14: PackItem( new ProtectionScroll(), pack ); break;
                case 15: PackItem( new StrengthScroll(), pack ); break;
                case 16: PackItem( new BlessScroll(), pack ); break;
                case 17: PackItem( new FireballScroll(), pack ); break;
                case 18: PackItem( new MagicLockScroll(), pack ); break;
                case 19: PackItem( new PoisonScroll(), pack ); break;
                case 20: PackItem( new TelekinisisScroll(), pack ); break;
                case 21: PackItem( new TeleportScroll(), pack ); break;
                case 22: PackItem( new UnlockScroll(), pack ); break;
                case 23: PackItem( new WallOfStoneScroll(), pack ); break;
            }
        }

        public static List<Skill> GetTopSkills( Mobile m )
        {
            List<Skill> skills = new List<Skill>();
            for( int i = 0; i < m.Skills.Length; i++ )
                skills.Add( m.Skills[ i ] );

            skills.Sort();
            skills.Reverse();
            return skills;
        }

        public static void AddSkillItems( SkillName skill, Mobile m )
        {
            Container bp = m.Backpack;
            if( bp == null )
                return;

            switch( skill )
            {
                case SkillName.Alchemy:
                    {
                        // 3 each of 4 random Reagents, 5 Bottles, Mortar & Pestle, Robe (red)
                        PackItem( new Bottle( 4 ), bp );
                        PackItem( new MortarPestle(), bp );
                        EquipItem( m, new Robe( Utility.RandomRedHue() ) );
                        break;
                    }
                case SkillName.Anatomy:
                    {
                        // 3 Bandages, Robe (grey)
                        PackItem( new Bandage( 3 ), bp );
                        EquipItem( m, new Robe( Utility.RandomNeutralHue() ) );
                        break;
                    }
                case SkillName.AnimalLore:
                    {
                        // Robe (green), Shepherd's Crook [practice weapon]
                        EquipItem( m, new PracticeShepherdsCrook(), true );
                        EquipItem( m, new Robe( Utility.RandomGreenHue() ) );
                        break;
                    }
                case SkillName.AnimalTaming:
                    {
                        // Shepherd's Crook [practice weapon]
                        EquipItem( m, new PracticeShepherdsCrook() );
                        break;
                    }
                case SkillName.Archery:
                    {
                        // 25 Arrows, Bow [practice weapon]
                        PackItem( new Arrow( 25 ), bp );
                        EquipItem( m, new PracticeBow() );
                        break;
                    }
                case SkillName.ArmsLore:
                    {
                        // random practice weapon
                        break;
                    }
                case SkillName.Begging:
                    {
                        // Gnarled Staff [practice weapon]
                        EquipItem( m, new PracticeGnarledStaff() );
                        break;
                    }
                case SkillName.Blacksmith:
                    {
                        // Half Apron (brown), random blacksmithing tool
                        PackItem( new Tongs(), bp );
                        PackItem( new Pickaxe(), bp );
                        PackItem( new Pickaxe(), bp );
                        PackItem( new IronIngot( 50 ), bp );
                        EquipItem( m, new HalfApron( Utility.RandomYellowHue() ) );
                        break;
                    }
                case SkillName.Fletching:
                    {
                        // 14 Boards, 5 Feathers, 5 Shafts
                        PackItem( new Board( 14 ), bp );
                        PackItem( new Feather( 5 ), bp );
                        PackItem( new Shaft( 5 ), bp );
                        break;
                    }
                case SkillName.Camping:
                    {
                        // Bedroll, 5 Kindling
                        PackItem( new Bedroll(), bp );
                        PackItem( new Kindling( 5 ), bp );
                        break;
                    }
                case SkillName.Carpentry:
                    {
                        // 10 Boards, Half Apron (brown), random carpentry tool
                        PackItem( new Board( 10 ), bp );
                        PackItem( new Saw(), bp );
                        EquipItem( m, new HalfApron( Utility.RandomYellowHue() ) );
                        break;
                    }
                case SkillName.Cartography:
                    {
                        // 4 Blank Maps, Sextant
                        for( int i = 0; i < 4; i++ )
                            PackItem( new BlankMap(), bp );
                        PackItem( new Sextant(), bp );
                        break;
                    }
                case SkillName.Cooking:
                    {
                        // 2 Kindling, 3 Raw Birds, Flour Sack (full), Water Pitcher (full)
                        PackItem( new Kindling( 2 ), bp );
                        PackItem( new RawBird( 3 ), bp );
                        PackItem( new SackFlour(), bp );
                        PackItem( new Pitcher( BeverageType.Water ), bp );
                        break;
                    }
                case SkillName.DetectHidden:
                    {
                        // Cloak (dark grey)
                        EquipItem( m, new Cloak( 0x455 ) );
                        break;
                    }
                case SkillName.Discordance: // Enticement
                    {
                        // random musical instrument
                        PackInstrument( bp );
                        break;
                    }
                case SkillName.EvalInt:
                    {
                        break;
                    }
                case SkillName.Fencing:
                    {
                        // Spear [practice weapon]
                        EquipItem( m, new PracticeWoodenSpear() );
                        break;
                    }
                case SkillName.Fishing:
                    {
                        // Fishing Pole, Floppy Hat (brown)
                        EquipItem( m, new FishingPole() );
                        EquipItem( m, new FloppyHat( Utility.RandomYellowHue() ) );
                        break;
                    }
                case SkillName.Forensics:
                    {
                        break;
                    }
                case SkillName.Healing:
                    {
                        // 5 Bandages, Scissors
                        PackItem( new Bandage( 5 ), bp );
                        PackItem( new Scissors(), bp );
                        break;
                    }
                case SkillName.Herding:
                    {
                        // Shepherd's Crook [practice weapon]
                        EquipItem( m, new PracticeShepherdsCrook() );
                        break;
                    }
                case SkillName.Hiding:
                    {
                        // Cloak (dark grey)
                        EquipItem( m, new Cloak( 0x455 ) );
                        break;
                    }
                case SkillName.Inscribe:
                    {
                        // 2 Blank Scrolls
                        PackItem( new BlankScroll( 2 ), bp );
                        break;
                    }
                case SkillName.ItemID:
                    {
                        // Gnarled Staff [practice weapon]
                        EquipItem( m, new PracticeGnarledStaff() );
                        break;
                    }
                case SkillName.Lockpicking:
                    {
                        // 5 Lockpicks
                        PackItem( new Lockpick( 5 ), bp );
                        break;
                    }
                case SkillName.Lumberjacking:
                    {
                        // Hatchet [practice weapon]
                        EquipItem( m, new PracticeWoodenHatchet() );
                        break;
                    }
                case SkillName.Macing:
                    {
                        // Mace [practice weapon]
                        EquipItem( m, new PracticeWoodenMace() );
                        break;
                    }
                case SkillName.Magery:
                    {
                        // 3 random First Circle Scrolls, 3 of each Reagent, Spellbook
                        BagOfReagents regs = new BagOfReagents( 3 );
                        foreach( Item item in regs.Items )
                            item.LootType = LootType.Newbied;
                        PackItem( regs, bp );
                        regs.LootType = LootType.Regular;

                        for( int i = 0; i < 3; i++ )
                            PackScroll( 0, bp );

                        Spellbook book = new Spellbook( (ulong)0x382A8C38 );
                        EquipItem( m, book );
                        book.LootType = LootType.Blessed;
                        break;
                    }
                case SkillName.Meditation:
                    {
                        // Robe
                        EquipItem( m, new Robe( Utility.RandomNeutralHue() ) );
                        break;
                    }
                case SkillName.Mining:
                    {
                        // Pickaxe or Shovel
                        switch( Utility.Random( 2 ) )
                        {
                            case 0: PackItem( new Pickaxe(), bp ); break;
                            case 1: PackItem( new Shovel(), bp ); break;
                        }

                        break;
                    }
                case SkillName.Musicianship:
                    {
                        // random musical instrument
                        PackInstrument( bp );
                        break;
                    }
                case SkillName.Parry:
                    {
                        // Wooden Shield
                        EquipItem( m, new WoodenShield() );
                        break;
                    }
                case SkillName.Peacemaking:
                    {
                        // random musical instrument
                        PackInstrument( bp );
                        break;
                    }
                case SkillName.Poisoning:
                    {
                        // 2 Poison (Green) Potions
                        PackItem( new LesserPoisonPotion( 2 ), bp );
                        break;
                    }
                case SkillName.Provocation:
                    {
                        // random musical instrument
                        PackInstrument( bp );
                        break;
                    }
                case SkillName.RemoveTrap:
                    {
                        // you cannot select this as a starting skill
                        break;
                    }
                case SkillName.MagicResist:
                    {
                        // Spellbook
                        Spellbook book = new Spellbook( (ulong)0x382A8C38 );
                        EquipItem( m, book, true );
                        book.LootType = LootType.Blessed;
                        break;
                    }
                case SkillName.Snooping:
                    {
                        // 4 Lockpicks
                        PackItem( new Lockpick( 4 ), bp );
                        break;
                    }
                case SkillName.SpiritSpeak:
                    {
                        // Cloak (dark grey)
                        EquipItem( m, new Cloak( 0x455 ) );
                        break;
                    }
                case SkillName.Stealing:
                    {
                        // 4 Lockpicks
                        PackItem( new Lockpick( 4 ), bp );
                        break;
                    }
                case SkillName.Stealth:
                    {
                        // you cannot select this as a starting skill
                        break;
                    }
                case SkillName.Swords:
                    {
                        // Longsword [practice weapon]
                        EquipItem( m, new PracticeWoodenLongSword() );
                        break;
                    }
                case SkillName.Tactics:
                    {
                        break;
                    }
                case SkillName.Tailoring:
                    {
                        // 4 Folded Cloth, Sewing Kit
                        PackItem( new Cloth( 4 ), bp );
                        PackItem( new SewingKit(), bp );
                        break;
                    }
                case SkillName.TasteID:
                    {
                        // 3 random Potions
                        for( int i = 0; i < 3; i++ )
                            PackItem( Loot.RandomPotion(), bp );
                        break;
                    }
                case SkillName.Tinkering:
                    {
                        // Half Apron (brown), random component, Tinker Tools
                        EquipItem( m, new HalfApron( Utility.RandomYellowHue() ) );
                        PackItem( new TinkerTools(), bp );
                        break;
                    }
                case SkillName.Tracking:
                    {
                        // Boots (brown), Skinning Knife [practice weapon]
                        Item shoes = m.FindItemOnLayer( Layer.Shoes );
                        if( shoes != null )
                            shoes.Delete();

                        EquipItem( m, new Boots( Utility.RandomYellowHue() ) );
                        EquipItem( m, new PracticeSkinningKnife() );
                        break;
                    }
                case SkillName.Veterinary:
                    {
                        // 5 Bandages, Scissors
                        PackItem( new Bandage( 5 ), bp );
                        PackItem( new Scissors(), bp );
                        break;
                    }
                case SkillName.Wrestling:
                    {
                        // Leather Gloves
                        EquipItem( m, new LeatherGloves() );
                        break;
                    }
            }
        }
    }
}