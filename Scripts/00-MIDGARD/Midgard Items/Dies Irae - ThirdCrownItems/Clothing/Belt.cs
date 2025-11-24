using Server;
using Server.Items;
using Midgard.Engines.Races;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3364 Belt - ( nel vendor provisoner , colorabile da hue )
    /// </summary>
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
    public class Belt : BaseOuterLegs
    {
        public override string DefaultName { get { return "belt"; } }

        [Constructable]
        public Belt()
            : this( 0 )
        {
        }

        [Constructable]
        public Belt( int hue )
            : base( 0x3364, hue )
        {
            Weight = 1.0;
        }

        #region serialization
        public Belt( Serial serial )
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