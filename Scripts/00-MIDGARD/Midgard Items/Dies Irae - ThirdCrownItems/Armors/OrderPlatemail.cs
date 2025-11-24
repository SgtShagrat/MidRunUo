using Midgard.Engines.MidgardTownSystem;

using Server;
using Server.Guilds;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Items
{
    /// <summary>
    /// 0x2687 Order Platemail ( craft del fabbro, se possibile solo da fazionati Order ) N.B. sostituisce e cancella "tiranni stendardo" da script
    /// </summary>
    public class OrderPlatemail : PlateChest
    {
        public override string DefaultName { get { return "order platemail"; } }

        [Constructable]
        public OrderPlatemail()
            : this( 0 )
        {
        }

        [Constructable]
        public OrderPlatemail( int hue )
        {
            ItemID = 0x2687;
            Hue = hue;
        }

        public override bool CanBeCraftedBy( Mobile from )
        {
            return from is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)from ).IsOrder;
        }

        public override bool OnEquip( Mobile from )
        {
            return Validate( from ) && base.OnEquip( from );
        }

        public override void OnSingleClick( Mobile from )
        {
            if( Validate( Parent as Mobile ) )
                base.OnSingleClick( from );
        }

        public virtual bool Validate( Mobile m )
        {
            if( m == null || !m.Player || m.AccessLevel != AccessLevel.Player )
                return true;

            bool isOrder = m is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)m ).IsOrder;

            if( !isOrder )
            {
                m.FixedEffect( 0x3728, 10, 13 );
                Delete();

                return false;
            }

            return true;
        }

        #region serialization
        public OrderPlatemail( Serial serial )
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