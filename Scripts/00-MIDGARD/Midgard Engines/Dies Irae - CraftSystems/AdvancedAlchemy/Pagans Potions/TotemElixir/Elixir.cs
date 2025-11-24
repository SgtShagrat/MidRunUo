/***************************************************************************
 *                               Dies Irae - Elixir.cs
 *                            -------------------
 *   begin                : 24 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Midgard.Items
{
    public class Elixir : BasePaganPotion
    {
        public override int Strength
        {
            get { return 1; }
        }

        public override double AlchemyRequiredToDrink
        {
            get { return 100.0; }
        }

        public override int CustomSound
        {
            get { return 0x1fe; }
        }

        public override int CustomAnim
        {
            get { return 0x0014; }
        }

        public override int CustomEffects
        {
            get { return 0x3728; }
        }

        [Constructable]
        public Elixir( int amount )
            : base( PotionEffect.Elixir, amount )
        {
            // Name = "Elixir";
            Hue = 2616;
        }

        [Constructable]
        public Elixir()
            : this( 1 )
        {
        }

        public Elixir( Serial serial )
            : base( serial )
        {
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !IsIdentifiedFor( from ) )
                from.SendMessage( "You don't know how to use this potion." );

            from.SendMessage( "Target your totem to rebind it." );
            from.Target = new InternalTarget( this );
        }

        public override bool DoPaganEffect( Mobile from )
        {
            return true;
        }

        #region serial deserial
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
        #endregion

        private class InternalTarget : Target
        {
            private readonly Elixir m_Owner;

            public InternalTarget( Elixir owner )
                : base( 10, false, TargetFlags.None )
            {
                m_Owner = owner;
            }

            protected override void OnTarget( Mobile from, object o )
            {
                if( o is TotemCreature )
                {
                    m_Owner.Target( from, (TotemCreature)o );
                }
            }
        }

        public void Target( Mobile from, TotemCreature o )
        {
            if( o.GetMaster() == from )
            {
                o.StartRebind( from );
            }
        }
    }
}