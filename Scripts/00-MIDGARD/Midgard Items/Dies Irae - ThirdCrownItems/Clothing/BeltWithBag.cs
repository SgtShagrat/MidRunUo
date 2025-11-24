using Midgard.Engines.Races;

using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3363 Belt with Bag - ( nel vendor provisoner , colorabile da hue, container )
    /// </summary>
    [RaceAllowance( typeof( MountainDwarf ) )]
    public class BeltWithBag : BaseContainer, IDyable
    {
        public override string DefaultName { get { return "belt with bag"; } }

        [Constructable]
        public BeltWithBag()
            : base( 0x3363 )
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
        public BeltWithBag( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}