using Midgard.Engines.Races;

using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x1152 Kilt Bag - ( nel vendor provisoner, colorabile da hue, container )
    /// </summary>
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
    public class KiltBag : BaseContainer, IDyable
    {
        public override string DefaultName { get { return "kilt bag"; } }

        [Constructable]
        public KiltBag()
            : base( 0x1152 )
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
        public KiltBag( Serial serial )
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