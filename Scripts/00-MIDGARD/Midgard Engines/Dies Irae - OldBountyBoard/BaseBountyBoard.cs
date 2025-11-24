using System;
using System.Collections.Generic;

using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public abstract class BaseBountyBoard : Item
    {
        private string m_BoardName;

        [CommandProperty( AccessLevel.GameMaster )]
        public string BoardName
        {
            get { return m_BoardName; }
            set { m_BoardName = value; }
        }

        public BaseBountyBoard( int itemID )
            : base( itemID )
        {
            m_BoardName = "bounty board";
            Movable = false;
        }

        // Threads will be removed six hours after the last post was made
        // private static TimeSpan ThreadDeletionTime = TimeSpan.FromHours( 6.0 );

        // A player may only create a thread once every two minutes
        private static readonly TimeSpan ThreadCreateTime = TimeSpan.FromMinutes( 2.0 );

        // A player may only reply once every thirty seconds
        private static readonly TimeSpan ThreadReplyTime = TimeSpan.FromSeconds( 30.0 );

        public static bool CheckTime( DateTime time, TimeSpan range )
        {
            return ( time + range ) < DateTime.Now;
        }

        public static string FormatTS( TimeSpan ts )
        {
            int totalSeconds = (int)ts.TotalSeconds;
            int seconds = totalSeconds % 60;
            int minutes = totalSeconds / 60;

            if( minutes != 0 && seconds != 0 )
                return String.Format( "{0} minute{1} and {2} second{3}", minutes, minutes == 1 ? "" : "s", seconds, seconds == 1 ? "" : "s" );
            else if( minutes != 0 )
                return String.Format( "{0} minute{1}", minutes, minutes == 1 ? "" : "s" );
            else
                return String.Format( "{0} second{1}", seconds, seconds == 1 ? "" : "s" );
        }

        public virtual void Cleanup()
        {
            List<Item> items = Items;

            for( int i = items.Count - 1; i >= 0; --i )
            {
                if( i >= items.Count )
                    continue;

                BountyBoardMessage msg = items[ i ] as BountyBoardMessage;
                if( msg == null )
                    continue;

                msg.Delete();
            }
        }

        private void RecurseDelete( BountyBoardMessage msg )
        {
            List<BountyBoardMessage> found = new List<BountyBoardMessage>();
            List<Item> items = Items;

            for( int i = items.Count - 1; i >= 0; --i )
            {
                if( i >= items.Count )
                    continue;

                BountyBoardMessage check = items[ i ] as BountyBoardMessage;
                if( check == null )
                    continue;

                if( check.Thread == msg )
                {
                    check.Delete();
                    found.Add( check );
                }
            }

            foreach( BountyBoardMessage t in found )
                RecurseDelete( t );
        }

        public virtual bool GetLastPostTime( Mobile poster, bool onlyCheckRoot, ref DateTime lastPostTime )
        {
            List<Item> items = Items;
            bool wasSet = false;

            foreach( Item t in items )
            {
                BountyBoardMessage msg = t as BountyBoardMessage;
                if( msg == null || msg.Poster != poster )
                    continue;

                if( onlyCheckRoot && msg.Thread != null )
                    continue;

                if( msg.Time > lastPostTime )
                {
                    wasSet = true;
                    lastPostTime = msg.Time;
                }
            }

            return wasSet;
        }

        public virtual void BountyApply()
        {
            List<BountySort> bountyMobile = new List<BountySort>();
            foreach( Mobile m in World.Mobiles.Values )
            {
                //if( m is PlayerMobile && m.Bounty > 0 )
                //{
                //    bountyMobile.Add( new BountySort( m ) );
                //}

                /*
                for( int i = 0; i < bountyMobile.Count; i++ )
                {
                    if( i > 9 )
                        break;

                    BountySort p = bountyMobile[ i ] as BountySort;
                }
                */
            }

            bountyMobile.Sort();

            foreach( BountySort g in bountyMobile )
            {
                bool female = g.Fugitive.Female;

                string[] meggage = new string[]
                                   {
                                       "The foul scum known as ", 
                                       string.Concat(g.Fugitive.Name + (female ? " shed" : " hath") + " innocent blood! "), 
                                       string.Concat("For " + (female ? "she" : "he") + " is responsible for "), 
                                       string.Concat(g.Fugitive.Kills + " murders." + " Claim the reward! "), 
                                       // string.Concat("`Tis of " + g.Fugitive.Bounty + " gold pieces for "), 
                                       string.Concat((female ? "her" : "his") + " head! ")
                                   };

                AddItem( new BountyBoardMessage( g.Fugitive, null, g.Fugitive.Name, meggage ) );
            }
        }

        public class BountySort : IComparable
        {
            public Mobile Fugitive;
            public int Prices;

            public BountySort( Mobile m )
            {
                Fugitive = m;
                // Prices = m.Bounty;
            }

            public int CompareTo( object obj )
            {
                BountySort p = (BountySort)obj;
                return p.Prices - Prices;
            }
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( CheckRange( from ) )
            {
                Cleanup();
                BountyApply();
                from.Send( new BountyBoardDisplayBoard( this ) );
            }
            else
            {
                from.LocalOverheadMessage( MessageType.Regular, 0x3B2, true, "I can't reach that." );
            }
        }

        public virtual bool CheckRange( Mobile from )
        {
            if( from.AccessLevel >= AccessLevel.GameMaster )
                return true;

            return ( from.Map == Map && from.InRange( GetWorldLocation(), 2 ) );
        }

        public void PostMessage( Mobile from, BountyBoardMessage thread, string subject, string[] lines )
        {
            if( thread != null )
                thread.LastPostTime = DateTime.Now;

            AddItem( new BountyBoardMessage( from, thread, subject, lines ) );
        }

        #region serialization
        public BaseBountyBoard( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( (string)m_BoardName );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        m_BoardName = reader.ReadString();
                        break;
                    }
            }
        }
        #endregion

        public static void Initialize()
        {
            PacketHandlers.Register( 0x71, 0, true, new OnPacketReceive( BBClientRequest ) );
        }

        public static void BBClientRequest( NetState state, PacketReader pvSrc )
        {
            Mobile from = state.Mobile;

            int packetID = pvSrc.ReadByte();

            BaseBountyBoard board = World.FindItem( pvSrc.ReadInt32() ) as BaseBountyBoard;
            if( board == null || !board.CheckRange( from ) )
                return;

            switch( packetID )
            {
                case 3: BBRequestContent( from, board, pvSrc ); break;
                case 4: BBRequestHeader( from, board, pvSrc ); break;
                case 5: BBPostMessage( from, board, pvSrc ); break;
                case 6: BBRemoveMessage( from, board, pvSrc ); break;
            }
        }

        public static void BBRequestContent( Mobile from, BaseBountyBoard board, PacketReader pvSrc )
        {
            BountyBoardMessage msg = World.FindItem( pvSrc.ReadInt32() ) as BountyBoardMessage;
            if( msg == null || msg.Parent != board )
                return;

            from.Send( new BountyBoardMessageContent( board, msg ) );
        }

        public static void BBRequestHeader( Mobile from, BaseBountyBoard board, PacketReader pvSrc )
        {
            BountyBoardMessage msg = World.FindItem( pvSrc.ReadInt32() ) as BountyBoardMessage;
            if( msg == null || msg.Parent != board )
                return;

            from.Send( new BountyBoardMessageHeader( board, msg ) );
        }

        public static void BBPostMessage( Mobile from, BaseBountyBoard board, PacketReader pvSrc )
        {
            BountyBoardMessage thread = World.FindItem( pvSrc.ReadInt32() ) as BountyBoardMessage;
            if( thread != null && thread.Parent != board )
                thread = null;

            int breakout = 0;

            while( thread != null && thread.Thread != null && breakout++ < 10 )
                thread = thread.Thread;

            DateTime lastPostTime = DateTime.MinValue;

            if( board.GetLastPostTime( from, ( thread == null ), ref lastPostTime ) )
            {
                if( !CheckTime( lastPostTime, ( thread == null ? ThreadCreateTime : ThreadReplyTime ) ) )
                {
                    if( thread == null )
                        from.SendMessage( "You must wait {0} before creating a new thread.", FormatTS( ThreadCreateTime ) );
                    else
                        from.SendMessage( "You must wait {0} before replying to another thread.", FormatTS( ThreadReplyTime ) );

                    return;
                }
            }

            string subject = pvSrc.ReadUTF8StringSafe( pvSrc.ReadByte() );

            if( subject.Length == 0 )
                return;

            string[] lines = new string[ pvSrc.ReadByte() ];
            if( lines.Length == 0 )
                return;

            for( int i = 0; i < lines.Length; ++i )
                lines[ i ] = pvSrc.ReadUTF8StringSafe( pvSrc.ReadByte() );

            board.PostMessage( from, thread, subject, lines );
        }

        public static void BBRemoveMessage( Mobile from, BaseBountyBoard board, PacketReader pvSrc )
        {
            BountyBoardMessage msg = World.FindItem( pvSrc.ReadInt32() ) as BountyBoardMessage;
            if( msg == null || msg.Parent != board )
                return;

            if( from.AccessLevel < AccessLevel.GameMaster && msg.Poster != from )
                return;

            msg.Delete();
        }
    }
}