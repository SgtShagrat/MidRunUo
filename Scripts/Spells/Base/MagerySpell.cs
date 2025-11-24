using System;

using Midgard.Engines.Classes;
using Midgard.Engines.SpellSystem;

using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Spells
{
	public abstract class MagerySpell : Spell
	{
        public static readonly bool OldSchoolRules = true;

		public MagerySpell( Mobile caster, Item scroll, SpellInfo info )
			: base( caster, scroll, info )
		{
		}

		public abstract SpellCircle Circle { get; }

		public override bool ConsumeReagents()
		{
			if( base.ConsumeReagents() )
				return true;

			if( ArcaneGem.ConsumeCharges( Caster, (Core.SE ? 1 : 1 + (int)Circle) ) )
				return true;

			return false;
		}

		private const double ChanceOffset = 22.0, ChanceLength = 100.0 / 8.0;

		private const double ChanceOffsetAfterSixth = 20.9;

		private static readonly int[] m_SecondAgeOffset = new int[] { 20, 15, 10, 5, 0, 0, 0, 0 };

		public override void GetCastSkills( out double min, out double max )
		{
			int circle = (int)Circle;

			if( Scroll != null )
				circle -= 2;

			double avg = ChanceLength * circle;

			#region mod by Dies Irae
			avg += m_SecondAgeOffset[ (int)Circle ];

			if( ClassSystem.IsDruid( Caster ) )
			{
				avg += DruidSystem.GetMageryMalus( Caster );
				if( Caster.AccessLevel == AccessLevel.Developer )
					Caster.SendMessage( "Debug: MageryMalus: " + DruidSystem.GetMageryMalus( Caster ) );

				//if( this is IElementalSummon )
				//{
				// ther shoud be a special bonus for elemental spells?
				//}
			}

			double offset = Circle >= SpellCircle.Sixth ? ChanceOffsetAfterSixth : ChanceOffset;
          
			if( !Core.AOS )
			{
				min = avg - offset;
				max = avg + offset;
				return;
			}
			#endregion

			min = avg - ChanceOffset;
			max = avg + ChanceOffset;
		}

		private static int[] m_ManaTable = new int[] { 4, 6, 9, 11, 14, 20, 40, 50 };

		public override int GetMana()
		{
			if( Scroll is BaseWand )
				return 0;

			return m_ManaTable[(int)Circle];
		}

		private const int ResistOffsetA = 16;
		private const int ResistOffsetB = 25;

		public override double GetResistSkill( Mobile m )
		{
			int maxSkill = ( 1 + (int)Circle ) * ResistOffsetA;
			maxSkill += ( 1 + ( (int)Circle / 6 ) ) * ResistOffsetB;

			if( m.Skills[SkillName.MagicResist].Value < maxSkill )
				m.CheckSkill( SkillName.MagicResist, 0.0, 120 );

			return m.Skills[SkillName.MagicResist].Value;
		}

		public virtual bool CheckResisted( Mobile target )
		{
			double n = GetResistPercent( target );

			#region mod by Dies Irae
			bool isParalyzeSpell = this is Fifth.ParalyzeSpell;

			TransformContext context = TransformationSpellHelper.GetContext( target );
			if( context != null && context.Spell is ReaperFormSpell )
				return true;

			if( isParalyzeSpell || GetInscribeSkill( Caster ) == 100.0 )
				n -= 10;
			#endregion

			n /= 100.0;

			if( n < 0 )
				n = 0;

			if( n > 1.0 )
				n = 1.0;

			int maxSkill = ( 1 + (int)Circle ) * ResistOffsetA;
			maxSkill += ( 1 + ( (int)Circle / 6 ) ) * ResistOffsetB;

			bool isCreatureVsPlayer = Caster != null && Caster is BaseCreature && target is PlayerMobile;
			double rawPercent = (isCreatureVsPlayer ? 0 : (7- (double)Circle)/10);


			if( rawPercent < 0 )
				rawPercent = 0;

			if( rawPercent > 1.0 )
				rawPercent = 1.0;

			bool success = n >= Utility.RandomDouble();

			if( target.Skills[SkillName.MagicResist].Value < maxSkill )
				target.CheckSkill( SkillName.MagicResist, (success? rawPercent : (rawPercent+1)/2) );

			#region mod by Dies Irae + fix Arlas
			if( success )
			{
				if( Caster != null && Caster != target && Caster.Player )
					Caster.SendMessage( Caster.Language == "ITA" ? "Il tuo nemico ha resistito all'incantesimo!" : "Your enemy resisted the spell!" );

				target.FixedParticles( 0x374A, 10, 15, 5028, EffectLayer.Waist );
				target.PlaySound( 0x1EA );
			}
			#endregion

			if( target.PlayerDebug )
			{
				target.SendMessage( "Debug: CheckResisted. Is ParalyzeSpell: {0}. Inscribe res malus: {1}.",
							this is Fifth.ParalyzeSpell, GetInscribeSkill( Caster ) == 100.0 );
				target.SendMessage( "Debug: GetResistPercent: {0:F3}", n );
				target.SendMessage( "Debug: Max respell {0:F2}", maxSkill );
				target.SendMessage( "Debug: Raw % {0:F2}", 1- (success? rawPercent : (rawPercent+1)/2) );

			}

			return ( success );
		}

		public virtual double GetResistPercentForCircle( Mobile target, SpellCircle circle )
		{
			double res = target.Skills[ SkillName.MagicResist ].Value;
			double mag = Caster.Skills[ CastSkill ].Value;

			double firstPercent = res / 5.0;
			double secondPercent = res - ( ( ( mag - 20.0 ) / 5.0 ) + ( 1 + (int)circle ) * 5.0 );

			/*
			// POL script: mageryByPassing.inc
			var percres:=cint(res-(cint(cdbl((mag-20)/5)) + circle*5));
			if (percres<(res/5))
				percres:=cdbl(res/5);
			endif

			if (NewGetSkill(attacker,SKILLID_INSCRIPTION)=100 or type=MAG_PARALYZE)
				percres:=percres-10;
			endif
			*/

			return (firstPercent > secondPercent ? firstPercent : secondPercent) /* / 2.0 */; // Seems should be about half of what stratics says.
		}

		public virtual double GetResistPercent( Mobile target )
		{
			return GetResistPercentForCircle( target, Circle );
		}

		public override TimeSpan GetCastDelay()
		{
			if( Scroll is BaseWand )
				return TimeSpan.Zero;
            
			// mod by Dies Irae
			// http://web.archive.org/web/20000309041730/uo.stratics.com/spells.htm
			// Spell-delay is equal to 0.5 seconds per Circle of the spell being cast.
			double secondi = GetOldCastDelayBonus() + ( (int)Circle + 1 ) * 0.5 + ( ((int)Circle == 7) ? 2 : 0 );

			if (DoEquippedBlock())
				secondi = GetOldCastDelayBonus() + ( (int)Circle + 1 ) * 0.3 + ( ((int)Circle == 7) ? 1.5 : 0 );

			if( OldSchoolRules )
				return TimeSpan.FromSeconds( secondi );//0.25 + 0.50 + ( 0.25 * ( (int)Circle + 1 ) ) + GetOldCastDelayBonus() );

			if( !Core.AOS )
				return TimeSpan.FromSeconds( 0.5 + (0.25 * (int)Circle) );

			return base.GetCastDelay();
		}

		public override TimeSpan CastDelayBase
		{
			get
			{
				return TimeSpan.FromSeconds( (3 + (int)Circle) * CastDelaySecondsPerTick );
			}
		}

		#region mod by Dies Irae: pre-aos stuff
		public override int GetScrollStrangeEffectChance()
		{
			// var per:=circle*10-NewGetSkill(who,SKILLID_MAGERY);

			int chance = (int)( ( 1 + (int)Circle ) * 10 - Caster.Skills[ CastSkill ].Value );

			if( Caster.PlayerDebug )
			{
				Caster.SendMessage( "Debug: strangeEffectChance: {0} (will be 100%)", chance );
				chance = 100;
			}

			return chance;
		}

		public override void SayMantra()
		{
			if( Scroll != null && Scroll is BaseWand )
				return;

			if( Info == null )
				return;

			if( string.IsNullOrEmpty( Info.Mantra ) )
				return;

			if( Caster == null )
				return;

			if( ( ( Caster is BaseCreature && ( (BaseCreature)Caster ).CanSpeakMantra ) || Caster.Player ) )
			{
				if( Caster.Map != null )
				{
					IPooledEnumerable eable = Caster.Map.GetClientsInRange( Caster.Location );

					foreach( NetState state in eable )
					{
						if( state == null )
							continue;

						if( state.Mobile == Caster || !state.Mobile.CanSee( Caster ) )
							continue;

						Caster.PrivateOverheadMessage( MessageType.Spell, Caster.SpeechHue, true, PerCheck( state.Mobile.Int ) ? Info.Mantra : "* You hear strange power words! *", state );
					}

					eable.Free();
				}

				Caster.LocalOverheadMessage( MessageType.Spell, Caster.SpeechHue, true, Info.Mantra );
			}
		}

		public int GetBaseDamage()
		{
			switch( Circle )
			{
				case SpellCircle.First:
					return Midgard.DiceRoll.Roll( "1d3+3" ); // 1d3+3 = 4-6
				case SpellCircle.Second:
					return Midgard.DiceRoll.Roll( "1d8+4" ); // 1d8+4 = 5-12
				case SpellCircle.Third:
					return Midgard.DiceRoll.Roll( "4d4+4" ); // 4d4+4 = 8-20
				case SpellCircle.Fourth:
					return Midgard.DiceRoll.Roll( "3d8+5" ); // 3d8+5 = 8-29
				case SpellCircle.Fifth:
					return Midgard.DiceRoll.Roll( "5d8+6" ); // 5d8+6 = 11-46
				case SpellCircle.Sixth:
					return Midgard.DiceRoll.Roll( "6d8+6" ); // 6d8+6 = 12-54
				case SpellCircle.Seventh:
					return Midgard.DiceRoll.Roll( "7d8+10" ); // 7d8+10 = 17-66
				case SpellCircle.Eighth:
					return Midgard.DiceRoll.Roll( "8d8+10" ); // 8d8+10 = 18-66
				default:
					return 0;
			}
		}

		public override int GetFatigueBySpellDamage( Mobile target, int damage )
		{
		    return 0; // Utility.Dice( 1, (int)Circle, 1 );
		}

		public override bool DoEquipBlocksSpellCircle( bool message )
		{
			foreach( Item obj in Caster.Items )
			{
				if( obj is BaseWand )
					continue;

				if( obj.BlockCircle != -1 && (int)Circle + 1 >= obj.BlockCircle )
				{
					Caster.SendMessage( Caster.Language == "ITA" ? "Il tuo vestiario non ti permette di lanciare questo incantesimo!" : "Your equipment prevents you from casting this spell!" );
					return true;
				}
			}

			return false;
		}

		public bool DoEquippedBlock()
		{
			foreach( Item obj in Caster.Items )
			{
				if( obj is BaseWand )
					continue;

				if( obj.BlockCircle != -1 )
					return true;
			}

			return false;
		}

		/*
		public override bool CheckDisturb( DisturbType type, bool firstCircle, bool resistable )
		{
			if( Core.AOS || !resistable )
			{
				return base.CheckDisturb( type, firstCircle, resistable );
			}
			else
			{
				// chance is between 0.1 and 0.8 minus 0.0 to 0.5
				double chance = ( (int)Circle / 10.0 ) - ( Caster.Skills[ CastSkill ].Value / 200.0 );

				// return true means disrupt resisted
				bool resisted = Utility.RandomDouble() < chance;

				if( Core.Debug && Caster.Player )
				{
					Caster.SendMessage( "Debug: chance to resist disruption: {0}", chance.ToString( "F3" ) );
					if( resisted )
						Caster.SendMessage( "Debug: You have not been disrupted." );
				}

				return resisted;
			}
		}
		*/
		//if (!UnModCheckSkill(who,SKILLID_WRESTLING,50+(circle*10)));
		//SendSysmessage(who,"You have lost your concentration!");
        /*function UnModCheckSkill(byref who,skillid,diff)
	        var skill:=NewGetSkill(who,cint(skillid));
	        if (diff=-1)
		        return percheck(skill);
	        endif
	        var mini:=diff-20;
	        var maxi:=diff+20;
	        var per:=10;
	        if (skill>=maxi)
		        per:=990;
	        else
		        per:=25*(skill-mini);
	        endif
	        if (randomint(1000)+1<=per)
		        return 1;
	        else
		        return 0;
	        endif
        endfunction*/
	    public override void OnCasterDamaged( Mobile from, int damage )
		{
			double chance;
			//Old fizz:
			if(OldSchoolRules)
			{
				double skill = Caster.Skills[ SkillName.Wrestling ].Value;
				int diff = 40 + damage + ((int)Circle + 1)*10;//50 + ((int)Circle + 1)*10;
				chance = (skill >= diff + 20 ? 0.99 : 0.025*(skill-diff+20) );
				//Caster.SendMessage( "Debug");
				//Caster.SendMessage( chance.ToString() );
			}
			else
			{
				double difficulty = ( (double)Circle * 100.0 ) / 7.0;
				double skill = Caster.Skills[ CastSkill ].Value;
				//chance = ( skill - ( difficulty + ( damage * 2.0 ) ) ) / 100.0;//edit, proviamo a togliere il *2.0
				chance = ( skill - ( difficulty + damage ) ) / 100.0;

				if( Caster is BaseCreature && !((BaseCreature)Caster).Controlled )
					chance /= 2.0;
			}

			if( Caster is BaseCreature && ((BaseCreature)Caster).Controlled && from is BaseCreature)
				chance += 0.80;
			//Caster.Say("chance {0}", chance);

			// return true means disrupt resisted
			bool resist = Utility.RandomDouble() < chance;

			TransformContext context = TransformationSpellHelper.GetContext( Caster );
			if( context != null && context.Spell is ReaperFormSpell )
				resist = true;

			if( Caster.PlayerDebug )
				Caster.SendMessage( "Debug MagerySpell: chance to resist disruption: {0}% - successed: {1}", chance * 100, resist );

			if( !resist )
				OnCasterHurt();
		}
		#endregion
	}
}
