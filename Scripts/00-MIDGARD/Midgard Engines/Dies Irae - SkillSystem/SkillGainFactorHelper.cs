/***************************************************************************
 *                               SkillGainFactorHelper.cs
 *
 *   begin                : 29 luglio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using Server;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Prompts;

namespace Midgard.Engines.SkillSystem
{
    public class SkillGainFactorHelper
    {
        private static readonly string m_FileNamePath = Path.Combine( "Saves/SkillSystem", "SkillGainFactors.xml" ); //Magius(CHE): writing datas must be into SAVES

        public static double GainFactorAtTwenty = 0.1;

        public static double GainFactorAtHundred = 0.001;

        public static double MinChanceToGain = 0.0025;

        internal static void RegisterCommmands()
        {
            CommandSystem.Register( "ConfigureGainFactors", AccessLevel.Administrator, new CommandEventHandler( ConfigureGainFactors_OnCommand ) );
        }

        public static double[] CustomTable;

        /// <summary>
        /// Returns a double value which is the custom modifier applied after GetMaxGainFactor check
        /// </summary>
        public static double GetCustomGainFactor( Mobile mobile, Skill skill )
        {
            if( CustomTable == null )
                return 1.00;

            if( !Config.Enabled ) //Mod by Magius(CHE)
                return 1.00;

            if( mobile is Midgard2PlayerMobile )
                return skill != null ? CustomTable[ skill.Info.SkillID ] : 1.00;

            return 1.00;
        }

        /// <summary>
        /// Returns a double value which is a linear interpolation between values at 20 and 100 in target skill.
        /// </summary>
        public static double GetMaxChanceToGain( double skill )
        {
            double delta = GainFactorAtHundred - GainFactorAtTwenty;

            return ( ( delta / 80.0 ) * skill ) - ( delta / 4.0 ) + GainFactorAtTwenty;
        }

        [Usage( "ConfigureGainFactors" )]
        [Description( "Configure Gain Factors." )]
        private static void ConfigureGainFactors_OnCommand( CommandEventArgs e )
        {
            e.Mobile.CloseGump( typeof( SkillGainFactorGump ) );
            e.Mobile.SendGump( new SkillGainFactorGump() );
        }

        private static void CreateDefaultFile()
        {
            ScriptCompiler.EnsureDirectory( Path.GetDirectoryName( m_FileNamePath ) );

            using( TextWriter streamWriter = new StreamWriter( m_FileNamePath ) )
            {
                streamWriter.WriteLine( "<SkillGainFactors>" );

                for( int i = 0; i < CustomTable.Length; i++ )
                    streamWriter.WriteLine( "\t<{0} value=\"{1}\" />", Enum.GetName( typeof( SkillName ), i ), CustomTable[ i ] );

                streamWriter.WriteLine( "</SkillGainFactors>" );
            }
        }

        internal static void LoadSettings()
        {
            InitTables();

            if( Config.Debug )
                Config.Pkg.LogInfoLine( "Skill system. Gain factor custom values loading..." );

            if( !File.Exists( m_FileNamePath ) )
                CreateDefaultFile();

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load( Path.Combine( Server.Core.BaseDirectory, m_FileNamePath ) );

                XmlElement root = doc[ "SkillGainFactors" ];
                if( root == null )
                    return;

                double val = -1.0;

                for( int i = 0; i < CustomTable.Length; i++ )
                {
                    ReadNode( root, Enum.GetName( typeof( SkillName ), i ), ref val );
                    if( val != 1.0 )
                    {
                        CustomTable[ i ] = val;

                        if( Config.Debug )
                            Config.Pkg.LogInfo( "Skill system: gain factor for {0} skill is set to {1:F2}.", Enum.GetName( typeof( SkillName ), i ), val );
                    }
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }

        private static void InitTables()
        {
            List<double> list = new List<double>();

            for( int i = 0; i < Enum.GetNames( typeof( SkillName ) ).Length; i++ )
                list.Add( 1.00 );

            CustomTable = list.ToArray();

            if( Config.Debug )
                Config.Pkg.LogInfo( "Skill system. Gain factor table initialized." );
        }

        internal static void SaveSettings()
        {
            if( Config.Debug )
                Config.Pkg.LogInfo( "Skill system. Gain factor custom values saving..." );

            ScriptCompiler.EnsureDirectory( Path.GetDirectoryName( m_FileNamePath ) );

            if( !File.Exists( m_FileNamePath ) )
                CreateDefaultFile();

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load( m_FileNamePath );

                XmlElement root = doc[ "SkillGainFactors" ];

                if( root == null )
                    return;

                for( int i = 0; i < CustomTable.Length; i++ )
                {
                    UpdateNode( root, Enum.GetName( typeof( SkillName ), i ), CustomTable[ i ] );
                }

                doc.Save( m_FileNamePath );
            }
            catch( Exception ex )
            {
                Console.WriteLine( "Error while updating SkillGainFactors: {0}", ex );
            }
        }

        private static void ReadNode( XmlElement root, string skillname, ref double val )
        {
            if( root == null )
                return;

            XmlNodeList childs = root.SelectNodes( skillname );
            if( childs == null || childs.Count == 0 )
                return;

            foreach( XmlElement element in childs )
            {
                if( element.HasAttribute( "value" ) )
                    val = XmlConvert.ToDouble( element.GetAttribute( "value" ) );
            }
        }

        private static void UpdateNode( XmlElement root, string skillname, double val )
        {
            if( root == null )
                return;

            XmlNodeList childs = root.SelectNodes( skillname );
            if( childs == null || childs.Count == 0 )
                return;

            foreach( XmlElement element in childs )
            {
                if( element.HasAttribute( "value" ) )
                    element.SetAttribute( "value", XmlConvert.ToString( val ) );
            }
        }

        private class SkillGainFactorGump : Gump
        {
            public SkillGainFactorGump()
                : base( 50, 50 )
            {
                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                AddPage( 0 );
                AddBackground( 0, 0, 300, 525, 5054 );

                AddImageTiled( 10, 10, 280, 21, 3004 );
                AddLabel( 13, 11, 0, "Choose the gain factor to edit:" );

                AddImageTiled( ( 290 - 30 - 50 ), ( 515 - 22 ), 48, 21, 0xBBC );
                AddLabel( ( 290 - 30 - 50 + 3 ), ( 515 - 22 + 1 ), 0, "Close:" );
                AddButton( ( 290 - 30 ), ( 515 - 22 ), 4018, 4019, 0, GumpButtonType.Reply, 0 ); // Close

                for( int i = 0; i < CustomTable.Length; i++ )
                {
                    int page = i / 20;
                    int pos = i % 20;

                    if( pos == 0 )
                    {
                        if( page > 0 )
                            AddButton( ( 290 - 16 ), 13, 0x15E1, 0x15E5, 20000, GumpButtonType.Page, page + 1 ); // Next

                        AddPage( page + 1 );

                        if( page > 0 )
                            AddButton( ( 290 - 16 - 20 ), 13, 0x15E3, 0x15E7, 20000, GumpButtonType.Page, page ); // Back
                    }

                    int y = pos * 22 + 32;

                    AddImageTiled( 10, y, 238, 21, 0xBBC );
                    AddLabelCropped( 13, y + 1, 150, 21, 0, Enum.GetName( typeof( SkillName ), i ) );

                    AddImageTiled( 181, y, 48, 21, 0xBBC );
                    AddLabelCropped( 182, y + 1, 234, 21, 0, CustomTable[ i ].ToString() );

                    AddButton( 231, y + 4, 0x15E1, 0x15E5, i + 1, GumpButtonType.Reply, 0 );
                }
            }

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                Mobile from = sender.Mobile;

                if( info.ButtonID == 0 )
                {
                    SaveSettings();
                    return;
                }

                if( info.ButtonID > 0 && info.ButtonID < CustomTable.Length )
                {
                    from.SendMessage( "Enter the new gain factor value for that skill:" );
                    from.Prompt = new SetFactorPrompt( info.ButtonID - 1 );
                    return;
                }
            }
        }

        private class SetFactorPrompt : Prompt
        {
            private readonly int m_SkillID;

            public SetFactorPrompt( int skillID )
            {
                m_SkillID = skillID;
            }

            public override void OnCancel( Mobile from )
            {
                from.SendGump( new SkillGainFactorGump() );
            }

            public override void OnResponse( Mobile from, string text )
            {
                if( text.Length > 0 )
                {
                    try
                    {
                        double val = double.Parse( text.Trim() );

                        CustomTable[ m_SkillID ] = val;
                        from.SendMessage( "You have set skillgain factor to {0}", val.ToString() );

                        Utility.Log( Path.Combine( "Logs", "gain-factor.log" ), "Player {0} set gain factor for skill {1} to {2:F3}.", from.Name, Enum.GetName( typeof( SkillName ), m_SkillID ), val );
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex.ToString() );
                    }
                }

                from.CloseGump( typeof( SkillGainFactorGump ) );
                from.SendGump( new SkillGainFactorGump() );
            }
        }
    }
}