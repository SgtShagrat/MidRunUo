using System;
using System.Collections.Generic;
using Server.Items;
using Server;
using Server.Spells;

namespace Midgard.Items
{
    public class BaseCustomSpellScroll : SpellScroll, IIdentificable
    {
        public BaseCustomSpellScroll( int index, int itemID, int amount )
            : base( index, itemID, amount )
        {
            m_IdentifiersList = new List<Mobile>();
        }

        public override bool StackWith( Mobile from, Item dropped, bool playSound )
        {
            if( dropped is BaseCustomSpellScroll && !( (BaseCustomSpellScroll)dropped ).IsIdentifiedFor( from ) )
                return false;

            return base.StackWith( from, dropped, playSound );
        }

        public override void OnSingleClick( Mobile from )
        {
            if( !IsIdentifiedFor( from ) )
                LabelTo( from, "a magical scroll" );
            else
            {
                string name = GetName();

                if( name != null )
                    LabelTo( from, "a scroll of {0}", name );
            }

            base.OnSingleClick( from );
        }

        public string GetName()
        {
            string name = null;

            SpellInfo info = SpellRegistry.GetSpellInfoByID( SpellID );
            if( info != null )
                name = info.Name;

            return name;
        }

        #region serialization
        public BaseCustomSpellScroll( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 1 ); // version

            writer.Write( m_IdentifiersList != null );
            if( m_IdentifiersList != null )
                writer.Write( m_IdentifiersList, true );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    if( reader.ReadBool() )
                        m_IdentifiersList = reader.ReadStrongMobileList();
                    goto case 0;
                case 0:
                    if( version < 1 )
                        reader.ReadBool();
                    break;
            }
        }
        #endregion

        #region IIdentificable members
        private List<Mobile> m_IdentifiersList;

        private const int MaxIdentifiers = 5;

        public void CopyIdentifiersTo( IIdentificable identificable )
        {
            if( m_IdentifiersList != null && m_IdentifiersList.Count > 0 )
            {
                foreach( Mobile mobile in m_IdentifiersList )
                {
                    identificable.AddIdentifier( mobile );
                }
            }
        }

        public void ClearIdentifiers()
        {
            if( m_IdentifiersList != null && m_IdentifiersList.Count > 0 )
                m_IdentifiersList.Clear();
        }

        public void AddIdentifier( Mobile from )
        {
            if( m_IdentifiersList == null )
                m_IdentifiersList = new List<Mobile>();

            if( !m_IdentifiersList.Contains( from ) )
                m_IdentifiersList.Add( from );

            if( m_IdentifiersList.Count > MaxIdentifiers )
                m_IdentifiersList.RemoveAt( 0 );
        }

        public bool IsIdentifiedFor( Mobile from )
        {
            if( m_IdentifiersList == null )
                return false;

            return m_IdentifiersList.Contains( from );
        }

        public void DisplayItemInfo( Mobile from )
        {
            if( IsIdentifiedFor( from ) )
                from.SendMessage( "You already know what kind of scroll that is." );
            else
                OnSingleClick( from );
        }

        public void InvalidateSecondAgeNames()
        {
        }
        #endregion
    }
}