using System;
using System.Xml;
using Server;

namespace Server.Regions
{
	public class TownRegion : GuardedRegion
	{
		public TownRegion( XmlElement xml, Map map, Region parent ) : base( xml, map, parent )
		{
        }

        #region modifica by Dies Irae
        public override void OnEnter( Mobile m )
        {
            if( Midgard.Engines.MidgardTownSystem.TownHelper.IsTownBanned( m ) )
                m.SendMessage( 37, m.Language == "ITA" ? "Attenzione, sei entrato in una cittadina in cui sei stato ESILIATO. Tutti gli altri giocatori ti vedranno come nemico (arancio)!" : "Be aware, you have entered a town region and you are EXILED from that. All other player will recognize you as an enemy (orange)!" );

            base.OnEnter( m );
        }
        #endregion
    }
}