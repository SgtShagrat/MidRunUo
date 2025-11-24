using System;
using System.IO;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2.Xts
{
    // this is the return message for spawner unload status information
    [Serializable]
    public class ReturnSpawnerUnloadStatus : TransferMessage
    {
        public int ProcessedMaps;
        public int ProcessedSpawners;

        public ReturnSpawnerUnloadStatus( int nspawners, int nmaps )
        {
            ProcessedMaps = nmaps;
            ProcessedSpawners = nspawners;
        }

        public ReturnSpawnerUnloadStatus()
        {
        }
    }

    // this message will unload spawners that are provided in the data stream
    [Serializable]
    public class UnloadSpawnerData : TransferMessage
    {
        // this is the data that will be streamed to the XmlUnLoadFromStream method.

        public byte[] Data { get; set; }

        public UnloadSpawnerData( byte[] data )
        {
            Data = data;
        }

        public UnloadSpawnerData()
        {
        }

        // fill the return message with a list of object locations and names
        // access can be restricted with the TransferAccess attribute

        [TransferAccess( AccessLevel.Seer )]
        public override TransferMessage ProcessMessage()
        {
            // place the xml spawner info into a memory buffer
            MemoryStream mstream = new MemoryStream( Data );

            int processedmaps;
            int processedspawners;
            Mobile from = null;
            TransferServer.AuthEntry auth = TransferServer.GetAuthEntry( this );

            if( auth != null )
            {
                from = auth.User;
            }
            XmlSpawner.XmlUnLoadFromStream( mstream, "Spawn Editor MemStream", String.Empty, from, out processedmaps, out processedspawners );

            return new ReturnSpawnerUnloadStatus( processedspawners, processedmaps );
        }
    }
}