using System;

namespace Midgard
{
    public struct MACAddress
    {
        long m_Internal;

        public MACAddress( long mac )
        {
            m_Internal = mac;
        }

        public MACAddress( byte[] mac )
        {
            if( mac.Length != 6 )
                throw new Exception( "This is not a MAC!" );

            m_Internal = mac[ 0 ] << 40 | mac[ 1 ] << 32 | mac[ 2 ] << 24 | mac[ 3 ] << 16 | mac[ 4 ] << 8 | mac[ 5 ];
        }

        public static explicit operator long( MACAddress mac )
        {
            return mac.m_Internal;
        }

        public static explicit operator MACAddress( long value )
        {
            return new MACAddress( value );
        }

        public static bool operator ==( MACAddress mac1, MACAddress mac2 )
        {
            return ( mac1.m_Internal == mac2.m_Internal );
        }

        public static bool operator !=( MACAddress mac1, MACAddress mac2 )
        {
            return ( mac1.m_Internal != mac2.m_Internal );
        }

        public override bool Equals( object obj )
        {
            if( obj is MACAddress )
                return ( (MACAddress)obj ).m_Internal == m_Internal;

            return base.Equals( obj );
        }

        public override int GetHashCode()
        {
            return m_Internal.GetHashCode();
        }

        public override string ToString()
        {
            string retval = "";
            for( int i = 5; i > 0; i-- )
            {
                retval += Convert.ToString( ( m_Internal >> ( i * 5 ) ) & 0xFF, 16 ) + "-";
            }

            return retval.Substring( 0, retval.Length - 1 );
        }
    }
}