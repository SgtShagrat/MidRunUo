/***************************************************************************
 *							   IncendiaryArrow.cs
 *
 *   begin				: 08 October, 2009
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections;

using Server;
using Server.Items;
using Server.Spells;

namespace Midgard.Items
{
	public class IncendiaryArrow : BaseScoutArrow, ICommodity
	{
		#region ICommodity members
		string ICommodity.Description{get { return String.Format( Amount == 1 ? "{0} incendiary arrow" : "{0} incendiary arrows", Amount ); }}
		int ICommodity.DescriptionNumber{get { return 0; }}
		#endregion

		public override string DefaultName{get { return "incendiary arrow"; }}

		[Constructable]
		public IncendiaryArrow() : this( 1 )
		{
		}

		[Constructable]
		public IncendiaryArrow( int amount )
		{
			Amount = amount;
			Hue = 1654;
		}

		public override void OnSingleClick( Mobile from )
		{
			LabelTo( from, StringUtility.ConvertItemName( from.Language == "ITA" ? "frecc%iae% incendiari%ae%" : "incendiary arrow%s%", Amount, from.Language ) );
		}

		public override void OnHit( BaseRanged baseRanged, Mobile attacker, Mobile defender, double damageBonus )
		{
			//if( attacker.CanBeginAction( typeof( IncendiaryArrow ) ) )
			//{
				if( CheckSkill() )
				{
					attacker.SendMessage( attacker.Language == "ITA" ? "Hai colpito il nemico con successo con una freccia incendiaria." : "You successfully hit your enemy with an incendiary arrow." );
					defender.SendMessage( defender.Language == "ITA" ? "Sei stato colpito da una freccia incendiaria." : "You have been hit by an incendiary arrow." );

					//attacker.BeginAction( typeof( IncendiaryArrow ) );
					Explode( attacker, defender.Location, defender.Map );
					//Timer.DelayCall( baseRanged.GetCoolingDelay( attacker, typeof( IncendiaryArrow ) ), new TimerStateCallback( ReleaseIncendiaryArrowLock ), attacker );
				}
				else
				{
					attacker.SendMessage( attacker.Language == "ITA" ? "La tua freccia incendiaria non è esplosa!" : "Your incendiary arrow did not explode!" );
					defender.SendMessage( defender.Language == "ITA" ? "Il tuo nemico ti sta lanciando frecce incendiarie!" : "Your opponent tried to hit you with an exploding ammo but failed!" );
				}
			//}

			base.OnHit( baseRanged, attacker, defender, damageBonus );
		}

		private void Explode( Mobile from, IPoint3D loc, Map map )
		{
			if( Deleted || map == null )
				return;

			// Effects
			Effects.PlaySound( loc, map, 0x20C );

			for( int i = -2; i <= 2; i++ )
			{
				for( int j = -2; j <= 2; j++ )
				{
					Point3D p = new Point3D( loc.X + i, loc.Y + j, loc.Z );

					if( map.CanFit( p, 12, true, false ) && from.InLOS( p ) )
						new InternalItem( from, p, map, 5, 10 );
				}
			}
		}

		private static void ReleaseIncendiaryArrowLock( object state )
		{
			( (Mobile)state ).EndAction( typeof( IncendiaryArrow ) );
		}

		private class InternalItem : Item
		{
			private static int[] m_ItemIDs = new int[]{0x398C, 0x3996};
			private Mobile m_Scout;
			private DateTime m_End;
			private Timer m_Timer;
			private int m_Min;
			private int m_Max;

			public override bool BlocksFit { get { return true; } }

			public InternalItem( Mobile scout, Point3D loc, Map map, int min, int max ) : base( Utility.RandomList( m_ItemIDs ) )
			{
				Movable = false;
				Light = LightType.Circle300;

				MoveToWorld( loc, map );

				m_Scout = scout;
				m_End = DateTime.Now + TimeSpan.FromSeconds( 10 );
				m_Min = min;
				m_Max = max;

				m_Timer = new InternalTimer( this, min, max );
				m_Timer.Start();
			}

			public override void OnAfterDelete()
			{
				base.OnAfterDelete();

				if( m_Timer != null )
					m_Timer.Stop();
			}

			public InternalItem( Serial serial ) : base( serial )
			{
			}

			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );

				writer.Write( 0 ); // version

				writer.Write( m_Scout );
				writer.Write( m_End );
				writer.Write( m_Min );
				writer.Write( m_Max );
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );

				int version = reader.ReadInt();

				m_Scout = reader.ReadMobile();
				m_End = reader.ReadDateTime();
				m_Min = reader.ReadInt();
				m_Max = reader.ReadInt();

				m_Timer = new InternalTimer( this, m_Min, m_Max );
				m_Timer.Start();
			}

			public override bool OnMoveOver( Mobile m )
			{
				if( Visible && m_Scout != null && m != m_Scout && SpellHelper.ValidIndirectTarget( m_Scout, m ) && m_Scout.CanBeHarmful( m, false ) )
				{
					m_Scout.DoHarmful( m );

					int damage = Utility.RandomMinMax( m_Min, m_Max );

					AOS.Damage( m, m_Scout, damage, 0, 100, 0, 0, 0 );
					m.PlaySound( 0x208 );
				}

				return true;
			}

			private class InternalTimer : Timer
			{
				private InternalItem m_Item;
				private int m_Min;
				private int m_Max;

				private static Queue m_Queue = new Queue();

				public InternalTimer( InternalItem item, int min, int max ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 1.0 ) )
				{
					m_Item = item;
					m_Min = min;
					m_Max = max;

					Priority = TimerPriority.FiftyMS;
				}

				protected override void OnTick()
				{
					if( m_Item.Deleted )
						return;

					if( DateTime.Now > m_Item.m_End )
					{
						m_Item.Delete();
						Stop();
					}
					else
					{
						Map map = m_Item.Map;
						Mobile scout = m_Item.m_Scout;

						if( map != null && scout != null )
						{
							foreach( Mobile m in m_Item.GetMobilesInRange( 0 ) )
							{
								if( ( m.Z + 16 ) > m_Item.Z && ( m_Item.Z + 12 ) > m.Z && ( !Core.AOS || m != scout ) && SpellHelper.ValidIndirectTarget( scout, m ) && scout.CanBeHarmful( m, false ) )
									m_Queue.Enqueue( m );
							}

							while( m_Queue.Count > 0 )
							{
								Mobile m = (Mobile)m_Queue.Dequeue();

								scout.DoHarmful( m );

								int damage = Utility.RandomMinMax( m_Min, m_Max );

								AOS.Damage( m, scout, damage, 0, 100, 0, 0, 0 );
								m.PlaySound( 0x208 );
							}
						}
					}
				}
			}
		}

		#region serialization
		public IncendiaryArrow( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
		#endregion
	}
}