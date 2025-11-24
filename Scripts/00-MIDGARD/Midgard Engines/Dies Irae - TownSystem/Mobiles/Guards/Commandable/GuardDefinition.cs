using System;
using Server;

namespace Midgard.Engines.MidgardTownSystem
{
    public class GuardDefinition
    {
        public Type Type { get; private set; }

        public int Price { get; private set; }
        public int Upkeep { get; private set; }
        public int Maximum { get; private set; }
        public int ItemID { get; private set; }

        public TextDefinition Header { get; private set; }
        public TextDefinition Label { get; private set; }

        public GuardDefinition( Type type, int itemID, int price, int upkeep, int maximum, TextDefinition header, TextDefinition label )
        {
            Type = type;

            Price = price;
            Upkeep = upkeep;
            Maximum = maximum;
            ItemID = itemID;

            Header = header;
            Label = label;
        }

        public static GuardDefinition[] MinaxDef = new GuardDefinition[]
        {
            new GuardDefinition( typeof( TownHenchman ),	0x1403, 5000, 1000, 10,		new TextDefinition( 1011526, "HENCHMAN" ),		new TextDefinition( 1011510, "Hire TownHenchman" ) ),
            new GuardDefinition( typeof( TownMercenary ),	0x0F62, 6000, 2000, 10,		new TextDefinition( 1011527, "MERCENARY" ),		new TextDefinition( 1011511, "Hire TownMercenary" ) ),
            new GuardDefinition( typeof( TownBerserker ),	0x0F4B, 7000, 3000, 10,		new TextDefinition( 1011505, "BERSERKER" ),		new TextDefinition( 1011499, "Hire TownBerserker" ) ),
            new GuardDefinition( typeof( TownDragoon ),		0x1439, 8000, 4000, 10,		new TextDefinition( 1011506, "DRAGOON" ),		new TextDefinition( 1011500, "Hire TownDragoon" ) ),
        };

        public static GuardDefinition[] TrueBritDef = new GuardDefinition[]
        {
            new GuardDefinition( typeof( TownHenchman ),		0x1403, 5000, 1000, 10,		new TextDefinition( 1011526, "HENCHMAN" ),		new TextDefinition( 1011510, "Hire TownHenchman" ) ),
            new GuardDefinition( typeof( TownMercenary ),	    0x0F62, 6000, 2000, 10,		new TextDefinition( 1011527, "MERCENARY" ),		new TextDefinition( 1011511, "Hire TownMercenary" ) ),
            new GuardDefinition( typeof( TownKnight ),		    0x0F4D, 7000, 3000, 10,		new TextDefinition( 1011528, "KNIGHT" ),		new TextDefinition( 1011497, "Hire TownKnight" ) ),
            new GuardDefinition( typeof( TownPaladin ),	0x143F, 8000, 4000, 10,		new TextDefinition( 1011529, "PALADIN" ),		new TextDefinition( 1011498, "Hire Paladin" ) ),
        };

        public static GuardDefinition[] MagesDef = new GuardDefinition[]
        {
            new GuardDefinition( typeof( TownHenchman ),		0x1403, 5000, 1000, 10,		new TextDefinition( 1011526, "HENCHMAN" ),		new TextDefinition( 1011510, "Hire TownHenchman" ) ),
            new GuardDefinition( typeof( TownMercenary ),	    0x0F62, 6000, 2000, 10,		new TextDefinition( 1011527, "MERCENARY" ),		new TextDefinition( 1011511, "Hire TownMercenary" ) ),
            new GuardDefinition( typeof( TownSorceress ),	0x0E89, 7000, 3000, 10,		new TextDefinition( 1011507, "SORCERESS" ),		new TextDefinition( 1011501, "Hire Sorceress" ) ),
            new GuardDefinition( typeof( TownWizard ),		0x13F8, 8000, 4000, 10,		new TextDefinition( 1011508, "ELDER WIZARD" ),	new TextDefinition( 1011502, "Hire Elder Wizard" ) ),
        };

        public static GuardDefinition[] ShadowLordsDef = new GuardDefinition[]
        {
            new GuardDefinition( typeof( TownHenchman ),		    0x1403, 5000, 1000, 10,		new TextDefinition( 1011526, "HENCHMAN" ),		new TextDefinition( 1011510, "Hire TownHenchman" ) ),
            new GuardDefinition( typeof( TownMercenary ),	        0x0F62, 6000, 2000, 10,		new TextDefinition( 1011527, "MERCENARY" ),		new TextDefinition( 1011511, "Hire TownMercenary" ) ),
            new GuardDefinition( typeof( TownDeathKnight ),	        0x0F45, 7000, 3000, 10,		new TextDefinition( 1011512, "DEATH KNIGHT" ),	new TextDefinition( 1011503, "Hire Death TownKnight" ) ),
            new GuardDefinition( typeof( TownNecromancer ),	0x13F8, 8000, 4000, 10,		new TextDefinition( 1011513, "SHADOW MAGE" ),	new TextDefinition( 1011504, "Hire Shadow Mage" ) ),
        };
    }
}