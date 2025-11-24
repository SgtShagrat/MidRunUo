using System;
using System.Collections.Generic;
using Midgard.Misc;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

using MidgardClasses = Midgard.Engines.Classes.Classes;

namespace Midgard.Engines.Classes
{
    public sealed class PaladinSystem : ClassSystem
    {
        private static readonly Type[] m_PaladinStoneTypes = new Type[]{
                                                              typeof ( HonorStone ),
                                                              typeof ( CompassionStone ),
                                                              typeof ( HonestyStone ),
                                                              typeof ( HumiltyStone ),
                                                              typeof ( JusticeStone ),
                                                              typeof ( SacrificeStone ),
                                                              typeof ( SpiritualityStone ),
                                                              typeof ( ValorStone )
                                                              };

        public PaladinSystem()
        {
            Definition = new ClassDefinition( "Paladin",
                                                MidgardClasses.Paladin,
                                                0,
                                                DefaultWelcomeMessage,
                                                new PowerDefinition[]{
                                                                     new SwordOfLightDefinition(),
                                                                     new BanishEvilDefinition(),
                                                                     new BlessedDropsDefinition(),
                                                                     new HolyMountDefinition(),
                                                                     new HolyWillDefinition(),
                                                                     new InvulnerabilityDefinition(),
                                                                     new OneEnemyOneShotDefinition(),
                                                                     new PathToHeavenDefinition(),
                                                                     new SacredBeamDefinition(),
                                                                     new SacredFeastDefinition(),
                                                                     new ShieldOfRighteousnessDefinition(),
                                                                     new LayOfHandsDefinition(),
                                                                     new CurePoisonDefinition(),
                                                                     new ChalmChaosDefinition(),
                                                                     new HolyCircleDefinition(),
                                                                     new LegalThoughtsDefinition()
                                                                     }
            );
        }

        public override ClassPlayerState IstantiateState()
        {
            return new ClassPlayerState();
            // return new PaladinPlayerState( this ); // TODO
        }

        public static void RegisterCommandsOrEvents()
        {
        }

        public override void VerifyPlayerStates()
        {
            // if( Config.Debug )
            Config.Pkg.LogInfo( "Verifying {0} class player states...", Definition.ClassName );

            /*
            for( int i = 0; i < Players.Count; i++ )
            {
                ClassPlayerState state = Players[ i ];
                if( state == null || state is PaladinPlayerState )
                    continue;

                Console.WriteLine( "State of paladin {0} should be fixed.", state.Mobile.Name );

                PaladinPlayerState paladinState = new PaladinPlayerState( this, state.Mobile );

                foreach( PowerDefinition def in Definition.PowersDefinitions )
                    paladinState.SetLevel( def, state.GetLevel( def ) );

                state.Detach();

                paladinState.Attach( true );
                paladinState.Invalidate();
            }
            */

            // if( Config.Debug )
            Config.Pkg.LogInfo( "done\n" );
        }

        public override void SetStartingSkills( Mobile mobile )
        {
            mobile.Skills[ SkillName.Chivalry ].Base = 35.0;
        }

        public override bool IsEligible( Mobile mob )
        {
            if( mob.Race == Races.Core.Drow )
                return false;
            else if( mob.Race == Races.Core.Vampire )
                return false;
            else if( mob.Race == Races.Core.Undead )
                return false;
            else if( mob.Race == Races.Core.HighOrc )
                return false;
            else if( mob.Race == Races.Core.HalfDaemon )
                return false;
            else if( mob.Race == Races.Core.FairyOfAir )
                return false;
            else if( mob.Race == Races.Core.FairyOfEarth )
                return false;
            else if( mob.Race == Races.Core.FairyOfFire )
                return false;
            else if( mob.Race == Races.Core.FairyOfWater )
                return false;
            else if( mob.Race == Races.Core.FairyOfWood )
                return false;
            else if( mob.Race == Races.Core.Sprite )
                return false;

            return base.IsEligible( mob );
        }

