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
    public class JusticeVirtue : BaseVirtue
    {
        public JusticeVirtue()
        {
            Definition = new VirtueDefinition( Virtues.Justice,
                                               "Justice",
                                               new Point3D(1600,633,16),
                                               false,
                                               typeof( JusticeAddon ),
                                               "Beh",
                                               AntiVirtues.Wrong,
                                               Colors.Green,
                                               QuestChain.VirtueChampionJustice,
                                               typeof( CompassionQuestOne ),
                                               typeof( CompassionQuestTwo ),
                                               typeof( CompassionQuestThree ) );
        }
    }
}