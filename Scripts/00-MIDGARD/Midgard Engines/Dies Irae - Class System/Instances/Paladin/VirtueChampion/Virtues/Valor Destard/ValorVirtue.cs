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
    public class ValorVirtue : BaseVirtue
    {
        public ValorVirtue()
        {
            Definition = new VirtueDefinition( Virtues.Valor,
                                               "Valor",
                                               new Point3D(2491,3931,5),
                                               true,
                                               typeof( ValorAddon ),
                                               "Ra",
                                               AntiVirtues.Destard,
                                               Colors.Red,
                                               QuestChain.VirtueChampionValor,
                                               typeof( CompassionQuestOne ),
                                               typeof( CompassionQuestTwo ),
                                               typeof( CompassionQuestThree ) );
        }
    }
}