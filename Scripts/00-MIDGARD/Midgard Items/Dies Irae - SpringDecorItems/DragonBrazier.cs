using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public class DragonBrazier : BaseLight, ISecurable
    {
        private SecureLevel m_Level;

        [Constructable]
        public DragonBrazier() : base( 0x194D )
        {
            LootType = LootType.Blessed;
            Weight = 10;
            Light = LightType.Circle150;
            Burning = true;
        }

        public DragonBrazier( Serial serial ) : base( serial )
        {
        }

        public override int LitItemID
        {
            get { return 0x194D; }
        }

        public override int UnlitItemID
        {
            get { return 0x194E; }
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
            else
                base.OnDoubleClick( from );
        }

        public override void GetContextMenuEntries( Mobile from, List< ContextMenuEntry > list )
        {
            base.GetContextMenuEntries( from, list );
            SetSecureLevelEntry.AddTo( from, this, list );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.WriteEncodedInt( (int)m_Level );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Level = (SecureLevel)reader.ReadEncodedInt();
        }
    }
}