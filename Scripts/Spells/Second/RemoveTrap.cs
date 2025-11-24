using System;
using Server.Targeting;
using Server.Network;
using Server.Items;

namespace Server.Spells.Second
{
	public class RemoveTrapSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Remove Trap", "An Jux",
				212,
				9001,
				Reagent.Bloodmoss,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Second; } }

		public RemoveTrapSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
			Caster.SendMessage( "What do you wish to untrap?" );
		}

		public void Target( TrapableContainer item )
		{
			if ( !Caster.CanSee( item ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( item.TrapType != TrapType.None && item.TrapType != TrapType.MagicTrap )
			{
				base.DoFizzle();
			}
			else if ( CheckSequence() )
			{
				SpellHelper.Turn( Caster, item );

				Point3D loc = item.GetWorldLocation();

				Effects.SendLocationParticles( EffectItem.Create( loc, item.Map, EffectItem.DefaultDuration ), 0x376A, 9, 32, 5015 );
				Effects.PlaySound( loc, item.Map, 0x1F0 );

				item.TrapType = TrapType.None;
				item.TrapPower = 0;
				item.TrapLevel = 0;
			}

			FinishSequence();
		}

        #region mod by Dies Irae
        public void Target( BaseDoor door )
        {
            if( !Caster.CanSee( door ) )
            {
                Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
            }
            else if( door.TrapType != TrapType.None && door.TrapType != TrapType.MagicTrap )
            {
                Caster.SendMessage( "You cannot disarm this powerfull trap with a remove trap spell!" );
                base.DoFizzle();
            }
            else if( CheckSequence() )
            {
                SpellHelper.Turn( Caster, door );

                double difficulty = door.TrapPower * 2;
                if( difficulty < 30.0 )
                    difficulty = 30.0;

                double skill = Caster.Skills[ SkillName.Magery ].Value + 20.0;

                if( skill <= difficulty )
                {
                    Caster.SendMessage( "You cannot disarm this trap!" );
                }
                else if( (int)( skill - difficulty ) > Utility.Random( 120 ) )
                {
                    Caster.SendMessage( "You failed to disarm this trap!" );

                    if( Utility.Dice( 1, 4, 0 ) == 1 )
                        door.ExecuteTrap( Caster );
                }
                else
                {
                    Point3D loc = door.GetWorldLocation();

                    Effects.SendLocationParticles( EffectItem.Create( loc, door.Map, EffectItem.DefaultDuration ), 0x376A, 9, 32, 5015 );
                    Effects.PlaySound( loc, door.Map, 0x1F0 );

                    door.RemoveTrap( Caster, true );

                    Caster.SendLocalizedMessage( 502377 ); // You successfully render the trap harmless
                }
            }

            FinishSequence();
        }
        #endregion

		private class InternalTarget : Target
		{
			private RemoveTrapSpell m_Owner;

			public InternalTarget( RemoveTrapSpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is TrapableContainer )
				{
					m_Owner.Target( (TrapableContainer)o );
				}
                #region mod by Dies Irae
                else if( o is BaseDoor )
                {
                    m_Owner.Target( (BaseDoor)o );
                }
                #endregion
				else
				{
					from.SendMessage( "You can't disarm that" );
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}