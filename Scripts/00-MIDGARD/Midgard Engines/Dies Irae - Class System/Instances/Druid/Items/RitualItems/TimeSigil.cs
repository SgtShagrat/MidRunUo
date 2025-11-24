using Server;
using Server.Mobiles;

using MidgardClasses = Midgard.Engines.Classes.Classes;

namespace Midgard.Engines.Classes
{
    public class TimeSigil : RitualItem
    {
        public override string DefaultName
        {
            get { return "a sigil of time"; }
        }

        public override MidgardClasses Class
        {
            get { return MidgardClasses.Druid; }
        }

        [Constructable]
        public TimeSigil()
            : base( 8811 )
        {
        }

        public override bool CanDrop( PlayerMobile pm )
        {
            return false;
        }

        #region serialization
        public TimeSigil( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}