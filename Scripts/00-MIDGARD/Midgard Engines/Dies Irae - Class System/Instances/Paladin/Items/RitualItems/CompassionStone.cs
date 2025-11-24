using Server;
using Server.Mobiles;
using MidgardClasses = Midgard.Engines.Classes.Classes;

namespace Midgard.Engines.Classes
{
    public class CompassionStone : RitualItem
    {
        public override string DefaultName
        {
            get { return "a compassion stone"; }
        }

        public override MidgardClasses Class
        {
            get { return MidgardClasses.Paladin; }
        }

        [Constructable]
        public CompassionStone()
            : base( 0x186E )
        {
        }

        public override bool CanDrop( PlayerMobile pm )
        {
            return false;
        }

        #region serialization
        public CompassionStone( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}