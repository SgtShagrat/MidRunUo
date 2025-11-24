/***************************************************************************
 *                                  MurderInfo.cs
 *                            		-------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Midgard.Engines.MurderInfo
{
    public class MurderInfo
    {
        public Mobile Killer { get; set; }

        public Mobile Victim { get; set; }

        public DateTime TimeOfDeath { get; set; }

        public MurderInfo( Mobile killer, Mobile victim, DateTime timeOfDeath )
        {
            Killer = killer;
            Victim = victim;
            TimeOfDeath = timeOfDeath;
        }

        public static void Initialize()
        {
            if( Config.MurderInfoEnabled && Config.CheckType == MurderInfoCheckTipe.AtKill )
                EventSink.Login += new LoginEventHandler( OnLogin );
        }

        private static void OnLogin( LoginEventArgs e )
        {
            Midgard2PlayerMobile m2pm = e.Mobile as Midgard2PlayerMobile;
            if( m2pm == null || m2pm.Deleted )
                return;

            if( m2pm.MurderInfoes == null )
                return;

            for( int i = 0; i < m2pm.MurderInfoes.Count; i++ )
            {
                MurderInfo mi = m2pm.MurderInfoes[ i ];

                if( mi != null && !MurderInfoHelper.IsValidInfo( mi ) )
                    MurderInfoPersistance.UnregisterInfoForKiller( m2pm, mi );
            }
        }

        /// <summary>
        /// Comparer per ordinare la lista di MurdersInfoes
        /// </summary>
        public class MurderInfoesComparer : IComparer<MurderInfo>
        {
            public static readonly IComparer<MurderInfo> Instance = new MurderInfoesComparer();

            public int Compare( MurderInfo x, MurderInfo y )
            {
                if( x == null || y == null )
                    throw new ArgumentException();

                if( x.TimeOfDeath < y.TimeOfDeath )
                    return -1;
                else if( x.TimeOfDeath > y.TimeOfDeath )
                    return 1;
                else
                    return 0;
            }
        }

        public MurderInfo()
        {
        }

        public MurderInfo( GenericReader reader )
        {
            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    {
                        Killer = reader.ReadMobile();
                        Victim = reader.ReadMobile();
                        TimeOfDeath = reader.ReadDateTime();
                        break;
                    }
            }
        }

        public void Serialize( GenericWriter writer )
        {
            writer.Write( 1 ); // version

            writer.WriteMobile( Killer );
            writer.WriteMobile( Victim );

            if( TimeOfDeath < DateTime.MaxValue && TimeOfDeath > DateTime.MinValue )
                writer.Write( (DateTime)TimeOfDeath );
            else
                writer.Write( DateTime.MinValue );
        }
    }
}