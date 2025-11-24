using System;

using Midgard.Engines.SpellSystem;

using Server.Mobiles;
using Server.Targeting;
using Server.Network;

namespace Server.Spells.Fifth
{
	public class MindBlastSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Mind Blast", "Por Corp Wis",
				218,
				Core.AOS ? 9002 : 9032,
				Reagent.BlackPearl,
				Reagent.MandrakeRoot,
				Reagent.Nightshade,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Fifth; } }

		public MindBlastSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
			if ( Core.AOS )
				m_Info.LeftHandEffect = m_Info.RightHandEffect = 9002;
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		private void AosDelay_Callback( object state )
		{
			object[] states = (object[])state;
			Mobile caster = (Mobile)states[0];
			Mobile target = (Mobile)states[1];
			Mobile defender = (Mobile)states[2];
			int damage = (int)states[3];

			if ( caster.HarmfulCheck( defender ) )
			{
				SpellHelper.Damage( this, target, Utility.RandomMinMax( damage, damage + 4 ), 0, 0, 100, 0, 0 );

				target.FixedParticles( 0x374A, 10, 15, 5038, 1181, 2, EffectLayer.Head );
				target.PlaySound( 0x213 );
			}
		}

		public override bool DelayedDamage{ get{ return !Core.AOS; } }

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( Core.AOS )
			{
				if ( Caster.CanBeHarmful( m ) && CheckSequence() )
				{
					Mobile from = Caster, target = m;

					SpellHelper.Turn( from, target );

					SpellHelper.CheckReflect( (int)this.Circle, ref from, ref target );

					int damage = (int)((Caster.Skills[SkillName.Magery].Value + Caster.Int) / 5);

					if ( damage > 60 )
						damage = 60;

					Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ),
						new TimerStateCallback( AosDelay_Callback ),
						new object[]{ Caster, target, m, damage } );
				}
			}
			else if ( CheckHSequence( m ) )
			{
				Mobile from = Caster, target = m;

				SpellHelper.Turn( from, target );

				#region midgard POL algorithm
				// var my_int := cint(caster.intelligence);
				// var his_int := cint(cast_on.intelligence);
				int casterInt = Caster.Int;
				int targetInt = m.Int;

				/*
				if (his_int = my_int)
					SendSysMessage(caster, "You are of equal intellect!");
					return;
				endif
				*/

				if( targetInt == casterInt )
				{
					Caster.SendMessage( Caster.Language == "ITA" ? "Avete lo stesso quoziente intellettivo!" : "You are of equal intellect!" );
					return;
				}
				#endregion

				SpellHelper.CheckReflect( (int)this.Circle, ref from, ref target );

				#region midgard POL algorithm
				// var intdmg:=cint((my_int-his_int)/2);
				// intdmg:=intdmg+RandomInt(10);
				int damage = Math.Abs( ( casterInt - targetInt ) / 2 ) + 5 + Utility.Random( 10 );
				if( damage < 1 )
					damage = 1;

				/*
				if (his_int > my_int)
					cast_on := caster;
					SendSysMessage(caster, "Their superior intellect reflects the spell!");
					intdmg:=cint((his_int-my_int)/2);
				else
				*/

				if( targetInt > casterInt )
				{
					target = Caster;
					Caster.SendMessage( Caster.Language == "ITA" ? "Il suo intelletto superiore riflette l'incantesimo!" : "Their superior intellect reflects the spell!" );
				}

                /*
	            if (cast_on!=caster)
		            if (Reflected(cast_on))
			            if ( cast_on.serial = caster.serial )
				            return;
			            endif
			            cast_on := caster;
			            if (GetObjProperty( caster, "mr") = "1")
				            EraseObjProperty(caster, "mr");
				            SendSysMessage(caster, "Your magic reflect spell saved you!");
				            return;
			            endif
		            endif
	            endif
                */

				if( target == Caster && Caster.MagicDamageAbsorb > 0 )
				{
					Caster.MagicDamageAbsorb = 0;
					Caster.SendMessage( Caster.Language == "ITA" ? "L'incantesimo viene riflesso e sei salvo!" : "Your magic reflect spell saved you!" );
					return;
				}

				if ( damage > 80 )//cap alzato da 60 a 80
					damage = 80;

				if ( CheckResisted( target ) )
				{
					damage = (int)(damage*( 1.0 - GetResistScalar( target ) ));// /= 2;
					target.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
				}
			    #endregion

                // Algorithm: (highestStat - lowestStat) / 2 [- 50% if resisted]
                /*
				int highestStat = target.Str, lowestStat = target.Str;

				if ( target.Dex > highestStat )
					highestStat = target.Dex;

				if ( target.Dex < lowestStat )
					lowestStat = target.Dex;

				if ( target.Int > highestStat )
					highestStat = target.Int;

				if ( target.Int < lowestStat )
					lowestStat = target.Int;

				if ( highestStat > 150 )
					highestStat = 150;

				if ( lowestStat > 150 ) 
					lowestStat = 150;

				int damage = (highestStat - lowestStat) / 4;//less damage

				if ( damage > 45 )
					damage = 45;

				if ( CheckResisted( target ) )
				{
					damage /= 2;
					target.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
				}
                */

				from.FixedParticles( 0x374A, 10, 15, 2038, EffectLayer.Head );

				target.FixedParticles( 0x374A, 10, 15, 5038, EffectLayer.Head );
				target.PlaySound( 0x213 );

				#region mod by Dies Irae
				if( Core.AOS )
					SpellHelper.Damage( this, target, damage, 0, 0, 100, 0, 0 );
				else
					MidgardSpellHelper.Damage( this, target, damage, SpellType.Mental );
				#endregion
			}

			FinishSequence();
		}

		public override double GetSlayerDamageScalar( Mobile target )
		{
			return 1.0; //This spell isn't affected by slayer spellbooks
		}

		private class InternalTarget : Target
		{
			private MindBlastSpell m_Owner;

			public InternalTarget( MindBlastSpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
					m_Owner.Target( (Mobile)o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}