        public override string IsEligibleString( Mobile mob )
        {
            if( mob.Race == Races.Core.Drow )
                return ( mob.Language == "ITA" ? "Sei un Drow, non potrai mai diventare Paladino." : "You're a Drow, you cannot become a Paladin." );
            else if( mob.Race == Races.Core.Vampire )
                return ( mob.Language == "ITA" ? "Sei un Vampiro, non potrai mai diventare Paladino." : "You're a Vampire, you cannot become a Paladin." );
            else if( mob.Race == Races.Core.Undead )
                return ( mob.Language == "ITA" ? "Sei un Non-morto, non potrai mai diventare Paladino." : "You're an Undead, you cannot become a Paladin." );
            else if( mob.Race == Races.Core.HighOrc )
                return ( mob.Language == "ITA" ? "Sei un Orco, non potrai mai diventare Paladino." : "You're an Orc, you cannot become a Paladin." );
            else if( mob.Race == Races.Core.HalfDaemon )
                return ( mob.Language == "ITA" ? "Sei un Demone, non potrai mai diventare Paladino." : "You're a Daemon, you cannot become a Paladin." );
            else if( mob.Race == Races.Core.Sprite )
                return ( mob.Language == "ITA" ? "Sei un Folletto, non potrai mai diventare Paladino." : "You're a Sprite, you cannot become a Paladin." );
            else if( mob.Race == Races.Core.FairyOfAir )
                return ( mob.Language == "ITA" ? "Sei una Fata, non potrai mai diventare Paladino." : "You're a Fairy, you cannot become a Paladin." );
            else if( mob.Race == Races.Core.FairyOfEarth )
                return ( mob.Language == "ITA" ? "Sei una Fata, non potrai mai diventare Paladino." : "You're a Fairy, you cannot become a Paladin." );
            else if( mob.Race == Races.Core.FairyOfFire )
                return ( mob.Language == "ITA" ? "Sei una Fata, non potrai mai diventare Paladino." : "You're a Fairy, you cannot become a Paladin." );
            else if( mob.Race == Races.Core.FairyOfWater )
                return ( mob.Language == "ITA" ? "Sei una Fata, non potrai mai diventare Paladino." : "You're a Fairy, you cannot become a Paladin." );
            else if( mob.Race == Races.Core.FairyOfWood )
                return ( mob.Language == "ITA" ? "Sei una Fata, non potrai mai diventare Paladino." : "You're a Fairy, you cannot become a Paladin." );

            return base.IsEligibleString( mob );
        }

        public override bool IsAllowedSkill( SkillName skillName )
        {
            if( skillName == SkillName.Poisoning )
                return false;
            else if( skillName == SkillName.Archery )
                return false;
            else if( skillName == SkillName.Magery )
                return false;
            else if( skillName == SkillName.Necromancy )
                return false;
            else if( skillName == SkillName.Spellweaving )
                return false;

            return true;
        }

        #region [rituals & spells]
        public override void MakeRitual( Mobile ritualist, PowerDefinition definition )
        {
            PaladinRitual ritual = new PaladinRitual( definition, ritualist );
            ritual.Start();
        }

        public override bool SupportsRitualItems
        {
            get { return true; }
        }

        public override RitualItem RandomRitualItem()
        {
            return Loot.Construct( m_PaladinStoneTypes ) as RitualItem;
        }

        public override int GetSpellLabelHueByLevel( int level )
        {
            switch( level )
            {
                case 0:
                    return DisabledLabelHue;
                case 1:
                    return Colors.Orange;
                case 2:
                    return Colors.Darkorange;
                case 3:
                    return Colors.DarkGoldenRod;
                case 4:
                    return Colors.GoldenRod;
                case 5:
                    return Colors.Gold;
                default:
                    return DisabledLabelHue;
            }
        }
        #endregion

        public override bool HandlesOnSpeech
        {
            get { return true; }
        }

        public override void OnSpeech( SpeechEventArgs e )
        {
            if( FindAnkh( e.Mobile ) != null )
                HandleAnkhOnSpeech( e );

            base.OnSpeech( e );
        }

        private static void HandleAnkhOnSpeech( SpeechEventArgs e )
        {
            if( e.Blocked || e.Handled )
                return;

            Midgard2PlayerMobile mobile = e.Mobile as Midgard2PlayerMobile;
            if( mobile == null || !mobile.Alive )
                return;

            if( !IsPaladine( mobile ) )
                return;

            if( Insensitive.Equals( PrayString, e.Speech ) )
            {
                if( CanPray( mobile ) )
                {
                    StartPray( mobile );
                    e.Blocked = true;
                    e.Handled = true;
                }
            }
            else if( Insensitive.Equals( OfferString, e.Speech ) )
            {
                StartOffer( mobile );

                e.Blocked = true;
                e.Handled = true;
            }
            else if( Insensitive.Equals( ThitesString, e.Speech ) )
            {
                TellThites( mobile );

                e.Blocked = true;
                e.Handled = true;
            }
        }

        #region [tithes]

        private const string OfferString = "Offero In Nomine Virtute";

        private const string ThitesString = "Vis Mea";

        private static void TellThites( Mobile mobile )
        {
            if( mobile == null || !mobile.Alive )
                return;

            if( !IsPaladine( mobile ) )
                mobile.SendMessage( "Thou're not a Holy Paladine." );
            else
            {
                mobile.PlaySound( 0x1fa ); // 506
                mobile.SendMessage( "Thou have {0} tithes.", mobile.TithingPoints );
            }
        }

