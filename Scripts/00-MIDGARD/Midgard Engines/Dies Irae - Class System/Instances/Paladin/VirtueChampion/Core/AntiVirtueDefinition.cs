/***************************************************************************
 *                               AntiVirtueDefinition.cs
 *
 *   begin                : 09 giugno 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Engines.Quests;

namespace Midgard.Engines.Classes.VirtueChampion
{
    public class AntiVirtueDefinition
    {
        public QuestChain ChainID { get; private set; }
        public Type QuestStageOneType { get; private set; }
        public Type QuestStageTwoType { get; private set; }
        public Type QuestStageThreeType { get; private set; }

        public AntiVirtues AntiVirtue { get; private set; }
        public string Name { get; private set; }
        public Point3D SymbolLocation { get; set; }
        public bool East { get; set; }
        public Type AddonType { get; set; }
        public string AntiMantra { get; private set; }
        public Colors Color { get; private set; }
        public Virtues Virtue { get; set; }

        public AntiVirtueDefinition( AntiVirtues antiVirtue, string name, Point3D symbolLocation, bool east, Type addonType, string antiMantra, Virtues virtue, Colors color, QuestChain id, Type questOne, Type questTwo, Type questThree )
        {
            AntiVirtue = antiVirtue;
            Name = name;
            SymbolLocation = symbolLocation;
            East = east;
            AddonType = addonType;
            AntiMantra = antiMantra;
            Virtue = virtue;
            Color = color;

            ChainID = id;
            QuestStageOneType = questOne;
            QuestStageTwoType = questTwo;
            QuestStageThreeType = questThree;
        }

        public override string ToString()
        {
            return Enum.GetName( typeof( AntiVirtues ), AntiVirtue );
        }
    }
}