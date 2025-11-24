/***************************************************************************
 *                                  BaseResistancePotion.cs
 *                            		-----------------------
 *  begin                	: Gennaio, 2007
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info:
 *  		Classe base per l'implementazione di pozioni che diano dei bonus
 * 			alle resistenze.
 ***************************************************************************/

using System;

namespace Server.Items
{
    public abstract class BaseResistancePotion : BasePotion
    {
        #region proprieta
        public abstract int Level { get; }
        public abstract double PercProperFun { get; }

        public abstract ResistanceType ResType { get; }
        #endregion

        #region costruttori
        public BaseResistancePotion( PotionEffect effect, int amount )
            : base( 0xF0E, effect, amount )
        {
        }

        public BaseResistancePotion()
            : this( 1 )
        {
        }

        public BaseResistancePotion( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override int LabelNumber { get { return 1064000 + (int)PotionEffect; } } // da 1064150 a 1064161

        public override void Drink( Mobile from )
        {
            if( DoResistanceEffect( from ) )
            {
                PlayDrinkEffect( from );

                from.FixedParticles( 0x373A, 10, 15, 5012, EffectLayer.Waist );
                from.PlaySound( 0x1E0 );

                Consume();
            }
        }

        public bool DoResistanceEffect( Mobile from )
        {
            if( from.BeginAction( typeof( BaseResistancePotion ) ) )
            {
                // La percentuale che la pozione funzioni è scalabile con EP
                double chance = Scale( from, PercProperFun );

                if( chance > Utility.RandomDouble() )
                {
                    // La modifica alle res è di Level * 5
                    int resMod = Level * 5;

                    // Viene poi scalata con EP
                    resMod = Scale( from, resMod );

                    // La durata e' Level * 30 secondi
                    int duration = Level * 30;

                    // Viene poi scalata con EP
                    duration = Scale( from, duration );

                    // Creazione del modificatore alle res
                    ResistanceMod mod = new ResistanceMod( ResType, +resMod );

                    // Applicazione del modificatore
                    from.AddResistanceMod( mod );

                    // Fa partire il timer per la durata del bonus alla res
                    Timer t = new InternalTimer( this, from, TimeSpan.FromSeconds( duration ), mod );
                    t.Start();

                    if( from.PlayerDebug )
                        from.SendMessage( "Debug Message: Avevi una chance del {0}% che la pozione di Resistenza Elementale funzionasse." +
                                            " Ottieni un bonus alla resistenza {1} di {2} punti per {3} secondi poichè" +
                                            " la durata base è {4} e la tua enhancepotions vale {5}%.",
                                            ( chance * 100 ).ToString(),
                                            ResType.ToString(),
                                            resMod,
                                            duration,
                                            ( Level * 30 ).ToString(),
                                            AosAttributes.GetValue( from, AosAttribute.EnhancePotions ).ToString() );
                    return true;
                }
                else
                {
                    // Se non funziona spreca la pozione fa partire il delaytimer per l'uso successivo e non alza la res
                    Timer.DelayCall( TimeSpan.FromSeconds( DelayUse ), new TimerStateCallback( ReleaseResistanceLock ), from );
                    from.SendMessage( "La pozione di Resistenza Elementale non funziona e viene distrutta." );
                    return true;
                }
            }
            else
            {
                // Se il lock all'uso non e' disabilitato...
                from.SendMessage( "Non puoi ancora usare un'altra pozione di Resistenza Elementale!" );
                return false;
            }
        }

        private static void ReleaseResistanceLock( object state )
        {
            ( (Mobile)state ).EndAction( typeof( BaseResistancePotion ) );
        }
        #endregion

        #region serial deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion

        #region internal timer
        private class InternalTimer : Timer
        {
            #region campi
            private BaseResistancePotion m_Pot;
            private Mobile m_From;
            private ResistanceMod m_Mod;
            #endregion

            #region costruttori
            public InternalTimer( BaseResistancePotion potion, Mobile from,
                                  TimeSpan duration,
                                  ResistanceMod mod )
                : base( duration )
            {
                Priority = TimerPriority.OneSecond;

                m_Pot = potion;
                m_From = from;

                m_Mod = mod;
            }
            #endregion

            #region metodi
            protected override void OnTick()
            {
                if( m_From != null )
                {
                    m_From.RemoveResistanceMod( m_Mod );
                    m_From.SendMessage( "La pozione si esaurisce e le tue resistenze ritornano normali!" );
                    Stop();
                    DelayCall( TimeSpan.FromSeconds( m_Pot.DelayUse ), new TimerStateCallback( ReleaseResistanceLock ), m_From );
                }
            }
            #endregion
        }
        #endregion
    }
}