        private static void StartOffer( Mobile mobile )
        {
            if( mobile == null || !mobile.Alive )
                return;

            if( !IsPaladine( mobile ) )
            {
                mobile.SendMessage( "Thou're not a Holy Paladine." );
                return;
            }

            Item ankh = FindAnkh( mobile );
            if( ankh == null )
                mobile.SendMessage( "Thou're not near a holy ankh." );
            else if( mobile.Mounted )
                mobile.SendMessage( "You cant do that while mounted." );
            else
                mobile.SendGump( new TithingGump( mobile, 0 ) );
        }
        #endregion

        #region [pray]

        private const string PrayString = "Verae Virtutes Deservo";

        private const int PraySound = 0x24a; // 586

        private static readonly int[] RandomSparkleList = new int[] { 0x373A, 0x374A, 0x375A, 0x3779 };

        public static int RandomSparkle()
        {
            return Utility.RandomList( RandomSparkleList );
        }

        public static void PlayRandomWhiteSparkle( IEntity e )
        {
            Effects.SendLocationEffect( new Point3D( e.X, e.Y, e.Z + 1 ), e.Map, RandomSparkle(), 16, 0x920, 0 );
        }

        private static bool CanPray( Midgard2PlayerMobile mobile )
        {
            ClassPlayerState state = mobile.ClassState;
            if( state == null )
                return false;

            if( state.HasPrayed )
            {
                mobile.SendMessage( "Your soul does not require any pray now." );
                return false;
            }
            else if( mobile.Mounted )
            {
                mobile.SendMessage( "You cant pray while mounted." );
                return false;
            }
            else if( mobile.Weapon != null && !( mobile.Weapon is Fists ) )
            {
                mobile.SendMessage( "You cant pray while armed." );
                return false;
            }
            else
                return true;
        }

        private const double AnimateDelay = 1.5;
        private const double AnimateDuration = 6.0;

        private static Item FindAnkh( Mobile paladine )
        {
            Item ankh = null;
            foreach( Item item in paladine.GetItemsInRange( 2 ) )
            {
                if( item is AnkhNorth || item is AnkhWest )
                {
                    ankh = item;
                    break;
                }
            }

            return ankh;
        }

        private static void StartPray( Mobile mobile )
        {
            Item ankh = FindAnkh( mobile );
            if( ankh == null )
            {
                mobile.SendMessage( "Thou're not near a holy ankh." );
                return;
            }

            string message = mobile.TrueLanguage == LanguageType.Ita
                                 ? "* congiunge le mani in un atto di preghiera *"
                                 : "* prays *";

            mobile.PublicOverheadMessage( MessageType.Regular, Utility.RandomMinMax( 90, 95 ), true, message );

            int count = (int)Math.Ceiling( AnimateDuration / AnimateDelay );
            if( count > 0 )
            {
                new PrayAnimTimer( mobile, count ).Start();
                double effectiveDuration = ( count * AnimateDelay ) + 1.0;
                mobile.Freeze( TimeSpan.FromSeconds( effectiveDuration ) );
            }

            Timer.DelayCall( TimeSpan.FromSeconds( 3.0 ), new TimerStateCallback( Pray_Callback ), new object[] { mobile } );
        }

        private static void Pray_Callback( object state )
        {
            object[] states = (object[])state;

            Mobile paladine = (Mobile)states[ 0 ];

            paladine.Animate( (int)Animations.AreaEffectSpellcast, 7, 1, true, false, 0 );
            PlayRandomWhiteSparkle( paladine );
            paladine.PlaySound( 0x5c8 ); // 1480
            paladine.PublicOverheadMessage( MessageType.Regular, Utility.RandomMinMax( 90, 95 ), true, PrayString );

            Item ankh = FindAnkh( paladine );
            if( ankh != null )
                ankh.PublicOverheadMessage( MessageType.Regular, 17, true, "* you soul is satisfied *" );

            string message = paladine.TrueLanguage == LanguageType.Ita
                                 ? "Per la durata di due ore guadagnerai molti più punti fede per  ogni avversario malvagio che cadrà sotto la sua mano benedetta."
                                 : "For two hours, every evil creature being slayed by your holy hand will increase you holy power.";

            paladine.SendMessage( message );

            ClassPlayerState classState = ClassPlayerState.Find( paladine );
            if( classState != null )
                classState.LastPrayed = DateTime.Now;
        }

        private class PrayAnimTimer : SimpleAnimationTimer
        {
            public PrayAnimTimer( Mobile from, int count )
                : base( from, new int[] { 32 }, count )
            {
            }

            public override void Animate( Mobile from )
            {
                if( !From.Mounted && From.Body.IsHuman )
                    From.Animate( GetAnimId(), 7, 1, true, false, 0 );

                PlayRandomWhiteSparkle( from );
                From.PlaySound( 0x5c8 ); // 1480
            }
        }

