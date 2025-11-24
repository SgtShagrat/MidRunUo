using Midgard.Items;

using Server;
using Server.Accounting;
using Server.Commands;
using Server.Mobiles;
using Server.Targeting;

namespace Midgard.Engines.JailSystem
{
    public class WarnTarget : Target
    {
        private readonly bool m_Warn;

        public WarnTarget( bool warn )
            : base( -1, false, TargetFlags.None )
        {
            m_Warn = warn;
        }

        public WarnTarget()
            : base( -1, false, TargetFlags.None )
        {
            m_Warn = true;
        }

        protected override void OnTarget( Mobile from, object targeted )
        {
            if( from is PlayerMobile && targeted is PlayerMobile )
            {
                var m = (Mobile)targeted;
                if( m_Warn )
                    from.SendGump( new JailWarnGump( from, m, "", 0, null ) );
                else
                {
                    from.SendGump( new JailReviewGump( from, m, 0, null ) );
                }
            }
        }
    }

    public class MacroTarget : Target
    {
        public MacroTarget()
            : base( -1, false, TargetFlags.None )
        {
        }

        protected override void OnTarget( Mobile from, object targeted )
        {
            if( from is PlayerMobile && targeted is PlayerMobile )
            {
                var m = (Mobile)targeted;
                JailSystem.MacroTest( from, m );
            }
        }
    }

    public class CageTarget : Target
    {
        public CageTarget()
            : base( -1, false, TargetFlags.None )
        {
        }

        protected override void OnTarget( Mobile from, object targeted )
        {
            if( from is PlayerMobile && targeted is PlayerMobile )
            {
                if( from is PlayerMobile && targeted is PlayerMobile )
                {
                    var m = (Mobile)targeted;
                    new ACage( m );
                    Point3D newcell = m.Location;
                    m.Location = new Point3D( 0, 0, 0 );
                    m.Location = newcell;
                }
            }
        }
    }

    public class JailTarget : Target
    {
        private readonly bool m_Releasing;

        public JailTarget( bool releasing )
            : base( -1, false, TargetFlags.None )
        {
            m_Releasing = releasing;
        }

        protected override void OnTarget( Mobile from, object targeted )
        {
            if( from is PlayerMobile && targeted is PlayerMobile )
            {
                string temp = "jail";
                var m = (Mobile)targeted;
                if( from.AccessLevel < m.AccessLevel )
                {
                    from.SendMessage( "{0} has a higher access level than you and you can not do that.", m.Name );
                    if( m_Releasing )
                        temp = "release";
                    CommandLogging.WriteLine( from, from.Name + " tried to " + temp + " " + m.Name );
                    m.SendMessage( from.Name + " tried to " + temp + " you" );
                }
                else
                {
                    //jailor has a higher (or equal) access level than the target				
                    if( m_Releasing )
                    {
                        JailSystem js = JailSystem.FromAccount( (Account)m.Account );
                        if( js == null )
                        {
                            from.SendMessage( m.Name + " no jail object" );
                            return;
                        }
                        js.ForceRelease( from );
                        m.SendLocalizedMessage( 501659 );
                    }
                    else
                    {
                        //temp="jailed";
                        JailSystem.NewJailingFromGMandPlayer( from, m );
                    }
                }
            }
            else
            {
                from.SendLocalizedMessage( 503312 );
            }
        }
    }
}