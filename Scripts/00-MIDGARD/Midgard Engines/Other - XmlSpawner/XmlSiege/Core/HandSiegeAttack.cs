using System;

using Server.Items;
using Server.Targeting;

namespace Server.Engines.XmlSpawner2
{
    public class HandSiegeAttack : XmlAttachment
    {
        private const double DamageScaleFactor = 0.5; // multiplier of weapon min/max damage used to calculate siege damage.
        private const double BaseWeaponDelay = 9.0; // base delay in seconds between attacks.  Actual delay will be reduced by weapon speed.

        private Item m_AttackTarget; // target of the attack
        private int m_MaxDistance = 2; // max distance away from the target allowed

        private InternalTimer m_Timer;

        [CommandProperty( AccessLevel.GameMaster )]
        public Item AttackTarget
        {
            get { return m_AttackTarget; }
            set
            {
                m_AttackTarget = value;

                if( m_AttackTarget != null )
                {
                    // immediate attack unless already attacking
                    DoTimer( TimeSpan.Zero, true );
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int MaxDistance { get { return m_MaxDistance; } set { m_MaxDistance = value; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public Point3D CurrentLoc { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Map CurrentMap { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Point3D TargetLoc { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Map TargetMap { get; set; }

        public HandSiegeAttack( ASerial serial )
            : base( serial )
        {
        }

        [Attachable]
        public HandSiegeAttack()
        {
        }

        public static void SelectTarget( Mobile from, Item weapon )
        {
            if( from == null || weapon == null )
                return;

            // does this weapon have a HandSiegeAttack attachment on it already?
            var a = (HandSiegeAttack)XmlAttach.FindAttachment( weapon, typeof( HandSiegeAttack ) );

            if( a == null || a.Deleted )
            {
                a = new HandSiegeAttack();
                XmlAttach.AttachTo( weapon, a );
            }

            from.Target = new HandSiegeTarget( weapon, a );
        }

        private class HandSiegeTarget : Target
        {
            private Item m_Weapon;
            private HandSiegeAttack m_Attachment;

            public HandSiegeTarget( Item weapon, HandSiegeAttack attachment )
                : base( 30, true, TargetFlags.None )
            {
                m_Weapon = weapon;
                m_Attachment = attachment;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( from == null || m_Weapon == null || from.Map == null || m_Attachment == null )
                    return;

                if( targeted is StaticTarget )
                {
                    int staticid = ( (StaticTarget)targeted ).ItemID;
                    int staticx = ( (StaticTarget)targeted ).Location.X;
                    int staticy = ( (StaticTarget)targeted ).Location.Y;

                    Item multiitem = null;

                    // find the possible multi owner of the static tile
                    foreach( Item item in from.Map.GetItemsInRange( ( (StaticTarget)targeted ).Location, 50 ) )
                    {
                        if( item is BaseMulti )
                        {
                            // search the component list for a match
                            MultiComponentList mcl = ( (BaseMulti)item ).Components;
                            bool found = false;
                            if( mcl != null && mcl.List != null )
                            {
                                for( int i = 0; i < mcl.List.Length; i++ )
                                {
                                    MultiTileEntry t = mcl.List[ i ];

                                    int x = t.m_OffsetX + item.X;
                                    int y = t.m_OffsetY + item.Y;
                                    // int z = t.m_OffsetZ + item.Z;
                                    int itemID = t.m_ItemID & 0x3FFF;

                                    if( itemID == staticid && x == staticx && y == staticy )
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                            }

                            if( found )
                            {
                                multiitem = item;
                                break;
                            }
                        }
                    }

                    if( multiitem != null )
                    {
                        //Console.WriteLine("attacking {0} at {1}:{2}", multiitem, tileloc, ((StaticTarget)targeted).Location);
                        // may have to reconsider the use tileloc vs target loc
                        //m_cannon.AttackTarget(from, multiitem, ((StaticTarget)targeted).Location);

                        //m_Weapon.AttackTarget(from, multiitem, multiitem.Map.GetPoint(targeted, true), m_checklos);

                        m_Attachment.BeginAttackTarget( from, multiitem, multiitem.Map.GetPoint( targeted, true ) );
                    }
                }
                else if( targeted is AddonComponent )
                {
                    // if the addon doesnt have an xmlsiege attachment, then attack the addon
                    var a = (XmlSiege)XmlAttach.FindAttachment( targeted, typeof( XmlSiege ) );
                    if( a == null || a.Deleted )
                        m_Attachment.BeginAttackTarget( from, ( (AddonComponent)targeted ).Addon, ( (Item)targeted ).Location );
                    else
                        m_Attachment.BeginAttackTarget( from, (Item)targeted, ( (Item)targeted ).Location );
                }
                else if( targeted is Item )
                    m_Attachment.BeginAttackTarget( from, (Item)targeted, ( (Item)targeted ).Location );
                else
                    from.SendMessage( "Thou cannot attack that." );
            }
        }

        public void BeginAttackTarget( Mobile from, Item target, Point3D targetloc )
        {
            if( from == null || target == null )
                return;

            // check the target line of sight
            var adjustedloc = new Point3D( targetloc.X, targetloc.Y, targetloc.Z + target.ItemData.Height );
            var fromloc = new Point3D( from.Location.X, from.Location.Y, from.Location.Z + 14 );

            if( !from.Map.LineOfSight( fromloc, adjustedloc ) )
            {
                from.SendMessage( "Cannot see target." );
                return;
            }

            var distance = (int)XmlSiege.GetDistance( from.Location, targetloc );

            if( distance <= MaxDistance )
            {
                CurrentLoc = from.Location;
                CurrentMap = from.Map;
                TargetLoc = target.Location;
                TargetMap = target.Map;

                AttackTarget = target;
            }
            else
                from.SendLocalizedMessage( 500446 ); // That is too far away.
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }

        public override void OnAttach()
        {
            base.OnAttach();

            if( !( AttachedTo is Item ) )
                Delete();
        }

        public override void OnDelete()
        {
            base.OnDelete();

            if( m_Timer != null )
                m_Timer.Stop();
        }

        public void DoTimer( TimeSpan delay, bool wait )
        {
            // is there a timer already running?  Then let it finish
            if( m_Timer != null && m_Timer.Running && wait )
                return;

            if( m_Timer != null )
                m_Timer.Stop();

            m_Timer = new InternalTimer( this, delay );
            m_Timer.Start();
        }

        // added the duration timer that begins on spawning
        private class InternalTimer : Timer
        {
            private HandSiegeAttack m_Attachment;

            public InternalTimer( HandSiegeAttack attachment, TimeSpan delay )
                : base( delay )
            {
                Priority = TimerPriority.TwoFiftyMS;
                m_Attachment = attachment;
            }

            protected override void OnTick()
            {
                if( m_Attachment == null )
                    return;

                var weapon = m_Attachment.AttachedTo as Item;
                Item target = m_Attachment.AttackTarget;

                if( weapon == null || weapon.Deleted || target == null || target.Deleted )
                {
                    Stop();
                    return;
                }

                // the weapon must be equipped
                var attacker = weapon.Parent as Mobile;

                if( attacker == null || attacker.Deleted )
                {
                    Stop();
                    return;
                }

                // the attacker cannot be fighting

                if( attacker.Combatant != null )
                {
                    attacker.SendMessage( "Cannot siege while fighting." );
                    Stop();
                    return;
                }

                // get the location of the attacker
                Point3D attackerloc = attacker.Location;
                Map attackermap = attacker.Map;

                Map targetmap = target.Map;

                if( targetmap == null || targetmap == Map.Internal || attackermap == null || attackermap == Map.Internal ||
                    targetmap != attackermap )
                {
                    // if the attacker or target has an invalid map, then stop
                    Stop();
                    return;
                }

                // compare it against previous locations.  If they have moved then break off the attack
                if( attackerloc != m_Attachment.CurrentLoc || attackermap != m_Attachment.CurrentMap )
                {
                    Stop();
                    return;
                }

                // attack the target
                // Animate( int action, int frameCount, int repeatCount, bool forward, bool repeat, int delay )
                int action = 26; // 1-H bash animation, 29=2-H mounted

                // get the layer
                switch( weapon.Layer )
                {
                    case Layer.OneHanded:
                        action = attacker.Mount == null ? 9 : 26;
                        break;
                    case Layer.TwoHanded:
                        action = attacker.Mount == null ? 12 : 29;
                        break;
                }

                // attack animation
                attacker.Animate( action, 7, 1, true, false, 0 );

                // attack sound
                attacker.PlaySound( 0x3b4 ); // mod by Dies Irae

                int basedamage = 1;
                double basedelay = BaseWeaponDelay;

                if( weapon is BaseWeapon )
                {
                    var b = (BaseWeapon)weapon;
                    // calculate the siege damage based on the weapon min/max damage and the overall damage scale factor
                    basedamage = (int)( Utility.RandomMinMax( b.MinDamage, b.MaxDamage ) * DamageScaleFactor );
                    // reduce the actual delay by the weapon speed
                    basedelay -= b.Speed / 10.0;
                }

                if( basedelay < 1 )
                    basedelay = 1;
                if( basedamage < 1 )
                    basedamage = 1;

                // apply siege damage, all physical
                XmlSiege.Attack( attacker, target, basedamage, 0 );

                // prepare for the next attack
                m_Attachment.DoTimer( TimeSpan.FromSeconds( basedelay ), false );
            }
        }
    }
}