/***************************************************************************
 *                               DespiseAntiVirtue.cs
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
    public class DespiseAntiVirtue : BaseAntiVirtue
    {
        public DespiseAntiVirtue()
        {
            Definition = new AntiVirtueDefinition( AntiVirtues.Despise,
                                                   "Despise",
                                                   new Point3D(5425, 570, 65),
                                                   true,
                                                   null,
                                                   "Vilis",
                                                   Virtues.Compassion,
                                                   Colors.Yellow,
                                                   QuestChain.VirtueChampionCompassion,
                                                   typeof( CompassionQuestOne ),
                                                   typeof( CompassionQuestTwo ),
                                                   typeof( CompassionQuestThree ) );
        }
    }
}