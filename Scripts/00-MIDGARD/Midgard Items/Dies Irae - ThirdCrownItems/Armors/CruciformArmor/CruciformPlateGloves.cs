using Midgard.Engines.Classes;

using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3e6 cruciform gloves
    /// </summary>
    public class CruciformPlateGloves : PlateGloves
    {
        [Constructable]
        public CruciformPlateGloves()
            : this( 0 )
        {
        }

        [Constructable]
        public CruciformPlateGloves( int hue )
        {
            ItemID = 0x3e6;
            Hue = hue;
        }

        public override string DefaultName
        {
            get { return "cruciform gloves"; }
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

        public CruciformPlateGloves( Serial serial )
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