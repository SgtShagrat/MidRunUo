/***************************************************************************
 *                               WorldSaveProfiler.cs
 *
 *   begin                : 26 dicembre 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Server;

namespace Midgard
{
    public class WorldSaveProfiler
    {      
        private List<SaveProfileEntry> m_List = new List<SaveProfileEntry>();
        private Stopwatch m_Watch;
        private WorldSaveEventHandler m_Handler;

        private static readonly string LogPath = Path.Combine( "Profiles", "worldSaveProfiler.log" );

        #region Singleton pattern
        private static WorldSaveProfiler m_Instance;

        public static WorldSaveProfiler Instance
        {
            get { return m_Instance ?? ( m_Instance = new WorldSaveProfiler() ); }
        }
        #endregion

        #region constructors
        [Constructable]
        public WorldSaveProfiler()
        {
            if( m_Instance == null )
                m_Instance = this;

            ScriptCompiler.EnsureDirectory( "Profiles" );
        }
        #endregion

        public void StartMainProfile()
        {
            if( m_Watch == null )
                m_Watch = new Stopwatch();

            if( m_List == null )
                m_List = new List<SaveProfileEntry>();
            else
                m_List.Clear();
        }

        public void EndMainProfile()
        {
            if( m_Watch.IsRunning )
                m_Watch.Stop();

            int sum = 0;
            foreach( SaveProfileEntry entry in m_List )
                sum += entry.Duration;

            foreach( SaveProfileEntry entry in m_List )
                entry.Percentual = ( entry.Duration / (double) sum ) * 100;

            m_List.Sort( new EntrySorter() );

            using( StreamWriter op = new StreamWriter( LogPath, true ) )
            {
                op.WriteLine( "Profile: (Date {0} - Time {1} - Total duration {2}ms)", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), sum );

                foreach( SaveProfileEntry entry in m_List )
                    op.WriteLine( entry.ToString() );

                op.WriteLine( "" );
            }
        }

        public void StartHandlerProfile( WorldSaveEventHandler handler )
        {
            if( m_Watch.IsRunning )
                m_Watch.Reset();

            m_Handler = handler;
            m_Watch.Start();
        }

        public void EndHandlerProfile()
        {
            if( m_Watch.IsRunning )
                m_Watch.Stop();

            foreach( Delegate d in m_Handler.GetInvocationList() )
                m_List.Add( new SaveProfileEntry( string.Format( "{0}.{1}", d.Method.DeclaringType.FullName, d.Method.Name ), (int)m_Watch.ElapsedMilliseconds ) );

            m_Watch.Reset();
        }

        private class SaveProfileEntry
        {
            public string Name { get; private set; }
            public int Duration { get; private set; }
            public double Percentual { private get; set; }

            public SaveProfileEntry( string name, int duration )
            {
                Name = name;
                Duration = duration;
                Percentual = 0;
            }

            public override string ToString()
            {
                return string.Format( "{0} - {1} ({2:F2}%)", Name, Duration, Percentual );
            }
        }

        private class EntrySorter : IComparer<SaveProfileEntry>
        {
            public int Compare( SaveProfileEntry a, SaveProfileEntry b )
            {
                int v = -a.Duration.CompareTo( b.Duration );

                if( v == 0 )
                    v = a.Name.CompareTo( b.Name );

                return v;
            }
        }
    }
}