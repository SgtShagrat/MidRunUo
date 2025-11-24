/***************************************************************************
 *                               HumilityVirtue.cs
 *
 *   begin                : 28 giugno 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Engines.Quests;

namespace Midgard.Engines.Classes.VirtueChampion
{
    public class HumilityVirtue : BaseVirtue
    {
        public HumilityVirtue()
        {
            Definition = new VirtueDefinition( Virtues.Humility,
                                               "Humility",
                                               new Point3D( 4273, 3697, 0 ),
                                               true,
                                               typeof( HumilityAddon ),
                                               "Lum",
                                               AntiVirtues.Pride,
                                               Colors.Black,
                                               QuestChain.VirtueChampionHumility,
                                               typeof( CompassionQuestOne ),
                                               typeof( CompassionQuestTwo ),
                                               typeof( CompassionQuestThree ) );
        }
    }
}