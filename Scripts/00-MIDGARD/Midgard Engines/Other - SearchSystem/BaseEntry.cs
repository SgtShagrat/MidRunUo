using System;
using System.Collections.Generic;

namespace Midgard.Engines.Searches
{
    public class Entry
    {
        public Entry( int x, int y, DistanceFunction distanceFunction )
        {
            X = x;
            Y = y;

            if( distanceFunction == DistanceFunction.Geometric )
                ApplyGeometricDistance();
            else
                ApplyLinearDistance();
        }

        public int Distance { get; private set; }

        public int X { get; private set; }

        public int Y { get; private set; }

        public void ApplyGeometricDistance()
        {
            Distance = (int)Math.Sqrt( X * X + Y * Y );
        }

        public void ApplyLinearDistance()
        {
            int absX = Math.Abs( X );
            int absY = Math.Abs( Y );

            if( absY > absX )
                Distance = absY;
            else
                Distance = absX;
        }

        public override string ToString()
        {
            return "" + Distance;
        }
    }

    public class EntrySet
    {
        public EntrySet( int distance )
        {
            Distance = distance;
            Entries = new List<Entry>();
        }

        public List<Entry> Entries { get; private set; }

        public int Distance { get; private set; }

        public void AddEntry( Entry entry )
        {
            Entries.Add( entry );
        }

        public override string ToString()
        {
            string str = "[ ";

            foreach( Entry entry in Entries )
                str += entry + ", ";

            str += " ]";

            return str;
        }
    }
}