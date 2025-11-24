/***************************************************************************
 *                                    FixCaps2.cs
 *                            		---------------------
 *  begin               	: Febbraio, 2007
 * 	version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 * 
 * 	TODO
 * 
 ***************************************************************************/

using System;
using System.Collections;
using System.IO;
using System.Xml;

using Server;
using Server.Commands;
using Server.Mobiles;

namespace Midgard.Commands
{
    public class FixCaps2
    {
        #region Registrazione
        public static void Initialize()
        {
            CommandSystem.Register( "ListSkillsSkillcap", AccessLevel.Developer, new CommandEventHandler( ListaSkillsSkillcap_OnCommand ) );
            // CommandSystem.Register( "CreateXml" , AccessLevel.Developer, new CommandEventHandler( CreaXml_OnCommand ) );
            // CommandSystem.Register( "LeggiXml" , AccessLevel.Developer, new CommandEventHandler( LeggiXml_OnCommand ) );
        }
        #endregion

        #region Callback
        [Usage( "ListaSkillsSkillcap" )]
        [Description( "Lista tutte gli skillcap non a 100 dei pg di midgard 1" )]
        public static void ListaSkillsSkillcap_OnCommand( CommandEventArgs e )
        {
            string FileName = "ListaSkillsSkillcap.txt";
            ArrayList MobileArray = null;

            try
            {
                MobileArray = new ArrayList( World.Mobiles.Values );
            }
            catch
            {
                e.Mobile.SendMessage( "Errore di lettura della tabella dei Mobiles." );
                return;
            }

            TextWriter tw = File.AppendText( FileName );

            foreach( Mobile m in MobileArray )
            {
                if( m == null || m.Deleted )
                {
                    continue;
                }

                var From = m as PlayerMobile;
                if( From == null || From.Deleted || From.Account == null )
                {
                    continue;
                }
                if( From.AccessLevel > AccessLevel.Player )
                {
                    continue;
                }

                tw.WriteLine( "{0}-{1}", From.Account, From.Name );
                for( int s = 0; s < From.Skills.Length; s++ )
                {
                    var sn = (SkillName)s;
                    double valore = From.Skills[ s ].Base;
                    double cap = From.Skills[ s ].Cap;

                    if( cap > 100.0 )
                    {
                        tw.WriteLine( "\t{0}\t\t{1}/{2}", sn, valore.ToString( "F1" ), cap.ToString( "F1" ) );
                    }
                }
            }

            tw.WriteLine( "" );
            tw.WriteLine( "" );
            tw.WriteLine( "" );
            tw.WriteLine( "Da Fixare:" );

            foreach( Mobile m in MobileArray )
            {
                if( m == null || m.Deleted )
                {
                    continue;
                }

                var From = m as PlayerMobile;
                if( From == null || From.Deleted || From.Account == null )
                {
                    continue;
                }
                if( From.AccessLevel > AccessLevel.Player )
                {
                    continue;
                }

                for( int s = 0; s < From.Skills.Length; s++ )
                {
                    var sn = (SkillName)s;
                    double valore = From.Skills[ s ].Base;
                    double cap = From.Skills[ s ].Cap;

                    if( cap > 100 && cap - valore >= 5.0 )
                    {
                        tw.WriteLine( "{0}-{1}\t{2}\t\t{3}/{4}", From.Account, From.Name, sn, valore.ToString( "F1" ), cap.ToString( "F1" ) );
                    }
                }
            }
            tw.Flush();
            tw.Close();
        }

