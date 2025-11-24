using System;
using System.Collections.Generic;
using Midgard.Engines.CheeseCrafting;
using Server;
using Server.Items;
using Server.Targeting;
using Midgard.Items;

namespace Midgard.Engines.AdvancedCooking
{
    public class FoodHelper
    {
        /// <summary>
        /// Check if object targeted is a valid cooking heat source
        /// </summary>
        public static bool IsHeatSource( object targeted )
        {
            int itemID;

            if( targeted is Item )
                itemID = ( (Item)targeted ).ItemID & 0x3FFF;
            else if( targeted is StaticTarget )
                itemID = ( (StaticTarget)targeted ).ItemID & 0x3FFF;
            else
                return false;

            if( itemID >= 0xDE3 && itemID <= 0xDE9 )
                return true;
            else if( itemID >= 0x461 && itemID <= 0x48E )
                return true;
            else if( itemID >= 0x92B && itemID <= 0x96C )
                return true;
            else if( itemID == 0xFAC )
                return true;
            else if( itemID >= 0x398C && itemID <= 0x399F )
                return true;
            else if( itemID == 0xFB1 )
                return true;
            else if( itemID >= 0x197A && itemID <= 0x19A9 )
                return true;
            else if( itemID >= 0x184A && itemID <= 0x184C )
                return true;
            else if( itemID >= 0x184E && itemID <= 0x1850 )
                return true;

            return false;
        }

        public static bool IsFlourMill( object targeted )
        {
            int itemID;

            if( targeted is Item )
                itemID = ( (Item)targeted ).ItemID & 0x3FFF;
            else if( targeted is StaticTarget )
                itemID = ( (StaticTarget)targeted ).ItemID & 0x3FFF;
            else
                return false;

            if( itemID >= 0x1883 && itemID <= 0x1893 )
                return true;
            else if( itemID >= 0x1920 && itemID <= 0x1937 )
                return true;

            return false;
        }

        public static void GiveFood( Mobile toGive, Item food )
        {
            GiveFood( toGive, food, null );
        }

        /// <summary>
        /// Try to place given food to 'toGive' mobile. If not success place
        /// that food at ground. If 'message' is not null it is sent to 'toGive'
        /// </summary>
        public static void GiveFood( Mobile toGive, Item food, string message )
        {
            Container pack = toGive.Backpack;

            if( pack == null || !pack.TryDropItem( toGive, food, false ) )
                food.MoveToWorld( toGive.Location, toGive.Map );

            if( !String.IsNullOrEmpty( message ) )
                toGive.SendMessage( message );
        }

        private static Dictionary<Type, string> m_NameList = new Dictionary<Type, string>();

