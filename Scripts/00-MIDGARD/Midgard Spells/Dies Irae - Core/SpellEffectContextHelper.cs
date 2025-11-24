/***************************************************************************
 *                               SpellEffectContextHelper.cs
 *
 *   begin                : 07 giugno 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
    public class SpellEffectContext
    {
        public Timer Timer { get; private set; }
        public IEffectSpell Spell { get; private set; }
        public Type Type { get; private set; }

        public SpellEffectContext( Timer timer, IEffectSpell spell, double durationInMinutes )
        {
            Timer = timer;
            Spell = spell;
            ExpirationTime = DateTime.Now + TimeSpan.FromMinutes( durationInMinutes );

            Type = Spell.GetType();
        }

        public DateTime ExpirationTime { get; private set; }

        public bool Expired { get { return DateTime.Now > ExpirationTime; } }

        public void StartEffect( Mobile mobile )
        {
            if( Spell != null )
                Spell.StartEffect( mobile );
        }

        public void EndEffect( Mobile mobile )
        {
            if( Spell != null )
                Spell.EndEffect( mobile );
        }
    }

    public interface IEffectSpell
    {
        void StartEffect( Mobile m );
        void EndEffect( Mobile m );

        double TickRate { get; }
        void OnTick( Mobile m );

        double DurationInMinutes { get; }
    }

    public class SpellEffectContextHelper
    {
        public static void Initialize()
        {
            EventSink.Crashed += new CrashedEventHandler( EventSink_Crashed );
            EventSink.Shutdown += new ShutdownEventHandler( EventSink_Shutdown );
        }

        public static void EventSink_Crashed( CrashedEventArgs e )
        {
            if( m_Table == null || m_Table.Count == 0 )
                return;

            try
            {
                foreach( KeyValuePair<Mobile, List<SpellEffectContext>> kvp in m_Table )
                {
                    if( kvp.Key == null || kvp.Key.Deleted )
                        continue;

                    Console.WriteLine( "Trying to remove spell context from {0}...", kvp.Key.Name ?? "a mobile." );
                    RemoveAnyContext( kvp.Key );
                }
            }
            catch( Exception exception )
            {
                Console.WriteLine( exception );
            }
        }

        public static void EventSink_Shutdown( ShutdownEventArgs e )
        {
            if( m_Table == null || m_Table.Count == 0 )
                return;

            try
            {
                foreach( KeyValuePair<Mobile, List<SpellEffectContext>> kvp in m_Table )
                {
                    if( kvp.Key == null || kvp.Key.Deleted )
                        continue;

                    Console.WriteLine( "Trying to remove spell context from {0}...", kvp.Key.Name ?? "a mobile." );
                    RemoveAnyContext( kvp.Key );
                }
            }
            catch( Exception exception )
            {
                Console.WriteLine( exception );
            }
        }

        private static Dictionary<Mobile, List<SpellEffectContext>> m_Table;

        public static void StartEffect( Mobile m, SpellEffectContext context )
        {
            if( m_Table == null )
                m_Table = new Dictionary<Mobile, List<SpellEffectContext>>();

            if( !m_Table.ContainsKey( m ) || m_Table[ m ] == null )
                m_Table[ m ] = new List<SpellEffectContext>();

            if( !m_Table[ m ].Contains( context ) )
            {
                m_Table[ m ].Add( context );
                context.StartEffect( m );
            }
        }

        public static void RemoveAnyContext( Mobile m )
        {
            if( !m_Table.ContainsKey( m ) )
                return;

            List<SpellEffectContext> contexts = GetContexts( m );

            foreach( SpellEffectContext context in contexts )
                RemoveContext( m, context );
        }

        public static void RemoveContextByType( Mobile m, Type spellEffectType )
        {
            SpellEffectContext context = GetContext( m, spellEffectType );
            if( context != null )
                RemoveContext( m, context );
        }

        public static void RemoveContext( Mobile m, SpellEffectContext context )
        {
            if( context == null || !m_Table.ContainsKey( m ) )
                return;

            List<SpellEffectContext> contexts = GetContexts( m );
            if( contexts == null )
                return;

            if( contexts.Contains( context ) )
                contexts.Remove( context );

            context.Timer.Stop();
            context.EndEffect( m );

            if( contexts.Count == 0 )
                m_Table.Remove( m );
        }

        public static SpellEffectContext GetContext( Mobile m, Type spellEffectType )
        {
            try
            {
                List<SpellEffectContext> contexts;
                if( m_Table != null )
                    m_Table.TryGetValue( m, out contexts );
                else
                    return null;

                if( contexts != null )
                {
                    foreach( SpellEffectContext context in contexts )
                    {
                        if( context.Type == spellEffectType )
                            return context;
                    }
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
                Utility.Log( "SpellEffectContextErrors.log", ex.ToString() );
            }

            return null;
        }

        public static List<SpellEffectContext> GetContexts( Mobile m )
        {
            List<SpellEffectContext> contexts;
            if( m_Table != null )
                m_Table.TryGetValue( m, out contexts );
            else
                return null;

            return contexts;
        }

        public static bool IsUnderEffect( Mobile m, Type spellEffectType )
        {
            return GetContext( m, spellEffectType ) != null;
        }

        public static bool OnCast( Mobile caster, Spell spell )
        {
            IEffectSpell effectSpell = spell as IEffectSpell;
            if( effectSpell == null )
                return false;

            if( spell.CheckSequence() )
            {
                Type ourType = spell.GetType();
                SpellEffectContext context = GetContext( caster, ourType );

                bool wasTransformed = ( context != null );
                bool ourTransform = ( wasTransformed && context.Type == ourType );

                if( wasTransformed )
                {
                    RemoveContext( caster, context );

                    if( ourTransform )
                    {
                        caster.PlaySound( 0xFA );
                        caster.FixedParticles( 0x3728, 1, 13, 5042, EffectLayer.Waist );
                    }
                }

                if( !ourTransform )
                {
                    Timer timer = new EffectTimer( caster, effectSpell );
                    timer.Start();

                    StartEffect( caster, new SpellEffectContext( timer, effectSpell, effectSpell.DurationInMinutes ) );
                    return true;
                }
            }

            return false;
        }

        private class EffectTimer : Timer
        {
            private readonly Mobile m_Caster;
            private readonly IEffectSpell m_Spell;

            public EffectTimer( Mobile caster, IEffectSpell spell )
                : base( TimeSpan.FromSeconds( spell.TickRate ), TimeSpan.FromSeconds( spell.TickRate ) )
            {
                m_Caster = caster;
                m_Spell = spell;

                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                Type ourType = m_Spell.GetType();

                SpellEffectContext context = GetContext( m_Caster, ourType );
                if( context != null && context.Expired )
                {
                    RemoveContextByType( m_Caster, ourType );
                    Stop();
                }
                else if( m_Caster.Deleted || !m_Caster.Alive )
                {
                    RemoveContextByType( m_Caster, ourType );
                    Stop();
                }
                else
                    m_Spell.OnTick( m_Caster );
            }
        }
    }
}