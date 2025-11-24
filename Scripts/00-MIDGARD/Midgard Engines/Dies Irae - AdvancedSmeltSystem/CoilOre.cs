/***************************************************************************
 *                              Coil.cs
 *                            -----------
 *   begin                : 02 gennaio, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using Server;
using Server.Items;
using Server.Targeting;

namespace Midgard.Engines.AdvancedSmelting
{
    public class CoilOre : Item, ICommodity
    {
        public override double DefaultWeight
        {
            get { return 1.0; }
        }

        string ICommodity.Description
        {
            get
            {
                return String.Format( "Coil (Amount: {0})", Amount );
            }
        }

        int ICommodity.DescriptionNumber { get { return 0; } }

        public override string DefaultName { get { return "Coil"; } }

        [Constructable]
        public CoilOre( int amount )
        : base( 0x19B9 )
        {
            Stackable = true;
            Amount = amount;
            Hue = 0x497;
        }

        [Constructable]
        public CoilOre()
        : this( 1 )
        {
        }

        public override void AddNameProperty( ObjectPropertyList list )
        {
            if( Amount > 1 )
                list.Add( 1050039, "Coil (Amount:\t{0})", Amount ); // ~1_NUMBER~ ~2_ITEMNAME~
            else
                list.Add( "Coil" );
        }

        public override void OnSingleClick( Mobile from )
        {
            LabelTo( from, Amount == 1 ? "{0} coil" : "{0} coil pieces" , Amount );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !Movable )
                return;

            if( from.InRange( GetWorldLocation(), 2 ) )
            {
                from.SendMessage( "Select the forge to put this coil into." );
                from.Target = new InternalTarget( this );
            }
            else
            {
                from.SendLocalizedMessage( 501976 ); // The ore is too far away.
            }
        }

        private class InternalTarget : Target
        {
            private readonly CoilOre m_CoilOre;

            public InternalTarget( CoilOre coilOre )
            : base( 2, false, TargetFlags.None )
            {
                m_CoilOre = coilOre;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_CoilOre.Deleted )
                    return;

                if( !from.InRange( m_CoilOre.GetWorldLocation(), 2 ) )
                {
                    from.SendLocalizedMessage( 501976 ); // The ore is too far away.
                    return;
                }

                if( targeted is AdvancedForge )
                {
                    ( (AdvancedForge)targeted ).CheckAddResource( m_CoilOre );
                }
            }
        }

        #region serialization
        public CoilOre( Serial serial )
        : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)1 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}