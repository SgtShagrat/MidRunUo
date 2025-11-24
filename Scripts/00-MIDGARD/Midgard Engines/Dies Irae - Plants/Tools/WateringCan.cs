using Server;
using Server.Items;

namespace Midgard.Engines.PlantSystem
{
    public class WateringCan : BaseBeverage
    {
        public override int EmptyLabelNumber
        {
            get { return 1065771; } // empty watering can
        }

        public override int BaseLabelNumber
        {
            get { return 1065770; } // watering can
        }

        public override int MaxQuantity
        {
            get { return 10; }
        }

        public override int LabelNumber
        {
            get
            {
                if( IsEmpty )
                    return EmptyLabelNumber;

                return BaseLabelNumber;
            }
        }

        public override int ComputeItemID()
        {
            return 0x2004;
        }

        public override void Pour_OnTarget( Mobile from, object targ )
        {
            if( IsEmpty || !Pourable || !ValidateUse( from, false ) )
                return;

            if( targ is BasePlant )
            {
                BasePlant plant = (BasePlant)targ;

                if( BasePlant.CheckAccess( from, plant ) )
                {
                    plant.Waterize();
                    from.SendMessage( "You carefully water that plant." );
                    Quantity--;
                }
            }
        }

        [Constructable]
        public WateringCan()
            : base( BeverageType.Water )
        {
            Weight = 2.0;
        }

        public WateringCan( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }

        #endregion
    }
}