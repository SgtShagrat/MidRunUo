using System;
using System.Collections;
using System.Collections.Generic;

using Server;

namespace Midgard.Engines.AuctionSystem
{
    /// <summary>
    ///     Provides search methods for the auction system
    /// </summary>
    public class AuctionSearch
    {
        /// <summary>
        ///     Merges search results
        /// </summary>
        public static List<AuctionItem> Merge( List<AuctionItem> first, List<AuctionItem> second )
        {
            List<AuctionItem> result = new List<AuctionItem>( first );

            foreach( AuctionItem item in second )
            {
                if( !result.Contains( item ) )
                    result.Add( item );
            }

            return result;
        }

        /// <summary>
        ///     Performs a text search
        /// </summary>
        /// <param name = "items">The items to search</param>
        /// <param name = "text">The text search, names should be whitespace separated</param>
        public static List<AuctionItem> SearchForText( List<AuctionItem> items, string text )
        {
            string[] split = text.Split( ' ' );
            List<AuctionItem> result = new List<AuctionItem>();

            foreach( string s in split )
                result = Merge( result, TextSearch( items, s ) );

            return result;
        }

        /// <summary>
        ///     Performs a text search
        /// </summary>
        /// <param name = "list">The <see cref = "AuctionItem" /> objects to search</param>
        /// <param name = "name">The text to search for</param>
        private static List<AuctionItem> TextSearch( List<AuctionItem> list, string name )
        {
            List<AuctionItem> results = new List<AuctionItem>();

            if( list == null || name == null )
                return results;

            IEnumerator ie = null;

            try
            {
                name = name.ToLower();

                ie = list.GetEnumerator();

                while( ie.MoveNext() )
                {
                    AuctionItem item = ie.Current as AuctionItem;

                    if( item != null )
                    {
                        if( item.ItemName.ToLower().IndexOf( name ) > -1 )
                            results.Add( item );
                        else if( item.Description.ToLower().IndexOf( name ) > -1 )
                            results.Add( item );
                        else
                        {
                            // Search individual items
                            foreach( AuctionItem.ItemInfo info in item.Items )
                            {
                                if( info.Name.ToLower().IndexOf( name ) > -1 )
                                {
                                    results.Add( item );
                                    break;
                                }
                                else if( info.Properties.ToLower().IndexOf( name ) > .1 )
                                {
                                    results.Add( item );
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
            finally
            {
                IDisposable d = ie as IDisposable;

                if( d != null )
                    d.Dispose();
            }

            return results;
        }

        /// <summary>
        ///     Performs a search for types being auctioned
        /// </summary>
        /// <param name = "list">The items to search</param>
        /// <param name = "types">The list of types to find matches for</param>
        public static List<AuctionItem> ForTypes( List<AuctionItem> list, List<Type> types )
        {
            List<AuctionItem> results = new List<AuctionItem>();

            if( list == null || types == null )
                return results;

            IEnumerator ie = null;

            try
            {
                ie = list.GetEnumerator();

                while( ie.MoveNext() )
                {
                    AuctionItem item = ie.Current as AuctionItem;

                    if( item == null )
                        continue;

                    foreach( Type t in types )
                    {
                        if( MatchesType( item, t ) )
                        {
                            results.Add( item );
                            break;
                        }
                    }
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
            finally
            {
                IDisposable d = ie as IDisposable;

                if( d != null )
                    d.Dispose();
            }

            return results;
        }

        /// <summary>
        ///     Verifies if a specified type is a match to the items sold through an auction
        /// </summary>
        /// <param name = "item">The <see cref = "AuctionItem" /> being evaluated</param>
        /// <param name = "type">The type looking to match</param>
        /// <returns>True if there's a match</returns>
        private static bool MatchesType( AuctionItem item, Type type )
        {
            foreach( AuctionItem.ItemInfo info in item.Items )
            {
                if( info.AItem != null )
                {
                    if( type.IsAssignableFrom( info.AItem.GetType() ) )
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Performs a search for Artifacts by evaluating the ArtifactRarity property
        /// </summary>
        /// <param name = "items">The list of items to search</param>
        /// <returns>An <c>ArrayList</c> of results</returns>
        public static List<AuctionItem> ForArtifacts( List<AuctionItem> items )
        {
            List<AuctionItem> results = new List<AuctionItem>();

            foreach( AuctionItem auction in items )
            {
                foreach( AuctionItem.ItemInfo info in auction.Items )
                {
                    Item item = info.AItem;

                    if( Misc.IsArtifact( item ) )
                    {
                        results.Add( auction );
                        break;
                    }
                }
            }

            return results;
        }

        /// <summary>
        ///     Searches a list of auctions for ICommodities
        /// </summary>
        /// <param name = "items">The list to search</param>
        /// <returns>An <c>ArrayList</c> of results</returns>
        public static List<AuctionItem> ForCommodities( List<AuctionItem> items )
        {
            List<AuctionItem> results = new List<AuctionItem>();

            foreach( AuctionItem auction in items )
            {
                foreach( AuctionItem.ItemInfo info in auction.Items )
                {
                    if( info.AItem == null )
                        continue;

                    Type t = info.AItem.GetType();

                    if( t.GetInterface( "ICommodity" ) != null )
                    {
                        results.Add( auction );
                        break;
                    }
                }
            }

            return results;
        }
    }
}