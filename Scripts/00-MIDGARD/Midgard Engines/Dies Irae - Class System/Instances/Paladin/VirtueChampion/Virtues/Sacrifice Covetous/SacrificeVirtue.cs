/***************************************************************************
 *                               SacrificeVirtue.cs
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
    public class SacrificeVirtue : BaseVirtue
    {
        public SacrificeVirtue()
        {
            Definition = new VirtueDefinition( Virtues.Sacrifice,
                                               "Sacrifice",
                                               new Point3D(3354,289,4),
                                               false,
                                               typeof( SacrificeAddon ),
                                               "Cah",
                                               AntiVirtues.Covetous,
                                               Colors.Orange,
                                               QuestChain.VirtueChampionSacrifice,
                                               typeof( CompassionQuestOne ),
                                               typeof( CompassionQuestTwo ),
                                               typeof( CompassionQuestThree ) );
        }
    }
}