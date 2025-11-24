/***************************************************************************
 *                               MidgardStatueTarget.cs
 *
 *   begin                : 07 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections;

using Server;
using Server.Items;
using Server.Multis;
using Server.Spells;
using Server.Targeting;

namespace Midgard.Items.StatueSystem
{
    public class MidgardStatueTarget : Target
    {
        private readonly Item m_Maker;
        public CraftResource Material { get; set; }

        public MidgardStatueTarget( CraftResource material, Item maker )
            : base( -1, true, TargetFlags.None )
        {
            Material = material;
            m_Maker = maker;
        }

        protected override void OnTarget( Mobile from, object targeted )
        {
            if( from == null )
                return;

            Map map = from.Map;
            if( map == null )
                return;

            if( targeted == null )
            {
                from.SendMessage( "Your target is invalid." );
                return;
            }

            IPoint3D p = targeted as IPoint3D;
            if( p == null )
            {
                from.SendMessage( "Your target is invalid." );
                return;
            }

            if( m_Maker == null || m_Maker.Deleted )
            {
                from.SendMessage( "Your tool is invalid or deleted." );
                return;
            }

            if( from.IsBodyMod )
            {
                from.SendMessage( "You may only proceed while in your original state..." );
                return;
            }

            if( from.Backpack == null || !m_Maker.IsChildOf( from.Backpack ) )
            {
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
                return;
            }

            SpellHelper.GetSurfaceTop( ref p );
            BaseHouse house = null;
            Point3D loc = new Point3D( p );

            bool isGM = from.AccessLevel > AccessLevel.Counselor;
            bool isStaffGranted = m_Maker is MidgardStatueMaker && ( (MidgardStatueMaker)m_Maker ).IsStaffGranted;

            if( ( !isGM && !isStaffGranted ) && targeted is Item && !( (Item)targeted ).IsLockedDown && !( (Item)targeted ).IsSecure && !( targeted is AddonComponent ) )
            {
                from.SendLocalizedMessage( 1076191 ); // Statues can only be placed in houses.
                return;
            }

            try
            {
                AddonFitResult result = CouldFit( loc, map, from, ref house );

                if( isGM || isStaffGranted || result == AddonFitResult.Valid )
                {
                    MidgardStatue statue = new MidgardStatue( from );
                    MidgardStatuePlinth plinth = new MidgardStatuePlinth( statue );

                    if( house != null && house.Addons != null )
                        house.Addons.Add( plinth );

                    statue.Plinth = plinth;
                    plinth.MoveToWorld( loc, map );

                    statue.Material = Material;
                    statue.InvalidatePose();

                    from.CloseGump( typeof( MidgardStatueGump ) );
                    from.SendGump( new MidgardStatueGump( m_Maker, statue, from ) );

                    if( !m_Maker.Deleted )
                        m_Maker.Delete();
                }
                else if( result == AddonFitResult.Blocked )
                    from.SendLocalizedMessage( 500269 ); // You cannot build that there.
                else if( result == AddonFitResult.NotInHouse )
                    from.SendLocalizedMessage( 1076192 ); // Statues can only be placed in houses where you are the owner or co-owner.
                else if( result == AddonFitResult.DoorsNotClosed )
                    from.SendMessage( "You must close all house doors before placing this." );
                else if( result == AddonFitResult.DoorTooClose )
                    from.SendLocalizedMessage( 500271 ); // You cannot build near the door.	
            }
            catch( Exception e )
            {
                from.SendMessage( "An error occurred. Contact the Midgard Staff." );
                Console.WriteLine( "Debug Midgard statue: {0}", e );
            }
        }

        public static AddonFitResult CouldFit( Point3D p, Map map, Mobile from, ref BaseHouse house )
        {
            if( !map.CanFit( p.X, p.Y, p.Z, 20, true, true, true ) )
                return AddonFitResult.Blocked;
            else if( !BaseAddon.CheckHouse( from, p, map, 20, ref house ) )
                return AddonFitResult.NotInHouse;
            else
                return CheckDoors( p, 20, house );
        }

        public static AddonFitResult CheckDoors( Point3D p, int height, BaseHouse house )
        {
            ArrayList doors = house.Doors;

            foreach( object t in doors )
            {
                BaseDoor door = t as BaseDoor;
                if( door == null )
                    continue;

                if( door.Open )
                    return AddonFitResult.DoorsNotClosed;

                Point3D doorLoc = door.GetWorldLocation();
                int doorHeight = door.ItemData.CalcHeight;

                if( Utility.InRange( doorLoc, p, 1 ) && ( p.Z == doorLoc.Z || ( ( p.Z + height ) > doorLoc.Z && ( doorLoc.Z + doorHeight ) > p.Z ) ) )
                    return AddonFitResult.DoorTooClose;
            }

            return AddonFitResult.Valid;
        }
    }
}