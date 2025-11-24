using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Midgard.Engines.MidgardTownSystem;
using Server;

namespace Midgard.Engines.MurderInfo
{
    class MurderInfoReport
    {
        public static readonly bool Enabled = true;

        public static void WriteReport( string path )
        {
            if( !Enabled )
                return;

            XmlDocument doc = new XmlDocument();

            if( !File.Exists( path ) )
            {
                XmlTextWriter xmlTw = new XmlTextWriter( path, Encoding.UTF8 );
                xmlTw.Formatting = Formatting.Indented;
                xmlTw.WriteStartElement( "MurderReport" );
                xmlTw.WriteElementString( "EoF", "----------" );
                xmlTw.WriteEndElement();
                xmlTw.Flush();
                xmlTw.Close();
            }

            FileStream loadStream = new FileStream( path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite );

            try
            {
                doc.Load( loadStream );
            }
            catch( XmlException ex )
            {
                Console.WriteLine( "Error in MurderInfoReport: {0}", ex.ToString() );
                return;
            }

            XmlElement root = doc.DocumentElement;
            loadStream.Close();

            // Conta le notizie presenti
            // int reportsCounter = root.GetElementsByTagName( "report" ).Count;

            // Crea l'elemento report da aggiungere
            XmlElement reportElement = doc.CreateElement( "report" );

            reportElement.SetAttribute( "date", DateTime.Today.ToShortDateString() );

            List<Mobile> killers = MurderInfoPersistance.GetKillers();
            List<MurderInfo> murders;

            XmlElement killerElement;
            XmlElement murderInfoElement;
            TownSystem system;

            if( killers != null && killers.Count > 0 )
            {
                foreach( Mobile k in killers )
                {
                    murders = MurderInfoPersistance.GetMurderInfoForKiller( k );

                    if( murders != null && murders.Count > 0 )
                    {
                        killerElement = doc.CreateElement( "killer" );
                        killerElement.SetAttribute( "name", String.IsNullOrEmpty( k.Name ) ? "unknown name" : k.Name );

                        system = TownSystem.Find( k );
                        killerElement.SetAttribute( "town", system == null ? "None" : system.Definition.TownName.String );

                        foreach( MurderInfo info in murders )
                        {
                            if( info != null && info.Victim != null )
                            {
                                murderInfoElement = doc.CreateElement( "murderInfo" );
                                murderInfoElement.SetAttribute( "victimName", String.IsNullOrEmpty( info.Victim.Name ) ? "unknown name" : info.Victim.Name );
                                murderInfoElement.SetAttribute( "timeOfDeath", info.TimeOfDeath.ToShortTimeString() );

                                killerElement.AppendChild( murderInfoElement );
                            }
                        }

                        reportElement.AppendChild( killerElement );
                    }
                }
            }

            doc.DocumentElement.InsertAfter( reportElement, root.LastChild );

            FileStream saveStream = new FileStream( path, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite );
            doc.Save( saveStream );
            saveStream.Close();
        }
    }
}