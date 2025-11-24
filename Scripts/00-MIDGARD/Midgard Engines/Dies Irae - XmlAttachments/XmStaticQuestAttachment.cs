using System;
using System.Collections.Generic;
using Server.Engines.Quests;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.XmlSpawner2
{
    public class XmStaticQuestAttachment : XmlAttachment, IMidgardQuester
    {
        private DateTime m_Spoken;

        public virtual int AutoTalkRange { get { return -1; } }
        public virtual int AutoSpeakRange { get { return 10; } }
        public virtual TimeSpan SpeakDelay { get { return TimeSpan.FromMinutes( 1 ); } }

        public override bool HandlesOnMovement
        {
            get { return true; }
        }

        public Type[] Quests { get; set; }

        public string QuestsString
        {
            set
            {
                if( string.IsNullOrEmpty( value ) )
                    return;

                string[] tokens = value.Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries );
                if( tokens.Length < 1 )
                    return;

                List<Type> list = new List<Type>();
                foreach( var token in tokens )
                {
                    Type t = ScriptCompiler.FindTypeByName( token, true );
                    if( t != null )
                    {
                        Console.WriteLine( "Loading quest {0} into an xmlAttachment", t.Name );
                        list.Add( t );
                    }
                    else
                        Console.WriteLine( "Warning: type not found parsing quest attachment: " + token );
                }

                if( list.Count > 0 )
                    Quests = list.ToArray();
            }
        }

        public Region Region
        {
            get
            {
                if( AttachedTo is IEntity )
                    return Region.Find( ( (IEntity)AttachedTo ).Location, ( (IEntity)AttachedTo ).Map );
                else
                    return null;
            }
        }

        [Attachable]
        public XmStaticQuestAttachment( string questsString )
        {
            QuestsString = questsString;
        }

        public virtual void OnTalk( PlayerMobile player )
        {
            if( Owner is Mobile && QuestHelper.DeliveryArrived( player, (Mobile)Owner ) )
                return;

            if( QuestHelper.InProgress( player, Quests ) )
                return;

            if( QuestHelper.QuestLimitReached( player ) )
                return;

            foreach( KeyValuePair<QuestChain, BaseChain> pair in player.Chains )
            {
                BaseChain chain = pair.Value;

                if( chain != null && chain.Quester != null && chain.Quester == GetType() )
                {
                    BaseQuest quest = QuestHelper.RandomQuest( player, new Type[] { chain.CurrentQuest }, this );

                    if( quest != null )
                    {
                        player.CloseGump( typeof( MondainQuestGump ) );
                        player.SendGump( new MondainQuestGump( quest ) );
                        return;
                    }
                }
            }

            BaseQuest questt = QuestHelper.RandomQuest( player, Quests, this );

            if( questt != null )
            {
                player.CloseGump( typeof( MondainQuestGump ) );
                player.SendGump( new MondainQuestGump( questt ) );
            }
        }

        public virtual void OnOfferFailed()
        {
            Say( 1075575 ); // I'm sorry, but I don't have anything else for you right now. Could you check back with me in a few minutes?
        }

        public virtual void Advertise()
        {
            Say( Utility.RandomMinMax( 1074183, 1074223 ) );
        }

        public override void OnMovement( MovementEventArgs e )
        {
            if( e.Mobile != null && AttachedTo is Mobile && AttachedTo != null )
                OnMovement( e.Mobile, e.Mobile.Location );
        }

        public virtual void OnMovement( Mobile m, Point3D oldLocation )
        {
            Mobile owner = AttachedTo as Mobile;
            if( owner == null )
                return;

            if( m.Alive && !m.Hidden && m is PlayerMobile )
            {
                PlayerMobile pm = (PlayerMobile)m;

                int range = AutoTalkRange;

                if( range >= 0 && owner.InRange( m, range ) && !owner.InRange( oldLocation, range ) )
                    OnTalk( pm );

                range = AutoSpeakRange;

                if( range >= 0 && owner.InRange( m, range ) && !owner.InRange( oldLocation, range ) && DateTime.Now >= m_Spoken + SpeakDelay )
                {
                    if( Utility.Random( 100 ) < 50 )
                        Advertise();

                    m_Spoken = DateTime.Now;
                }
            }
        }

        public override void OnUse( Mobile m )
        {
            if( m.Alive && m is PlayerMobile )
                OnTalk( (PlayerMobile)m );
        }

        public virtual void FocusTo( Mobile to )
        {
            if( AttachedTo is Mobile )
                QuestSystem.FocusTo( ( (Mobile)Owner ), to );
        }

        public virtual void Say( int i )
        {
            if( AttachedTo is Mobile )
                ( (Mobile)AttachedTo ).PublicOverheadMessage( MessageType.Regular, 37, i );
        }

        public override string OnIdentify( Mobile from )
        {
            if( from == null || from.AccessLevel == AccessLevel.Player )
                return null;

            return "";
        }

        #region serialization
        public XmStaticQuestAttachment( ASerial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );

            if( ( Quests == null || Quests.Length == 0 ) )
                writer.Write( 0 );
            else
            {
                writer.Write( Quests.Length );
                foreach( Type type in Quests )
                {
                    writer.Write( type.FullName );
                }
            }
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            int quests = reader.ReadInt();
            if( quests > 0 )
            {
                List<Type> list = new List<Type>();
                for( int i = 0; i < quests; i++ )
                {
                    Type t = ScriptCompiler.FindTypeByFullName( reader.ReadString() );
                    if( t != null )
                        list.Add( t );
                }

                Quests = list.ToArray();
            }
        }
        #endregion
    }
}