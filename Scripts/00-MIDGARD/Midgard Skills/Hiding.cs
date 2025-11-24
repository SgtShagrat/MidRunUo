/***************************************************************************
 *                               Hiding.cs
 *
 *   begin                : 01 ottobre 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Midgard.Engines.Classes;
using Midgard.Items;

using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Network;
using Server.Multis;

namespace Midgard.SkillHandlers
{
    namespace Server.SkillHandlers
    {
        public class Hiding
        {
            public static void Initialize()
            {
                SkillInfo.Table[ (int)SkillName.Hiding ].Callback = new SkillUseCallback( OnUse );
            }

            private static bool AllowHideIfFrozen = false;
            private static bool AllowHideDuringCast = false;
            private static bool DisruptCancelOnUse = true;

            private const double SkillReqToHideMounted = 80.0;
            private const double DelayPostDetectionToReHide = 10.0;

            private const double MinCheckCombatMalus = 119.9;//95.0; edit by Arlas
            private const double MaxCheckCombatMalus = 40.0;
            private const int CombatRange = 13;
            private const int MinCombatRangeToHide = 5;

            private const double MinBaseSkill = -20.0;
            private const double MaxBaseSkill = 80.0;

            private const int MaxArmorRatingAllowed = 42;

            private static void LocalOverheadMessage( Mobile m, string message )
            {
                LocalOverheadMessage( m, 0x22, message );
            }

            private static void LocalOverheadMessage( Mobile m, int hue, string message )
            {
                m.LocalOverheadMessage( MessageType.Regular, hue, true, message );
            }

            private static int GetHouseBonus( Mobile m )
            {
                BaseHouse house = BaseHouse.FindHouseAt( m );

                if( house != null && house.IsFriend( m ) )
                    return 100;
                else
                {
                    if( house == null )
                        house = BaseHouse.FindHouseAt( new Point3D( m.X - 1, m.Y, 127 ), m.Map, 16 );

                    if( house == null )
                        house = BaseHouse.FindHouseAt( new Point3D( m.X + 1, m.Y, 127 ), m.Map, 16 );

                    if( house == null )
                        house = BaseHouse.FindHouseAt( new Point3D( m.X, m.Y - 1, 127 ), m.Map, 16 );

                    if( house == null )
                        house = BaseHouse.FindHouseAt( new Point3D( m.X, m.Y + 1, 127 ), m.Map, 16 );

                    if( house != null )
                        return 50;
                }

                return 0;
            }

            public static TimeSpan OnUse( Mobile m )
            {
                if( m.Spell != null && !AllowHideDuringCast )
                {
                    m.SendLocalizedMessage( 501238 ); // You are busy doing something else and cannot hide.
                    m.RevealingAction( true );
                    return TimeSpan.FromSeconds( 1.0 );
                }
                else if( ( m.Frozen || m.Paralyzed ) && !AllowHideIfFrozen )
                {
                    LocalOverheadMessage( m, "You can't seem to hide right now." );
                    m.RevealingAction( true );
                    return TimeSpan.FromSeconds( 1.0 );
                }
                else if( m is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)m ).LastHideDetectionTime + TimeSpan.FromSeconds( DelayPostDetectionToReHide ) > DateTime.Now )
                {
                    LocalOverheadMessage( m, "You have been detected recently and cannot hide right now!" );
                    m.RevealingAction( true );
                    return TimeSpan.FromSeconds( 1.0 );
                }
                else if( m is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)m ).LastHideDetector != null && m.InLOS( ( (Midgard2PlayerMobile)m ).LastHideDetector ) )
                {
                    LocalOverheadMessage( m, "You have been detected recently your detector is near!" );
                    m.RevealingAction( true );
                    return TimeSpan.FromSeconds( 1.0 );
                }
                else if( m.Mounted && m.Skills[ SkillName.Hiding ].Value < SkillReqToHideMounted )
                {
                    LocalOverheadMessage( m, "You are too unexperted to hide while mounted." );
                    m.RevealingAction( true );
                    return TimeSpan.FromSeconds( 1.0 );
                }
                else
                {
                    int armorRating = Stealth.GetArmorRating( m );

                    if( armorRating >= MaxArmorRatingAllowed )
                    {
                        LocalOverheadMessage( m, "You could not hope to move quietly wearing this much armor." );
                        m.RevealingAction( true );
                        return TimeSpan.FromSeconds( 1.0 );
                    }

                    if( m.Target != null && DisruptCancelOnUse )
                        Target.Cancel( m );

                    double bonus = GetHouseBonus( m ) / 100.0;
                    double malus = ThiefSystem.GetNearbyMobilesMalus( m );

                    if( ClassSystem.IsScout( m ) && ScoutMimeticPaint.IsUnderMimetism( m ) && ScoutSystem.IsInForest( m ) )
                        bonus = 1.0;

                    double value = m.Skills[ SkillName.Hiding ].Value;

                    int combatRange = Math.Max( (int)( CombatRange - ( value / 10 ) ), MinCombatRangeToHide );
                    bool oldCheckCombat = ThiefSystem.CheckCombat( m, combatRange );

                    if( m.PlayerDebug )
                        m.SendMessage( "Hiding debug: check combat: range {0} - check passed {1}", combatRange, oldCheckCombat );

                    double minSkill = MinBaseSkill + ( armorRating * 2 ) + ( oldCheckCombat ? MinCheckCombatMalus : 0 );
                    double maxSkill = MaxBaseSkill + ( armorRating * 2 ) + ( oldCheckCombat ? MaxCheckCombatMalus : 0 );

                    double chance = ( ( value - minSkill ) / ( maxSkill - minSkill ) ) - malus + bonus;

                    bool ok = m.CheckSkill( SkillName.Hiding, chance );

                    if( m.PlayerDebug )
                        m.SendMessage( "Hiding debug: malus: {0:F2} - bonus: {1:F2} - value {2:F1} - chance {3:F2} - success {4}.", malus, bonus, value, chance, ok );

                    if( !ok )
                    {
                        m.RevealingAction( true );
                        LocalOverheadMessage( m, "You can't seem to hide here." );
                    }
                    else
                    {
                        m.Hidden = true;
                        m.Warmode = false;
                        LocalOverheadMessage( m, 0x1F4, "You have hidden yourself well." );
                    }

                    return TimeSpan.FromSeconds( 10.0 );
                }
            }
        }
    }
}