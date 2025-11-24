/***************************************************************************
 *                                  TownDefinition.cs
 *                            		-------------------
 *  begin                	: Gennaio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Definizione delle proprietà legate alla città
 * 
 ***************************************************************************/

using Server;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public class TownDefinition
    {
        public MidgardTowns Town { get; private set; }
        public TextDefinition TownName { get; private set; }
        public string WelcomeMessage { get; private set; }
        public string StandardRegionName { get; private set; }
        public string[] ExtraRegionNames { get; private set; }
        public CityInfo StartCityInfo { get; private set; }
        public TownBanFlag BanFlag { get; private set; }
        public Point3D TownstoneLocation { get; private set; }

        public TownDefinition(TextDefinition townName, MidgardTowns town, string regionName, string[] extraRegionNames, CityInfo startCityInfo, TownBanFlag banFlag, Point3D townstoneLocation)
            :this( townName, town, TownSystem.DefaultWelcomeMessage, regionName, extraRegionNames, startCityInfo, banFlag, townstoneLocation )
        {
        }

        public TownDefinition(TextDefinition townName, MidgardTowns town, string regionName, CityInfo startCityInfo, TownBanFlag banFlag, Point3D townstoneLocation)
            : this( townName, town, regionName, null, startCityInfo, banFlag, townstoneLocation )
        {
        }

        public TownDefinition( TextDefinition townName, MidgardTowns town, string welcomeMessage, string regionName, string[] extraRegionNames, CityInfo startCityInfo, TownBanFlag banFlag, Point3D townstoneLocation )
        {
            TownName = townName;
            Town = town;
            WelcomeMessage = welcomeMessage;
            StandardRegionName = regionName;
            ExtraRegionNames = extraRegionNames;
            StartCityInfo = startCityInfo;
            BanFlag = banFlag;
            TownstoneLocation = townstoneLocation;
        }

        public virtual GuardDefinition[] Guards{ get{ return GuardDefinition.MinaxDef; } }

		public TextDefinition GuardIgnore{ get{ return "Civilians will now be ignored."; } }
		public TextDefinition GuardWarn{ get{ return "Civilians will now be told to go away."; } }
		public TextDefinition GuardAttack{ get{ return "Civilians will now be hanged by their toes."; } }
    }
}