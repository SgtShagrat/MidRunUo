/***************************************************************************
 *                               Stealth.cs
 *
 *   begin                : 01 ottobre 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Midgard.Engines.Classes;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Midgard.SkillHandlers
{
    public class Stealth
    {
        public static void Initialize()
        {
            SkillInfo.Table[ (int)SkillName.Stealth ].Callback = new SkillUseCallback( OnUse );
        }

        private const double DelayPostDetectionToReStealth = 40.0;

        private const double MinCheckCombatMalus = 119.9; //95.0; edit by Arlas
        private const double MaxCheckCombatMalus = 40.0;
        private const int CombatRange = 13;
        private const int MinCombatRangeToStealth = 5;

        private const double HidingRequiredToStealth = 80.0;

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

        public static TimeSpan OnUse( Mobile m )
        {
            if( m.Frozen || m.Paralyzed )
            {
                LocalOverheadMessage( m, "You can't seem to stealth here." );
                return TimeSpan.FromSeconds( 1.0 );
            }
            else if( m is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)m ).LastHideDetectionTime + TimeSpan.FromSeconds( DelayPostDetectionToReStealth ) > DateTime.Now )
            {
                LocalOverheadMessage( m, "You have been detected recently and cannot stealth right now!" );
                m.RevealingAction( true );
                return TimeSpan.FromSeconds( 1.0 );
            }
            else if( m.Skills[ SkillName.Hiding ].Base < HidingRequiredToStealth )
            {
                m.SendLocalizedMessage( 502726 ); // You are not hidden well enough.  Become better at hiding.
                m.RevealingAction( true );
                return TimeSpan.FromSeconds( 1.0 );
            }
            else if( !m.CanBeginAction( typeof( Stealth ) ) )
            {
                m.SendLocalizedMessage( 1063086 ); // You cannot use this skill right now.
                m.RevealingAction( true );
                return TimeSpan.FromSeconds( 1.0 );
            }
            else
            {
                if( !m.Hidden && !m.UseSkill( SkillName.Hiding ) )
                {
                    m.LocalOverheadMessage( MessageType.Regular, 0x22, 501237 ); // You can't seem to hide right now.
                    return TimeSpan.FromSeconds( 10.0 );
                }

                TimeSpan mountResult;
                if( ThiefSystem.HandleStealthOnMount( m, out mountResult ) )
                    return mountResult;

                int armorRating = GetArmorRating( m );

                if( armorRating >= MaxArmorRatingAllowed )
                {
                    LocalOverheadMessage( m, "You could not hope to move quietly wearing this much armor." );
                    m.RevealingAction( true );
                    return TimeSpan.FromSeconds( 1.0 );
                }

                double bonus = 0.0;
                double malus = ThiefSystem.GetNearbyMobilesMalus( m );

                double value = m.Skills[ SkillName.Stealth ].Value;

                int combatRange = Math.Max( (int)( CombatRange - ( value / 10 ) ), MinCombatRangeToStealth );
                bool oldCheckCombat = ThiefSystem.CheckCombat( m, combatRange );

                if( m.PlayerDebug )
                    m.SendMessage( "Stealth debug: check combat: range {0} - check passed {1}", combatRange, oldCheckCombat );

                double minSkill = MinBaseSkill + ( armorRating * 2 ) + ( oldCheckCombat ? MinCheckCombatMalus : 0 );
                double maxSkill = MaxBaseSkill + ( armorRating * 2 ) + ( oldCheckCombat ? MaxCheckCombatMalus : 0 );

                double chance = ( ( value - minSkill ) / ( maxSkill - minSkill ) ) - malus + bonus;

                bool ok = m.CheckSkill( SkillName.Stealth, chance );

                if( m.PlayerDebug )
                    m.SendMessage( "Stealth debug: malus: {0:F2} - bonus: {1:F2} - value {2:F1} - chance {3:F2} - success {4}.", malus, bonus, value, chance, ok );

                if( ok )
                {
                    int steps = (int)( m.Skills[ SkillName.Stealth ].Value / 10.0 );

                    steps -= ThiefSystem.StealthHpMalus( m );
                    if( steps < 1 )
                        steps = 1;

                    m.AllowedStealthSteps = steps;
                    PlayerMobile pm = m as PlayerMobile;
                    if( pm != null )
                        pm.IsStealthing = true;

                    m.SendLocalizedMessage( 502730 ); // You begin to move quietly.

                    return TimeSpan.FromSeconds( 10.0 );
                }
                else
                {
                    m.SendLocalizedMessage( 502731 ); // You fail in your attempt to move unnoticed.
                    m.RevealingAction( true );
                }
            }

            return TimeSpan.FromSeconds( 10.0 );
        }

        #region armor tabel
        public static int[ , ] ArmorTable { get { return m_ArmorTable; } }

        private static readonly int[ , ] m_ArmorTable = new int[ , ]
                                                            {
                                                                //	Gorget	Gloves	Helmet	Arms	Legs	Chest	Shield
                                                                /* Cloth	*/	{ 0,	0,		0,		0,		0,		0,		0 },
                                                                /* Leather	*/	{ 0,	0,		0,		0,		0,		0,		0 },
                                                                /* Studded	*/	{ 2,	2,		0,		4,		6,		10,		0 },
                                                                /* Bone		*/ 	{ 0,	5,		10,		10,		15,		25,		0 },
                                                                /* Spined	*/	{ 0,	0,		0,		0,		0,		0,		0 },
                                                                /* Horned	*/	{ 0,	0,		0,		0,		0,		0,		0 },
                                                                /* Barbed	*/	{ 0,	0,		0,		0,		0,		0,		0 },
                                                                /* Ring		*/	{ 0,	5,		0,		10,		15,		25,		0 },
                                                                /* Chain	*/	{ 0,	0,		10,		0,		15,		25,		0 },
                                                                /* Plate	*/	{ 5,	5,		10,		10,		15,		25,		0 },
                                                                /* Dragon	*/	{ 0,	5,		10,		10,		15,		25,		0 }
                                                            };
        #endregion

        public static int GetArmorRating( Mobile m )
        {
            int ar = 0;

            foreach( Item t in m.Items )
            {
                BaseArmor armor = t as BaseArmor;
                if( armor == null )
                    continue;

                int materialType = (int)armor.MaterialType;
                int bodyPosition = (int)armor.BodyPosition;

                if( materialType >= m_ArmorTable.GetLength( 0 ) || bodyPosition >= m_ArmorTable.GetLength( 1 ) )
                    continue;

                if( armor.ArmorAttributes.MageArmor == 0 )
                    ar += m_ArmorTable[ materialType, bodyPosition ];
            }

            return ar;
        }
    }
}