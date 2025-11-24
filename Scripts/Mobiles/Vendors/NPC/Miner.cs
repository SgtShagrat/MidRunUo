using System;
using System.Collections.Generic;
using Server;

namespace Server.Mobiles
{
	public class Miner : BaseVendor
	{
        public override SpeechFragment PersonalFragmentObj { get { return PersonalFragment.Miner; } } // mod by Dies Irae

		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

		[Constructable]
		public Miner() : base( "the miner" )
		{
			SetSkill( SkillName.Mining, 65.0, 88.0 );
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBMiner() );
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

            if( FindItemOnLayer( Layer.Shirt ) == null ) // mod by Dies Irae
                AddItem( new Server.Items.FancyShirt( 0x3E4 ) );
            if( FindItemOnLayer( Layer.Pants ) == null ) // mod by Dies Irae
                AddItem( new Server.Items.LongPants( 0x192 ) );
            AddItem( new Server.Items.Pickaxe() );
            if( FindItemOnLayer( Layer.Shoes ) == null ) // mod by Dies Irae
                AddItem( new Server.Items.ThighBoots( 0x283 ) );
		}

		public Miner( Serial serial ) : base( serial )
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