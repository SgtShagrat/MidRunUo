using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Midgard.Engines.BossSystem.WishSpectre;
using Midgard.Engines.Packager;
using Server;

namespace Midgard.Engines.BossSystem
{
    public class Core
    {
        private static readonly List<Dungeon> Dungeons = new List<Dungeon>();
		private static readonly Dictionary<string,List<Dungeon>> DungeonGroups = new Dictionary<string,List<Dungeon>>(StringComparer.InvariantCultureIgnoreCase);
        private static readonly string m_SavePath = Path.Combine( Path.Combine( "Saves", "BossSystem" ), "BossSystemSave.bin" );
        public static bool Debug = true;

        public static object[] Package_Info = {
                                                  "Script Title", "Boss System",
                                                  "Enabled by Default",
#if MAGIUSDEBUG
			true,
#else
			false,
#endif
                                                  "Script Version", new Version(1, 0, 0, 0),
                                                  "Author name", "Magius(CHE)",
                                                  "Creation Date", new DateTime(2009, 09, 05),
                                                  "Author mail-contact", "cheghe@tiscali.it",
                                                  "Author home site", "http://www.magius.it",
                                                  //"Author notes",           null,
                                                  "Script Copyrights", "(C) Midgard Shard - Magius(CHE",                                                  "Provided packages", new string[] {"Midgard.Engines.BossSystem"},
                                                  //"Required packages",      new string[0],
                                                  //"Conflicts with packages",new string[0],
                                                  "Research tags", new string[] {"Boss", "Engine"}
                                              };

        internal static Package Pkg;

        public static bool Enabled
        {
            get { return Pkg.Enabled; }
            set { Pkg.Enabled = value; }
        }

        /// <summary>
        /// All dungeons must me instantiated here!
        ///  Dungeon will be loaded from save and it update relavire m_Dungeons[] object.
        /// </summary>
        private static void CreateDungeons()
        {
            Dungeons.Add( new CustomDungeon() );					
        }
		
		private static void AfterLoadOrInitialization()
		{
			//assemble dungeons into groups
			DungeonGroups.Clear();
			foreach(var dun in Dungeons)
			{
				var grps = new List<string>();
				var attr = dun.GetType().GetCustomAttributes(typeof(DungeonGroupAttribute),false);
				if (attr != null && attr.Length>0)
				{
					foreach(DungeonGroupAttribute att in attr)
					{
						if ( ! grps.Contains( att.Group ))
							grps.Add( att.Group );
					}
				}
				else
					grps.Add(""); //uncategorized group.
				
				foreach(var g in grps)
				{
					if ( ! DungeonGroups.ContainsKey( g ) )
						DungeonGroups.Add( g , new List<Dungeon>() );
					if ( ! DungeonGroups[g].Contains( dun ) )
						DungeonGroups[g].Add( dun );
				}
				
				if (dun.Created) //need to inform dungeon 
				{
					if (!dun.ReCreateTimers())
						dun.Destroy();
				}
			}
			
			new DungeonTimer().Start();
		}
		
		#region Timer
		private class DungeonTimer : Timer
		{
			public DungeonTimer() : base(TimeSpan.Zero,TimeSpan.FromSeconds(1))
			{
			}
			protected override void OnTick ()
			{
				base.OnTick ();
				
				//each cycle.. check dungeon timer....
				foreach(var grp in DungeonGroups.Values)
				{
					//select not enabled dungeon
					var toCreate = new List<Dungeon>();
					var atleastcreated = false;
					foreach(var dun in grp)
					{
						if (dun.CanBeCreated)
							toCreate.Add(dun);
						else if (dun.Created)
						{
							atleastcreated=true;
							break;
						}
							
					}
					if (!atleastcreated) //no dungeon of this grup is created... create a new one
					{
						var rnd = toCreate[Utility.Random(toCreate.Count)];
						rnd.Create();
						rnd.Enabled = true; //enable and start dungeon system.
					}
				}
			}
		}
		#endregion

        #region Configure / Load / Save

        public static void Package_Configure()
        {
            Pkg = Packager.Core.Singleton[ typeof( Core ) ];
			
			Dungeons.Clear();
			
            EventSink.WorldLoad += new WorldLoadEventHandler( EventSink_WorldLoad );
						
			if (Pkg.Enabled)
			{
				EventSink.WorldSave += new WorldSaveEventHandler( EventSink_WorldSave );	
			}
        }

