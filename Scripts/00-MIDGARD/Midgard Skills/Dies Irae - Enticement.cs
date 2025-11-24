using System;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;

namespace Server.SkillHandlers
{
    public class Enticement
    {
        public static void Initialize()
        {
            SkillInfo.Table[ (int)SkillName.Discordance ].Callback = new SkillUseCallback( OnUse );
        }

        public static TimeSpan OnUse( Mobile m )
        {
            Console.WriteLine( "Enticement.OnUse" );

            m.RevealingAction( true );

            if( !m.Player )
                return TimeSpan.Zero;

            //if( pm.LastInstrumentUsed != null && pm.LastInstrumentUsed.RootParent == pm )
            //    OnPickedInstrument( m, pm.LastInstrumentUsed );
            //else
            BaseInstrument.PickInstrument( m, new InstrumentPickedCallback( OnPickedInstrument ) );

            return TimeSpan.FromSeconds( 10.0 ); // Cannot use another skill for 10 seconds
        }

        public static void OnPickedInstrument( Mobile from, BaseInstrument instrument )
        {
            from.RevealingAction( true );
            from.SendLocalizedMessage( 500878 );  // Whom do you wish to entice?
            from.Target = new EnticementTarget( from, instrument );
        }

        public class EnticementTarget : Target
        {
            private BaseInstrument m_Instrument;

            public EnticementTarget( Mobile from, BaseInstrument inst )
                : base( BaseInstrument.GetBardRange( from, SkillName.Discordance ), false, TargetFlags.None )
            {
                m_Instrument = inst;
            }

            protected override void OnTarget( Mobile from, object target )
            {
                from.RevealingAction( true );

                if( from.Region != null && !from.Region.OnSkillUse( from, (int)( SkillName.Discordance ) ) )
                    return;

                if( target is Mobile )
                {
                    Mobile targ = (Mobile)target;

                    BaseCreature bc = targ as BaseCreature;

                    if( targ == from )
                        from.SendLocalizedMessage( 500880 );  //You cannot entice yourself!
                    else if( targ is PlayerMobile || ( bc != null && bc.BardImmune ) )
                        from.SendLocalizedMessage( 500879 );  //You cannot entice that!
                    else if( targ is TownCrier /*|| targ is Banker || targ is BaseVendor */)
                        from.SendLocalizedMessage( 500887 );  //They look too dedicated to their job to be lured away.
                    else if( targ is BaseVendor && bc != null && bc.Home != Point3D.Zero && bc.RangeHome > 0 && !bc.InRange( bc.Home, bc.RangeHome ) )
                        targ.Say( 500890 );  //Oh, but I cannot wander too far from my shop!
                    else if( targ is BaseCreature && ( (BaseCreature)targ ).Controlled && !( (BaseCreature)targ ).IsEnticed )
                        from.SendAsciiMessage( 0x3B2, String.Format( "You cannot entice tamed creatures." ) );
                    else if( bc != null )
                    {
                        from.NextSkillTime = DateTime.Now.Add( TimeSpan.FromSeconds( 8.0 ) );
                        double music = from.Skills[ SkillName.Musicianship ].Value;

                        if( bc.Combatant != null )
                            from.SendAsciiMessage( 0x3B2, String.Format( "That creature is too angry to be enticed." ) );
                        else if( !BaseInstrument.CheckMusicianship( from ) )
                        {
                            from.SendAsciiMessage( 0x3B2, String.Format( "You play poorly, and there is no effect." ) );
                            m_Instrument.PlayInstrumentBadly( from );
                        }
                        else if( from.CheckTargetSkill( SkillName.Discordance, target, 0.0, 100.0 ) ) //diff-25.0, diff+25.0
                        {
                            if( !bc.IsEnticed )
                            {
                                from.SendAsciiMessage( 0x3B2, String.Format( "You successfully enticed your target." ) );
                                m_Instrument.PlayInstrumentWell( from );

                                bc.Enticed( from );
                            }
                            else
                                from.SendAsciiMessage( 0x3B2, String.Format( "Your target is already enticed." ) );
                        }
                        else
                        {
                            from.SendAsciiMessage( 0x3B2, String.Format( "You fail to entice your target" ) );
                            m_Instrument.PlayInstrumentBadly( from );
                        }
                    }
                    else
                        m_Instrument.PlayInstrumentBadly( from );
                }
            }
        }
    }
}