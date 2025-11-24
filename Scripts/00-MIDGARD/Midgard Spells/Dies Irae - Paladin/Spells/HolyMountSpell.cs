/***************************************************************************
 *                               HolyMountSpell.cs
 *
 *   begin                : 05 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Mobiles;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
    public class HolyMountSpell : RPGPaladinSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Holy Mount", "Equus Divinus",
            266,
            9040
            );

        private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
            (
            typeof( HolyMountSpell ),
            "Holy Mount",
            "The Paladine summons a holy friend to help his journeys.",
            "Il Paladino evoca un destriero benedetto che lo aiutera' nei momenti difficili.",
            "Il paladino evoca il destriero benedetto. La durata è PowerValueScaled * 3.0 secondi." +
            "Il destriero è particolarmente efficace e resistente contro i malvagi." +
            "In particolare ogni 30 minuti puo' usare l'abilità di curare il proprio paladino (simile a quella del kirin).",
            0x5109
            );

        public override ExtendedSpellInfo ExtendedInfo
        {
            get { return m_ExtendedInfo; }
        }

        public override string GetModificationsForChangelog()
        {
            return "Rimosso il delay di riuso.<br>" +
                "La durata ora sarà di PowerValueScaled * 3.0 secondi." +
                "Il cavallo cambia colore e body a seconda del livello (1, 3 o 5)." +
                "Ridotta la resistenza della cavalcatura al danno del nemico al 50% del danno subito." +
                "Il cavallo da una certa rigenerazione al mana del paladino che lo cavalca.<br>" +
                "Ora ha il soffio solo dal quarto livello in sui.<br>" +
                "Ora per rimuovere lo steed basta rilanciare lo spell.";
        }

        public override SpellCircle Circle
        {
            get { return SpellCircle.Second; }
        }

        public override TimeSpan CastDelayBase
        {
            get { return TimeSpan.FromSeconds( 3.0 ); }
        }

        public override void SayMantra()
        {
            Caster.PlaySound( 0x24A );
            base.SayMantra();
        }

        public HolyMountSpell( Mobile caster, Item scroll )
            : base( caster, scroll, m_Info )
        {
        }

        public static void EventSink_Crashed( CrashedEventArgs e )
        {
            if( m_Table == null || m_Table.Count == 0 )
                return;

            try
            {
                foreach( KeyValuePair<Mobile, HolyMount> kvp in m_Table )
                {
                    if( kvp.Key == null || kvp.Key.Deleted )
                        continue;

                    Unregister( kvp.Key, false );
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
                foreach( KeyValuePair<Mobile, HolyMount> kvp in m_Table )
                {
                    if( kvp.Key == null || kvp.Key.Deleted )
                        continue;

                    Unregister( kvp.Key, false );
                }
            }
            catch( Exception exception )
            {
                Console.WriteLine( exception );
            }
        }

        private static readonly Dictionary<Mobile, HolyMount> m_Table = new Dictionary<Mobile, HolyMount>();

        public static Dictionary<Mobile, HolyMount> Table
        {
            get { return m_Table; }
        }

        public static bool HasMount( Mobile m )
        {
            return m_Table != null && m_Table.ContainsKey( m ) && m_Table[ m ] != null;
        }

        public static HolyMount GetMount( Mobile m )
        {
            if( m_Table != null && m_Table.ContainsKey( m ) && m_Table[ m ] != null )
                return m_Table[ m ];
            else
                return null;
        }

        public static void SetMount( Mobile m, HolyMount mount )
        {
            if( m_Table != null && m_Table.ContainsKey( m ) && m_Table[ m ] != null )
            {
                if( mount == null )
                    m_Table[ m ].Delete();
                else
                    m_Table[ m ] = mount;
            }
        }

        public override void OnCast()
        {
            if( Caster == null )
                return;

            if( Caster.CanBeginAction( typeof( HolyMountSpell ) ) )
            {
                if( HasMount( Caster ) )
                {
                    HolyMount mount = m_Table[ Caster ];
                    if( mount != null && !mount.Deleted )
                        mount.Delete();

                    FinishSequence();
                    return;
                }

                if( CheckSequence() )
                {
                    HolyMount familiar = new HolyMount( Caster, GetPowerLevel() );

                    if( BaseCreature.Summon( familiar, Caster, Caster.Location, 0x217, TimeSpan.FromSeconds( PowerValueScaled * 3.0 ) ) )
                    {
                        Caster.SendLangMessage( 10000302 ); // "Paladin, your you have now a new friend. Respect its soul and protect its body."
                        if( !familiar.Deleted && familiar.Rider == null && Validate( Caster ) )
                        {
                            familiar.SetDirection( Caster.Direction );
                            familiar.Rider = Caster;

                            Caster.FixedParticles( 0x3728, 1, 10, 9910, EffectLayer.Head );
                            familiar.PlaySound( familiar.GetIdleSound() );

                            Table[ Caster ] = familiar;

                            Caster.BeginAction( typeof( HolyMountSpell ) );
                        }
                    }
                }
            }
            else
            {
                Caster.SendLangMessage( 10000305 ); // You cannot summon a holy friend now.
            }

            FinishSequence();
        }

        public virtual bool Validate( Mobile from )
        {
            if( !BaseMount.CheckMountAllowed( from, true ) )
            {
                // CheckMountAllowed sends the message
                return false;
            }
            else if( from.Mounted )
            {
                from.SendLocalizedMessage( 1005583 ); // Please dismount first.
                return false;
            }
            else if( from.IsBodyMod && !from.Body.IsHuman )
            {
                from.SendLocalizedMessage( 1061628 ); // You can't do that while polymorphed.
                return false;
            }
            else if( from.HasTrade )
            {
                from.SendLocalizedMessage( 1042317, "", 0x41 ); // You may not ride at this time
                return false;
            }
            else if( ( from.Followers + 1 ) > from.FollowersMax )
            {
                from.SendLocalizedMessage( 1049679 ); // You have too many followers to summon your mount.
                return false;
            }

            return true;
        }

        public static void Unregister( Mobile master, bool message )
        {
            if( master == null )
                return;

            int level = GetPowerLevelByType( master, typeof( HolyMountSpell ) );

            Timer.DelayCall( TimeSpan.FromMinutes( DelayOfReuseInMinutes( master, level ) ), new TimerStateCallback( ReleaseHolyMountLock ), master );

            if( m_Table.ContainsKey( master ) )
                m_Table.Remove( master );

            if( message )
                master.SendLangMessage( 10000303 ); // "Your Holy Friend has been released!"
        }

        private static void ReleaseHolyMountLock( object state )
        {
            ( (Mobile)state ).EndAction( typeof( HolyMountSpell ) );
            ( (Mobile)state ).SendLangMessage( 10000304 ); // "You can now summon a new holy mount."
        }

        public static int DelayOfReuseInMinutes( Mobile master, int level )
        {
            if( master == null )
                return 30;
            else
            {
                switch( level )
                {
                    case 1:
                    case 2: return 30;
                    case 3:
                    case 4: return 20;
                    case 5: return 0;

                    default: return 30;
                }
            }
        }
    }
}