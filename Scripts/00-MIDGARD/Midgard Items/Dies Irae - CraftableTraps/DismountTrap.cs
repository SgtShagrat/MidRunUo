/***************************************************************************
 *                                  .cs
 *                            		-------------------
 *  begin                	: Mese, 2000
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using Midgard.Engines.MidgardTownSystem;
using Server;
using Server.Mobiles;

namespace Midgard.Items
{
    public abstract class BaseDismountTrap : BaseCraftableTrap
    {
        public override int AttackMessage { get { return 1040023; } } // You have been knocked off of your mount!
        public override int DisarmMessage { get { return 1010539; } } // You carefully remove the pressure trigger and disable the trap.

        public override int EffectSound { get { return 0; } }
        public override int MessageHue { get { return 0x78; } }

        public abstract double DismountSeconds { get; }
        public abstract int MountDamages { get; }
        public abstract int RiderDamages { get; }

        public override void DoVisibleEffect()
        {
        }

        public override void DoAttackEffect( Mobile m )
        {
            m.Damage( RiderDamages );

            IMount mt = m.Mount;

            if( mt != null )
            {
                if( mt is BaseCreature )
                    ( (BaseCreature)mt ).Damage( MountDamages );

                mt.Rider = null;
            }

            BaseMount.SetMountPrevention( m, BlockMountType.Dazed, TimeSpan.FromSeconds( DismountSeconds ) );
        }

        public BaseDismountTrap( TownSystem system, Mobile owner )
            : base( system, owner, 0x11C1 )
        {
        }

        public BaseDismountTrap( Serial serial )
            : base( serial )
        {
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

            if( ItemID != 0x11C1 )
                ItemID = 0x11C1;
        }
    }

    public class LightDismountTrap : BaseDismountTrap
    {
        public override TrapLevel Level { get { return TrapLevel.Light; } }
        public override TimeSpan DecayPeriod { get { return TimeSpan.FromDays( 1.0 ); } }
        public override double DismountSeconds { get { return 3.0; } }
        public override int MountDamages { get { return 0; } }
        public override int RiderDamages { get { return 1; } }

        public override Type DeedType { get { return typeof( LightDismountTrapDeed ); } }

        public LightDismountTrap( TownSystem system, Mobile owner )
            : base( system, owner )
        {
        }

        public LightDismountTrap( Serial serial )
            : base( serial )
        {
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

    public class MediumDismountTrap : BaseDismountTrap
    {
        public override TrapLevel Level { get { return TrapLevel.Medium; } }
        public override TimeSpan DecayPeriod { get { return TimeSpan.FromDays( 2.0 ); } }
        public override double DismountSeconds { get { return 4.0; } }
        public override int MountDamages { get { return 10; } }
        public override int RiderDamages { get { return 5; } }

        public override Type DeedType { get { return typeof( MediumDismountTrapDeed ); } }

        public MediumDismountTrap( TownSystem system, Mobile owner )
            : base( system, owner )
        {
        }

        public MediumDismountTrap( Serial serial )
            : base( serial )
        {
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

    public class HeavyDismountTrap : BaseDismountTrap
    {
        public override TrapLevel Level { get { return TrapLevel.Heavy; } }
        public override TimeSpan DecayPeriod { get { return TimeSpan.FromDays( 3.0 ); } }
        public override double DismountSeconds { get { return 5.0; } }
        public override int MountDamages { get { return 20; } }
        public override int RiderDamages { get { return 10; } }

        public override Type DeedType { get { return typeof( HeavyDismountTrapDeed ); } }

        public HeavyDismountTrap( TownSystem system, Mobile owner )
            : base( system, owner )
        {
        }

        public HeavyDismountTrap( Serial serial )
            : base( serial )
        {
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

    public class LightDismountTrapDeed : BaseCraftableTrapDeed
    {
        public override Type TrapType { get { return typeof( LightDismountTrap ); } }

        [Constructable]
        public LightDismountTrapDeed()
        {
            Name = "Light Dismount Trap Deed";
        }

        public LightDismountTrapDeed( Serial serial )
            : base( serial )
        {
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

    public class MediumDismountTrapDeed : BaseCraftableTrapDeed
    {
        public override Type TrapType { get { return typeof( MediumDismountTrap ); } }

        [Constructable]
        public MediumDismountTrapDeed()
        {
            Name = "Medium Dismount Trap Deed";
        }

        public MediumDismountTrapDeed( Serial serial )
            : base( serial )
        {
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

    public class HeavyDismountTrapDeed : BaseCraftableTrapDeed
    {
        public override Type TrapType { get { return typeof( HeavyDismountTrap ); } }

        [Constructable]
        public HeavyDismountTrapDeed()
        {
            Name = "Heavy Dismount Trap Deed";
        }

        public HeavyDismountTrapDeed( Serial serial )
            : base( serial )
        {
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
