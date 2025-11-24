/***************************************************************************
 *                             Death.cs
 *                            ----------
 *   author               : Dies Irae
 *   begin                : 28/09/2008
 *   copyright            : (C) Midgard Shard - Dies Irae
 *   email                : matteo_visintin@hotmail.com
 *
 ***************************************************************************/

/***************************************************************************
 *  Descrition:
 *
 *  A simple mobile which visits a player when dead, say a glitch and disappear.
 *  
 ***************************************************************************/
 
using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    public class VisitingDeath : BaseCreature
    {
        public static void SummonTheDeath( Mobile to )
        {
            if( to == null || to.Map == Map.Internal )
                return;

            Point3D loc = to.Location;
            Map map = to.Map;

            Effects.SendLocationParticles( EffectItem.Create( loc, map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 0, 0, 2023, 0 );
            Effects.PlaySound( loc, map, 0x1FE );

            new VisitingDeath( to );
        }

        private Mobile m_Visited;

        public override bool CanTeach
        {
            get
            {
                return false;
            }
        }

        public override bool ClickTitle
        {
            get
            {
                return false;
            }
        }

        [Constructable]
        public VisitingDeath( Mobile visited )
            : base( AIType.AI_Mage, FightMode.None, 10, 1, 0.8, 0.8 )
        {
            InitStats( 1000, 1000, 1000 );

            Hue = 0x8455;
            Body = 0x190;

            NameHue = 37;
            Blessed = true;

            SpeechHue = 37;
            EmoteHue = 37;

            Title = ", last comforter";
            Name = "The Death";

            AddItem( Immovable( Newbied( new HoodedShroudOfShadows() ) ) );
            AddItem( Immovable( Newbied( new Scythe() ) ) );

            m_Visited = visited;

            Visite();

            Timer.DelayCall( TimeSpan.FromSeconds( 2.0 ), new TimerCallback( DoSpeech ) );
            BeginRemove( TimeSpan.FromSeconds( 5.0 ) );
        }

        private void DoSpeech()
        {
            Direction = (Direction)( 7 & ( 4 + (int)GetDirectionTo( m_Visited ) ) );
            Say( true, GetSpeech() );
        }

        private void BeginRemove( TimeSpan delay )
        {
            Timer.DelayCall( delay, new TimerCallback( EndRemove ) );
        }

        private void EndRemove()
        {
            if( Deleted )
                return;

            Point3D loc = Location;
            Map map = Map;

            Effects.SendLocationParticles( EffectItem.Create( loc, map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 0, 0, 2023, 0 );
            Effects.PlaySound( loc, map, 0x1FE );

            Delete();
        }

        private static int[] m_Offsets = new int[]
			{
				-1, -1,
				-1,  0,
				-1,  1,
				0, -1,
				0,  1,
				1, -1,
				1,  0,
				1,  1
			};

        #region m_SpeechTable
        private static string[] m_SpeechTable = new string[]
            {
"Come With Me.",
                       "Thy Worldly Pain Is Ended.",
                       "I Have Come For Thee",
                       "Would You Like A Game Of Chess?",
                       "Your Life Does Pass Before Your Eyes Before You Die. The Process Is Called \"Living\".",
                       "Drop The Scythe, And Turn Around Slowly",
                       "There's No Justice. There's Just Me.",
                       "I Don't Know About You, But I Could Murder A Curry",
                       "It Would Be A Bloody Stupid World If People Got Killed Without Dying.",
                       "For What Can The Harvest Hope But The Care Of The Reaper Man",
                       "I Swear I Never Touched Him.",
                       "Awfuly Dark In Here, Isn't It?",
                       "Follow The Dark.",
                       "I Find The Best Approach Is To Take Life As It Comes.",
                       "I Am Death, Not Taxes. I Turn Up Only Once",
                       "I'm Death, And I Really Am Not Here To Take Your Money. Which Part Of This Don't You Understand?",
                       "I Was Expecting To Meet Thee In Destard.",
                       "What's It All About? Seriously? When You Get Down To It?",
                       "I Usher Souls Into The Next World. I Am The Grave Of All Hope. I Am The Ultimate Reality. I Am The Assassin Against Whom No Lock Would Hold",
                       "They All Hate Me. Everybody Hates Me. I Don't Have A Single Friend.",
                       "Wah Wah Wah Is That All You Ever Do?.",
                       "I Realy Hate That Chuck Noris Guy Talking All Day Long.",
                       "What Did You Expect - The Tooth Fairy.",
                       "Sorry Bud, Time Is Up.",
                       "This Way - Now You Need To Follow The White Light.",
                       "Death - Ya I Am Death, So What Of It.",
                       "Maybe You Should Try Something Easier Next Time.",
                       "Forget Your Armor Or Something.",
                       "Jay And Silent Who?",
                       "Now That Was Funny",
                       "The Happy Huntting Ground - Yes - Go North Through Vallhalla Then Make A Left - Can't Miss It",
                       "And You Thought All You Wanted Was Your 2 Front Teeth",
                       "What Ever",
                       "Talk To The Hand.",
                       "Drop Your Weapon?",
                       "Lets Go - I Do Not Have All Day You Know.",
                       "Come On Now - Run Run Run - Else Hades Might Catch Up To You.",
                       "All It Takes Is A Blink At The Wrong Time.",
                       "Its 11:00 Pm Do You Know Where Your Body Is?",
                       "Stop Complaining - The Ankh Is That Away",
                       "Smile - No One Can See You Naked Here.",
            };
        #endregion

        private static string GetSpeech()
        {
            return m_SpeechTable[ Utility.Random( m_SpeechTable.Length ) ];
        }

        private void Visite()
        {
            if( m_Visited != null && m_Visited.Map != Map.Internal )
            {
                int offset = Utility.Random( 8 ) * 2;
                bool foundLoc = false;

                for( int i = 0; i < m_Offsets.Length; i += 2 )
                {
                    int x = m_Visited.X + m_Offsets[ ( offset + i ) % m_Offsets.Length ];
                    int y = m_Visited.Y + m_Offsets[ ( offset + i + 1 ) % m_Offsets.Length ];

                    if( m_Visited.Map.CanSpawnMobile( x, y, m_Visited.Z ) )
                    {
                        MoveToWorld( new Point3D( x, y, m_Visited.Z ), m_Visited.Map );
                        foundLoc = true;
                        break;
                    }
                    else
                    {
                        int z = m_Visited.Map.GetAverageZ( x, y );

                        if( m_Visited.Map.CanSpawnMobile( x, y, z ) )
                        {
                            MoveToWorld( new Point3D( x, y, z ), m_Visited.Map );
                            foundLoc = true;
                            break;
                        }
                    }
                }

                if( !foundLoc )
                    MoveToWorld( m_Visited.Location, m_Visited.Map );

                Direction = (Direction)( 7 & ( 4 + (int)m_Visited.GetDirectionTo( m_Visited.Location ) ) );
            }
        }

        private static Item Immovable( Item item )
        {
            item.Movable = false;
            return item;
        }

        private static Item Newbied( Item item )
        {
            item.LootType = LootType.Newbied;
            return item;
        }

        #region serialization
        public VisitingDeath( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 );	// version 
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            reader.ReadInt();
        }
        #endregion
    }
}