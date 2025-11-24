/***************************************************************************
 *                               AcademyDefinition.cs
 *
 *   begin                : 05 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;

namespace Midgard.Engines.Academies
{
    public class AcademyDefinition
    {
        public AcademyDefinition( TextDefinition academyName, string welcomeMessage )
        {
            AcademyName = academyName;
            WelcomeMessage = welcomeMessage;
        }

        public TextDefinition AcademyName { get; private set; }
        public string WelcomeMessage { get; private set; }
    }
}