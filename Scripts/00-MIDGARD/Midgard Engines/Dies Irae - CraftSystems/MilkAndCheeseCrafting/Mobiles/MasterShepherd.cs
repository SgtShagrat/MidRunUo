
using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Midgard.Engines.CheeseCrafting
{
	public class MasterShepherd : BaseVendor
	{
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

		[Constructable]
		public MasterShepherd() : base( "the Master Shepherd" )
		{
			SetSkill( SkillName.TasteID, 36.0, 68.0 );
			SetSkill( SkillName.Cooking, 36.0, 68.0 );
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBMasterShepherd() );
		}

		public override VendorShoeType ShoeType
		{
			get{ return VendorShoeType.Boots; }
		}

		public override int GetShoeHue()
		{
			return 0;
		}

        public MasterShepherd( Serial serial )
            : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}