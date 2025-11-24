using Server;
using Server.Mobiles;

using MidgardClasses = Midgard.Engines.Classes.Classes;

namespace Midgard.Engines.Classes
{
    public class EarthSigil : RitualItem
    {
        public override string DefaultName
        {
            get { return "a sigil of earth"; }
        }

        public override MidgardClasses Class
        {
            get { return MidgardClasses.Druid; }
        }

        [Constructable]
        public EarthSigil()
            : base( 8823 )
        {
        }

        public override bool CanDrop( PlayerMobile pm )
        {
            return false;
        }

        #region serialization
        public EarthSigil( Serial serial )
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