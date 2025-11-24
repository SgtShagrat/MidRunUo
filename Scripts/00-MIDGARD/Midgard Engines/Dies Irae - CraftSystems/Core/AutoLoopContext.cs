/***************************************************************************
 *                               AutoLoop.cs
 *                            -----------------
 *   begin                : 17 luglio, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Network;
using Server.StringQueries;

namespace Midgard.Engines.AutoLoop
{
    public delegate void AutoLoopQueryCallback( Mobile from, bool okay, string text, object state );

    public class AutoLoopQuery : StringQuery
    {
        private AutoLoopQueryCallback m_Callback;
        private object m_State;

        public AutoLoopQuery( AutoLoopContext context )
            : this( context, null )
        {
        }

        public AutoLoopQuery( AutoLoopContext context, object state )
            : base(
                context.QueryTopFormat, true, StringQueryStyle.Numerical, context.MaxNumber,
                string.Format( context.QueryEntryFormat, context.MaxNumber ) )
        {
            m_Callback = context.ContextCallback;
            m_State = state;
        }

        public override void OnResponse( NetState sender, bool okay, string text )
        {
            if( m_Callback != null )
                m_Callback( sender.Mobile, okay, text, m_State );
        }
    }

    public class AutoLoopContext
    {
        public virtual string QueryTopFormat { get { return "How many items of this type would you like to make?"; } }

        public virtual string QueryEntryFormat { get { return "(Max {0})"; } }

        public virtual int MaxNumber { get { return 20; } }

        private static Dictionary<Mobile, AutoLoopContext> m_ContextTable = new Dictionary<Mobile, AutoLoopContext>();

        public static bool HasPendingAutoLoopContext( Mobile m )
        {
            return m != null && m_ContextTable.ContainsKey( m );
        }

        public static void EndAutoLoopContext( Mobile m )
        {
            if( m != null )
                m_ContextTable.Remove( m );
        }

        public static AutoLoopContext GetContext( Mobile m )
        {
            if( m == null )
                return null;

            if( m.Deleted )
            {
                m_ContextTable.Remove( m );
                return null;
            }

            AutoLoopContext c;
            m_ContextTable.TryGetValue( m, out c );

            return c;
        }

        private Mobile m_Player;
        private Point3D m_StartLocation;
        private DateTime m_ContextStartTime;
        private DateTime m_ActionStartTime;
        private int m_RemainingLoops;
        private int m_ChosenLoops;

        public AutoLoopContext( Mobile player )
        {
            if( player == null )
                return;

            if( player.Deleted )
                m_ContextTable.Remove( player );
            else
            {
                m_ContextStartTime = DateTime.Now;
                m_Player = player;
                m_StartLocation = m_Player.Location;

                Init();
            }
        }

        public void Init()
        {
            if( HasPendingAutoLoopContext( m_Player ) )
                return;

            m_ContextTable[ m_Player ] = this;
            m_Player.SendStringQuery( new AutoLoopQuery( this ) );
        }

        public void ContextCallback( Mobile from, bool okay, string text, object state )
        {
            if( !CheckTime( true ) )
            {
                EndAutoLoopContext( m_Player );
                return;
            }

            if( okay )
            {
                int result;
                if( int.TryParse( text, out result ) )
                {
                    if( result <= MaxNumber )
                    {
                        m_ChosenLoops = result;
                        m_RemainingLoops = m_ChosenLoops;

                        Start();
                    }
                    else
                        m_Player.SendMessage( m_Player.Language == "ITA" ? "Hai inserito un numero non valido: max {0}" : "You entered an invalid number: max is {0}", MaxNumber );
                }
                else
                    m_Player.SendMessage( m_Player.Language == "ITA" ? "Hai inserito un numero non valido." : "You entered an invalid number." );
            }
            else
                m_Player.SendMessage( m_Player.Language == "ITA" ? "Loop cancellato." : "Looping aborted." );
        }

        public virtual void Start()
        {
            m_ActionStartTime = DateTime.Now; // first action is special...
            DoNextOrEnd();
        }

        public virtual void DoAction()
        {
        }

        public virtual void DoNextOrEnd()
        {
            if( m_RemainingLoops > 0 && CheckLoop() && CheckActionTime( true ) )
            {
                m_Player.SendMessage( m_Player.Language == "ITA" ? "Loop [{0} da fare]." : "Looping [{0} more to go].", m_RemainingLoops );
                m_RemainingLoops--;
                m_ActionStartTime = DateTime.Now;

                DoAction();
            }
            else
                Finish( false );
        }

        public virtual void Finish( )
        {
            m_RemainingLoops = 0;
        }

        public virtual void Finish( bool aborted )
        {
            if( m_RemainingLoops < 0 )
            {
                if( aborted )
                    m_Player.SendMessage( m_Player.Language == "ITA" ? "Loop cancellato." : "Looping aborted." );
                else
                    m_Player.SendMessage( "You finished looping." );
            }

            m_RemainingLoops = 0;

            EndAutoLoopContext( m_Player );
        }

        public virtual void Disrupt()
        {
            Finish( true );
        }

        #region checks
        public virtual bool IsExpired( Mobile from )
        {
            bool expired = !CheckActionTime( false );
            if( expired )
                EndAutoLoopContext( m_Player );

            return expired;
        }

        public virtual bool CheckLoop()
        {
            return m_Player != null && m_Player.Alive && m_Player.NetState != null && CheckLocation( true );
        }

        public virtual bool CheckLocation( bool message )
        {
            bool hasMoved = !Utility.RangeCheck( m_Player.Location, m_StartLocation, 2 );

            if( hasMoved && message )
            {
                m_Player.SendMessage( m_Player.Language == "ITA" ? "Ti sei mosso." : "You moved." );
            }

            return !hasMoved;
        }

        public virtual bool CheckActionTime( bool message )
        {
            if( DateTime.Now - m_ActionStartTime > TimeSpan.FromSeconds( 15.0 ) )
            {
                if( message )
                    m_Player.SendMessage( m_Player.Language == "ITA" ? "E' passato troppo tempo." : "Too much time has passed." );

                return false;
            }

            return true;
        }

        public virtual bool CheckTime( bool message )
        {
            if( DateTime.Now - m_ContextStartTime > TimeSpan.FromSeconds( 60.0 ) )
            {
                if( message )
                    m_Player.SendMessage( m_Player.Language == "ITA" ? "E' passato troppo tempo." : "Too much time has passed." );

                return false;
            }

            return true;
        }
        #endregion
    }
}