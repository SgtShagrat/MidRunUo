/***************************************************************************
 *                                	 HardLabourCommand.cs
 *
 *  begin                	: Gennaio, 2007
 *  version					: 0.1
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 * 			Regione della colonia penitenziaria di malas
 *  
 ***************************************************************************/

using Server;
using Server.Regions;

namespace Midgard.Engines.HardLabour
{
    public class HardLabourColonyRegion : BaseRegion
    {
        private static Rectangle3D Bounds = new Rectangle3D( 2110, 1645, -120, 87, 85, 200 );

        public HardLabourColonyRegion()
            : base( "Hard Labour Penitentiary", Map.Malas,
                    Find( new Point3D( 2152, 1682, -54 ), Map.Malas ),
                    Bounds )
        {
            GoLocation = new Point3D( 2152, 1682, -54 );
        }

        public override bool CanUseStuckMenu( Mobile m )
        {
            return false;
        }

        public override bool AllowHousing( Mobile from, Point3D p )
        {
            return false;
        }

        public override bool SendInaccessibleMessage( Item item, Mobile from )
        {
            return false;
        }

        public override void AlterLightLevel( Mobile m, ref int global, ref int personal )
        {
            global = LightCycle.JailLevel;
        }

        public override bool OnBeginSpellCast( Mobile from, ISpell s )
        {
            if( from.AccessLevel == AccessLevel.Player )
            {
                from.SendLocalizedMessage( 502629 ); // You cannot cast spells here.
            }

            return ( from.AccessLevel > AccessLevel.Player );
        }

        public override bool OnSkillUse( Mobile from, int Skill )
        {
            if( from.AccessLevel == AccessLevel.Player )
            {
                from.SendMessage( "You may not use skills while condamned to Hard Labours." );
            }

            return ( from.AccessLevel > AccessLevel.Player );
        }

        public override void OnEnter( Mobile m )
        {
            m.SendMessage( "You have entered the hard labour colony region." );
        }

        public override void OnExit( Mobile m )
        {
            m.SendMessage( "You already hard labour colony region." );
        }

        public override bool OnMoveInto( Mobile m, Direction d, Point3D newLocation, Point3D oldLocation )
        {
            if( !base.OnMoveInto( m, d, newLocation, oldLocation ) )
            {
                return false;
            }

            if( m.AccessLevel > AccessLevel.Player || Contains( oldLocation ) )
            {
                return true;
            }

            return ( m.AccessLevel > AccessLevel.Player );
        }
    }
}