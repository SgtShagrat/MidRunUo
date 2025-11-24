/***************************************************************************
 *                               BlessedDrop.cs
 *
 *   begin                : 05 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Targeting;
using Midgard.Engines.Classes;

namespace Midgard.Engines.SpellSystem
{
    public class BlessedDrop : BasePotion
    {
        private static readonly bool InstantExplosion = false;
        private static readonly int ExplosionRange = 5;
        public static readonly int MinDamage = 11;
        public static readonly int MaxDamage = 20;

        private bool m_Filled;
        private bool m_HolyBlessed;
        private Timer m_Timer;
        private List<Mobile> m_Users;
        private BlessedDrop m_ThrownPotion;

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public bool Filled
        {
            get { return m_Filled; }
            set
            {
                bool oldValue = m_Filled;
                if( oldValue != value )
                {
                    m_Filled = value;
                    OnFilledChanged( oldValue );
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public bool HolyBlessed
        {
            get { return m_HolyBlessed; }
            set
            {
                bool oldValue = m_HolyBlessed;
                if( oldValue != value )
                {
                    m_HolyBlessed = value;
                    OnBlessedChanged( oldValue );
                }
            }
        }

        public override string DefaultName
        {
            get { return "crystal bottle"; }
        }

        public override bool RequireFreeHand
        {
            get { return false; }
        }

        public override bool DisplayWeight
        {
            get { return false; }
        }

        [Constructable]
        public BlessedDrop()
            : base( 0xF0E, PotionEffect.ExplosionGreater, 1 )
        {
            Stackable = false;
            Weight = 1.0;
        }

        public BlessedDrop( Serial serial )
            : base( serial )
        {
        }

        public override void OnSingleClick( Mobile from )
        {
            if( from.TrueLanguage == LanguageType.Ita )
            {
                LabelTo( from, m_Filled ? "una bottiglia finemente decorata piena d'acqua" : "una bottiglia finemente decorata" );

                if( m_HolyBlessed )
                    LabelTo( from, "infusa di Sacro Potere" );
            }
            else
            {
                LabelTo( from, m_Filled ? "bottle filled of water" : "fine decorated crystal bottle" );

                if( m_HolyBlessed )
                    LabelTo( from, "infused with Holy power" );
            }
        }

        public virtual object FindParent( Mobile from, BlessedDrop pot )
        {
            Mobile m = pot.HeldBy;

            if( m != null && m.Holding == pot )
                return m;

            object obj = pot.RootParent;

            if( obj != null )
                return obj;

            if( Map == Map.Internal )
                return from;

            return pot;
        }

        public virtual object FindParent( Mobile from )
        {
            return FindParent( from, this );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( ClassSystem.IsPaladine( from ) )
            {
                if( !m_Filled )
                {
                    from.BeginTarget( -1, true, TargetFlags.None, new TargetCallback( Fill_OnTarget ) );
                    from.SendMessage( "Choose a pure water to fill this bottle." );
                }
                else if( !m_HolyBlessed )
                    from.SendMessage( "You have to bless this water before use." );
                else
                {
                    base.OnDoubleClick( from );
                }
            }
            else
                from.SendMessage( "Only a Paladin can use this item!" );
        }

        public override void Drink( Mobile from )
        {
            if( from.Paralyzed || from.Frozen || ( from.Spell != null && from.Spell.IsCasting ) || !from.CanBeginAction( typeof( BlessedDrop ) ) )
            {
                from.SendMessage( "{0}, you cannot use this hoy water now!" );
                return;
            }

            if( !IsChildOf( from.Backpack ) )
            {
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
                return;
            }

            ThrowTarget targ = from.Target as ThrowTarget;
            if( targ != null && targ.Potion == this )
                return;

            from.RevealingAction( true );

            if( m_Users == null )
                m_Users = new List<Mobile>();

            if( !m_Users.Contains( from ) )
                m_Users.Add( from );

            from.SendMessage( "The water in that bottle blinks around with a luminescent sparkle!" );
            Consume();

            m_ThrownPotion = null;
            m_Timer = null;

            m_ThrownPotion = new BlessedDrop();
            m_ThrownPotion.Hue = 1154;

            m_ThrownPotion.Stackable = false;

            from.AddToBackpack( m_ThrownPotion );
            from.Target = new ThrowTarget( m_ThrownPotion );

            if( m_Timer != null )
                return;

            from.BeginAction( typeof( BlessedDrop ) );
            from.SendMessage( "Paladin, throw that bottle upon your enemy!" );
            m_Timer = Timer.DelayCall( TimeSpan.FromSeconds( 0.75 ),
                                       TimeSpan.FromSeconds( 1.0 ), 4,
                                       new TimerStateCallback( Detonate_OnTick ),
                                       new object[] { from, 3, m_ThrownPotion } );

            Timer.DelayCall( TimeSpan.FromSeconds( 2.0 ), new TimerStateCallback( ReleaseLock ), from );
        }

        private static void ReleaseLock( object state )
        {
            ( (Mobile)state ).EndAction( typeof( BlessedDrop ) );
        }

        public void Explode( Mobile from, bool direct, Point3D loc, Map map )
        {
            if( Deleted )
                return;

            Delete();

            for( int i = 0; m_Users != null && i < m_Users.Count; ++i )
            {
                Mobile m = m_Users[ i ];
                ThrowTarget targ = m.Target as ThrowTarget;

                if( targ != null && targ.Potion == this )
                    Target.Cancel( m );
            }

            if( map == null )
                return;

            Effects.PlaySound( loc, map, 0x207 );
            Effects.SendLocationEffect( loc, map, 0x36BD, 20 );

            IPooledEnumerable eable = map.GetMobilesInRange( loc, ExplosionRange );
            List<Mobile> toExplode = new List<Mobile>();

            foreach( object o in eable )
            {
                if( o is Mobile )
                    toExplode.Add( (Mobile)o );
            }

            eable.Free();

            int bonus = (int)( from.Karma / 100.0 );

            if( bonus > 150 )
                bonus = 150;
            else if( bonus < 100 )
                bonus = 100;

            bonus += RPGSpellsSystem.GetPowerLevel( from, typeof( BlessedDropsSpell ) ) * 10;

            int min = AOS.Scale( MinDamage, bonus );
            int max = AOS.Scale( MaxDamage, bonus );

            foreach( Mobile m in toExplode )
            {
                if( m == null || m.Deleted )
                    continue;

                if( ( SpellHelper.ValidIndirectTarget( from, m ) && from.CanBeHarmful( m, false ) ) )
                {
                    if( RPGPaladinSpell.IsEnemy( from, m ) )
                    {
                        from.DoHarmful( m );

                        int damage = Utility.RandomMinMax( min, max );

                        if( !m.Player )
                        {
                            if( m is BaseCreature )
                            {
                                ( (BaseCreature)m ).BeginFlee( TimeSpan.FromSeconds( 30.0 ) );
                                m.PublicOverheadMessage( MessageType.Regular, 1154, true, "* the creatures flees in pain *" );
                            }

                            damage *= 2;
                        }

                        AOS.Damage( m, from, damage, 0, 50, 0, 0, 50 );
                    }
                }
            }
        }

        private class ThrowTarget : Target
        {
            public BlessedDrop Potion { get; private set; }

            public ThrowTarget( BlessedDrop potion )
                : base( 12, true, TargetFlags.None )
            {
                Potion = potion;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( Potion.Deleted || Potion.Map == Map.Internal )
                    return;

                IPoint3D p = targeted as IPoint3D;

                if( p == null )
                    return;

                Map map = from.Map;

                if( map == null )
                    return;

                SpellHelper.GetSurfaceTop( ref p );

                from.RevealingAction( true );

                IEntity to;

                if( p is Mobile )
                    to = (Mobile)p;
                else
                    to = new Entity( Serial.Zero, new Point3D( p ), map );

                Effects.SendMovingEffect( from, to, Potion.ItemID & 0x3FFF, 7, 0, false, false, Potion.Hue, 0 );

                Potion.Internalize();
                Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( Reposition_OnTick ), new object[] { from, p, map, Potion } );
            }
        }

        public void Fill_OnTarget( Mobile from, object targ )
        {
            if( m_Filled )
                return;

            if( targ is Item )
            {
                Item item = (Item)targ;

                IWaterSource src = ( item as IWaterSource );

                if( src == null && item is AddonComponent )
                    src = ( ( (AddonComponent)item ).Addon as IWaterSource );

                if( src == null || src.Quantity <= 0 )
                    return;

                if( from.Map != item.Map || !from.InRange( item.GetWorldLocation(), 2 ) || !from.InLOS( item ) )
                    from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
                else
                {
                    m_Filled = true;
                    from.SendMessage( "You filled the bottle with some pure water." );
                }
            }
            else
                from.SendMessage( "That is not a good way to fill this bottle." );
        }

        private void Detonate_OnTick( object state )
        {
            if( m_ThrownPotion.Deleted )
                return;

            object[] states = (object[])state;
            Mobile from = (Mobile)states[ 0 ];
            int timer = (int)states[ 1 ];
            BlessedDrop potion = (BlessedDrop)states[ 2 ];

            object parent = FindParent( from, potion );

            if( timer == 0 )
            {
                Point3D loc;
                Map map;

                if( parent is Item )
                {
                    Item item = (Item)parent;

                    loc = item.GetWorldLocation();
                    map = item.Map;
                }
                else if( parent is Mobile )
                {
                    Mobile m = (Mobile)parent;

                    loc = m.Location;
                    map = m.Map;
                }
                else
                    return;

                potion.Explode( from, true, loc, map );
            }
            else
            {
                if( parent is Item )
                    ( (Item)parent ).PublicOverheadMessage( MessageType.Regular, 0x3B2, false, timer.ToString() );
                else if( parent is Mobile )
                    ( (Mobile)parent ).PublicOverheadMessage( MessageType.Regular, 0x3B2, false, timer.ToString() );

                states[ 1 ] = timer - 1;
            }
        }

        private static void Reposition_OnTick( object state )
        {
            object[] states = (object[])state;
            if( states == null )
                return;

            Mobile from = (Mobile)states[ 0 ];
            IPoint3D p = (IPoint3D)states[ 1 ];
            Map map = (Map)states[ 2 ];
            BlessedDrop potion = (BlessedDrop)states[ 3 ];

            Point3D loc = new Point3D( p );

            if( InstantExplosion )
                potion.Explode( from, true, loc, map );
            else
                potion.MoveToWorld( loc, map );
        }

        public void OnFilledChanged( bool oldValue )
        {
            Hue = m_Filled ? 0x482 : 0;
            Delta( ItemDelta.Update );
        }

        public void OnBlessedChanged( bool oldValue )
        {
            Hue = m_HolyBlessed ? 0x866 : 0x482;
            Delta( ItemDelta.Update );
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( m_Filled );
            writer.Write( m_HolyBlessed );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Filled = reader.ReadBool();
            m_HolyBlessed = reader.ReadBool();
        }
        #endregion
    }
}