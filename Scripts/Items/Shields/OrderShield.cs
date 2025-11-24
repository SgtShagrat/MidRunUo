using System;
using Server;
using Server.Guilds;
using Server.Mobiles;

namespace Server.Items
{
	public class OrderShield : BaseShield
	{
        public override string DefaultName { get { return "order shield"; } } // mod by Dies Irae

		public override int BasePhysicalResistance{ get{ return 1; } }
		public override int BaseFireResistance{ get{ return 0; } }
		public override int BaseColdResistance{ get{ return 0; } }
		public override int BasePoisonResistance{ get{ return 0; } }
		public override int BaseEnergyResistance{ get{ return 0; } }

		public override int InitMinHits{ get{ return 100; } }
		public override int InitMaxHits{ get{ return 125; } }

		public override int AosStrReq{ get{ return 95; } }

		public override int ArmorBase{ get{ return 30; } }

        public override int BlockCircle { get { return 1; } } // mod by Dies Irae : pre-aos stuff

        public override int OldDexBonus { get { return -7; } } // mod by Dies Irae : pre-aos stuff

        public override int OldStrReq { get { return 30; } } // mod by Dies Irae : pre-aos stuff

		[Constructable]
		public OrderShield() : base( 0x1BC4 )
		{
            /* mod by Dies Irae
			if ( !Core.AOS )
				LootType = LootType.Newbied;
            */

			Weight = 7.0;
		}

		public OrderShield( Serial serial ) : base(serial)
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( Weight == 6.0 )
				Weight = 7.0;

            #region mod by Dies Irae
            if( LootType == LootType.Newbied )
                LootType = LootType.Regular;
            #endregion
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );//version
		}

        #region mod by Dies Irae
        public override bool CanBeCraftedBy( Mobile from )
        {
            return from != null && from is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)from ).IsOrder;
        }
        #endregion

		public override bool OnEquip( Mobile from )
		{
			return Validate( from ) && base.OnEquip( from );
		}

		public override void OnSingleClick( Mobile from )
		{
			if ( Validate( Parent as Mobile ) )
				base.OnSingleClick( from );
		}

		public virtual bool Validate( Mobile m )
		{
			if ( Core.AOS || m == null || !m.Player || m.AccessLevel != AccessLevel.Player )
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
	}
}