        public static TimeSpan GetPrayDuration( Mobile m )
        {
            return TimeSpan.FromHours( m == null ? 24.0 : 2.0 );
        }

        public static void EmotePray( Mobile mobile )
        {
            if( mobile == null )
                return;

            int animId = Utility.RandomList( (int)Animations.AreaEffectSpellcast, (int)Animations.DirectionalSpellcast, (int)Animations.Salute );
            animId = AnimsOnMount.GetAnim( animId, mobile.Mount != null );
            mobile.Animate( animId, 5, 1, true, false, 0 );

            mobile.FixedEffect( RandomSparkle(), 10, 30 );
            mobile.PlaySound( PraySound );
        }
        #endregion

        #region [noto]
        public static bool IsAggressor( Mobile aggressor, Mobile aggressed )
        {
            return IsAggressor( aggressed.Aggressors, aggressor );
        }

        private static bool IsAggressor( IEnumerable<AggressorInfo> list, Mobile aggressor )
        {
            foreach( AggressorInfo t in list )
            {
                if( t.Attacker == aggressor )
                    return true;
            }

            return false;
        }

        public static bool IsAggressed( Mobile aggressor, Mobile aggressed )
        {
            return IsAggressed( aggressed.Aggressed, aggressor );
        }

        private static bool IsAggressed( IEnumerable<AggressorInfo> list, Mobile aggressor )
        {
            foreach( AggressorInfo t in list )
            {
                if( t.CriminalAggression && t.Defender == aggressor )
                    return true;
            }

            return false;
        }
        #endregion

        public static void RegisterKill( Mobile paladine, Mobile killed )
        {
            if( Config.Enabled )
            {
                if( paladine == null || !IsPaladine( paladine ) )
                    return;

                if( killed != null )
                {
                    int chival = paladine.Skills[ SkillName.Chivalry ].BaseFixedPoint;
                    int paladineKarma = paladine.Karma;
                    int killedKarma = killed.Karma;
                    int killedFame = Math.Abs( killed.Fame );

                    if( killedKarma > 0 )
                        return;

                    const double demultiFactor = 0.005;

                    double karmaFactor = 0.5 + ( paladineKarma / 10000.0 );
                    if( karmaFactor > 1.5 )
                        karmaFactor = 1.5;
                    else if( karmaFactor < 0.5 )
                        karmaFactor = 0.5;

                    double skillFactor = 0.5 + ( chival / 1000.0 );
                    if( skillFactor > 1.5 )
                        skillFactor = 1.5;
                    else if( skillFactor < 0.5 )
                        skillFactor = 0.5;

                    double fameFactor = 1.0 + ( killedFame / 5000.0 );
                    if( fameFactor > 2.0 )
                        fameFactor = 2.0;

                    int thiteAward = (int)Math.Abs( ( killedKarma * demultiFactor * karmaFactor * skillFactor * fameFactor ) );

                    bool hasPrayed = false;
                    ClassPlayerState state = ClassPlayerState.Find( paladine );
                    if( state != null )
                        hasPrayed = state.HasPrayed;

                    thiteAward = (int)( hasPrayed ? ( thiteAward * 1.1 ) : ( 0.5 * thiteAward ) );

                    if( thiteAward > 1000 )
                        thiteAward = 1000;
                    else if( thiteAward < 5 )
                        thiteAward = 5;

                    if( paladine.PlayerDebug )
                    {
                        paladine.SendMessage( "Debug Thites Award: killedKarma {0}, demultiFactor {1}, karmaFactor {2}, fameFactor {3}, thiteAward {4}.",
                                          killedKarma, demultiFactor.ToString( "F2" ), karmaFactor.ToString( "F2" ), fameFactor.ToString( "F2" ), thiteAward );
                    }

                    paladine.TithingPoints += thiteAward;

                    if( paladine.BeginAction( typeof( PaladinSystem ) ) )
                    {
                        paladine.SendMessage( "You earned {0} thites for slaying the creature.", thiteAward );
                        Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerStateCallback( EndMessageLock ), paladine );
                    }
                }
            }
        }

        private static void EndMessageLock( object state )
        {
            ( (Mobile)state ).EndAction( typeof( PaladinSystem ) );
        }

        public override BaseClassAttributes GetNewPowerAttributes( ClassPlayerState classPlayerState )
        {
            return new PaladinAttributes( classPlayerState );
        }

        public override bool CanEquipWeapon( Mobile from, BaseWeapon weapon, bool message )
        {
            if( weapon != null && weapon.Layer == Layer.TwoHanded )
            {
                if( message )
                    from.SendMessage( "Thou cannot wear two-handed weapons." );
                return false;
            }
            return true;
        }
    }
}