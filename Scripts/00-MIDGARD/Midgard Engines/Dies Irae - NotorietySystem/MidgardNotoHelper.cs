using System.Collections.Generic;

using Server;
using Server.Mobiles;

namespace Midgard.Engines.NotoSystem
{
    public static class MidgardNotoHelper
    {
        public enum TeamViewState
        {
            Unchanged,
            Allied,
            Enemy,
            Invulnerable
        }

        public static bool HandleMobileNotoriety( Mobile source, Mobile target, out int noto )
        {
            int sourceTeam = source is Midgard2PlayerMobile ? ( (Midgard2PlayerMobile)source ).NotoTeam : 0;
            int targetTeam = target is Midgard2PlayerMobile ? ( (Midgard2PlayerMobile)target ).NotoTeam : 0;

            if( sourceTeam > 0 && targetTeam == sourceTeam )
            {
                noto = Notoriety.Ally;
                return true;
            }

            noto = 1;
            return false;
        }

        internal class NotoTeam
        {
            public int Hue { get; set; }
            public string Name { get; set; }

            public List<NotoTeam> Enemies { get; set; }
            public List<NotoTeam> Allies { get; set; }

            public bool IsEnemyTo( NotoTeam other )
            {
                if( other == this )
                    return false;

                return other != null && Enemies != null && Enemies.Contains( other );
            }

            public bool IsAlliedTo( NotoTeam other )
            {
                if( other == this )
                    return true;

                return other != null && Allies != null && Allies.Contains( other );
            }

            public bool IsNeutralTo( NotoTeam other )
            {
                return other != null && !IsEnemyTo( other ) && !IsAlliedTo( other );
            }

            public void RegisterAlly( NotoTeam alliedSystem )
            {
                if( Allies == null )
                    Allies = new List<NotoTeam>();

                if( alliedSystem != null && !Allies.Contains( alliedSystem ) )
                    Allies.Add( alliedSystem );
            }

            public void RegisterEnemy( NotoTeam enemySystem )
            {
                if( Enemies == null )
                    Enemies = new List<NotoTeam>();

                if( enemySystem != null && !Enemies.Contains( enemySystem ) )
                    Enemies.Add( enemySystem );
            }

            public void RemoveAlly( NotoTeam toRemove )
            {
                if( Allies == null )
                    return;

                if( toRemove != null && Allies.Contains( toRemove ) )
                    Allies.Remove( toRemove );
            }

            public void RemoveEnemy( NotoTeam toRemove )
            {
                if( Enemies == null )
                    return;

                if( toRemove != null && Enemies.Contains( toRemove ) )
                    Enemies.Remove( toRemove );
            }
        }
    }
}