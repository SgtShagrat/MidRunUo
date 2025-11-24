using System;
using Server;

namespace Midgard.Engines.Races
{
    public class StatBonuses
    {
        public static void RegisterSink()
        {
            EventSink.Login += new LoginEventHandler( LoginRaceValidation );
        }

        public static void LoginRaceValidation( LoginEventArgs args )
        {
            if( !Config.StatBonusesEnabled )
                return;

            Mobile m = args.Mobile;
            if( m == null )
                return;

            m.RemoveStatMod( "RaceStatBonusStr" );
            m.RemoveStatMod( "RaceStatBonusDex" );
            m.RemoveStatMod( "RaceStatBonusInt" );
            foreach( RaceStatMod rstm in m_List )
            {
                if( rstm.ModRace == m.Race )
                {
                    m.AddStatMod( rstm );
                }
            }
        }

        #region StatBonus List
        public static readonly StatMod[] m_List = new RaceStatMod[]
        {
            new RaceStatMod( StatType.Str, "RaceStatBonusStr", -5, TimeSpan.Zero, Core.HighElf ),
            new RaceStatMod( StatType.Dex, "RaceStatBonusDex", +5, TimeSpan.Zero, Core.HighElf ),

            new RaceStatMod( StatType.Str, "RaceStatBonusStr", -5, TimeSpan.Zero, Core.NorthernElf ),
            new RaceStatMod( StatType.Dex, "RaceStatBonusDex", +5, TimeSpan.Zero, Core.NorthernElf ),

            new RaceStatMod( StatType.Dex, "RaceStatBonusDex", -5, TimeSpan.Zero, Core.MountainDwarf ),

            new RaceStatMod( StatType.Int, "RaceStatBonusInt", 5, TimeSpan.Zero, Core.FairyOfWood ),

            new RaceStatMod( StatType.Int, "RaceStatBonusInt", 5, TimeSpan.Zero, Core.FairyOfFire ),

            new RaceStatMod( StatType.Int, "RaceStatBonusInt", 5, TimeSpan.Zero, Core.FairyOfWater ),

            new RaceStatMod( StatType.Int, "RaceStatBonusInt", 5, TimeSpan.Zero, Core.FairyOfAir ),

            new RaceStatMod( StatType.Int, "RaceStatBonusInt", 5, TimeSpan.Zero, Core.FairyOfEarth ),

            new RaceStatMod( StatType.Str, "RaceStatBonusStr", 5, TimeSpan.Zero, Core.HighOrc ),

            new RaceStatMod( StatType.Dex, "RaceStatBonusDex", 5, TimeSpan.Zero, Core.Sprite ),

            // new RaceStatMod( StatType.Str, "RaceStatBonusStr", -5, TimeSpan.Zero, MidgardRaces.HalfElf ),

            new RaceStatMod( StatType.Dex, "RaceStatBonusDex", 5, TimeSpan.Zero, Core.Drow ),

            new RaceStatMod( StatType.Str, "RaceStatBonusStr", 5, TimeSpan.Zero, Core.NorthernHuman ),
            new RaceStatMod( StatType.Dex, "RaceStatBonusDex", -5, TimeSpan.Zero, Core.NorthernHuman ),

            new RaceStatMod( StatType.Str, "RaceStatBonusStr", -5, TimeSpan.Zero, Core.Undead ),
            new RaceStatMod( StatType.Int, "RaceStatBonusInt", 5, TimeSpan.Zero, Core.Undead ),
        };
        #endregion
    }

    public class RaceStatMod : StatMod
    {
        [CommandProperty(AccessLevel.Player)]
        public Race ModRace { get; private set; }

        public RaceStatMod( StatType type, string name, int offset, TimeSpan duration, Race race )
            : base( type, name, offset, duration )
        {
            ModRace = race;
        }
    }
}