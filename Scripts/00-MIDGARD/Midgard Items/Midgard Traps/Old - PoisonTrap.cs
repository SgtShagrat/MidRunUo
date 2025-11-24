using System;

using Server.Spells;

namespace Server.Items
{
    public class PoisonTrap : BaseTrap
    {
        [Constructable]
        public PoisonTrap() : base( 0x1B71 )
        {
        }

        public override bool PassivelyTriggered
        {
            get { return true; }
        }

        public override TimeSpan PassiveTriggerDelay
        {
            get { return TimeSpan.FromSeconds( 2.0 ); }
        }

        public override int PassiveTriggerRange
        {
            get { return 3; }
        }

        public override TimeSpan ResetDelay
        {
            get { return TimeSpan.FromSeconds( 0.5 ); }
        }

        public override void OnTrigger( Mobile from )
        {
            Effects.SendLocationParticles( EffectItem.Create( Location, Map, EffectItem.DefaultDuration ), 0x113a, 10, 30, 5052 );
            Effects.PlaySound( Location, Map, 0x1DE );

            if( from.Alive && from.Location == Location )
            {
                SpellHelper.Damage( TimeSpan.FromSeconds( 0.5 ), from, Utility.RandomMinMax( 10, 15 ), 0, 100, 0, 0, 0 );
                from.Poison = Poison.Lethal;
            }
        }

        public PoisonTrap( Serial serial ) : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}