using Server.Engines.VeteranRewards;

namespace Server.Items
{
	public class VetMiniHouseAddon : MiniHouseAddon, IRewardItem
	{
		private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get { return m_IsRewardItem; }
			set { m_IsRewardItem = value; }
		}

		public override BaseAddonDeed Deed
		{
			get
			{
				if (Type == MiniHouseType.MalasMountainPass)
				{
					MalasMountainPassDeed deed = new MalasMountainPassDeed();
					deed.IsRewardItem = IsRewardItem;
					return deed;
				}
				else if (Type == MiniHouseType.ChurchAtNight)
				{
					ChurchAtNightDeed deed = new ChurchAtNightDeed();
					deed.IsRewardItem = IsRewardItem;
					return deed;
				}
				else
					return new MiniHouseDeed( Type );
			}
		}

		[Constructable]
		public VetMiniHouseAddon() : this( MiniHouseType.ChurchAtNight )
		{
		}

		[Constructable]
		public VetMiniHouseAddon( MiniHouseType type ) : base( type )
		{
		}

		public VetMiniHouseAddon( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int)0 ); // version

			writer.Write( m_IsRewardItem );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_IsRewardItem = reader.ReadBool();
		}
	}
	
	public class MalasMountainPassDeed : MiniHouseDeed, IRewardItem
	{
		private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get { return m_IsRewardItem; }
			set { m_IsRewardItem = value; }
		}

		public override BaseAddon Addon
		{
			get
			{
				VetMiniHouseAddon addon = new VetMiniHouseAddon( Type );
				addon.IsRewardItem = IsRewardItem;
				return addon;
			}
		}

		[Constructable]
        public MalasMountainPassDeed() : base(MiniHouseType.MalasMountainPass)
		{
			LootType = LootType.Blessed;
        }

        public MalasMountainPassDeed(Serial serial) : base(serial)
        {
        }

        public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			
			if( m_IsRewardItem )
				list.Add( 1065528 ); // Midgard Veteran Reward
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int)0 ); // version

			writer.Write( m_IsRewardItem );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_IsRewardItem = reader.ReadBool();
		}
	}
	
	public class ChurchAtNightDeed : MiniHouseDeed, IRewardItem
	{
		private bool m_IsRewardItem;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsRewardItem
		{
			get { return m_IsRewardItem; }
			set { m_IsRewardItem = value; }
		}

		public override BaseAddon Addon
		{
			get
			{
				VetMiniHouseAddon addon = new VetMiniHouseAddon( Type );
				addon.IsRewardItem = IsRewardItem;
				return addon;
			}
		}

		[Constructable]
        public ChurchAtNightDeed() : base( MiniHouseType.ChurchAtNight )
		{
			LootType = LootType.Blessed;
        }

        public ChurchAtNightDeed(Serial serial) : base(serial)
        {
        }

        public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			
			if( m_IsRewardItem )
				list.Add( 1065528 ); // Midgard Veteran Reward
		}
        
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int)0 ); // version

			writer.Write( m_IsRewardItem );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();

			m_IsRewardItem = reader.ReadBool();
		}
	}
}
