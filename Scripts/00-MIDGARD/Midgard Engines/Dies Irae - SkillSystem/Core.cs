using System;
using System.Collections.Generic;
using System.IO;

using Midgard.Engines.Races;

using Server;
using Server.Misc;
using Server.Mobiles;

namespace Midgard.Engines.SkillSystem
{
    public class Core
    {
        public static void InitCore()
        {
            if( Config.Debug )
            {
                using( TextWriter tw = File.CreateText( Path.Combine( "Logs", "rawPoints.cfg" ) ) )
                {
                    for( int i = 0; i < 1000; i++ )
                    {
                        if( i % 10 == 0 )
                            tw.WriteLine( "{0}\t{1}", i, BaseSkillToRawSkill( i ) );
                    }
                }
            }

            /*
            using( TextWriter tw = File.CreateText( "random.log" ) )
                for( int i = 0; i < 1000000; i++ )
                    tw.WriteLine( Utility.RandomDouble() );
            */

            var list = new List<int>();
            for( var i = 0; i <= 1000; i++ )
                list.Add( BaseSkillToRawSkill( i ) );

            RawPointsCurve = list.ToArray();
        }

        public static double PolGetChance( SkillName skill, double chance, double skillValue )
        {
            // chance is lineary dependant of the difficulty
            // so raw points must be scaled
            int rolled = IsCombatSkill( skill ) ? DefaultCombatSkillAdv.Roll() : DefaultSkillAdv.Roll();
            int realPoints = (int)( rolled * ( 1 - chance ) );
            int oldPoints = RawPointsCurve[ (int)( skillValue * 10 ) ];
            int newPoints = RawPointsCurve[ (int)( ( skillValue * 10 ) + 1 ) ];

            // gain factor is chance (between 0 and 1) to get the gain
            // gc * num_of_repeats * ( newPoints - oldPoints ) 
            // is the area under gain curve for a give 0.1 skill interval
            double gc = realPoints / (double)( newPoints - oldPoints );

            // be sure not to ger out of acceptable max values of gc for that skill
            double maxGainFactor = SkillGainFactorHelper.GetMaxChanceToGain( skillValue );
            if( gc > maxGainFactor )
                gc = maxGainFactor;

            // be sure not to ger out of acceptable min values of gc for that skill
            double minGainFactor = SkillGainFactorHelper.MinChanceToGain;

            if( gc < minGainFactor )
                gc = minGainFactor;

            // gain factor is internal modification of runuo team (default is 1.00)
            gc *= SkillInfo.Table[ (int)skill ].GainFactor;

            return gc;
        }

