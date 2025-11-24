using System;
using System.Collections.Generic;

using Server.ContextMenus;
using Server.Engines.XmlSpawner2;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public class SiegeComponent : AddonComponent
    {
        public override bool ForceShowProperties { get { return true; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsDraggable
        {
            get
            {
                if( Addon is ISiegeWeapon )
                    return ( (ISiegeWeapon)Addon ).IsDraggable;
                return false;
            }
            set
            {
                if( Addon is ISiegeWeapon )
                    ( (ISiegeWeapon)Addon ).IsDraggable = value;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsPackable
        {
            get
            {
                if( Addon is ISiegeWeapon )
                    return ( (ISiegeWeapon)Addon ).IsPackable;
                return false;
            }
            set
            {
                if( Addon is ISiegeWeapon )
                    ( (ISiegeWeapon)Addon ).IsPackable = value;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool FixedFacing
        {
            get
            {
                if( Addon is ISiegeWeapon )
                    return ( (ISiegeWeapon)Addon ).FixedFacing;
                return false;
            }
            set
            {
                if( Addon is ISiegeWeapon )
                    ( (ISiegeWeapon)Addon ).FixedFacing = value;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Facing
        {
            get
            {
                if( Addon is ISiegeWeapon )
                    return ( (ISiegeWeapon)Addon ).Facing;
                return 0;
            }
            set
            {
                if( Addon is ISiegeWeapon )
                    ( (ISiegeWeapon)Addon ).Facing = value;
            }
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( Addon != null )
                Addon.OnDoubleClick( from );
        }

        public SiegeComponent( int itemID )
            : base( itemID )
        {
        }

        public SiegeComponent( int itemID, string name )
            : base( itemID )
        {
            Name = name;
        }

        public SiegeComponent( Serial serial )
            : base( serial )
        {
        }

        private class RotateNextEntry : ContextMenuEntry
        {
            private ISiegeWeapon m_Weapon;

            public RotateNextEntry( ISiegeWeapon weapon )
                : base( 406 )
            {
                m_Weapon = weapon;
            }

            public override void OnClick()
            {
                if( m_Weapon != null )
                    m_Weapon.Facing++;
            }
        }

        private class RotatePreviousEntry : ContextMenuEntry
        {
            private ISiegeWeapon m_Weapon;

            public RotatePreviousEntry( ISiegeWeapon weapon )
                : base( 405 )
            {
                m_Weapon = weapon;
            }

            public override void OnClick()
            {
                if( m_Weapon != null )
                    m_Weapon.Facing--;
            }
        }

        private class BackpackEntry : ContextMenuEntry
        {
            private ISiegeWeapon m_Weapon;
            private Mobile m_From;

            public BackpackEntry( Mobile from, ISiegeWeapon weapon )
                : base( 2139 )
            {
                m_Weapon = weapon;
                m_From = from;
            }

            public override void OnClick()
            {
                if( m_Weapon != null )
                {
                    m_Weapon.StoreWeapon( m_From );
                }
            }
        }

        private class ReleaseEntry : ContextMenuEntry
        {
            private Mobile m_From;
            private XmlDrag m_Drag;

            public ReleaseEntry( Mobile from, XmlDrag drag )
                : base( 6118 )
            {
                m_From = from;
                m_Drag = drag;
            }

            public override void OnClick()
            {
                if( m_Drag == null )
                    return;

                var pet = m_Drag.DraggedBy as BaseCreature;

                // only allow the person dragging it or their pet to release
                if( m_Drag.DraggedBy == m_From ||
                    ( pet != null && ( pet.ControlMaster == m_From || pet.ControlMaster == null ) ) )
                    m_Drag.DraggedBy = null;
            }
        }

        private class ConnectEntry : ContextMenuEntry
        {
            private Mobile m_From;
            private XmlDrag m_Drag;

            public ConnectEntry( Mobile from, XmlDrag drag )
                : base( 5119 )
            {
                m_From = from;
                m_Drag = drag;
            }

            public override void OnClick()
            {
                if( m_Drag != null && m_From != null )
                {
                    m_From.SendMessage( "Select a mobile to drag the weapon" );
                    m_From.Target = new DragTarget( m_Drag );
                }
            }
        }

        private class SetupEntry : ContextMenuEntry
        {
            private ISiegeWeapon m_Weapon;
            private Mobile m_From;

            public SetupEntry( Mobile from, ISiegeWeapon weapon )
                : base( 97 )
            {
                m_Weapon = weapon;
                m_From = from;
            }

            public override void OnClick()
            {
                if( m_Weapon != null && m_From != null )
                    m_Weapon.PlaceWeapon( m_From, m_From.Location, m_From.Map );
            }
        }

        public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
        {
            if( Addon is ISiegeWeapon )
            {
                var weapon = (ISiegeWeapon)Addon;

                if( !weapon.FixedFacing )
                {
                    list.Add( new RotateNextEntry( weapon ) );
                    list.Add( new RotatePreviousEntry( weapon ) );
                }

                if( weapon.IsPackable )
                    list.Add( new BackpackEntry( from, weapon ) );

                if( weapon.IsDraggable )
                {
                    // does it support dragging?
                    var a = (XmlDrag)XmlAttach.FindAttachment( weapon, typeof( XmlDrag ) );
                    if( a != null )
                    {
                        // is it currently being dragged?
                        if( a.DraggedBy != null && !a.DraggedBy.Deleted )
                            list.Add( new ReleaseEntry( from, a ) );
                        else
                            list.Add( new ConnectEntry( from, a ) );
                    }
                }
            }

            base.GetContextMenuEntries( from, list );
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            var weapon = Addon as ISiegeWeapon;

            if( weapon == null )
                return;

            if( weapon.Projectile == null || weapon.Projectile.Deleted )
            {
                //list.Add(1061169, "empty"); // range ~1_val~
                list.Add( 1042975 ); // It's empty
            }
            else
            {
                list.Add( 500767 ); // Reloaded
                list.Add( 1060658, "Type\t{0}", weapon.Projectile.Name ); // ~1_val~: ~2_val~

                var projectile = weapon.Projectile as ISiegeProjectile;
                if( projectile != null )
                {
                    list.Add( 1061169, projectile.Range.ToString() ); // range ~1_val~
                }
            }
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            var weapon = Addon as ISiegeWeapon;

            if( weapon == null )
                return;

            if( weapon.Projectile == null || weapon.Projectile.Deleted )
            {
                LabelTo( from, 1042975 ); // It's empty
            }
            else
            {
                LabelTo( from, 500767 ); // Reloaded
                LabelTo( from, 1060658, string.Format( "Type\t{0}", weapon.Projectile.Name ) ); // ~1_val~: ~2_val~

                var projectile = weapon.Projectile as ISiegeProjectile;
                if( projectile != null )
                    LabelTo( from, 1061169, projectile.Range.ToString() ); // range ~1_val~
            }
        }

        public void ToggleConnection( Mobile from, bool connect )
        {
            if( IsDraggable )
            {
                var a = (XmlDrag)XmlAttach.FindAttachment( Addon, typeof( XmlDrag ) );

                if( a != null && from != null )
                {
                    if( connect && a.DraggedBy != null )
                    {
                        from.SendMessage( "That is already being dragged." );
                        return;
                    }
                    else if( !connect && a.DraggedBy == null )
                    {
                        from.SendMessage( "That is already un-dragged." );
                        return;
                    }

                    if( a.DraggedBy != null && !a.DraggedBy.Deleted )
                        BeginRelease( from, a );
                    else
                        BeginDrag( from, a );
                }
            }
        }

        public void BeginRelease( Mobile from, XmlDrag drag )
        {
            BaseCreature pet = drag.DraggedBy as BaseCreature;
            if( drag.DraggedBy == from || ( pet != null && ( pet.ControlMaster == from || pet.ControlMaster == null ) ) )
                drag.DraggedBy = null;
        }

        public void BeginDrag( Mobile from, XmlDrag drag )
        {
            if( drag != null )
            {
                from.SendMessage( "Select a mobile to drag the weapon" );
                from.Target = new DragTarget( drag );
            }
        }

        private class DragTarget : Target
        {
            private XmlDrag m_Attachment;

            public DragTarget( XmlDrag attachment )
                : base( 30, false, TargetFlags.None )
            {
                m_Attachment = attachment;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_Attachment == null || from == null )
                    return;

                if( !( targeted is Mobile ) )
                {
                    from.SendMessage( "Must target a mobile" );
                    return;
                }

                var m = (Mobile)targeted;

                if( m == from ||
                    ( m is BaseCreature && ( ( (BaseCreature)m ).Controlled && ( (BaseCreature)m ).ControlMaster == from ) ) )
                    m_Attachment.DraggedBy = m;
                else
                    from.SendMessage( "You dont control that." );
            }
        }

        public override void OnMapChange()
        {
            if( Addon != null && Map != Map.Internal )
                Addon.Map = Map;
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}