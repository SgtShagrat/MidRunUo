using Midgard.Engines.Classes;

using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x38C5 cruciform helm
    /// </summary>
    public class CruciformPlateHelm : PlateHelm
    {
        [Constructable]
        public CruciformPlateHelm()
            : this( 0 )
        {
        }

        [Constructable]
        public CruciformPlateHelm( int hue )
        {
            ItemID = 0x38C5;
            Hue = hue;
        }

        public override string DefaultName
        {
            get { return "cruciform helm"; }
        }

        public override int ComputeStatBonus( StatType type )
        {
            if( type == StatType.Int )
                return Quality == ArmorQuality.Exceptional ? +2 : +1;

            return base.ComputeStatBonus( type );
        }

        public override bool CannotBeHuedOnCraft
        {
            get { return true; }
        }

        public override Classes RequiredClass
        {
            get { return Classes.Paladin; }
        }

        public override bool CanBeCraftedBy( Mobile from )
        {
            return from.AccessLevel > AccessLevel.Counselor || from.Karma > 0;
        }

        #region serialization

        public CruciformPlateHelm( Serial serial )
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