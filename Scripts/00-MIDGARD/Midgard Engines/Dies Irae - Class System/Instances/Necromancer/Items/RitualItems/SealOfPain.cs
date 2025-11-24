using Server;
using Server.Mobiles;
using MidgardClasses = Midgard.Engines.Classes.Classes;

namespace Midgard.Engines.Classes
{
    public class SealOfPain : RitualItem
    {
        public override string DefaultName
        {
            get { return "a seal of pain"; }
        }

        public override MidgardClasses Class
        {
            get { return MidgardClasses.Necromancer; }
        }

        [Constructable]
        public SealOfPain()
            : base( 8820 )
        {
            Hue = 1404;
        }

        public override bool CanDrop( PlayerMobile pm )
        {
            return false;
        }

        #region serialization
        public SealOfPain( Serial serial )
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