        private static void Initialize()
        {
            if( m_NameList == null )
                m_NameList = new Dictionary<Type, string>();

            m_NameList.Add( typeof( Apple ), "apple" );
            m_NameList.Add( typeof( Banana ), "Banana" );
            m_NameList.Add( typeof( Bananas ), "Bananas" );
            m_NameList.Add( typeof( Cantaloupe ), "Cantaloupe" );
            m_NameList.Add( typeof( Coconut ), "Coconut" );
            m_NameList.Add( typeof( SplitCoconut ), "Coconut" );
            m_NameList.Add( typeof( Cucumber ), "Cucumber" );
            m_NameList.Add( typeof( Dates ), "Dates" );
            m_NameList.Add( typeof( Grapes ), "Grapes" );
            m_NameList.Add( typeof( HoneydewMelon ), "HoneydewMelon" );
            m_NameList.Add( typeof( Lemon ), "Lemon" );
            m_NameList.Add( typeof( Orange ), "Orange" );
            m_NameList.Add( typeof( Peach ), "Peach" );
            m_NameList.Add( typeof( Pear ), "Pear" );
            m_NameList.Add( typeof( Pumpkin ), "Pumpkin" );
            m_NameList.Add( typeof( Tomato ), "Tomato" );
            m_NameList.Add( typeof( Watermelon ), "Watermelon" );
            m_NameList.Add( typeof( RawBacon ), "Bacon" );
            m_NameList.Add( typeof( Bacon ), "Bacon" );
            m_NameList.Add( typeof( ChickenLeg ), "Chicken meat" );
            m_NameList.Add( typeof( RawChickenLeg ), "Chicken meat" );
            m_NameList.Add( typeof( CookedBird ), "bird meat" );
            m_NameList.Add( typeof( RawBird ), "bird meat" );
            m_NameList.Add( typeof( FishSteak ), "fish meat" );
            m_NameList.Add( typeof( RawFishSteak ), "fish meat" );
            m_NameList.Add( typeof( RawHamSlices ), "ham meat" );
            m_NameList.Add( typeof( HamSlices ), "ham meat" );
            m_NameList.Add( typeof( LambLeg ), "lamb meat" );
            m_NameList.Add( typeof( RawLambLeg ), "lamb meat" );
            m_NameList.Add( typeof( Ribs ), "ribs" );
            m_NameList.Add( typeof( RawRibs ), "ribs" );
            m_NameList.Add( typeof( Sausage ), "sausage" );
            m_NameList.Add( typeof( Dough ), "thick crust" );
            m_NameList.Add( typeof( TanMushroom ), "mushrooms" );
            m_NameList.Add( typeof( RedMushroom ), "mushrooms" );
            // m_NameList.Add( typeof( Silverleaf ), "silverleaf" );
            // m_NameList.Add( typeof( Spam ), "spam" );
            m_NameList.Add( typeof( Garlic ), "garlic" );
            m_NameList.Add( typeof( Ginseng ), "ginseng" );
            m_NameList.Add( typeof( Cabbage ), "cabbage" );
            m_NameList.Add( typeof( Carrot ), "carrot" );
            m_NameList.Add( typeof( Corn ), "corn" );
            m_NameList.Add( typeof( Ginseng ), "ginseng" );
            m_NameList.Add( typeof( Lettuce ), "lettuce" );
            m_NameList.Add( typeof( Onion ), "onion" );
            m_NameList.Add( typeof( Turnip ), "turnip" );
            m_NameList.Add( typeof( BrightlyColoredEggs ), "surprise" );
            m_NameList.Add( typeof( EasterEggs ), "surprise" );
            m_NameList.Add( typeof( Eggs ), "egg" );
            m_NameList.Add( typeof( FriedEggs ), "egg" );
            m_NameList.Add( typeof( FishHeads ), "fish head" );
            m_NameList.Add( typeof( CheeseWedgeSmall ), "extra cheese" );
            // m_NameList.Add( typeof( RedRaspberry ), "raspberry" );
            // m_NameList.Add( typeof( BlackRaspberry ), "raspberry" );
            // m_NameList.Add( typeof( Strawberries ), "strawberry" );
            m_NameList.Add( typeof( CowRawCheeseWedgeSmall ), "cow cheese" );
            m_NameList.Add( typeof( GoatRawCheeseWedgeSmall ), "goat cheese" );
            m_NameList.Add( typeof( SheepRawCheeseWedgeSmall ), "sheep cheese" );
        }

        /// <summary>
        /// Get a name for a given ingredient
        /// </summary>
        public static string GetNameFromType( Type t )
        {
            string name;

            if( m_NameList == null )
                Initialize();

            if( !m_NameList.TryGetValue( t, out name ) )
                return string.Empty;
            else
                return name;
        }

        /// <summary>
        /// Check if an ingredient has a name in m_NameList table
        /// </summary>
        public static bool IsNamedIngredient( Type t )
        {
            if( m_NameList == null )
                Initialize();

            return m_NameList.ContainsKey( t );
        }

        private static Type[] m_TooLargeIngredients = new Type[]
          {
              typeof (RawHam),
              typeof (Ham),
              typeof (RawBaconSlab),
              typeof (BaconSlab),
              typeof (CheeseWheel),
              typeof (CheeseWedge),
              typeof (GoatRawCheeseForm),
              typeof (GoatRawCheeseWedge),
              typeof (SheepRawCheeseForm),
              typeof (SheepRawCheeseWedge),
              typeof (CowRawCheeseForm),
              typeof (CowRawCheeseWedge),
          };

        /// <summary>
        /// Check if an ingredient must be scissored before cooking
        /// </summary>
        public static bool IsTooLargeIngredient( Type t )
        {
            return Array.LastIndexOf( m_TooLargeIngredients, t ) > -1;
        }

        public class InternalTimer : Timer
        {
            private Mobile m_From;
            private IPoint3D m_Point;
            private Map m_Map;
            private int Min;
            private int Max;
            private Server.Items.Food m_CookedFood;

            public InternalTimer( Mobile from, IPoint3D p, Map map, int min, int max, Server.Items.Food cookedFood )
                : base( TimeSpan.FromSeconds( 1.0 ) )
            {
                m_From = from;
                m_Point = p;
                m_Map = map;
                Min = min;
                Max = max;
                m_CookedFood = cookedFood;

            }

            protected override void OnTick()
            {
                m_From.EndAction( typeof( Item ) );

                if( m_From.Map != m_Map || ( m_Point != null && m_From.GetDistanceToSqrt( m_Point ) > 3 ) )
                {
                    m_From.SendLocalizedMessage( 500686 );
                    return;
                }

                if( m_From.CheckSkill( SkillName.Cooking, Min, Max ) )
                {
                    if( m_From.AddToBackpack( m_CookedFood ) )
                        m_From.PlaySound( 0x57 );
                }
                else
                {
                    m_From.PlaySound( 0x57 );

                    m_From.SendLocalizedMessage( 500686 );
                }
            }
        }
    }
}
