/***************************************************************************
 *                                 StringQueries.cs
 *                            -------------------
 *   begin                : March 25, 2009
 *   copyright            : (C) The Midgard and UOSA Development Team
 *   email                : tocasia@alice.it
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
using Server.Network;

namespace Server.StringQueries
{
    public enum StringQueryStyle : byte
    {
        Disable = 0,
        Normal = 1,
        Numerical = 2
    };

    /// <summary>
    /// From: http://docs.polserver.com/packets/index.php?Packet=0xAB
    /// 
    /// Packet Name: Gump Text Entry Dialog
    /// 
    /// Packet: 0xAB
    /// Sent By: Server
    /// Size: Variable
    /// 
    /// Packet Build
    /// BYTE[1] 0xAB
    /// BYTE[2] blockSize
    /// BYTE[4] id
    /// BYTE[1] parentID
    /// BYTE[1] buttonID
    /// BYTE[1] textlen
    /// BYTE[?] text
    /// BYTE[1] cancel (0=disable, 1=enable)
    /// BYTE[1] style (0=disable, 1=normal, 2=numerical)
    /// BYTE[4] format (if style 1, max text len, if style2, max numeric value)
    /// BYTE[1] text2 length
    /// BYTE[?] text2
    /// </summary>
    public class StringQuery
    {
        private string m_TopText = "";
        private string m_EntryText = "";

        private bool m_Cancellable = false;
        private StringQueryStyle m_Style = StringQueryStyle.Normal;
        private int m_Max = 10;

        private int m_Serial;
        private static int m_NextSerial = 1;

        public int Serial
        {
            get
            {
                return m_Serial;
            }
        }

        public string TopText
        {
            get
            {
                return m_TopText;
            }
        }

        public string EntryText
        {
            get
            {
                return m_EntryText;
            }
        }

        public bool Cancellable
        {
            get
            {
                return m_Cancellable;
            }
        }

        public StringQueryStyle Style
        {
            get
            {
                return m_Style;
            }
        }

        public int Max
        {
            get
            {
                return m_Max;
            }
        }

        public StringQuery( string topText, bool cancellable, StringQueryStyle number, int max, string entryText )
        {
            m_TopText = topText;
            m_Cancellable = cancellable;
            m_Style = number;
            m_Max = max;
            m_EntryText = entryText;

            do
            {
                m_Serial = m_NextSerial++;
                m_Serial &= 0x7FFFFFFF;
            } while( m_Serial == 0 );

            m_Serial = (int)( (uint)m_Serial | 0x80000000 );
        }

        public void SendTo( NetState state )
        {
            state.AddStringQuery( this );
            state.Send( new DisplayStringQuery( this ) );
        }

        public virtual void OnResponse( NetState sender, bool okay, string text )
        {
        }
    }

    public class DisplayStringQuery : Packet
    {
        //Text Entry
        //________________________________________
        //byte	ID (AB)
        //word	Packet Size
        //dword	Serial
        //byte	Parent ID
        //byte	Buttom ID
        //word	Text Length
        //char[*]	Text
        //byte	Style (0=none, 1=normal, 2=numerical)
        //dword	Max Length
        //word	Label Length
        //char[*]	Label

        //Packet Build
        //BYTE[1] cmd
        //BYTE[2] blockSize
        //BYTE[4] id
        //BYTE[1] parentID
        //BYTE[1] buttonID
        //BYTE[1] textlen
        //BYTE[?] text
        //BYTE[1] cancel (0=disable, 1=enable)
        //BYTE[1] style (0=disable, 1=normal, 2=numerical)
        //BYTE[4] format (if style 1, max text len, if style2, max numeric value)
        //BYTE[1] text2 length
        //BYTE[?] text2
        public DisplayStringQuery( StringQuery stringQuery )
            : base( 0xAB )
        {
            int len = stringQuery.TopText.Length + stringQuery.EntryText.Length + 21;
            string entry = stringQuery.EntryText;

            EnsureCapacity( len );

            m_Stream.Write( stringQuery.Serial );
            m_Stream.Write( (short)0 ); // unknown

            m_Stream.Write( (short)( stringQuery.TopText.Length + 1 ) );
            m_Stream.WriteAsciiNull( stringQuery.TopText );

            m_Stream.Write( stringQuery.Cancellable ); // is able to cancel? true/false

            m_Stream.Write( (byte)stringQuery.Style ); // style (0=disable, 1=normal, 2=numerical)

            m_Stream.Write( stringQuery.Max ); // max. if string it's max length, if number it's maximum number

            m_Stream.Write( (short)( stringQuery.EntryText.Length + 1 ) );
            m_Stream.WriteAsciiNull( stringQuery.EntryText );
        }
    }
}