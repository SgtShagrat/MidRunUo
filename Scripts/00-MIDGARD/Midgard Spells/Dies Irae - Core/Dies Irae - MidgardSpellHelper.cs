/***************************************************************************
 *                               Dies Irae - MidgardSpellHelper.cs
 *
 *   begin                : 02 ottobre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Midgard.Engines.PVMAbsorbtions;

using Server;
using Server.Mobiles;
using Server.Regions;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Sixth;

using Core = Midgard.Engines.PVMAbsorbtions.Core;

namespace Midgard.Engines.SpellSystem
{
    public enum SpellType
    {
        Electric,       // elettrico
        Explosion,      // esplosivo
        Spectre,        // abbassa mana, recupera hp
        Sonic,          // abbassa stamina e mana
        Venom,          // poison
        Paralyze,       // paralisi
        Perforation,    // distruggi armature
        Vampire,        // curatore
        Fire,           // fuoco
        Mental,         // mentale
        Impact,         // magia da impatto
        General         // magia generale
    }

    public class MidgardSpellHelper
    {
        public static CustomResType GetCustomResFromSpellType( SpellType spellType )
        {
            switch( spellType )
            {
                case SpellType.Electric:
                    return CustomResType.Electric;
                case SpellType.Explosion:
                case SpellType.Fire:
                    return CustomResType.Fire;
                case SpellType.Venom:
                    return CustomResType.Venom;
                case SpellType.Spectre:
                case SpellType.Sonic:
                case SpellType.Mental:
                    return CustomResType.Mental;
                case SpellType.Impact:
                    return CustomResType.Impact;
                case SpellType.Paralyze:
                case SpellType.Perforation:
                case SpellType.Vampire:
                case SpellType.General:
                    return CustomResType.General;

                default:
                    return CustomResType.General;
            }
        }

        #region [methods from Spellhelper]
        /*
        public static void Damage( TimeSpan delay, Mobile target, double damage )
        {
            Damage( delay, target, null, damage );
        }

        public static void Damage( TimeSpan delay, Mobile target, Mobile from, double damage )
        {
            Damage( null, delay, target, from, damage );
        }

        public static void Damage( TimeSpan delay, Mobile target, double damage, SpellType type )
        {
            Damage( delay, target, null, damage, type );
        }
        */

        public static void Damage( TimeSpan delay, Mobile target, Mobile from, double damage, SpellType type )
        {
            Damage( null, delay, target, from, damage, type );
        }

        /*
        public static void Damage( Spell spell, Mobile target, double damage )
        {
            TimeSpan ts = SpellHelper.GetDamageDelayForSpell( spell );

            Damage( spell, ts, target, spell.Caster, damage );
        }
        
        public static void Damage( Spell spell, TimeSpan delay, Mobile target, Mobile from, double damage )
        {
            int iDamage = (int)damage;

            if( target is PlayerMobile && iDamage <= target.Hits )
            {
                if( iDamage > 30 )
                {
                    if( target.Female )
                        target.PlaySound( 807 );
                    else
                        target.PlaySound( 1080 );
                }
                else
                {
                    if( target.Female )
                        target.PlaySound( 814 );
                    else
                        target.PlaySound( 1088 );
                }
            }

            if( delay == TimeSpan.Zero )
            {
                if( from is BaseCreature )
                    ( (BaseCreature)from ).AlterSpellDamageTo( target, ref iDamage );

                if( target is BaseCreature )
                    ( (BaseCreature)target ).AlterSpellDamageFrom( from, ref iDamage );

                target.Damage( iDamage, from );
            }
            else
            {
                new SpellDamageTimer( spell, target, from, iDamage, delay ).Start();
            }

            if( target is BaseCreature && from != null && delay == TimeSpan.Zero )
                ( (BaseCreature)target ).OnDamagedBySpell( from );
        }
        */

        public static void Damage( Spell spell, Mobile target, double damage, SpellType type )
        {
            TimeSpan ts = SpellHelper.GetDamageDelayForSpell( spell );

            Damage( spell, ts, target, spell.Caster, damage, type );
        }

        private static void Damage( Spell spell, TimeSpan delay, Mobile target, Mobile from, double damage, SpellType type )
        {
            if( target == null || target.Deleted )
                return;

            int iDamage = (int)damage;

            if( from is BaseCreature && target.Player )
            {
                BaseCreature bc = ( (BaseCreature)from );
                if( bc.Summoned && bc.SummonMaster != null && bc.SummonMaster.Player && iDamage > 20 )
                    iDamage = 15 + Utility.Dice( 1, 5, 0 );
            }

            if( delay == TimeSpan.Zero )
            {
                if( from != null )
                {
                    if( from is BaseCreature )
                        ( (BaseCreature)from ).AlterSpellDamageTo( target, ref iDamage );

                    if( target is BaseCreature )
                        ( (BaseCreature)target ).AlterSpellDamageFrom( from, ref iDamage );
                }

                // we do not ignore armor, we do not keep alive the target, we are not archer
                Damage( target, from, iDamage, false, GetCustomResFromSpellType( type ), false, false );

                if( spell != null )
                    target.Stam -= spell.GetFatigueBySpellDamage( target, iDamage );
            }
            else
            {
                new SpellDamageTimerSA( spell, target, from, iDamage, type, delay ).Start();
            }

            if( target is BaseCreature && from != null && delay == TimeSpan.Zero )
                ( (BaseCreature)target ).OnDamagedBySpell( from );
        }
        #endregion

        /*
        private class SpellDamageTimer : Timer
        {
            private readonly Mobile m_Target;
            private readonly Mobile m_From;
            private int m_Damage;
            private readonly Spell m_Spell;

            public SpellDamageTimer( Spell s, Mobile target, Mobile from, int damage, TimeSpan delay )
                : base( delay )
            {
                m_Target = target;
                m_From = from;
                m_Damage = damage;
                m_Spell = s;

                if( m_Spell != null && m_Spell.DelayedDamage && !m_Spell.DelayedDamageStacking )
                    m_Spell.StartDelayedDamageContext( target, this );

                Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                if( m_From is BaseCreature )
                    ( (BaseCreature)m_From ).AlterSpellDamageTo( m_Target, ref m_Damage );

                if( m_Target is BaseCreature )
                    ( (BaseCreature)m_Target ).AlterSpellDamageFrom( m_From, ref m_Damage );

                m_Target.Damage( m_Damage );
                if( m_Spell != null )
                    m_Spell.RemoveDelayedDamageContext( m_Target );
            }
        }
        */

        private class SpellDamageTimerSA : Timer
        {
            private readonly Mobile m_Target;
            private readonly Mobile m_From;
            private int m_Damage;
            private readonly Spell m_Spell;
            private readonly SpellType m_Type;

            public SpellDamageTimerSA( Spell s, Mobile target, Mobile from, int damage, SpellType type, TimeSpan delay )
                : base( delay )
            {
                m_Type = type;
                m_Target = target;
                m_From = from;
                m_Damage = damage;
                m_Spell = s;
                if( m_Spell != null && m_Spell.DelayedDamage && !m_Spell.DelayedDamageStacking )
                    m_Spell.StartDelayedDamageContext( target, this );

                Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                if( m_From is BaseCreature && m_Target != null )
                    ( (BaseCreature)m_From ).AlterSpellDamageTo( m_Target, ref m_Damage );

                if( m_Target is BaseCreature && m_From != null )
                    ( (BaseCreature)m_Target ).AlterSpellDamageFrom( m_From, ref m_Damage );

                Damage( m_Target, m_From, m_Damage, false, GetCustomResFromSpellType( m_Type ), false, false );

                if( m_Target is BaseCreature && m_From != null )
                    ( (BaseCreature)m_Target ).OnDamagedBySpell( m_From );

                if( m_Spell != null )
                    m_Spell.RemoveDelayedDamageContext( m_Target );
            }
        }

        #region [from AOS]
        /*
        public static int Damage( Mobile m, int damage, bool ignoreArmor, CustomResType type )
        {
            return Damage( m, null, damage, ignoreArmor, type );
        }

        public static int Damage( Mobile m, int damage, CustomResType type )
        {
            return Damage( m, null, damage, type );
        }
        */

        /// <summary>
        /// Used only by Firefield spell
        /// </summary>
        public static int Damage( Mobile m, Mobile from, int damage, CustomResType type )
        {
            return Damage( m, from, damage, false, type, false, false );
        }

        public static int Damage( Mobile m, Mobile from, int damage, bool ignoreArmor, CustomResType type )
        {
            return Damage( m, from, damage, ignoreArmor, type, false, false );
        }

        /*
        public static int Damage( Mobile m, Mobile from, int damage, CustomResType type, bool keepAlive )
        {
            return Damage( m, from, damage, false, type, keepAlive, false );
        }
        */

        public static int Damage( Mobile m, Mobile from, int damage, bool ignoreArmor, CustomResType type, bool keepAlive, bool archer )
        {
            if( m == null || m.Deleted || !m.Alive || damage <= 0 )
                return 0;

            if( type == CustomResType.Fire )
                MeerMage.StopEffect( m, true );

            int totalDamage = ScaleByCustomRes( damage, m, type );
            if( totalDamage < 1 )
                totalDamage = 1;

            if( m.Player && totalDamage > 90 )
                totalDamage = 85 + Utility.Random( 6 );

			totalDamage = Core.OnDealDamage(from, m, totalDamage, DamageTypes.Magical); //mod by magius(CHE)
			
			if( keepAlive && totalDamage > m.Hits )
                totalDamage = m.Hits;
			
			m.Damage( totalDamage, from );

            return totalDamage;
        }

        public static int ScaleByCustomRes( int input, Mobile m, CustomResType type )
        {
            return ( input * ( 100 - Math.Min( 10, m.GetCustomResistance( type ) ) ) ) / 100;
        }

        public static double ScaleByCustomRes( double input, Mobile m, CustomResType type )
        {
            return ( input * ( 100 - Math.Min( 10, m.GetCustomResistance( type ) ) ) ) / 100.0;
        }
        #endregion


        #region [resist]
        private const int ResistOffsetA = 16;
        private const int ResistOffsetB = 25;

        public static int GetCircle( Spell spell )
        {
            if( spell is MagerySpell )
                return (int)( (MagerySpell)spell ).Circle;
            else if( spell is DruidSpell )
                return (int)( (DruidSpell)spell ).Circle;
            else if( spell is RPGPaladinSpell )
                return 1;
            else if( spell is RPGNecromancerSpell )
                return 1;
            else
                return 1;
        }

        public static bool CheckResisted( Mobile caster, Mobile target, Spell spell, bool message, bool effect, bool checkGain )
        {
            int circle = GetCircle( spell );

            double n = GetResistPercent( caster, target, circle );

            bool isParalyzeSpell = spell is ParalyzeSpell || spell is ParalyzeFieldSpell;

            if( isParalyzeSpell || caster.Skills[ SkillName.Inscribe ].Value == 100.0 )
            {
                n -= 10;
                if( n < 0 )
                    n = 0;
            }

            n /= 100.0;

            if( n <= 0.0 )
                return false;

            if( n >= 1.0 )
                return true;

            int maxSkill = ( 1 + circle ) * ResistOffsetA;
            maxSkill += ( 1 + ( circle / 6 ) ) * ResistOffsetB;

            bool isCreatureVsPlayer = caster != null && caster is BaseCreature && target is PlayerMobile;

            if( checkGain && target.Skills[ SkillName.MagicResist ].Value < maxSkill )
                target.CheckSkill( SkillName.MagicResist, 0.0, 120 + ( isCreatureVsPlayer ? 20 : 0 ) );

            bool success = n >= Utility.RandomDouble();

            if( success && target.Player )
            {
                if( caster != target && caster != null && message )
                    caster.SendMessage( "Your enemy resisted the spell!" );

                if( effect )
                {
                    target.FixedParticles( 0x374A, 10, 15, 5028, EffectLayer.Waist );
                    target.PlaySound( 0x1EA );
                }
            }

            return success;
        }

        public static double GetResistPercentForCircle( Mobile caster, Mobile target, int circle )
        {
            double res = target.Skills[ SkillName.MagicResist ].Value;
            double mag = caster.Skills[ SkillName.Magery ].Value;

            double firstPercent = res / 5.0;
            double secondPercent = res - ( ( ( mag - 20.0 ) / 5.0 ) + ( 1 + circle ) * 5.0 );

            return ( firstPercent > secondPercent ? firstPercent : secondPercent ) ;
        }

        public static double GetResistPercent( Mobile caster, Mobile target, int circle )
        {
            return GetResistPercentForCircle( caster, target, circle );
        }
        #endregion

        /// <summary>
        /// Check if loc is in a region which blocks magical fields.
        /// </summary>
        /// <returns>true if we can place a field on loc</returns>
        public static bool CheckBlockField( Point3D loc, Mobile caster )
        {
            Map map = caster.Map;
            if( map == null )
                return false;

            BaseRegion reg = Region.Find( loc, map ) as BaseRegion;
            if( reg == null )
                return true; // mod by Magius(CHE), region==null means BLOCKFIELD = false so this function must return TRUE on empty region!

            bool blocked = reg.BlockFields;

            if( blocked )
                caster.SendMessage( "You cannot cast that there!" );

            return !blocked;
        }

        /// <summary>
        /// Check if loc is in a region which blocks magical fields.
        /// </summary>
        /// <returns>true if we can place a field on loc</returns>
        public static bool CheckBlockField( IPoint3D loc, Mobile caster )
        {
            if( loc is Item )
                loc = ( (Item)loc ).GetWorldLocation();

            return CheckBlockField( new Point3D( loc ), caster );
        }

        /// <summary>
        /// Check if there is another field item nearby our mobile.
        /// </summary>
        /// <returns>true if a field is NOT found</returns>
        public static bool CheckNoMoreFieldsNearby( Mobile m, int radius )
        {
            IPooledEnumerable eable = m.GetItemsInRange( radius );

            foreach( Item item in eable )
            {
                if( item.GetType().IsDefined( typeof( NoSameNearbyItemAttribute ), false ) )
                    return false;
            }

            eable.Free();
            return true;
        }

        public static bool CheckNoMoreFieldsNearby( Map map, Point3D p, int radius )
        {
            IPooledEnumerable eable = map.GetItemsInRange( p, radius );

            foreach( Item item in eable )
            {
                if( item.GetType().IsDefined( typeof( NoSameNearbyItemAttribute ), false ) )
                    return false;
            }

            eable.Free();
            return true;
        }
    }

	[AttributeUsage( AttributeTargets.Class )]
	public class NoSameNearbyItemAttribute : Attribute
	{
    }
}