using Server;
using Server.Items;
using Server.Targeting;

namespace Midgard
{
    public interface IGem
    {
        GemType GemType { get; }
    }
}

namespace Midgard.Engines.OldCraftSystem
{
    public class GemTarget : Target
    {
        private BaseJewel m_Jewel;

        public GemTarget( BaseJewel jewel )
            : base( -1, false, TargetFlags.None )
        {
            m_Jewel = jewel;
        }

        protected override void OnTarget( Mobile from, object targeted )
        {
            int message = 0;

            Item gem = targeted as Item;

            if( gem == null || !( gem is IGem ) )
                message = 502961; // That's not a gem or jewel of the proper type.
            else if( !gem.IsAccessibleTo( from ) )
                message = 502937; // That is inaccessible.
            else if( !from.CheckSkill( SkillName.Tinkering, 40.0, 90.0 ) )
                message = 502960; // You fail to make the jewelry properly.

            if( message > 0 )
                from.SendLocalizedMessage( message );
            else
                ApplyGem( from, gem );
        }

        private void ApplyGem( Mobile from, Item gem )
        {
            if( gem == null || gem.Deleted )
                return;

            if( m_Jewel != null )
            {
                m_Jewel.GemType = ( (IGem) gem ).GemType;
                from.SendMessage( "You applied the gem successfully." );
            }
        }
    }
}