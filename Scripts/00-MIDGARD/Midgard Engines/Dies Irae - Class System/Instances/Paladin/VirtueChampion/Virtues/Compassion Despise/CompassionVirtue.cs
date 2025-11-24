/***************************************************************************
 *                               CompassionVirtue.cs
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
    public class CompassionVirtue : BaseVirtue
    {
        public CompassionVirtue()
        {
            Definition = new VirtueDefinition( Virtues.Compassion,
                                               "Compassion",
                                               new Point3D( 1857, 874, -1 ),
                                               false,
                                               typeof( CompassionAddon ),
                                               "Mu",
                                               AntiVirtues.Despise,
                                               Colors.Yellow,
                                               QuestChain.VirtueChampionCompassion,
                                               typeof( CompassionQuestOne ),
                                               typeof( CompassionQuestTwo ),
                                               typeof( CompassionQuestThree ) );
        }
    }
}