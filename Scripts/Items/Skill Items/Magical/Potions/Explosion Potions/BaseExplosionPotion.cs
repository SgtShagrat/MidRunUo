using System;
using System.Collections;

using Midgard.Engines.SpellSystem;

using Server.Network;
using Server.Targeting;
using Server.Spells;

namespace Server.Items
{
	public abstract class BaseExplosionPotion : BasePotion
	{
		public abstract int MinDamage { get; }
		public abstract int MaxDamage { get; }

		public override bool RequireFreeHand{ get{ return false; } }

		private static bool LeveledExplosion = true; // Should explosion potions explode other nearby potions?
		private static bool InstantExplosion = false; // Should explosion potions explode on impact?
		private const int ExplosionRange = 3;     // How long is the blast radius?

		private Timer m_Timer;
		private ArrayList m_Users;
		private BaseExplosionPotion m_ThrownPotion;

		public BaseExplosionPotion( PotionEffect effect, int amount ) : base( 0xF0D, effect, amount )
		{
		}

		public BaseExplosionPotion( Serial serial ) : base( serial )
		{
		}

		public virtual object FindParent( Mobile from, BaseExplosionPotion pot )
		{
			Mobile m = pot.HeldBy;

			if ( m != null && m.Holding == pot )
				return m;

			object obj = pot.RootParent;

			if ( obj != null )
				return obj;

			if ( Map == Map.Internal )
				return from;

			return pot;			
		}
		
		public virtual object FindParent( Mobile from )
		{
			return FindParent( from, this );
		}

        #region mod by Dies Irae

	    public virtual int ChanceOfAutoIgnition
	    {
	       get { return 12; } 
	    }

        public override int DelayUse
        {
            get { return 10; }
        }

        public override int BonusOnDelayAtHundred
        {
            get { return 2; }
        }

        public override bool CanDrink( Mobile from, bool message )
        {
            bool canDrink = from.CanBeginAction( typeof( BaseExplosionPotion ) );

            if( !canDrink && message )
                from.SendMessage( "You must wait until you can use another explosion potion!" );

            return canDrink;
        }

        public void LockExplosionPotionUse( Mobile from )
        {
            from.BeginAction( typeof( BaseExplosionPotion ) );
            Timer.DelayCall( GetDelayOfReuse( from ), new TimerStateCallback( ReleaseExplosionPotionLock ), from );
        }

	    private static void ReleaseExplosionPotionLock( object state )
        {
            ( (Mobile)state ).EndAction( typeof( BaseExplosionPotion ) );
        }

        #endregion

        public override void Drink( Mobile from )
		{
			if ( Core.AOS && (from.Paralyzed || from.Frozen || (from.Spell != null && from.Spell.IsCasting)) )
			{
				from.SendLocalizedMessage( 1062725 ); // You can not use a purple potion while paralyzed.
				return;
			}

			// Sanity per non rendere nullo from nel RepositionOnTick
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
				return;
			}	
			
			ThrowTarget targ = from.Target as ThrowTarget;
			if ( targ != null && targ.Potion == this )
				return;

			from.RevealingAction( true );

			if ( m_Users == null )
				m_Users = new ArrayList();

			if ( !m_Users.Contains( from ) )
				m_Users.Add( from );

			this.Consume();
	
			m_ThrownPotion = null;
			m_Timer = null;

            switch( PotionEffect )
            {
                case PotionEffect.ExplosionLesser: m_ThrownPotion = new LesserExplosionPotion( 1 ); break;
                case PotionEffect.Explosion: m_ThrownPotion = new ExplosionPotion( 1 ); break;
                default: m_ThrownPotion = new GreaterExplosionPotion( 1 ); break;
            }

			m_ThrownPotion.Stackable = false;
			from.AddToBackpack( m_ThrownPotion );
			
            #region mod by Dies Irae

            if( !Core.AOS )
                LockExplosionPotionUse( from );
            
            if( Utility.Random( 100 ) < ChanceOfAutoIgnition )
            {
                from.SendMessage( "Damn! Your potion exploded while being used." );
                m_ThrownPotion.Explode( from, true, from.Location, from.Map );
                return;
            }

            #endregion

			// from.Target = new ThrowTarget( this );
            from.Target = new ThrowTarget( m_ThrownPotion );

			if( m_Timer == null )
			{
				from.SendLocalizedMessage( 500236 ); // You should throw it now!
				m_Timer = Timer.DelayCall( TimeSpan.FromSeconds( 0.75 ), TimeSpan.FromSeconds( 1.0 ), 4, new TimerStateCallback( Detonate_OnTick ), new object[]{ from, 3, m_ThrownPotion } );
			}
		}