        public static void CreaXml_OnCommand( CommandEventArgs e )
        {
            ArrayList MobileArray = null;

            try
            {
                MobileArray = new ArrayList( World.Mobiles.Values );
            }
            catch
            {
                e.Mobile.SendMessage( "Errore di lettura della tabella dei Mobiles." );
                return;
            }

            var Doc = new XmlDocument();
            var XmlTw = new XmlTextWriter( "FixCaps.xml", null );
            XmlTw.Formatting = Formatting.Indented;
            XmlTw.WriteStartElement( "ToFix" );
            XmlTw.WriteElementString( "PgToFix", "-" );
            XmlTw.WriteEndElement();
            XmlTw.Flush();
            XmlTw.Close();

            var FsLoadXml = new FileStream( "FixCaps.xml", FileMode.Open, FileAccess.Read, FileShare.ReadWrite );
            try
            {
                Doc.Load( FsLoadXml );
            }
            catch
            {
                e.Mobile.SendMessage( "Operazione non riuscita: file gia' in uso." );
            }
            XmlElement Root = Doc.DocumentElement;
            FsLoadXml.Close();

            foreach( Mobile m in MobileArray )
            {
                if( m == null || m.Deleted )
                {
                    continue;
                }

                var From = m as PlayerMobile;
                if( From == null || From.Deleted || From.Account == null )
                {
                    continue;
                }
                if( From.AccessLevel > AccessLevel.Player )
                {
                    continue;
                }

                for( int s = 0; s < From.Skills.Length; s++ )
                {
                    var sn = (SkillName)s;
                    double valore = From.Skills[ s ].Base;
                    double cap = From.Skills[ s ].Cap;

                    if( cap > 100 && cap - valore >= 5.0 )
                    {
                        XmlElement PgToFix = Doc.CreateElement( "PgToFix" );

                        XmlElement Seriale = Doc.CreateElement( "Seriale" );
                        Seriale.InnerText = From.Serial.ToString();
                        PgToFix.AppendChild( Seriale );

                        XmlElement SkillName = Doc.CreateElement( "SkillName" );
                        SkillName.InnerText = sn.ToString();
                        PgToFix.AppendChild( SkillName );

                        XmlElement SkillCap = Doc.CreateElement( "SkillCap" );
                        SkillCap.InnerText = cap.ToString();
                        PgToFix.AppendChild( SkillCap );

                        Doc.DocumentElement.InsertAfter( PgToFix, Root.LastChild );
                    }
                }
            }
            var FsSaveXml = new FileStream( "FixCaps.xml", FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite );
            Doc.Save( FsSaveXml );
            FsSaveXml.Close();
        }

        public static void LeggiXml_OnCommand( CommandEventArgs e )
        {
            ArrayList MobileArray = null;

            try
            {
                MobileArray = new ArrayList( World.Mobiles.Values );
            }
            catch
            {
                e.Mobile.SendMessage( "Errore di lettura della tabella dei Mobiles." );
                return;
            }

            // string seriale = e.GetString(0);

            var Doc = new XmlDocument();
            TextWriter tw = File.AppendText( "Logs/AutoFix.log" );

            if( File.Exists( "FixCaps.xml" ) )
            {
                Doc.Load( "FixCaps.xml" );
                XmlElement Root = Doc.DocumentElement;
                //XmlNodeList ElemListXPath;

                //ElemListXPath = Root.SelectNodes( "descendant::PgToFix[Seriale='" + seriale  + "']" );

                XmlNodeList list = Root.GetElementsByTagName( "PgToFix" );

                string ser = string.Empty;
                string ski = string.Empty;
                string cap = string.Empty;

                foreach( XmlNode pg in list )
                {
                    foreach( XmlNode Figlio in pg.ChildNodes )
                    {
                        if( Figlio.Name == "Seriale" )
                        {
                            ser = Figlio.InnerText;
                        }
                        else if( Figlio.Name == "SkillName" )
                        {
                            ski = Figlio.InnerText;
                        }
                        else if( Figlio.Name == "SkillCap" )
                        {
                            cap = Figlio.InnerText;
                        }
                    }
                    if( ser.Length > 0 && ski.Length > 0 && cap.Length > 0 )
                    {
                        foreach( Mobile m in MobileArray )
                        {
                            if( m == null || m.Deleted )
                            {
                                continue;
                            }

                            var From = m as PlayerMobile;
                            if( From == null || From.Deleted || From.Account == null )
                            {
                                continue;
                            }
                            if( From.AccessLevel > AccessLevel.Player )
                            {
                                continue;
                            }

                            if( From.Serial.ToString() == ser )
                            {
                                var sn = (SkillName)Enum.Parse( typeof( SkillName ), ski );
                                double capnum = double.Parse( cap );
                                tw.WriteLine( "{0}-{1}  Skillcap che vale {2} nella skill {3}",
                                             From.Name, From.Account,
                                             From.Skills[ (int)sn ].Cap.ToString( "F1" ), sn );

                                From.Skills[ (int)sn ].Cap = capnum;
                                tw.WriteLine( "{0}-{1}  Skillcap che viene settato a {2}",
                                             From.Name, From.Account,
                                             From.Skills[ (int)sn ].Cap.ToString( "F1" ) );
                            }
                        }
                    }
                }
                tw.Close();
            }
        }
        #endregion
    }
}