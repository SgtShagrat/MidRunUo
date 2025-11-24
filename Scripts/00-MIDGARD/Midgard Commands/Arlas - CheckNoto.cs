using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Targeting;

namespace Midgard.Commands
{
	public class CheckNoto
	{
		public static void Initialize()
		{
			CommandSystem.Register( "CheckNoto", AccessLevel.GameMaster, new CommandEventHandler( CheckNoto_OnCommand ) );
		}

		[Usage( "CheckNoto" )]
		[Description( "Tells you what notoriety exist between targets" )]
		private static void CheckNoto_OnCommand( CommandEventArgs e )
		{
			e.Mobile.SendMessage( "Target the first mobile." );
			e.Mobile.Target = new SelectAggressorTarget();
		}

		private static void EndSelect( Mobile from, Mobile aggressor )
		{
			from.SendMessage( "Target the second mobile." );
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

				if (targ is Mobile)
				{
					int noto = Notoriety.Compute( m_Aggressor, (Mobile)targ );
					if ( noto == 1 )
						from.SendMessage( "Innocent = 1" ) ;
					else if ( noto == 2 )
						from.SendMessage( "Ally = 2" ) ;
					else if ( noto == 3 )
						from.SendMessage( "CanBeAttacked = 3" ) ;
					else if ( noto == 4 )
						from.SendMessage( "Criminal = 4" ) ;
					else if ( noto == 5 )
						from.SendMessage( "Enemy = 5" ) ;
					else if ( noto == 6 )
						from.SendMessage( "Murderer = 6" ) ;
					else if ( noto == 7 )
						from.SendMessage( "Invulnerable = 7" ) ;
				}
				else
					from.SendMessage( "not a Mobile." );
			}
		}
	}
}