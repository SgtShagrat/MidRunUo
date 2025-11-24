/***************************************************************************
 *							   BloodVault.cs
 *							-------------------
 *   begin				: 26 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.SpellSystem
{
	public class BloodVault : BloodPentagram
	{
		private Mobile m_Necromancer;

		[Constructable]
		public BloodVault( Mobile necromancer )
		{
			m_Necromancer = necromancer;

			if( m_Necromancer != null )
				AddComponent( new FlamesOfSorrow( necromancer ), 3, 3, 10 );
		}

		#region serialization
		public BloodVault( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
		#endregion

		private class FlamesOfSorrow : AddonComponent
		{
			private Mobile m_Necromancer;

			[Constructable]
			public FlamesOfSorrow( Mobile necromancer ) : base( 14089 )
			{
				m_Necromancer = necromancer;

				Hue = 1157;
			}

			public override bool OnMoveOver( Mobile m )
			{
				if( m_Necromancer == null )
					return false;

				if( m != null && m_Necromancer != null && m == m_Necromancer )
				{
					m.PublicOverheadMessage( MessageType.Regular, 37, true, m.Language == "ITA" ? "*il sangue non può aiutare se stesso*" : "*blood cannot aid blood*" );
					return false;
				}
				else if( m is PlayerMobile && !m.Alive && m.Map != Map.Internal )
				{
					if( Classes.ClassSystem.IsPaladine( m ) )
					{
						m.PublicOverheadMessage( MessageType.Regular, 37, true, m.Language == "ITA"? "*il sangue è stato purificato*" : "*the blood has been purified*" );

						int range = 5 + 2 * RPGSpellsSystem.GetPowerLevel( m_Necromancer, typeof( VaultOfBloodSpell ) );

						List<Mobile> mobileList = new List<Mobile>();
						foreach( Mobile mobile in GetMobilesInRange( range ) )
						{
							if( !m_Necromancer.CanBeHarmful( mobile ) || mobile.Karma > 0 )
								continue;

							mobileList.Add( mobile );
						}

						foreach( Mobile mobile in mobileList )
						{
							Effects.SendLocationEffect( mobile.Location, mobile.Map, 0x3709, 30, 10, Utility.RandomRedHue(), 0 );
							mobile.Damage( Utility.Dice( 1, 30, 30 ) );
						}

						Delete();
						return false;
					}
					else
					{
						Effects.SendLocationEffect( m.Location, m.Map, 0x3709, 30, 10, Utility.RandomRedHue(), 0 );
						m.PlaySound( 0x208 );
						m.Resurrect();
						m.PublicOverheadMessage( MessageType.Regular, 37, true, m.Language == "ITA" ? "*il sangue ti ha dato un'altra opportunità*" : "*the power of blood gave you another chance*" );

						Effects.SendLocationEffect( m_Necromancer.Location, m_Necromancer.Map, 0x3709, 30, 10, Utility.RandomRedHue(), 0 );
						m_Necromancer.PlaySound( 0x208 );
						m_Necromancer.Damage( Utility.Dice( 1, 30, 10 ) );
						m_Necromancer.PublicOverheadMessage( MessageType.Regular, 37, true,m_Necromancer.Language == "ITA" ? "*il sangue brucia il tuo corpo*" : "*the power of blood burns your body*" );

						return false;
					}
				}

				return false;
			}

			#region serialization
			public FlamesOfSorrow( Serial serial ) : base( serial )
			{
			}

			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );

				writer.Write( (int)0 ); // version
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );

				int version = reader.ReadInt();
			}
			#endregion
		}
	}
}