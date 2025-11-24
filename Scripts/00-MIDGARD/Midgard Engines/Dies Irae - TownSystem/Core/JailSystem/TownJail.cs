using System;
using System.Xml;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Network;

namespace Midgard.Engines.MidgardTownSystem
{
    public class TownJail : BaseRegion
    {
        public TownJail( XmlElement xml, Map map, Region parent )
            : base( xml, map, parent )
        {
            XmlElement cells = xml[ "jailCells" ];
            if( cells == null )
                return;

            foreach( XmlNode node in cells.ChildNodes )
            {
                /*
                <cell name="Cell 1">
                    <go x="262" y="766" z="0" />
                    <door x="262" y="769 " z="-2" />
                </cell>
                */

                XmlElement el = node as XmlElement;
                if( el == null )
                    continue;

                JailCell cell = new JailCell( el );
                TownJailSystem.Instance.RegisterCell( cell );
            }
        }

        public override bool AllowBeneficial( Mobile from, Mobile target )
        {
            if( from.AccessLevel == AccessLevel.Player )
                from.SendMessage( "You may not do that in jail." );

            return ( from.AccessLevel > AccessLevel.Player );
        }

        public override bool AllowHarmful( Mobile from, Mobile target )
        {
            if( from.AccessLevel == AccessLevel.Player )
                from.SendMessage( "You may not do that in jail." );

            return ( from.AccessLevel > AccessLevel.Player );
        }

        public override bool AllowHousing( Mobile from, Point3D p )
        {
            return false;
        }

        public override bool OnBeginSpellCast( Mobile from, ISpell s )
        {
            if( from.AccessLevel == AccessLevel.Player )
                from.SendLocalizedMessage( 502629 ); // You cannot cast spells here.

            return ( from.AccessLevel > AccessLevel.Player );
        }

        public override bool OnSkillUse( Mobile from, int skill )
        {
            if( from.AccessLevel == AccessLevel.Player )
                from.SendMessage( "You may not use skills in jail." );

            return ( from.AccessLevel > AccessLevel.Player );
        }

        public override bool OnCombatantChange( Mobile from, Mobile oldCombatant, Mobile newCombatant )
        {
            return ( from.AccessLevel > AccessLevel.Player );
        }

        public override void OnSpeech( SpeechEventArgs e )
        {
            base.OnSpeech( e );

            Mobile m = e.Mobile;

            if( TownJailSystem.Instance.IsActuallyCondemned( m ) && e.Speech.ToLower() == "detenction time" )
            {
                Condemn condemn = TownJailSystem.Instance.FindCondemnForPrisoner( m );
                if( condemn != null )
                {
                    TimeSpan duration = condemn.ExpirationTime - DateTime.Now;
                    string message = string.Format( "You will stay here for {0:F0} minutes.", duration.TotalMinutes );
                    m.PublicOverheadMessage( MessageType.Regular, 0x37, true, message );

                    e.Handled = true;
                }
            }

            if( !e.Handled && TownJailSystem.Instance.IsValidJailor( m ) && e.Speech.ToLower() == "i will release you" )
            {
                m.SendMessage( "Target the prisoner you wish to release." );
                m.Target = new ReleaseTarget();

                e.Handled = true;
            }
        }

        private class ReleaseTarget : Target
        {
            public ReleaseTarget()
                : base( 10, false, TargetFlags.None )
            {
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( !( targeted is Mobile ) )
                    return;

                Mobile prisoner = targeted as Mobile;
                if( TownJailSystem.Instance.IsActuallyCondemned( prisoner ) )
                {
                    Condemn condemn = TownJailSystem.Instance.FindCondemnForPrisoner( prisoner );
                    if( condemn != null )
                    {
                        condemn.Release();
                        from.SendMessage( "That player is now free." );
                    }
                    else
                        from.SendMessage( "There is no condemn for this player" );
                }
                else
                    from.SendMessage( "There is no condemn for this player" );
            }
        }
    }
}