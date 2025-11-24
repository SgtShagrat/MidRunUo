/***************************************************************************
 *                               ShameAntiVirtue.cs
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
    public class ShameAntiVirtue : BaseAntiVirtue
    {
        public ShameAntiVirtue()
        {
            Definition = new AntiVirtueDefinition( AntiVirtues.Shame,
                                                   "Shame",
                                                   new Point3D(5816, 79, 0),
                                                   true,
                                                   null,
                                                   "Infama",
                                                   Virtues.Honor,
                                                   Colors.Purple,
                                                   QuestChain.VirtueChampionHonor,
                                                   typeof( CompassionQuestOne ),
                                                   typeof( CompassionQuestTwo ),
                                                   typeof( CompassionQuestThree ) );
        }
    }
}