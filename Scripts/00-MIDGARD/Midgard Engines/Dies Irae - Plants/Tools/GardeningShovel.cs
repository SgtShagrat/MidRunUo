using Server;
using Server.Items;
using Server.Targeting;

namespace Midgard.Engines.PlantSystem
{
    public class GardeningShovel : Item, IUsesRemaining
    {
        public override string DefaultName
        {
            get
            {
                return "Gardening Shovel";
            }
        }

        public bool ShowUsesRemaining { get { return true; } set { } }

        private int m_UsesRemaining;

        [CommandProperty( AccessLevel.GameMaster )]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
        }

        public virtual void UnscaleUses()
        {
            int scale = 100 + GetDurabilityBonus();
            m_UsesRemaining = ( ( m_UsesRemaining * 100 ) + ( scale - 1 ) ) / scale;
        }

        public virtual void ScaleUses()
        {
            int scale = 100 + GetDurabilityBonus();
            m_UsesRemaining = ( ( m_UsesRemaining * scale ) + 99 ) / 100;
        }

        public int GetDurabilityBonus()
        {
            return 0;
        }

        private static readonly DiceRoll m_DefaultUsesDice = new DiceRoll( 1, 10, 50 ); // 1d10+50

        public static int GetDefaultUses()
        {
            return m_DefaultUsesDice.Roll();
        }

        public void ConsumeCharges( Mobile from )
        {
            --UsesRemaining;

            if( UsesRemaining <= 0 )
            {
                if( from != null )
                    from.SendMessage( "You have used up your shovel." );
                Delete();
            }
        }

        [Constructable]
        public GardeningShovel()
            : this( GetDefaultUses() )
        {
        }

        [Constructable]
        public GardeningShovel( int uses )
            : base( 0xF39 )
        {
            m_UsesRemaining = uses;

            Weight = 1.0;
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( IsChildOf( from.Backpack ) )
            {
                if( UsesRemaining <= 0 )
                {
                    from.SendMessage( "You have used up your shovel." );
                    Delete();
                }
                else
                {
                    from.SendMessage( "Select the plant thou want to examinate." );
                    from.Target = new InternalTarget( this );
                }
            }
            else
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
        }

        private class InternalTarget : Target
        {
            private GardeningShovel m_Shovel;

            public InternalTarget( GardeningShovel shovel )
                : base( 2, true, TargetFlags.None )
            {
                m_Shovel = shovel;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( targeted is BasePlant )
                {
                    if( BasePlant.CheckAccess( from, (BasePlant)targeted ) )
                    {
                        from.SendGump( new PlantInfoGump( (BasePlant)targeted, from ) );
                        m_Shovel.ConsumeCharges( from );
                    }
                }
                else
                {
                    from.SendLocalizedMessage( 1049083 ); // You cannot use the powder on that item.
                }
            }
        }

        #region serial-deserial
        public GardeningShovel( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 1 ); // version

            writer.Write( m_UsesRemaining );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    {
                        m_UsesRemaining = reader.ReadInt();
                        break;
                    }
                case 0:
                    {
                        m_UsesRemaining = 50;
                        break;
                    }
            }
        }
        #endregion
    }
}