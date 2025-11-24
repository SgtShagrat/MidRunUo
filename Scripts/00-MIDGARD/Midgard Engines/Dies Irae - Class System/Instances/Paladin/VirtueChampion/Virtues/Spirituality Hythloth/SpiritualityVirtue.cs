/***************************************************************************
 *                               SpiritualityVirtue.cs
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
    public class SpiritualityVirtue : BaseVirtue
    {
        public SpiritualityVirtue()
        {
            Definition = new VirtueDefinition( Virtues.Spirituality,
                                               "Spirituality",
                                               new Point3D( 1594, 2490, 20 ),
                                               true,
                                               typeof( SpiritualityAddon ),
                                               "Om",
                                               AntiVirtues.Hythloth,
                                               Colors.White,
                                               QuestChain.VirtueChampionSpirituality,
                                               typeof( CompassionQuestOne ),
                                               typeof( CompassionQuestTwo ),
                                               typeof( CompassionQuestThree ) );
        }
    }
}