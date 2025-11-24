using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using Server.Misc;
using Server.Items;
using Server.Mobiles;
using Server.Regions;
using Midgard.Engines.MidgardTownSystem;

namespace Server.Mobiles
{
	public abstract class BaseGuard : /* Mobile */ BaseCreature // mod by Dies Irae : killable guards
	{
		#region mod by Dies Irae
		public override SpeechFragment PersonalFragmentObj { get { return PersonalFragment.Guard; } } // mod by Dies Irae

		public override Poison PoisonImmune { get { return Poison.Lethal; } }

		public override bool GuardImmune { get { return true; } }

		public override bool BardImmune { get { return true; } }

		public override bool IsScaryToPets { get { return true; } }

		public override bool CanRummageCorpses { get { return false; } }

		public override bool CanOpenDoors { get { return true; } }

		private bool m_IsBackupGuard = false;

		public virtual bool IsBackupGuard { get { return m_IsBackupGuard; } set { m_IsBackupGuard = value; } }

		// public override bool ReacquireOnMovement { get { return true; } }

		public override bool AutoDispel{ get{ return true; } }

		public override bool UsesWeaponDamage { get { return true; } }

		public override double GetFightModeRanking( Mobile m, FightMode acqType, bool bPlayerOnly )
		{
			return base.GetFightModeRanking( m, acqType, false );
		}

		public BaseGuard( Mobile target, AIType AI ) : base( AI, FightMode.Closest, 10, 1, 0.1, 4.0 )
		{

			if( target != null )
			{
				Location = target.Location;
				Map = target.Map;

				Effects.SendLocationParticles( EffectItem.Create( Location, Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 5023 );
			}

			if( target == null )
				IsBackupGuard = false;
		}

		//All guards will default to AI_Melee unless you give them an alternate AI
		public BaseGuard( Mobile target ): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.1, 4.0 ) 
		{ 

			if ( target != null )
			{
				Location = target.Location;
				Map = target.Map;

				Effects.SendLocationParticles( EffectItem.Create( Location, Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 5023 );
			}

			if ( target == null )
				IsBackupGuard = false;
		}

		public override void OnKilledBy( Mobile mob )
		{
			LogGuardDeath( mob );

			base.OnKilledBy( mob );
		}

		private void LogGuardDeath( Mobile killer )
		{
			try
			{
				string message = String.Format( "Warning: The guard {0} has been killed by {1} al location {2} in the map of {3}",
									Name ?? "", killer != null ? killer.Name : "no mobile", Location, Map.Name );

				Config.Pkg.LogWarningLine( message );

				using( StreamWriter op = new StreamWriter( "Logs/townGuardKills.log", true ) )
				{
					op.WriteLine( "{0}\t{1}", DateTime.Now, message );
					op.WriteLine();
				}
			}
			catch
			{
			}
		}
		#endregion

		public static void Spawn( Mobile caller, Mobile target )
		{
			Spawn( caller, target, 1, false, false );
		}

		public static void Spawn( Mobile caller, Mobile target, bool backup )
		{
			Spawn( caller, target, 1, false, backup );
		}

		public static void Spawn( Mobile caller, Mobile target, int amount, bool onlyAdditional, bool backup )
		{
			if ( target == null || target.Deleted )
				return;

			int count = 0;

			foreach ( Mobile m in target.GetMobilesInRange( 15 ) )
			{
				if ( m is BaseGuard )
				{
					count++;

					BaseGuard g = (BaseGuard)m;

					if ( g.Focus == null ) // idling
					{
						g.Focus = target;

						--amount;
					}
					else if ( g.Focus == target && !onlyAdditional )
					{
						--amount;
					}
				}
			}

			if( count > 5 )
			{
				GuardLog( target );
				return;
			}

			while ( amount-- > 0 )
				caller.Region.MakeGuard( target, backup );
		}

		private static void GuardLog( Mobile m )
		{
			string message = String.Format( "Warning: Attempted to create more than 5 guards in the same location." +
				"\nLocation: {0}", m.Location );

			Console.WriteLine( message );

			try
			{
				using ( StreamWriter op = new StreamWriter( "Logs/guards-errors.log", true ) )
				{
					op.WriteLine( "{0}\t{1}", DateTime.Now, message );
					op.WriteLine( new StackTrace( 2 ).ToString() );
					op.WriteLine();
				}
			} 
			catch
			{
			}
		}
		
		/* mod by Dies Irae : killable guards
		public BaseGuard( Mobile target )
		{
			if ( target != null )
			{
				Location = target.Location;
				Map = target.Map;

				Effects.SendLocationParticles( EffectItem.Create( Location, Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 5023 );
			}
		}
		*/

		public BaseGuard( Serial serial ) : base( serial )
		{
		}

		#region mod by Dies Irae : killable guards
		public override bool IsEnemy( Mobile m )
		{
			if( m == null )
			{
				DebugSay( "What? I don't bother null things!" );
				return false;
			}

			// If you add in this line it will be not an enemy
			if( ( m is BaseCreature && ((BaseCreature)m).GuardImmune ) || m is TownCrier )
			{
				DebugSay( "{0} it's not a valid enemy: It's a protected being!", m.Name ?? "no named." );
				return false;
			}

			if( !m.CanBeDamaged() )
			{
				DebugSay( "{0} it's not a valid enemy: cannot be damaged!", m.Name ?? "no named." );
				return false;
			}

			GuardedRegion rgn = null;
			if( m.Region != null )
				rgn = m.Region as GuardedRegion;

			if( rgn != null && m.Region.IsPartOf( rgn ) )
			{
				int noto = Notoriety.Compute( this, m );

				if( m.Criminal || ( !rgn.AllowReds && noto == Notoriety.Murderer ) )
				{
					DebugSay( "My enemy is {0}", m.Name ?? "no named." );
					return true;
				}

				BaseCreature c = m as BaseCreature;
				if( c != null && ( c.AlwaysMurderer && !rgn.AllowReds ) )
				{
					DebugSay( "My enemy is an always murderer ({0}).", m.Name ?? "no named." );
					return true;
				}
			}

			DebugSay( "{0} is not an enemy.", m.Name ?? "no named." );

			return false;
		}

		public override bool IsHarmfulCriminal( Mobile target )
		{
			return false;
		}

		/* public override bool OnBeforeDeath()
		{
			Effects.SendLocationParticles( EffectItem.Create( Location, Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );

			PlaySound( 0x1FE );

			Delete();

			return false;
		}
		*/
		#endregion

		public abstract Mobile Focus{ get; set; }

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