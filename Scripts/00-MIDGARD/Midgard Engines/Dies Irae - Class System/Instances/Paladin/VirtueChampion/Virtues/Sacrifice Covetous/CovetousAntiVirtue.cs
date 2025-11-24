/***************************************************************************
 *                               CovetousAntiVirtue.cs
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
    public class CovetousAntiVirtue : BaseAntiVirtue
    {
        public CovetousAntiVirtue()
        {
            Definition = new AntiVirtueDefinition( AntiVirtues.Covetous,
                                                   "Covetous",
                                                   new Point3D(5538, 1880, 0),
                                                   true,
                                                   null,
                                                   "Avidus",
                                                   Virtues.Sacrifice,
                                                   Colors.Orange,
                                                   QuestChain.VirtueChampionSacrifice,
                                                   typeof( CompassionQuestOne ),
                                                   typeof( CompassionQuestTwo ),
                                                   typeof( CompassionQuestThree ) );
        }
    }
}