using System;

namespace Server.Engines.XmlSpawner2
{
    [Serializable]
    public abstract class TransferMessage
    {
        public Guid AuthenticationID;
        public bool UseMainThread;

        public virtual byte[] Compress()
        {
            return ZLib.Compress( this );
        }

        public static TransferMessage Decompress( byte[] data, Type type )
        {
            return ( ZLib.Decompress( data, type ) as TransferMessage );
        }

        public virtual TransferMessage ProcessMessage()
        {
            return new ErrorMessage( "Empty ProcessMessage" );
        }
    }
}