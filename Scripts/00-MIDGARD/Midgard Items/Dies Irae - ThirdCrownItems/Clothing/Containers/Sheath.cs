using Midgard.Engines.Races;

using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3826 Sheath trad. fodero - ( nel vendor provisoner, colorabile da hue )
    /// </summary>
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
    public class Sheath : BaseContainer, IDyable
    {
        public override string DefaultName { get { return "sheath"; } }

        [Constructable]
        public Sheath()
            : base( 0x3826 )
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
        public Sheath( Serial serial )
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