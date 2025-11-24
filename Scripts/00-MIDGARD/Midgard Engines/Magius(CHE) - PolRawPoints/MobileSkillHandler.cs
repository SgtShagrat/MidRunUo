using System;
using System.Collections.Generic;

using Midgard.Engines.SkillSystem;

using Server;
using Server.Engines.Quests;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
using Server.Regions;

using Core = Midgard.Engines.SkillSystem.Core;

namespace Midgard.Engines.PolRawPoints
{
    public class MobileSkillHandler : IDisposable
    {
        public PlayerMobile Mobile { get; private set; }

        #region RPInfo (RarPointsInfo class)
        private class RPInfo
        {
            public int LastSkillBaseFixedPoint { get; set; }

            public uint ActualRawPoints { get; set; }

            private SkillName Skill { get; set; }

            private PlayerMobile Mobile { get; set; }

            public RPInfo( PlayerMobile mobile, SkillName skill, int lastSkillBaseFixedPoint )
                : this( mobile, skill, lastSkillBaseFixedPoint, FromBaseFixedPoints2Raw( lastSkillBaseFixedPoint ) )
            {
            }

            public RPInfo( PlayerMobile mobile, SkillName skill, int lastSkillBaseFixedPoint, uint actualRowPoints )
            {
                Mobile = mobile;
                Skill = skill;
                LastSkillBaseFixedPoint = lastSkillBaseFixedPoint;
                ActualRawPoints = actualRowPoints;
            }

            /// <summary>
            ///     If someone like GMCommand setskill changes skillvalue, rawpoints will be realligned! :-)
            /// </summary>
            /// <returns>
            ///     A <see cref = "System.Boolean" />
            ///     returns true if rawpoints are realligned!
            /// </returns>
            public bool EnsureConistancy()
            {
                int act = FromRawPoints2BaseFixed( ActualRawPoints );

                if( LastSkillBaseFixedPoint != act )
                {
                    if( Core.RawPointsCurve != null )
                    {
                        if( LastSkillBaseFixedPoint > Core.RawPointsCurve.Length - 1 )
                        {
                            int old = LastSkillBaseFixedPoint;
                            LastSkillBaseFixedPoint = Core.RawPointsCurve.Length - 1;

                            if( Mobile != null && Mobile.Skills != null )
                            {
                                Mobile.Skills[ Skill ].BaseFixedPoint = LastSkillBaseFixedPoint;
                                //ActualRawPoints = FromBaseFixedPoints2Raw ( LastSkillBaseFixedPoint );

                                Config.Pkg.LogWarningLine( "MobileSkillHandler.EnsureConistancy [IMPRESSIVE] Mobile={0}, Skill={1}, PreBase={2}, NewBase={3}", Mobile.Name ?? "-no name-", Skill, old, LastSkillBaseFixedPoint );
                            }
                        }
                        else
                            Config.Pkg.LogWarningLine( "MobileSkillHandler.EnsureConistancy Mobile={0}, Skill={1}, PreBase={2}, NewBase={3}", Mobile.Name ?? "-no name-", Skill, LastSkillBaseFixedPoint, act );
                    }

                    ActualRawPoints = FromBaseFixedPoints2Raw( LastSkillBaseFixedPoint );
                    return true;
                }

                if( Mobile != null && Mobile.Skills != null && Mobile.Skills[ Skill ].BaseFixedPoint != act )
                {
                    Config.Pkg.LogWarningLine( "MobileSkillHandler.EnsureConistancy Mobile={0}, Skill={1}, PreBase={2}, NewBase={3}", Mobile.Name ?? "-no name-", Skill, LastSkillBaseFixedPoint, act );
                    LastSkillBaseFixedPoint = Mobile.Skills[ Skill ].BaseFixedPoint;
                    ActualRawPoints = FromBaseFixedPoints2Raw( LastSkillBaseFixedPoint );

                    return true;
                }

                return false;
            }
        }
        #endregion

        private readonly Dictionary<SkillName, RPInfo> RawPoints;

        /// <summary>
        ///     Initialize rowpoints system for the first time.
        /// </summary>
        /// <param name = "mobile">
        ///     A <see cref = "PlayerMobile" />
        /// </param>
        public MobileSkillHandler( PlayerMobile mobile )
        {
            Mobile = mobile;
            RawPoints = new Dictionary<SkillName, RPInfo>();

            foreach( Skill skill in Mobile.Skills )
            {
                /*
                if( skill == null )
                {
                    int a = 1;
                }
                else 
                */
                    RawPoints.Add( skill.SkillName, new RPInfo( mobile, skill.SkillName, skill.BaseFixedPoint ) );
            }

            Config.Pkg.LogInfoLine( "MobileSkillHandler Activated on Player \"{0}\"", mobile.Name );
        }

