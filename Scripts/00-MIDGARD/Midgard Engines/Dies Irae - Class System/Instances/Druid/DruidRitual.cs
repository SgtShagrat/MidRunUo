using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Midgard.Engines.Classes
{
	public sealed class DruidRitual : Ritual
	{
		private static readonly Type AirSigilType = typeof( AirSigil );
		private static readonly Type EarthSigilType = typeof( EarthSigil );
		private static readonly Type FireSigilType = typeof( FireSigil );
		private static readonly Type WaterSigilType = typeof( WaterSigil );
		private static readonly Type TimeSigilType = typeof( TimeSigil );
		private static readonly Type EquilibriumSigilType = typeof( EquilibriumSigil );

		public static RequirementDefinition AirReq = new RequirementDefinition( AirSigilType, 1, "air sigil" );
		public static RequirementDefinition EarthReq = new RequirementDefinition( EarthSigilType, 1, "earth sigil" );
		public static RequirementDefinition FireReq = new RequirementDefinition( FireSigilType, 1, "fire sigil" );
		public static RequirementDefinition WaterReq = new RequirementDefinition( WaterSigilType, 1, "water sigil" );
		public static RequirementDefinition TimeReq = new RequirementDefinition( TimeSigilType, 1, "time sigil" );
		public static RequirementDefinition EquilibriumReq = new RequirementDefinition( EquilibriumSigilType, 1, "equilibrium sigil" );

		public DruidRitual( PowerDefinition definition, Mobile ritualist ) : base( definition, ritualist )
		{
		}

		public override void Start()
		{
			if( CheckCombat( Ritualist ) )
				Ritualist.PublicOverheadMessage( MessageType.Regular, 0x3B2, true, Ritualist.Language == "ITA" ? "*Il tuo spirito è inquieto per via di un combattimento*" : "*Your soul is not quiet due to a recent engage*" );
			else if( !Ritualist.CanBeginAction( typeof( Ritual ) ) )
				Ritualist.SendMessage( Ritualist.Language == "ITA" ? "{0}, non puoi ancora iniziare un rituale." : "{0}, you cannot start a new ritual yet.", Ritualist.Name );
			else
			{
				Ritualist.SendMessage( Ritualist.Language == "ITA" ? "Scegli un contenitore nello zaino contenente gli oggetti necessari al rituale." : "Choose a container in your backpack where the ritual items are placed in." );
				Ritualist.Target = new InternalTarget( this );
			}
		}

		public void Target( Item item )
		{
			if( !Ritualist.CanSee( item ) )
				Ritualist.SendLocalizedMessage( 500237 ); // Target can not be seen.
			else if( !( item is Container ) )
				Ritualist.SendMessage( Ritualist.Language == "ITA" ? "Non è un contenitore valido." : "That is not a valid container." );
			else if( !( item.IsChildOf( Ritualist.Backpack ) ) )
				Ritualist.SendMessage( Ritualist.Language == "ITA" ? "Non è accessibile." : "That is not accessible." );
			else if( !CheckResources( (Container)item, Ritualist as Midgard2PlayerMobile ) )
				Ritualist.SendMessage( Ritualist.Language == "ITA" ? "Manca qualche oggetto." : "Some item is lacking." );
			else
				BeginRitual();
		}

		public override double GetRitualDuration( int level )
		{
			return AnimTimer.DurationPerLevel * level;
		}

		public override void BeginRitual()
		{
			Midgard2PlayerMobile from = (Midgard2PlayerMobile)Ritualist;
			if( from == null || from.ClassState == null )
			{
				Ritualist.SendMessage( Ritualist.Language == "ITA" ? "Non puoi fare questo rituale!" : "You cannot make this ritual!" );
				return;
			}

			double delay = AnimTimer.AnimateDelay;
			int level = from.ClassState.GetLevel( Definition );
			int count = (int)Math.Ceiling( GetRitualDuration( level ) / delay ) + 5;

			if( Config.Debug )
				Config.Pkg.LogInfoLine( "Druid ritual. BeginRitual for: level {0}", count );

			if( count > 0 )
			{
				AnimTimer animTimer = new AnimTimer( Ritualist, this, count, level );
				animTimer.Start();

				double effectiveDuration = ( count * delay ) + 1.0;
				Ritualist.Freeze( TimeSpan.FromSeconds( effectiveDuration ) );

				Timer.DelayCall( TimeSpan.FromSeconds( effectiveDuration ), new TimerStateCallback( MakeRitualCallback ), null );
			}
		}

		public override void MakeRitualCallback( object state )
		{
			Midgard2PlayerMobile from = (Midgard2PlayerMobile)Ritualist;

			Type[] types;
			int[] quantities;
			Definition.GetReqTypesQuantArray( out types, out quantities );

			int level = from.ClassState.GetLevel( Definition );

			for( int i = 0; i < quantities.Length; i++ )
				quantities[ i ] *= level + 1;

			if( Ritualist.Backpack.ConsumeTotal( types, quantities ) != -1 )
			{
				from.SendMessage( from.Language == "ITA" ? "Il rituale è fallito per la mancanza di qualche oggetto." : "Your ritual failed because some item is lacking." );
			}
			else if( from.ClassState.IncreasePowerLevel( Definition ) )
			{
				int newLevel = from.ClassState.GetLevel( Definition );
				from.SendMessage( from.Language == "ITA" ? "Il potere *{0}* è migliorato. Ora è a livello {1}." : "Your power named *{0}* has increased. It's level is now {1}.", Definition.Name, newLevel );
			}
			else
				from.SendMessage( from.Language == "ITA" ? "Il rituale è fallito." : "Your ritual failed." );

			EndRitual( from );
		}

		public override void DoEffects( Mobile from, bool success )
		{
			from.FixedParticles( 0x376A, 9, 32, 5007, EffectLayer.Waist );
			from.PlaySound( 0x210 );
		}

		public override void EndRitual( Mobile from )
		{
			from.EndAction( typeof( Ritual ) );
		}

		#region Nested type: InternalTarget
		private class InternalTarget : Target
		{
			private DruidRitual m_Owner;

			public InternalTarget( DruidRitual owner ) : base( 1, false, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if( o is Item )
					m_Owner.Target( (Item)o );
			}
		}
		#endregion
	}
}