using System;
using Server;
using Server.Mobiles;

namespace Midgard.Engines.MidgardTownSystem
{
    public class TownCharacterStatue : CharacterStatue
    {
        private TownSystem m_System;
        private string m_EngravedText;

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public TownSystem System
        {
            get { return m_System; }
            set { m_System = value; }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public string EngravedText
        {
            get { return m_EngravedText; }
            set
            {
                m_EngravedText = value;

                InvalidateProperties();
            }
        }

        public TownCharacterStatue( Mobile from, StatueType type )
            : base( from, type )
        {
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            AddNameProperties( list );

            if( m_System != null && SculptedBy != null )
            {
                list.Add( 1070722, "WarLord of {0}", m_System.Definition.TownName );

                if( !String.IsNullOrEmpty( m_EngravedText ) )
                    list.Add( 1042971, m_EngravedText );
            }
        }

        #region serialization
        public TownCharacterStatue( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( (int)1 ); // version

            // Version 1
            writer.Write( m_EngravedText );

            // Version 0
            if( m_System != null )
                writer.Write( (int)m_System.Definition.Town );
            else
                writer.Write( (int)MidgardTowns.None );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();

            switch( version )
            {
                case 1:
                    {
                        m_EngravedText = reader.ReadString();
                        goto case 0;
                    }
                case 0:
                    {
                        m_System = TownSystem.Find( (MidgardTowns)reader.ReadInt() );
                        break;
                    }
            }
        }
        #endregion
    }
}