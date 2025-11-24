using Server;
using Server.Misc;

using Stat = Server.Misc.SkillCheck.Stat;

namespace Midgard.Engines.SkillSystem
{
    public class OldStatGainSystem
    {
        private class StatGainInfo
        {
            public SkillName SkillName { get; private set; }
            private Stat Primary { get; set; }
            private Stat Secondary { get; set; }

            public StatGainInfo( SkillName skillName, Stat primary, Stat secondary )
            {
                SkillName = skillName;
                Primary = primary;
                Secondary = secondary;
            }

            public static void NullifyInfo( SkillInfo info )
            {
                info.StrGain = info.DexGain = info.IntGain = 0;
            }

            public void SetPrimaryGain( SkillInfo info )
            {
                switch( Primary )
                {
                    case Stat.Str:
                        info.StrGain = 0.75;
                        break;
                    case Stat.Dex:
                        info.DexGain = 0.75;
                        break;
                    case Stat.Int:
                        info.IntGain = 0.75;
                        break;
                }
            }

            public void SetSecondaryGain( SkillInfo info )
            {
                switch( Secondary )
                {
                    case Stat.Str:
                        info.StrGain = 0.25;
                        break;
                    case Stat.Dex:
                        info.DexGain = 0.25;
                        break;
                    case Stat.Int:
                        info.IntGain = 0.25;
                        break;
                }
            }

            public bool CheckPrimaryGain( Mobile from )
            {
                return ( Utility.RandomDouble() < ( 0.75 / 33.3 ) ) && GainStat( from, Primary );
            }

            public bool CheckSecondaryGain( Mobile from )
            {
                return ( Utility.RandomDouble() < ( 0.25 / 33.3 ) ) && GainStat( from, Secondary );
            }

            private bool GainStat( Mobile from, Stat stat )
            {
                switch( stat )
                {
                    case Stat.Str:
                        if( ( from.StrLock == StatLockType.Up ) )
                        {
                            SkillCheck.GainStat( from, Stat.Str );
                            return true;
                        }
                        break;
                    case Stat.Dex:
                        if( ( from.DexLock == StatLockType.Up ) )
                        {
                            SkillCheck.GainStat( from, Stat.Dex );
                            return true;
                        }
                        break;
                    case Stat.Int:
                        if( ( from.IntLock == StatLockType.Up ) )
                        {
                            SkillCheck.GainStat( from, Stat.Int );
                            return true;
                        }
                        break;
                }

                return false;
            }
        }

        public static void CheckGain( Mobile from, SkillName name )
        {
            StatGainInfo info = GetInfo( name );

            if( !info.CheckPrimaryGain( from ) )
                info.CheckSecondaryGain( from );
        }

        private static StatGainInfo GetInfo( SkillName name )
        {
            foreach( StatGainInfo info in m_StatGainInfos )
            {
                if( info.SkillName == name )
                    return info;
            }

            return m_StatGainInfos[ 0 ];
        }

        public static void InitStatGains()
        {
            foreach( StatGainInfo info in m_StatGainInfos )
            {
                SkillInfo skillInfo = SkillInfo.Table[ (int)info.SkillName ];
                StatGainInfo.NullifyInfo( skillInfo );
                info.SetPrimaryGain( skillInfo );
                info.SetSecondaryGain( skillInfo );
            }
        }

