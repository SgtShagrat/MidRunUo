using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Targeting;

namespace Midgard.Commands
{
	public class CheckAreaSpell
	{
		public static void Initialize()
		{
			CommandSystem.Register( "CheckAreaSpell", AccessLevel.GameMaster, new CommandEventHandler( CheckAreaSpell_OnCommand ) );
		}

		[Usage( "CheckAreaSpell" )]
		[Description( "Tells you if with an area spell that being will attack" )]
		private static void CheckAreaSpell_OnCommand( CommandEventArgs e )
		{
			e.Mobile.SendMessage( "Target the caster mobile." );
			e.Mobile.Target = new SelectAggressorTarget();
		}

		private static void EndSelect( Mobile from, Mobile aggressor )
		{
			from.SendMessage( "Target the mobile who must be aggressed." );
			from.Target = new SelectAggressedTarget( aggressor );
		}

		private class SelectAggressorTarget : Target
		{
			public SelectAggressorTarget() : base( 20, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object targ )
			{
				if ( targ is Mobile )
					EndSelect( from, (Mobile)targ );
				else
					from.SendMessage( "That is not a creature." );
			}
		}

		private class SelectAggressedTarget : Target
		{
			private readonly Mobile m_Aggressor;

			public SelectAggressedTarget( Mobile aggressor ) : base( 20, false, TargetFlags.None )
			{
				m_Aggressor = aggressor;
			}

			protected override void OnTarget( Mobile from, object targ )
			{
				//if ((m_Aggressor is BaseCreature && targ is BaseCreature && ((BaseCreature)m_Aggressor).CanAreaHarmful( (BaseCreature)targ ))
				//|| (m_Aggressor is PlayerMobile && targ is PlayerMobile && ((PlayerMobile)m_Aggressor).CanAreaHarmful( (PlayerMobile)targ ))
				//|| (m_Aggressor is BaseCreature && targ is PlayerMobile && ((BaseCreature)m_Aggressor).CanAreaHarmful( (PlayerMobile)targ ))
				//|| (m_Aggressor is PlayerMobile && targ is BaseCreature && ((PlayerMobile)m_Aggressor).CanAreaHarmful( (BaseCreature)targ )))
				if (targ is Mobile && m_Aggressor.CanAreaHarmful( (Mobile)targ ))
					from.SendMessage( "That creature will attack the other." );
				else
					from.SendMessage( "They will damage each other with no consequenses." );
			}
		}
	}
}