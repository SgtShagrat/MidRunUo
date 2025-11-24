/***************************************************************************
 *                               WebCommandEntry.cs
 *                            ------------------------
 *   begin                : 24 agosto, 2009
 *   author               :	Dies Irae - Magius(CHE)
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae - Magius(CHE)		
 *
 ***************************************************************************/

using System;

namespace Midgard.Engines.MyMidgard
{
    public class WebCommandEntry : IComparable<WebCommandEntry>
    {
        public string Command { get; private set; }

        public WebCommandEventHandler Handler { get; private set; }

        public WebCommandEntry( string command, WebCommandEventHandler handler )
        {
            Command = command;
            Handler = handler;
        }

        public int CompareTo( WebCommandEntry other )
        {
            if( other == this )
                return 0;

            return other == null ? 1 : Command.CompareTo( other.Command );
        }
    }
}