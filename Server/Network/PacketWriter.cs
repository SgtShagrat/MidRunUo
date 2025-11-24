/***************************************************************************
 *                              PacketWriter.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: PacketWriter.cs 114 2006-12-13 18:28:37Z mark $
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Server.Network
{
	/// <summary>
	/// Provides functionality for writing primitive binary data.
	/// </summary>
	public class PacketWriter
	{
		private static Stack<PacketWriter> m_Pool = new Stack<PacketWriter>();

		public static PacketWriter CreateInstance()
		{
			return CreateInstance( 32 );
		}

		public static PacketWriter CreateInstance( int capacity )
		{
			PacketWriter pw = null;

			lock ( m_Pool )
			{
				if ( m_Pool.Count > 0 )
				{
					pw = m_Pool.Pop();

					if ( pw != null )
					{
						pw.m_Capacity = capacity;
						pw.m_Stream.SetLength( 0 );
					}
				}
			}

			if ( pw == null )
				pw = new PacketWriter( capacity );

			return pw;
		}

		public static void ReleaseInstance( PacketWriter pw )
		{
			lock ( m_Pool )
			{
				if ( !m_Pool.Contains( pw ) )
				{
					m_Pool.Push( pw );
				}
				else
				{
					try
					{
						using ( StreamWriter op = new StreamWriter( "Logs/neterr.log" ) )
						{
							op.WriteLine( "{0}\tInstance pool contains writer", DateTime.Now );
						}
					}
					catch
					{
						Console.WriteLine( "net error" );
					}
				}
			}
		}

		/// <summary>
		/// Internal stream which holds the entire packet.
		/// </summary>
		private MemoryStream m_Stream;

		private int m_Capacity;

		/// <summary>
		/// Internal format buffer.
		/// </summary>
		private static byte[] m_Buffer = new byte[4];

		/// <summary>
		/// Instantiates a new PacketWriter instance with the default capacity of 4 bytes.
		/// </summary>
		public PacketWriter() : this( 32 )
		{
		}

		/// <summary>
		/// Instantiates a new PacketWriter instance with a given capacity.
		/// </summary>
		/// <param name="capacity">Initial capacity for the internal stream.</param>
		public PacketWriter( int capacity )
		{
			m_Stream = new MemoryStream( capacity );
			m_Capacity = capacity;
		}

		/// <summary>
		/// Writes a 1-byte boolean value to the underlying stream. False is represented by 0, true by 1.
		/// </summary>
		public void Write( bool value )
		{
			m_Stream.WriteByte( (byte)(value ? 1 : 0) );
		}

		/// <summary>
		/// Writes a 1-byte unsigned integer value to the underlying stream.
		/// </summary>
		public void Write( byte value )
		{
			m_Stream.WriteByte( value );
		}

		/// <summary>
		/// Writes a 1-byte signed integer value to the underlying stream.
		/// </summary>
		public void Write( sbyte value )
		{
			m_Stream.WriteByte( (byte) value );
		}

		/// <summary>
		/// Writes a 2-byte signed integer value to the underlying stream.
		/// </summary>
		public void Write( short value )
		{
			m_Buffer[0] = (byte)(value >> 8);
			m_Buffer[1] = (byte) value;

			m_Stream.Write( m_Buffer, 0, 2 );
		}

		/// <summary>
		/// Writes a 2-byte unsigned integer value to the underlying stream.
		/// </summary>
		public void Write( ushort value )
		{
			m_Buffer[0] = (byte)(value >> 8);
			m_Buffer[1] = (byte) value;

			m_Stream.Write( m_Buffer, 0, 2 );
		}

		/// <summary>
		/// Writes a 4-byte signed integer value to the underlying stream.
		/// </summary>
		public void Write( int value )
		{
			m_Buffer[0] = (byte)(value >> 24);
			m_Buffer[1] = (byte)(value >> 16);
			m_Buffer[2] = (byte)(value >>  8);
			m_Buffer[3] = (byte) value;

			m_Stream.Write( m_Buffer, 0, 4 );
		}

		/// <summary>
		/// Writes a 4-byte unsigned integer value to the underlying stream.
		/// </summary>
		public void Write( uint value )
		{
			m_Buffer[0] = (byte)(value >> 24);
			m_Buffer[1] = (byte)(value >> 16);
			m_Buffer[2] = (byte)(value >>  8);
			m_Buffer[3] = (byte) value;

			m_Stream.Write( m_Buffer, 0, 4 );
		}

		/// <summary>
		/// Writes a sequence of bytes to the underlying stream
		/// </summary>
		public void Write( byte[] buffer, int offset, int size )
		{
			m_Stream.Write( buffer, offset, size );
		}

	    #region mod by Dies Irae
	    private static Dictionary<char, string> m_CharRep = null;

	    public static string FixAccentsAscii( string value )
	    {
	        if( m_CharRep == null ) // initialize
	        {
	            m_CharRep = new Dictionary<char, string>( 16 );

	            m_CharRep.Add( 'à', "a'" );
	            m_CharRep.Add( 'è', "e'" );
                m_CharRep.Add( 'é', "e'" );
				m_CharRep.Add( 'ì', "i'" );
				m_CharRep.Add( 'ò', "o'" );
				m_CharRep.Add( 'ù', "u'" );

	            //m_CharRep.Add('â', "a");
	            //m_CharRep.Add('å', "a");
	            //m_CharRep.Add('ä', "a");
	            //m_CharRep.Add('ç', "c");
	            //m_CharRep.Add('ê', "e");
	            //m_CharRep.Add('ë', "e");
	            //m_CharRep.Add('ü', "u");
	            //m_CharRep.Add('ö', "o");
	            //m_CharRep.Add('g', "g");
	            //m_CharRep.Add('s', "s");

	            //m_CharRep.Add('á', "a");
	            //m_CharRep.Add('ã', "a");
	            //m_CharRep.Add('é', "e");
	            //m_CharRep.Add('í', "i");
	            //m_CharRep.Add('ó', "o");
	            //m_CharRep.Add('ô', "o");
	            //m_CharRep.Add('õ', "o");
	            //m_CharRep.Add('ú', "u");

	            //m_CharRep.Add('À', "A'");
	            //m_CharRep.Add('È', "E'");
	            //m_CharRep.Add('Ì', "I'");
	            //m_CharRep.Add('Ò', "O'");
	            //m_CharRep.Add('Ù', "U'");

	            //m_CharRep.Add('Â', "A");
	            //m_CharRep.Add('Å', "A");
	            //m_CharRep.Add('Ä', "A");
	            //m_CharRep.Add('Ç', "C");
	            //m_CharRep.Add('Ê', "E");
	            //m_CharRep.Add('Ë', "E");
	            //m_CharRep.Add('Ü', "U");
	            //m_CharRep.Add('Ö', "O");
	            //m_CharRep.Add('G', "G");
	            //m_CharRep.Add('S', "S");

	            //m_CharRep.Add('Á', "A");
	            //m_CharRep.Add('Ã', "A");
	            //m_CharRep.Add('É', "E");
	            //m_CharRep.Add('Í', "I");
	            //m_CharRep.Add('Ó', "O");
	            //m_CharRep.Add('Ô', "O");
	            //m_CharRep.Add('Õ', "O");
	            //m_CharRep.Add('Ú', "U");

	            //m_CharRep.Add('ß', "B");
	            //m_CharRep.Add('i', "i");
	            //m_CharRep.Add('I', "I");
	        }

            char[] chars = value.ToCharArray();
            StringBuilder outStr = null;
            int pos = 0;

            foreach( char c in chars )
            {
                if( m_CharRep.ContainsKey( c ) )
                {
                    if( outStr == null )
                    {
                        outStr = new StringBuilder( value.Length );
                        outStr.Append( value.Substring( 0, pos ) );
                    }
                    outStr.Append( m_CharRep[ c ] );
                }
                else
                {
                    if( outStr != null )
                        outStr.Append( c );
                }
                pos++;
            }

            return outStr != null ? outStr.ToString() : value;

            //char[] chars = value.ToCharArray();
            //StringBuilder outStr = new StringBuilder( value.Length );
            //foreach( char c in chars )
            //{
            //    if( m_CharRep.ContainsKey( c ) )
            //        outStr = outStr.Append( m_CharRep[ c ] );
            //    else
            //        outStr = outStr.Append( c );
            //}
            //return outStr.ToString();
	    }
	    #endregion

		/// <summary>
		/// Writes a fixed-length ASCII-encoded string value to the underlying stream. To fit (size), the string content is either truncated or padded with null characters.
		/// </summary>
		public void WriteAsciiFixed( string value, int size )
		{
			if ( value == null )
			{
				Console.WriteLine( "Network: Attempted to WriteAsciiFixed() with null value" );
				value = String.Empty;
			}
            else
                value=FixAccentsAscii(value);

			int length = value.Length;

			m_Stream.SetLength( m_Stream.Length + size );

			if ( length >= size )
				m_Stream.Position += Encoding.ASCII.GetBytes( value, 0, size, m_Stream.GetBuffer(), (int)m_Stream.Position );
			else
			{
				Encoding.ASCII.GetBytes( value, 0, length, m_Stream.GetBuffer(), (int)m_Stream.Position );
				m_Stream.Position += size;
			}

			/*byte[] buffer = Encoding.ASCII.GetBytes( value );

			if ( buffer.Length >= size )
			{
				m_Stream.Write( buffer, 0, size );
			}
			else
			{
				m_Stream.Write( buffer, 0, buffer.Length );
				Fill( size - buffer.Length );
			}*/
		}

		/// <summary>
		/// Writes a dynamic-length ASCII-encoded string value to the underlying stream, followed by a 1-byte null character.
		/// </summary>
		public void WriteAsciiNull( string value )
		{
			if ( value == null )
			{
				Console.WriteLine( "Network: Attempted to WriteAsciiNull() with null value" );
				value = String.Empty;
			}
            else
                value = FixAccentsAscii(value);

			int length = value.Length;

			m_Stream.SetLength( m_Stream.Length + length + 1 );

			Encoding.ASCII.GetBytes( value, 0, length, m_Stream.GetBuffer(), (int)m_Stream.Position );
			m_Stream.Position += length + 1;

			/*byte[] buffer = Encoding.ASCII.GetBytes( value );

			m_Stream.Write( buffer, 0, buffer.Length );
			m_Stream.WriteByte( 0 );*/
		}

		/// <summary>
		/// Writes a dynamic-length little-endian unicode string value to the underlying stream, followed by a 2-byte null character.
		/// </summary>
		public void WriteLittleUniNull( string value )
		{
			if ( value == null )
			{
				Console.WriteLine( "Network: Attempted to WriteLittleUniNull() with null value" );
				value = String.Empty;
			}

			int length = value.Length;

			m_Stream.SetLength( m_Stream.Length + ( ( length + 1 ) * 2 ) );

			m_Stream.Position += Encoding.Unicode.GetBytes( value, 0, length, m_Stream.GetBuffer(), (int)m_Stream.Position );
			m_Stream.Position += 2;

			/*byte[] buffer = Encoding.Unicode.GetBytes( value );

			m_Stream.Write( buffer, 0, buffer.Length );

			m_Buffer[0] = 0;
			m_Buffer[1] = 0;
			m_Stream.Write( m_Buffer, 0, 2 );*/
		}

		/// <summary>
		/// Writes a fixed-length little-endian unicode string value to the underlying stream. To fit (size), the string content is either truncated or padded with null characters.
		/// </summary>
		public void WriteLittleUniFixed( string value, int size )
		{
			if ( value == null )
			{
				Console.WriteLine( "Network: Attempted to WriteLittleUniFixed() with null value" );
				value = String.Empty;
			}

			size *= 2;

			int length = value.Length;

			m_Stream.SetLength( m_Stream.Length + size );

			if ( ( length * 2 ) >= size )
				m_Stream.Position += Encoding.Unicode.GetBytes( value, 0, length, m_Stream.GetBuffer(), (int)m_Stream.Position );
			else
			{
				Encoding.Unicode.GetBytes( value, 0, length, m_Stream.GetBuffer(), (int)m_Stream.Position );
				m_Stream.Position += size;
			}

			/*size *= 2;

			byte[] buffer = Encoding.Unicode.GetBytes( value );

			if ( buffer.Length >= size )
			{
				m_Stream.Write( buffer, 0, size );
			}
			else
			{
				m_Stream.Write( buffer, 0, buffer.Length );
				Fill( size - buffer.Length );
			}*/
		}

		/// <summary>
		/// Writes a dynamic-length big-endian unicode string value to the underlying stream, followed by a 2-byte null character.
		/// </summary>
		public void WriteBigUniNull( string value )
		{
			if ( value == null )
			{
				Console.WriteLine( "Network: Attempted to WriteBigUniNull() with null value" );
				value = String.Empty;
			}

			int length = value.Length;

			m_Stream.SetLength( m_Stream.Length + ( ( length + 1 ) * 2 ) );

			m_Stream.Position += Encoding.BigEndianUnicode.GetBytes( value, 0, length, m_Stream.GetBuffer(), (int)m_Stream.Position );
			m_Stream.Position += 2;

			/*byte[] buffer = Encoding.BigEndianUnicode.GetBytes( value );

			m_Stream.Write( buffer, 0, buffer.Length );

			m_Buffer[0] = 0;
			m_Buffer[1] = 0;
			m_Stream.Write( m_Buffer, 0, 2 );*/
		}

		/// <summary>
		/// Writes a fixed-length big-endian unicode string value to the underlying stream. To fit (size), the string content is either truncated or padded with null characters.
		/// </summary>
		public void WriteBigUniFixed( string value, int size )
		{
			if ( value == null )
			{
				Console.WriteLine( "Network: Attempted to WriteBigUniFixed() with null value" );
				value = String.Empty;
			}

			size *= 2;

			int length = value.Length;

			m_Stream.SetLength( m_Stream.Length + size );

			if ( ( length * 2 ) >= size )
				m_Stream.Position += Encoding.BigEndianUnicode.GetBytes( value, 0, length, m_Stream.GetBuffer(), (int)m_Stream.Position );
			else
			{
				Encoding.BigEndianUnicode.GetBytes( value, 0, length, m_Stream.GetBuffer(), (int)m_Stream.Position );
				m_Stream.Position += size;
			}

			/*size *= 2;

			byte[] buffer = Encoding.BigEndianUnicode.GetBytes( value );

			if ( buffer.Length >= size )
			{
				m_Stream.Write( buffer, 0, size );
			}
			else
			{
				m_Stream.Write( buffer, 0, buffer.Length );
				Fill( size - buffer.Length );
			}*/
		}

		/// <summary>
		/// Fills the stream from the current position up to (capacity) with 0x00's
		/// </summary>
		public void Fill()
		{
			Fill( (int) (m_Capacity - m_Stream.Length) );
		}

		/// <summary>
		/// Writes a number of 0x00 byte values to the underlying stream.
		/// </summary>
		public void Fill( int length )
		{
			if ( m_Stream.Position == m_Stream.Length )
			{
				m_Stream.SetLength( m_Stream.Length + length );
				m_Stream.Seek( 0, SeekOrigin.End );
			}
			else
			{
				m_Stream.Write( new byte[length], 0, length );
			}
		}

		/// <summary>
		/// Gets the total stream length.
		/// </summary>
		public long Length
		{
			get
			{
				return m_Stream.Length;
			}
		}

		/// <summary>
		/// Gets or sets the current stream position.
		/// </summary>
		public long Position
		{
			get
			{
				return m_Stream.Position;
			}
			set
			{
				m_Stream.Position = value;
			}
		}

		/// <summary>
		/// The internal stream used by this PacketWriter instance.
		/// </summary>
		public MemoryStream UnderlyingStream
		{
			get
			{
				return m_Stream;
			}
		}

		/// <summary>
		/// Offsets the current position from an origin.
		/// </summary>
		public long Seek( long offset, SeekOrigin origin )
		{
			return m_Stream.Seek( offset, origin );
		}

		/// <summary>
		/// Gets the entire stream content as a byte array.
		/// </summary>
		public byte[] ToArray()
		{
			return m_Stream.ToArray();
		}
	}
}