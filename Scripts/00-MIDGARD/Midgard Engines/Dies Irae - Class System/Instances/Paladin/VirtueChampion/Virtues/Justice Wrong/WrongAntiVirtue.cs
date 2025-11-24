/***************************************************************************
 *                               WrongAntiVirtue.cs
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
    public class WrongAntiVirtue : BaseAntiVirtue
    {
        public WrongAntiVirtue()
        {
            Definition = new AntiVirtueDefinition( AntiVirtues.Wrong,
                                                   "Wrong",
                                                   new Point3D(5784, 527, 10),
                                                   true,
                                                   null,
                                                   "Malum",
                                                   Virtues.Justice,
                                                   Colors.Green,
                                                   QuestChain.VirtueChampionJustice,
                                                   typeof( CompassionQuestOne ),
                                                   typeof( CompassionQuestTwo ),
                                                   typeof( CompassionQuestThree ) );
        }
    }
}