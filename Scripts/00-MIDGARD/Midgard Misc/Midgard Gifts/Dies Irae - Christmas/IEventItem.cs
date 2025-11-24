namespace Midgard.Engines.Events
{
    public interface IEventItem
    {
        int Year { get; set; }
        EventType Event { get; }
    }
}