using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    [Flipable( 0x1947, 0x1948, 0x1947, 0x1949, 0x194A, 0x1949 )]
    public class BlessedStatue : Item, ISecurable
    {
        private static int m_MaxRegsPerDay = 5;

        private static Type[] m_ResourceTypes = new Type[]
                                                    {
                                                        typeof( BlackPearl ),
                                                        typeof( Bloodmoss ),
                                                        typeof( Garlic ),
                                                        typeof( Ginseng ),
                                                        typeof( MandrakeRoot ),
                                                        typeof( Nightshade ),
                                                        typeof( SpidersSilk ),
                                                        typeof( SulfurousAsh )
                                                    };

        private SecureLevel m_Level;

        private DateTime m_NextFill;
        private int[] m_ResourceAmounts = new int[ 0 ];

        [Constructable]
        public BlessedStatue()
            : base( 0x1947 )
        {
            Weight = 10; // weight 0 from itemdata makes me think its an addon...
            NextFill = DateTime.Now + TimeSpan.FromSeconds( 1 );
            LootType = LootType.Blessed;
        }

        public BlessedStatue( Serial serial )
            : base( serial )
        {
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime NextFill
        {
            get { return m_NextFill; }
            set { m_NextFill = value; }
        }

        #region ISecurable Members

        [CommandProperty( AccessLevel.GameMaster )]
        public SecureLevel Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

        #endregion

        public override void OnDoubleClick( Mobile from )
        {
            if( !from.InRange( GetWorldLocation(), 1 ) )
                from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
            else if( ItemID > 0x1948 )
                Gather( from );
            else
                from.SendMessage( "The Blessed Statue is not holding any reagents yet." );
        }

        private void Fill()
        {
            m_ResourceAmounts = new int[ m_ResourceTypes.Length ];
            for( int i = 0; i < m_ResourceAmounts.Length; ++i )
                m_ResourceAmounts[ i ] = Utility.Random( m_MaxRegsPerDay ) + 1;

            ItemID += 2;
        }

        private void Gather( Mobile from )
        {
            int amount = 0;
            Item reg;
            bool canHold = true;
            for( int i = 0; i < m_ResourceAmounts.Length && i < m_ResourceTypes.Length; ++i )
            {
                if( m_ResourceAmounts[ i ] > 0 && canHold )
                {
                    reg =
                        Activator.CreateInstance( m_ResourceTypes[ i ], new object[] { m_ResourceAmounts[ i ] } ) as Item;

                    if( !from.PlaceInBackpack( reg ) )
                    {
                        canHold = false;
                        if( reg != null ) 
                            reg.Delete();
                    }
                    else
                        amount += m_ResourceAmounts[ i ];
                }
                m_ResourceAmounts[ i ] = 0;
            }

            if( amount == 1 )
                from.SendMessage( "You take a reagent from the statue and clean it." );
            else if( amount > 1 )
                from.SendMessage( "You gather {0} reagents from the statue and clean it.", amount );
            else
                from.SendMessage( "You remove the reagents from the statue." );

            NextFill = DateTime.Now + TimeSpan.FromDays( 1 );

            ItemID = ItemID == 0x1949 ? 0x1947 : 0x1948;
        }

        public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
        {
            base.GetContextMenuEntries( from, list );
            SetSecureLevelEntry.AddTo( from, this, list );
        }

        public override void Serialize( GenericWriter writer )
        {
            if( !Deleted && ItemID < 0x1949 && DateTime.Now > NextFill )
                Fill();

            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( m_ResourceAmounts.Length );
            for( int i = 0; i < m_ResourceAmounts.Length; ++i )
                writer.Write( m_ResourceAmounts[ i ] );

            writer.Write( m_NextFill );
            writer.WriteEncodedInt( (int)m_Level );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_ResourceAmounts = new int[ reader.ReadInt() ];
            for( int i = 0; i < m_ResourceAmounts.Length; ++i )
                m_ResourceAmounts[ i ] = reader.ReadInt();

            m_NextFill = reader.ReadDateTime();
            m_Level = (SecureLevel)reader.ReadEncodedInt();
        }
    }
}