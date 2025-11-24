using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Midgard.Engines.Classes
{
    public sealed class NecromancerRitual : Ritual
    {
        public static Type SealOfDeathType = typeof( SealOfDeath );
        public static Type SealOfEvilType = typeof( SealOfEvil );
        public static Type SealOfFearType = typeof( SealOfFear );
        public static Type SealOfPainType = typeof( SealOfPain );
        public static Type SealOfRevengeType = typeof( SealOfRevenge );
        public static Type SealOfUndeadType = typeof( SealOfUndead );

        public static RequirementDefinition DeathReq = new RequirementDefinition( SealOfDeathType, 1, "death seal" );
        public static RequirementDefinition EvilReq = new RequirementDefinition( SealOfEvilType, 1, "evil seal" );
        public static RequirementDefinition FearReq = new RequirementDefinition( SealOfFearType, 1, "fear seal" );
        public static RequirementDefinition PainReq = new RequirementDefinition( SealOfPainType, 1, "pain seal" );
        public static RequirementDefinition RevengeReq = new RequirementDefinition( SealOfRevengeType, 1, "revenge seal" );
        public static RequirementDefinition UndeadReq = new RequirementDefinition( SealOfUndeadType, 1, "undead seal" );

        // public static RequirementDefinition HonorReq = new RequirementDefinition( HonorStoneType, 1, "honor stone" );
        public NecromancerRitual( PowerDefinition definition, Mobile ritualist )
            : base( definition, ritualist )
        {
        }

        public override void Start()
        {
            //Item ankh;

            //if( IsNearAnkh( Ritualist, out ankh ) )
            //    Ritualist.PublicOverheadMessage( MessageType.Regular, 0x3B2, true, "* You must be near an ankh to make a ritual *" );
            //else if( CheckCombat( Ritualist ) )
            //    Ritualist.PublicOverheadMessage( MessageType.Regular, 0x3B2, true, "* Your soul is not quiet due to a recent engage *" );
            //else if( !Ritualist.CanBeginAction( typeof( Ritual ) ) )
            //    Ritualist.SendMessage( "{0}, you cannot start a new ritual yet.", Ritualist.Name );
            //else
            //{
            Ritualist.SendMessage( "Choose a container in your backpack where the ritual items are placed in." );
            Ritualist.Target = new InternalTarget( this );
            //}
        }

        public void Target( Item item )
        {
            if( !Ritualist.CanSee( item ) )
                Ritualist.SendLocalizedMessage( 500237 ); // Target can not be seen.
            else if( !( item is Container ) )
                Ritualist.SendMessage( "That is not a valid container." );
            else if( !( item.IsChildOf( Ritualist.Backpack ) ) )
                Ritualist.SendMessage( "That is not accessible." );
            else if( !CheckResources( (Container)item, Ritualist as Midgard2PlayerMobile ) )
                Ritualist.SendMessage( "Some item is lacking." );
            else
                BeginRitual();
        }

        public override double GetRitualDuration( int level )
        {
            return AnimTimer.DurationPerLevel * level;
        }

        public override void BeginRitual()
        {
            Midgard2PlayerMobile from = (Midgard2PlayerMobile)Ritualist;
            if( from == null || from.ClassState == null )
            {
                Ritualist.SendMessage( "You cannot make this ritual" );
                return;
            }

            double delay = AnimTimer.AnimateDelay;
            int level = from.ClassState.GetLevel( Definition );
            int count = (int)Math.Ceiling( GetRitualDuration( level ) / delay ) + 5;

            Console.WriteLine( "Necromancer ritual. BeginRitual for: level {0}", count );

            if( count > 0 )
            {
                AnimTimer animTimer = new AnimTimer( Ritualist, this, count, level );
                animTimer.Start();

                double effectiveDuration = ( count * delay ) + 1.0;
                Ritualist.Freeze( TimeSpan.FromSeconds( effectiveDuration ) );

                Timer.DelayCall( TimeSpan.FromSeconds( effectiveDuration ), new TimerStateCallback( MakeRitualCallback ), null );
            }
        }

        public override void MakeRitualCallback( object state )
        {
            Midgard2PlayerMobile from = (Midgard2PlayerMobile)Ritualist;

            Type[] types;
            int[] quantities;
            Definition.GetReqTypesQuantArray( out types, out quantities );

            int level = from.ClassState.GetLevel( Definition );

            for (int i = 0; i < quantities.Length; i++)
                quantities[i] *= level + 1;

            if( Ritualist.Backpack.ConsumeTotal( types, quantities ) != -1 )
            {
                from.SendMessage( "Your ritual failed because some item is lacking." );
            }
            else if( from.ClassState.IncreasePowerLevel( Definition ) )
            {
                int newLevel = from.ClassState.GetLevel( Definition );
                from.SendMessage( "Your power named *{0}* has increased. It's level is now {1}", Definition.Name, newLevel );
            }
            else
                from.SendMessage( "Your ritual failed." );

            EndRitual( from );
        }

        public override void DoEffects( Mobile from, bool success )
        {
            from.FixedParticles( 0x376A, 9, 32, 5007, EffectLayer.Waist );
            from.PlaySound( 0x210 );
        }

        public override void EndRitual( Mobile from )
        {
            from.EndAction( typeof( Ritual ) );
        }

        #region checks
        //public bool IsNearAnkh( Mobile toCheck, out Item ankh )
        //{
        //    ankh = null;

        //    foreach( Item item in toCheck.GetItemsInRange( 2 ) )
        //    {
        //        if( item is AnkhNorth || item is AnkhWest )
        //        {
        //            ankh = item;
        //            break;
        //        }
        //    }

        //    return ankh == null;
        //}
        #endregion

        #region Nested type: InternalTarget
        private class InternalTarget : Target
        {
            private NecromancerRitual m_Owner;

            public InternalTarget( NecromancerRitual owner )
                : base( 1, false, TargetFlags.None )
            {
                m_Owner = owner;
            }

            protected override void OnTarget( Mobile from, object o )
            {
                if( o is Item )
                    m_Owner.Target( (Item)o );
            }
        }
        #endregion
    }
}