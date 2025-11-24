using System;
using System.Collections.Generic;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Midgard.Engines.MidgardTownSystem
{
    public abstract class BaseTownGuard : BaseCreature
    {
        public override SpeechFragment PersonalFragmentObj
        {
            get { return PersonalFragment.Guard; }
        }

        private const int ListenRange = 12;

        private static readonly Type[] m_StrongPotions = new Type[]
                                                         {
                                                             typeof (GreaterHealPotion), typeof (GreaterHealPotion), typeof (GreaterHealPotion),
                                                             typeof (GreaterCurePotion), typeof (GreaterCurePotion), typeof (GreaterCurePotion),
                                                             typeof (GreaterStrengthPotion), typeof (GreaterStrengthPotion),
                                                             typeof (GreaterAgilityPotion), typeof (GreaterAgilityPotion),
                                                             typeof (TotalRefreshPotion), typeof (TotalRefreshPotion),
                                                             typeof (GreaterExplosionPotion)
                                                         };

        private static readonly Type[] m_WeakPotions = new Type[]
                                                       {
                                                           typeof (HealPotion), typeof (HealPotion), typeof (HealPotion),
                                                           typeof (CurePotion), typeof (CurePotion), typeof (CurePotion),
                                                           typeof (StrengthPotion), typeof (StrengthPotion),
                                                           typeof (AgilityPotion), typeof (AgilityPotion),
                                                           typeof (RefreshPotion), typeof (RefreshPotion),
                                                           typeof (ExplosionPotion)
                                                       };

        private DateTime m_OrdersEnd;

        public BaseTownGuard( string title )
            : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
            Orders = new Orders( this );
            Title = title;

            RangeHome = 6;

            Timer.DelayCall( TimeSpan.Zero, Register );
        }

        public BaseTownGuard( Serial serial )
            : base( serial )
        {
        }

        public override bool GuardImmune
        {
            get { return true; }
        }

        public override bool BardImmune
        {
            get { return true; }
        }

        public override bool IsScaryToPets
        {
            get { return true; }
        }

        public override bool CanRummageCorpses
        {
            get { return false; }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public TownSystem System
        {
            get { return TownSystem.Find( Town ); }
        }

        public Orders Orders { get; private set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public MidgardTowns Town { get; set; }

        public abstract GuardAI GuardAI { get; }

        protected override BaseAI ForcedAI
        {
            get { return new TownGuardAI( this ); }
        }

        public override TimeSpan ReacquireDelay
        {
            get { return TimeSpan.FromSeconds( 2.0 ); }
        }

        public override bool ClickTitle
        {
            get { return false; }
        }

        public void Register()
        {
            if( System != null )
            {
                System.RegisterGuard( this );
            }
        }

        public void Unregister()
        {
            if( System != null )
            {
                System.UnregisterGuard( this );
            }
        }

        public override bool IsEnemy( Mobile m )
        {
            TownSystem ourTownSystem = System;
            TownSystem theirTownSystem = TownSystem.Find( m );

            if( theirTownSystem == null && m is BaseTownGuard )
            {
                theirTownSystem = ( (BaseTownGuard)m ).System;
            }

            if( ourTownSystem != null && theirTownSystem != null && ourTownSystem != theirTownSystem )
            {
                ReactionType reactionType = Orders.GetReaction( theirTownSystem ).Type;

                if( ourTownSystem.IsEnemyTo( theirTownSystem ) && reactionType == ReactionType.Attack )
                {
                    return true;
                }

                List<AggressorInfo> list = m.Aggressed;

                foreach( AggressorInfo ai in list )
                {
                    if( ai.Defender is BaseTownGuard )
                    {
                        BaseTownGuard bf = (BaseTownGuard)ai.Defender;

                        if( bf.System == ourTownSystem )
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public override void OnMovement( Mobile m, Point3D oldLocation )
        {
            if( m.Player && m.Alive && InRange( m, 10 ) && !InRange( oldLocation, 10 ) && InLOS( m ) && Orders.GetReaction( TownSystem.Find( m ) ).Type == ReactionType.Warn )
            {
                Direction = GetDirectionTo( m );

                string warning = null;

                switch( Utility.Random( 6 ) )
                {
                    case 0:
                        warning = "I warn you, {0}, you would do well to leave this area before someone shows you the world of gray.";
                        break;
                    case 1:
                        warning = "It would be wise to leave this area, {0}, lest your head become my commanders' trophy.";
                        break;
                    case 2:
                        warning = "You are bold, {0}, for one of the meager {1}. Leave now, lest you be taught the taste of dirt.";
                        break;
                    case 3:
                        warning = "Your presence here is an insult, {0}. Be gone now, knave.";
                        break;
                    case 4:
                        warning = "Dost thou wish to be hung by your toes, {0}? Nay? Then come no closer.";
                        break;
                    case 5:
                        warning = "Hey, {0}. Yeah, you. Get out of here before I beat you with a stick.";
                        break;
                }

                TownSystem system = TownSystem.Find( m );

                Say( warning, m.Name, system == null ? "civilians" : system.Definition.TownName.ToString() );
            }
        }

        public override bool HandlesOnSpeech( Mobile from )
        {
            if( InRange( from, ListenRange ) )
            {
                return true;
            }

            return base.HandlesOnSpeech( from );
        }

        private void ChangeReaction( TownSystem townSystem, ReactionType type )
        {
            if( townSystem == null )
            {
                switch( type )
                {
                    case ReactionType.Ignore:
                        Say( 1005179 );
                        break; // Civilians will now be ignored.
                    case ReactionType.Warn:
                        Say( 1005180 );
                        break; // Civilians will now be warned of their impending deaths.
                    case ReactionType.Attack:
                        return;
                }
            }
            else
            {
                TextDefinition def = null;

                switch( type )
                {
                    case ReactionType.Ignore:
                        def = townSystem.Definition.GuardIgnore;
                        break;
                    case ReactionType.Warn:
                        def = townSystem.Definition.GuardWarn;
                        break;
                    case ReactionType.Attack:
                        def = townSystem.Definition.GuardAttack;
                        break;
                }

                if( def != null && def.Number > 0 )
                {
                    Say( def.Number );
                }
                else if( def != null && def.String != null )
                {
                    Say( def.String );
                }
            }

            Orders.SetReaction( townSystem, type );
        }

        public override void OnSpeech( SpeechEventArgs e )
        {
            Mobile from = e.Mobile;

            if( !e.Handled && InRange( from, ListenRange ) && from.Alive )
            {
                if( e.HasKeyword( 0xE6 ) && ( Insensitive.Equals( e.Speech, "orders" ) || WasNamed( e.Speech ) ) ) // *orders*
                {
                    if( System == null || !System.HasAccess( TownAccessFlags.CommandGuards, from ) )
                    {
                        Say( 1042189 ); // I don't work for you!
                    }
                    else if( TownSystem.Find( Region ) == System )
                    {
                        Say( 1042180 ); // Your orders, sire?
                        m_OrdersEnd = DateTime.Now + TimeSpan.FromSeconds( 10.0 );
                    }
                }
                else if( DateTime.Now < m_OrdersEnd )
                {
                    if( System != null && System.HasAccess( TownAccessFlags.CommandGuards, from ) && TownSystem.Find( Region ) == System )
                    {
                        m_OrdersEnd = DateTime.Now + TimeSpan.FromSeconds( 10.0 );

                        bool understood = true;
                        ReactionType newType = 0;

                        if( Insensitive.Contains( e.Speech, "attack" ) )
                        {
                            newType = ReactionType.Attack;
                        }
                        else if( Insensitive.Contains( e.Speech, "warn" ) )
                        {
                            newType = ReactionType.Warn;
                        }
                        else if( Insensitive.Contains( e.Speech, "ignore" ) )
                        {
                            newType = ReactionType.Ignore;
                        }
                        else
                        {
                            understood = false;
                        }

                        if( understood )
                        {
                            understood = false;

                            if( Insensitive.Contains( e.Speech, "civil" ) )
                            {
                                ChangeReaction( null, newType );
                                understood = true;
                            }

                            List<TownSystem> systems = new List<TownSystem>( TownSystem.TownSystems );

                            foreach( TownSystem system in systems )
                            {
                                if( system != System && Insensitive.Contains( e.Speech, system.Definition.TownName ) )
                                {
                                    ChangeReaction( system, newType );
                                    understood = true;
                                }
                            }
                        }
                        else if( Insensitive.Contains( e.Speech, "patrol" ) )
                        {
                            Home = Location;
                            RangeHome = 6;
                            Combatant = null;
                            Orders.Movement = MovementType.Patrol;
                            Say( 1005146 ); // This spot looks like it needs protection!  I shall guard it with my life.
                            understood = true;
                        }
                        else if( Insensitive.Contains( e.Speech, "follow" ) )
                        {
                            Home = Location;
                            RangeHome = 6;
                            Combatant = null;
                            Orders.Follow = from;
                            Orders.Movement = MovementType.Follow;
                            Say( 1005144 ); // Yes, Sire.
                            understood = true;
                        }
                        else if( Insensitive.Contains( e.Speech, "waypoint" ) )
                        {
                            Combatant = null;
                            e.Mobile.Target = new ChooseWayPoint( this );
                            Say( "Choose the way I will follow." );
                            understood = true;
                        }

                        if( !understood )
                        {
                            Say( 1042183 ); // I'm sorry, I don't understand your orders...
                        }
                    }
                }
            }
            else
            {
                base.OnSpeech( e );
            }
        }

        public class ChooseWayPoint : Target
        {
            private readonly BaseTownGuard m_Guard;

            public ChooseWayPoint( BaseTownGuard guard )
                : base( 10, false, TargetFlags.None )
            {
                m_Guard = guard;
            }

            protected override void OnTarget( Mobile from, object target )
            {
                if( target is TownWayPoint && m_Guard != null )
                {
                    m_Guard.CurrentWayPoint = (WayPoint)target;
                    m_Guard.Say( 1005144 ); // Yes, Sire.
                }
                else
                {
                    from.SendMessage( "Target a town way point." );
                }
            }
        }

        public override void OnSingleClick( Mobile from )
        {
            if( System != null )
            {
                string text = String.Concat( "(Guard, ", System.Definition.TownName, ")" );

                int hue = ( TownSystem.Find( from ) == System ? 98 : 38 );

                PrivateOverheadMessage( MessageType.Label, hue, true, text, from.NetState );
            }

            base.OnSingleClick( from );
        }

        public virtual void GenerateRandomHair()
        {
            Utility.AssignRandomHair( this );
            Utility.AssignRandomFacialHair( this, HairHue );
        }

        public void PackStrongPotions( int min, int max )
        {
            PackStrongPotions( Utility.RandomMinMax( min, max ) );
        }

        public void PackStrongPotions( int count )
        {
            for( int i = 0; i < count; ++i )
            {
                PackStrongPotion();
            }
        }

        public void PackStrongPotion()
        {
            PackItem( Loot.Construct( m_StrongPotions ) );
        }

        public void PackWeakPotions( int min, int max )
        {
            PackWeakPotions( Utility.RandomMinMax( min, max ) );
        }

        public void PackWeakPotions( int count )
        {
            for( int i = 0; i < count; ++i )
            {
                PackWeakPotion();
            }
        }

        public void PackWeakPotion()
        {
            PackItem( Loot.Construct( m_WeakPotions ) );
        }

        public Item Immovable( Item item )
        {
            item.Movable = false;
            return item;
        }

        public Item Newbied( Item item )
        {
            item.LootType = LootType.Newbied;
            return item;
        }

        public Item Rehued( Item item, int hue )
        {
            item.Hue = hue;
            return item;
        }

        public Item Layered( Item item, Layer layer )
        {
            item.Layer = layer;
            return item;
        }

        public Item Resourced( BaseWeapon weapon, CraftResource resource )
        {
            weapon.Resource = resource;
            return weapon;
        }

        public Item Resourced( BaseArmor armor, CraftResource resource )
        {
            armor.Resource = resource;
            return armor;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();
            Unregister();
        }

        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            c.Delete();
        }

        public virtual void GenerateBody( bool isFemale, bool randomHair )
        {
            Hue = Utility.RandomSkinHue();

            if( isFemale )
            {
                Female = true;
                Body = 401;
                Name = NameList.RandomName( "female" );
            }
            else
            {
                Female = false;
                Body = 400;
                Name = NameList.RandomName( "male" );
            }

            if( randomHair )
            {
                GenerateRandomHair();
            }
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( (int)Town );

            Orders.Serialize( writer );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            Town = (MidgardTowns)reader.ReadInt();

            Orders = new Orders( this, reader );

            Timer.DelayCall( TimeSpan.Zero, new TimerCallback( Register ) );
        }
    }

    public class VirtualMount : IMount
    {
        private readonly VirtualMountItem m_Item;

        public VirtualMount( VirtualMountItem item )
        {
            m_Item = item;
        }

        #region IMount Members
        public Mobile Rider
        {
            get { return m_Item.Rider; }
            set { }
        }

        public virtual void OnRiderDamaged( int amount, Mobile from, bool willKill )
        {
        }
        #endregion
    }

    public class VirtualMountItem : Item, IMountItem
    {
        private readonly VirtualMount m_Mount;
        private Mobile m_Rider;

        public VirtualMountItem( Mobile mob )
            : base( 0x3EA0 )
        {
            Layer = Layer.Mount;

            m_Rider = mob;
            m_Mount = new VirtualMount( this );
        }

        public VirtualMountItem( Serial serial )
            : base( serial )
        {
            m_Mount = new VirtualMount( this );
        }

        public Mobile Rider
        {
            get { return m_Rider; }
        }

        #region IMountItem Members
        public IMount Mount
        {
            get { return m_Mount; }
        }
        #endregion

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( (Mobile)m_Rider );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Rider = reader.ReadMobile();

            if( m_Rider == null )
            {
                Delete();
            }
        }
    }
}