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
    public class HonestyVirtue : BaseVirtue
    {
        public HonestyVirtue()
        {
            Definition = new VirtueDefinition( Virtues.Honesty,
                                               "Honesty",
                                               new Point3D(4208, 564, 47),
                                               true,
                                               typeof( HonestyAddon ),
                                               "Ahm",
                                               AntiVirtues.Deceit,
                                               Colors.Blue,
                                               QuestChain.VirtueChampionHonesty,
                                               typeof( CompassionQuestOne ),
                                               typeof( CompassionQuestTwo ),
                                               typeof( CompassionQuestThree ) );
        }
    }
}