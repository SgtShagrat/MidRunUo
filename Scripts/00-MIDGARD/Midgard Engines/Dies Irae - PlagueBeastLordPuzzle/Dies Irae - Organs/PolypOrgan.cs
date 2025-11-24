using Server;

namespace Midgard.Engines.PlagueBeastLordPuzzle
{
    public class PolypOrgan : BaseOrgan
    {
        private DecoItem m_CuttedTissue;

        public PolypOrgan( BrainTypes type )
            : base( 0x1772, 1066115, 0x66B, type ) // an organ
        {
        }

        #region members
        public override void CreateTissue()
        {
            PutGuts( new DecoItem( 0x1EB1, 1066123, Hue ), -25, 32 ); // sustenance cannulas
            PutGuts( new DecoItem( 0x1EB2, 1066123, Hue ), 20, 32 ); // sustenance cannulas

            m_CuttedTissue = new DecoItem( 0x122A, 1066108, 1, false ); // cutted tissue
            PutGuts( m_CuttedTissue, 1, 13 );

            Brain = new Gland( BrainType, this );
            PutGuts( Brain, 0, 20 );

            Gland brain = Brain as Gland;
            brain.CreateTissue( -4, -5 );
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

                m_CuttedTissue.Visible = true;
                IsOpened = true;

                if( Brain == null || Brain.Deleted )
                    return;

                if( BrainType == BrainTypes.None )
                {
                    Gland brain = Brain as Gland;
                    if( brain != null )
                        brain.MakeBleed( from );
                }
            }
        }
        #endregion

        #region serial-deserial
        public PolypOrgan( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( m_CuttedTissue );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_CuttedTissue = (DecoItem)reader.ReadItem();

            if( m_CuttedTissue == null )
                Delete();
        }
        #endregion
    }
}