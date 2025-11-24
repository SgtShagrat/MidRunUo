/***************************************************************************
 *                                  TreasureHuntingChest.cs
 *                            		-----------------------
 *  begin                	: October, 2007
 *  version					: 2.0 **VERSION FOR RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			This chest can be used to set up a treasure hunting quest.
 * 			There are three chest statuses: 
 * 				The First Chest of quest chain wont check for previous quest chains.
 * 				The intermidiate chest will check if previous chest has already been 
 * 					discovered by a guy.
 * 				The last chest will also check for previous chest discover.
 * 
 * 			Game master will see all hunters list on chest doubleclick.
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;

using Server;
using Server.Items;

namespace Midgard.Items
{
    [Flipable( 0xE41, 0xE40 )]
    public class TreasureHuntingChest : Container
    {
        public enum TreasureChestType
        {
            None,
            First,
            Last
        }

        private List<Mobile> m_Hunters;

        [CommandProperty( AccessLevel.GameMaster )]
        public TreasureHuntingChest PreviousLinkedChest { get; set; }

        public List<Mobile> Hunters
        {
            get
            {
                if( m_Hunters == null )
                    m_Hunters = new List<Mobile>();

                return m_Hunters;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int NumHunters
        {
            get { return Hunters.Count; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public TreasureChestType ChestType { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool HasToLog { get; set; }

        public override int DefaultMaxWeight
        {
            get { return 0; }
        }

        public override bool IsDecoContainer
        {
            get { return false; }
        }

        public override bool DisplaysContent
        {
            get { return false; }
        }

        public override bool DisplayWeight
        {
            get { return false; }
        }

        [Constructable]
        public TreasureHuntingChest( bool randomContainerID )
            : this()
        {
            if( randomContainerID )
                RandomizeId();
        }

        [Constructable]
        public TreasureHuntingChest()
            : base( 0xE41 )
        {
            m_Hunters = new List<Mobile>();
            ChestType = TreasureChestType.None;
            HasToLog = false;
        }

        public TreasureHuntingChest( Serial serial )
            : base( serial )
        {
        }

        private void RandomizeId()
        {
            bool useFirstItemId = Utility.RandomBool();

            switch( Utility.Random( 8 ) )
            {
                case 0: // Large Crate
                    ItemID = ( useFirstItemId ? 0xE3C : 0xE3D );
                    GumpID = 0x44;
                    break;
                case 1: // Medium Crate
                    ItemID = ( useFirstItemId ? 0xE3E : 0xE3F );
                    GumpID = 0x44;
                    break;
                case 2: // Small Crate
                    ItemID = ( useFirstItemId ? 0x9A9 : 0xE7E );
                    GumpID = 0x44;
                    break;
                case 3: // Wooden Chest
                    ItemID = ( useFirstItemId ? 0xE42 : 0xE43 );
                    GumpID = 0x49;
                    break;
                case 4: // Metal Chest
                    ItemID = ( useFirstItemId ? 0x9AB : 0xE7C );
                    GumpID = 0x4A;
                    break;
                case 5: // Metal Golden Chest
                    ItemID = ( useFirstItemId ? 0xE40 : 0xE41 );
                    GumpID = 0x42;
                    break;
                case 6: // Keg
                    ItemID = 0xE7F;
                    GumpID = 0x3E;
                    break;
                case 7: // Barrel
                    ItemID = 0xE77;
                    GumpID = 0x3E;
                    break;
                default:
                    ItemID = ( useFirstItemId ? 0x9AB : 0xE7C );
                    GumpID = 0x4A;
                    break;
            }
        }

        #region events OnSomething
        /// <summary>
        /// Overridable. Event invoked when a GameMaster doubleclick on our chest.
        /// </summary>
        /// <param name="from">Mobile who doubleclicked our chest</param>
        public virtual void OnGameMasterDoubleClick( Mobile from )
        {
            if( from != null && !from.Deleted )
            {
                if( from.AccessLevel > AccessLevel.Counselor )
                {
                    base.OnDoubleClick( from );

                    if( Hunters.Count < 1 )
                    {
                        from.SendMessage( "There are no guys in Hunters list." );
                    }
                    else
                    {
                        from.SendMessage( "Guys in hunters list:" );
                        for( int i = 0; i < Hunters.Count; i++ )
                        {
                            Mobile m = Hunters[ i ];
                            if( m != null && !m.Deleted )
                            {
                                if( m.Name != null )
                                {
                                    from.SendMessage( "{0} - {1} ({2})", i, m.Name, m.Account.Username );
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Overridable. Event invoked when a "First" chest is doubleclicked.
        /// </summary>
        /// <param name="from">Mobile who doubleclicked our chest</param>
        public virtual void OnFirstChestFound( Mobile from )
        {
            if( from != null && !from.Deleted )
            {
                if( HasToLog )
                {
                    string toLog = string.Format( "Player {0} (account {1}) has discovered first chain chest in date {2} and time {3}.",
                                                  from.Name, from.Account.Username, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString() );
                    StringToLog( toLog, "Logs/TreasureChestLog.log" );
                }
                from.SendMessage( "You have found the first chest of the treasure hunting quest. Good luck guy! Let's find the next one." );
                if( !Hunters.Contains( from ) )
                    Hunters.Add( from );
                base.OnDoubleClick( from );
            }
        }

        /// <summary>
        /// Overridable. Event invoked when an "Imtermidiate" chest is doubleclicked.
        /// </summary>
        /// <param name="from">Mobile who doubleclicked our chest</param>
        public virtual void OnMediumChestFound( Mobile from )
        {
            if( from != null && !from.Deleted )
            {
                if( !PreviousHasHunter( from ) )
                    from.SendMessage( "You have to find the previous chest to proceede in the hunting quest!" );
                else
                {
                    if( HasToLog )
                    {
                        string toLog = string.Format( "Player {0} (account {1}) has discovered intermidiate chain chest in date {2} and time {3}.",
                                                      from.Name, from.Account.Username, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString() );
                        StringToLog( toLog, "Logs/TreasureChestLog.log" );
                    }
                    from.SendMessage( "You can proceede in the hunting quest. Let's find the next chest." );
                    if( !Hunters.Contains( from ) )
                        Hunters.Add( from );
                    base.OnDoubleClick( from );
                }
            }
        }

        /// <summary>
        /// Overridable. Event invoked when a "Last" chest is doubleclicked.
        /// </summary>
        /// <param name="from">Mobile who doubleclicked our chest</param>
        public virtual void OnLastChestFound( Mobile from )
        {
            if( from != null && !from.Deleted )
            {
                if( !PreviousHasHunter( from ) )
                    from.SendMessage( "You have to find the previous chest to proceede in the hunting quest!" );
                else
                {
                    if( HasToLog )
                    {
                        string toLog = string.Format( "Player {0} (account {1}) has discovered last chain chest in date {2} and time {3}.",
                                                      from.Name, from.Account.Username, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString() );
                        StringToLog( toLog, "Logs/TreasureChestLog.log" );
                    }
                    from.SendMessage( "Congratulations. You have found the last chest." );
                    if( !Hunters.Contains( from ) )
                        Hunters.Add( from );
                    base.OnDoubleClick( from );
                }
            }
        }

        /// <summary>
        /// Overridable. Event invoked when a chest is doubleclicked but has already been discovered.
        /// </summary>
        /// <param name="from">Mobile who doubleclicked our chest</param>
        public virtual void OnDiscoveredChestDoubleClick( Mobile from )
        {
            if( from != null && !from.Deleted )
            {
                from.SendMessage( "You have already discovered this treasure hunting chest!" );
                base.OnDoubleClick( from );
            }
        }
        #endregion

        /// <summary>
        /// Overridable. Check if previous chest is set and if has from in Hunters list.
        /// </summary>
        /// <param name="from">Mobile to check</param>
        /// <returns>true previous chest was already opened by from. NB: First chest is always true.</returns>
        public virtual bool PreviousHasHunter( Mobile from )
        {
            if( ChestType == TreasureChestType.First ) // First chest has always a "virtual" previous chest found.
                return true;

            if( PreviousLinkedChest != null )
                return PreviousLinkedChest.Hunters.Contains( from );

            return false;
        }

        /// <summary>
        /// Overridable. Check if our chest hunters list contains from.
        /// </summary>
        /// <param name="from">Mobile to check</param>
        /// <returns>true if from is a valid chest hunter</returns>
        public virtual bool CheckHunted( Mobile from )
        {
            if( from != null && !from.Deleted )
                return Hunters.Contains( from );

            return false;
        }

        /// <summary>
        /// Static. Member invoked to log a string on a log file with try{} catch{}.
        /// <param name="toLog"  >string to log</param>
        /// <param name="fileNamePath" >runUO core relative path where log has to be created or appended.</param>
        /// </summary>
        private static void StringToLog( string toLog, string fileNamePath )
        {
            try
            {
                TextWriter tw = File.AppendText( fileNamePath );
                tw.WriteLine( toLog );
                tw.Close();
            }
            catch( Exception ex )
            {
                Console.Write( "Log failed: {0}", ex );
            }
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( from != null && !from.Deleted )
            {
                if( from.AccessLevel > AccessLevel.Counselor )
                    OnGameMasterDoubleClick( from );
                else
                {
                    if( CheckHunted( from ) )
                        OnDiscoveredChestDoubleClick( from );
                    else
                    {
                        switch( ChestType )
                        {
                            case TreasureChestType.First:
                                OnFirstChestFound( from );
                                break;
                            case TreasureChestType.None:
                                OnMediumChestFound( from );
                                break;
                            case TreasureChestType.Last:
                                OnLastChestFound( from );
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        public override bool OnDragDrop( Mobile from, Item dropped )
        {
            if( !base.OnDragDrop( from, dropped ) )
                return false;

            if( from.AccessLevel == AccessLevel.Player )
                return false;

            if( dropped != null && !dropped.Deleted )
                dropped.Movable = false;

            return base.OnDragDrop( from, dropped );
        }

        public override bool OnDragDropInto( Mobile from, Item item, Point3D p )
        {
            if( !base.OnDragDropInto( from, item, p ) )
                return false;

            if( from != null && from.AccessLevel == AccessLevel.Player )
                return false;

            if( item != null && !item.Deleted )
                item.Movable = false;

            return base.OnDragDropInto( from, item, p );
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 1 ); // version

            writer.Write( HasToLog );
            writer.Write( Hunters );
            writer.WriteItem( (Item)PreviousLinkedChest );
            writer.Write( (int)ChestType );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            if( m_Hunters == null )
                m_Hunters = new List<Mobile>();

            switch( version )
            {
                case 1:
                    {
                        HasToLog = reader.ReadBool();
                        goto case 0;
                    }
                case 0:
                    {
                        m_Hunters = reader.ReadStrongMobileList();

                        PreviousLinkedChest = (TreasureHuntingChest)reader.ReadItem();
                        ChestType = (TreasureChestType)reader.ReadInt();

                        break;
                    }
            }
        }
        #endregion
    }
}