namespace Midgard.Engines.SpellSystem
{
    public class GumpMulti
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int ID { get; private set; }
        public int IDPressed { get; private set; }

        public GumpMulti( int x, int y, int id, int idpressed )
        {
            X = x;
            Y = y;
            ID = id;
            IDPressed = idpressed;
        }

        public GumpMulti( int x, int y, int id )
        {
            X = x;
            Y = y;
            ID = id;
        }
    }
}