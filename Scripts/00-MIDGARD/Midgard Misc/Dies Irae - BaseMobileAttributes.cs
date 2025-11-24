/***************************************************************************
 *                               BaseMobileAttributes.cs
 *
 *   begin                : 09 ottobre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;

namespace Midgard
{
    [PropertyObject]
    public abstract class BaseMobileAttributes
    {
        private uint m_Names;
        private int[] m_Values;

        private static readonly int[] m_Empty = new int[ 0 ];

        public bool IsEmpty { get { return ( m_Names == 0 ); } }

        public Mobile Owner { get; private set; }

        protected BaseMobileAttributes( Mobile owner )
        {
            Owner = owner;

            m_Values = m_Empty;
        }

        public int GetValue( int bitmask )
        {
            uint mask = (uint)bitmask;

            if( ( m_Names & mask ) == 0 )
                return 0;

            int index = GetIndex( mask );

            if( index >= 0 && index < m_Values.Length )
                return m_Values[ index ];

            return 0;
        }

        public void SetValue( int bitmask, int value )
        {
            uint mask = (uint)bitmask;

            if( value != 0 )
            {
                if( ( m_Names & mask ) != 0 )
                {
                    int index = GetIndex( mask );

                    if( index >= 0 && index < m_Values.Length )
                        m_Values[ index ] = value;
                }
                else
                {
                    int index = GetIndex( mask );

                    if( index >= 0 && index <= m_Values.Length )
                    {
                        int[] old = m_Values;
                        m_Values = new int[ old.Length + 1 ];

                        for( int i = 0; i < index; ++i )
                            m_Values[ i ] = old[ i ];

                        m_Values[ index ] = value;

                        for( int i = index; i < old.Length; ++i )
                            m_Values[ i + 1 ] = old[ i ];

                        m_Names |= mask;
                    }
                }
            }
            else if( ( m_Names & mask ) != 0 )
            {
                int index = GetIndex( mask );

                if( index >= 0 && index < m_Values.Length )
                {
                    m_Names &= ~mask;

                    if( m_Values.Length == 1 )
                    {
                        m_Values = m_Empty;
                    }
                    else
                    {
                        int[] old = m_Values;
                        m_Values = new int[ old.Length - 1 ];

                        for( int i = 0; i < index; ++i )
                            m_Values[ i ] = old[ i ];

                        for( int i = index + 1; i < old.Length; ++i )
                            m_Values[ i - 1 ] = old[ i ];
                    }
                }
            }

            if( Owner != null )
            {
                Owner.CheckStatTimers();
                Owner.UpdateResistances();
                Owner.Delta( MobileDelta.Stat | MobileDelta.WeaponDamage | MobileDelta.Hits | MobileDelta.Stam | MobileDelta.Mana );
            }
        }

        private int GetIndex( uint mask )
        {
            int index = 0;
            uint ourNames = m_Names;
            uint currentBit = 1;

            while( currentBit != mask )
            {
                if( ( ourNames & currentBit ) != 0 )
                    ++index;

                if( currentBit == 0x80000000 )
                    return -1;

                currentBit <<= 1;
            }

            return index;
        }

        public abstract void ComputeValue( int bitmask );

        public void InvalidateAll()
        {
            foreach( int value in m_Values )
            {
                ComputeValue( value );
            }
        }

        public void Invalidate( int bitmask )
        {
            ComputeValue( bitmask );
        }
        #region serialization
        protected BaseMobileAttributes( Mobile owner, GenericReader reader )
        {
            Owner = owner;

            int version = reader.ReadByte();

            switch( version )
            {
                case 0:
                    {
                        m_Names = reader.ReadUInt();
                        m_Values = new int[ reader.ReadEncodedInt() ];

                        for( int i = 0; i < m_Values.Length; ++i )
                            m_Values[ i ] = reader.ReadEncodedInt();

                        break;
                    }
            }
        }

        public void Serialize( GenericWriter writer )
        {
            writer.Write( (byte)0 ); // version;

            if( m_Values == null )
                m_Values = m_Empty;

            writer.Write( m_Names );
            writer.WriteEncodedInt( m_Values.Length );

            foreach( int i in m_Values )
                writer.WriteEncodedInt( i );
        }
        #endregion
    }
}