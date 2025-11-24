/***************************************************************************
 *                               VirtueDefinition.cs
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
    public class VirtueDefinition
    {
        public QuestChain ChainID { get; private set; }
        public Type QuestStageOneType { get; private set; }
        public Type QuestStageTwoType { get; private set; }
        public Type QuestStageThreeType { get; private set; }

        public Virtues Virtue { get; private set; }
        public string Name { get; private set; }
        public Point3D SymbolLocation { get; set; }
        public bool East { get; set; }
        public Type AddonType { get; set; }
        public string Mantra { get; private set; }
        public Colors Color { get; private set; }
        protected AntiVirtues AntiVirtue { get; private set; }

        public VirtueDefinition( Virtues virtue, string name, Point3D symbolLocation, bool east, Type addonType, string mantra, AntiVirtues antiVirtue, Colors color, QuestChain id, Type questOne, Type questTwo, Type questThree )
        {
            Virtue = virtue;
            Name = name;
            SymbolLocation = symbolLocation;
            East = east;
            AddonType = addonType;
            Mantra = mantra;
            AntiVirtue = antiVirtue;
            Color = color;

            ChainID = id;
            QuestStageOneType = questOne;
            QuestStageTwoType = questTwo;
            QuestStageThreeType = questThree;
        }

        public override string ToString()
        {
            return Enum.GetName( typeof( Virtues ), Virtue );
        }
    }
}