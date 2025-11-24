/***************************************************************************
 *                               DecorationItem.cs
 *
 *   begin                : 20 febbraio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using Server;
using Server.Items;

namespace Midgard.Engines.WarSystem
{
    public class DecorationItem : Static
    {
        private WarTeam m_DecoTeam;

        public DecorationItem()
            : this( 0x80 )
        {
        }

        [Constructable]
        public DecorationItem( int itemID )
            : base( itemID )
        {
            Battle = BattleType.None;

            m_DecoTeam = null;
        }

        [CommandProperty( AccessLevel.Counselor, AccessLevel.Seer )]
        public WarTeam DecoTeam
        {
            get { return m_DecoTeam; }
            set
            {
                WarTeam oldValue = m_DecoTeam;
                if( oldValue != value )
                {
                    m_DecoTeam = value;
                    Hue = Core.GetHue( m_DecoTeam );
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

            writer.Write( 0 ); // version

            // TODO write reference
            // writer.Write( (int)m_DecoTeam );

            writer.Write( (int)Battle );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            // TODO write reference
            // m_DecoTeam = (Virtue)reader.ReadInt();

            Battle = (BattleType)reader.ReadInt();

            if( Battle != BattleType.None )
                Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( RegisterDeco_Callback ), this );
        }
        #endregion
    }
}