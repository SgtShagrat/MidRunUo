/***************************************************************************
 *                               TrapRemovalKit.cs
 *
 *   begin                : 09 agosto 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;

namespace Midgard.Items
{
    public class TrapRemovalKit : Item
    {
        [Constructable]
        public TrapRemovalKit()
            : base( 0x1ebb )
        {
            Hue = 0x7e9;
            Charges = 25;
        }

        public override string DefaultName
        {
            get { return "trap removal kit"; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Charges { get; set; }

        public void ConsumeCharge( Mobile from )
        {
            --Charges;

            if( Charges <= 0 )
            {
                if( from != null )
                    from.SendLocalizedMessage( 1042531 ); // You have used all of the parts in your trap removal kit.

                Delete();
            }
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            LabelTo( from, 1060584, Charges.ToString() );
        }

        #region serialization
        public TrapRemovalKit( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.WriteEncodedInt( Charges );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        Charges = reader.ReadEncodedInt();
                        break;
                    }
            }
        }
        #endregion
    }
}