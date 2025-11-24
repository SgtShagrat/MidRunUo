using Midgard.Engines.Classes;

using Server;
using Server.Items;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3e7 cruciform arms
    /// </summary>
    public class CruciformPlateArms : PlateArms
    {
        [Constructable]
        public CruciformPlateArms()
            : this( 0 )
        {
        }

        [Constructable]
        public CruciformPlateArms( int hue )
        {
            ItemID = 0x3e7;
            Hue = hue;
        }

        public override int ComputeStatBonus( StatType type )
        {
            if( type == StatType.Int )
                return Quality == ArmorQuality.Exceptional ? +2 : +1;

            return base.ComputeStatBonus( type );
        }

        #region serialization

        public CruciformPlateArms( Serial serial )
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

        public override string DefaultName
        {
            get { return "cruciform arms"; }
        }

        public override bool CannotBeHuedOnCraft
        {
            get { return true; }
        }

        public override Classes RequiredClass
        {
            get { return Classes.Paladin; }
        }
    }
}