/***************************************************************************
 *                                  DeceitCrystalBall.cs
 *                            		--------------------
 *  begin                	: July 29th, 2009
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server.Network;

namespace Server.Items
{
    public class DeceitCrystalBall : Item
    {
        private static string[] m_RandomMessage = new string[]
                                                      {
            "The shimmering clouds have revealed a dark destiny, one wrought with peril.",
            "The cloudy mist of the all-seeing eye have revealed a path filled with still waters.",
            "The clouds of time reveal the ghosts of the past, still priesting over their congregation.",
            "Ye shant not fail, have you the eye of an eagle, the strength of an ox and the nibleness of a wolverine.",
            "The clouds reveal a philosopher, a priest and three thousand leagues of skeletons marching row by row.",
            "The halls of fate, the halls of doom, in wells of thought, one might find room.",
            "Ghastly shadows on forlorn walls, echo the death of foes and falls.",
            "Beware ye who pass this way, for darkness works in mysterious ways.",
            "On bended knee they came and slew, ten thousand souls for the freedom of Yew.",
            "Brave hearts dive in and yet not return, their fates swallowed by the fires that burn.",
            "Once a treasure was laid at the bottom of the tomb, in darkness and light, its wealth did bloom.",
            "Cast not one eye to the flames' disguise, for marching in order are invisible eyes."
                                                      };

        public override bool DisplayLootType { get { return false; } }
        public override bool DisplayWeight { get { return false; } }

        private PredictionTimer m_Timer = null;

        [Constructable]
        public DeceitCrystalBall()
            : base( 0xE2D )
        {
            Weight = 1.0;
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( m_Timer != null && m_Timer.Running )
            {
                from.SendMessage( "A little bit of patience my lord..." );
                return;
            }

            // only for debug
            if( from.AccessLevel > AccessLevel.Player )
            {
                BarkOracleMessage( from );
                return;
            }

            if( DateTime.Now.Hour >= 20 )
            {
                BarkOracleMessage( from );
            }
            else
            {
                BarkOracleRandomMessage();
            }
        }

        private void BarkOracleRandomMessage()
        {
            SendMessage( m_RandomMessage[ Utility.Random( m_RandomMessage.Length ) ] );
        }

        public void SendMessage( string message )
        {
            PublicOverheadMessage( MessageType.Regular, 0x3B2, true, message );
        }

        private void BarkOracleMessage( Mobile from )
        {
            int rawStr = from.RawStr;
            int rawInt = from.RawInt;
            int rawDex = from.RawDex;

            int karmaFame = from.Fame > 0 ? from.Fame : from.Karma;
            string name = from.Name;
            bool female = from.Female;

            int random = Utility.Random( 4 );

            Console.WriteLine( "BarkOracleMessage random: " + random );
            List<string> prediction = new List<string>();

            switch( Utility.Random( 4 ) )
            {
                case 0:

                    if( rawStr >= 50 && ( rawInt >= 50 ) && ( rawDex >= 50 ) )
                    {
                        prediction.Add( "Thou shant not fear, the dangers near, sword, mind and nimble feet shall clear." );
                    }
                    else if( rawStr >= 50 && !( rawInt >= 50 ) && !( rawDex >= 50 ) )
                    {
                        prediction.Add( "A warrior in thee, I see, for the depths of this place should challenge thee." );
                    }
                    else if( rawInt >= 50 && !( rawStr >= 50 ) && !( rawDex >= 50 ) )
                    {
                        prediction.Add( "Strong of mind, must bind, the tricks of the depths that ye shall find." );
                    }
                    else if( rawDex >= 50 && !( rawStr >= 50 ) && !( rawInt >= 50 ) )
                    {
                        prediction.Add( "Nimble of feet, thou shall defeat, what traps of the depths ye mayhaps meet." );
                    }
                    else
                    {
                        prediction.Add( "The dangers one must decide, they do reside, and fear might be thy only guide." );
                    }
                    break;

                case 1:

                    if( rawInt >= 50 )
                    {
                        prediction.Add( "Crying shame, 'twas but a game, only thee might uncover the philosopher's name." );
                    }
                    else
                    {
                        prediction.Add( "'Tis but a shame, the secret remains, in the depths of this deep and dark domain." );
                    }
                    break;

                case 2:

                    if( rawStr < 50 && ( rawInt < 50 ) && ( rawDex < 50 ) )
                    {
                        prediction.Add( "Fear thee well," );
                        prediction.Add( "Thou canst not tell," );
                        prediction.Add( "What beast shall feast," );
                        prediction.Add( "Once thou hast fell." );
                    }

                    List<string> temp = new List<string>();

                    if( rawStr >= 50 )
                    {
                        prediction.Add( "Strength in thy arm," );
                        temp.Add( "Thy foes ye meet, no doubt ye should harm." );
                    }
                    else if( rawDex >= 50 )
                    {
                        prediction.Add( "Swift in thy feet," );
                        temp.Add( "Thy traps ye find, ye shall defeat." );
                    }
                    else if( rawInt >= 50 )
                    {
                        prediction.Add( "Thy mind shant not flee," );
                        temp.Add( "The treasures within, should become part of thee." );
                    }

                    prediction.Add( temp[ Utility.Random( temp.Count ) ] );
                    break;

                case 3:

                    string title = "";

                    if( karmaFame < -350 )
                    {
                        title = "foul";
                    }
                    else if( karmaFame > 350 )
                    {
                        title = "most honorable";
                    }
                    else if( karmaFame >= -350 && karmaFame <= 350 )
                    {
                        title = "good";
                    }

                    string sexTitle = !female ? "sir" : "lady";

                    prediction.Add( string.Format( "{0}, {1} {2}, thou hast thy bravery at least.", name, title, sexTitle ) );

                    break;

                case 4:

                    string speech;
                    if( karmaFame > 350 )
                    {
                        speech = "Your reputation, " + name + ", it doth preceed thee,";
                        prediction.Add( speech );
                        prediction.Add( "Your fate doth swirl, I canst not see." );
                    }
                    else if( karmaFame >= ( 0 - 350 ) && ( karmaFame <= 350 ) )
                    {
                        speech = "I know not of you, " + name + ", your fate is shrouded in mystery.";
                        prediction.Add( speech );
                    }
                    else if( karmaFame < ( 0 - 350 ) )
                    {
                        speech = "Foul beasts within, whom ye may find good company, most wicked " + name + ".";
                        prediction.Add( speech );
                    }

                    break;
            }

            if( m_Timer != null )
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            m_Timer = new PredictionTimer( prediction.ToArray(), this );
            m_Timer.Start();
        }

        #region serial-deserial
        public DeceitCrystalBall( Serial serial )
            : base( serial )
        {
        }

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

        private sealed class PredictionTimer : Timer
        {
            private DeceitCrystalBall m_Owner;
            private string[] m_Prediction;

            private int m_Index = 0;

            public PredictionTimer( string[] prediction, DeceitCrystalBall owner )
                : base( TimeSpan.Zero, TimeSpan.FromSeconds( 0.25 ) )
            {
                m_Owner = owner;
                m_Prediction = prediction;

                Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                if( m_Index < 0 || m_Index > m_Prediction.Length - 1 )
                {
                    Stop();
                }
                else
                {
                    m_Owner.SendMessage( m_Prediction[ m_Index ] );
                    m_Index++;
                }
            }
        }
    }
}