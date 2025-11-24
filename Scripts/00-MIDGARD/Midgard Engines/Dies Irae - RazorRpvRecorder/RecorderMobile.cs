using System;
using System.Collections.Generic;
using System.Xml.Linq;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.RazorRpvRecorder
{
    public class RecorderMobile : PlayerMobile
    {
        private bool m_Recording;

        [Constructable]
        public RecorderMobile()
        {
            InitStats( 100, 100, 25 );

            IsSocketEmulated = true;

            Hue = 0x8455;
            Body = 0x190;
            Female = false;

            Direction = Direction.South;
            AccessLevel = AccessLevel.Counselor;
            Hits = HitsMax;
            Blessed = true;
            Frozen = true;

            Name = "Memory of Midgard";

            HoodedShroudOfShadows shroud = new HoodedShroudOfShadows();
            shroud.LootType = LootType.Blessed;
            EquipItem( shroud );

            Reset();
            InitRecorder();
        }

        /// <summary>
        ///     List of all instances of RecorderMobile class on the server
        /// </summary>
        private static List<RecorderMobile> RecorderMobileInstances { get; set; }

        internal RPVVideo RPVVideo { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Debug { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Recording
        {
            get { return m_Recording; }
            set
            {
                bool oldValue = m_Recording;
                if( value != oldValue )
                {
                    m_Recording = value;
                    OnIsRecordingChanged( oldValue );
                }
            }
        }

        /// <summary>
        ///     Used to take count into RecorderMobileInstances of all instances of RecorderMobile
        /// </summary>
        private void RegisterRecorderMobile()
        {
            if( RecorderMobileInstances == null )
                RecorderMobileInstances = new List<RecorderMobile>();

            if( !RecorderMobileInstances.Contains( this ) )
                RecorderMobileInstances.Add( this );
        }

        /// <summary>
        ///     Turn on/off all recording instances on the server
        /// </summary>
        /// <param name = "from">the mobile who sent the command (may be null if it is the system)</param>
        /// <param name = "enabled">true to enable recording, false otherwise</param>
        internal static void ToggleRecordingStates( Mobile from, bool enabled )
        {
            if( RecorderMobileInstances == null )
                return;

            foreach( RecorderMobile recorder in RecorderMobileInstances )
            {
                if( recorder != null && !recorder.Deleted )
                    recorder.Recording = enabled;
            }

            if( from != null )
                from.SendMessage( "All recording mobiles are {0}.", enabled ? "enabled" : "disabled" );
        }

        internal static void InitSystem()
        {
            EventSink.Crashed += new CrashedEventHandler( EventSink_Crashed );
            EventSink.Shutdown += new ShutdownEventHandler( EventSink_Shutdown );
        }

        public static void EventSink_Crashed( CrashedEventArgs e )
        {
            ToggleRecordingStates( null, false );
        }

        public static void EventSink_Shutdown( ShutdownEventArgs e )
        {
            ToggleRecordingStates( null, false );
        }

        /// <summary>
        ///     Reset the recording mobile state
        /// </summary>
        private void Reset()
        {
            if( RPVVideo != null )
                RPVVideo.Stop();

            m_Recording = false;
            RPVVideo = null;
        }

        /// <summary>
        ///     Initialize the recording netstate
        /// </summary>
        private void InitRecorder()
        {
            NetState = new RecordingNetState( this, null );

            if( Config.Debug )
                Config.Pkg.LogInfoLine( this + "> InitNetState with NULL socket" );

            if( Map == Map.Internal )
                Map = LogoutMap;

            RegisterRecorderMobile();
        }

        public virtual void StartRecording( object starter, IEnumerable<XElement> CustomInfo )
        {
            if( m_Recording )
            {
                if( starter is PlayerMobile )
                    PrivateOverheadMessage( MessageType.Regular, 0, true, "I'm already recording now.", ( (PlayerMobile)starter ).NetState );
                DebugSay( "I'm already recording." );
                return;
            }

            List<XElement> ret = new List<XElement>();
            if( CustomInfo != null )
                ret.AddRange( CustomInfo );

            if( starter != null )
                ret.Add( new XElement( "starter", new XAttribute( "type", starter.GetType().FullName ), new XAttribute( "name", starter ) ) );

            RPVVideo = new RPVVideo( this, ret );
            RPVVideo.Start();
            m_Recording = true;
        }

        /// <summary>
        ///     Event invoked when the Recording proprty is changed
        /// </summary>
        /// <param name = "oldValue"></param>
        public virtual void OnIsRecordingChanged( bool oldValue )
        {
            if( m_Recording )
            {
                DebugSay( "For sure my Lord, I will record anything!" );

                IEnumerable<XElement> oldCustomInfo = null;
                if( RPVVideo != null )
                {
                    oldCustomInfo = RPVVideo.CustomInfo;
                    RPVVideo.Stop();
                }

                RPVVideo = new RPVVideo( this, oldCustomInfo );
                RPVVideo.Start();
            }
            else
            {
                DebugSay( "Well, just a moment to finish my work." );

                if( RPVVideo != null )
                    RPVVideo.Stop();

                RPVVideo = null;

                DebugSay( "Well, my work is done!." );
            }
        }

        /// <summary>
        ///     Used to force a text over our recording mobile.
        ///     Active in debug mode only.
        /// </summary>
        /// <param name = "text">the string we want to log</param>
        public void DebugSay( string text )
        {
            if( Debug )
                PublicOverheadMessage( MessageType.Regular, 41, true, text );

            if( Config.Debug && Debug )
                Config.Pkg.LogInfoLine( this + "> Say: " + text );
        }

        /// <summary>
        ///     Used to force a text over our recording mobile.
        ///     Active in debug mode only.
        /// </summary>
        /// <param name = "format">the format of the string</param>
        /// <param name = "args">the arguments to fill the format</param>
        public void DebugSay( string format, params object[] args )
        {
            DebugSay( String.Format( format, args ) );
        }

        #region serialization
        public RecorderMobile( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );

            writer.Write( Debug );
            writer.Write( Recording );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            IsSocketEmulated = true; //force- Should Be into Mobile.Serialize

            int version = reader.ReadInt();

            Debug = reader.ReadBool();

            bool record = reader.ReadBool();

            // In any case Init the netstate and register our mobile
            Timer.DelayCall( TimeSpan.Zero, new TimerCallback( InitRecorder ) );

            if( record )
            {
                Timer.DelayCall( TimeSpan.Zero, new TimerCallback( delegate
                                                                     {
                                                                         Recording = true;
                                                                     } ) );
            }
        }
        #endregion
    }
}