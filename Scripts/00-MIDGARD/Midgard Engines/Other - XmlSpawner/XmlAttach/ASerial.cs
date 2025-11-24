namespace Server.Engines.XmlSpawner2
{
    public class ASerial
    {
        public int Value { get; private set; }

        public ASerial( int serial )
        {
            Value = serial;
        }

        private static int m_GlobalSerialValue;

        public static bool SerialInitialized;

        public static ASerial NewSerial()
        {
            // it is possible for new attachments to be constructed before existing attachments are deserialized and the current m_globalserialvalue
            // restored.  This creates a possible serial conflict, so dont allow assignment of valid serials until proper deser of m_globalserialvalue
            // Resolve unassigned serials in initialization
            if( !SerialInitialized )
                return new ASerial( 0 );

            if( m_GlobalSerialValue == int.MaxValue || m_GlobalSerialValue < 0 )
                m_GlobalSerialValue = 0;

            // try the next serial number in the series
            int newserialno = m_GlobalSerialValue + 1;

            // check to make sure that it is not in use
            while( XmlAttach.AllAttachments.Contains( newserialno ) )
            {
                newserialno++;
                if( newserialno == int.MaxValue || newserialno < 0 )
                    newserialno = 1;
            }

            m_GlobalSerialValue = newserialno;

            return new ASerial( m_GlobalSerialValue );
        }

        internal static void GlobalSerialize( GenericWriter writer )
        {
            writer.Write( m_GlobalSerialValue );
        }

        internal static void GlobalDeserialize( GenericReader reader )
        {
            m_GlobalSerialValue = reader.ReadInt();
        }
    }
}