        private static readonly StatGainInfo[] m_StatGainInfos = new StatGainInfo[]
                                                                 {
                                                                     new StatGainInfo(SkillName.Alchemy, Stat.Int, Stat.Str),
                                                                     new StatGainInfo(SkillName.Anatomy, Stat.Int, Stat.Str),
                                                                     new StatGainInfo(SkillName.AnimalLore, Stat.Int, Stat.Str),
                                                                     new StatGainInfo(SkillName.AnimalTaming, Stat.Str, Stat.Int),
                                                                     new StatGainInfo(SkillName.Archery, Stat.Dex, Stat.Str),
                                                                     new StatGainInfo(SkillName.ArmsLore, Stat.Int, Stat.Str),
                                                                     new StatGainInfo(SkillName.Begging, Stat.Dex, Stat.Int),
                                                                     new StatGainInfo(SkillName.Blacksmith, Stat.Str, Stat.Dex),
                                                                     new StatGainInfo(SkillName.Fletching, Stat.Dex, Stat.Str),
                                                                     new StatGainInfo(SkillName.Bushido, Stat.Str, Stat.Int),
                                                                     new StatGainInfo(SkillName.Camping, Stat.Dex, Stat.Int),
                                                                     new StatGainInfo(SkillName.Carpentry, Stat.Str, Stat.Dex),
                                                                     new StatGainInfo(SkillName.Cartography, Stat.Int, Stat.Dex),
                                                                     new StatGainInfo(SkillName.Chivalry, Stat.Str, Stat.Int),
                                                                     new StatGainInfo(SkillName.Cooking, Stat.Int, Stat.Dex),
                                                                     new StatGainInfo(SkillName.DetectHidden, Stat.Int, Stat.Dex),
                                                                     new StatGainInfo(SkillName.Discordance, Stat.Int, Stat.Dex),
                                                                     new StatGainInfo(SkillName.EvalInt, Stat.Int, Stat.Str),
                                                                     new StatGainInfo(SkillName.Fencing, Stat.Dex, Stat.Str),
                                                                     new StatGainInfo(SkillName.Fishing, Stat.Dex, Stat.Str),
                                                                     new StatGainInfo(SkillName.Focus, Stat.Dex, Stat.Int),
                                                                     new StatGainInfo(SkillName.Forensics, Stat.Int, Stat.Dex),
                                                                     new StatGainInfo(SkillName.Healing, Stat.Int, Stat.Dex),
                                                                     new StatGainInfo(SkillName.Herding, Stat.Int, Stat.Dex),
                                                                     new StatGainInfo(SkillName.Hiding, Stat.Dex, Stat.Int),
                                                                     new StatGainInfo(SkillName.Inscribe, Stat.Int, Stat.Dex),
                                                                     new StatGainInfo(SkillName.ItemID, Stat.Int, Stat.Dex),
                                                                     new StatGainInfo(SkillName.Lockpicking, Stat.Dex, Stat.Int),
                                                                     new StatGainInfo(SkillName.Lumberjacking, Stat.Str, Stat.Dex),
                                                                     new StatGainInfo(SkillName.Macing, Stat.Str, Stat.Dex),
                                                                     new StatGainInfo(SkillName.Magery, Stat.Int, Stat.Str),
                                                                     new StatGainInfo(SkillName.Meditation, Stat.Int, Stat.Str),
                                                                     new StatGainInfo(SkillName.Mining, Stat.Str, Stat.Dex),
                                                                     new StatGainInfo(SkillName.Musicianship, Stat.Dex, Stat.Int),
                                                                     new StatGainInfo(SkillName.Necromancy, Stat.Int, Stat.Str),
                                                                     new StatGainInfo(SkillName.Ninjitsu, Stat.Dex, Stat.Int),
                                                                     new StatGainInfo(SkillName.Parry, Stat.Dex, Stat.Str),
                                                                     new StatGainInfo(SkillName.Peacemaking, Stat.Int, Stat.Dex),
                                                                     new StatGainInfo(SkillName.Poisoning, Stat.Int, Stat.Dex),
                                                                     new StatGainInfo(SkillName.Provocation, Stat.Int, Stat.Dex),
                                                                     new StatGainInfo(SkillName.RemoveTrap, Stat.Dex, Stat.Int),
                                                                     new StatGainInfo(SkillName.MagicResist, Stat.Str, Stat.Dex),
                                                                     new StatGainInfo(SkillName.Snooping, Stat.Dex, Stat.Int),
                                                                     new StatGainInfo(SkillName.Spellweaving, Stat.Int, Stat.Str),
                                                                     new StatGainInfo(SkillName.SpiritSpeak, Stat.Int, Stat.Str),
                                                                     new StatGainInfo(SkillName.Stealing, Stat.Dex, Stat.Int),
                                                                     new StatGainInfo(SkillName.Stealth, Stat.Dex, Stat.Int),
                                                                     new StatGainInfo(SkillName.Swords, Stat.Str, Stat.Dex),
                                                                     new StatGainInfo(SkillName.Tactics, Stat.Str, Stat.Dex),
                                                                     new StatGainInfo(SkillName.Tailoring, Stat.Dex, Stat.Int),
                                                                     new StatGainInfo(SkillName.TasteID, Stat.Int, Stat.Str),
                                                                     new StatGainInfo(SkillName.Tinkering, Stat.Dex, Stat.Int),
                                                                     new StatGainInfo(SkillName.Tracking, Stat.Int, Stat.Dex),
                                                                     new StatGainInfo(SkillName.Veterinary, Stat.Int, Stat.Dex),
                                                                     new StatGainInfo(SkillName.Wrestling, Stat.Str, Stat.Dex),
                                                                 };
    }
}