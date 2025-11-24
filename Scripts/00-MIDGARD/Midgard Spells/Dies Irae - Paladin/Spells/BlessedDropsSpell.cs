/***************************************************************************
 *                               BlessedDropsSpell.cs
 *
 *   begin                : 05 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Network;
using Server.Spells;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
    public class BlessedDropsSpell : RPGPaladinSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Blessed Drops", "Sanctae Stillae",
            266,
            9002
            );

        private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
            (
            typeof( BlessedDropsSpell ),
            "Blessed Drops",
            "This Miracle blesses the water in a special glass vial. That water is devasting for undead and evil creatures.",
            "Il Paladino con il suo potere e' in grado di benedire l'acqua contenuta in fiale appositamente riempite." +
            "Tali fiale saranno come fuoco per le creature malvagie.",
            "Il paladino benedisce una fiala che diventerà, se lanciata una temibile carica esplosiva contro il male." +
            "Le fiale sono create dai tinker. Le pozioni se lanciate danneggiano creature malvagie. Il danno è influenzato " +
            "dal karma del paladino (max 50% bonus) e dal livello dello spell (10% per livello). Le creature colpite fuggono (flee)." +
            "Il danno base è 20-40pf.",
            0x510A
            );

        public override ExtendedSpellInfo ExtendedInfo
        {
            get { return m_ExtendedInfo; }
        }

        public override string GetModificationsForChangelog()
        {
            return "Aggiunta la fiala in vendita dal Provisioner e dal Tinker<br>";
        }

        public override SpellCircle Circle
        {
            get { return SpellCircle.First; }
        }

        public override TimeSpan CastDelayBase
        {
            get { return TimeSpan.FromSeconds( 2.0 ); }
        }

        public BlessedDropsSpell( Mobile caster, Item scroll )
            : base( caster, scroll, m_Info )
        {
        }

        public override void OnCast()
        {
            if( CheckSequence() )
            {
                Caster.BeginTarget( 12, false, TargetFlags.None, new TargetStateCallback( Target ), Caster );
                Caster.SendLangMessage( 10000209, Caster.Name ); // "{0}, choose the water you want to bless."
            }

            FinishSequence();
        }

        private static double AnimateDelay = 1.5;
        private static double EnchantDuration = 6.0;
        private static readonly int[] m_AnimIds = new int[] { 245, 266 };

        private static readonly string[] HolyWords = new string[]
                                                     {
                                                         "* I Bless Thee *",
                                                         "* Protect my soul from Evil *",
                                                         "* I give you my heart, give me your power *",
                                                         "* Let slay my enemies *",
                                                         "* Clean my sins *"
                                                     };

        private static readonly string[] HolyWordsITA = new string[]
                                                     {
                                                         "* Io Ti Benedico *",
                                                         "* Proteggi la mia anima dal Male *",
                                                         "* Ti dono la mia anima, donami il tuo potere *",
                                                         "* Dammi la forza *",
                                                         "* Perdona i miei peccati *"
                                                     };

        public void Target( Mobile from, object obj, object state )
        {
            Item item = obj as Item;

            if( item != null && !item.Deleted )
            {
                if( item.IsChildOf( from.Backpack ) )
                {
                    if( item is BlessedDrop )
                    {
                        BlessedDrop water = item as BlessedDrop;

                        if( !water.Deleted )
                        {
                            if( water.Filled )
                            {
                                if( !water.HolyBlessed )
                                    BeginEnchant( from, water );
                                else
                                    from.LocalLangOverheadMessageTo( 10000201 ); // "* That bottle is already blessed *"
                            }
                            else
                                from.LocalLangOverheadMessageTo( 10000202 ); // "* That bottle is not filled yet *";
                        }
                        else
                            from.LocalLangOverheadMessageTo( 10000203 ); // "* You may not bless that *";
                    }
                    else
                        from.LocalLangOverheadMessageTo( 10000204 ); // "* You may not bless that *";
                }
                else
                    from.LocalLangOverheadMessageTo( 10000205 ); // "* You may only bless a filled bottle in your backpack *";
            }
            else
                from.LocalLangOverheadMessageTo( 10000206 ); // "* You may not bless that *";

            FinishSequence();
        }

        public void BeginEnchant( Mobile from, BlessedDrop water )
        {
            if( from.BeginAction( typeof( BlessedDrop ) ) )
            {
                int count = (int)Math.Ceiling( EnchantDuration / AnimateDelay );

                if( count > 0 )
                {
                    AnimTimer animTimer = new AnimTimer( from, count );
                    animTimer.Start();

                    double effectiveDuration = ( count * AnimateDelay ) + 1.0;
                    from.Freeze( TimeSpan.FromSeconds( effectiveDuration ) );
                    Timer.DelayCall( TimeSpan.FromSeconds( effectiveDuration ), new TimerStateCallback( EnchantCallback ), new object[] { from, water } );
                }
            }
            else
            {
                from.SendLangMessage( 10000207, from.Name ); // "{0}, you cannot do anything yet."
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

                for( int i = 0; i < 10; i++ )
                    DoCircle( m_From );
            }

            private static void DoCircle( Mobile from )
            {
                if( from.Map == Map.Internal )
                    return;

                Point3D p = from.Location;

                for( int i = 0; i < m_Offsets.Length; i += 2 )
                {
                    Point3D finalLoc = new Point3D( p.X + m_Offsets[ i ], p.Y + m_Offsets[ i + 1 ], p.Z );
                    Effects.SendLocationEffect( finalLoc, from.Map, 0x3709, 30, 10, 2024, 0 );
                }

                from.PlaySound( 0x208 );
            }

            private static readonly int[] m_Offsets = new int[]
                                                      {
                                                          -4, 1,
                                                          -3, 3,
                                                          -1, 4,
                                                          1, 4,
                                                          3, 3,
                                                          4, 1,
                                                          4, -1,
                                                          3, -3,
                                                          1, -4,
                                                          -1, -4,
                                                          -3, -3,
                                                          -4, -1
                                                      };
        }

        private static void EnchantCallback( object state )
        {
            object[] states = (object[])state;

            Mobile from = (Mobile)states[ 0 ];
            BlessedDrop water = (BlessedDrop)states[ 1 ];

            water.HolyBlessed = true;

            from.LocalLangOverheadMessageTo( 10000208 ); // "* You have successfully blessed that sacred water *"
            from.PlaySound( 0x1FA );
            Effects.SendLocationEffect( from, from.Map, 14201, 16 );

            water.Delta( ItemDelta.Update );

            from.EndAction( typeof( BlessedDrop ) );
        }
    }
}