        private string Message( string varname, params object[] formatargs )
        {
            return Config.Message( Mobile, varname, formatargs );
        }

        public bool SkillRawGain( SkillName skill, object amObj, double lastChanceCause, bool lastSuccess )
        {
            return SkillRawGain( skill, amObj, lastChanceCause, lastSuccess, Config.DefaultRawAdvancement );
        }

        public bool SkillRawGain( SkillName skill, object amObj, double lastChanceCause, bool lastSuccess, uint baseAdvance )
        {
            var mobSkill = Mobile.Skills[ skill ];

            bool shoudlGain = SkillSystem.Config.GuaranteedGainSystemEnabled && GuaranteedGainSystem.ForceSkillGain( Mobile, mobSkill );

            /*too easy too difficult*/
            if( lastChanceCause < 0 )
            {
                Mobile.SendMessage( 32, Message( "SkilltooDifficult", mobSkill.Name ) );
                return false;
            }
            if( lastChanceCause >= 1 )
            {
                Mobile.SendMessage( 32, Message( "SkilltooEasy", mobSkill.Name ) );
                return false;
            }

            if( ( SkillCheck.AllowGain( Mobile, mobSkill, amObj ) || mobSkill.Base < 10.0 || shoudlGain ) && CanGain( mobSkill ) )
            {
                var advpartRandom = baseAdvance * Config.RandomizedAdvancementPart; //random advancement part
                var advpartMain = baseAdvance - advpartRandom; //secure (fixed) advancement part

                var gf = Utility.RandomDouble();
                if( gf == 0 )
                    gf = 1.0f; //prevent side effects: never be 1.0!
                advpartRandom *= gf;

                var advpart_success = baseAdvance * Config.SuccessBonus; //bonus if success (based on DefaultRawAdvancement)
                var advpart_fail = baseAdvance * Config.FailBonus; //bonus if fail (based on DefaultRawAdvancement)

                var advancement = (uint)Math.Ceiling( advpartMain + advpartRandom + advpart_success + advpart_fail );

                /*modification by chance*/
                advancement = (uint)Math.Ceiling( advancement * ( 1.0 - lastChanceCause ) );

                /*modification by custom gain factors*/
                var gaincustomscale = SkillGainFactorHelper.GetCustomGainFactor( Mobile, mobSkill );
                advancement = (uint)Math.Ceiling( advancement * ( gaincustomscale ) );

                double ingamebonusses = 1.0;

                #region Mondain's Legacy
                if( QuestHelper.EnhancedSkill( Mobile, mobSkill ) )
                    ingamebonusses *= ( ( Utility.RandomDouble() * 2 ) + 2 ); // from x2 to x4
                #endregion

                #region Scroll of Alacrity
                if( skill == Mobile.AcceleratedSkill && Mobile.AcceleratedStart > DateTime.Now )
                    ingamebonusses *= ( ( Utility.RandomDouble() * 3 ) + 2 ); // from x2 to x5
                #endregion

                if( ingamebonusses != 1.0 )
                    advancement = (uint)Math.Ceiling( advancement * ingamebonusses );

                bool baseskilladvance = GainRaw( skill, advancement );

                return true; /*gain completed*/
            }
            return false; /*nothing to gain*/
        }

        public uint RawOf( SkillName skillname )
        {
            var info = GetRPInfo( skillname, true );
            try
            {
                info.EnsureConistancy();
            }
            catch( Exception e )
            {
                SkillSystem.Config.Pkg.LogErrorLine( e.ToString() );
            }
            return info.ActualRawPoints;
        }

        /// <summary>
        ///     Adds rawPoints to skill. If FromRawPoints2BaseFixed returns a different skill value from before skillbase will be modifified.
        /// </summary>
        /// <param name = "skilltogain">
        ///     A <see cref = "SkillName" />
        /// </param>
        /// <param name = "rawPoints">
        ///     A <see cref = "System.UInt32" />
        /// </param>
        /// <returns>
        ///     A <see cref = "System.Boolean" />
        ///     True if skillbase is modified.
        /// </returns>
        public bool GainRaw( SkillName skilltogain, uint rawPoints )
        {
            var info = GetRPInfo( skilltogain, true ); //ensure creation

            try
            {
                info.EnsureConistancy();
            }
            catch( Exception e )
            {
                SkillSystem.Config.Pkg.LogErrorLine( e.ToString() );
            }

            if( Mobile.PlayerDebug )
                Mobile.SendMessage( "PolSkill.GainRaw:: ActualRaw={0}, Adv={1}, Final={2}", info.ActualRawPoints, rawPoints, info.ActualRawPoints + rawPoints );

            var prefized = FromRawPoints2BaseFixed( info.ActualRawPoints );

            var oldrarpoints = info.ActualRawPoints;

            info.ActualRawPoints += rawPoints;

            var postfixed = FromRawPoints2BaseFixed( info.ActualRawPoints );

            var rangefixed = postfixed - prefized;

            if( rangefixed > 0 )
            {
                /*GainFixed method does not update rawpoints...*/
                //GAIN NOW!!!!
                if( !GainFixed( Mobile.Skills[ skilltogain ], rangefixed ) )
                {
                    //SKILLCAP
                    //DebugOverheadMessage("PolSkillRawGain:: Skill does not advance. You reach skillcap?");
                    Config.Pkg.LogWarningLine( "PolSkillRawGain:: Skill does not advance. You reach skillcap? Mobile={0}, Skill={1}, PreBase={2}, NewBase={3}", Mobile.Name, skilltogain, prefized, postfixed );

                    info.ActualRawPoints = oldrarpoints; //rollback :-)
                    return false;
                }
                TraceAdvancedRP( skilltogain, rawPoints );
                return true;
            }
            else
                TraceAdvancedRP( skilltogain, rawPoints );
            return false;
        }

