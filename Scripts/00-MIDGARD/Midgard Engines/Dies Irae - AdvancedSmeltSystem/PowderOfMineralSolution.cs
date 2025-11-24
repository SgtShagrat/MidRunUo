/***************************************************************************
 *                               PowderOfMineralSolution.cs
 *
 *   begin                : 06 agosto, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Items;
using Server.Network;
using Server.Targeting;

namespace Midgard.Engines.AdvancedSmelting
{
    public class PowderOfMineralSolution : Item, IUsesRemaining
    {
        private int m_UsesRemaining;

        [CommandProperty( AccessLevel.GameMaster )]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
        }

        public bool ShowUsesRemaining { get { return true; } set { } }

        public override string DefaultName
        {
            get
            {
                return "Powder of Mineral Solution";
            }
        }

        [Constructable]
        public PowderOfMineralSolution()
            : this( 10 )
        {
        }

        [Constructable]
        public PowderOfMineralSolution( int charges )
            : base( 4102 )
        {
            Weight = 1.0;
            Hue = 2419;
            UsesRemaining = charges;
        }

        public PowderOfMineralSolution( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
            writer.Write( (int)m_UsesRemaining );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        m_UsesRemaining = reader.ReadInt();
                        break;
                    }
            }
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( 1060584, m_UsesRemaining.ToString() ); // uses remaining: ~1_val~
        }

        public virtual void DisplayDurabilityTo( Mobile m )
        {
            LabelToAffix( m, 1017323, AffixType.Append, ": " + m_UsesRemaining ); // Durability
        }

        public override void OnSingleClick( Mobile from )
        {
            DisplayDurabilityTo( from );

            base.OnSingleClick( from );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( IsChildOf( from.Backpack ) )
            {
                from.SendMessage( "Select the forge you want to clean." );
                from.Target = new InternalTarget( this );
            }
            else
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
        }

        private class InternalTarget : Target
        {
            private PowderOfMineralSolution m_Powder;

            public InternalTarget( PowderOfMineralSolution powder )
                : base( 2, false, TargetFlags.None )
            {
                m_Powder = powder;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_Powder.Deleted || m_Powder.UsesRemaining <= 0 )
                {
                    from.SendMessage( "You have used up your powder." );
                    return;
                }

                if( targeted is AdvancedForge )
                {
                    from.SendMessage( "The forge is now empty." );
                    ( (AdvancedForge)targeted ).ClearForge();

                    --m_Powder.UsesRemaining;

                    if( m_Powder.UsesRemaining <= 0 )
                    {
                        from.SendMessage( "You have used up your powder." );
                        m_Powder.Delete();
                    }
                }
                else
                {
                    from.SendLocalizedMessage( 1049083 ); // You cannot use the powder on that item.
                }
            }
        }
    }
}