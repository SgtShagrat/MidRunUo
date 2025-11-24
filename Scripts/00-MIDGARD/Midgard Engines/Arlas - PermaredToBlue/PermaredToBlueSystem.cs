using System;
using System.Collections.Generic;
using System.IO;
using Server;
using Server.Accounting;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.PermaredToBlue
{
	public class PermaredToBlueSystem
	{
		public virtual int MessageHue { get { return 37; } }

		#region Is... something
		public static bool IsGoodOne( Mobile m )
		{
			return ( m != null && m.Karma > 0 );
		}

		public static bool IsEvilOne( Mobile m )
		{
			return ( m != null && m.Karma < 0 );
		}

		public static bool IsPermared( Midgard2PlayerMobile m )//da modificare
		{
			return ( m != null && (m.PermaRed || m.LifeTimeKills > 4) );
		}

		public virtual bool IsEligible( Mobile mob )
		{
			return ( mob != null && mob.Player && mob is Midgard2PlayerMobile && IsPermared((Midgard2PlayerMobile)mob) );
		}

		public virtual string IsEligibleString( Mobile mob )
		{
			return ( mob.Language == "ITA" ? "Non puoi chedere una redenzione." : "Thou doesn't need redemption." );
		}
		#endregion

		protected PermaredToBlueSystem()
		{
			InitializePermaredToBlueSystem();
		}

		public static void RegisterEventSink()
		{
		}

		public void InitializePermaredToBlueSystem()
		{
			m_Candidates = new List<Mobile>();
		}

		public void SendOverheadMessage( Mobile sender, string message )
		{
			sender.PublicOverheadMessage( MessageType.Regular, MessageHue, true, message );
		}

		public virtual void Deserialize( GenericReader reader )
		{
			int version = reader.ReadEncodedInt();
			m_Candidates = reader.ReadStrongMobileList();
		}

		public virtual void Serialize( GenericWriter writer )
		{
			writer.WriteEncodedInt( 0 ); // version

			// Version 0
			writer.Write( m_Candidates, true );
		}

		private List<Mobile> m_Candidates;
		public List<Mobile> Candidates{get { return m_Candidates; }}

		public bool IsCandidate( Mobile from ){return m_Candidates != null && m_Candidates.Contains( from );}

		public void AddCandidate( Mobile m )
		{
			if( m_Candidates != null && m_Candidates.Contains( m ) )
				return;

			if( m_Candidates == null )
				m_Candidates = new List<Mobile>();

			m_Candidates.Add( m );
		}

		public void RemoveCandidate( Mobile m )
		{
			if( m_Candidates == null )
				return;

			if( m_Candidates.Contains( m ) )
				m_Candidates.Remove( m );
		}
	}
}