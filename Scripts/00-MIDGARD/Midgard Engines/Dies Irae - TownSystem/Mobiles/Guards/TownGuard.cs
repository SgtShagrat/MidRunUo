using System;

using Server.Items;
using Server.Mobiles;
using Server;
using Server.Network;
using Server.Regions;

namespace Midgard.Engines.MidgardTownSystem
{
    public class TownGuard : BaseGuard
    {
        public override bool GuardImmune { get { return true; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool HasAttackTimer
        {
            get { return m_AttackTimer != null; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool HasIdleTimer
        {
            get { return m_IdleTimer != null; }
        }

        private Timer m_AttackTimer, m_IdleTimer;
        private Mobile m_Focus;

        [CommandProperty( AccessLevel.GameMaster )]
        public override Mobile Focus
        {
            get
            {
                return m_Focus;
            }
            set
            {
                if( Deleted )
                    return;

                Mobile oldFocus = m_Focus;

                if( oldFocus != value )
                {
                    m_Focus = value;

                    if( value != null )
                        AggressiveAction( value );

                    Combatant = value;

                    if( oldFocus != null && !oldFocus.Alive )
                        Say( "Thou hast suffered thy punishment, scoundrel." );

                    if( value != null )
                        Say( 500131 ); // Thou wilt regret thine actions, swine!

                    if( m_AttackTimer != null )
                    {
                        m_AttackTimer.Stop();
                        m_AttackTimer = null;
                    }

                    if( m_IdleTimer != null )
                    {
                        m_IdleTimer.Stop();
                        m_IdleTimer = null;
                    }

                    if( m_Focus != null )
                    {
                        m_AttackTimer = new AttackTimer( this );
                        m_AttackTimer.Start();
                        ( (AttackTimer)m_AttackTimer ).DoOnTick();
                    }
                    else
                    {
                        m_IdleTimer = new IdleTimer( this );
                        m_IdleTimer.Start();
                    }
                }
                else if( m_Focus == null && m_IdleTimer == null )
                {
                    m_IdleTimer = new IdleTimer( this );
                    m_IdleTimer.Start();
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public MidgardTowns Town { get; private set; }

        public TownSystem System
        {
            get { return TownSystem.Find( Town ); }
        }

        public TownGuard( Mobile target, MidgardTowns town )
            : base( target, AIType.AI_Melee )
        {
            Town = town;

            SetStr( 351, 400 );
            SetDex( 151, 165 );
            SetInt( 76, 100 );

            SetHits( 448, 470 );

            SetDamage( 15, 25 );

            VirtualArmor = 40;

            SetSkill( SkillName.Swords, 90.0, 100.0 );
            SetSkill( SkillName.Macing, 90.0, 100.0 );
            SetSkill( SkillName.Wrestling, 90.0, 100.0 );
            SetSkill( SkillName.Tactics, 90.0, 100.0 );
            SetSkill( SkillName.MagicResist, 90.0, 100.0 );
            SetSkill( SkillName.Healing, 90.0, 100.0 );
            SetSkill( SkillName.Anatomy, 90.0, 100.0 );

            if( System != null )
                System.DressTownGuard( this );
            else
            {
                Config.Pkg.LogWarningLine( "Warning: guard spawned with null town system at location {0} of Map {1}.", Location, Map.Name );
                DressWarriorGuard();
            }

            NextCombatTime = DateTime.Now + TimeSpan.FromSeconds( 0.5 );
            Focus = target;

            Console.WriteLine( "DressTownGuard creata per: {0}", Focus != null ? Focus.Name : "nessuno" );
        }

        public TownGuard( Serial serial )
            : base( serial )
        {
        }

        public virtual void DressWarriorGuard()
        {
            InitStats( 1000, 1000, 1000 );
            Title = "the guard";

            SpeechHue = Utility.RandomDyedHue();

            Hue = Utility.RandomSkinHue();
            Female = Utility.RandomBool();

            if( Female )
            {
                Body = 0x191;
                Name = NameList.RandomName( "female" );

                switch( Utility.Random( 2 ) )
                {
                    case 0: AddItem( new LeatherSkirt() ); break;
                    case 1: AddItem( new LeatherShorts() ); break;
                }

                switch( Utility.Random( 5 ) )
                {
                    case 0: AddItem( new FemaleLeatherChest() ); break;
                    case 1: AddItem( new FemaleStuddedChest() ); break;
                    case 2: AddItem( new LeatherBustierArms() ); break;
                    case 3: AddItem( new StuddedBustierArms() ); break;
                    case 4: AddItem( new FemalePlateChest() ); break;
                }
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName( "male" );

                AddItem( new PlateChest() );
                AddItem( new PlateArms() );
                AddItem( new PlateLegs() );

                switch( Utility.Random( 3 ) )
                {
                    case 0: AddItem( new Doublet( Utility.RandomNondyedHue() ) ); break;
                    case 1: AddItem( new Tunic( Utility.RandomNondyedHue() ) ); break;
                    case 2: AddItem( new BodySash( Utility.RandomNondyedHue() ) ); break;
                }
            }
            Utility.AssignRandomHair( this );

            if( Utility.RandomBool() )
                Utility.AssignRandomFacialHair( this, HairHue );

            Halberd weapon = new Halberd();

            weapon.Movable = false;
            weapon.Crafter = this;
            weapon.Quality = WeaponQuality.Exceptional;

            AddItem( weapon );

            Container pack = new Backpack();

            pack.Movable = false;

            pack.DropItem( new Gold( 10, 25 ) );

            AddItem( pack );

            Skills[ SkillName.Anatomy ].Base = 120.0;
            Skills[ SkillName.Tactics ].Base = 120.0;
            Skills[ SkillName.Swords ].Base = 120.0;
            Skills[ SkillName.MagicResist ].Base = 120.0;
            Skills[ SkillName.DetectHidden ].Base = 100.0;
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            if( System != null )
                list.Add( "Town Guard of {0}", System.Definition.TownName );
        }

        public override void OnSingleClick( Mobile from )
        {
            base.OnSingleClick( from );

            if( System != null )
            {
                int hue = Server.Notoriety.GetHue( Server.Notoriety.Compute( from, this ) );

                PrivateOverheadMessage( MessageType.Label, hue, true, String.Format( "Town Guard of {0}", System.Definition.TownName ), from.NetState );
            }
        }

        public override void AlterMeleeDamageTo( Mobile to, ref int damage )
        {
            if( to is BaseCreature )
                damage *= 10;

            base.AlterMeleeDamageTo( to, ref damage );
        }

        public override bool OnBeforeDeath()
        {
            if( m_Focus != null && m_Focus.Alive )
                new AvengeTimer( this, m_Focus ).Start(); // If a guard dies, three more guards will spawn

            return base.OnBeforeDeath();
        }

        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            if( System != null )
                c.Name = string.Format( "a town guard of {0}", System.Definition.TownName );
            else
                c.Name = "a town guard";
        }

        public override void OnAfterDelete()
        {
            if( m_AttackTimer != null )
            {
                m_AttackTimer.Stop();
                m_AttackTimer = null;
            }

            if( m_IdleTimer != null )
            {
                m_IdleTimer.Stop();
                m_IdleTimer = null;
            }

            base.OnAfterDelete();
        }

        public virtual bool CheckNewEnemies()
        {
            if( Focus != null ) // not idling
            {
                DebugSay( "I should be idle but I'm not." );
                return false;
            }

            DebugSay( "I'm idle. I will search for new enemies." );

            foreach( Mobile m in GetMobilesInRange( 15 ) )
            {
                if( IsEnemy( m ) )
                {
                    Focus = m;
                    DebugSay( "Here I am, my friend!! I will attack {0}", m.Name ?? m.GetType().Name );
                    return true;
                }
                else
                    DebugSay( "{0} is not a good enemy", m.Name ?? m.GetType().Name );
            }

            return false;
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.Dismount;
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( m_Focus );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        m_Focus = reader.ReadMobile();

                        if( m_Focus != null )
                        {
                            m_AttackTimer = new AttackTimer( this );
                            m_AttackTimer.Start();
                        }
                        else
                        {
                            m_IdleTimer = new IdleTimer( this );
                            m_IdleTimer.Start();
                        }

                        break;
                    }
            }
        }
        #endregion

        #region timers
        private class AvengeTimer : Timer
        {
            private readonly Mobile m_Focus;
            private readonly bool m_IsBackup;

            public AvengeTimer( BaseGuard guard, Mobile focus )
                : base( TimeSpan.FromSeconds( 10.0 ), TimeSpan.FromSeconds( 1.0 ), 3 )
            {
                m_Focus = focus;
                m_IsBackup = guard.IsBackupGuard;

                if( guard.Focus != null )
                    m_Focus = guard.Focus;
                else if( guard.Focus == null && guard.Combatant != null )
                    m_Focus = guard.Combatant;
            }

            protected override void OnTick()
            {
                if( !m_IsBackup )
                {
                    Spawn( m_Focus, m_Focus, 1, true, true );
                }
            }
        }

        private class AttackTimer : Timer
        {
            private TownGuard m_Owner;

            public AttackTimer( TownGuard owner )
                : base( TimeSpan.FromSeconds( 0.25 ), TimeSpan.FromSeconds( 0.1 ) )
            {
                m_Owner = owner;
            }

            public void DoOnTick()
            {
                OnTick();
            }

            protected override void OnTick()
            {
                if( m_Owner.Deleted )
                {
                    Stop();
                    return;
                }

                m_Owner.Criminal = false;
                m_Owner.Kills = 0;
                m_Owner.Stam = m_Owner.StamMax;

                Mobile target = m_Owner.Focus;

                if( target != null && ( target.Deleted || !target.Alive || !m_Owner.CanBeHarmful( target ) ) )
                {
                    m_Owner.Focus = null;
                    Stop();
                    return;
                }
                else if( m_Owner.Weapon is Fists )
                {
                    Config.Pkg.LogWarningLine( "Warning: guard disarmed at location {0} of Map {1}.", m_Owner.Location, m_Owner.Map.Name );

                    //m_Owner.Kill();
                    m_Owner.Say( "Hey, I have no weapon in my hand! I cannot combat anymore." );

                    m_Owner.EquipWeapon();
                    // Stop();
                    return;
                }

                if( target != null && m_Owner.Combatant != target )
                    m_Owner.Combatant = target;

                GuardedRegion reg = null;
                if( target != null )
                    reg = (GuardedRegion)target.Region.GetRegion( typeof( GuardedRegion ) );

                if( target == null )
                {
                    Stop();
                }
                else if( reg == null || reg.Disabled || !m_Owner.InRange( target, 20 ) || !reg.IsGuardCandidate( target ) )
                {
                    m_Owner.Focus = null;
                }
                else if( !m_Owner.CanSee( target ) && target.Criminal )
                {
                    if( !m_Owner.UseSkill( SkillName.DetectHidden ) && Utility.Random( 50 ) == 0 )
                        m_Owner.Say( "Reveal!" );
                }
                else if( !m_Owner.InRange( target, 10 ) || !m_Owner.InLOS( target ) )
                {
                    TeleportTo( target );
                }
                else if( !m_Owner.InRange( target, 1 ) )
                {
                    if( !m_Owner.AIObject.MoveTo( target, true, 1 ) )
                        TeleportTo( target );

                    if( !m_Owner.Move( m_Owner.GetDirectionTo( target ) | Direction.Running ) )
                        TeleportTo( target );
                }
                else if( target is BaseCreature && ( (BaseCreature)target ).GetMaster() == null )
                {
                    ( (BaseCreature)target ).NoKillAwards = true;
                    ( (BaseCreature)target ).KilledByGuard = true;

                    if( !World.Loading )
                    {
                        target.Damage( target.HitsMax, m_Owner );
                        target.Kill();
                    }
                }
            }

            private void TeleportTo( IEntity target )
            {
                Point3D from = m_Owner.Location;
                Point3D to = target.Location;

                m_Owner.Location = to;

                Effects.SendLocationParticles( EffectItem.Create( from, m_Owner.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
                Effects.SendLocationParticles( EffectItem.Create( to, m_Owner.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 5023 );

                m_Owner.PlaySound( 0x1FE );
            }
        }

        public virtual void EquipWeapon()
        {
            Halberd weapon = new Halberd();

            weapon.Crafter = this;
            weapon.Movable = false;
            weapon.Quality = WeaponQuality.Exceptional;
            weapon.CustomQuality = Quality.Exceptional;
            weapon.MaxHitPoints = 1000;
            weapon.HitPoints = 1000;

            AddItem( weapon );
        }

        private class IdleTimer : Timer
        {
            private TownGuard m_Owner;
            private int m_Stage;

            public IdleTimer( TownGuard owner )
                : base( TimeSpan.FromSeconds( 2.0 ), TimeSpan.FromSeconds( 2.5 ) )
            {
                m_Owner = owner;
            }

            protected override void OnTick()
            {
                if( m_Owner.Deleted )
                {
                    Stop();
                    return;
                }

                if( m_Owner != null && m_Owner.CheckNewEnemies() )
                {
                    Stop();
                    return;
                }

                if( m_Owner == null )
                {
                    Stop();
                    return;
                }

                if( ( m_Stage++ % 4 ) == 0 || !m_Owner.Move( m_Owner.Direction ) )
                    m_Owner.Direction = (Direction)Utility.Random( 8 );

                if( m_Stage > 16 )
                {
                    Effects.SendLocationParticles( EffectItem.Create( m_Owner.Location, m_Owner.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
                    m_Owner.PlaySound( 0x1FE );

                    m_Owner.Delete();
                }
            }
        }
        #endregion
    }
}