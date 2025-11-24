/***************************************************************************
 *                               MidgardLayerValidator.cs
 *
 *   begin                : 09 January, 2010
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Server;
using Server.Items;

namespace Midgard.Engines
{
    public class MidgardLayerValidator
    {
        private static bool Enabled = false;

        private static Dictionary<LayerError, int> m_Dict = new Dictionary<LayerError, int>();
        private static List<LayerError> m_List = new List<LayerError>();

        public static void Initialize()
        {
            if( !Enabled )
                return;

            ValidateMobiles();
            BuildReport();
            FixErrors();
        }

        private static void ValidateMobiles()
        {
            List<Mobile> toCheck = GetMobilesToCheck();

            foreach( Mobile mobile in toCheck )
            {
                VerifyLayersConfliction( mobile );
            }
        }

        private static List<Mobile> GetMobilesToCheck()
        {
            List<Mobile> mobilearray = null;
            ICollection mobilevalues = World.Mobiles.Values;
            lock( mobilevalues.SyncRoot )
            {
                try
                {
                    mobilearray = new List<Mobile>( World.Mobiles.Values );
                }
                catch( SystemException e )
                {
                    Console.WriteLine( "Unable to search World.Mobiles: " + e.Message );
                }
            }

            return mobilearray;
        }

        private static void VerifyLayersConfliction( Mobile mobile )
        {
            for( int i = 0; i < mobile.Items.Count; i++ )
            {
                Item item = mobile.Items[ i ];

                if( item == null || item.Deleted )
                    continue;

                LogItemConfliction( mobile, item );
            }
        }

        private static void LogItemConfliction( Mobile mobile, Item item )
        {
            for( int i = 0; i < mobile.Items.Count; ++i )
            {
                Item temp = mobile.Items[ i ];
                if( temp == item )
                    continue;

                if( temp.CheckConflictingLayer( mobile, item, item.Layer ) || item.CheckConflictingLayer( mobile, temp, temp.Layer ) )
                    RegisterLayerError( mobile, item, temp, item.Layer );
            }
        }

        private static void RegisterLayerError( Mobile mobile, Item firstItem, Item secondItem, Layer layer )
        {
            LayerError error = new LayerError( mobile, firstItem, secondItem, layer );
            m_List.Add( error );

            LayerError key;

            if( ContainsError( error, out key ) )
                m_Dict[ key ]++;
            else
                m_Dict[ error ] = 1;
        }

        private static bool ContainsError( LayerError error, out LayerError result )
        {
            foreach( KeyValuePair<LayerError, int> keyValuePair in m_Dict )
            {
                if( !keyValuePair.Key.Equals( error ) )
                    continue;

                result = keyValuePair.Key;
                return true;
            }

            result = null;
            return false;
        }

        private static void GetTotalErrors( out int unique, out int total )
        {
            unique = total = 0;

            foreach( KeyValuePair<LayerError, int> keyValuePair in m_Dict )
            {
                total += keyValuePair.Value;
                unique++;
            }
        }

        private static void BuildReport()
        {
            int total;
            int unique;
            GetTotalErrors( out unique, out total );

            // no report is required if no error is found
            if( total == 0 )
                return;

            using( StreamWriter op = new StreamWriter( "Logs/creature-layer-confliction-report.log", true ) )
            {
                op.WriteLine( "List generated on {0} in time {1}.", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString() );
                op.WriteLine( "Total mobile processed {0}", World.Mobiles.Values.Count );
                op.WriteLine( "Total unique errors found {0}", unique );
                op.WriteLine( "Total unique errors found {0} (with duplicates)", total );

                op.WriteLine( "" );

                foreach( KeyValuePair<LayerError, int> keyValuePair in m_Dict )
                {
                    LayerError error = keyValuePair.Key;
                    int count = keyValuePair.Value;

                    op.WriteLine( "Conflicting layer for creature: {0}", error.Mobile.GetType().Name );
                    op.WriteLine( "Total occurrances: {0}", count );
                    op.WriteLine( "Wrong Layer: {0}", error.WrongLayer );
                    op.WriteLine( "\tItem 1: {0}", error.FirstItem.GetType().Name );
                    op.WriteLine( "\tItem 2: {0}", error.SecondItem.GetType().Name );

                    op.WriteLine( "" );
                }
            }
        }

        private static void FixErrors()
        {
            for( int i = 0; i < m_List.Count; i++ )
            {
                LayerError error = m_List[ i ];
                error.Fix();
            }
        }

        #region Nested type: LayerError
        private class LayerError : IComparable<LayerError>
        {
            public LayerError( Mobile mobile, Item firstItem, Item secondItem, Layer wrongLayer )
            {
                Mobile = mobile;
                FirstItem = firstItem;
                SecondItem = secondItem;
                WrongLayer = wrongLayer;
            }

            public Mobile Mobile { get; private set; }
            public Item FirstItem { get; private set; }
            public Item SecondItem { get; private set; }
            public Layer WrongLayer { get; private set; }

            public override bool Equals( object obj )
            {
                if( obj == null || !( obj is LayerError ) )
                    return false;

                LayerError other = (LayerError)obj;

                if( other.Mobile.GetType() != Mobile.GetType() )
                    return false;

                if( other.WrongLayer != WrongLayer )
                    return false;

                if( ( FirstItem.GetType() == other.FirstItem.GetType() &&
                      SecondItem.GetType() == other.SecondItem.GetType() ) ||
                    ( FirstItem.GetType() == other.SecondItem.GetType() &&
                      SecondItem.GetType() == other.FirstItem.GetType() ) )
                    return true;

                return false;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public void Fix()
            {
                Mobile m = Mobile;
                if( m == null || m.Deleted )
                    return;

                Container pack = m.Backpack;

                bool hasPack = pack != null;

                using( StreamWriter op = new StreamWriter( "Logs/creature-layer-confliction-fixes.log", true ) )
                {
                    op.WriteLine( "Conflicting layer for creature: {0}", Mobile.GetType().Name );
                    op.WriteLine( "Wrong Layer: {0}", WrongLayer );

                    if( hasPack && FirstItem != pack )
                    {
                        op.WriteLine( "Item dropped to pack: {0}", FirstItem.GetType().Name );
                        pack.DropItem( FirstItem );
                    }
                    else
                    {
                        if( !FirstItem.Deleted )
                        {
                            op.WriteLine( "Item deleted: {0}", FirstItem.GetType().Name );
                            FirstItem.Delete();
                        }
                        else if( !SecondItem.Deleted )
                        {
                            op.WriteLine( "Item deleted: {0}", SecondItem.GetType().Name );
                            SecondItem.Delete();
                        }
                    }

                    op.WriteLine( "" );
                }
            }

            #region IComparable<LayerError> Members
            public int CompareTo( LayerError other )
            {
                if( other == null )
                    return -1;

                if( other.Mobile.GetType() != GetType() )
                    return Insensitive.Compare( other.GetType().Name, GetType().Name );

                if( other.WrongLayer == WrongLayer )
                {
                    if( FirstItem.GetType() == other.FirstItem.GetType() &&
                        SecondItem.GetType() == other.SecondItem.GetType() )
                        return 0;
                    else if( FirstItem.GetType() == other.SecondItem.GetType() &&
                             SecondItem.GetType() == other.FirstItem.GetType() )
                        return 0;
                    else
                        return Insensitive.Compare( other.FirstItem.GetType().Name, FirstItem.GetType().Name );
                }
                else
                {
                    if( other.WrongLayer > WrongLayer )
                        return 1;
                    else if( other.WrongLayer < WrongLayer )
                        return -1;
                }

                return -1;
            }
            #endregion
        }
        #endregion
    }
}