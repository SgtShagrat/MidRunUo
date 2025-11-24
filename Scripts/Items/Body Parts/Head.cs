using System;
using Server;

using Midgard.Engines.BountySystem; // mod by Dies Irae

namespace Server.Items
{
	public enum HeadType
	{
		Regular,
		Duel,
		Tournament,

        Murderer // mod by Dies Irae
	}

	public class Head : Item
    {
        #region mod by Dies Irae : bounty system
        private DateTime m_CreationTime;
		private Mobile m_Owner;
		private Mobile m_Killer;
		private bool m_Player;

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime CreationTime
		{
			get{ return m_CreationTime; }
			set{ m_CreationTime = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner
		{
			get{ return m_Owner; }
			set{ m_Owner = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Killer
		{
			get{ return m_Killer; }
			set{ m_Killer = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsPlayer
		{
			get{ return m_Player; }
			set{ m_Player = value; }
        }
        #endregion

        private string m_PlayerName;
		private HeadType m_HeadType;

		[CommandProperty( AccessLevel.GameMaster )]
		public string PlayerName
		{
			get { return m_PlayerName; }
			set { m_PlayerName = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public HeadType HeadType
		{
			get { return m_HeadType; }
			set { m_HeadType = value; }
		}

		public override string DefaultName
		{
			get
			{
				if ( m_PlayerName == null )
					return base.DefaultName;

				switch ( m_HeadType )
				{
					default:
						return String.Format( "the head of {0}", m_PlayerName );

                    #region mod by Dies Irae
                    case HeadType.Murderer:
                        return String.Format( "the head of {0}, the murderer", m_PlayerName );
                    #endregion

                    case HeadType.Duel:
						return String.Format( "the head of {0}, taken in a duel", m_PlayerName );

					case HeadType.Tournament:
						return String.Format( "the head of {0}, taken in a tournament", m_PlayerName );
				}
			}
		}

		[Constructable]
		public Head()
			: this( null )
		{
		}

		[Constructable]
		public Head( string playerName )
			: this( HeadType.Regular, playerName )
		{
		}

		[Constructable]
		public Head( HeadType headType, string playerName )
			: base( 0x1DA0 )
		{
			m_HeadType = headType;
			m_PlayerName = playerName;

            #region mod by Dies Irae : bounty system
			m_Player = false;
			m_Owner = null;
			m_Killer = null;
			m_CreationTime = DateTime.Now;
            #endregion
        }

		public Head( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version

            #region mod by Dies Irae : bounty system
			writer.Write( m_Player );
			writer.Write( m_CreationTime );

			if( m_Player )
			{
				writer.Write(  m_Owner );
				writer.Write(  m_Killer );
			}
            #endregion

			writer.Write( (string) m_PlayerName );
			writer.WriteEncodedInt( (int) m_HeadType );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
                #region mod by Dies Irae : bounty system
                case 2:
					m_Player = reader.ReadBool();
					m_CreationTime = reader.ReadDateTime();
					if( m_Player )
					{
						m_Owner = reader.ReadMobile();
						m_Killer = reader.ReadMobile();
					}
                    goto case 1;
                #endregion

                case 1:
					m_PlayerName = reader.ReadString();
					m_HeadType = (HeadType) reader.ReadEncodedInt();
					break;

				case 0:
					string format = this.Name;

					if ( format != null )
					{
						if ( format.StartsWith( "the head of " ) )
							format = format.Substring( "the head of ".Length );

						if ( format.EndsWith( ", taken in a duel" ) )
						{
							format = format.Substring( 0, format.Length - ", taken in a duel".Length );
							m_HeadType = HeadType.Duel;
						}
						else if ( format.EndsWith( ", taken in a tournament" ) )
						{
							format = format.Substring( 0, format.Length - ", taken in a tournament".Length );
							m_HeadType = HeadType.Tournament;
						}
					}

                    m_PlayerName = format;
                    this.Name = null;

                    #region mod by Dies Irae : bounty system
                    m_Owner = null;
                    m_Killer = null;
                    m_Player = false;
                    m_CreationTime = DateTime.Now - Config.DefaultDecayRate;
                    #endregion

                    break;
			}
		}
	}
}