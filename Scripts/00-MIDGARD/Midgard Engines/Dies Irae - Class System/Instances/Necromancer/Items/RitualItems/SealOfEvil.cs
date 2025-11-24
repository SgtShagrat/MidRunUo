using Server;
using Server.Mobiles;
using MidgardClasses = Midgard.Engines.Classes.Classes;

namespace Midgard.Engines.Classes
{
    public class SealOfEvil : RitualItem
    {
        public override string DefaultName
        {
            get { return "a seal of evil"; }
        }

        public override MidgardClasses Class
        {
            get { return MidgardClasses.Necromancer; }
        }

        [Constructable]
        public SealOfEvil()
            : base( 8824 )
        {
            Hue = 1404;
        }

        public override bool CanDrop( PlayerMobile pm )
        {
            return false;
        }

        #region serialization
        public SealOfEvil( Serial serial )
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