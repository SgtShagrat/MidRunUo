using System;

namespace Server.Engines.XmlSpawner2
{
    [Serializable]
    public class ErrorMessage : TransferMessage
    {
        public ErrorMessage()
        {
        }

        public ErrorMessage( string message )
        {
            Message = message;
        }

        public ErrorMessage( string message, params string[] args )
        {
            Message = string.Format( message, args );
        }

        public string Message { get; set; }
    }
}