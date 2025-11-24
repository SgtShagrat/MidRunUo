using Midgard.Engines.Races;

using Server;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3366 Monocle - ( craftabile esclusivamente dal pg tinker , colorabile da hue )
    /// </summary>
    [RaceAllowanceAttribute( typeof( MountainDwarf ) )]
    public class Monocle : BaseGlasses
    {
        public override string DefaultName { get { return "monocole"; } }

        [Constructable]
        public Monocle()
            : base( 0x3366 )
        {
        }

        #region serial-deserial
        public Monocle( Serial serial )
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