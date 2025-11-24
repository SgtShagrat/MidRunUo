using Server;
using Server.Targeting;

namespace Midgard.Engines.UOArchitect
{
    public class UoarObjectTarget : Target
    {
        public delegate void TargetCancelEvent();

        public delegate void TargetObjectEvent( object targeted );

        public TargetObjectEvent OnTargetObject;
        public TargetCancelEvent OnCancelled;

        public UoarObjectTarget()
            : base( -1, true, TargetFlags.None )
        {
        }

        protected override void OnTarget( Mobile from, object targeted )
        {
            if( OnTargetObject != null )
                OnTargetObject( targeted );
        }

        protected override void OnTargetCancel( Mobile from, TargetCancelType cancelType )
        {
            if( OnCancelled != null )
                OnCancelled();

            base.OnTargetCancel( from, cancelType );
        }
    }
}