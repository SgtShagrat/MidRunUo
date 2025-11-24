using System;

namespace Server.Engines.XmlSpawner2
{
    public class RemoteMessaging : MarshalByRefObject
    {
        public delegate byte[] Message( string typeName, byte[] data, out string answerType );

        private static int m_Instances;

        public RemoteMessaging()
        {
            m_Instances++;
        }

        public static event Message OnReceiveMessage;

        ~RemoteMessaging()
        {
            m_Instances--;
        }

        public byte[] PerformRemoteRequest( string typeName, byte[] data, out string answerType )
        {
            answerType = null;
            return OnReceiveMessage != null ? OnReceiveMessage( typeName, data, out answerType ) : null;
        }
    }
}