        public void TraceAdvancedRP( SkillName skilltogain, uint addedRawPoints )
        {
            var m2P = Mobile as Midgard2PlayerMobile;

            if( m2P == null || !m2P.NotificationPolRawPointsEnabled )
                return;

            var info = GetRPInfo( skilltogain, true );

            var nextstep = FromBaseFixedPoints2Raw( info.LastSkillBaseFixedPoint + 1 );
            var prestep = FromBaseFixedPoints2Raw( info.LastSkillBaseFixedPoint );

            var range = nextstep - prestep;
            var per = ( info.ActualRawPoints - prestep ) / (double)( range );

            Mobile.SendMessage( 65, "{0} {1}{2} rp, {3:P1} to {4:P1}", Mobile.Skills[ skilltogain ].Name, addedRawPoints >= 0 ? "+" : "", addedRawPoints, per, ( Mobile.Skills[ skilltogain ].Fixed + 1 ) / 1000.0 );
        }

        public void DebugOverheadMessage( string format, params object[] args )
        {
            if( Mobile.PlayerDebug || Config.Debug )
                Mobile.PrivateOverheadMessage( MessageType.Regular, 37, true, string.Format( format, args ), Mobile.NetState );
        }

        /// <summary>
        ///     This method add fixed value to skillbase. If cap causes a skilldown, those skills will be updated in rawpoints array.
        ///     Note: this skill does not update rawpoints of main modified skill.
        /// </summary>
        /// <param name = "skillToGain">
        ///     A <see cref = "Skill" />
        /// </param>
        /// <param name = "toGain">
        ///     A <see cref = "System.Int32" />
        /// </param>
        /// <returns>
        ///     A <see cref = "System.Boolean" />
        /// </returns>
        public bool GainFixed( Skill skillToGain, int toGain )
        {
            Skills skills = Mobile.Skills;

            if( ( skills.Total / skills.Cap ) >= Utility.RandomDouble() && skills.Total >= skills.Cap ) // mod by Dies Irae
            {
                for( int i = 0; i < skills.Length; ++i )
                {
                    Skill toLower = skills[ i ];

                    if( toLower != skillToGain && toLower.Lock == SkillLock.Down && toLower.BaseFixedPoint >= toGain )
                    {
                        SetSkill( toLower.SkillName, toLower.BaseFixedPoint - toGain, true );
                        break;
                    }
                }
            }

            if( skills.Total + toGain > skills.Cap )
                toGain = Math.Max( 0, skills.Cap - skills.Total );

            if( skills.Total + toGain <= skills.Cap )
            {
                SetSkill( skillToGain.SkillName, skillToGain.BaseFixedPoint + toGain, false );

                #region mod by Dies Irae
                if( SkillSystem.Config.GuaranteedGainSystemEnabled )
                    GuaranteedGainSystem.RegisterSkillGain( Mobile, skillToGain );
                #endregion

                return true;
            }
            return false;
        }

        private RPInfo GetRPInfo( SkillName skillname, bool create )
        {
            if( !RawPoints.ContainsKey( skillname ) )
            {
                if( create )
                    RawPoints.Add( skillname, new RPInfo( Mobile, skillname, Mobile.Skills[ skillname ].BaseFixedPoint ) );
                else
                    return null;
            }
            return RawPoints[ skillname ];
        }

