using Server;
using Server.Guilds;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Items
{
    /// <summary>
    /// 0x2688 Chaos Platemail ( craft del fabbro solo da fazionati chaos )
    /// </summary>
    public class ChaosPlatemail : PlateChest
    {
        public override string DefaultName { get { return "chaos platemail"; } }

        [Constructable]
        public ChaosPlatemail()
            : this( 0 )
        {
        }

        [Constructable]
        public ChaosPlatemail( int hue )
        {
            ItemID = 0x2688;
            Hue = hue;
        }

        public override bool CanBeCraftedBy( Mobile from )
        {
            return from is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)from ).IsChaos;
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

            bool isChaos = m is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)m ).IsChaos;

            if( !isChaos )
            {
                m.FixedEffect( 0x3728, 10, 13 );
                Delete();

                return false;
            }

            return true;
        }

        #region serialization
        public ChaosPlatemail( Serial serial )
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