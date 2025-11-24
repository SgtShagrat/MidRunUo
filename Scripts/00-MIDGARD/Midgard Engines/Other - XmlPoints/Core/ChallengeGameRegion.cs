using System.Xml;
using Server.Items;
using Server.Mobiles;
using Server.Regions;

namespace Server.Engines.XmlPoints
{
    public class ChallengeGameRegion : GuardedRegion
    {
        public ChallengeGameRegion( string name, Map map, int priority, params Rectangle3D[] area )
            : base( name, map, priority, area )
        {
        }

        public ChallengeGameRegion( XmlElement xml, Map map, Region parent )
            : base( xml, map, parent )
        {
        }

        public BaseChallengeGame ChallengeGame { get; set; }

        public override bool AllowHarmful( Mobile from, Mobile target )
        {
            if( from == null || target == null )
                return false;

            // during a challenge games or 1-on-1 duels, restrict harmful acts to opponents
            return XmlPointsAttach.AreChallengers( from, target );
        }

        public override bool AllowBeneficial( Mobile from, Mobile target )
        {
            if( from == null || target == null )
                return false;

            // during a challenge game, beneficial acts on participants is restricted to between team members
            if( XmlPointsAttach.AreInAnyGame( target ) )
                return XmlPointsAttach.AreTeamMembers( from, target );

            // restrict everyone else
            return false;
        }

        public override bool OnDoubleClick( Mobile m, object o )
        {
            if( o is Corpse )
            {
                // dont allow other players to loot corpses while a challenge game is present
                if( ( ChallengeGame != null ) && !ChallengeGame.Deleted && ( m != null ) &&
                    !( ( (Corpse)o ).Owner is BaseCreature ) && ( ( (Corpse)o ).Owner != m ) &&
                    ( m.AccessLevel == AccessLevel.Player ) )
                {
                    XmlPointsAttach.SendText( m, 100105 ); // "You are not allowed to open that here."
                    return false;
                }
            }

            return base.OnDoubleClick( m, o );
        }

        public override void OnEnter( Mobile m )
        {
            if( m != null )
                XmlPointsAttach.SendText( m, 100106, Name ); // "You have entered the Challenge Game region '{0}'"

            base.OnEnter( m );
        }

        public override void OnExit( Mobile m )
        {
            if( m != null )
                XmlPointsAttach.SendText( m, 100107, Name ); // "You have left the Challenge Game region '{0}'"

            base.OnExit( m );
        }
    }
}