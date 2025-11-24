using System;
using System.Collections.Generic;

namespace Midgard.Engines.MidgardTownSystem
{
    public class AgeCrimeComparer : IComparer<TownCrime>
    {
        public static readonly IComparer<TownCrime> Instance = new AgeCrimeComparer();

        private AgeCrimeComparer()
        {
        }

        public int Compare( TownCrime x, TownCrime y )
        {
            if( x == null || y == null )
                throw new ArgumentException();

            if( x.DateOfCrime > y.DateOfCrime )
                return 1;

            if( x.DateOfCrime < y.DateOfCrime )
                return -1;

            return 0;
        }
    }
}