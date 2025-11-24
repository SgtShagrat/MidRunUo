/***************************************************************************
 *                               Dies Irae - ConvertServerToPreAos.cs
 *                            ------------------------------------------
 *   begin                : 20 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Midgard.Commands;
using Midgard.Engines.MidgardTownSystem;

using Server.Accounting;
using Server.Commands;
using Server;
using Server.Guilds;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Network;

namespace Midgard.Engines.ThirdCrownPorting
{
    public class ConvertServerToPreAos
    {
        public static void RegisterCommands()
        {
            CommandSystem.Register( "ConvertServerToPreAos", AccessLevel.Developer, new CommandEventHandler( ConvertServerToPreAos_OnCommand ) );
        }

        #region callback
        [Usage( "ConvertServerToPreAos" )]
        [Description( "Convert players and items to pre-aos era." )]
        public static void ConvertServerToPreAos_OnCommand( CommandEventArgs e )
        {
            // delete houses
            BaseHouse.DeleteAllHouses();
            Console.WriteLine( "All houses are wiped." );

            #region guilds
            List<BaseGuild> guilds = new List<BaseGuild>();

            foreach( BaseGuild g in BaseGuild.List.Values )
                guilds.Add( g );

            for( int i = 0; i < guilds.Count; i++ )
            {
                Guild guild = guilds[ i ] as Guild;
                if( guild != null && !guild.Disbanded )
                    guild.Disband();
            }
            Console.WriteLine( "All guildes are disbanded." );
            #endregion

            Point3D chestLoc = new Point3D( 5511, 1120, 0 );
            int locX = 0;
            int locY = 0;

            List<Account> accountList = new List<Account>();
            foreach( Account account in Accounts.GetAccounts() )
                accountList.Add( account );

            accountList.Sort( AccountComparer.Instance );

            for( int index = 0; index < accountList.Count; index++ )
            {
                Account a = accountList[ index ];
                if( a == null )
                    continue;

                if( a.Count < 1 )
                {
                    Console.WriteLine( "Account {0} is empty. It is deleted.", a.Username );
                    a.Delete();
                    continue;
                }

                if( a.Banned )
                {
                    Console.WriteLine( "Account {0} is banned. It is deleted.", a.Username );
                    a.Delete();
                    continue;
                }

                TownHelper.DoAccountReset( a ); // townsystem reset

                MetalBox itemsForAccount = new MetalBox();
                itemsForAccount.Hue = Utility.RandomMetalHue();
                itemsForAccount.Name = "items for account " + a.Username;
                itemsForAccount.MoveToWorld( new Point3D( chestLoc.X + locX, chestLoc.Y + locY, chestLoc.Z ), Map.Felucca );

                for( int i = 0; i < a.Length; i++ )
                {
                    PlayerMobile p = a[ i ] as PlayerMobile;
                    if( p == null || p.Deleted )
                        continue;

                    WoodenBox itemsForSinglePlayer = FillBox( "items for player " + p.Name, itemsForAccount, i );

                    #region equip
                    WoodenBox bagForEquip = FillBox( "items equipped by " + p.Name, itemsForSinglePlayer, 0 );

                    List<Item> equipitems = new List<Item>( p.Items );
                    for( int j = 0; j < equipitems.Count; j++ )
                    {
                        if( equipitems[ j ].Movable && ( equipitems[ j ].Layer != Layer.Bank ) && ( equipitems[ j ].Layer != Layer.Mount ) && ( equipitems[ j ].Layer != Layer.Backpack ) )
                        {
                            equipitems[ j ].IsPortingItem = true;
                            equipitems[ j ].PortingOwner = p;
                            bagForEquip.DropItem( equipitems[ j ] );
                        }
                    }

                    if( p.Backpack != null )
                    {
                        List<Item> packitems = new List<Item>( p.Backpack.Items );
                        for( int j = 0; j < packitems.Count; j++ )
                        {
                            packitems[ j ].IsPortingItem = true;
                            packitems[ j ].PortingOwner = p;
                            bagForEquip.DropItem( packitems[ j ] );
                        }
                    }
                    #endregion

                    #region bank
                    WoodenBox bagForBank = FillBox( "items in bank of " + p.Name, itemsForSinglePlayer, 1 );

                    List<Item> bankitems = new List<Item>( p.BankBox.Items );
                    for( int j = 0; j < bankitems.Count; j++ )
                    {
                        bankitems[ j ].IsPortingItem = true;
                        bankitems[ j ].PortingOwner = p;
                        bagForBank.DropItem( bankitems[ j ] );
                    }
                    #endregion

                    #region stable
                    if( p.Mounted )
                    {
                        IMount mount = p.Mount;
                        if( mount != null )
                            mount.Rider = null;

                        if( mount is BaseCreature )
                        {
                            BaseCreature petMounted = (BaseCreature)mount;

                            petMounted.ControlTarget = null;
                            petMounted.ControlOrder = OrderType.Stay;
                            petMounted.Internalize();
                            petMounted.SetControlMaster( null );
                            petMounted.SummonMaster = null;
                            petMounted.IsStabled = true;
                            petMounted.Loyalty = BaseCreature.MaxLoyalty;
                            p.Stabled.Add( petMounted );
                        }
                    }
                    #endregion

                    #region pets
                    WoodenBox pets = FillBox( "pets of " + p.Name, itemsForSinglePlayer, 2 );

                    for( int k = 0; k < p.Stabled.Count; ++k )
                    {
                        BaseCreature pet = p.Stabled[ k ] as BaseCreature;

                        if( pet == null || pet.Deleted )
                        {
                            if( pet != null )
                                pet.IsStabled = false;
                            p.Stabled.RemoveAt( k );
                            --k;
                            continue;
                        }

                        pet.SetControlMaster( p );

                        if( pet.Summoned )
                            pet.SummonMaster = p;

                        pet.ControlTarget = p;
                        pet.ControlOrder = OrderType.Follow;

                        pet.IsStabled = false;

                        p.Stabled.RemoveAt( k );

                        OldShrinkItem si = new OldShrinkItem();
                        si.Filled = true;
                        si.PetHue = pet.Hue;
                        si.PetName = pet.Name;
                        si.PetControlled = pet.Controlled;
                        si.PetControlMasterName = p.Name;
                        si.PetBonded = pet.IsBonded;
                        si.PetTypeString = pet.GetType().Name;
                        si.IsPortingItem = true;
                        si.PortingOwner = p;

                        pets.DropItem( si );
                        pet.Delete(); // force new pet 

                        --k;
                    }
                    #endregion

                    PreAoSCharacterCreation.Dress( p );

                    p.MoveToWorld( StartLocation.Location, StartLocation.Map );

                    if( p is Midgard2PlayerMobile )
                        ( (Midgard2PlayerMobile)p ).QuestDeltaTimeExpiration = TimeSpan.FromDays( 15 );

                    if( p.BankBox.Items.Count > 0 )
                    {
                        Console.WriteLine( "Error unloading bank of {0}", p.Name );
                        Console.ReadKey();
                    }
                }

                locX++;
                if( locX >= 20 )
                {
                    locX = 0;
                    locY++;
                }

                itemsForAccount.Delete();
            }

            Console.WriteLine( "All accounts are town free. Their items and pets have been wiped." );
        }
        #endregion

        private static readonly CityInfo StartLocation = new CityInfo( "Cove", "Centro", 2230, 1224, 0, Map.Felucca );

        private static WoodenBox FillBox( string name, Container dropTo, int position )
        {
            WoodenBox wb = new WoodenBox();
            wb.ItemID = 0xE7D;
            wb.Name = name;

            wb.Location = new Point3D( 28 + position * 15, 51, 0 );
            wb.Movable = false;

            dropTo.DropItem( wb );

            return wb;
        }

        private class AccountComparer : IComparer<Account>
        {
            public static readonly IComparer<Account> Instance = new AccountComparer();

            public int Compare( Account x, Account y )
            {
                if( x == null && y == null )
                    return 0;
                else if( x == null )
                    return -1;
                else if( y == null )
                    return 1;

                return Insensitive.Compare( x.Username, y.Username );
            }
        }
    }
}