using System;

using Server;
using Server.Items;
using Server.SkillHandlers;

using MidgardClasses = Midgard.Engines.Classes.Classes;

namespace Midgard.Engines.Classes
{
    public sealed class ThiefSystem : ClassSystem
    {
        public ThiefSystem()
        {
            Definition = new ClassDefinition( "Thief",
                                                MidgardClasses.Thief,
                                                0,
                                                DefaultWelcomeMessage,
                                                null
            );

            Hiding.CombatOverride = false;
            Config.Pkg.LogInfoLine( "Hiding combat override: {0}", Hiding.CombatOverride );
        }

        public override bool IsEligible( Mobile mob )
        {
            // Fairy
            if( mob.Race == Races.Core.FairyOfAir )
                return false;
            else if( mob.Race == Races.Core.FairyOfEarth )
                return false;
            else if( mob.Race == Races.Core.FairyOfFire )
                return false;
            else if( mob.Race == Races.Core.FairyOfWater )
                return false;
            else if( mob.Race == Races.Core.FairyOfWood )
                return false;

            return base.IsEligible( mob );
        }

        public override string IsEligibleString( Mobile mob )
        {
            if( mob.Race == Races.Core.FairyOfAir )
                return (mob.Language == "ITA" ? "Sei una Fata, non potrai mai diventare Ladro." : "You're a Fairy, you cannot become a Thief.");
            else if( mob.Race == Races.Core.FairyOfEarth )
                return (mob.Language == "ITA" ? "Sei una Fata, non potrai mai diventare Ladro." : "You're a Fairy, you cannot become a Thief.");
            else if( mob.Race == Races.Core.FairyOfFire )
                return (mob.Language == "ITA" ? "Sei una Fata, non potrai mai diventare Ladro." : "You're a Fairy, you cannot become a Thief.");
            else if( mob.Race == Races.Core.FairyOfWater )
                return (mob.Language == "ITA" ? "Sei una Fata, non potrai mai diventare Ladro." : "You're a Fairy, you cannot become a Thief.");
            else if( mob.Race == Races.Core.FairyOfWood )
                return (mob.Language == "ITA" ? "Sei una Fata, non potrai mai diventare Ladro." : "You're a Fairy, you cannot become a Thief.");

            return base.IsEligibleString( mob );
        }

        public override void MakeRitual( Mobile ritualist, PowerDefinition definition )
        {
            // Thieves have no ritual until now...
        }

        public override bool ShowsTitle
        {
            get { return false; }
        }

        public static bool HandleNonLocalDrop( Mobile from, Item item, Item target )
        {
            if( from.AccessLevel >= AccessLevel.Counselor )
                return true;

            if( item.LootType != LootType.Blessed )
            {
                int maxWeight = from.Skills[ SkillName.Stealing ].Fixed / 100;
                int str = from.Str;

                if( str <= 80 )
                    maxWeight -= ( 80 - str ) / 10;
                else
                    maxWeight += ( str - 80 ) / 20;

                double w = item.Weight + item.TotalWeight;

                if( w >= maxWeight )
                {
                    from.SendMessage( "That is to heavy to place in that pack." );
                    return false;
                }
                else
                {
                    int iw = (int)Math.Ceiling( w );
                    iw *= 10;

                    bool success = from.CheckTargetSkill( SkillName.Stealing, item, iw - 22.5, iw + 27.5 );

                    if( success )
                        from.SendMessage( "You carefully placed that item without being noticed." );
                    else
                    {
                        from.SendMessage( "You failed to place that item into your target container." );
                        from.CriminalAction( false );
                    }

                    return success;
                }
            }
            else
            {
                from.SendMessage( "You cannot place blessed items in that pack!" );
                return false;
            }
        }

        public static bool NearbyMobilesMalusEnabled = false;

        public static double GetNearbyMobilesMalus( Mobile m )
        {
            if( !NearbyMobilesMalusEnabled )
                return 0;

            int malus = 0;
            const int radius = 8;

            foreach( Mobile nearby in m.GetMobilesInRange( radius ) )
            {
                if( !nearby.InLOS( m ) )
                    continue;

                int nearbyMalus = Math.Max( 0, (int)( radius - m.GetDistanceToSqrt( m ) ) );

                if( nearby.Player )
                    nearbyMalus *= 2;

                if( nearby.Combatant == m )
                    nearbyMalus *= 4;

                malus += nearbyMalus;
            }

            return malus / 100.0;
        }

