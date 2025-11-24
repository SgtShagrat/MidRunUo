/***************************************************************************
 *                               VirtueChampionGem.cs
 *
 *   begin                : 09 giugno 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;

namespace Midgard.Engines.Classes.VirtueChampion
{
    class VirtueChampionGem : Item
    {
        private bool m_Purified;

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Purified
        {
            get { return m_Purified; }
            set
            {
                if( value != m_Purified )
                {
                    bool oldValue = m_Purified;
                    OnPurifiedChanged( oldValue );
                    m_Purified = value;
                }
            }
        }

        private Virtues m_Virtue;

        [CommandProperty( AccessLevel.GameMaster )]
        public Virtues Virtue
        {
            get { return m_Virtue; }
            set
            {
                if( value != m_Virtue )
                {
                    Virtues oldValue = m_Virtue;
                    OnVirtueChanged( oldValue );
                    m_Virtue = value;
                }
            }
        }

        private void OnVirtueChanged( Virtues oldValue )
        {
            UpdateItemID( oldValue );
        }

        private void UpdateItemID( Virtues oldValue )
        {
            int itemID = 0;

            switch( m_Virtue )
            {
                case Virtues.Humility: itemID = 0x186E; break;
                case Virtues.Sacrifice: itemID = 0x186E; break;
                case Virtues.Compassion: itemID = 0x186E; break;
                case Virtues.Spirituality: itemID = 0x186E; break;
                case Virtues.Valor: itemID = 0x186E; break;
                case Virtues.Honor: itemID = 0x186E; break;
                case Virtues.Justice: itemID = 0x186E; break;
                case Virtues.Honesty: itemID = 0x186E; break;
            }

            ItemID = itemID;
        }

        private void OnPurifiedChanged( bool oldValue )
        {
            Hue = !oldValue ? 0x000 : 0x483;
        }

        public override void OnSingleClick( Mobile from )
        {
            if( Purified )
                LabelTo( from, "a purified gem of {0}", Enum.GetName( typeof( Virtues ), m_Virtue ).ToLower() );
            else
                LabelTo( from, "a corrupted gem of {0}", Enum.GetName( typeof( Virtues ), m_Virtue ).ToLower() );
        }

        [Constructable]
        public VirtueChampionGem( Virtues virtue, bool purified )
            : this( virtue )
        {
            m_Purified = purified;
        }

        [Constructable]
        public VirtueChampionGem( Virtues virtue )
            : base( 0x186E )
        {
            m_Virtue = virtue;
            m_Purified = false;

            Weight = 10.0;
            LootType = LootType.Blessed;
        }

        [Constructable]
        public VirtueChampionGem()
            : this( Virtues.Compassion )
        {
        }

        #region serialization
        public VirtueChampionGem( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( m_Purified );
            writer.Write( (int)m_Virtue );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Purified = reader.ReadBool();
            m_Virtue = (Virtues)reader.ReadInt();
        }
        #endregion
    }
}
