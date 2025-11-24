using System;
using System.Collections.Generic;
using Server;
using Server.Engines.BulkOrders;

using Midgard.Misc;
using Midgard.Engines.Craft;

namespace Server.Mobiles
{
	public class Weaver : BaseVendor
	{
        public override SpeechFragment PersonalFragmentObj { get { return PersonalFragment.Weaponsmith; } } // mod by Dies Irae

		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

		public override NpcGuild NpcGuild{ get{ return NpcGuild.TailorsGuild; } }

		[Constructable]
		public Weaver() : base( "the weaver" )
		{
			SetSkill( SkillName.Tailoring, 65.0, 88.0 );
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBWeaver() );
		}

		public override VendorShoeType ShoeType
		{
			get{ return VendorShoeType.Sandals; }
		}

		#region Bulk Orders
		public override Item CreateBulkOrder( Mobile from, bool fromContextMenu )
		{
			PlayerMobile pm = from as PlayerMobile;

			if ( pm != null && pm.NextTailorBulkOrder == TimeSpan.Zero && (fromContextMenu || 0.2 > Utility.RandomDouble()) )
			{
				double theirSkill = pm.Skills[SkillName.Tailoring].Base;

				if ( theirSkill >= 70.1 )
					pm.NextTailorBulkOrder = TimeSpan.FromHours( 6.0 );
				else if ( theirSkill >= 50.1 )
					pm.NextTailorBulkOrder = TimeSpan.FromHours( 2.0 );
				else
					pm.NextTailorBulkOrder = TimeSpan.FromHours( 1.0 );

				if ( theirSkill >= 70.1 && ((theirSkill - 40.0) / 300.0) > Utility.RandomDouble() )
					return Midgard.Engines.Craft.BodHelper.CreateRandomLargeTailorBodFor( from ); // LargeTailorBOD(); // mod by Dies Irae

				return SmallTailorBOD.CreateRandomFor( from );
			}

			return null;
		}

		public override bool IsValidBulkOrder( Item item )
		{
			return ( item is SmallTailorBOD || item is LargeTailorBOD );
		}

		public override bool SupportsBulkOrders( Mobile from )
		{
			return ( Midgard2Persistance.TailorBulksEnabled && from is PlayerMobile && from.Skills[SkillName.Tailoring].Base > 0 );
		}

		public override TimeSpan GetNextBulkOrder( Mobile from )
		{
			if ( from is PlayerMobile )
				return ((PlayerMobile)from).NextTailorBulkOrder;

			return TimeSpan.Zero;
		}

		#region  mod by Dies Irae
		/*
		public override void OnSuccessfulBulkOrderReceive( Mobile from )
		{
			
		}
		*/

        public override void OnSuccessfulBulkOrderReceive( Mobile from, Item dropped )
        {
            if( Core.SE && from is PlayerMobile )
            {
                /*
				TimeSpan oldDelay = ((PlayerMobile)from).NextTailorBulkOrder;
				TimeSpan newDelay = oldDelay;

				if( dropped is LargeBOD )
					newDelay = Midgard.Engines.Craft.BodHelper.ScaleTime( oldDelay, (LargeBOD)dropped );
				else if( dropped is SmallBOD )
					newDelay = Midgard.Engines.Craft.BodHelper.ScaleTime( oldDelay, (SmallBOD)dropped );

				((PlayerMobile)from).NextTailorBulkOrder = newDelay;
                 */
                if( from is Midgard2PlayerMobile )
                    ( (Midgard2PlayerMobile)from ).LastTailorBulkOrderValue = BodHelper.GetBulkValue( dropped );

                ( (PlayerMobile)from ).NextTailorBulkOrder = TimeSpan.Zero;
            }
        }
		#endregion
		#endregion

		public Weaver( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}