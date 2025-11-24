/***************************************************************************
 *                               HonorVirtue.cs
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
    public class HonorVirtue : BaseVirtue
    {
        public HonorVirtue()
        {
            Definition = new VirtueDefinition( Virtues.Honor,
                                               "Honor",
                                               new Point3D(1726,3528,3),
                                               true,
                                               typeof( HonorAddon ),
                                               "Summ",
                                               AntiVirtues.Shame,
                                               Colors.Purple,
                                               QuestChain.VirtueChampionHonor,
                                               typeof( CompassionQuestOne ),
                                               typeof( CompassionQuestTwo ),
                                               typeof( CompassionQuestThree ) );
        }
    }
}