        private static void EventSink_WorldSave( WorldSaveEventArgs e )
        {
            WorldSaveProfiler.Instance.StartHandlerProfile( EventSink_WorldSave );

            if( !Directory.Exists( Path.GetDirectoryName( m_SavePath ) ) )
                Directory.CreateDirectory( Path.GetDirectoryName( m_SavePath ) );

            using( var writer = new BinaryWriter( new FileStream( m_SavePath, FileMode.Create, FileAccess.Write ), Encoding.UTF8 ) )
            {
                writer.Write( BossSystemPersistence.Singleton.SyncKey );
                writer.Write( (ushort) Dungeons.Count );
                foreach( var dun in Dungeons )
                {
                    //cache save.. to prevent crash on single dungeon
                    writer.Write( dun.GetType().FullName );
                    try
                    {
                        var singlewriter = new BinaryFileWriter ( new MemoryStream(), true );
                        try
                        {
                            //write here all dungeon datas
                            dun.Serialize( singlewriter );
                            //end write dun data.

                            singlewriter.Flush();

                            writer.Write( true ); //saved successfully.                            
                            var retbytes = ( (MemoryStream)singlewriter.UnderlyingStream ).ToArray();
                            writer.Write( (uint) retbytes.Length );
                            if( retbytes.Length > 0 )
                                writer.Write( retbytes, 0, retbytes.Length );
                        }
                        finally
                        {
                            singlewriter.UnderlyingStream.Dispose();
                        }
                    }
                    catch( Exception ex )
                    {
                        writer.Write( false ); //saved failed.
                        Pkg.LogErrorLine( "{0} failed to save due to {1}.", ex.GetType().Name, dun.Name );
                        Pkg.LogErrorLine( "{0}: {1}", ex.Message, ex.StackTrace );
                    }
                }
            }

            WorldSaveProfiler.Instance.EndHandlerProfile();
        }

        private static void EventSink_WorldLoad()
        {
			if (Pkg.Enabled)
			{
	            try
	            {
	                CreateDungeons();
	
	                foreach( var dun in Dungeons )
	                    dun.Configure();
	
	                if( File.Exists( m_SavePath ) )
	                {
	                    using( var reader = new BinaryReader( new FileStream( m_SavePath, FileMode.Open, FileAccess.Read ), Encoding.UTF8 ) )
	                    {
	                        var synckey = reader.ReadInt64();
	                        if( synckey != BossSystemPersistence.Singleton.SyncKey )
	                            throw new Exception( "Critical desyncronization." );
	
	                        var count = reader.ReadUInt16();
	                        var todeserialize = new List<Dungeon>( Dungeons );
	                        var removedones = new List<string>();
	                        var notsavedone = new List<Dungeon>();
	                        for( int h = 0; h < count; h++ )
	                        {
	                            var typename = reader.ReadString();
	                            var saved = reader.ReadBoolean();
	                            Dungeon existdun = null;
	                            foreach( var dun in todeserialize )
	                                if( dun.GetType().FullName == typename )
	                                {
	                                    existdun = dun;
	                                    break;
	                                }
	
	                            if( saved )
	                            {
	                                var toread = reader.ReadUInt32();
									if(toread>0)
									{
		                                var bread = reader.ReadBytes( (int) toread );
		                                var singlereader = new BinaryFileReader ( new BinaryReader( new MemoryStream( bread ) ) );
										try
		                                {
		                                    if( existdun != null )
		                                    {
		                                        todeserialize.Remove( existdun );
		                                        existdun.Deserialize( singlereader );
		                                    }
		                                    else
		                                    {
		                                        removedones.Add( typename );
		                                    }
		                                }
										finally
										{
											singlereader.Close();
										}
									}
	                            }
	                            else
	                                notsavedone.Add( existdun );
	                        }
	                        //Pkg.LogWarningLine("{0} will not loaded from save. It was crashed during previously save.");
	                        if( todeserialize.Count - notsavedone.Count > 0 )
	                            Pkg.LogWarningLine( "{0} dungeons will not loaded from save. They are new", todeserialize.Count - notsavedone.Count );
	                        if( notsavedone.Count > 0 )
	                            Pkg.LogWarningLine( "{0} dungeons will not loaded from save due to previously save error.", notsavedone.Count );
	                    }
	                }
	            }
	            catch( Exception ex )
	            {
	                Pkg.LogErrorLine( "{0}: {1} All dungeons will resetted.", ex.GetType().Name, ex.Message );
	                foreach( var dun in Dungeons )
	                    dun.ResetAsDefault();
	            }
			}
			
			PurifyQuestItems();
			
			if (Pkg.Enabled)
				AfterLoadOrInitialization();
        }

        public static void Package_Initialize()
        {
            Pkg.LogInfoLine("{0} Dungeons initialized.", Dungeons.Count);
            BossSystemPersistence.EnsureExistence();
        }
		
		private static void PurifyQuestItems()
		{
			var torem = new List<Item>();
			foreach(var item in World.Items)
			{
				if (item.Value is QuestItem)
				{
					if (((QuestItem)item.Value).Dungeon==null)
						torem.Add(item.Value);
				}
			}
			if (torem.Count>0)
			{
				foreach(var item in torem)
					item.Delete();
				Pkg.LogWarningLine( "Removed {0} orphan quest items.", torem.Count );
			}
		}

        #endregion

        public static Region GetRegion( string name )
        {
            foreach( var r in Region.Regions )
                if( !string.IsNullOrEmpty( name ) && name == r.Name )
                    return r;
            return null;
        }
    }
}