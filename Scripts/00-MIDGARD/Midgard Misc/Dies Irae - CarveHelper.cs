/***************************************************************************
 *                               CarveHelper
 *                            -----------------
 *   begin                : 27 gennaio, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.IO;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Midgard.Misc
{
    public class CarveHelper
    {
        private static string ConfigFile = Core.AOS ? "AOSLeatherLoot.cfg" : "OldLeatherLoot.cfg";

        public static readonly bool CarverHelperEnabled = true;

        private static Dictionary<Type, LeatherDefinition> m_Dict = new Dictionary<Type, LeatherDefinition>();

        private class LeatherDefinition
        {
            public double Chance { get; private set; }
            public HideType Hide { get; private set; }
            public LootPackDice Quantity { get; private set; }
            public int QuantityInt { get; private set; }

            public LeatherDefinition( double chance, HideType hide, LootPackDice quantity )
            {
                Chance = chance;
                Hide = hide;
                Quantity = quantity;
                QuantityInt = -1;
            }

            public LeatherDefinition( double chance, HideType hide, int quantityInt )
            {
                Chance = chance;
                Hide = hide;
                QuantityInt = quantityInt;
            }
        }

        public static void Initialize()
        {
            string path = Path.Combine( Core.BaseDirectory, string.Format( "Data/{0}", ConfigFile ) );

            if( File.Exists( path ) )
            {
                using( StreamReader ip = new StreamReader( path ) )
                {
                    string line;

                    while( ( line = ip.ReadLine() ) != null )
                    {
                        if( line.Length == 0 || line.StartsWith( "#" ) )
                            continue;

                        string[] split = line.Split( '\t' );

                        try
                        {
                            Type creatureType = ScriptCompiler.FindTypeByName( split[ 0 ], true );
                            double chance = double.Parse( split[ 1 ] );
                            HideType hide = (HideType)Enum.Parse( typeof( HideType ), split[ 2 ], true );

                            if( !Core.AOS )
                            {
                                LootPackDice quantity = new LootPackDice( split[ 3 ] );
                                m_Dict.Add( creatureType, new LeatherDefinition( chance, hide, quantity ) );
                            }
                            else
                            {
                                int aosQuantity = int.Parse( split[ 3 ] );
                                m_Dict.Add( creatureType, new LeatherDefinition( chance, hide, aosQuantity ) );
                            }
                        }
                        catch
                        {
                            try
                            {
                                using( StreamWriter op = new StreamWriter( "Logs/leather-loot-errors.log", true ) )
                                {
                                    op.WriteLine( "{0}", DateTime.Now );
                                    op.WriteLine( "Warning: Invalid leather definition entry:" );
                                    op.WriteLine( line );
                                    op.WriteLine();
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine( "Warning: Leather loot config file does not exist" );
            }
        }

        public static readonly double RarityMultiplier = 1.0;

        public static bool IsSpecialCarveTarget( Type t )
        {
            return m_Dict.ContainsKey( t );
        }

        public static bool HandleLeatherCarve( Mobile from, BaseCreature bc, Corpse corpse, Item with )
        {
            if( bc == null || corpse == null )
                return false;

            LeatherDefinition ld = null;
            m_Dict.TryGetValue( bc.GetType(), out ld );
            if( ld == null )
                return false;

            double chance = Utility.RandomDouble();

            if( Core.Debug )
                Utility.Log( "CarveHelperHandleLeatherCarve.log", "Debug HandleLeatherCarve. random: {0}, chance: {1}", chance.ToString( "F2" ), ld.Chance.ToString( "F2" ) );

            if( chance < ld.Chance * RarityMultiplier )
            {
                if( Core.AOS )
                {
                    if( ld.Hide == HideType.Humanoid )
                        corpse.DropItem( BaseCreature.MakeInstanceOwner( new HumanoidHides( ld.QuantityInt ), from ) );
                    else if( ld.Hide == HideType.Undead )
                        corpse.DropItem( BaseCreature.MakeInstanceOwner( new UndeadHides( ld.QuantityInt ), from ) );
                    else if( ld.Hide == HideType.Wolf )
                        corpse.DropItem( BaseCreature.MakeInstanceOwner( new WolfHides( ld.QuantityInt ), from ) );
                    else if( ld.Hide == HideType.Aracnid )
                        corpse.DropItem( BaseCreature.MakeInstanceOwner( new AracnidHides( ld.QuantityInt ), from ) );
                    else if( ld.Hide == HideType.Fey )
                        corpse.DropItem( BaseCreature.MakeInstanceOwner( new FeyHides( ld.QuantityInt ), from ) );
                    else if( ld.Hide == HideType.GreenDragon )
                        corpse.DropItem( BaseCreature.MakeInstanceOwner( new GreenDragonHides( ld.QuantityInt ), from ) );
                    else if( ld.Hide == HideType.BlackDragon )
                        corpse.DropItem( BaseCreature.MakeInstanceOwner( new BlackDragonHides( ld.QuantityInt ), from ) );
                    else if( ld.Hide == HideType.BlueDragon )
                        corpse.DropItem( BaseCreature.MakeInstanceOwner( new BlueDragonHides( ld.QuantityInt ), from ) );
                    else if( ld.Hide == HideType.RedDragon )
                        corpse.DropItem( BaseCreature.MakeInstanceOwner( new RedDragonHides( ld.QuantityInt ), from ) );
                    else if( ld.Hide == HideType.Abyss )
                        corpse.DropItem( BaseCreature.MakeInstanceOwner( new AbyssHides( ld.QuantityInt ), from ) );
                }
                else
                {
                    if( ld.Hide == HideType.Arctic )
                        corpse.DropItem( BaseCreature.MakeInstanceOwner( new ArcticHides( ld.Quantity.Roll() ), from ) );
                    else if( ld.Hide == HideType.Orcish )
                        corpse.DropItem( BaseCreature.MakeInstanceOwner( new OrcishHides( ld.Quantity.Roll() ), from ) );
                    else if( ld.Hide == HideType.Ophidian )
                        corpse.DropItem( BaseCreature.MakeInstanceOwner( new HornedHides( ld.Quantity.Roll() ), from ) );
                    else if( ld.Hide == HideType.Lava )
                        corpse.DropItem( BaseCreature.MakeInstanceOwner( new LavaHides( ld.Quantity.Roll() ), from ) );
                    else if( ld.Hide == HideType.Reptile )
                        corpse.DropItem( BaseCreature.MakeInstanceOwner( new SpinedHides( ld.Quantity.Roll() ), from ) );
                    else if( ld.Hide == HideType.Undead )
                        corpse.DropItem( BaseCreature.MakeInstanceOwner( new UndeadHides( ld.Quantity.Roll() ), from ) );
                    else if( ld.Hide == HideType.Humanoid )
                        corpse.DropItem( BaseCreature.MakeInstanceOwner( new BarbedHides( ld.Quantity.Roll() ), from ) );
                    else if( ld.Hide == HideType.Arachnid )
                        corpse.DropItem( BaseCreature.MakeInstanceOwner( new AracnidHides( ld.Quantity.Roll() ), from ) );
                    else if( ld.Hide == HideType.Wolf )
                        corpse.DropItem( BaseCreature.MakeInstanceOwner( new WolfHides( ld.Quantity.Roll() ), from ) );
                    else if( ld.Hide == HideType.Bear )
                        corpse.DropItem( BaseCreature.MakeInstanceOwner( new BearHides( ld.Quantity.Roll() ), from ) );
                    else if( ld.Hide == HideType.RedDragon )
                        corpse.DropItem( BaseCreature.MakeInstanceOwner( new RedDragonHides( ld.Quantity.Roll() ), from ) );
                    else if( ld.Hide == HideType.GreenDragon )
                        corpse.DropItem( BaseCreature.MakeInstanceOwner( new GreenDragonHides( ld.Quantity.Roll() ), from ) );
                    else if( ld.Hide == HideType.BlackDragon )
                        corpse.DropItem( BaseCreature.MakeInstanceOwner( new BlackDragonHides( ld.Quantity.Roll() ), from ) );
                    else if( ld.Hide == HideType.BlueDragon )
                        corpse.DropItem( BaseCreature.MakeInstanceOwner( new BlueDragonHides( ld.Quantity.Roll() ), from ) );
                    else if( ld.Hide == HideType.Demon )
                        corpse.DropItem( BaseCreature.MakeInstanceOwner( new AbyssHides( ld.Quantity.Roll() ), from ) );
                }

                return true;
            }

            return false;
        }
    }
}