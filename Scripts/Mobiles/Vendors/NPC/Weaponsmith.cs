using System;
using System.Collections.Generic;
using Server;
using Server.Engines.BulkOrders;

using Midgard.Misc;
using Midgard.Engines.Craft;

namespace Server.Mobiles
{
	public class Weaponsmith : BaseVendor
	{
        public override SpeechFragment PersonalFragmentObj { get { return PersonalFragment.Weaponsmith; } } // mod by Dies Irae

		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

		[Constructable]
		public Weaponsmith() : base( "the weaponsmith" )
		{
			SetSkill( SkillName.ArmsLore, 64.0, 100.0 );
			SetSkill( SkillName.Blacksmith, 65.0, 88.0 );
			SetSkill( SkillName.Fencing, 45.0, 68.0 );
			SetSkill( SkillName.Macing, 45.0, 68.0 );
			SetSkill( SkillName.Swords, 45.0, 68.0 );
			SetSkill( SkillName.Tactics, 36.0, 68.0 );
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBWeaponSmith() );
			
			if ( IsTokunoVendor )
				m_SBInfos.Add( new SBSEWeapons() );
		}

		public override VendorShoeType ShoeType
		{
			get{ return Utility.RandomBool() ? VendorShoeType.Boots : VendorShoeType.ThighBoots; }
		}

		public override int GetShoeHue()
		{
			return 0;
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem( new Server.Items.HalfApron() );
		}

		#region Bulk Orders
		public override Item CreateBulkOrder( Mobile from, bool fromContextMenu )
		{
			PlayerMobile pm = from as PlayerMobile;

			if ( pm != null && pm.NextSmithBulkOrder == TimeSpan.Zero && (fromContextMenu || 0.2 > Utility.RandomDouble()) )
			{
				double theirSkill = pm.Skills[SkillName.Blacksmith].Base;

				if ( theirSkill >= 70.1 )
					pm.NextSmithBulkOrder = TimeSpan.FromHours( 6.0 );
				else if ( theirSkill >= 50.1 )
					pm.NextSmithBulkOrder = TimeSpan.FromHours( 2.0 );
				else
					pm.NextSmithBulkOrder = TimeSpan.FromHours( 1.0 );

                #region mod by Dies Irae
                double coeff = BodHelper.IsValidMidgardCandidate( from ) ? 51.0 : 40.0;
                if( from is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)from ).LastSmithBulkOrderValue < BodHelper.SmithPointsLimit )
                    coeff = 40;
                #endregion

                if( theirSkill >= 70.1 && ( ( theirSkill - coeff ) / 300.0 ) > Utility.RandomDouble() ) // mod by Dies Irae
                    return BodHelper.CreateRandomLargeSmithBodFor( from ); // LargeSmithBOD(); // mod by Dies Irae

				return SmallSmithBOD.CreateRandomFor( from );
			}

			return null;
		}

		public override bool IsValidBulkOrder( Item item )
		{
			return ( item is SmallSmithBOD || item is LargeSmithBOD );
		}

		public override bool SupportsBulkOrders( Mobile from )
		{
			return ( Midgard2Persistance.SmithBulksEnabled && from is PlayerMobile && Core.AOS && from.Skills[SkillName.Blacksmith].Base > 0 );
		}

		public override TimeSpan GetNextBulkOrder( Mobile from )
		{
			if ( from is PlayerMobile )
				return ((PlayerMobile)from).NextSmithBulkOrder;

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
				TimeSpan oldDelay = ((PlayerMobile)from).NextSmithBulkOrder;
				TimeSpan newDelay = oldDelay;

				if( dropped is LargeBOD )
					newDelay = Midgard.Engines.Craft.BodHelper.ScaleTime( oldDelay, (LargeBOD)dropped );
				else if( dropped is SmallBOD )
					newDelay = Midgard.Engines.Craft.BodHelper.ScaleTime( oldDelay, (SmallBOD)dropped );

				((PlayerMobile)from).NextSmithBulkOrder = newDelay;
                 */
                if( from is Midgard2PlayerMobile )
                    ( (Midgard2PlayerMobile)from ).LastSmithBulkOrderValue = BodHelper.GetBulkValue( dropped );

                ( (PlayerMobile)from ).NextSmithBulkOrder = TimeSpan.Zero;
            }
        }
		#endregion
		#endregion

		public Weaponsmith( Serial serial ) : base( serial )
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