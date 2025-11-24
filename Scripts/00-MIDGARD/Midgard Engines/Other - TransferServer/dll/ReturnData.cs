using System;

namespace Server.Engines.XmlSpawner2
{
    [Serializable]
    public class ReturnData : TransferMessage
    {
        public ReturnData()
        {
        }

        public ReturnData( object data )
        {
            Data = data;
        }

        public ReturnData( object data, string type )
        {
            Data = data;
            Typename = type;
        }

        public object Data { get; set; }

        public string Typename { get; set; }
    }
}