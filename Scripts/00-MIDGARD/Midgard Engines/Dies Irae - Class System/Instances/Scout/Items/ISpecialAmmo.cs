using Server;
using Server.Items;

namespace Midgard.Items
{
    public interface ISpecialAmmo
    {
        void OnHit( BaseRanged baseRanged, Mobile attacker, Mobile defender, double damageBonus );
        void OnMiss( BaseRanged baseRanged, Mobile attacker, Mobile defender );
        bool OnFired( BaseRanged baseRanged, Mobile attacker, Mobile defender );
    }
}