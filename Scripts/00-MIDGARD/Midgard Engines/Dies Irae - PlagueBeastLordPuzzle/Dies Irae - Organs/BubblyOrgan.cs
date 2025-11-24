using System.Collections.Generic;
using Server;

namespace Midgard.Engines.PlagueBeastLordPuzzle
{
    public class BubblyOrgan : BaseOrgan
    {
        private DecoItem m_CuttedTissue;
        private int m_CutVeinsCount;

        [Constructable]
        public BubblyOrgan( BrainTypes type )
            : base( 0x342D, 1066115, m_VeinsColors[ Utility.Random( 7 ) ], type ) // an organ
        {
            m_CutVeinsCount = 0;
        }

        #region members
        public override void CreateTissue()
        {
            List<Vein> veins = new List<Vein>();
            bool oneIsPrincipal = false;
            int random = Utility.Random( 7 );

            for( int i = 0; i < 4; i++ )
            {
                Vein vein = new Vein( this, m_VeinsColors[ ( random + i ) % 4 ], m_VeinsFlipped[ i ] ? 1 : 0 );
                PutGuts( vein, m_VeinsOffset[ 2 * i ], m_VeinsOffset[ ( 2 * i ) + 1 ] );
                veins.Add( vein );
                if( vein.Hue == Hue )
                    oneIsPrincipal = true;
            }

            if( !oneIsPrincipal )
                veins[ Utility.Random( veins.Count ) ].Hue = Hue;

            m_CuttedTissue = new DecoItem( 0x122A, 1066108, 1, false ); // cutted tissue
            PutGuts( m_CuttedTissue, 1, 13 );

            Brain = new Gland( BrainType, this );
            PutGuts( Brain, 0, 20 );

            Gland gland = Brain as Gland;
            gland.CreateTissue( -4, -5 );
        }

        public override void OpenOrganTo( Mobile from )
        {
            if( m_CutVeinsCount < 3 )
                SendLocalizedMessageTo( from, 1066147 ); // * You cannot proceed. There are too veins around that organ *
            else
            {
                PuzzlePlagueBeastLord pbl = RootParent as PuzzlePlagueBeastLord;
                if( pbl == null || pbl.Deleted || pbl.Map == Map.Internal )
                    return;

                from.RevealingAction();
                from.Direction = from.GetDirectionTo( pbl );

                if( IsOpened )
                {
                    SendLocalizedMessageTo( from, 1066121 ); // * This organ was already opened *
                }
                else
                {
                    SendLocalizedMessageTo( from, 1066122 ); // * You open the plague beast's organ *
                    from.PlaySound( 0x2AC );

                    m_CuttedTissue.Visible = true;
                    IsOpened = true;

                    if( Brain == null || Brain.Deleted )
                        return;

                    Gland gland = Brain as Gland;
                    if( gland == null || gland.Deleted )
                        return;

                    if( BrainType == BrainTypes.None )
                        gland.MakeBleed( from );
                    else
                        Gland.RevealGland( gland );
                }
            }
        }

        public void OnVeinCut( Mobile from, int hue )
        {
            from.PlaySound( 0x248 );

            if( hue != Hue )
            {
                SendLocalizedMessageTo( from, 1066148 ); // * You removed a little secondary vein *
                ++m_CutVeinsCount;
            }
            else
            {
                SendLocalizedMessageTo( from, 1066149 ); // * You removed the principal vein and the organ withers *
                PuzzlePlagueBeastLord pbl2 = RootParent as PuzzlePlagueBeastLord;
                if( pbl2 != null && !pbl2.Deleted && pbl2.Map != Map.Internal )
                    pbl2.Kill();
            }
        }
        #endregion

        #region lists
        private static int[] m_VeinsColors = new int[] { 0x0492, 0x00E9, 0x0107, 0x0392, 0x0487, 0x048A, 0x0494 };

        private static int[] m_VeinsOffset = new int[]
		{
			-19, 3,
			23, -2,
			26, 24, 
			18, 45,
		};

        private static bool[] m_VeinsFlipped = new bool[] { false, true, false, false };
        #endregion

        #region serial-deserial
        public BubblyOrgan( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( m_CutVeinsCount );
            writer.Write( m_CuttedTissue );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_CutVeinsCount = reader.ReadInt();
            m_CuttedTissue = (DecoItem)reader.ReadItem();

            if( m_CuttedTissue == null )
                Delete();
        }
        #endregion
    }
}