/***************************************************************************
 *								  RPGNecromancerSpell.cs
 *									----------------------
 *  begin					: Gennaio, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright				: Midgard Uo Shard - Matteo Visintin
 *  email					: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info
 * 
 ***************************************************************************/

using System;
using System.Text;

using Midgard.Engines.Classes;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
	public abstract class RPGNecromancerSpell : Spell, ICustomSpell
	{
		public RPGNecromancerSpell( Mobile caster, Item scroll, SpellInfo info ) : base( caster, scroll, info )
		{
		}

		public abstract int RequiredMana { get; }

		public abstract double DelayOfReuse { get; }

		public override SkillName CastSkill{get { return SkillName.Necromancy; }}
		public override SkillName DamageSkill{get { return SkillName.SpiritSpeak; }}

		public override bool ClearHandsOnCast{get { return false; }}

		public virtual SpellCircle Circle{get { return SpellCircle.First; }}

		public override bool DoEquipBlocksSpellCircle( bool message )
		{
			foreach( Item obj in Caster.Items )
			{
				if( obj is BaseWand )
					continue;

				if( obj.BlockCircle != -1 && (int)Circle + 1 >= obj.BlockCircle )
				{
					Caster.SendMessage( (Caster.Language == "ITA" ? "Il tuo vestiario non ti permette di lanciare questo incantesimo!" : "Your equipment prevents you from casting this spell!") );
					return true;
				}
			}

			return false;
		}

		#region ICustomSpell Members
		public abstract ExtendedSpellInfo ExtendedInfo { get; }
		public abstract double RequiredSkill { get; }

		public SchoolFlag SpellSchool
		{
			get { return SchoolFlag.Necromancer; }
		}
		#endregion

		public string GetInfo()
		{
			ExtendedSpellInfo ei = ExtendedInfo;

			StringBuilder stringBuilder = new StringBuilder();

			stringBuilder.AppendLine( Name );
			stringBuilder.AppendLine( string.Format( "ID: {0}", SpellRegistry.GetRegistryNumber( this ) ) );
			stringBuilder.AppendLine( string.Format( "Mantra: {0}", Mantra ) );
			stringBuilder.AppendLine( string.Format( "RequiredMana: {0}", GetMana() ) );
			stringBuilder.AppendLine( string.Format( "RequiredSkill: {0}", RequiredSkill ) );
			stringBuilder.AppendLine( string.Format( "BlocksMovement: {0}", BlocksMovement ) );
			stringBuilder.AppendLine( string.Format( "Description: {0}", ei.Description ) );
			stringBuilder.AppendLine( string.Format( "DescriptionIta: {0}", ei.DescriptionIta ) );
			stringBuilder.AppendLine( string.Format( "SpellIcon: {0}", ei.SpellIcon ) );
			stringBuilder.AppendLine( string.Format( "Reagents: {0}", ei.Reagents ) );

			stringBuilder.AppendLine( "" );

			return stringBuilder.ToString();
		}

		public override bool CheckCast()
		{
			if( !base.CheckCast() )
				return false;

			if( Caster.Skills[ CastSkill ].Value < RequiredSkill )
			{
				Caster.SendMessage( (Caster.Language == "ITA" ? "Devi avere almeno " + RequiredSkill + " " + CastSkill + " per invocare questo potere." : "You must have at least " + RequiredSkill + " " + CastSkill + " to invoke this power.") );
				return false;
			}
			else if( Caster.Mana < RequiredMana )
			{
				Caster.SendMessage( (Caster.Language == "ITA" ? "Devi avere almeno " + RequiredMana + " Mana per invocare questo potere." : "You must have at least " + RequiredMana + " Mana to invoke this power.") );
				return false;
			}
			return true;
		}

		public override void GetCastSkills( out double min, out double max )
		{
			int offset = 5 * ( GetPowerLevel() - 1 );

			min = offset + RequiredSkill - 25.0;
			max = offset + RequiredSkill + 25.0;
		}

		public override int GetMana()
		{
			return RequiredMana;
		}

		public int GetPowerLevel()
		{
			ClassPlayerState playerState = ClassPlayerState.Find( Caster );
			if( playerState != null )
				return playerState.GetLevel( GetType() );
			else
				return 0;
		}

		public int ComputePowerValue( int div )
		{
			return ComputePowerValue( Caster, div );
		}

		public static int ComputePowerValue( Mobile from, int div )
		{
			if( from == null )
				return 0;

			int karmaFixed = Math.Min( from.Karma, 0 ) * ( -1 );
			int v = (int)Math.Sqrt( karmaFixed + 20000 + ( from.Skills.Necromancy.Fixed * 10 ) );

			return v / div;
		}

		public TimeSpan GetDelayOfReuse()
		{
			return TimeSpan.FromSeconds( DelayOfReuse );
		}

		public static bool IsSuperVulnerable( Mobile m )
		{
			return (ClassSystem.IsPaladine( m ) || m.Karma > 5000);
		}

		public static bool IsImmune( Mobile m )
		{
			return ( ClassSystem.IsNecromancer( m ) || ClassSystem.IsUndead( m ) );
		}

		public virtual double GetResistPercent( Mobile target )
		{
			double res = target.Skills[ SkillName.MagicResist ].Value / 2 - (target.Karma / 300);
			double mag = Caster.Skills[ SkillName.Necromancy ].Value;

			double firstPercent = res / 5.0;
			double secondPercent = res - ( ( ( mag - 20.0 ) / 5.0 ) + ( GetPowerLevel() ) * 5.0 );


			return (firstPercent > secondPercent ? firstPercent : secondPercent) ;
		}

		public virtual bool CheckResisted( Mobile target )
		{
			double n = GetResistPercent( target );

			n /= 100.0;

			if( n < 0 )
				n = 0;

			if( n > 1.0 )
				n = 1.0;

			bool isCreatureVsPlayer = Caster != null && Caster is BaseCreature && target is PlayerMobile;
			double rawPercent = (isCreatureVsPlayer ? 0 : (7 - GetPowerLevel())/10);

			if( rawPercent < 0 )
				rawPercent = 0;

			if( rawPercent > 1.0 )
				rawPercent = 1.0;

			bool success = n >= Utility.RandomDouble();

			target.CheckSkill( SkillName.MagicResist, (success? rawPercent : (rawPercent+1)/2) );

			#region mod by Dies Irae + fix Arlas
			if( success )
			{
				if( Caster != null && Caster != target && Caster.Player )
					Caster.SendMessage( Caster.Language == "ITA" ? "Il tuo nemico resiste all'incantesimo!" : "Your enemy resisted the spell!" );

				target.FixedParticles( 0x374A, 10, 15, 5028, EffectLayer.Waist );
				target.PlaySound( 0x1EA );
			}
			#endregion

			if( Caster.PlayerDebug )
			{
				Caster.SendMessage( "Debug: GetResistPercent: {0:F3}", n );
				Caster.SendMessage( "Debug: Raw % {0:F2}", 1- (success? rawPercent : (rawPercent+1)/2) );

			}
			return ( success );
		}

		public override void OnCasterDamaged( Mobile from, int damage )
		{
			double chance;
			//Old fizz:
				double skill = Caster.Skills[ SkillName.Necromancy ].Value;
				int diff = 50 + damage;
				chance = (skill >= diff + 20 ? 0.99 : 0.025*(skill-diff+20) );
			//	Caster.SendMessage( "Debug");
			//	Caster.SendMessage( chance.ToString() );

			// return true means disrupt resisted
			bool resist = Utility.RandomDouble() < chance;

			if( !resist )
				OnCasterHurt();
		}

	}
}