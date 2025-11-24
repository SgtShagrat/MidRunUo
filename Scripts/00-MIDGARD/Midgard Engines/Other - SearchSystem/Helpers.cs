using System;

namespace Midgard.Engines.Searches
{
    public class Trig
    {
        public static double Theta( int x, int y )
        {
            double theta;

            if( y == 0 && x < 0 )
                theta = Math.PI / 2;
            else if( y == 0 && x > 0 )
                theta = Math.PI / 2 + Math.PI;
            else if( y < 0 && x == 0 )
                theta = 0;
            else if( y > 0 && x == 0 )
                theta = Math.PI;
            else if( x == 0 && y == 0 )
                theta = 0;
            else
            {
                float val = x / (float)y;

                theta = Math.Atan( val );

                if( x < 0 && y < 0 )
                    return theta;
                else if( x < 0 && y > 0 )
                    theta += Math.PI;
                else if( x > 0 && y > 0 )
                    theta += Math.PI;
                else if( x > 0 && y < 0 )
                    theta += Math.PI * 2;
            }

            return theta;
        }
    }
}