        public static bool PolCheckSkill( Mobile from, Skill skill, object amObj, double chance )
        {
            if( from.Skills.Cap == 0 )
                return false;

            if( skill.Fixed < 0 )
                return false;

            if( Config.SkillLogEnabled )
                SkillSystemLog.RegisterUse( skill.Info.SkillID );

            // success chance is calculated before this method
            bool success = ( chance >= Utility.RandomDouble() );

            if( skill.Fixed >= 1000 )
                return success;

            // if we cannot gain return
            if( skill.Base >= skill.Cap )
                return success;

            if( !from.Alive )
                return success;

            if( SendLog && from.PlayerDebug )
                from.SendMessage( "Debug PolCheckSkill. Skill: {0}, Value (fixed): {1}", skill.Name, skill.Fixed );

            #region Mob my Magius(CHE): Real PolRawPointsEngine
            var pMob = from as Midgard2PlayerMobile;
            if( pMob != null )
            {
                if( pMob.PolSkillRawGain( skill.SkillName, amObj, chance, success ) )
                {
                    return success; //PolRawPointsEngine Override other advancements :-)
                }
            }

            #endregion

            // chance is lineary dependant of the difficulty
            // so raw points must be scaled
            int rolled = IsCombatSkill( skill.SkillName ) ? DefaultCombatSkillAdv.Roll() : DefaultSkillAdv.Roll();
            int realPoints = (int)( rolled * ( 1 - chance ) );
            int oldPoints = RawPointsCurve[ skill.Fixed ];
            int newPoints = RawPointsCurve[ skill.Fixed + 1 ];

            // gc is the chance (between 0 and 1) to get the gain
            // gc * num_of_repeats * ( newPoints - oldPoints ) 
            // is the area under gain curve for a give 0.1 skill interval
            double gc = realPoints / (double)( newPoints - oldPoints );

            // racial bonus is a double value from 0 (0%) to 1 (100%)
            if( Races.Config.RaceGainFactorBonusEnabled )
            {
                if( from.Race != Race.Human && from.Race is MidgardRace )
                    gc += ( (MidgardRace)from.Race ).GetGainFactorBonuses( skill );
            }

            // be sure not to ger out of acceptable max values of gc for that skill
            double maxChance = SkillGainFactorHelper.GetMaxChanceToGain( skill.Base );
            if( gc > maxChance )
                gc = maxChance;

            // be sure not to ger out of acceptable min values of gc for that skill
            double minChance = SkillGainFactorHelper.MinChanceToGain;
            if( gc < minChance )
                gc = minChance;

            if( from is BaseCreature && ( (BaseCreature)from ).Controlled )
                gc *= 2;

            double customGainFactor = Config.Enabled ? SkillGainFactorHelper.GetCustomGainFactor( from, skill ) : skill.Info.GainFactor;
            gc *= customGainFactor;

            if( SendLog && from.PlayerDebug )
            {
                from.SendMessage( "mc: {0:F3} - Mc {1:F3} - custom {2:F3} - final chance: {3:F3}", minChance, maxChance, customGainFactor, gc );
                from.SendMessage( "chance: {0:F3}, oldPoints: {1}, newPoints {2}, gc {3:F4}", chance, oldPoints, newPoints, gc );
            }

            double random = Utility.RandomDouble();

            if( SendLog && from.PlayerDebug )
                from.SendMessage( "random: {0:F3}, final gc: {1:F3}, success {2}", random, gc, random <= gc );

            bool shoudlGain = Config.GuaranteedGainSystemEnabled && GuaranteedGainSystem.ForceSkillGain( from, skill );

            if( ( ( gc >= random && SkillCheck.AllowGain( from, skill, amObj ) ) || skill.Base < 10.0 || shoudlGain ) )
                SkillCheck.Gain( from, skill );

            return success;
        }

        protected static bool SendLog = true;

        private static readonly DiceRoll DefaultSkillAdv = new DiceRoll( "4d100" );
        private static readonly DiceRoll DefaultCombatSkillAdv = new DiceRoll( "4d100" );
        // private static int DefaultPassiveRawPointsForSuccess = DefaultSkillAdv / 6;

        public static int[] RawPointsCurve;

        private static readonly SkillName[] m_CombatSkills = new SkillName[]
	                                                {
                                                        SkillName.Swords,
                                                        SkillName.Archery,
                                                        SkillName.Macing,
                                                        SkillName.Fencing,
                                                        SkillName.Wrestling
	                                                };

        private static bool IsCombatSkill( SkillName skillName )
        {
            return Array.IndexOf( m_CombatSkills, skillName ) > -1;
        }

        public static int BaseSkillToRawSkill( int skillFixedValue )
        {
            if( skillFixedValue <= 200 )
            {
                double rawValue = skillFixedValue * 20.48;
                if( rawValue > (int)rawValue )
                    return (int)( rawValue + 1 );
                else
                    return (int)rawValue;
            }
            else
            {
                double scale = skillFixedValue / 100.0;
                double remain = scale - (int)( scale );
                return (int)( Math.Pow( 2, (int)( scale ) ) * ( 1024 * ( 1.0 + remain ) + 1 ) );
            }
        }
    }
}
