/***************************************************************************
 *                                  HarvestHelper.cs
 *                                  -------------------
 *  begin                   : Dicembre, 2007
 *  version                 : 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright               : Midgard Uo Shard - Matteo Visintin
 *  email                   : tocasia@alice.it
 *
 ***************************************************************************/

#define DebugGetVeinAt
#define DebugMutateType
#define DebugGetHarvestVeinFromTileId

using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Engines.Harvest;
using Server.Network;

namespace Midgard
{
    public class HarvestHelper
    {
        public static readonly bool TalismansEnabled = true;
        public static readonly bool RewardsEnabled = true;
        public static readonly bool PerfectGemsEnabled = true;

        public static bool GiveLumberBonus( HarvestSystem system, Mobile m, Item item, bool placeAtFeet )
        {
            if( !TalismansEnabled )
                return false;

            if( m.Skills.Lumberjacking.Value >= 100 )
            {
                if( Utility.RandomDouble() < 0.15 )
                {
                    Item sitem;
                    int message;
                    double chance = Utility.RandomDouble();

                    if( chance < 0.0025 )
                    {
                        sitem = new BrilliantAmber();
                        message = 1072551; // You found a brilliant amber!
                    }
                    else if( chance < 0.05 )
                    {
                        sitem = new ParasiticPlant();
                        message = 1072549; // You found a parasitic plant!
                    }
                    else if( chance < 0.35 )
                    {
                        if( Utility.RandomBool() )
                        {
                            // Sostituito Switch con un oggetto alternativo
                            sitem = new Log();
                            message = 1072547; // You found a switch! (modificato)
                        }
                        else
                        {
                            sitem = new LuminescentFungi();
                            message = 1072550; // You found a luminescent fungi!
                        }
                    }
                    else
                    {
                        sitem = new BarkFragment();
                        message = 1072548; // You found a bark fragment!
                    }

                    if( sitem != null )
                    {
                        if( !m.PlaceInBackpack( sitem ) && placeAtFeet )
                        {
                            ArrayList atFeet = new ArrayList();

                            foreach( Item obj in m.GetItemsInRange( 0 ) )
                                atFeet.Add( obj );

                            for( int i = 0; i < atFeet.Count; ++i )
                            {
                                Item check = (Item)atFeet[ i ];

                                if( check.StackWith( m, sitem, false ) )
                                    return system.Give( m, item, placeAtFeet );
                            }

                            sitem.MoveToWorld( m.Location, m.Map );
                        }
                        m.SendLocalizedMessage( message );
                    }
                }
            }

            return system.Give( m, item, placeAtFeet );
        }

        public static bool GiveMiningBonus( HarvestSystem system, Mobile m, Item item, bool placeAtFeet )
        {
            if( !RewardsEnabled )
                return false;

            if( m.Skills.Mining.Value >= 90 && RewardsEnabled )
            {
                if( Utility.RandomDouble() < 0.05 || m.AccessLevel == AccessLevel.Developer )
                {
                    m.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1064376 ); // * You have found something Special *
                    m.PlaySound( 0x244 );

                    Item sitem = null;
                    int message = 0;
                    double chance = Utility.RandomDouble();

                    #region generazione dell'oggetto random
                    if( chance < 0.05 )         // intervallo del 5% sul totale
                    {
                        switch( Utility.RandomMinMax( 1, 3 ) )
                        {
                            case 1:
                                sitem = new AncientSmithyHammer( Utility.RandomMinMax( 1, 5 ) );
                                message = 1064377; // You found an Ancient Smith Hammer!
                                break;
                            case 2:
                                sitem = new TreasureMap( Utility.RandomMinMax( 3, 5 ), m.Map );
                                message = 1064378; // You found a Treasure Map!
                                break;
                            case 3:
                                CraftResource resourceType = (CraftResource)Utility.RandomMinMax( 2, 7 );
                                sitem = new RunicHammer( resourceType, Utility.Random( 2 ) + 1 );
                                message = 1064379; // You found a Runic Hammer!
                                break;
                            default:
                                sitem = new TreasureMap( Utility.RandomMinMax( 2, 5 ), m.Map );
                                message = 1064378; // You found a Treasure Map!
                                break;
                        }
                    }
                    else if( chance < 0.2 )     // intervallo del 10% sul totale
                    {
                        sitem = Loot.RandomArmorOrShieldOrJewelry();

                        if( sitem != null )
                        {
                            // Sistema semplificato senza PreAoSLootHelper
                            int attributi = Utility.RandomMinMax( 1, 3 );
                            for( int i = 0; i < attributi; i++ )
                            {
                                if( sitem is BaseWeapon )
                                    ((BaseWeapon)sitem).Attributes.WeaponDamage += Utility.RandomMinMax( 1, 10 );
                                else if( sitem is BaseArmor )
                                    ((BaseArmor)sitem).Attributes.DefendChance += Utility.RandomMinMax( 1, 5 );
                            }
                        }

                        message = 1064380; // You found a Valuable Thing!
                    }
                    else if( chance < 0.3 )     // intervallo del 10% sul totale
                    {
                        if( PerfectGemsEnabled )
                        {
                            switch( Utility.Random( 6 ) )
                            {
                                case 0: sitem = new Emerald(); break;
                                case 1: sitem = new Sapphire(); break;
                                case 2: sitem = new Tourmaline(); break;
                                case 3: sitem = new Citrine(); break;
                                case 4: sitem = new Ruby(); break;
                                case 5: sitem = new Diamond(); break;
                            }
                            message = 1064381; // You found a precious Gem!
                        }
                        else
                        {
                            sitem = Loot.RandomGem();
                            message = 1064381; // You found a precious Gem!
                        }
                    }
                    else if( chance < 0.7 )     // intervallo del 30% sul totale
                    {
                        sitem = Loot.RandomGem();
                        message = 1064381; // You found a precious Gem!
                    }
                    else                        // intervallo del 45% sul totale
                    {
                        sitem = RandomMiningCommonReward();
                        message = 1064382; // You found something special!
                    }
                    #endregion

                    if( sitem != null )
                    {
                        if( !m.PlaceInBackpack( sitem ) && placeAtFeet )
                        {
                            ArrayList atFeet = new ArrayList();

                            foreach( Item obj in m.GetItemsInRange( 0 ) )
                                atFeet.Add( obj );

                            for( int i = 0; i < atFeet.Count; ++i )
                            {
                                Item check = (Item)atFeet[ i ];

                                if( check.StackWith( m, sitem, false ) )
                                    return system.Give( m, item, placeAtFeet );
                            }

                            sitem.MoveToWorld( m.Location, m.Map );
                        }
                        m.SendLocalizedMessage( message );
                    }
                }
            }

            return system.Give( m, item, placeAtFeet );
        }

        public static Item RandomMiningCommonReward()
        {
            switch( Utility.Random( 5 ) )
            {
                case 0: return new IronOre( Utility.RandomMinMax( 5, 10 ) );
                case 1: return new DullCopperOre( Utility.RandomMinMax( 3, 6 ) );
                case 2: return new ShadowIronOre( Utility.RandomMinMax( 2, 4 ) );
                case 3: return new CopperOre( Utility.RandomMinMax( 2, 4 ) );
                default: return new BronzeOre( Utility.RandomMinMax( 1, 3 ) );
            }
        }
    }
}