using System;
using Midgard.Commands;
using Midgard.Engines.AdvancedSmelting;
using Midgard.Engines.PlantSystem;
using Midgard.Engines.StoneEnchantSystem;
using Midgard.Items;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public sealed class BagOfTestitems : Bag
    {
        public enum BagType
        {
            Kegs,
            Tools,
            Amno,
            Maps,
            Materials,
            SpellStuff,
            Alchemical,
        }

        [Constructable]
        public BagOfTestitems( BagType type )
            : this( type, 100 )
        {
        }

        [Constructable]
        public BagOfTestitems( BagType type, int amount )
        {
            if( type == BagType.Kegs )
            {
                Name = "Various Potion Kegs";
                Hue = Utility.RandomMetalHue();

                PlaceItemIn( this, 45, 149, MakePotionKeg( PotionEffect.CureGreater, 0x2D ) );
                PlaceItemIn( this, 69, 149, MakePotionKeg( PotionEffect.HealGreater, 0x499 ) );
                PlaceItemIn( this, 93, 149, MakePotionKeg( PotionEffect.PoisonDeadly, 0x46 ) );
                PlaceItemIn( this, 117, 149, MakePotionKeg( PotionEffect.RefreshTotal, 0x21 ) );
                PlaceItemIn( this, 141, 149, MakePotionKeg( PotionEffect.ExplosionGreater, 0x74 ) );

                PlaceItemIn( this, 93, 82, new Bottle( amount ) );
            }
            else if( type == BagType.Tools )
            {
                Name = "Tool Bag";
                Hue = Utility.RandomMetalHue();

                PlaceItemIn( this, 30, 35, new TinkerTools( amount * 10 ) );
                PlaceItemIn( this, 60, 35, new HousePlacementTool() );
                PlaceItemIn( this, 90, 35, new DovetailSaw( amount * 10 ) );
                PlaceItemIn( this, 30, 60, new Scissors() );
                PlaceItemIn( this, 45, 60, new MortarPestle( amount * 10 ) );
                PlaceItemIn( this, 75, 60, new ScribesPen( amount * 10 ) );
                PlaceItemIn( this, 90, 60, new SmithHammer( amount * 10 ) );
                PlaceItemIn( this, 30, 85, new TwoHandedAxe() );
                PlaceItemIn( this, 60, 85, new FletcherTools( amount * 10 ) );
                PlaceItemIn( this, 90, 85, new SewingKit( amount * 10 ) );
            }
            else if( type == BagType.Amno )
            {
                Name = "Bag Of Archery Ammo";
                Hue = Utility.RandomMetalHue();

                PlaceItemIn( this, 48, 76, new Arrow( amount ) );
                PlaceItemIn( this, 72, 76, new Bolt( amount ) );
            }
            else if( type == BagType.Maps )
            {
                Name = "Bag Of Treasure Maps";
                Hue = Utility.RandomMetalHue();

                PlaceItemIn( this, 30, 35, new TreasureMap( 1, Map.Felucca ) );
                PlaceItemIn( this, 45, 35, new TreasureMap( 2, Map.Felucca ) );
                PlaceItemIn( this, 60, 35, new TreasureMap( 3, Map.Felucca ) );
                PlaceItemIn( this, 75, 35, new TreasureMap( 4, Map.Felucca ) );
                PlaceItemIn( this, 90, 35, new TreasureMap( 5, Map.Felucca ) );
                PlaceItemIn( this, 90, 35, new TreasureMap( 6, Map.Felucca ) );

                PlaceItemIn( this, 30, 50, new TreasureMap( 1, Map.Felucca ) );
                PlaceItemIn( this, 45, 50, new TreasureMap( 2, Map.Felucca ) );
                PlaceItemIn( this, 60, 50, new TreasureMap( 3, Map.Felucca ) );
                PlaceItemIn( this, 75, 50, new TreasureMap( 4, Map.Felucca ) );
                PlaceItemIn( this, 90, 50, new TreasureMap( 5, Map.Felucca ) );
                PlaceItemIn( this, 90, 50, new TreasureMap( 6, Map.Felucca ) );

                PlaceItemIn( this, 55, 100, new Lockpick( amount ) );
                PlaceItemIn( this, 60, 100, new Pickaxe() );
            }
            else if( type == BagType.Materials )
            {
                Hue = Utility.RandomMetalHue();
                Name = "Raw Materials Bag";

                PlaceItemIn( this, 92, 60, new TailorBag( amount ) );

                PlaceItemIn( this, 30, 118, new Cloth( amount ) );
                PlaceItemIn( this, 30, 84, new CarpenterBag( amount ) );
                PlaceItemIn( this, 57, 80, new BlankScroll( amount ) );

                PlaceItemIn( this, 30, 35, new BagOfingots( amount ) );

                PlaceItemIn( this, 30, 59, new RedScales( amount ) );
                PlaceItemIn( this, 36, 59, new YellowScales( amount ) );
                PlaceItemIn( this, 42, 59, new BlackScales( amount ) );
                PlaceItemIn( this, 48, 59, new GreenScales( amount ) );
                PlaceItemIn( this, 54, 59, new WhiteScales( amount ) );
                PlaceItemIn( this, 60, 59, new BlueScales( amount ) );
            }
            else if( type == BagType.SpellStuff )
            {
                Hue = Utility.RandomMetalHue();
                Name = "Spell Casting Stuff";

                PlaceItemIn( this, 45, 105, new Spellbook( UInt64.MaxValue ) );

                Runebook runebook = new Runebook( 10 );
                runebook.CurCharges = runebook.MaxCharges;
                PlaceItemIn( this, 145, 105, runebook );

                Item toHue = new BagOfReagents( amount );
                toHue.Hue = 0x2D;
                PlaceItemIn( this, 45, 150, toHue );

                for( int i = 0; i < 9; ++i )
                    PlaceItemIn( this, 45 + ( i * 10 ), 75, new RecallRune() );

            }
            else if( type == BagType.Alchemical )
            {
                Name = "Bag Of Alchemical resources";
                Hue = Utility.RandomMetalHue();

                PlaceItemIn( this, 30, 35, new AlchemicalBag( amount ) );
                PlaceItemIn( this, 45, 35, new BagOfPaganReagents( amount ) );
                PlaceItemIn( this, 60, 35, new PaganAlchemicalBag( amount ) );
                PlaceItemIn( this, 75, 35, new AlchemistTableSouthDeed() );
            }
        }

        private static void PlaceItemIn( Item parent, int x, int y, Item item )
        {
            parent.AddItem( item );
            item.Location = new Point3D( x, y, 0 );
        }

        private static Item MakePotionKeg( PotionEffect type, int hue )
        {
            PotionKeg keg = new PotionKeg();

            keg.Held = 100;
            keg.Type = type;
            keg.Hue = hue;

            return MakeNewbie( keg );
        }

        private static Item MakeNewbie( Item item )
        {
            item.LootType = LootType.Newbied;

            return item;
        }

        #region serialization
        public BagOfTestitems( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    public sealed class MidgardTestStone : Item
    {
        private static TestStoneEntry[] m_Table = new TestStoneEntry[]
        {
            new TestStoneEntry( "gold",         typeof( BankCheck ), 1000000 ),
            new TestStoneEntry( "reagents",     typeof( BagOfReagents ), 100 ),
            new TestStoneEntry( "ingots",       typeof( BagOfingots ), 100 ),
            new TestStoneEntry( "house deeds",  typeof( BagOfHouseDeeds ) ),
            new TestStoneEntry( "kegs",         typeof( BagOfTestitems ),BagOfTestitems.BagType.Kegs  ),
            new TestStoneEntry( "tools",        typeof( BagOfTestitems ),BagOfTestitems.BagType.Tools ),
            new TestStoneEntry( "amno",         typeof( BagOfTestitems ),BagOfTestitems.BagType.Amno ),
            new TestStoneEntry( "maps",         typeof( BagOfTestitems ),BagOfTestitems.BagType.Maps ),
            new TestStoneEntry( "materials",    typeof( BagOfTestitems ),BagOfTestitems.BagType.Materials ),
            new TestStoneEntry( "magical items",typeof( BagOfTestitems ),BagOfTestitems.BagType.SpellStuff ),
            new TestStoneEntry( "alchemicals",  typeof( BagOfTestitems ),BagOfTestitems.BagType.Alchemical ),
            new TestStoneEntry( "artifacts",    typeof( BagOfOldArtifacts ) ),
            new TestStoneEntry( "old weapons",  typeof( BagOfOldWeapons ) ),
            new TestStoneEntry( "enchant stones", typeof( BagOfEnchantingStones ), 3 ),
            new TestStoneEntry( "adv smelting", typeof( BagOfAdvancedSmelting ), 100 ),
            new TestStoneEntry( "pagan regs",   typeof( BagOfPaganReagents ), 50 ),
            new TestStoneEntry( "pagan alcs",   typeof( PaganAlchemicalBag ), 5 ),
            new TestStoneEntry( "traps",        typeof( BagOfTraps ), 5 ),
            new TestStoneEntry( "door traps",   typeof( DoorTrapsBag ), 5 ),
            new TestStoneEntry( "practice weapons", typeof( BagOfPracticeWeapons ) ),
            new TestStoneEntry( "thief things", typeof( ThiefBag ), 3 ),

            new TestStoneEntry( "crop seeds", typeof( BagOfCropSeeds ), 3 ),
            new TestStoneEntry( "bag of gardening", typeof( BagOfGarneding ), 3 ),
            new TestStoneEntry( "reagents crop seeds", typeof( BagOfReagentCropSeeds ), 3 ),
            new TestStoneEntry( "tree seeds", typeof( BagOfTreeSeeds ), 3 ),

            new TestStoneEntry( "various armors/shields", typeof( BagOfItemTestingArAndSh ) ),
            new TestStoneEntry( "various wearables", typeof( BagOfItemTestingClothing )),
            new TestStoneEntry( "various weapons", typeof( BagOfItemTestingWeapons )),
            new TestStoneEntry( "various misc items", typeof( BagOfItemTestingMisc ) ),

            new TestStoneEntry( "bag of barbarian armor", typeof( BagOfBarbarianArmor ) ),
            new TestStoneEntry( "bag of cruciform armor", typeof( BagOfCruciformArmor ) ),
            new TestStoneEntry( "bag of elven armor", typeof( BagOfElvenArmor ) ),
            new TestStoneEntry( "bag of evil armor", typeof( BagOfEvilArmor ) ),
            new TestStoneEntry( "bag of scout armor", typeof( BagOfScoutArmor ) ),
            new TestStoneEntry( "bag of sea armor", typeof( BagOfSeaArmor ) ),

            new TestStoneEntry( "bag of druid ritual items", typeof( BagOfDruidRitualItems ) ),
            new TestStoneEntry( "bag of paladin ritual items", typeof( BagOfPaladinRitualItems ) ),
            new TestStoneEntry( "bag of necro ritual items", typeof( BagOfNecromancerRitualItems ) ),

            new TestStoneEntry( "bag of scout items", typeof( BagOfScoutEquipment ) )
        };

        public override string DefaultName
        {
            get { return "a test stone"; }
        }

        [Constructable]
        public MidgardTestStone()
            : base( 0xED4 )
        {
            Movable = false;
            Hue = 0x2D1;
        }

        public override void OnDoubleClick( Mobile from )
        {
            from.CloseGump( typeof( InternalGump ) );
            from.SendGump( new InternalGump( from ) );
        }

        #region serialization
        public MidgardTestStone( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion

        private class TestStoneEntry
        {
            private Type m_ItemType;
            private string m_NameString;
            private object[] m_Args;

            public string NameString { get { return m_NameString; } }

            public TestStoneEntry( string name, Type itemType, params object[] args )
            {
                m_ItemType = itemType;
                m_NameString = name;
                m_Args = args;
            }

            public Item Construct()
            {
                try
                {
                    Item item = Activator.CreateInstance( m_ItemType, m_Args ) as Item;
                    return item;
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }

                return null;
            }
        }

        private class InternalGump : Gump
        {
            #region constants
            private const int Fields = 9;
            private const int HueTit = 662;
            private const int DeltaBut = 2;
            private const int FieldsDist = 25;
            private const int HuePrim = 92;
            #endregion

            #region fields
            private int m_Page;
            private Mobile m_From;
            #endregion

            #region constructors
            public InternalGump( Mobile from )
                : this( from, 1 )
            {
            }

            private InternalGump( Mobile from, int page )
                : base( 50, 50 )
            {
                Closable = false;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                m_From = from;
                m_Page = page;

                Design();
            }
            #endregion

            #region members
            private void Design()
            {
                AddPage( 0 );

                AddBackground( 0, 0, 275, 325, 9200 );

                AddImageTiled( 10, 10, 255, 25, 2624 );
                AddImageTiled( 10, 45, 255, 240, 2624 );
                AddImageTiled( 40, 295, 225, 20, 2624 );

                AddButton( 10, 295, 4017, 4018, 0, GumpButtonType.Reply, 0 );
                AddHtmlLocalized( 45, 295, 75, 20, 1011012, HueTit, false, false ); // CANCEL

                AddAlphaRegion( 10, 10, 255, 285 );
                AddAlphaRegion( 40, 295, 225, 20 );

                AddLabelCropped( 14, 12, 255, 25, HueTit, "Choose an object" );

                if( m_Page > 1 )
                    AddButton( 225, 297, 5603, 5607, 200, GumpButtonType.Reply, 0 ); // Previous page

                if( m_Page < Math.Ceiling( m_Table.Length / (double)Fields ) )
                    AddButton( 245, 297, 5601, 5605, 300, GumpButtonType.Reply, 0 ); // Next Page

                int indMax = ( m_Page * Fields ) - 1;
                int indMin = ( m_Page * Fields ) - Fields;
                int indTemp = 0;

                for( int i = 0; i < m_Table.Length; i++ )
                {
                    if( i >= indMin && i <= indMax )
                    {
                        AddLabelCropped( 35, 52 + ( indTemp * FieldsDist ), 225, 20, HuePrim, m_Table[ i ].NameString );
                        AddButton( 15, 52 + DeltaBut + ( indTemp * FieldsDist ), 1209, 1210, i + 1, GumpButtonType.Reply, 0 );
                        indTemp++;
                    }
                }
            }

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                Mobile from = sender.Mobile;

                if( info.ButtonID == 0 )
                    return;
                else if( info.ButtonID == 200 ) // Previous page
                {
                    m_Page--;
                    from.SendGump( new InternalGump( m_From, m_Page ) );
                }
                else if( info.ButtonID == 300 )  // Next Page
                {
                    m_Page++;
                    from.SendGump( new InternalGump( m_From, m_Page ) );
                }
                else
                {
                    Item item = m_Table[ info.ButtonID - 1 ].Construct();
                    if( item != null )
                    {
                        if( from.Backpack.TryDropItem( from, item, true ) )
                            from.SendMessage( "An item has been placed in your pack: {0}",
                                             m_Table[ info.ButtonID - 1 ].NameString );
                        else
                        {
                            item.MoveToWorld( from.Location, from.Map );
                            from.SendMessage( "An item has been placed at your feet: {0}",
                                             m_Table[ info.ButtonID - 1 ].NameString );
                        }
                    }
                }
            }
            #endregion
        }
    }
}