        public void SetSkill( SkillName skillname, int newvalue, bool updateRawValues )
        {
            var info = GetRPInfo( skillname, true );
            info.LastSkillBaseFixedPoint = newvalue;
            if( updateRawValues )
                info.ActualRawPoints = FromBaseFixedPoints2Raw( newvalue );

            var skobj = Mobile.Skills[ skillname ];

            var oldvalue = skobj.BaseFixedPoint;
            skobj.BaseFixedPoint = newvalue;
            var range = ( newvalue - oldvalue ) / 10.0;

            /*Config.Pkg.LogInfo("PolSkillRawGain:: Mobile={3}, SetSkill={0}, NewValue={1}, Raw={2}", skillname, newvalue, info.ActualRowPoints,Mobile.Name);
            Mobile.SendMessage(12,"Your \"{0}\" changed into {1}% ({2}{3}%)", skobj.Name, skobj.Base, range>=0 ? "+" : "", range) ;*/
        }

        public bool CanGain( Skill skillToGain )
        {
            //Copied from Server.Misc.SkillCheck.Gain
            if( Mobile.Region.IsPartOf( typeof( Jail ) ) )
                return false;

            //if( skillToGain.SkillName == SkillName.Focus && Mobile is BaseCreature )
            //    return false;

            if( ( SkillSubCap.IsUnderAnySubCap( skillToGain.SkillName ) && !SkillSubCap.CanGainUnderSubCaps( Mobile, skillToGain.SkillName ) ) )
                return false;

            if( skillToGain.Base < skillToGain.Cap && skillToGain.Lock == SkillLock.Up )
                return ( SkillSystem.Config.RateOverTimeEnabled ? RateOverTime.SkillGainAllowed( Mobile, skillToGain ) : true ); // mod by Dies Irae				
            return false;
        }

        #region Serialization
        /// <summary>
        ///     Deserialization constructor
        /// </summary>
        /// <param name = "mobile">
        ///     A <see cref = "PlayerMobile" />
        /// </param>
        /// <param name = "reader">
        ///     A <see cref = "GenericReader" />
        /// </param>
        public MobileSkillHandler( PlayerMobile mobile, GenericReader reader )
        {
            Mobile = mobile;
            RawPoints = new Dictionary<SkillName, RPInfo>();

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        var tot = reader.ReadByte();
                        for( var i = 0; i < tot; i++ )
                        {
                            var skname = (SkillName)reader.ReadByte();
                            var info = new RPInfo( mobile, skname, reader.ReadUShort(), reader.ReadUInt() );
                            try
                            {
                                info.EnsureConistancy();
                            }
                            catch( Exception e )
                            {
                                SkillSystem.Config.Pkg.LogErrorLine( e.ToString() );
                            }
                            RawPoints.Add( skname, info );
                        }
                    }
                    break;
            }
        }

        public void Serialize( GenericWriter writer )
        {
            writer.Write( 0 ); // version

            writer.Write( (byte)RawPoints.Count );
            foreach( KeyValuePair<SkillName, RPInfo> elem in RawPoints )
            {
                try
                {
                    elem.Value.EnsureConistancy();
                }
                catch( Exception e )
                {
                    SkillSystem.Config.Pkg.LogErrorLine( e.ToString() );
                }
                elem.Value.EnsureConistancy();
                writer.Write( (byte)elem.Key );
                writer.Write( (ushort)elem.Value.LastSkillBaseFixedPoint );
                writer.Write( elem.Value.ActualRawPoints );
            }
        }
        #endregion

        #region Utility
        public static uint FromBaseFixedPoints2Raw( int baseFixedPoints )
        {
            return (uint)Core.BaseSkillToRawSkill( baseFixedPoints );
        }

        public static int FromRawPoints2BaseFixed( int rawPoints )
        {
            return FromRawPoints2BaseFixed( (uint)rawPoints );
        }

        public static int FromRawPoints2BaseFixed( uint rawPoints )
        {
            //perform a QuickFind to increase performance
            var arr = Core.RawPointsCurve;
            return QuickFindSimilar( arr, 0, arr.Length, (int)rawPoints ); // Mismatch!!!! Rawpoints must be UINT!
        }

        /// <summary>
        ///     Perform a quickfind and return array index of the matched entry.
        ///     Nota: rotorna il numero più piccolo più vicino a quello passato.
        /// </summary>
        /// <returns>
        ///     A <see cref = "System.Int32" />
        /// </returns>
        public static int QuickFindSimilar( IList<int> array, int index, int length, int tofind )
        {
            if( length <= 0 )
                return -1; //not found
            if( length == 1 )
                return index;
            var middleidx = (int)Math.Floor( length / 2.0 ) + index;
            var value = array[ middleidx ];
            if( value == tofind )
                return middleidx;
            if( tofind < value )
                return QuickFindSimilar( array, index, middleidx - index, tofind );
            return QuickFindSimilar( array, middleidx, (int)Math.Ceiling( length / 2.0 ), tofind ); //Ceiling is important.. or use Floor + 1
        }
        #endregion

        #region IDisposable implementation
        public virtual void Dispose()
        {
            RawPoints.Clear();
        }
        #endregion
    }
}