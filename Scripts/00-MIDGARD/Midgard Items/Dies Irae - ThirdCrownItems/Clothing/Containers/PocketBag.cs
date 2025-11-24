using Midgard.Engines.Races;

using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3824 Pocket Bag - ( nel vendor provisoner, colorabile da hue, container )
    /// </summary>
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
    public class PocketBag : BaseContainer, IDyable
    {
        public override string DefaultName { get { return "pocket bag"; } }

        [Constructable]
        public PocketBag()
            : base( 0x3824 )
        {
            Weight = 1.0;
            Layer = Layer.Waist;
        }

        public bool Dye( Mobile from, DyeTub sender )
        {
            if( Deleted )
                return false;

            Hue = sender.DyedHue;

            return true;
        }

        #region serialization
        public PocketBag( Serial serial )
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