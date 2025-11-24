using System.Collections;

namespace Midgard.Engines.RandomEncounterSystem
{
    public interface IElementContainer
    {
        void AddElement( EncounterElement element );
        ArrayList Elements { get; }
    }

    public interface IProbability
    {
        float Probability { get; }
    }
}