using Server;
using Server.Mobiles;
using Server.Spells;
using Server.Targeting;

namespace Midgard.Engines.MidgardTownSystem
{
    public class TownCharacterStatueTarget : Target
    {
        private readonly StatueType m_Type;

        public TownCharacterStatueTarget( StatueType type )
            : base( -1, true, TargetFlags.None )
        {
            m_Type = type;
        }

        protected override void OnTarget( Mobile from, object targeted )
        {
            IPoint3D p = targeted as IPoint3D;
            Map map = from.Map;

            if( p == null || map == null )
                return;

            SpellHelper.GetSurfaceTop( ref p );
            Point3D loc = new Point3D( p );

            TownCharacterStatue statue = new TownCharacterStatue( from, m_Type );
            TownCharacterStatuePlinth plinth = new TownCharacterStatuePlinth( statue );

            statue.Plinth = plinth;
            plinth.MoveToWorld( loc, map );
            statue.InvalidatePose();
        }
    }
}