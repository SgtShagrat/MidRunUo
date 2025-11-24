/***************************************************************************
 *                               HythlothAntiVirtue.cs
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
    public class HythlothAntiVirtue : BaseAntiVirtue
    {
        public HythlothAntiVirtue()
        {
            Definition = new AntiVirtueDefinition( AntiVirtues.Hythloth,
                                                   "Hythloth",
                                                   new Point3D( 6028, 196, 22 ),
                                                   true,
                                                   null,
                                                   "Ignavus",
                                                   Virtues.Spirituality,
                                                   Colors.White,
                                                   QuestChain.VirtueChampionSpirituality,
                                                   typeof( CompassionQuestOne ),
                                                   typeof( CompassionQuestTwo ),
                                                   typeof( CompassionQuestThree ) );
        }
    }
}