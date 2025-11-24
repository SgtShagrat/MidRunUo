using Server.Mobiles;

namespace Server.Items
{
	public class PetGreaterCurePotion : BasePetHealthPotion
	{
		#region campi
		public override double PercProperFun{ get{ return 0.75;} }
		public override int DelayUse{ get{ return 10;} }
		#endregion
		
		#region CureLevelInfo
		private static CureLevelInfo[] m_LevelInfo = new CureLevelInfo[]
		{
			new CureLevelInfo( Poison.Lesser,  1.00 ),
			new CureLevelInfo( Poison.Regular, 1.00 ),
			new CureLevelInfo( Poison.Greater, 1.00 ),
			new CureLevelInfo( Poison.Deadly,  0.95 ),
			new CureLevelInfo( Poison.Lethal,  0.75 )
		};
		#endregion
		
		#region costruttori
		[Constructable]
		public PetGreaterCurePotion( int amount ) : base( PotionEffect.PetCureGreater, amount )
		{
			// Name = "Pet Greater Cure Potion";
			Hue = 2482;
		}
		
		[Constructable]
		public PetGreaterCurePotion() : this(1)
		{
		}
		
		public PetGreaterCurePotion( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region metodi
		public override void DoPetHealthEffect( Mobile user, BaseCreature pet )
		{
			if( pet.Poisoned )
			{
				bool cure = false;
				CureLevelInfo[] info = m_LevelInfo;
	
				for ( int i = 0; i < info.Length; ++i )
				{
					CureLevelInfo li = info[i];
					if ( li.Poison == pet.Poison && Scale( user, li.Chance ) > Utility.RandomDouble() )
					{
						cure = true;
						break;
					}
				}
	
				if( cure )
				{
					pet.Poison = null;
					pet.FixedParticles( 0x373A, 10, 15, 5012, EffectLayer.Waist );
					user.PlaySound( 0x1E0 );
					user.SendMessage( "Il tuo animale ora è di nuovo non avvelenato." );
				}
				else
				{
					user.SendMessage( "La pozione funziona ma il veleno è troppo potente e persiste." );										
				}
				
				this.Consume();
			}
			else
			{
				user.SendMessage( "Il tuo animale non è avvelenato." );
			}
		}
		#endregion
		
		#region serial deserial
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
		#endregion
	}
}