using Server;
using Server.Mobiles;
using MidgardClasses = Midgard.Engines.Classes.Classes;

namespace Midgard.Engines.Classes
{
    public class SealOfRevenge : RitualItem
    {
        public override string DefaultName
        {
            get { return "a seal of revenge"; }
        }

        public override MidgardClasses Class
        {
            get { return MidgardClasses.Necromancer; }
        }

        [Constructable]
        public SealOfRevenge()
            : base( 8812 )
        {
            Hue = 1404;
        }

        public override bool CanDrop( PlayerMobile pm )
        {
            return false;
        }

        #region serialization
        public SealOfRevenge( Serial serial )
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