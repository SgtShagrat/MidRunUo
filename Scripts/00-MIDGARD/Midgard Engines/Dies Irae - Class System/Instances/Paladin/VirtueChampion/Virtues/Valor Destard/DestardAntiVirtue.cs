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
    public class DestardAntiVirtue : BaseAntiVirtue
    {
        public DestardAntiVirtue()
        {
            Definition = new AntiVirtueDefinition( AntiVirtues.Destard,
                                                   "Destard",
                                                   new Point3D(5204, 776, 0),
                                                   true,
                                                   null,
                                                   "FalInopialax",
                                                   Virtues.Valor,
                                                   Colors.Red,
                                                   QuestChain.VirtueChampionValor,
                                                   typeof( CompassionQuestOne ),
                                                   typeof( CompassionQuestTwo ),
                                                   typeof( CompassionQuestThree ) );
        }
    }
}