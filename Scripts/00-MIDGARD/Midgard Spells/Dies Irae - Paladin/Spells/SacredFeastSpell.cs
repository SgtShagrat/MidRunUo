/***************************************************************************
 *                               SacredFeastSpell.cs
 *
 *   begin                : 05 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Items;
using Server.Network;
using Server.Spells;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
    public class SacredFeastSpell : RPGPaladinSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Sacred Feast", "Convivium Consecratum",
            266,
            9002
            );

        private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
            (
            typeof( SacredFeastSpell ),
            m_Info.Name,
            "This Miracle turn the substance of the food into a living bless.",
            "Questo Miracolo benedice il cibo toccato dal Paladino.",
            "Benedice il pane, l'uva o le mele infondendo poteri particolari a seconda del tipo di cibo.<br>" +
            "Il cibo benedetto ha anche un bonus continuativo (4 livello) e alla rigenerazione.<br>" +
            "Pane: cura le ferite, aumenta la str (4 livello), aumenta la rigenerazione hp.<br>" +
            "Uva: ristora il mana, aumenta la int (4 livello), aumenta la rigenerazione del mana.<br>" +
            "Mela: ristora la stamina, aumenta la dex (4 livello), aumenta la rigenerazione della stamina.<br>" +
            "L'effetto istantaneo avviene quando si mangia il cibo. L'effetto duraturo dura PowerValueScaled secondi.<br>",
            0x5106
            );

        public override ExtendedSpellInfo ExtendedInfo
        {
            get { return m_ExtendedInfo; }
        }

        public override string GetModificationsForChangelog()
        {
            return "Modificati gli effetti dei cibi benedetti. Hanno anche un bonus continuativo (dal quarto livello) e alla rigenerazione.<br>" +
                "Pane: cura le ferite, aumenta la str (4 livello), aumenta la rigenerazione hp.<br>" +
                "Uva: ristora il mana, aumenta la int (4 livello), aumenta la rigenerazione del mana.<br>" +
                "Mela: ristora la stamina, aumenta la dex (4 livello), aumenta la rigenerazione della stamina.<br>" +
                "L'effetto istantaneo avviene quando si mangia il cibo. L'effetto duraturo dura PowerValueScaled secondi.<br>";
        }

        public override SpellCircle Circle
        {
            get { return SpellCircle.Second; }
        }

        public SacredFeastSpell( Mobile caster, Item scroll )
            : base( caster, scroll, m_Info )
        {
        }

        public override void OnCast()
        {
            if( CheckSequence() )
            {
                Caster.BeginTarget( 12, false, TargetFlags.None, new TargetStateCallback( Target ), Caster );
                Caster.SendLangMessage( 10000901 ); // "Choose the food you would to bless!" );
            }
        }

        public void Target( Mobile from, object obj, object state )
        {
            Item item = obj as Item;

            if( item != null && !item.Deleted )
            {
                if( item.IsChildOf( from.Backpack ) )
                {
                    if( item.Amount == 1 )
                    {
                        if( item is BaseMagicalFood )
                            Caster.SendLangMessage( 10000902 ); // "That food is already blessed" );
                        else
                        {
                            if( CanBeSacredFood( item ) )
                                BeginBless( Caster, item );
                            else
                                Caster.SendLangMessage( 10000903 ); //  "You may not bless that" );
                        }
                    }
                    else
                        Caster.SendLangMessage( 10000904 ); // "You may only bless one piece of food at one time" );
                }
                else
                    Caster.SendLangMessage( 10000905 ); //  "You may only bless food in your backpack" );
            }
            else
                Caster.SendLangMessage( 10000906 ); //  "You may not bless that" );

            FinishSequence();
        }

        private const double AnimateDelay = 1.5;
        private const double EnchantDuration = 3.0;
        private static readonly int[] m_AnimIds = new int[] { 245, 266 };

        private static readonly string[] HolyWords = new string[]
                                                     {
                                                         "* I Bless Thy *",
                                                         "* Protect my soul from Evil *",
                                                         "* I give you my spirit, give me your power *",
                                                         "* Let slay my enemies *",
                                                         "* Clean my sins *"
                                                     };

        private static readonly string[] HolyWordsITA = new string[]
                                                     {
                                                         "* Io Ti Benedico *",
                                                         "* Proteggi la mia anima dal Male*",
                                                         "* Ti ho dato il mio spirito, donami il tuo potere *",
                                                         "* Sconfiggi i miei nemici *",
                                                         "* Pulisci la mia anima *"
                                                     };

        private static void BeginBless( Mobile from, Item item )
        {
            if( from.CanBeginAction( typeof( SacredFeastSpell ) ) )
            {
                int count = (int)Math.Ceiling( EnchantDuration / AnimateDelay );

                if( count > 0 )
                {
                    AnimTimer animTimer = new AnimTimer( from, count );
                    animTimer.Start();

                    double effectiveDuration = ( count * AnimateDelay ) + 1.0;
                    from.Freeze( TimeSpan.FromSeconds( effectiveDuration ) );
                    Timer.DelayCall( TimeSpan.FromSeconds( effectiveDuration ), new TimerStateCallback( Bless_Callback ), new object[] { from, item } );
                }
            }
            else
            {
                from.SendLangMessage( 10000907 ); // "You cannot do anything yet."
            }
        }

        private class AnimTimer : Timer
        {
            private readonly Mobile m_From;

            public AnimTimer( Mobile from, int count )
                : base( TimeSpan.Zero, TimeSpan.FromSeconds( AnimateDelay ), count )
            {
                m_From = from;

                Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                if( !m_From.Mounted && m_From.Body.IsHuman )
                    m_From.Animate( Utility.RandomList( m_AnimIds ), 7, 1, true, false, 0 );

                string words = m_From.TrueLanguage == LanguageType.Ita ?
                    HolyWordsITA[ Utility.Random( HolyWordsITA.Length ) ] :
                    HolyWords[ Utility.Random( HolyWords.Length ) ];

                m_From.PublicOverheadMessage( MessageType.Regular, 17, true, words, true );

                for( int i = 0; i < 3; i++ )
                {
                    Point3D p = new Point3D( m_From.X + Utility.RandomMinMax( -1, 1 ), m_From.Y + Utility.RandomMinMax( -1, 1 ), m_From.Z + Utility.RandomList( -1, +3, +7, +11 ) );

                    if( m_From.Map != Map.Internal )
                    {
                        Effects.SendLocationEffect( p, m_From.Map, 0x3709, 30, 10, 1154, 0 );
                        m_From.PlaySound( 0x208 );
                    }
                }
            }
        }

        private static bool CanBeSacredFood( Item item )
        {
            if( item == null || item.Deleted )
                return false;

            if( !String.IsNullOrEmpty( item.Name ) )
                return false;

            if( item is BreadLoaf || item is Grapes || item is Apple )
                return true;
            else
                return false;
        }

        private static void Bless_Callback( object state )
        {
            object[] states = (object[])state;

            Mobile from = (Mobile)states[ 0 ];
            Item toBless = (Item)states[ 1 ];

            if( toBless == null || toBless.Deleted )
                return;

            Item holyFood = CreateHolyFood( toBless );

            if( holyFood != null )
            {
                from.FixedParticles( 0, 10, 5, 2003, EffectLayer.RightHand );
                from.PlaySound( 0x1FA );
                from.AddToBackpack( holyFood );

                if( !toBless.Deleted )
                    toBless.Delete();

                string message = TextHelper.Text( 10000908, from.TrueLanguage );
                from.LocalOverheadMessage( MessageType.Regular, 0x3B2, true, message ); // "* You infused your holy spirit upon this food *"
            }

            Effects.SendLocationEffect( from, from.Map, 14201, 16 );

            from.EndAction( typeof( SacredFeastSpell ) );
        }

        private static Item CreateHolyFood( Item toBless )
        {
            Item item = null;
            Type type = null;

            if( toBless is BreadLoaf )
                type = typeof( HolyBread );
            else if( toBless is Grapes )
                type = typeof( HolyGrapes );
            else if( toBless is Apple )
                type = typeof( HolyApple );

            if( type == null )
                return null;

            try
            {
                item = (Item)Activator.CreateInstance( type );
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }

            return item;
        }
    }
}