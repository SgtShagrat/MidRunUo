/***************************************************************************
 *                               PrideAntiVirtue.cs
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
    public class PrideAntiVirtue : BaseAntiVirtue
    {
        public PrideAntiVirtue()
        {
            Definition = new AntiVirtueDefinition( AntiVirtues.Pride,
                                                   "Pride",
                                                   new Point3D( 4762, 3762, 0 ),
                                                   true,
                                                   null,
                                                   "Veramocor",
                                                   Virtues.Humility,
                                                   Colors.Black,
                                                   QuestChain.VirtueChampionHumility,
                                                   typeof( CompassionQuestOne ),
                                                   typeof( CompassionQuestTwo ),
                                                   typeof( CompassionQuestThree ) );
        }
    }
}