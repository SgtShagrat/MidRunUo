using System;
using Midgard.Engines.SpellSystem;
using Server;

namespace Midgard.Engines.Classes
{
    /*
    [PropertyObject]
    public class PaladinPlayerState : ClassPlayerState
    {
        public PaladinPlayerState( ClassSystem system, Mobile mobile )
            : base( system, mobile )
        {
        }

        [CommandProperty( AccessLevel.Seer )]
        public bool HasPrayed
        {
            get { return ( DateTime.Now - LastPrayed ) < PaladinSystem.GetPrayDuration( Mobile ); }
        }

        [CommandProperty( AccessLevel.Seer )]
        public DateTime LastPrayed { get; set; }

        [CommandProperty( AccessLevel.Seer )]
        public bool IsWaitingCriticalShot { get; set; }

        [CommandProperty( AccessLevel.Seer )]
        public HolyMount HolyMount
        {
            get { return HolyMountSpell.GetMount( Mobile ); }
            set { HolyMountSpell.SetMount( Mobile, value ); }
        }

        #region serialization
        public PaladinPlayerState( ClassSystem system )
            : base( system )
        {
        }

        public override void Deserialize( ClassSystem system, GenericReader reader )
        {
            base.Deserialize( system, reader );

            int version = reader.ReadInt();
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }
        #endregion
    }
    */
}