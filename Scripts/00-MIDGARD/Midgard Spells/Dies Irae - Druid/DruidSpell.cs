/***************************************************************************
 *								  DruidSpell.cs
 *									------------------------------
 *  begin					: Dicembre, 2006
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright				: Midgard Uo Shard - Matteo Visintin
 *  email					: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Text;

using Midgard.Engines.Classes;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells;

namespace Midgard.Engines.SpellSystem
{
	public interface IElementalSummon
	{
	}

	public abstract class DruidSpell : Spell, ICustomSpell
	{
		public static int MessageHue = 0x22;

		protected DruidSpell( Mobile caster, Item scroll, SpellInfo info ) : base( caster, scroll, info )
		{
		}

		private static readonly int[] m_ManaTable = new int[] { 5, 10, 15, 20, 25, 30, 40, 50 };

		private int m_CastTimeFocusLevel;

		public abstract SpellCircle Circle { get; }

		public override SkillName CastSkill{get { return SkillName.Spellweaving; }}
		public override SkillName DamageSkill{get { return SkillName.AnimalLore; }}
		public override bool ClearHandsOnCast{get { return false; }}
		public override int CastRecoveryBase{get { return 4; }}

		#region ICustomSpell Members
		public abstract ExtendedSpellInfo ExtendedInfo { get; }
		public SchoolFlag SpellSchool{get { return SchoolFlag.Druid; }}
		public virtual double RequiredSkill{get { return ( 5 + 13 * ( (int)Circle - 1 ) ); }}
		#endregion

		public override TimeSpan CastDelayBase
		{
			get
			{
				double delay = ( 3 + (int)(Circle)/2 ) * CastDelaySecondsPerTick;

				if( IsEligibleForCastBonus )
					delay -= CastDelayBonus;

				if( delay < 0.5 )
					delay = 0.5;

				return TimeSpan.FromSeconds( delay );
			}
		}

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

		#region focus
		public virtual int FocusLevel
		{
			get { return m_CastTimeFocusLevel; }
		}

		public static int GetFocusLevel( Mobile from )
		{
			DruidFocus focus = FindDruidFocus( from );

			if( focus == null || focus.Deleted )
				return 0;

			return focus.StrengthBonus + DruidEmpowermentSpell.GetGenericBonus( from );
		}

		public static DruidFocus FindDruidFocus( Mobile from )
		{
			if( from == null || from.Backpack == null )
				return null;

			return from.Backpack.FindItemByType<DruidFocus>();
		}
		#endregion

		#region spell info
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

		public string GetStaffInfo()
		{
			ExtendedSpellInfo ei = ExtendedInfo;

			StringBuilder stringBuilder = new StringBuilder();

			stringBuilder.AppendLine( string.Format( "[b]{0}[/b]", Name ) );
			stringBuilder.AppendLine( string.Format( "[i]Mantra / Faith:[/i] {0} / {1}", Mantra, RequiredSkill ) );
			stringBuilder.AppendLine( string.Format( "[i]Mana:[/i] {0}", GetMana() ) );
			stringBuilder.AppendLine( string.Format( "[i]Cast delay:[/i] {0}", CastDelayBase.TotalSeconds ) );
			stringBuilder.AppendLine( string.Format( "[i]Blocks Movement / Clear hands:[/i] {0} / {1}", BlocksMovement, ClearHandsOnCast ) );
			stringBuilder.AppendLine( string.Format( "[i]Description:[/i] {0}", ei.DescriptionStaff ?? "" ) );
			stringBuilder.AppendLine( "" );

			return stringBuilder.ToString();
		}

		public string GetCommaSeparatedInfo()
		{
			ExtendedSpellInfo ei = ExtendedInfo;

			return string.Format( "{0}, {1}, {2}, {3}, {4}, {5}, \"{6}\", \"{7}\", {8}",
								  Name, Mantra, Circle, RequiredSkill, GetMana(), CastDelayBase.TotalSeconds, BlocksMovement, ClearHandsOnCast, ei.DescriptionStaff ?? "" );
		}
		#endregion

		#region resist
		public SkillName ResistSkill
		{
			get {
			if ( DruidEmpowermentSpell.HasBonus( Caster ) )
				return SkillName.AnimalLore;

			return SkillName.MagicResist;
			}
		}

		public virtual double GetResistPercent( Mobile target )
		{
			return GetResistPercentForLevel( target, GetPowerLevel() );
		}

		public double GetResistPercentForLevel( Mobile target, int level )
		{
			double res = target.Skills[ ResistSkill ].Value;
			double chi = Caster.Skills[ CastSkill ].Value;

			double firstPercent = res / 5.0;
			double secondPercent = res - ( ( ( chi - 20.0 ) / 5.0 ) + ( 1 + level ) * 5.0 );

			return ( firstPercent > secondPercent ? firstPercent : secondPercent );
		}

		private const int ResistOffsetA = 16;
		private const int ResistOffsetB = 25;

		public bool CheckResisted( Mobile target )
		{
			TransformContext context = TransformationSpellHelper.GetContext( target );
			if( context != null && context.Spell is ReaperFormSpell )
				return true;

			double n = GetResistPercent( target );

			n /= 100.0;

			if( n < 0 )
				n = 0;

			if( n > 1.0 )
				n = 1.0;

			int level = GetPowerLevel();

			int maxSkill = ( 1 + level ) * ResistOffsetA;
			maxSkill += ( 1 + ( level / 6 ) ) * ResistOffsetB;

			bool isCreatureVsPlayer = Caster != null && Caster is BaseCreature && target is PlayerMobile;
			double rawPercent = (isCreatureVsPlayer ? 0 : (7- (double)Circle)/10);

			if( rawPercent < 0 )
				rawPercent = 0;

			if( rawPercent > 1.0 )
				rawPercent = 1.0;

			bool success = n >= Utility.RandomDouble();

			if( target.Skills[ResistSkill].Value < maxSkill )
				target.CheckSkill( ResistSkill, (success? rawPercent : (rawPercent+1)/2) );

			if( success )
			{
				if( Caster != target && Caster != null && Caster.Player )
					Caster.SendMessage( Caster.Language == "ITA" ? "Il nemico resiste alla tua magia!" : "Your enemy resisted the spell!" );

				target.FixedParticles( 0x374A, 10, 15, 5028, EffectLayer.Waist );
				target.PlaySound( 0x1EA );
			}

			return success;
		}

		public override double GetResistScalar( Mobile target )
		{
			double resSkill = target.Skills[ ResistSkill ].Value;
			double castSkill = Caster.Skills[ CastSkill ].Value;

			// resValue: from -50 to 100
			double resValue = 0.5 * ( 2 * castSkill - resSkill );

			// perc: from 150 to 0
			double perc = 100.0 - resValue;

			perc = perc / 100.0;

			// sanity
			if( perc > 1.0 )
				perc = 1.0;
			else if( perc < 0.0 )
				perc = 0.0;

			if( Caster.PlayerDebug )
				Caster.SendMessage( "Debug GetResistScalar: resist {0:F2} - cast {1:F2} - final scalar {2:F2}", resSkill, castSkill, perc );

			return perc;
		}
		#endregion

		#region cast bonus
		public bool IsEligibleForCastBonus
		{
			get
			{
				if( Caster != null )
					return Caster.Skills[ SkillName.Spellweaving ].Value >= 90.0;

				return false;
			}
		}

		public virtual double CastDelayBonus{get { return 0.1 * GetPowerLevel(); }}
		public virtual int CastManaBonus{get { return 2 * GetPowerLevel(); }}
		#endregion

		public override bool ConsumeReagents()
		{
			if( base.ConsumeReagents() )
				return true;

			if( ArcaneGem.ConsumeCharges( Caster, 1 ) )
				return true;

			return false;
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
			else if( Caster.Mana < GetMana() )
			{
				Caster.SendMessage( (Caster.Language == "ITA" ? "Devi avere almeno " + GetMana() + " Mana per invocare questo potere." : "Thou must have at least " + GetMana() + " Mana to invoke this power.") );
				return false;
			}

			return true;
		}

		public override bool CheckFizzle()
		{
			int mana = ScaleMana( GetMana() );
			double minSkill, maxSkill;

			GetCastSkills( out minSkill, out maxSkill );

			if( Caster.Skills[ CastSkill ].Value < RequiredSkill )
			{
				Caster.SendMessage( (Caster.Language == "ITA" ? "Devi avere almeno " + RequiredSkill + " " + CastSkill + " per invocare questo potere." : "You must have at least " + RequiredSkill + " " + CastSkill + " to invoke this power.") );
				return false;
			}
			else if( Caster.Mana < mana )
			{
				Caster.SendMessage( (Caster.Language == "ITA" ? "Devi avere almeno " + mana + " Mana per invocare questo potere." : "You must have at least " + mana + " Mana to invoke this power.") );
				return false;
			}

			return base.CheckFizzle();
		}

		public virtual int DelayOfReuse{get { return 5; }}

		public override void DoFizzle()
		{
			Caster.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1065502 ); // You fail to invoke the Nature Power
			Caster.PlaySound( 0x1D6 );
			Caster.NextSpellTime = DateTime.Now;
		}

		public override void DoHurtFizzle()
		{
			Caster.FixedEffect( 0x3735, 6, 30 );
			Caster.PlaySound( 0x1D6 );
		}

		public override void GetCastSkills( out double min, out double max )
		{
			int offset = ( GetPowerLevel() - 1 );

			min = offset + RequiredSkill - 25.0;
			max = offset + RequiredSkill + 25.0;
		}

		public TimeSpan GetDelayOfReuse(){return TimeSpan.FromSeconds( DelayOfReuse );}

		public virtual int GetBaseMana(){return m_ManaTable[ (int)Circle ];}

		public override int GetMana()
		{
			int mana = GetBaseMana();

			if( IsEligibleForCastBonus )
				mana -= CastManaBonus;

			if( mana < 1 )
				mana = 1;

			return mana;
		}

		public int GetPowerLevel()
		{
			ClassPlayerState playerState = ClassPlayerState.Find( Caster );
			return playerState != null ? playerState.GetLevel( GetType() ) : 0;
		}

		public static int GetPowerLevelByType( Mobile caster, Type t )
		{
			ClassPlayerState playerState = ClassPlayerState.Find( caster );
			return playerState != null ? playerState.GetLevel( t ) : 0;
		}

		public override void OnCasterDamaged( Mobile from, int damage )
		{
			double difficulty = ( (double)Circle * 100.0 ) / 7.0;
			double skill = Caster.Skills[ CastSkill ].Value;
			double chance = ( skill - ( difficulty + ( damage * 2.0 ) ) ) / 100.0;

			// return true means disrupt resisted
			bool resist = Utility.RandomDouble() < chance;

			TransformContext context = TransformationSpellHelper.GetContext( Caster );
			if( context != null && (context.Spell is ReaperFormSpell || context.Spell is AnimalFormSpell) )
				resist = true;

			if( Caster.PlayerDebug )
				Caster.SendMessage( "Debug DruidSpell: chance to resist disruption: {0}% - successed: {1}", chance * 100, resist );

			if( !resist )
				OnCasterHurt();
		}

		public override void OnDisturb( DisturbType type, bool message )
		{
			base.OnDisturb( type, message );

			if( message )
				Caster.PlaySound( 0x1D6 );
		}

		public override void OnBeginCast()
		{
			base.OnBeginCast();

			SendCastEffect();
			m_CastTimeFocusLevel = (int)( 0.5 * GetFocusLevel( Caster ) ) + GetPowerLevel();
		}

		public override void SayMantra()
		{
		}

		public virtual void SendCastEffect()
		{
			Caster.FixedEffect( 0x37C4, 10, (int)( GetCastDelay().TotalSeconds * 28 ), 4, 3 );
		}

		/*
		private static int GetManaByCircle( int circle )
		{
			return m_ManaTable[ circle ];
		}
		*/

		/*
		private static double GetSkillReqByID( int spellID )
		{
			Spell spell = SpellRegistry.GetSpellByID( spellID );

			if( spell != null )
			{
				double min, max;
				spell.GetCastSkills( out min, out max );

				return min;
			}

			return 0.0;
		}
		*/
	}
}