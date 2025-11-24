/***************************************************************************
 *                                  GasTraps.cs
 *                            		-------------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using Midgard.Engines.MidgardTownSystem;
using Server;

namespace Midgard.Items
{
    public abstract class BaseGasTrap : BaseCraftableTrap
    {
        public override int AttackMessage { get { return 1010542; } } // A noxious green cloud of poison gas envelops you!
        public override int DisarmMessage { get { return 502376; } } // The poison leaks harmlessly away due to your deft touch.
        public override int EffectSound { get { return 0x230; } }
        public override int MessageHue { get { return 0x44; } }

        public abstract Poison PoisonLevel { get; }

        public override void DoVisibleEffect()
        {
            Effects.SendLocationEffect( Location, Map, 0x3709, 28, 10, 0x1D3, 5 );
        }

        public override void DoAttackEffect( Mobile m )
        {
            m.ApplyPoison( m, PoisonLevel );
        }

        public BaseGasTrap( TownSystem system, Mobile owner )
            : base( system, owner, 0x113C )
        {
        }

        public BaseGasTrap( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class LightGasTrap : BaseGasTrap
    {
        public override TrapLevel Level { get { return TrapLevel.Light; } }
        public override TimeSpan DecayPeriod { get { return TimeSpan.FromDays( 1.0 ); } }
        public override Poison PoisonLevel { get { return Poison.Lesser; } }

        public override Type DeedType { get { return typeof( LightGasTrapDeed ); } }

        public LightGasTrap( TownSystem system, Mobile owner )
            : base( system, owner )
        {
        }

        public LightGasTrap( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class MediumGasTrap : BaseGasTrap
    {
        public override TrapLevel Level { get { return TrapLevel.Medium; } }
        public override TimeSpan DecayPeriod { get { return TimeSpan.FromDays( 2.0 ); } }
        public override Poison PoisonLevel { get { return Poison.Regular; } }

        public override Type DeedType { get { return typeof( MediumGasTrapDeed ); } }

        public MediumGasTrap( TownSystem system, Mobile owner )
            : base( system, owner )
        {
        }

        public MediumGasTrap( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class HeavyGasTrap : BaseGasTrap
    {
        public override TrapLevel Level { get { return TrapLevel.Heavy; } }
        public override TimeSpan DecayPeriod { get { return TimeSpan.FromDays( 3.0 ); } }
        public override Poison PoisonLevel { get { return Poison.Greater; } }

        public override Type DeedType { get { return typeof( HeavyGasTrapDeed ); } }

        public HeavyGasTrap( TownSystem system, Mobile owner )
            : base( system, owner )
        {
        }

        public HeavyGasTrap( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class LightGasTrapDeed : BaseCraftableTrapDeed
    {
        public override Type TrapType { get { return typeof( LightGasTrap ); } }

        [Constructable]
        public LightGasTrapDeed()
        {
            Name = "Light Gas Trap Deed";
        }

        public LightGasTrapDeed( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class MediumGasTrapDeed : BaseCraftableTrapDeed
    {
        public override Type TrapType { get { return typeof( MediumGasTrap ); } }

        [Constructable]
        public MediumGasTrapDeed()
        {
            Name = "Medium Gas Trap Deed";
        }

        public MediumGasTrapDeed( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }

    public class HeavyGasTrapDeed : BaseCraftableTrapDeed
    {
        public override Type TrapType { get { return typeof( HeavyGasTrap ); } }

        [Constructable]
        public HeavyGasTrapDeed()
        {
            Name = "Heavy Gas Trap Deed";
        }

        public HeavyGasTrapDeed( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}
