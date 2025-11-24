namespace Server.Engines.XmlPoints
{
    public class DuelLocationEntry
    {
        public string Name { get; set; }
        public Map Map { get; set; }
        public Point3D FirstChallenger { get; set; }
        public Point3D SecondChallenger { get; set; }
        public Rectangle2D Area { get; set; }

        public DuelLocationEntry( string name, Map map, Point3D firstChallenger, Point3D secondChallenger, Rectangle2D area )
        {
            Name = name;
            Map = map;
            FirstChallenger = firstChallenger;
            SecondChallenger = secondChallenger;
            Area = area;
        }
    }
}