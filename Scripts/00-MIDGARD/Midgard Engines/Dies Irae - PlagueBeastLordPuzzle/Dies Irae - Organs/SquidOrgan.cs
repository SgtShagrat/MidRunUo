using Server;

namespace Midgard.Engines.PlagueBeastLordPuzzle
{
    public class SquidOrgan : BaseOrgan
    {
        #region fields
        private DecoItem m_Suborgan;
        private DecoItem m_CuttedTissue;
        private DecoItem m_Tissue;
        private DecoItem m_VeinI;
        private DecoItem m_VeinII;
        private DecoItem m_VeinIII;
        private DiseasedGland m_GreenGland;
        private bool m_GlandIsReplaced;
        #endregion

        #region constructors
        public SquidOrgan( BrainTypes type )
            : base( 0x1362, 1066115, 0x26B, type ) // an organ
        {
            m_GlandIsReplaced = false;
        }

        public SquidOrgan( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region members
        public override void CreateTissue()
        {
            m_Tissue = new DecoItem( 0x122A, 1066124, 0x25A, false ); // periglandular tissue
            PutGuts( m_Tissue, -2, 8 );

            m_VeinI = new DecoItem( 0x1B1B, 1066126, 0x4F7 ); // a vein
            PutGuts( m_VeinI, 24, 22 );

            m_VeinII = new DecoItem( 0x1B1B, 1066126, 0x4F7 ); // a vein
            PutGuts( m_VeinII, 18, 29 );

            m_VeinIII = new DecoItem( 0x1B1B, 1066126, 0x4F7 ); // a vein
            PutGuts( m_VeinIII, 14, 35 );

            m_Suborgan = new DecoItem( 0x1360, 1066117, 0x4F7 ); // gland casing
            PutGuts( m_Suborgan, 47, 35 );

            m_CuttedTissue = new DecoItem( 0x1B73, 1066108, 1, false ); // cutted tissue
            PutGuts( m_CuttedTissue, 38, 35 );

            m_GreenGland = new DiseasedGland( false, false );
            PutGuts( m_GreenGland, -3, 15 );

            Brain = new Gland( BrainType, this );
            PutGuts( Brain, 37, 37 );

            Gland gland = Brain as Gland;
            gland.CreateTissue( -4, -5 );
        }

        public override void OpenOrganTo( Mobile from )
        {
            PuzzlePlagueBeastLord pbl = RootParent as PuzzlePlagueBeastLord;
            if( pbl == null || pbl.Deleted || pbl.Map == Map.Internal )
                return;

            from.RevealingAction();
            from.Direction = from.GetDirectionTo( pbl );

            if( IsOpened )
            {
                SendLocalizedMessageTo( from, 1066121 ); // This organ was already opened.
            }
            else
            {
                SendLocalizedMessageTo( from, 1066122 ); // You open the plague beast's organ.
                from.PlaySound( 0x2AC );

                m_Tissue.Visible = true;
                m_GreenGland.Visible = true;
            }
        }

        public void OnGlandDrop( Mobile from, Item gland )
        {
            if( m_GreenGland.Parent == Parent )
                from.SendLocalizedMessage( 1066118 ); // Remove the diseased gland before you place the healthy one.
            else if( m_GlandIsReplaced )
                from.SendLocalizedMessage( 1066119 ); // * There already is a healthy gland *
            else if( !m_GlandIsReplaced && !m_GreenGland.IsChildOf( RootParent ) )
            {
                m_Suborgan.Hue = Hue;

                m_VeinI.Hue = Hue;
                m_VeinII.Hue = Hue;
                m_VeinIII.Hue = Hue;

                gland.Movable = false;
                m_GlandIsReplaced = true;
                m_CuttedTissue.Visible = true;

                IsOpened = true;

                SendLocalizedMessageTo( from, 1066120 ); // * You place the healthy gland on the gland casing *

                if( Brain == null || Brain.Deleted )
                    return;

                if( BrainType == BrainTypes.None ) //If there is no brain, the wound begins to bleed directly
                {
                    Gland brain = Brain as Gland;
                    if( brain != null )
                        brain.MakeBleed( from );
                }
            }
        }
        #endregion

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( m_CuttedTissue );
            writer.Write( m_Tissue );
            writer.Write( m_Suborgan );
            writer.Write( m_VeinI );
            writer.Write( m_VeinII );
            writer.Write( m_VeinIII );
            writer.Write( m_GreenGland );
            writer.Write( m_GlandIsReplaced );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_CuttedTissue = (DecoItem)reader.ReadItem();
            m_Tissue = (DecoItem)reader.ReadItem();
            m_Suborgan = (DecoItem)reader.ReadItem();
            m_VeinI = (DecoItem)reader.ReadItem();
            m_VeinII = (DecoItem)reader.ReadItem();
            m_VeinIII = (DecoItem)reader.ReadItem();
            m_GlandIsReplaced = reader.ReadBool();
            m_GreenGland = (DiseasedGland)reader.ReadItem();

            if( m_CuttedTissue == null || m_GreenGland == null || m_Tissue == null || m_Suborgan == null
                 || m_VeinI == null || m_VeinII == null || m_VeinIII == null )
                Delete();
        }
        #endregion
    }
}