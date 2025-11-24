/***************************************************************************
 *                               BanishEvilSpell.cs
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
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
    public class BanishEvilSpell : RPGPaladinSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Banish Evil", "Exigo Malum",
            266,
            9002
            );

        private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
            (
            typeof( BanishEvilSpell ),
            m_Info.Name,
            "This miracle banish paladin surrounding creatures and force them to flee.",
            "Questo miracolo esilia le creature malvagie che circondano il paladino e le mette in fuga.",
            "Esilia il male attorno al paladino. Sono affetti necromanti classati e creature e player con karma negativo." +
            "Il raggio d'azione e' PowerValueScaled / 10. Le creature esiliate fuggono (flee) mentre i giocatori vengono teleportati lontano.",
            0x5103
            );

        public override ExtendedSpellInfo ExtendedInfo
        {
            get { return m_ExtendedInfo; }
        }

        public override string GetModificationsForChangelog()
        {
            return "Modificato il raggio in PowerValueScaled / 10.<br>" +
                "Modificato il flee delle creature. Ora durerà PowerValueScaled / 10 secondi.";
        }

        public override SpellCircle Circle
        {
            get { return SpellCircle.Third; }
        }

        private static readonly string[] BanCrawls = new string[]
                                                     {
                                                         "I BANISH THEE!",
                                                         "I banish thee Evil!",
                                                         "I banish thee!",
                                                         "I banish thee!"
                                                     };

        private static readonly string[] BanCrawlsITA = new string[]
                                                     {
                                                         "IO TI ESILIO!",
                                                         "Io ti esilio malvagia creatura!",
                                                         "Io ti esilio!",
                                                         "Io ti bandisco!"
                                                     };

        public BanishEvilSpell( Mobile caster, Item scroll )
            : base( caster, scroll, m_Info )
        {
        }

        public override int DelayOfReuseInSeconds
        {
            get { return 5; }
        }

        public override void OnCast()
        {
            if( Caster.CanBeginAction( typeof( BanishEvilSpell ) ) )
            {
                if( CheckSequence() )
                {
                    List<Mobile> toBan = new List<Mobile>();

                    foreach( Mobile m in Caster.GetMobilesInRange( PowerValueScaled / 10 ) )
                    {
                        if( m == null || m.Deleted || !Caster.CanBeHarmful( m ) || CheckResisted( m ) )
                            continue;

                        if( IsEnemy( m ) && ( GetPowerLevel() >= 4 || !m.Player ) )
                            toBan.Add( m );
                    }

                    if( toBan.Count > 0 )
                    {
                        Caster.BeginAction( typeof( BanishEvilSpell ) );

                        Caster.PlaySound( 0x212 );
                        Caster.PlaySound( 0x206 );

                        Effects.SendLocationParticles( EffectItem.Create( Caster.Location, Caster.Map, EffectItem.DefaultDuration ), 0x376A, 1, 29, 0x47D, 2, 9962, 0 );
                        Effects.SendLocationParticles( EffectItem.Create( new Point3D( Caster.X, Caster.Y, Caster.Z - 7 ), Caster.Map, EffectItem.DefaultDuration ), 0x37C4, 1, 29, 0x47D, 2, 9502, 0 );

                        Map map = Caster.Map;
                        if( map == Map.Internal )
                            return;

                        Timer.DelayCall( TimeSpan.FromSeconds( 0.75 ), TimeSpan.FromSeconds( 1.0 ), 4, new TimerStateCallback( BanRadiusOnTick ), new object[] { Caster, 3, toBan } );
                    }
                    else
                    {
                        Caster.SendLangMessage( 10000103 ); // "There is no valid target for your holy wrath!"
                    }
                }
            }
            else
            {
                Caster.SendLangMessage( 10000104 ); // "Not enough time has passed from last ban." 
            }

            FinishSequence();
        }

        public override void OnCasterDamaged( Mobile from, int damage )
        {
            if( Caster.PlayerDebug )
                Caster.SendMessage( "Debug PaladinSpell: chance to resist disruption: 100%." );
        }

        public override bool CheckDisturb( DisturbType type, bool firstCircle, bool resistable )
        {
            return false;
        }

        private static readonly Dictionary<Mobile, Point3D> m_RestoreLocs = new Dictionary<Mobile, Point3D>();

        private void BanRadiusOnTick( object state )
        {
            object[] states = (object[])state;

            Mobile from = (Mobile)states[ 0 ];
            int timer = (int)states[ 1 ];
            List<Mobile> toBan = (List<Mobile>)states[ 2 ];

            if( from == null )
                return;

            Map map = Caster.Map;
            if( map == null || map == Map.Internal )
                return;

            if( timer == 0 )
            {
                int range = PowerValueScaled / 10;

                foreach( Mobile banned in toBan )
                {
                    if( banned == null )
                        continue;

                    Point3D banLocation = FindBanLocation( Caster.Location, map, range );

                    if( banLocation == Point3D.Zero )
                        continue;

                    // removing previous banned locations we do not stack bans over time
                    if( m_RestoreLocs.ContainsKey( banned ) )
                        m_RestoreLocs.Remove( banned );

                    m_RestoreLocs.Add( banned, banned.Location );

                    Effects.SendLocationParticles( EffectItem.Create( banned.Location, map, EffectItem.DefaultDuration ), 0, 0, 0, 5033 );

                    banned.MoveToWorld( banLocation, map );
                    BaseCreature.TeleportPets( banned, banLocation, map, false );

                    Effects.SendLocationParticles( EffectItem.Create( banLocation, map, EffectItem.DefaultDuration ), 0, 0, 0, 5033 );

                    if( banned is BaseCreature )
                    {
                        BaseCreature bc = banned as BaseCreature;

                        bc.BeginFlee( TimeSpan.FromSeconds( PowerValueScaled / 2.0 ) );
                        bc.PublicOverheadMessage( MessageType.Regular, 1154, true, "* flees around *" );
                    }
                }

                Timer.DelayCall( GetDelayOfReuseInSeconds(), new TimerStateCallback( ReleaseBanishEvilLock ), Caster );
                Timer.DelayCall( TimeSpan.FromSeconds( PowerValueScaled / 10.0 ), new TimerStateCallback( RestoreBanned ), Caster );
            }
            else if( toBan != null )
            {
                from.PublicOverheadMessage( MessageType.Regular, MessageHue, true, from.TrueLanguage == LanguageType.Ita ?
                    BanCrawlsITA[ Utility.Random( BanCrawlsITA.Length ) ] :
                    BanCrawls[ Utility.Random( BanCrawls.Length ) ] );

                Effects.SendLocationParticles( EffectItem.Create( Caster.Location, Caster.Map, EffectItem.DefaultDuration ), 0x376A, 1, 29, 0x47D, 2, 9962, 0 );
                Effects.SendLocationParticles( EffectItem.Create( new Point3D( Caster.X, Caster.Y, Caster.Z - 7 ), Caster.Map, EffectItem.DefaultDuration ), 0x37C4, 1, 29, 0x47D, 2, 9502, 0 );

                states[ 1 ] = timer - 1;
            }
        }

        private static Point3D FindBanLocation( Point3D p, Map map, int range )
        {
            if( map == null )
                return p;

            for( int radius = range; radius >= 1; radius-- )
            {
                Point3D current = new Point3D( p.X + radius, p.Y, p.Z );

                for( int i = 0; i <= 360; i++ )
                {
                    int x = (int)Math.Round( Math.Cos( i ) * radius ) + p.X;
                    int y = (int)Math.Round( Math.Sin( i ) * radius ) + p.Y;

                    Point3D next = new Point3D( x, y, current.Z );
                    int avgZ = map.GetAverageZ( x, y );

                    if( map.CanSpawnMobile( next ) )
                        return next;
                    else if( map.CanSpawnMobile( new Point2D( next.X, next.Y ), avgZ ) )
                        return new Point3D( x, y, avgZ );

                    current = next;
                }
            }

            return Point3D.Zero;
        }

        private static void ReleaseBanishEvilLock( object state )
        {
            ( (Mobile)state ).EndAction( typeof( BanishEvilSpell ) );
            ( (Mobile)state ).SendLangMessage( 10000105 ); // "You can now exile another evil fow!" 
        }

        private static void RestoreBanned( object state )
        {
            if( m_RestoreLocs == null || m_RestoreLocs.Count == 0 )
                return;

            try
            {
                List<Mobile> mobiles = new List<Mobile>( m_RestoreLocs.Keys );

                foreach( Mobile m in mobiles )
                {
                    Point3D loc = m_RestoreLocs[ m ];

                    m.MoveToWorld( loc, m.Map );
                    BaseCreature.TeleportPets( m, loc, m.Map, false );

                    Effects.SendLocationParticles( EffectItem.Create( loc, m.Map, EffectItem.DefaultDuration ), 0, 0, 0, 5033 );
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }
    }
}