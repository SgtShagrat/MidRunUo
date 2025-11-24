/***************************************************************************
 *                                  GenPetsLogs.cs
 *                            		-------------------
 *  begin                	: Settembre, 2006
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 *  Lista i PG nn staff esistenti su Mid 2.
 *  
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using Server;
using Server.Commands;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Commands
{
    public class GenPetsLogs
    {
        #region registrazione
        public static void Initialize()
        {
            CommandSystem.Register( "GenPetsLogs", AccessLevel.Developer, new CommandEventHandler( GeneratePetLogs ) );
        }
        #endregion

        [Usage( "GenPetsLogs" )]
        [Description( "Generate logs for tamed creatures" )]
        public static void GeneratePetLogs( CommandEventArgs e )
        {
            ArrayList mobileArray = null;

            try
            {
                mobileArray = new ArrayList( World.Mobiles.Values );
            }
            catch
            {
            }

            if( mobileArray == null )
            {
                return;
            }

            var list = new List<BaseCreature>();

            foreach( Mobile m in mobileArray )
            {
                if( m == null || m.Deleted )
                {
                    continue;
                }

                var b = m as BaseCreature;
                if( b == null )
                {
                    continue;
                }

                if( !b.Tamable )
                {
                    continue;
                }

                list.Add( b );
            }

            list.Sort( InternalComparerBaseCreature.Instance );

            using( var op = new StreamWriter( "Logs/pets.log" ) )
            {
                op.WriteLine( "# Pets table generated on {0}", DateTime.Now );
                op.WriteLine();
                op.WriteLine();

                foreach( BaseCreature b in list )
                {
                    op.WriteLine( "Type: {0} - Name: {1} - Hue {2} - Generation {3} - Seriale {4}", b.GetType().Name, ( String.IsNullOrEmpty( b.Name ) ? "null" : b.Name ), b.Hue, b.Generation, b.Serial );
                }
            }

            e.Mobile.SendMessage( "Pets table has been generated. See the file : <runuo root>/Logs/pets.log" );

            ArrayList itemArray = null;

            try
            {
                itemArray = new ArrayList( World.Items.Values );
            }
            catch
            {
            }

            if( itemArray == null )
            {
                return;
            }

            var listPetPorting = new List<PetPorting>();
            var listShrinkItem = new List<ShrinkItem>();

            foreach( Item i in itemArray )
            {
                if( i == null || i.Deleted )
                {
                    continue;
                }

                if( i is PetPorting )
                {
                    listPetPorting.Add( (PetPorting)i );
                }
                else if( i is ShrinkItem )
                {
                    listShrinkItem.Add( (ShrinkItem)i );
                }
            }

            using( var op = new StreamWriter( "Logs/badShrinkItems.log" ) )
            {
                for( int i = 0; i < listShrinkItem.Count; i++ )
                {
                    if( listShrinkItem[ i ].Pet == null )
                    {
                        op.WriteLine( "Warning: ShrinkItem without pet linked. Serial: {0}", listShrinkItem[ i ].Serial );
                        listShrinkItem.RemoveAt( i );
                    }
                }
            }

            listPetPorting.Sort( InternalComparerPetPorting.Instance );
            listShrinkItem.Sort( InternalComparerShrinkItem.Instance );

            using( var op = new StreamWriter( "Logs/petPortings.log" ) )
            {
                op.WriteLine( "# PetPorting table generated on {0}", DateTime.Now );
                op.WriteLine();
                op.WriteLine();

                foreach( PetPorting p in listPetPorting )
                {
                    op.WriteLine( "PetTypeString: {0} - PetName: {1} - PetHue {2} - Serial {3}", p.PetTypeString, p.PetName, p.PetHue, p.Serial );
                }
            }

            e.Mobile.SendMessage( "PetPortings table has been generated. See the file : <runuo root>/Logs/petPortings.log" );

            using( var op = new StreamWriter( "Logs/shrinkItems.log" ) )
            {
                op.WriteLine( "# ShrinkItem table generated on {0}", DateTime.Now );
                op.WriteLine();
                op.WriteLine();

                foreach( ShrinkItem s in listShrinkItem )
                {
                    if( s.Pet != null )
                    {
                        op.WriteLine( "Type: {0} - Name: {1} - Hue {2} - Generation {3} - Seriale {4}", s.Pet.GetType().Name, ( String.IsNullOrEmpty( s.Pet.Name ) ? "null" : s.Pet.Name ), s.Pet.Hue, s.Pet.Generation, s.Pet.Serial );
                    }
                    else
                    {
                        op.WriteLine( "Warning: ShrinkItem without pet linked. Serial: {0}", s.Serial );
                    }
                }
            }

            e.Mobile.SendMessage( "ShrinkItem table has been generated. See the file : <runuo root>/Logs/shrinkItems.log" );

            string FilePath = "MidgardPets.xml";

            // Creazione dell'xml vuoto.
            var doc = new XmlDocument();
            var xmlTw = new XmlTextWriter( FilePath, null );
            xmlTw.Formatting = Formatting.Indented;
            xmlTw.WriteStartElement( "MidgardPGs" );
            xmlTw.WriteEndElement();
            xmlTw.Flush();
            xmlTw.Close();

            var fsXml = new FileStream( FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite );
            doc.Load( fsXml );
            fsXml.Close();

            foreach( Mobile m in mobileArray )
            {
                if( m == null || m.Deleted )
                {
                    continue;
                }

                Mobile pm = m as PlayerMobile;
                if( pm == null || pm.Deleted )
                {
                    continue;
                }

                if( pm.Account == null || pm.Name == null )
                {
                    continue;
                }

                // Crea l'elemento Pg con attributi Name, Account, Serial
                XmlElement pg = doc.CreateElement( "Pg" );
                XmlAttribute name = doc.CreateAttribute( "Name" );
                name.Value = pm.Name;
                XmlAttribute account = doc.CreateAttribute( "Account" );
                account.Value = pm.Account.ToString();
                XmlAttribute serial = doc.CreateAttribute( "Serial" );
                serial.Value = pm.Serial.ToString();
                pg.SetAttributeNode( name );
                pg.SetAttributeNode( account );
                pg.SetAttributeNode( serial );

                if( pm.Mounted )
                {
                    var mo = pm.Mount as BaseCreature;
                    if( mo != null )
                    {
                        XmlElement mountedPet = doc.CreateElement( "MountedPet" );

                        XmlAttribute type = doc.CreateAttribute( "Type" );
                        type.Value = mo.GetType().Name;

                        XmlAttribute hue = doc.CreateAttribute( "Hue" );
                        hue.Value = mo.Hue.ToString();

                        XmlAttribute petName = doc.CreateAttribute( "Name" );
                        petName.Value = mo.Name;

                        XmlAttribute creationTime = doc.CreateAttribute( "CreationTime" );
                        creationTime.Value = mo.CreationTime.ToShortDateString();

                        XmlAttribute petSeriale = doc.CreateAttribute( "Seriale" );
                        petSeriale.Value = mo.Serial.ToString();

                        mountedPet.SetAttributeNode( type );
                        mountedPet.SetAttributeNode( hue );
                        mountedPet.SetAttributeNode( petName );
                        mountedPet.SetAttributeNode( creationTime );
                        mountedPet.SetAttributeNode( petSeriale );

                        pg.AppendChild( mountedPet );
                    }
                }

                for( int i = 0; i < pm.Stabled.Count; i++ )
                {
                    var bc = pm.Stabled[ i ] as BaseCreature;
                    if( bc != null )
                    {
                        XmlElement pet = doc.CreateElement( "PetStabled" );

                        XmlAttribute type = doc.CreateAttribute( "Type" );
                        type.Value = bc.GetType().Name;

                        XmlAttribute hue = doc.CreateAttribute( "Hue" );
                        hue.Value = bc.Hue.ToString();

                        XmlAttribute petName = doc.CreateAttribute( "Name" );
                        petName.Value = bc.Name;

                        XmlAttribute creationTime = doc.CreateAttribute( "CreationTime" );
                        creationTime.Value = bc.CreationTime.ToShortDateString();

                        XmlAttribute petSeriale = doc.CreateAttribute( "Seriale" );
                        petSeriale.Value = bc.Serial.ToString();

                        pet.SetAttributeNode( type );
                        pet.SetAttributeNode( hue );
                        pet.SetAttributeNode( petName );
                        pet.SetAttributeNode( creationTime );
                        pet.SetAttributeNode( petSeriale );

                        pg.AppendChild( pet );
                    }
                }
                doc.DocumentElement.InsertAfter( pg, doc.DocumentElement.LastChild );
            }

            var fsXml2 = new FileStream( FilePath, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite );
            doc.Save( fsXml2 );
            fsXml2.Close();
        }

        #region Nested type: InternalComparerBaseCreature
        private class InternalComparerBaseCreature : IComparer<BaseCreature>
        {
            public static readonly IComparer<BaseCreature> Instance = new InternalComparerBaseCreature();

            private InternalComparerBaseCreature()
            {
            }

            #region IComparer<BaseCreature> Members
            public int Compare( BaseCreature x, BaseCreature y )
            {
                if( x == null || y == null )
                {
                    throw new ArgumentException();
                }

                string typeX = x.GetType().Name;
                string typeY = y.GetType().Name;

                if( typeX != typeY )
                {
                    return Insensitive.Compare( typeX, typeY );
                }
                else
                {
                    return Insensitive.Compare( x.Name, y.Name );
                }
            }
            #endregion
        }
        #endregion

        #region Nested type: InternalComparerPetPorting
        private class InternalComparerPetPorting : IComparer<PetPorting>
        {
            public static readonly IComparer<PetPorting> Instance = new InternalComparerPetPorting();

            private InternalComparerPetPorting()
            {
            }

            #region IComparer<PetPorting> Members
            public int Compare( PetPorting x, PetPorting y )
            {
                if( x == null || y == null )
                {
                    throw new ArgumentException();
                }

                string typeX = x.PetTypeString;
                string typeY = y.PetTypeString;

                if( typeX != typeY )
                {
                    return Insensitive.Compare( typeX, typeY );
                }
                else
                {
                    return Insensitive.Compare( x.PetName, y.PetName );
                }
            }
            #endregion
        }
        #endregion

        #region Nested type: InternalComparerShrinkItem
        private class InternalComparerShrinkItem : IComparer<ShrinkItem>
        {
            public static readonly IComparer<ShrinkItem> Instance = new InternalComparerShrinkItem();

            private InternalComparerShrinkItem()
            {
            }

            #region IComparer<ShrinkItem> Members
            public int Compare( ShrinkItem x, ShrinkItem y )
            {
                if( x == null || y == null )
                {
                    throw new ArgumentException();
                }

                string typeX = x.Pet.GetType().Name;
                string typeY = y.Pet.GetType().Name;

                if( typeX != typeY )
                {
                    return Insensitive.Compare( typeX, typeY );
                }
                else
                {
                    return Insensitive.Compare( x.Pet.Name, y.Pet.Name );
                }
            }
            #endregion
        }
        #endregion
    }
}