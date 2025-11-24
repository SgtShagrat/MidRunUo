using Server;
using Server.Mobiles;

using MidgardClasses = Midgard.Engines.Classes.Classes;

namespace Midgard.Engines.Classes
{
    public class FireSigil : RitualItem
    {
        public override string DefaultName
        {
            get { return "a sigil of fire"; }
        }

        public override MidgardClasses Class
        {
            get { return MidgardClasses.Druid; }
        }

        [Constructable]
        public FireSigil()
            : base( 8803 )
        {
        }

        public override bool CanDrop( PlayerMobile pm )
        {
            return false;
        }

        #region serialization
        public FireSigil( Serial serial )
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