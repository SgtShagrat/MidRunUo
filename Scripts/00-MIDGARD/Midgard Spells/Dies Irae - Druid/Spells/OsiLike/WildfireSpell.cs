/***************************************************************************
 *                               Wildfire.cs
 *
 *   begin                : 27 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections;

using Server;
using Server.Items;
using Server.Misc;
using Server.Spells;
using Server.Targeting;

namespace Midgard.Engines.SpellSystem
{
	public class WildfireSpell : DruidSpell
	{
		private static readonly SpellInfo m_Info = new SpellInfo(
			"Wildfire", "Vauk Ohm En Tia Crur",
			224,
			9011,
			Reagent.Kindling,
			Reagent.SulfurousAsh
		);

		private static readonly ExtendedSpellInfo m_ExtendedInfo = new ExtendedSpellInfo
		(
			typeof( WildfireSpell ),
			"Creates a field of fire that damages enemies within it for a short time.",
			"Una fiammata intensa avvolge tutti i nemici che circondano il bersaglio del Druido."+
			"Raggio (2+FL); Durata (SK/24 + FL*5); Danno (10 + FL/2).",
			0x59e1
		);

		public override ExtendedSpellInfo ExtendedInfo
		{
			get { return m_ExtendedInfo; }
		}

		public override SpellCircle Circle{get { return SpellCircle.Fifth; }}
		public override int GetBaseMana(){return (int)(GetPowerLevel()*20);}
		public override int DelayOfReuse {get {return (int)(GetPowerLevel()*60);}}

		public WildfireSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			if( Caster.CanBeginAction( typeof( WildfireSpell ) ) )
				Caster.Target = new InternalTarget( this );
			else
				Caster.SendMessage( Caster.Language == "ITA" ? "Sei troppo debole per lanciare un nuovo muro di fuoco." : "You are too weak to send flames again." );
		}

		public void Target( Point3D p )
		{
			if( !Caster.CanSee( p ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if(!MidgardSpellHelper.CheckNoMoreFieldsNearby( Caster, 12 ))
			{
				Caster.SendMessage( (Caster.Language == "ITA" ? "Non puoi evocarne più di uno!" : "You cannot cast more than one of those!") );
			}
			else if( SpellHelper.CheckTown( p, Caster ) && CheckSequence() )
			{
				Caster.BeginAction( typeof( WildfireSpell ) );

				int level = FocusLevel;//GetFocusLevel( Caster ) + (int)(GetPowerLevel()/2);
				double skill = Caster.Skills[ CastSkill ].Value;

				int tiles = 2 + level;
				int damage = 10 + ( level / 2 );
				int durationInSeconds = (int)Math.Max( 1, skill / 24 ) + ( level * 5 );

				for( int x = p.X - tiles; x <= p.X + tiles; x++ )
				{
					for( int y = p.Y - tiles; y <= p.Y + tiles; y++ )
					{
						int averZ = Caster.Map.GetAverageZ( x, y );
						IPoint3D loc = new Point3D( x, y, p.Z );//averZ );//Caster.Map.GetAverageZ( x, y );
						SpellHelper.GetSurfaceTop( ref loc );

						if (loc.Z + 10 >= averZ && loc.Z - 10 <= averZ)
							loc = new Point3D( loc.X, loc.Y, averZ );

						bool vis = ((y == p.Y - tiles || y == p.Y + tiles || x == p.X - tiles || x == p.X + tiles)? true: false);
						//bool canFit = SpellHelper.AdjustField( ref loc, Caster.Map, 22, false ) && Caster.Map.LineOfSight( p, loc );
						//if( !canFit )
						//	continue;
						// if( Caster.InLOS( p3D ) && Caster.Map.LineOfSight( startingPoint, p3D ) && Caster.Map.CanFit( p3D, 12, true, false ) )
						if( Caster.InLOS( loc ) )//&& p.Z + 10 >= averZ && p.Z - 10 <= averZ )
							new WildFireItem( Utility.RandomBool() ? 0x398C : 0x3996, new Point3D(loc), Caster, Caster.Map, durationInSeconds, damage, vis );
					}
				}

				Effects.PlaySound( p, Caster.Map, 0x5CF );
				Timer.DelayCall( GetDelayOfReuse(), new TimerStateCallback( ReleaseWildfireLock ), Caster );
			}

			FinishSequence();
		}

		public class InternalTarget : Target
		{
			private readonly WildfireSpell m_Owner;

			public InternalTarget( WildfireSpell owner ) : base( 12, false, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile m, object o )
			{
				if( o != null && o is Mobile && o == m_Owner.Caster )
				{
					m_Owner.Caster.SendMessage( (m_Owner.Caster.Language == "ITA" ? "Non puoi farlo su te stesso." : "Thou cannot target yourself." ) );
					return;
				}

				if( o is Mobile )
					m_Owner.Target( ( (Mobile)o ).Location );
				else if( m_Owner.Caster != null )
					m_Owner.Caster.SendMessage( (m_Owner.Caster.Language == "ITA" ? "Devi selezionare un essere vivente." : "Thou must target a valid living target.") );
			}

			protected override void OnTargetFinish( Mobile m )
			{
				m_Owner.FinishSequence();
			}
		}

		private static void ReleaseWildfireLock( object state )
		{
			( (Mobile)state ).EndAction( typeof( WildfireSpell ) );
			( (Mobile)state ).SendMessage( ((Mobile)state).Language == "ITA"? "Puoi evocare un nuovo muro di fuoco!" : "You can make a new wall of fire!" );
		}

		[NoSameNearbyItem]
		[DispellableField]
		public class WildFireItem : Item
		{
			private Timer m_Timer;
			private DateTime m_End;
			private Mobile m_Caster;
			private int m_Damage;

			private const double DamageScalar = 0.1;

			public override bool BlocksFit { get { return true; } }

			public WildFireItem( int itemID, Point3D loc, Mobile caster, Map map, int durationInSeconds, int damage, bool vis ) : base( itemID )
			{
				bool canFit = SpellHelper.AdjustField( ref loc, map, 12, false );

				Visible = vis;
				Movable = false;
				Light = LightType.Circle300;

				MoveToWorld( loc, map );

				m_Caster = caster;

				m_Damage = damage;
				m_End = DateTime.Now + TimeSpan.FromSeconds( durationInSeconds );

				m_Timer = new WildFireItemDamageTimer( this, TimeSpan.FromSeconds( 1.0 ), caster.InLOS( this ), canFit );
				m_Timer.Start();
			}

			public override void OnAfterDelete()
			{
				base.OnAfterDelete();

				if( m_Timer != null )
					m_Timer.Stop();
			}

			#region serialization
			public WildFireItem( Serial serial ) : base( serial )
			{
			}

			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );

				writer.Write( 0 ); // version

				writer.Write( m_Damage );
				writer.Write( m_Caster );
				writer.WriteDeltaTime( m_End );
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );

				int version = reader.ReadInt();

				m_Damage = reader.ReadInt();
				m_Caster = reader.ReadMobile();
				m_End = reader.ReadDeltaTime();

				m_Timer = new WildFireItemDamageTimer( this, TimeSpan.Zero, true, true );
				m_Timer.Start();
			}
			#endregion

			public override bool OnMoveOver( Mobile m )
			{
				if (!Visible)
					Visible = true;
				if( m_Caster != null && m != m_Caster && SpellHelper.ValidIndirectTarget( m_Caster, m ) && m_Caster.CanBeHarmful( m, false ) )
				{
					if( SpellHelper.CanRevealCaster( m ) )
						m_Caster.RevealingAction();

					if ( m_Caster.CanAreaHarmful( m ) )
						m_Caster.DoHarmful( m );

					int damage = (int)( m_Damage * ( m.Player ? DamageScalar : 1.0 ) );

					m.PlaySound( 0x208 );

					MidgardSpellHelper.Damage( m, m_Caster, damage, CustomResType.Fire );
				}

				return true;
			}

			private class WildFireItemDamageTimer : Timer
			{
				private readonly WildFireItem m_Item;
				private readonly bool m_InLOS;
				private readonly bool m_CanFit;
				//private static readonly Queue m_Queue = new Queue();

				public WildFireItemDamageTimer( WildFireItem item, TimeSpan delay, bool inLOS, bool canFit )
							: base( delay, TimeSpan.FromSeconds( 1.0 ) )
				{
					m_Item = item;
					m_InLOS = inLOS;
					m_CanFit = canFit;

					Priority = TimerPriority.FiftyMS;
				}

				protected override void OnTick()
				{
					if( m_Item.Deleted )
						return;

					if( !m_Item.Visible )
					{
						if( m_InLOS && m_CanFit )
						{
							if (Utility.Random( 20 ) == 1 )
								m_Item.Visible = true;
						}
						else
							m_Item.Delete();

						if( !m_Item.Deleted )
						{
							m_Item.ProcessDelta();
							//Effects.SendLocationParticles( EffectItem.Create( m_Item.Location, m_Item.Map, EffectItem.DefaultDuration ), 0x376A, 9, 10, 5029 );
						}
					}

					if( DateTime.Now > m_Item.m_End )
					{
						m_Item.Delete();
						Stop();
					}
					else
					{
						Map map = m_Item.Map;
						Mobile caster = m_Item.m_Caster;

						if( map != null && caster != null )
						{
							foreach( Mobile m in m_Item.GetMobilesInRange( 0 ) )
							{
								if( ( m.Z + 16 ) > m_Item.Z && ( m_Item.Z + 12 ) > m.Z && m != caster && SpellHelper.ValidIndirectTarget( caster, m ) && caster.CanBeHarmful( m, false ) )
								{
									if( SpellHelper.CanRevealCaster( m ) )
										caster.RevealingAction();

									if ( caster.CanAreaHarmful( m ) )
										caster.DoHarmful( m );

									int damage = (int)( m_Item.m_Damage * ( m.Player ? DamageScalar : 1.0 ) );

									m.PlaySound( 0x208 );

									MidgardSpellHelper.Damage( m, caster, damage, CustomResType.Fire );
								}
							}
						}
					}
				}
			}
		}
	}
}