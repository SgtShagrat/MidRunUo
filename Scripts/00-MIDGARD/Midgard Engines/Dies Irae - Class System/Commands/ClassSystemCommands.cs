/***************************************************************************
 *							   ClassSystemCommands.cs
 *
 *   revision			 : 03 January, 2010
 *   author			   :	Dies Irae	
 *   email				: tocasia@alice.it
 *   copyright			: (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Midgard.Engines.SpellSystem;

using Server;
using Server.Commands;
using Server.Misc;
using Server.Mobiles;
using Server.Targeting;

namespace Midgard.Engines.Classes
{
    public class ClassSystemCommands
    {
        public static void RegisterCommands()
        {
            CommandSystem.Register( "SetClass", TestCenter.Enabled ? AccessLevel.Player : AccessLevel.Seer, SetClass_OnCommand );
            CommandSystem.Register( "KickClass", TestCenter.Enabled ? AccessLevel.Player : AccessLevel.Seer, KickClass_OnCommand );
            CommandSystem.Register( "RandomPowers", TestCenter.Enabled ? AccessLevel.Player : AccessLevel.Seer, SetRandomPowers_OnCommand );
            CommandSystem.Register( "ClassPowers", TestCenter.Enabled ? AccessLevel.Player : AccessLevel.Seer, SetClassPowers_OnCommand );

            // CommandSystem.Register( "NascondiTitoloDiClasse", AccessLevel.Player, new CommandEventHandler( ToggleClassStatusDisplay_OnCommand ) );
        }

        [Usage( "NascondiTitoloDiClasse" )]
        [Description( "Abilita o disabilita lo status di classe per il giocatore." )]
        public static void ToggleClassStatusDisplay_OnCommand( CommandEventArgs e )
        {
            Midgard2PlayerMobile m = e.Mobile as Midgard2PlayerMobile;
            if( m == null )
                return;

            if( !m.CheckAlive() || ClassSystem.Find( m ) == null )
            {
                m.SendMessage( "Thou cannot use this command." );
                return;
            }

            m.SendMessage( m.DisplayClassStatus ? "You have chosen to hide your class status." : "You have chosen to display your class status." );
            m.DisplayClassStatus = !m.DisplayClassStatus;
        }

        [Usage( "RandomPowers" )]
        [Description( "Set random level powers for a given classed player." )]
        public static void SetRandomPowers_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;

            if( e.Length == 0 )
            {
                ClassPlayerState state = ClassPlayerState.Find( from );
                if( state != null )
                    state.RandomizePowers();
            }
            else
                from.SendMessage( "Command Use: [RandomPowers" );
        }

        [Usage( "ClassPowers" )]
        [Description( "Set level powers for a given classed player." )]
        public static void SetClassPowers_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;

            if( e.Length == 2 )
            {
                ClassPlayerState state = ClassPlayerState.Find( from );

                PowerDefinition spell = state.GetDefByName( e.GetString( 0 ) );
                int level = e.GetInt32( 1 );
                if( spell == null )
                    from.SendMessage( "Error: Spell null" );
                else
                    state.ClassPowers( spell, level );
            }
            else
                from.SendMessage( "Command Use: [ClassPowers Spell Power" );
        }

        [Usage( "SetClass <newClass>" )]
        [Description( "Set Class of Target Player." )]
        public static void SetClass_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;

            if( e.Length == 1 )
            {
                ClassSystem system = ClassSystem.Parse( e.GetString( 0 ) );
                if( system != null )
                    from.Target = new InternalTarget( system, ActionType.Set );
                else
                    from.SendMessage( "That is not a valid Midgard Class (\"Paladin\", \"Druid\", \"Necromancer\", \"Thief\", \"Scout\" )." );
            }
            else
                from.SendMessage( "Command Use: [SetClass <newClass>" );
        }

        [Usage( "KickClass" )]
        [Description( "Remove ClassState from Target Player." )]
        public static void KickClass_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;

            if( e.Length == 0 )
                from.Target = new InternalTarget( null, ActionType.Kick );
            else
                from.SendMessage( "Command Use: [KickClass" );
        }

        private enum ActionType
        {
            Kick,
            Set,
        }

        private class InternalTarget : Target
        {
            private readonly ClassSystem m_Class;
            private readonly ActionType m_Action;

            public InternalTarget( ClassSystem newClass, ActionType action )
                : base( 10, false, TargetFlags.None )
            {
                m_Class = newClass;
                m_Action = action;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( targeted is Midgard2PlayerMobile )
                {
                    Midgard2PlayerMobile m = (Midgard2PlayerMobile)targeted;

                    switch( m_Action )
                    {
                        case ActionType.Kick:
                            DoPlayerReset( m );
                            break;
                        case ActionType.Set:
                            DoSetClass( m, m_Class );
                            break;
                    }
                }
            }
        }

        public static void DoPlayerReset( Mobile mobile )
        {
            if( mobile == null || mobile.Deleted )
                return;

            ClassPlayerState tps = ClassPlayerState.Find( mobile );
            if( tps != null )
                tps.Detach();

            mobile.InvalidateProperties();
            CustomSpellbook.RemoveBooksFor( mobile );

            #region Mod by Magius(CHE): prevents Ghost instances :-)
            foreach( var sys in ClassSystem.ClassSystems )
                sys.ResetSkillsLocks( mobile );
            #endregion
        }

        public static void DoSetClass( Mobile mobile, ClassSystem system )
        {
            if( mobile == null || mobile.Deleted )
                return;

            if( system != null )
            {
                if( system.Candidates.Contains( mobile ) )
                    system.Candidates.Remove( mobile );

                ClassPlayerState tps = ClassPlayerState.Find( mobile );
                if( tps != null )
                    tps.Detach();

                ClassPlayerState state = system.IstantiateState( mobile );
                state.Attach( false );
                state.Invalidate();

                system.SetStartingSkills( mobile );
            }
        }
    }
}