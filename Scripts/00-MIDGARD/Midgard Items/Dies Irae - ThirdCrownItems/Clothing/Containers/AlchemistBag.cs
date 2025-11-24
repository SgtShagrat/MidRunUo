using Midgard.Engines.Races;

using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x113F Alchemist Bag - ( nel vendor provisoner, non colorabile da hue, container )
    /// </summary>
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
    public class AlchemistBag : BaseContainer, IDyable
    {
        public override string DefaultName { get { return "alchemist bag"; } }

        [Constructable]
        public AlchemistBag()
            : base( 0x113F )
        {
            Weight = 1.0;
            Layer = Layer.Waist;
        }

        public bool Dye( Mobile from, DyeTub sender )
        {
            from.SendLocalizedMessage( sender.FailMessage );
            return false;
        }

        #region serialization
        public AlchemistBag( Serial serial )
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