		public void Explode( Mobile from, bool direct, Point3D loc, Map map )
		{
			if ( Deleted )
				return;

			Delete();

			for ( int i = 0; m_Users != null && i < m_Users.Count; ++i )
			{
				Mobile m = (Mobile)m_Users[i];
				ThrowTarget targ = m.Target as ThrowTarget;

				if ( targ != null && targ.Potion == this )
					Target.Cancel( m );
			}

			if ( map == null )
				return;

			Effects.PlaySound( loc, map, Core.AOS ? 0x207 : 0x208 );
			Effects.SendLocationEffect( loc, map, Core.AOS ? 0x36BD : 0x36B0, 20 );

			int alchemyBonus = 0;

			if ( direct )
				alchemyBonus = (int)(from.Skills.Alchemy.Value / (Core.AOS ? 5 : 10));

			IPooledEnumerable eable = LeveledExplosion ? map.GetObjectsInRange( loc, ExplosionRange ) : map.GetMobilesInRange( loc, ExplosionRange );
			ArrayList toExplode = new ArrayList();

			int toDamage = 0;

			foreach ( object o in eable )
			{
				if ( o is Mobile )
				{
					toExplode.Add( o );
					++toDamage;
				}
				else if ( o is BaseExplosionPotion && o != this )
				{
					toExplode.Add( o );
				}
			}

			eable.Free();

			int min = Scale( from, MinDamage );
			int max = Scale( from, MaxDamage );

			for ( int i = 0; i < toExplode.Count; ++i )
			{
				object o = toExplode[i];

				if ( o is Mobile )
				{
					Mobile m = (Mobile)o;

					if ( from == null || (SpellHelper.ValidIndirectTarget( from, m, true, true, true ) && from.CanBeHarmful( m, false )) )
					{
                        if( m != null && !m.InLOS( loc ) )
                            continue;

						if ( from != null )
							from.DoHarmful( m );

						int damage = Utility.RandomMinMax( min, max );

                        if( Core.AOS)
						    damage += alchemyBonus;

						if ( !Core.AOS && damage > 40 )
							damage = 40;
						else if ( Core.AOS && toDamage > 2 )
							damage /= toDamage - 1;

                        if( Core.T2A )
                            MidgardSpellHelper.Damage( m, from, damage, CustomResType.Fire );
                        else
                            AOS.Damage( m, from, damage, 0, 100, 0, 0, 0 );
					}
				}
				else if ( o is BaseExplosionPotion )
				{
					BaseExplosionPotion pot = (BaseExplosionPotion)o;

					pot.Explode( from, false, pot.GetWorldLocation(), pot.Map );
				}
			}
		}

		private class ThrowTarget : Target
		{
			private BaseExplosionPotion m_Potion;

			public BaseExplosionPotion Potion
			{
				get{ return m_Potion; }
			}

			public ThrowTarget( BaseExplosionPotion potion ) : base( 12, true, TargetFlags.None )
			{
				m_Potion = potion;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Potion.Deleted || m_Potion.Map == Map.Internal )
					return;

				IPoint3D p = targeted as IPoint3D;

				if ( p == null )
					return;

				Map map = from.Map;

				if ( map == null )
					return;

				SpellHelper.GetSurfaceTop( ref p );

				from.RevealingAction( true );

				IEntity to;

				if ( p is Mobile )
					to = (Mobile)p;
				else
					to = new Entity( Serial.Zero, new Point3D( p ), map );

                from.PublicOverheadMessage( MessageType.Regular, 0x3B2, true, "* Bomber *" );

				Effects.SendMovingEffect( from, to, m_Potion.ItemID & 0x3FFF, 7, 0, false, false, m_Potion.Hue, 0 );

				if( m_Potion.Amount > 1 )
				{
					Mobile.LiftItemDupe( m_Potion, 1 );
				}

				m_Potion.Internalize();
				
				// Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( m_Potion.Reposition_OnTick ), new object[]{ from, p, map } );
				Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( m_Potion.Reposition_OnTick ), new object[]{ from, p, map, m_Potion } );
			}
		}

	    private void Detonate_OnTick( object state )
		{
			//if ( Deleted )
			// 	return;

            if( m_ThrownPotion.Deleted )
                return;

	        object[] states = (object[])state;
			Mobile from = (Mobile)states[0];
			int timer = (int)states[1];
			BaseExplosionPotion potion = (BaseExplosionPotion)states[2];

			// object parent = FindParent( from );	
			object parent = FindParent( from, potion );

			if ( timer == 0 )
			{
				Point3D loc;
				Map map;

				if ( parent is Item )
				{
					Item item = (Item)parent;

					loc = item.GetWorldLocation();
					map = item.Map;
				}
				else if ( parent is Mobile )
				{
					Mobile m = (Mobile)parent;

					loc = m.Location;
					map = m.Map;
				}
				else
				{
					return;
				}

				// Explode( from, true, loc, map );
				potion.Explode( from, true, loc, map );
			}
			else
			{
				if ( parent is Item )
					((Item)parent).PublicOverheadMessage( MessageType.Regular, 0x22, false, timer.ToString() );
				else if ( parent is Mobile )
					((Mobile)parent).PublicOverheadMessage( MessageType.Regular, 0x22, false, timer.ToString() );

				states[1] = timer - 1;
			}
		}

		private void Reposition_OnTick( object state )
		{
			//if ( Deleted )
			// 	return;
						
//			// Va considerata la pozione da lanciare non lo stack rimanente.
//			if( m_ThrownPotion.Deleted ) 
//			{
//				return;
//			}
			
			object[] states = (object[])state;
			
			Mobile from = (Mobile)states[0];
			IPoint3D p = (IPoint3D)states[1];
			Map map = (Map)states[2];
			BaseExplosionPotion potion = (BaseExplosionPotion)states[3];
			
			Point3D loc = new Point3D( p );

			if ( InstantExplosion )
			{
				// Explode( from, true, loc, map );
				potion.Explode( from, true, loc, map );
			}
			else
			{
				// MoveToWorld( loc, map );
				potion.MoveToWorld( loc, map );
			}
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