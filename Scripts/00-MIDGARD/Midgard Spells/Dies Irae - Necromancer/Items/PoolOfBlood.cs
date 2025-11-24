/***************************************************************************
 *							   PoolOfBlood.cs
 *
 *   begin				: 26 September, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using Server;
using Server.Items;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
	public class PoolOfBlood : PoolOfAcid
	{
		private Timer m_Timer;
		private Mobile m_Caster;
		public override string DefaultName { get { return "a pool of cursed blood"; } }
		private const double DamageScalar = 0.1;

		[Constructable]
		public PoolOfBlood( Mobile Caster, TimeSpan duration, int minDamage, int maxDamage ) : base( duration, minDamage, maxDamage )
		{
			m_Caster = Caster;
			Hue = 1456;//Utility.RandomRedHue();//1456

			m_Timer = new PoolOfBloodDamageTimer( this, TimeSpan.FromSeconds( 1.0 ) );
			m_Timer.Start();
		}

		public override void Damage( Mobile m )
		{
			//if( Classes.ClassSystem.IsNecromancer( m ) )
			//	return;

			if( m_Caster != null && m != m_Caster && SpellHelper.ValidIndirectTarget( m_Caster, m ) && m_Caster.CanBeHarmful( m, false ) )
			{
				if ( m_Caster.CanAreaHarmful( m ) )
					m_Caster.DoHarmful( m );

				int damage = (int)( Utility.RandomMinMax( 10, 15 ) * ( m.Player ? DamageScalar : 1.0 ) );

				MidgardSpellHelper.Damage( m, m_Caster, damage, CustomResType.General );

				//m.Damage( Utility.RandomMinMax( m_MinDamage, m_MaxDamage ) );
			}
			//base.Damage( m );
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			if( m_Timer != null )
				m_Timer.Stop();
		}

		#region serialization
		public PoolOfBlood( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
		}

		public override void Deserialize( GenericReader reader )
		{
		}
		#endregion

		private class PoolOfBloodDamageTimer : Timer
		{
			private readonly PoolOfBlood m_Item;

			public PoolOfBloodDamageTimer( PoolOfBlood item, TimeSpan delay ) : base( delay, TimeSpan.FromSeconds( 1.0 ) )
			{
				m_Item = item;

				Priority = TimerPriority.FiftyMS;
			}

			protected override void OnTick()
			{
				if( m_Item.Deleted )
					return;

				Map map = m_Item.Map;
				Mobile caster = m_Item.m_Caster;

				if( map != null && caster != null )
				{
					foreach( Mobile m in m_Item.GetMobilesInRange( 0 ) )
					{
						if( ( m.Z + 16 ) > m_Item.Z && ( m_Item.Z + 12 ) > m.Z )
						{
							m_Item.Damage( m );
						}
					}
				}
			}
		}
	}
}