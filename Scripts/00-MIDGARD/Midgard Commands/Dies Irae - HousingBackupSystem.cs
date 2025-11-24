/***************************************************************************
 *                               HousingBackupSystem.cs
 *                            ----------------------------
 *   begin                : 15 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/
 
using System;
using System.IO;
using Server;
using Server.Commands;
using Server.Commands.Generic;
using Server.Multis;

#region distro mod to HouseFoundation
/*
        public void Backup( GenericWriter writer )
        {
            writer.Write( m_Signpost );
            writer.Write( (int)m_SignpostGraphic );

            writer.Write( (int)m_Type );

            writer.Write( m_SignHanger );

            writer.Write( (int)m_LastRevision );
            writer.Write( m_Fixtures );

            if( m_Fixtures != null )
                Console.WriteLine( m_Fixtures.Count );

            CurrentState.Serialize( writer );
        }

        public void Restore( GenericReader reader, Mobile from )
        {
            ClearFixtures( from );

            m_Signpost = reader.ReadItem();
            m_SignpostGraphic = reader.ReadInt();
            m_Type = (FoundationType)reader.ReadInt();
            m_SignHanger = reader.ReadItem();

            m_LastRevision = reader.ReadInt();
            m_Fixtures = reader.ReadStrongItemList();

            DesignState toRestore = new DesignState( this, reader );
            AddFixtures( from, toRestore.Fixtures );

            m_Current = toRestore;
            m_Design = toRestore;
            m_Backup = toRestore;
        }
 */
#endregion

namespace Midgard.Commands
{
    class HousingBackupSystem
    {
        public static void Initialize()
        {
            TargetCommands.Register( new BackupHouseCommand() );
            TargetCommands.Register( new RestoreHouseCommand() );
        }

        public class BackupHouseCommand : BaseCommand
        {
            public BackupHouseCommand()
            {
                AccessLevel = AccessLevel.Developer;
                Supports = CommandSupport.All;
                ObjectTypes = ObjectTypes.Items;

                Commands = new string[] { "BackupHouse" };
                Usage = "BackupHouse";
                Description = "Backup a DesignHouse.";
            }

            private const string SavePath = @"HouseBackups/{0}.bin";

            public override void Execute( CommandEventArgs e, object obj )
            {
                Item target = (Item)obj;

                if( target != null )
                {
                    HouseFoundation fundation = null;
                    DesignState designState = null;

                    if( target is HouseSign && ( (HouseSign)target ).Owner is HouseFoundation )
                        fundation = (HouseFoundation)( ( (HouseSign)target ).Owner );
                    else if( target is HouseFoundation )
                        fundation = (HouseFoundation)target;
                    else
                        return;

                    if( fundation == null )
                        return;
                    else
                        designState = fundation.CurrentState;

                    if( designState != null )
                    {
                        try
                        {
                            string temp = string.Format( SavePath, target.Serial );

                            string dir = Path.Combine( Path.GetPathRoot( temp ), Path.GetDirectoryName( temp ) );
                            if( !Directory.Exists( dir ) )
                                Directory.CreateDirectory( dir );

                            BinaryFileWriter writer = new BinaryFileWriter( temp, true );

                            fundation.Backup( writer );
                            writer.Close();
                        }
                        catch
                        {
                            Console.WriteLine( "Error serializing a house design." );
                        }
                    }
                }
                else
                {
                    LogFailure( "Error." );
                }
            }
        }

        public class RestoreHouseCommand : BaseCommand
        {
            public RestoreHouseCommand()
            {
                AccessLevel = AccessLevel.Developer;
                Supports = CommandSupport.Single;
                ObjectTypes = ObjectTypes.Items;

                Commands = new string[] { "RestoreHouse" };
                Usage = "RestoreHouse";
                Description = "Restore a DesignHouse.";
            }

            private const string SavePath = @"HouseBackups/{0}.bin";

            public override void Execute( CommandEventArgs e, object obj )
            {
                Item target = (Item)obj;
                Mobile from = e.Mobile;

                if( target != null )
                {
                    HouseFoundation fundation = null;
                    Serial backupSerial = Serial.MinusOne;

                    if( target is HouseSign && ( (HouseSign)target ).Owner is HouseFoundation )
                        fundation = (HouseFoundation)( ( (HouseSign)target ).Owner );
                    else if( target is HouseFoundation )
                        fundation = (HouseFoundation)target;
                    else
                    {
                        Console.WriteLine( "Invalid object targeted." );
                        return;
                    }

                    if( fundation == null )
                    {
                        Console.WriteLine( "Invalid fundation targeted." );
                        return;
                    }
                    else
                        backupSerial = fundation.Serial;

                    Console.WriteLine( "Serial is {0}.", backupSerial );

                    if( backupSerial != Serial.MinusOne )
                    {
                        try
                        {
                            string temp = string.Format( SavePath, target.Serial );

                            BinaryReader bReader = new BinaryReader( File.OpenRead( temp ) );
                            BinaryFileReader reader = new BinaryFileReader( bReader );

                            fundation.Restore( reader, from );
                            bReader.Close();
                        }
                        catch
                        {
                            Console.WriteLine( "Error deserializing a design state." );
                        }

                        int oldPrice = fundation.Price;
                        int newPrice = oldPrice + fundation.CustomizationCost + ( fundation.CurrentState.Components.List.Length * 500 );

                        // Update house price
                        fundation.Price = newPrice - fundation.CustomizationCost;

                        // Notify the core that the foundation has changed and should be resent to all clients
                        fundation.Delta( ItemDelta.Update );
                        fundation.ProcessDelta();
                        fundation.CurrentState.SendDetailedInfoTo( from.NetState );

                        // Eject all from house
                        from.RevealingAction();

                        foreach( Item item in fundation.GetItems() )
                            item.Location = fundation.BanLocation;

                        foreach( Mobile mobile in fundation.GetMobiles() )
                            mobile.Location = fundation.BanLocation;

                        // Restore relocated entities
                        fundation.RestoreRelocatedEntities();
                    }
                    else
                    {
                        Console.WriteLine( "Design is null." );
                    }
                }
                else
                {
                    Console.WriteLine( "Invalid object targeted." );
                }
            }
        }
    }
}
