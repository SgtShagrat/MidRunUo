/***************************************************************************
 *                               DecorationItem.cs
 *                            -----------------------
 *   begin                : 07 novembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/
 
using System;
using Server;
using Server.Items;

namespace Midgard.Engines.OrderChaosWars
{
    public class DecorationItem : Static
    {
        private Virtue m_DecoVirtue;

        public DecorationItem()
            : this( 0x80 )
        {
        }

        [Constructable]
        public DecorationItem( int itemID )
            : base( itemID )
        {
            Battle = BattleType.None;

            m_DecoVirtue = Virtue.None;
        }

        [CommandProperty( AccessLevel.Counselor, AccessLevel.Seer )]
        public Virtue DecoVirtue
        {
            get { return m_DecoVirtue; }
            set
            {
                Virtue oldValue = m_DecoVirtue;
                if( oldValue != value )
                {
                    m_DecoVirtue = value;
                    Hue = Core.GetHue( m_DecoVirtue );
                }
            }
        }

        [CommandProperty( AccessLevel.Counselor, AccessLevel.Seer )]
        public BattleType Battle { get; set; }

        public static void RegisterDeco_Callback( object state )
        {
            DecorationItem deco = (DecorationItem)state;
            if( deco == null )
                return;

            Core.Instance.RegisterDeco( deco );
        }

        #region serialization
        public DecorationItem( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( (int)m_DecoVirtue );
            writer.Write( (int)Battle );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_DecoVirtue = (Virtue)reader.ReadInt();
            Battle = (BattleType)reader.ReadInt();

            if( Battle != BattleType.None )
                Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( RegisterDeco_Callback ), this );
        }
        #endregion
    }
}