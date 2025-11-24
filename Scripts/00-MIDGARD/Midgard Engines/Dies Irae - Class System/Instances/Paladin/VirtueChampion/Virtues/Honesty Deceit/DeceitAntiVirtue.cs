/***************************************************************************
 *                               DeceitAntiVirtue.cs
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
    public class DeceitAntiVirtue : BaseAntiVirtue
    {
        public DeceitAntiVirtue()
        {
            Definition = new AntiVirtueDefinition( AntiVirtues.Deceit,
                                                   "Deceit",
                                                   new Point3D( 5266, 675, 5 ),
                                                   true,
                                                   null,
                                                   "Fallax",
                                                   Virtues.Honesty,
                                                   Colors.Blue,
                                                   QuestChain.VirtueChampionHonesty,
                                                   typeof( CompassionQuestOne ),
                                                   typeof( CompassionQuestTwo ),
                                                   typeof( CompassionQuestThree ) );
        }
    }
}