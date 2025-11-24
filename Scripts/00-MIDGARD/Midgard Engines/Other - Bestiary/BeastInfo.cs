namespace Midgard.Engines.Bestiary
{
    /// <summary>
    /// Contains beast specific information
    /// </summary>
    public struct BeastInfo
    {
        public string Background;
        public string Name;

        public BeastInfo( string name, string background )
        {
            Name = name;
            Background = background;
        }
    } ;
}