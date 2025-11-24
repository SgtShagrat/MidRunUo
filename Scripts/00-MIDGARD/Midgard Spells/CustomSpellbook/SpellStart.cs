namespace Midgard.Engines.SpellSystem
{
    public class SpellStart
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public SpellStart( int x, int y )
        {
            X = x;
            Y = y;
        }
    }
}