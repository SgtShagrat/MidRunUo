#define NEWPROPSGUMP
#define BOOKTEXTENTRY

using System;
using System.Collections.Generic;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    public class XmlSpawnerGump : Gump
    {
        private static int m_Nclicks;

        public const int MaxSpawnEntries = 60;
        private const int MaxEntriesPerPage = 15;

        private int m_InitialMaxcount;
        private int m_Page;
        private ReplacementEntry m_Rentry;

        public class ReplacementEntry
        {
            public string Typename;
            public int Index;
            public int Color;
        }

        public XmlSpawnerGump( XmlSpawner spawner, int x, int y, int extension, int textextension, int newpage )
            : this( spawner, x, y, extension, textextension, newpage, null )
        {
        }

        public XmlSpawnerGump( XmlSpawner spawner, int x, int y, int extension, int textextension, int newpage,
                              ReplacementEntry rentry )
            : base( x, y )
        {
            if( spawner == null || spawner.Deleted )
                return;

            Spawner = spawner;
            spawner.SpawnerGump = this;
            Xoffset = textextension;
            m_InitialMaxcount = spawner.MaxCount;
            Page = newpage;
            Rentry = rentry;

            AddPage( 0 );

            // automatically change the gump depending on whether sequential spawning and/or subgroups are activated

            if( spawner.SequentialSpawn >= 0 || spawner.HasSubGroups() || spawner.HasIndividualSpawnTimes() )
            {
                // show the fully extended gump with subgroups and reset timer info
                ShowGump = 2;
            }
            /*
        else
            if(spawner.HasSubGroups() || spawner.SequentialSpawn >= 0)
        {
            // show partially extended gump with subgroups
            m_ShowGump = 1;
        }
        */

            if( extension > 0 )
            {
                ShowGump = extension;
            }
            if( extension < 0 )
            {
                ShowGump = 0;
            }

            // if the expanded gump toggle has been activated then override the auto settings.


            if( ShowGump > 1 )
            {
                AddBackground( 0, 0, 670 + Xoffset + 30, 474, 5054 );
                AddAlphaRegion( 0, 0, 670 + Xoffset + 30, 474 );
            }
            else if( ShowGump > 0 )
            {
                AddBackground( 0, 0, 335 + Xoffset, 474, 5054 );
                AddAlphaRegion( 0, 0, 335 + Xoffset, 474 );
            }
            else
            {
                AddBackground( 0, 0, 305 + Xoffset, 474, 5054 );
                AddAlphaRegion( 0, 0, 305 + Xoffset, 474 );
            }

            // spawner name area
            AddImageTiled( 3, 5, 227, 23, 0x52 );
            AddImageTiled( 4, 6, 225, 21, 0xBBC );
            AddTextEntry( 6, 5, 222, 21, 0, 999, spawner.Name ); // changed from color 50

            AddButton( 5, 450, 0xFAE, 0xFAF, 4, GumpButtonType.Reply, 0 );
            AddLabel( 38, 450, 0x384, "Goto" );

            //AddButton( 5, 428, 0xFB7, 0xFB9, 1, GumpButtonType.Reply, 0 );
            AddButton( 5, 428, 0xFAE, 0xFAF, 1, GumpButtonType.Reply, 0 );
            AddLabel( 38, 428, 0x384, "Help" );

            AddButton( 80, 428, 0xFB4, 0xFB6, 2, GumpButtonType.Reply, 0 );
            AddLabel( 113, 428, 0x384, "Bring Home" );

            AddButton( 80, 450, 0xFA8, 0xFAA, 3, GumpButtonType.Reply, 0 );
            AddLabel( 113, 450, 0x384, "Respawn" );

            // Props button
            AddButton( 200, 428, 0xFAB, 0xFAD, 9999, GumpButtonType.Reply, 0 );
            AddLabel( 233, 428, 0x384, "Props" );

            // Sort button
            AddButton( 200, 450, 0xFAB, 0xFAD, 702, GumpButtonType.Reply, 0 );
            AddLabel( 233, 450, 0x384, "Sort" );

            // Reset button
            AddButton( 80, 406, 0xFA2, 0xFA3, 701, GumpButtonType.Reply, 0 );
            AddLabel( 113, 406, 0x384, "Reset" );

            // Refresh button
            AddButton( 200, 406, 0xFBD, 0xFBE, 9998, GumpButtonType.Reply, 0 );
            AddLabel( 233, 406, 0x384, "Refresh" );

            // add run status display
            if( Spawner.Running )
            {
                AddButton( 5, 399, 0x2A4E, 0x2A3A, 700, GumpButtonType.Reply, 0 );
                AddLabel( 38, 406, 0x384, "On" );
            }
            else
            {
                AddButton( 5, 399, 0x2A62, 0x2A3A, 700, GumpButtonType.Reply, 0 );
                AddLabel( 38, 406, 0x384, "Off" );
            }

            // Add sequential spawn state
            if( Spawner.SequentialSpawn >= 0 )
            {
                AddLabel( 15, 365, 33, String.Format( "{0}", Spawner.SequentialSpawn ) );
            }

            // Add Current / Max count labels
            AddLabel( 231 + Xoffset, 9, 68, "Count" );
            AddLabel( 270 + Xoffset, 9, 33, "Max" );

            if( ShowGump > 0 )
            {
                // Add subgroup label
                AddLabel( 334 + Xoffset, 9, 68, "Sub" );
            }
            if( ShowGump > 1 )
            {
                // Add entry field labels
                AddLabel( 303 + Xoffset, 9, 68, "Per" );
                AddLabel( 329 + Xoffset + 30, 9, 68, "Reset" );
                AddLabel( 368 + Xoffset + 30, 9, 68, "To" );
                AddLabel( 392 + Xoffset + 30, 9, 68, "Kills" );
                AddLabel( 432 + Xoffset + 30, 9, 68, "MinD" );
                AddLabel( 472 + Xoffset + 30, 9, 68, "MaxD" );
                AddLabel( 515 + Xoffset + 30, 9, 68, "Rng" );
                AddLabel( 545 + Xoffset + 30, 9, 68, "RK" );
                AddLabel( 565 + Xoffset + 30, 9, 68, "Clr" );
                AddLabel( 590 + Xoffset + 30, 9, 68, "NextSpawn" );
            }

            // add area for spawner max
            AddLabel( 180 + Xoffset, 365, 50, "Spawner" );
            AddImageTiled( 267 + Xoffset, 365, 35, 23, 0x52 );
            AddImageTiled( 268 + Xoffset, 365, 32, 21, 0xBBC );
            AddTextEntry( 273 + Xoffset, 365, 33, 33, 33, 300, Spawner.MaxCount.ToString() );

            // add area for spawner count
            AddImageTiled( 231 + Xoffset, 365, 35, 23, 0x52 );
            AddImageTiled( 232 + Xoffset, 365, 32, 21, 0xBBC );
            AddLabel( 233 + Xoffset, 365, 68, Spawner.CurrentCount.ToString() );

            // add the status string
            AddTextEntry( 38, 384, 235, 33, 33, 900, Spawner.status_str );
            // add the page buttons
            for( int i = 0; i < ( MaxSpawnEntries / MaxEntriesPerPage ); i++ )
            {
                //AddButton( 38+i*30, 365, 2206, 2206, 0, GumpButtonType.Page, 1+i );
                AddButton( 38 + i * 25, 365, 0x8B1 + i, 0x8B1 + i, 4000 + i, GumpButtonType.Reply, 0 );
            }

            // add gump extension button
            if( ShowGump > 1 )
                AddButton( 645 + Xoffset + 30, 450, 0x15E3, 0x15E7, 200, GumpButtonType.Reply, 0 );
            else if( ShowGump > 0 )
                AddButton( 315 + Xoffset, 450, 0x15E1, 0x15E5, 200, GumpButtonType.Reply, 0 );
            else
                AddButton( 285 + Xoffset, 450, 0x15E1, 0x15E5, 200, GumpButtonType.Reply, 0 );

            // add the textentry extender button
            if( Xoffset > 0 )
            {
                AddButton( 160, 365, 0x15E3, 0x15E7, 201, GumpButtonType.Reply, 0 );
            }
            else
            {
                AddButton( 160, 365, 0x15E1, 0x15E5, 201, GumpButtonType.Reply, 0 );
            }


            for( int i = 0; i < MaxSpawnEntries; i++ )
            {
                if( Page != ( i / MaxEntriesPerPage ) )
                    continue;

                string str = String.Empty;
                int texthue = 0;
                int background = 0xBBC;

                if( i % MaxEntriesPerPage == 0 )
                {
                    //AddPage(page+1);
                    // add highlighted page button
                    AddImageTiled( 35 + Page * 25, 363, 25, 25, 0xBBC );
                    AddImage( 38 + Page * 25, 365, 0x8B1 + Page );
                }

                if( i < Spawner.SpawnObjects.Length )
                {
                    // disable button

                    if( Spawner.SpawnObjects[ i ].Disabled )
                    {
                        // change the background for the spawn text entry if disabled
                        background = 0x23F4;
                        AddButton( 2, 22 * ( i % MaxEntriesPerPage ) + 34, 0x82C, 0x82C, 6000 + i, GumpButtonType.Reply, 0 );
                    }
                    else
                    {
                        AddButton( 2, 22 * ( i % MaxEntriesPerPage ) + 36, 0x837, 0x837, 6000 + i, GumpButtonType.Reply, 0 );
                    }
                }

                bool hasreplacement = false;

                // check for replacement entries
                if( Rentry != null && Rentry.Index == i )
                {
                    hasreplacement = true;
                    str = Rentry.Typename;
                    background = Rentry.Color;
                    // replacement is one time only.
                    Rentry = null;
                }


                // increment/decrement buttons
                AddButton( 15, 22 * ( i % MaxEntriesPerPage ) + 34, 0x15E0, 0x15E4, 6 + ( i * 2 ), GumpButtonType.Reply, 0 );
                AddButton( 30, 22 * ( i % MaxEntriesPerPage ) + 34, 0x15E2, 0x15E6, 7 + ( i * 2 ), GumpButtonType.Reply, 0 );

                // categorization gump button
                AddButton( 171 + Xoffset - 18, 22 * ( i % MaxEntriesPerPage ) + 34, 0x15E1, 0x15E5, 5000 + i,
                          GumpButtonType.Reply, 0 );

                // goto spawn button
                AddButton( 171 + Xoffset, 22 * ( i % MaxEntriesPerPage ) + 30, 0xFAE, 0xFAF, 1300 + i, GumpButtonType.Reply, 0 );

                // text entry gump button
                AddButton( 200 + Xoffset, 22 * ( i % MaxEntriesPerPage ) + 30, 0xFAB, 0xFAD, 800 + i, GumpButtonType.Reply, 0 );

                // background for text entry area
                AddImageTiled( 48, 22 * ( i % MaxEntriesPerPage ) + 30, 133 + Xoffset - 25, 23, 0x52 );
                AddImageTiled( 49, 22 * ( i % MaxEntriesPerPage ) + 31, 131 + Xoffset - 25, 21, background );

                // Add page number
                //AddLabel( 15, 365, 33, String.Format("{0}",(int)(i/MaxEntriesPerPage + 1)) );
                //AddButton( 38+page*25, 365, 0x8B1+i, 0x8B1+i, 0, GumpButtonType.Page, 1+i );


                if( i < Spawner.SpawnObjects.Length )
                {
                    if( !hasreplacement )
                    {
                        str = Spawner.SpawnObjects[ i ].TypeName;
                    }

                    int count = Spawner.SpawnObjects[ i ].SpawnedObjects.Count;
                    int max = Spawner.SpawnObjects[ i ].ActualMaxCount;
                    int subgrp = Spawner.SpawnObjects[ i ].SubGroup;
                    int spawnsper = Spawner.SpawnObjects[ i ].SpawnsPerTick;

                    texthue = subgrp * 11;
                    if( texthue < 0 )
                        texthue = 0;

                    // Add current count
                    AddImageTiled( 231 + Xoffset, 22 * ( i % MaxEntriesPerPage ) + 30, 35, 23, 0x52 );
                    AddImageTiled( 232 + Xoffset, 22 * ( i % MaxEntriesPerPage ) + 31, 32, 21, 0xBBC );
                    AddLabel( 233 + Xoffset, 22 * ( i % MaxEntriesPerPage ) + 30, 68, count.ToString() );

                    // Add maximum count
                    AddImageTiled( 267 + Xoffset, 22 * ( i % MaxEntriesPerPage ) + 30, 35, 23, 0x52 );
                    AddImageTiled( 268 + Xoffset, 22 * ( i % MaxEntriesPerPage ) + 31, 32, 21, 0xBBC );
                    // AddTextEntry(x,y,w,ht,color,id,str)
                    AddTextEntry( 270 + Xoffset, 22 * ( i % MaxEntriesPerPage ) + 30, 30, 30, 33, 500 + i, max.ToString() );


                    if( ShowGump > 0 )
                    {
                        // Add subgroup
                        AddImageTiled( 334 + Xoffset, 22 * ( i % MaxEntriesPerPage ) + 30, 25, 23, 0x52 );
                        AddImageTiled( 335 + Xoffset, 22 * ( i % MaxEntriesPerPage ) + 31, 22, 21, 0xBBC );
                        AddTextEntry( 338 + Xoffset, 22 * ( i % MaxEntriesPerPage ) + 30, 17, 33, texthue, 600 + i,
                                     subgrp.ToString() );
                    }
                    if( ShowGump > 1 )
                    {
                        // Add subgroup timer fields

                        string strrst = null;
                        string strto = null;
                        string strkill = null;
                        string strmind = null;
                        string strmaxd = null;
                        string strnext = null;
                        string strpackrange = null;
                        string strspawnsper = spawnsper.ToString();

                        if( Spawner.SpawnObjects[ i ].SequentialResetTime > 0 && Spawner.SpawnObjects[ i ].SubGroup > 0 )
                        {
                            strrst = Spawner.SpawnObjects[ i ].SequentialResetTime.ToString();
                            strto = Spawner.SpawnObjects[ i ].SequentialResetTo.ToString();
                        }
                        if( Spawner.SpawnObjects[ i ].KillsNeeded > 0 )
                        {
                            strkill = Spawner.SpawnObjects[ i ].KillsNeeded.ToString();
                        }

                        if( Spawner.SpawnObjects[ i ].MinDelay >= 0 )
                        {
                            strmind = Spawner.SpawnObjects[ i ].MinDelay.ToString();
                        }

                        if( Spawner.SpawnObjects[ i ].MaxDelay >= 0 )
                        {
                            strmaxd = Spawner.SpawnObjects[ i ].MaxDelay.ToString();
                        }

                        if( Spawner.SpawnObjects[ i ].PackRange >= 0 )
                        {
                            strpackrange = Spawner.SpawnObjects[ i ].PackRange.ToString();
                        }

                        if( Spawner.SpawnObjects[ i ].NextSpawn > DateTime.Now )
                        {
                            // if the next spawn tick of the spawner will occur after the subgroup is available for spawning
                            // then report the next spawn tick since that is the earliest that the subgroup can actually be spawned
                            if( ( DateTime.Now + Spawner.NextSpawn ) > Spawner.SpawnObjects[ i ].NextSpawn )
                            {
                                strnext = Spawner.NextSpawn.ToString();
                            }
                            else
                            {
                                // estimate the earliest the next spawn could occur as the first spawn tick after reaching the subgroup nextspawn 
                                strnext =
                                    ( Spawner.SpawnObjects[ i ].NextSpawn - DateTime.Now + Spawner.NextSpawn ).ToString();
                            }
                        }
                        else
                        {
                            strnext = Spawner.NextSpawn.ToString();
                        }

                        int yoff = 22 * ( i % MaxEntriesPerPage ) + 30;

                        // spawns per tick
                        AddImageTiled( 303 + Xoffset, yoff, 30, 23, 0x52 );
                        AddImageTiled( 304 + Xoffset, yoff + 1, 27, 21, 0xBBC );
                        AddTextEntry( 307 + Xoffset, yoff, 22, 33, texthue, 1500 + i, strspawnsper );
                        // reset time
                        AddImageTiled( 329 + Xoffset + 30, yoff, 35, 23, 0x52 );
                        AddImageTiled( 330 + Xoffset + 30, yoff + 1, 32, 21, 0xBBC );
                        AddTextEntry( 333 + Xoffset + 30, yoff, 27, 33, texthue, 1000 + i, strrst );
                        // reset to
                        AddImageTiled( 365 + Xoffset + 30, yoff, 26, 23, 0x52 );
                        AddImageTiled( 366 + Xoffset + 30, yoff + 1, 23, 21, 0xBBC );
                        AddTextEntry( 369 + Xoffset + 30, yoff, 18, 33, texthue, 1100 + i, strto );
                        // kills needed
                        AddImageTiled( 392 + Xoffset + 30, yoff, 35, 23, 0x52 );
                        AddImageTiled( 393 + Xoffset + 30, yoff + 1, 32, 21, 0xBBC );
                        AddTextEntry( 396 + Xoffset + 30, yoff, 27, 33, texthue, 1200 + i, strkill );

                        // mindelay
                        AddImageTiled( 428 + Xoffset + 30, yoff, 41, 23, 0x52 );
                        AddImageTiled( 429 + Xoffset + 30, yoff + 1, 38, 21, 0xBBC );
                        AddTextEntry( 432 + Xoffset + 30, yoff, 33, 33, texthue, 1300 + i, strmind );

                        // maxdelay
                        AddImageTiled( 470 + Xoffset + 30, yoff, 41, 23, 0x52 );
                        AddImageTiled( 471 + Xoffset + 30, yoff + 1, 38, 21, 0xBBC );
                        AddTextEntry( 474 + Xoffset + 30, yoff, 33, 33, texthue, 1400 + i, strmaxd );

                        // packrange
                        AddImageTiled( 512 + Xoffset + 30, yoff, 33, 23, 0x52 );
                        AddImageTiled( 513 + Xoffset + 30, yoff + 1, 30, 21, 0xBBC );
                        AddTextEntry( 516 + Xoffset + 30, yoff, 25, 33, texthue, 1600 + i, strpackrange );

                        if( Spawner.SequentialSpawn >= 0 )
                        {
                            // restrict kills button
                            AddButton( 545 + Xoffset + 30, yoff,
                                      Spawner.SpawnObjects[ i ].RestrictKillsToSubgroup ? 0xD3 : 0xD2,
                                      Spawner.SpawnObjects[ i ].RestrictKillsToSubgroup ? 0xD2 : 0xD3, 300 + i,
                                      GumpButtonType.Reply, 0 );

                            //clear on advance button for spawn entries in subgroups that require kills
                            AddButton( 565 + Xoffset + 30, yoff, Spawner.SpawnObjects[ i ].ClearOnAdvance ? 0xD3 : 0xD2,
                                      Spawner.SpawnObjects[ i ].ClearOnAdvance ? 0xD2 : 0xD3, 400 + i,
                                      GumpButtonType.Reply, 0 );
                        }

                        // add the next spawn time
                        AddLabelCropped( 590 + Xoffset + 30, yoff, 70, 20, 55, strnext );
                    }

                    //AddButton( 20, 22 * (i%MaxEntriesPerPage) + 34, 0x15E3, 0x15E7, 5 + (i * 2), GumpButtonType.Reply, 0 );
                }
                // the spawn specification text
                //if(str != null)
                AddTextEntry( 52, 22 * ( i % MaxEntriesPerPage ) + 31, 119 + Xoffset - 25, 21, texthue, i, str );
            }
        }

        public int ShowGump { get; private set; }

        public int Xoffset { get; private set; }

        public int Page
        {
            get { return m_Page; }
            set { m_Page = value; }
        }

        public XmlSpawner Spawner { get; private set; }

        public ReplacementEntry Rentry
        {
            get { return m_Rentry; }
            set { m_Rentry = value; }
        }

        public SpawnObject[] CreateArray( RelayInfo info, Mobile from )
        {
            var spawnObjects = new List<SpawnObject>();

            for( int i = 0; i < MaxSpawnEntries; i++ )
            {
                TextRelay te = info.GetTextEntry( i );

                if( te != null )
                {
                    string str = te.Text;

                    if( str.Length > 0 )
                    {
                        str = str.Trim();
#if(BOOKTEXTENTRY)
                        if( i < Spawner.SpawnObjects.Length )
                        {
                            string currenttext = Spawner.SpawnObjects[ i ].TypeName;
                            if( currenttext != null && currenttext.Length >= 230 )
                            {
                                str = currenttext;
                            }
                        }
#endif
                        string typestr = BaseXmlSpawner.ParseObjectType( str );

                        Type type = null;
                        if( typestr != null )
                        {
                            try
                            {
                                type = SpawnerType.GetType( typestr );
                            }
                            catch
                            {
                            }
                        }

                        if( type != null )
                            spawnObjects.Add( new SpawnObject( from, Spawner, str, 0 ) );
                        else
                        {
                            // check for special keywords
                            if( typestr != null &&
                                ( BaseXmlSpawner.IsTypeOrItemKeyword( typestr ) || typestr.IndexOf( "{" ) != -1 ||
                                 typestr.StartsWith( "*" ) || typestr.StartsWith( "#" ) ) )
                            {
                                spawnObjects.Add( new SpawnObject( from, Spawner, str, 0 ) );
                            }
                            else
                                Spawner.status_str = String.Format( "{0} is not a valid type name.", str );
                            //from.SendMessage( "{0} is not a valid type name.", str );
                        }
                    }
                }
            }

            return spawnObjects.ToArray();
        }

        public void UpdateTypeNames( Mobile from, RelayInfo info )
        {
            for( int i = 0; i < MaxSpawnEntries; i++ )
            {
                TextRelay te = info.GetTextEntry( i );

                if( te != null )
                {
                    string str = te.Text;

                    if( str.Length > 0 )
                    {
                        str = str.Trim();
                        if( i < Spawner.SpawnObjects.Length )
                        {
                            // check to see if the existing typename is longer than the max textentry buffer
                            // if it is then dont update it since we will assume that the textentry has truncated the actual string
                            // that could be longer than the buffer if booktextentry is used

#if(BOOKTEXTENTRY)
                            string currentstr = Spawner.SpawnObjects[ i ].TypeName;
                            if( currentstr != null && currentstr.Length < 230 )
#endif
                            {
                                if( Spawner.SpawnObjects[ i ].TypeName != str )
                                {
                                    CommandLogging.WriteLine( from,
                                                             "{0} {1} changed XmlSpawner {2} '{3}' [{4}, {5}] ({6}) : {7} to {8}",
                                                             from.AccessLevel, CommandLogging.Format( from ),
                                                             Spawner.Serial, Spawner.Name,
                                                             Spawner.GetWorldLocation().X,
                                                             Spawner.GetWorldLocation().Y, Spawner.Map,
                                                             Spawner.SpawnObjects[ i ].TypeName, str );
                                }

                                Spawner.SpawnObjects[ i ].TypeName = str;
                            }
                        }
                    }
                }
            }
        }


#if(BOOKTEXTENTRY)

        public static void ProcessSpawnerBookEntry( Mobile from, object[] args, string entry )
        {
            if( from == null || args == null || args.Length < 6 )
                return;

            var spawner = (XmlSpawner)args[ 0 ];
            var index = (int)args[ 1 ];
            //int x = (int)args[ 2 ];
            //int y = (int)args[ 3 ];
            //int extension = (int)args[ 4 ];
            //int page = (int)args[ 5 ];

            if( spawner == null || spawner.SpawnObjects == null )
                return;

            // place the book text into the spawn entry
            if( index < spawner.SpawnObjects.Length )
            {
                SpawnObject so = spawner.SpawnObjects[ index ];

                if( so.TypeName != entry )
                {
                    CommandLogging.WriteLine( from, "{0} {1} changed XmlSpawner {2} '{3}' [{4}, {5}] ({6}) : {7} to {8}",
                                             from.AccessLevel, CommandLogging.Format( from ), spawner.Serial,
                                             spawner.Name, spawner.GetWorldLocation().X,
                                             spawner.GetWorldLocation().Y, spawner.Map, so.TypeName, entry );
                }

                so.TypeName = entry;
            }
            else
            {
                // add a new spawn entry
                spawner.m_SpawnObjects.Add( new SpawnObject( from, spawner, entry, 1 ) );


                index = spawner.SpawnObjects.Length - 1;
                // and bump the maxcount of the spawner
                spawner.MaxCount++;
            }

            // refresh the spawner gumps			
            RefreshSpawnerGumps( from );

            // and refresh the current one
            //from.SendGump( new XmlSpawnerGump(m_Spawner, m_X, m_Y, m_Extension,0, m_page) );

            // return the text entry focus to the book.  Havent figured out how to do that yet.
        }
#endif

        public static void Refresh_Callback( object state )
        {
            var args = (object[])state;
            var m = (Mobile)args[ 0 ];
            // refresh the spawner gumps			
            RefreshSpawnerGumps( m );
        }

        public static void RefreshSpawnerGumps( Mobile from )
        {
            if( from == null )
                return;

            NetState ns = from.NetState;

            if( ns != null && ns.Gumps != null )
            {
                var refresh = new List<XmlSpawnerGump>();

                foreach( Gump g in ns.Gumps )
                {
                    if( g is XmlSpawnerGump )
                    {
                        var xg = (XmlSpawnerGump)g;

                        // clear the gump status on the spawner associated with the gump
                        if( xg.Spawner != null )
                        {
                            // and add the old gump to the removal list
                            refresh.Add( xg );
                        }
                    }
                }

                // close all of the currently opened spawner gumps
                from.CloseGump( typeof( XmlSpawnerGump ) );

                // reopen the closed gumps from the gump collection
                foreach( XmlSpawnerGump g in refresh )
                {
                    // reopen a new gump for the spawner
                    if( g.Spawner != null /*&& g.m_Spawner.SpawnerGump == g */)
                    {
                        // flag the current gump on the spawner as closed
                        g.Spawner.GumpReset = true;

                        var xg = new XmlSpawnerGump( g.Spawner, g.X, g.Y, g.ShowGump, g.Xoffset, g.Page,
                                                               g.Rentry );

                        from.SendGump( xg );
                    }
                }
            }
        }

        private static bool ValidGotoObject( Mobile from, object o )
        {
            if( o is Item )
            {
                var i = o as Item;
                if( !i.Deleted && ( i.Map != null ) && ( i.Map != Map.Internal ) )
                    return true;

                if( from != null && !from.Deleted )
                {
                    from.SendMessage( "{0} is not available", i );
                }
            }
            else if( o is Mobile )
            {
                var m = o as Mobile;
                if( !m.Deleted && ( m.Map != null ) && ( m.Map != Map.Internal ) )
                    return true;

                if( from != null && !from.Deleted )
                {
                    from.SendMessage( "{0} is not available", m );
                }
            }

            return false;
        }

        public override void OnResponse( NetState state, RelayInfo info )
        {
            if( Spawner == null || Spawner.Deleted || state == null || info == null )
            {
                if( Spawner != null )
                    Spawner.SpawnerGump = null;
                return;
            }

            // restrict access to the original creator or someone of higher access level
            //if (m_Spawner.FirstModifiedBy != null && m_Spawner.FirstModifiedBy != state.Mobile && state.Mobile.AccessLevel <= m_Spawner.FirstModifiedBy.AccessLevel)
            //return;

            // Get the current name
            TextRelay tr = info.GetTextEntry( 999 );
            if( tr != null )
            {
                Spawner.Name = tr.Text;
            }

            // update typenames of the spawn objects based upon the current text entry strings
            UpdateTypeNames( state.Mobile, info );

            // Update the creature list
            Spawner.SpawnObjects = CreateArray( info, state.Mobile );

            if( Spawner.SpawnObjects == null )
            {
                Spawner.SpawnerGump = null;
                return;
            }

            for( int i = 0; i < Spawner.SpawnObjects.Length; i++ )
            {
                if( Page != ( i / MaxEntriesPerPage ) )
                    continue;

                // check the max count entry
                TextRelay temcnt = info.GetTextEntry( 500 + i );
                if( temcnt != null )
                {
                    int maxval = 0;
                    try
                    {
                        maxval = Convert.ToInt32( temcnt.Text, 10 );
                    }
                    catch
                    {
                    }
                    if( maxval < 0 )
                        maxval = 0;

                    Spawner.SpawnObjects[ i ].MaxCount = maxval;
                }

                if( ShowGump > 0 )
                {
                    // check the subgroup entry
                    TextRelay tegrp = info.GetTextEntry( 600 + i );
                    if( tegrp != null )
                    {
                        int grpval = 0;
                        try
                        {
                            grpval = Convert.ToInt32( tegrp.Text, 10 );
                        }
                        catch
                        {
                        }
                        if( grpval < 0 )
                            grpval = 0;

                        Spawner.SpawnObjects[ i ].SubGroup = grpval;
                    }
                }

                if( ShowGump > 1 )
                {
                    // note, while these values can be entered in any spawn entry, they will only be assigned to the subgroup leader
                    int subgroupindex = Spawner.GetCurrentSequentialSpawnIndex( Spawner.SpawnObjects[ i ].SubGroup );
                    TextRelay tegrp;

                    if( subgroupindex >= 0 && subgroupindex < Spawner.SpawnObjects.Length )
                    {
                        // check the reset time entry
                        tegrp = info.GetTextEntry( 1000 + i );
                        if( tegrp != null && !string.IsNullOrEmpty( tegrp.Text ) )
                        {
                            double grpval = 0;
                            try
                            {
                                grpval = Convert.ToDouble( tegrp.Text );
                            }
                            catch
                            {
                            }
                            if( grpval < 0 )
                                grpval = 0;

                            Spawner.SpawnObjects[ i ].SequentialResetTime = 0;

                            Spawner.SpawnObjects[ subgroupindex ].SequentialResetTime = grpval;
                        }
                        // check the reset to entry
                        tegrp = info.GetTextEntry( 1100 + i );
                        if( tegrp != null && !string.IsNullOrEmpty( tegrp.Text ) )
                        {
                            int grpval = 0;
                            try
                            {
                                grpval = Convert.ToInt32( tegrp.Text, 10 );
                            }
                            catch
                            {
                            }
                            if( grpval < 0 )
                                grpval = 0;

                            Spawner.SpawnObjects[ subgroupindex ].SequentialResetTo = grpval;
                        }
                        // check the kills entry
                        tegrp = info.GetTextEntry( 1200 + i );
                        if( tegrp != null && !string.IsNullOrEmpty( tegrp.Text ) )
                        {
                            int grpval = 0;
                            try
                            {
                                grpval = Convert.ToInt32( tegrp.Text, 10 );
                            }
                            catch
                            {
                            }
                            if( grpval < 0 )
                                grpval = 0;

                            Spawner.SpawnObjects[ subgroupindex ].KillsNeeded = grpval;
                        }
                    }

                    // check the mindelay
                    tegrp = info.GetTextEntry( 1300 + i );
                    if( tegrp != null )
                    {
                        if( !string.IsNullOrEmpty( tegrp.Text ) )
                        {
                            double grpval = -1;
                            try
                            {
                                grpval = Convert.ToDouble( tegrp.Text );
                            }
                            catch
                            {
                            }
                            if( grpval < 0 )
                                grpval = -1;

                            // if this value has changed, then update the next spawn time
                            if( grpval != Spawner.SpawnObjects[ i ].MinDelay )
                            {
                                Spawner.SpawnObjects[ i ].MinDelay = grpval;
                                Spawner.RefreshNextSpawnTime( Spawner.SpawnObjects[ i ] );
                            }
                        }
                        else
                        {
                            Spawner.SpawnObjects[ i ].MinDelay = -1;
                            Spawner.SpawnObjects[ i ].MaxDelay = -1;
                            Spawner.RefreshNextSpawnTime( Spawner.SpawnObjects[ i ] );
                        }
                    }

                    // check the maxdelay
                    tegrp = info.GetTextEntry( 1400 + i );
                    if( tegrp != null )
                    {
                        if( !string.IsNullOrEmpty( tegrp.Text ) )
                        {
                            double grpval = -1;
                            try
                            {
                                grpval = Convert.ToDouble( tegrp.Text );
                            }
                            catch
                            {
                            }
                            if( grpval < 0 )
                                grpval = -1;

                            // if this value has changed, then update the next spawn time
                            if( grpval != Spawner.SpawnObjects[ i ].MaxDelay )
                            {
                                Spawner.SpawnObjects[ i ].MaxDelay = grpval;
                                Spawner.RefreshNextSpawnTime( Spawner.SpawnObjects[ i ] );
                            }
                        }
                        else
                        {
                            Spawner.SpawnObjects[ i ].MinDelay = -1;
                            Spawner.SpawnObjects[ i ].MaxDelay = -1;
                            Spawner.RefreshNextSpawnTime( Spawner.SpawnObjects[ i ] );
                        }
                    }

                    // check the spawns per tick
                    tegrp = info.GetTextEntry( 1500 + i );
                    if( tegrp != null )
                    {
                        if( !string.IsNullOrEmpty( tegrp.Text ) )
                        {
                            int grpval = 1;
                            try
                            {
                                grpval = int.Parse( tegrp.Text );
                            }
                            catch
                            {
                            }
                            if( grpval < 0 )
                                grpval = 1;

                            // if this value has changed, then update the next spawn time
                            if( grpval != Spawner.SpawnObjects[ i ].SpawnsPerTick )
                            {
                                Spawner.SpawnObjects[ i ].SpawnsPerTick = grpval;
                            }
                        }
                        else
                        {
                            Spawner.SpawnObjects[ i ].SpawnsPerTick = 1;
                        }
                    }

                    // check the packrange
                    tegrp = info.GetTextEntry( 1600 + i );
                    if( tegrp != null )
                    {
                        if( !string.IsNullOrEmpty( tegrp.Text ) )
                        {
                            int grpval = 1;
                            try
                            {
                                grpval = int.Parse( tegrp.Text );
                            }
                            catch
                            {
                            }
                            if( grpval < 0 )
                                grpval = 1;

                            // if this value has changed, then update 
                            if( grpval != Spawner.SpawnObjects[ i ].PackRange )
                            {
                                Spawner.SpawnObjects[ i ].PackRange = grpval;
                            }
                        }
                        else
                        {
                            Spawner.SpawnObjects[ i ].PackRange = -1;
                        }
                    }
                }
            }

            // Update the maxcount
            TextRelay temax = info.GetTextEntry( 300 );
            if( temax != null )
            {
                int maxval = 0;
                try
                {
                    maxval = Convert.ToInt32( temax.Text, 10 );
                }
                catch
                {
                }
                if( maxval < 0 )
                    maxval = 0;

                // if the maxcount of the spawner has been altered external to the interface (e.g. via props, or by the running spawner itself
                // then that change will override the text entry
                if( Spawner.MaxCount == m_InitialMaxcount )
                {
                    Spawner.MaxCount = maxval;
                }
            }

            switch( info.ButtonID )
            {
                case 0: // Close
                    {
                        // clear any text entry books
                        Spawner.DeleteTextEntryBook();
                        // and reset the gump status
                        Spawner.GumpReset = true;

                        return;
                    }
                case 1: // Help
                    {
                        //state.Mobile.SendGump( new XmlSpawnerGump(m_Spawner, this.X, this.Y, this.m_ShowGump));
                        state.Mobile.SendGump( new HelpGump( Spawner, X + 290, Y ) );
                        break;
                    }
                case 2: // Bring everything home
                    {
                        Spawner.BringToHome();
                        break;
                    }
                case 3: // Complete respawn
                    {
                        Spawner.Respawn();
                        //m_Spawner.AdvanceSequential();
                        Spawner.m_killcount = 0;
                        break;
                    }
                case 4: // Goto
                    {
                        state.Mobile.Location = Spawner.Location;
                        state.Mobile.Map = Spawner.Map;
                        break;
                    }
                case 200: // gump extension
                    {
                        if( ShowGump > 1 )
                            state.Mobile.SendGump( new XmlSpawnerGump( Spawner, X, Y, -1, Xoffset, Page ) );
                        else
                            state.Mobile.SendGump( new XmlSpawnerGump( Spawner, X, Y, ShowGump + 2, Xoffset, Page ) );
                        return;
                    }
                case 201: // gump text extension
                    {
                        if( Xoffset > 0 )
                            state.Mobile.SendGump( new XmlSpawnerGump( Spawner, X, Y, ShowGump, 0, Page ) );
                        else
                            state.Mobile.SendGump( new XmlSpawnerGump( Spawner, X, Y, ShowGump, 250, Page ) );
                        return;
                    }
                case 700: // Start/stop spawner
                    {
                        if( Spawner.Running )
                            Spawner.Running = false;
                        else
                            Spawner.Running = true;
                        break;
                    }
                case 701: // Complete reset
                    {
                        Spawner.Reset();
                        break;
                    }
                case 702: // Sort spawns
                    {
                        Spawner.SortSpawns();
                        break;
                    }
                case 900: // empty the status string
                    {
                        Spawner.status_str = "";
                        break;
                    }
                case 9998: // refresh the gump
                    {
                        state.Mobile.SendGump( new XmlSpawnerGump( Spawner, X, Y, ShowGump, Xoffset, Page ) );
                        return;
                    }
                case 9999:
                    {
                        // Show the props window for the spawner, as well as a new gump
                        state.Mobile.SendGump( new XmlSpawnerGump( Spawner, X, Y, ShowGump, Xoffset, Page ) );
#if(NEWPROPSGUMP)
                        state.Mobile.SendGump( new XmlPropertiesGump( state.Mobile, Spawner ) );
#else
                    state.Mobile.SendGump( new PropertiesGump( state.Mobile, m_Spawner ) );
#endif
                        return;
                    }
                default:
                    {
                        // check the restrict kills flag
                        if( info.ButtonID >= 300 && info.ButtonID < 300 + MaxSpawnEntries )
                        {
                            int index = info.ButtonID - 300;
                            if( index < Spawner.SpawnObjects.Length )
                                Spawner.SpawnObjects[ index ].RestrictKillsToSubgroup =
                                    !Spawner.SpawnObjects[ index ].RestrictKillsToSubgroup;
                        }
                        else
                            // check the clear on advance flag
                            if( info.ButtonID >= 400 && info.ButtonID < 400 + MaxSpawnEntries )
                            {
                                int index = info.ButtonID - 400;
                                if( index < Spawner.SpawnObjects.Length )
                                    Spawner.SpawnObjects[ index ].ClearOnAdvance =
                                        !Spawner.SpawnObjects[ index ].ClearOnAdvance;
                            }
                            else
                                // text entry gump scroll buttons
                                if( info.ButtonID >= 800 && info.ButtonID < 800 + MaxSpawnEntries )
                                {
                                    // open the text entry gump
                                    int index = info.ButtonID - 800;
                                    // open a text entry gump
#if(BOOKTEXTENTRY)
                                    // display a new gump
                                    var newgump = new XmlSpawnerGump( Spawner, X, Y, ShowGump, Xoffset,
                                                                                Page );
                                    state.Mobile.SendGump( newgump );

                                    // is there an existing book associated with the gump?
                                    if( Spawner.m_TextEntryBook == null )
                                    {
                                        Spawner.m_TextEntryBook = new List<XmlTextEntryBook>();
                                    }

                                    var args = new object[ 6 ];

                                    args[ 0 ] = Spawner;
                                    args[ 1 ] = index;
                                    args[ 2 ] = X;
                                    args[ 3 ] = Y;
                                    args[ 4 ] = ShowGump;
                                    args[ 5 ] = Page;

                                    var book = new XmlTextEntryBook( 0, String.Empty, Spawner.Name, 20,
                                                                                 true,
                                                                                 new XmlTextEntryBookCallback(
                                                                                     ProcessSpawnerBookEntry ), args );

                                    Spawner.m_TextEntryBook.Add( book );

                                    book.Title = String.Format( "Entry {0}", index );
                                    book.Author = Spawner.Name;

                                    // fill the contents of the book with the current text entry data
                                    string text = String.Empty;
                                    if( Spawner.SpawnObjects != null && index < Spawner.SpawnObjects.Length )
                                    {
                                        text = Spawner.SpawnObjects[ index ].TypeName;
                                    }
                                    book.FillTextEntryBook( text );

                                    // put the book at the location of the player so that it can be opened, but drop it below visible range
                                    book.Visible = false;
                                    book.Movable = false;
                                    book.MoveToWorld(
                                        new Point3D( state.Mobile.Location.X, state.Mobile.Location.Y,
                                                    state.Mobile.Location.Z - 100 ), state.Mobile.Map );

                                    // and open it
                                    book.OnDoubleClick( state.Mobile );


#else
                            state.Mobile.SendGump( new TextEntryGump(m_Spawner,this, index, this.X, this.Y));
#endif
                                    return;
                                }
                                else
                                    // goto spawn buttons
                                    if( info.ButtonID >= 1300 && info.ButtonID < 1300 + MaxSpawnEntries )
                                    {
                                        m_Nclicks++;
                                        // find the location of the spawn at the specified index
                                        int index = info.ButtonID - 1300;
                                        if( index < Spawner.SpawnObjects.Length )
                                        {
                                            int scount = Spawner.SpawnObjects[ index ].SpawnedObjects.Count;
                                            if( scount > 0 )
                                            {
                                                object so =
                                                    Spawner.SpawnObjects[ index ].SpawnedObjects[ m_Nclicks % scount ];
                                                if( ValidGotoObject( state.Mobile, so ) )
                                                {
                                                    var o = so as IPoint3D;
                                                    if( o != null )
                                                    {
                                                        Map m = Spawner.Map;
                                                        if( o is Item )
                                                            m = ( (Item)o ).Map;
                                                        if( o is Mobile )
                                                            m = ( (Mobile)o ).Map;

                                                        state.Mobile.Location = new Point3D( o );
                                                        state.Mobile.Map = m;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                        // page buttons
                                        if( info.ButtonID >= 4000 &&
                                            info.ButtonID < 4001 + ( MaxSpawnEntries / MaxEntriesPerPage ) )
                                        {
                                            // which page
                                            Page = info.ButtonID - 4000;
                                        }
                                        else
                                            // toggle the disabled state of the entry
                                            if( info.ButtonID >= 6000 && info.ButtonID < 6000 + MaxSpawnEntries )
                                            {
                                                int index = info.ButtonID - 6000;

                                                if( index < Spawner.SpawnObjects.Length )
                                                {
                                                    Spawner.SpawnObjects[ index ].Disabled =
                                                        !Spawner.SpawnObjects[ index ].Disabled;

                                                    // clear any current spawns on the disabled entry
                                                    if( Spawner.SpawnObjects[ index ].Disabled )
                                                        Spawner.RemoveSpawnObjects( Spawner.SpawnObjects[ index ] );
                                                }
                                            }
                                            else if( info.ButtonID >= 5000 && info.ButtonID < 5000 + MaxSpawnEntries )
                                            {
                                                int i = info.ButtonID - 5000;


                                                string categorystring = null;
                                                string entrystring = null;

                                                TextRelay te = info.GetTextEntry( i );

                                                if( te != null && te.Text != null )
                                                {
                                                    // get the string

                                                    string[] cargs = te.Text.Split( ',' );

                                                    // parse out any comma separated args
                                                    categorystring = cargs[ 0 ];

                                                    entrystring = te.Text;
                                                }


                                                if( string.IsNullOrEmpty( categorystring ) )
                                                {
                                                    var newg = new XmlSpawnerGump( Spawner, X, Y, ShowGump,
                                                                                             Xoffset, Page );
                                                    state.Mobile.SendGump( newg );

                                                    // if no string has been entered then just use the full categorized add gump
                                                    state.Mobile.CloseGump( typeof( XmlCategorizedAddGump ) );
                                                    state.Mobile.SendGump( new XmlCategorizedAddGump( state.Mobile, i,
                                                                                                    newg ) );
                                                }
                                                else
                                                {
                                                    // use the XmlPartialCategorizedAddGump
                                                    state.Mobile.CloseGump( typeof( XmlPartialCategorizedAddGump ) );

                                                    //Type [] types = (Type[])XmlPartialCategorizedAddGump.Match( categorystring ).ToArray( typeof( Type ) );
                                                    List<SearchEntry> types = XmlPartialCategorizedAddGump.Match( categorystring );


                                                    var re = new ReplacementEntry();
                                                    re.Typename = entrystring;
                                                    re.Index = i;
                                                    re.Color = 0x1436;

                                                    var newg = new XmlSpawnerGump( Spawner, X, Y, ShowGump,
                                                                                             Xoffset, Page, re );

                                                    state.Mobile.SendGump( new XmlPartialCategorizedAddGump(
                                                                              state.Mobile, categorystring, 0, types,
                                                                              true, i, newg ) );

                                                    state.Mobile.SendGump( newg );
                                                }

                                                return;
                                            }
                                            else
                                            {
                                                // up and down arrows
                                                int buttonID = info.ButtonID - 6;
                                                int index = buttonID / 2;
                                                int type = buttonID % 2;

                                                TextRelay entry = info.GetTextEntry( index );

                                                if( entry != null && entry.Text.Length > 0 )
                                                {
                                                    string entrystr = entry.Text;

#if(BOOKTEXTENTRY)
                                                    if( index < Spawner.SpawnObjects.Length )
                                                    {
                                                        string str = Spawner.SpawnObjects[ index ].TypeName;

                                                        if( str != null && str.Length >= 230 )
                                                            entrystr = str;
                                                    }
#endif

                                                    if( type == 0 ) // Add creature
                                                    {
                                                        Spawner.AddSpawnObject( entrystr );
                                                    }
                                                    else // Remove creatures
                                                    {
                                                        Spawner.DeleteSpawnObject( state.Mobile, entrystr );
                                                    }
                                                }
                                            }
                        break;
                    }
            }
            // Create a new gump
            //m_Spawner.OnDoubleClick( state.Mobile);
            state.Mobile.SendGump( new XmlSpawnerGump( Spawner, X, Y, ShowGump, Xoffset, Page, Rentry ) );
        }
    }
}