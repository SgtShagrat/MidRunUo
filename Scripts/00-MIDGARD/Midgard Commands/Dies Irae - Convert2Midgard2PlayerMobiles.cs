using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Commands
{
    public class ConvertPlayers2Midgard2Players
    {
        public static void Initialize()
        {
            CommandSystem.Register( "Convert2Midgard2Players", AccessLevel.Developer, new CommandEventHandler( Convert2Midgard2Players_OnCommand ) );
            CommandSystem.Register( "ListPMnonM2PM", AccessLevel.Developer, new CommandEventHandler( ListPMnonM2PM_OnCommand ) );
        }

        [Usage( "Convert2Midgard2Players" )]
        [Description( "Converts all players class instances to Midgard2PlayerMobile ones" )]
        public static void Convert2Midgard2Players_OnCommand( CommandEventArgs e )
        {
            e.Mobile.SendMessage( "Inizio della conversione dei PlayerMobile in Midgard2PlayerMobile. " +
                                  "Ora verrai disconnesso." );

            List<Mobile> mobs = new List<Mobile>( World.Mobiles.Values );

            // Counter dei pg da convertire
            int count = 0;

            using( StreamWriter tw = new StreamWriter( "Logs/LogConvertPlayers1.log", true ) )
                tw.WriteLine( "Inizio Conversione dei pg." );

            foreach( Mobile m in mobs )
            {
                if( m.Player && !( m is Midgard2PlayerMobile ) )
                {
                    using( StreamWriter tw = new StreamWriter( "Logs/LogConvertPlayers1.log", true ) )
                        tw.WriteLine( "Sto convertendo {0}", m.Name );

                    count++;

                    if( m.NetState != null )
                    {
                        m.NetState.Dispose();
                    }

                    Midgard2PlayerMobile m2Pm = new Midgard2PlayerMobile( m.Serial );

                    m2Pm.DefaultMobileInit();

                    List<Item> copy = new List<Item>( m.Items );
                    for( int i = 0; i < copy.Count; i++ )
                    {
                        m2Pm.AddItem( copy[ i ] );
                    }

                    #region props e skills
                    CopyPropsMob2Pm( m2Pm, m );
                    CopyPropsPm2M2Pm( m2Pm, m );

                    for( int i = 0; i < m.Skills.Length; i++ )
                    {
                        m2Pm.Skills[ i ].Base = m.Skills[ i ].Base;
                        m2Pm.Skills[ i ].SetLockNoRelay( m.Skills[ i ].Lock );
                        m2Pm.Skills[ i ].Cap = m.Skills[ i ].Cap;
                    }

                    for( int v = 0; v < Enum.GetNames( typeof( VirtueName ) ).Length; v++ )
                    {
                        m2Pm.Virtues.SetValue( v, m.Virtues.GetValue( v ) );
                    }
                    #endregion

                    #region slot e worldsave
                    World.Mobiles[ m.Serial ] = m2Pm;
                    #endregion
                }
            }

            if( count > 0 )
            {
                NetState.ProcessDisposedQueue();
                World.Save();
                Console.WriteLine( "{0} PlayerMobile sono stati convertiti in  Midgard2PlayerMobile.  Restarta il server.", count );
                Console.ReadLine();
            }
            else
            {
                e.Mobile.SendMessage( "Non e' stato trovato nessun PlayerMobile da convertire." );
            }
        }

        [Usage( "ListPMnonM2PM" )]
        [Description( "List all players that are not Midgard2PlayerMobile instances" )]
        public static void ListPMnonM2PM_OnCommand( CommandEventArgs e )
        {
            List<Mobile> mobs = new List<Mobile>( World.Mobiles.Values );

            foreach( Mobile m in mobs )
            {
                if( m.Player && !( m is Midgard2PlayerMobile ) )
                {
                    Console.WriteLine( "Il Player {0} nell'account {1} non e' stato ancora convertito a M2pm.", m.Name, m.Account );
                }
            }
        }

        private static void CopyPropsMob2Pm( Mobile to, Mobile from )
        {
            Type type = typeof( Mobile );

            PropertyInfo[] props = type.GetProperties( BindingFlags.Public | BindingFlags.Instance );

            for( int p = 0; p < props.Length; p++ )
            {
                PropertyInfo prop = props[ p ];

                if( prop.CanRead && prop.CanWrite )
                {
                    try
                    {
                        prop.SetValue( to, prop.GetValue( from, null ), null );
                    }
                    catch
                    {
                    }
                }
            }
        }

        private static void CopyPropsPm2M2Pm( Mobile to, Mobile from )
        {
            Type type = typeof( PlayerMobile );

            PropertyInfo[] props = type.GetProperties( BindingFlags.Public | BindingFlags.Instance );

            for( int p = 0; p < props.Length; p++ )
            {
                PropertyInfo prop = props[ p ];

                if( prop.CanRead && prop.CanWrite )
                {
                    try
                    {
                        prop.SetValue( to, prop.GetValue( from, null ), null );
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
