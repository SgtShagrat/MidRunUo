/***************************************************************************
 *                               ListSerialsCommand
 *                            ------------------------
 *   begin                : lunedì 13 ottobre 2008
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
using Server.Commands;
using Server.Commands.Generic;
using Server.Items;

namespace Midgard.Commands
{
    public class ListSerialsCommand : BaseCommand
    {
        public ListSerialsCommand()
        {
            AccessLevel = AccessLevel.Developer;
            Supports = CommandSupport.AllItems;
            Commands = new string[] { "ListSerials" };
            ObjectTypes = ObjectTypes.Items;
            Usage = "ListSerials";
            Description = "List targeted items serial to output file. Generally used with condition arguments.";
            ListOptimized = true;
        }

        public override void ExecuteList( CommandEventArgs e, ArrayList list )
        {
            if( e.Arguments.Length != 0 )
                return;

            List<Container> packs = new List<Container>( list.Count );

            for( int i = 0; i < list.Count; ++i )
            {
                object obj = list[ i ];
                Container cont = null;

                if( obj is Container )
                    cont = (Container)obj;

                if( cont != null )
                    packs.Add( cont );
            }

            List<Serial> toLog = new List<Serial>();

            foreach( Container c in packs )
            {
                foreach( Item i in c.Items )
                {
                    if( !toLog.Contains( i.Serial ) )
                        toLog.Add( i.Serial );
                }
            }

            toLog.Sort();

            try
            {
                using( StreamWriter tw = new StreamWriter( e.GetString( 0 ), true ) )
                {
                    foreach( Serial s in toLog )
                    {
                        tw.WriteLine( s );
                    }
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }
    }
}