        public static bool StealthHpMalusEnabled = true;

        public static int StealthHpMalus( Mobile m )
        {
            if( !StealthHpMalusEnabled )
                return 0;

            int percHealth = 100 - ( m.Hits * 100 ) / m.HitsMax;

            if( percHealth < 10 )
                return 0;
            else
                return 1 + ( 5 * ( ( percHealth - 10 ) / 90 ) );

            /* POL code
            var malushp:=0;
            var norestealth:=0;

            var diffhp:= 100 - cint(chr.hp*100/chr.maxhp);

            if (diffhp>=10 and diffhp<20)
	            malushp:=-1;
            elseif (diffhp>=20 and diffhp<30)
	            malushp:=-2;
            elseif (diffhp>=30 and diffhp<40)
	            malushp:=-3;
            elseif (diffhp>=40 and diffhp<60)
	            malushp:=-4;
            elseif (diffhp>=60 and diffhp<80)
	            malushp:=-5;
	            norestealth:=1;
            elseif (diffhp>=80 and diffhp<=100)
	            malushp:=-6;
	            norestealth:=1;
            endif
            */
        }

        public static bool HasAnyArmor( Mobile from )
        {
            return HasArmor( from, true, true, true, true );
        }

        public static bool HasMoreThenLeatherArmor( Mobile from )
        {
            return HasArmor( from, true, true, true, false );
        }

        public static bool HasArmor( Mobile from, bool checkPlate, bool checkStudded, bool checkBone, bool checkLeather )
        {
            foreach( Item item in from.Items )
            {
                if( !( item is BaseArmor ) )
                    continue;

                BaseArmor armor = (BaseArmor)item;

                if( checkPlate && armor.MaterialType == ArmorMaterialType.Plate )
                    return true;

                if( checkStudded && armor.MaterialType == ArmorMaterialType.Studded )
                    return true;

                if( checkBone && armor.MaterialType == ArmorMaterialType.Bone )
                    return true;

                if( checkLeather && armor.MaterialType == ArmorMaterialType.Leather )
                    return true;
            }

            return false;
        }

        public static bool StealthOnMountEnabled = true;

        public static bool HandleStealthOnMount( Mobile m, out TimeSpan result )
        {
            if( !StealthOnMountEnabled )
            {
                result = TimeSpan.FromSeconds( 1.0 );
                return false;
            }

            if( m.Mounted )
            {
                bool isGMArcher = m.Skills[ SkillName.Archery ].Base >= 100.0;
                bool isGMFencer = m.Skills[ SkillName.Fencing ].Base >= 100.0;

                if( !isGMArcher && !IsThief( m ) && !isGMFencer )
                {
                    if( HasAnyArmor( m ) )
                    {
                        m.SendMessage( "Thou cannot hope to move quietly while mounting and wearing this armor." );
                        m.RevealingAction();
                        result = TimeSpan.FromSeconds( 10.0 );
                        return true;
                    }
                }
                else if( HasMoreThenLeatherArmor( m ) )
                {
                    m.SendMessage( "Thou cannot hope to move quietly while mounting and wearing this armor." );
                    m.RevealingAction();
                    result = TimeSpan.FromSeconds( 10.0 );
                    return true;
                }
            }

            result = TimeSpan.FromSeconds( 1.0 );
            return false;
        }

        public static bool CheckCombat( Mobile m, int range )
        {
            if( m != null && m.Combatant != null && !m.Combatant.Deleted && m.Combatant.Alive && m.CanSee( m.Combatant ) && m.InRange( m.Combatant, (int)( range * 1.5 ) ) && m.Combatant.InLOS( m ) )
            {
                if( m.PlayerDebug )
                    m.SendMessage( "ThiefSystem CheckCombat: range {0} - thief combatant ({1}) in range.", range, m.Combatant.Name ?? "unnamed" );
                return true;
            }

            IPooledEnumerable eable = m.GetMobilesInRange( range );
            foreach( Mobile check in eable )
            {
                if( check.Combatant != m || !check.InLOS( m ) )
                    continue;

                eable.Free();

                if( m.PlayerDebug )
                    m.SendMessage( "ThiefSystem CheckCombat: range {0} - mobile targeting thief ({1}) in range.", range, m.Combatant.Name ?? "unnamed" );

                return true;
            }

            if( m.PlayerDebug )
                m.SendMessage( "ThiefSystem CheckCombat: range {0} - check passed.", range );

            eable.Free();
            return false;
        }
    }
}