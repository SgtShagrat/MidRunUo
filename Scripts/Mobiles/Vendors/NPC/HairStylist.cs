using System;
using System.Collections.Generic; 
using Server;

using Midgard.Gumps;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
    public class HairStylist : BaseVendor
    {
		private List<SBInfo> m_SBInfos = new List<SBInfo>(); 
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } } 

        [Constructable]
		public HairStylist() : base( "the hair stylist" ) 
        {
            SetSkill( SkillName.Alchemy, 80.0, 100.0 );
            SetSkill( SkillName.Magery, 90.0, 110.0 );
            SetSkill( SkillName.TasteID, 85.0, 100.0 );
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add( new SBHairStylist() );
        }

        #region mod by Dies Irae
        private static string HairString = "cut my hair";
        private static string BearString = "shave my beard";

        public override bool HandlesOnSpeech( Mobile from )
        {
            return from.InRange( Location, 2 ) || base.HandlesOnSpeech( from );
        }

        public override void OnSpeech( SpeechEventArgs e )
        {
            Mobile from = e.Mobile;

            if( !e.Handled && from is PlayerMobile && from.InRange( Location, 2 ) )
            {
                if( e.Speech.ToLower().Contains( HairString ) )
                {
                    if( !( Banker.GetBalance( from ) >= 100 ) )
                    {
                        Say( "I'm sorry. You lack funds to access my services." );
                    }
                    else if( IsWorking )
                    {
                        Say( "I'm busy now, i'm sorry." );
                    }
                    else
                    {
                        Say( "For sure my friend." );
                        Say( "Please, select your new style!" );
                        from.SendGump( new OldHairSelectionGump( from, this ) );
                    }

                    e.Handled = true;
                }
                else if( e.Speech.ToLower().Contains( BearString ) )
                {
                    if( !( Banker.GetBalance( from ) >= 100 ) )
                    {
                        Say( "I'm sorry. You lack funds to access my services." );
                    }
                    else if( IsWorking )
                    {
                        Say( "I'm busy now, i'm sorry." );
                    }
                    else
                    {
                        Say( "For sure my friend." );
                        Say( "Please, select your new style!" );
                        from.SendGump( new OldBeardSelectionGump( from, this ) );
                    }

                    e.Handled = true;
                }
            }

            base.OnSpeech( e );
        }

        private static void AddHair( IEntity from )
        {
            new CutHair().MoveToWorld( from.Location, from.Map );

            int extraBlood = Utility.RandomMinMax( 0, 1 );

            for( int i = 0; i < extraBlood; i++ )
            {
                new CutHair().MoveToWorld( new Point3D(
                    from.X + Utility.RandomMinMax( -1, 1 ),
                    from.Y + Utility.RandomMinMax( -1, 1 ),
                    from.Z ), from.Map );
            }
        }

        private WorkTimer m_Timer;

        public void StartWorking( Mobile m, BeardStyleInfo info )
        {
            if( IsWorking )
                m_Timer.Stop();

            double delay = WorkTimer.AnimateDelay;
            int count = (int)Math.Ceiling( Utility.RandomMinMax( 3, 5 ) / delay ) + 1;
            double effectiveDuration = ( count * delay ) + 1.0;

            m_Timer = new WorkTimer( this, m, count );
            m_Timer.Start();

            Timer.DelayCall( TimeSpan.FromSeconds( effectiveDuration ), new TimerStateCallback( MakeNewBeardCallback ), new object[] { info, m } );
        }

        public void StartWorking( Mobile m, HairStyleInfo info )
        {
            if( IsWorking )
                m_Timer.Stop();

            double delay = WorkTimer.AnimateDelay;
            int count = (int)Math.Ceiling( Utility.RandomMinMax( 3, 5 ) / delay ) + 1;
            double effectiveDuration = ( count * delay ) + 1.0;

            m_Timer = new WorkTimer( this, m, count );
            m_Timer.Start();

            Timer.DelayCall( TimeSpan.FromSeconds( effectiveDuration ), new TimerStateCallback( MakeNewHairCallback ), new object[] { info, m } );
        }

        private void MakeNewBeardCallback( object state )
        {
            object[] states = state as object[];
            if( states == null )
                return;

            BeardStyleInfo info = states[ 0 ] as BeardStyleInfo;
            if( info == null )
                return;

            Mobile from = states[ 1 ] as Mobile;
            if( from == null )
                return;

            if( from is PlayerMobile )
            {
                PlayerMobile pm = (PlayerMobile)from;
                pm.SetHairMods( -1, -1 ); // clear any hairmods (disguise kit, incognito)
            }

            from.FacialHairItemID = info.MaleItemID;
            Say( "That's done. Wonderful!" );
            Banker.Withdraw( from, 50 );

            StopWorking();
        }

        private void MakeNewHairCallback( object state )
        {
            object[] states = state as object[];
            if( states == null )
                return;

            HairStyleInfo info = states[ 0 ] as HairStyleInfo;
            if( info == null )
                return;

            Mobile from = states[ 1 ] as Mobile;
            if( from == null )
                return;

            if( from is PlayerMobile )
            {
                PlayerMobile pm = (PlayerMobile)from;
                pm.SetHairMods( -1, -1 ); // clear any hairmods (disguise kit, incognito)
            }

            from.HairItemID = info.GetItemID( from );
            Say( "That's done. Wonderful!" );
            Banker.Withdraw( from, 100 );

            StopWorking();
        }

        private void StopWorking()
        {
            if( m_Timer != null )
                m_Timer.Stop();

            m_Timer = null;
        }

        private bool IsWorking
        {
            get
            {
                return m_Timer != null;
            }
        }

        private class WorkTimer : Timer
        {
            private static readonly int[] AnimIds = new int[] { 245, 266 };

            public const double AnimateDelay = 1.5;

            private readonly HairStylist m_Stylish;
            private readonly Mobile m_From;

            public WorkTimer( HairStylist stylish, Mobile from, int count )
                : base( TimeSpan.Zero, TimeSpan.FromSeconds( AnimateDelay ), count )
            {
                m_Stylish = stylish;
                m_From = from;

                Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                if( m_From == null || m_From.Deleted || m_Stylish == null || m_Stylish.Deleted )
                {
                    Stop();
                    return;
                }

                if( !m_From.InRange( m_Stylish.Location, 3 ) )
                {
                    m_From.SendMessage( "You are too far away to continue." );
                    if( Running )
                        Stop();

                    return;
                }

                SpellHelper.Turn( m_Stylish, m_From );
                m_Stylish.Animate( Utility.RandomList( AnimIds ), 7, 1, true, false, 0 );
                m_Stylish.PlaySound( 0x248 );
                AddHair( m_From );
            }
        }
        #endregion

        public HairStylist( Serial serial )
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
    }
}