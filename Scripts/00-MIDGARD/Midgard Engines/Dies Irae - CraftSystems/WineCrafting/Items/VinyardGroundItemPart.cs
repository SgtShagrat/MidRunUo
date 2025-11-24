namespace Midgard.Engines.WineCrafting
{
    public class VinyardGroundItemPart
    {
        private int m_ItemID;
        private VinyardGroundPosition m_Info;
        private int m_OffsetX;
        private int m_OffsetY;

        public int ItemID
        {
            get { return m_ItemID; }
        }

        public VinyardGroundPosition VinyardGroundPosition
        {
            get { return m_Info; }
        }

        // For Gump Rendering
        public int OffsetX
        {
            get { return m_OffsetX; }
        }

        // For Gump Rendering
        public int OffsetY
        {
            get { return m_OffsetY; }
        }

        public VinyardGroundItemPart( int itemID, VinyardGroundPosition info, int offsetX, int offsetY )
        {
            m_ItemID = itemID;
            m_Info = info;
            m_OffsetX = offsetX;
            m_OffsetY = offsetY;
        }
    }
}