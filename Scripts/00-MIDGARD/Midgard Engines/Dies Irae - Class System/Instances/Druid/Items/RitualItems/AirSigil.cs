using Server;
using Server.Mobiles;

using MidgardClasses = Midgard.Engines.Classes.Classes;

namespace Midgard.Engines.Classes
{
    public class AirSigil : RitualItem
    {
        public override string DefaultName
        {
            get { return "a sigil of air"; }
        }

        public override MidgardClasses Class
        {
            get { return MidgardClasses.Druid; }
        }

        [Constructable]
        public AirSigil()
            : base( 8825 )
        {
        }

        public override bool CanDrop( PlayerMobile pm )
        {
            return false;
        }

        #region serialization
        public AirSigil( Serial serial )
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