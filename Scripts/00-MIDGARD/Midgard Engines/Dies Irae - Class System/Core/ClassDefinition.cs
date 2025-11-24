/***************************************************************************
 *                               ClassDefinition.cs
 *
 *   revision             : 03 January, 2010
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;

using MidgardClasses = Midgard.Engines.Classes.Classes;

namespace Midgard.Engines.Classes
{
    public class ClassDefinition
    {
        public ClassDefinition( TextDefinition className, MidgardClasses classSystem, int localizedWelcomeMessage, string welcomeMessage, PowerDefinition[] powerDefinitions )
        {
            ClassName = className;
            Class = classSystem;
            LocalizedWelcomeMessage = localizedWelcomeMessage;
            WelcomeMessage = welcomeMessage;
            PowersDefinitions = powerDefinitions;
        }

        public MidgardClasses Class { get; private set; }
        public TextDefinition ClassName { get; private set; }
        public int LocalizedWelcomeMessage { get; private set; }
        public string WelcomeMessage { get; private set; }
        public PowerDefinition[] PowersDefinitions { get; private set; }
    }
}