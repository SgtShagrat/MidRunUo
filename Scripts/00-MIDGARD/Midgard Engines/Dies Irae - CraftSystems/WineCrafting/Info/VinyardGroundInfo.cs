namespace Midgard.Engines.WineCrafting
{
    public class VinyardGroundInfo
    {
        private VinyardGroundItemPart[] m_Entries;

        public VinyardGroundItemPart[] Entries { get { return m_Entries; } }

        public VinyardGroundInfo( VinyardGroundItemPart[] entries )
        {
            m_Entries = entries;
        }

        public VinyardGroundItemPart GetItemPart( VinyardGroundPosition pos )
        {
            int i = (int)pos;

            if( i < 0 || i >= m_Entries.Length )
                i = 0;

            return m_Entries[ i ];
        }

        public static VinyardGroundInfo GetInfo( int type )
        {
            if( type < 0 || type >= m_Infos.Length )
                type = 0;

            return m_Infos[ type ];
        }

        #region VinyardGroundInfo definitions
        private static VinyardGroundInfo[] m_Infos = new VinyardGroundInfo[] {
/* FurrowNoBorder */	new VinyardGroundInfo( new VinyardGroundItemPart[] { 
                    	                                                       new VinyardGroundItemPart( 0x32C9, VinyardGroundPosition.Top, -1, -1 ),
                    	                                                       new VinyardGroundItemPart( 0x32C9, VinyardGroundPosition.Bottom, -1, -1 ),
                    	                                                       new VinyardGroundItemPart( 0x32C9, VinyardGroundPosition.Left, -1, -1 ),
                    	                                                       new VinyardGroundItemPart( 0x32C9, VinyardGroundPosition.Right, -1, -1 ),
                    	                                                       new VinyardGroundItemPart( 0x32C9, VinyardGroundPosition.West, -1, -1 ),
                    	                                                       new VinyardGroundItemPart( 0x32C9, VinyardGroundPosition.North, -1, -1 ),
                    	                                                       new VinyardGroundItemPart( 0x32C9, VinyardGroundPosition.East, -1, -1 ),
                    	                                                       new VinyardGroundItemPart( 0x32C9, VinyardGroundPosition.South, -1, -1 ),
                    	                                                       new VinyardGroundItemPart( 0x32C9, VinyardGroundPosition.Center, 44, 24 )
                    	                                                   }),
/* FurrowBorder */		new VinyardGroundInfo( new VinyardGroundItemPart[] { 
                  		                                                       new VinyardGroundItemPart( 0x1B30, VinyardGroundPosition.Top, 44, 7 ),
                  		                                                       new VinyardGroundItemPart( 0x1B2F, VinyardGroundPosition.Bottom, 44, 68 ),
                  		                                                       new VinyardGroundItemPart( 0x1B31, VinyardGroundPosition.Left, 0, 35 ),
                  		                                                       new VinyardGroundItemPart( 0x1B32, VinyardGroundPosition.Right, 88, 32 ),
                  		                                                       new VinyardGroundItemPart( 0x1B29, VinyardGroundPosition.West, 22, 12 ),
                  		                                                       new VinyardGroundItemPart( 0x1B2A, VinyardGroundPosition.North, 66, 12 ),
                  		                                                       new VinyardGroundItemPart( 0x1B28, VinyardGroundPosition.East, 66, 46 ),
                  		                                                       new VinyardGroundItemPart( 0x1B27, VinyardGroundPosition.South, 22, 46 ),
                  		                                                       new VinyardGroundItemPart( 0x32C9, VinyardGroundPosition.Center, 44, 24 )
                  		                                                   }),
/* PlainNoBorder */		new VinyardGroundInfo( new VinyardGroundItemPart[] { 
                   		                                                       new VinyardGroundItemPart( 0x31F4, VinyardGroundPosition.Top, -1, -1 ),
                   		                                                       new VinyardGroundItemPart( 0x31F4, VinyardGroundPosition.Bottom, -1, -1 ),
                   		                                                       new VinyardGroundItemPart( 0x31F4, VinyardGroundPosition.Left, -1, -1 ),
                   		                                                       new VinyardGroundItemPart( 0x31F4, VinyardGroundPosition.Right, -1, -1 ),
                   		                                                       new VinyardGroundItemPart( 0x31F4, VinyardGroundPosition.West, -1, -1 ),
                   		                                                       new VinyardGroundItemPart( 0x31F4, VinyardGroundPosition.North, -1, -1 ),
                   		                                                       new VinyardGroundItemPart( 0x31F4, VinyardGroundPosition.East, -1, -1 ),
                   		                                                       new VinyardGroundItemPart( 0x31F4, VinyardGroundPosition.South, -1, -1 ),
                   		                                                       new VinyardGroundItemPart( 0x31F4, VinyardGroundPosition.Center, 44, 24 )
                   		                                                   }),
/* PlainBorder */		new VinyardGroundInfo( new VinyardGroundItemPart[] { 
                 		                                                       new VinyardGroundItemPart( 0x1B30, VinyardGroundPosition.Top, 44, 7 ),
                 		                                                       new VinyardGroundItemPart( 0x1B2F, VinyardGroundPosition.Bottom, 44, 68 ),
                 		                                                       new VinyardGroundItemPart( 0x1B31, VinyardGroundPosition.Left, 0, 35 ),
                 		                                                       new VinyardGroundItemPart( 0x1B32, VinyardGroundPosition.Right, 88, 32 ),
                 		                                                       new VinyardGroundItemPart( 0x1B29, VinyardGroundPosition.West, 22, 11 ),
                 		                                                       new VinyardGroundItemPart( 0x1B2A, VinyardGroundPosition.North, 66, 12 ),
                 		                                                       new VinyardGroundItemPart( 0x1B28, VinyardGroundPosition.East, 66, 46 ),
                 		                                                       new VinyardGroundItemPart( 0x1B27, VinyardGroundPosition.South, 22, 46 ),
                 		                                                       new VinyardGroundItemPart( 0x31F4, VinyardGroundPosition.Center, 44, 24 )
                 		                                                   })
                                                                             };
        #endregion

        public static VinyardGroundInfo[] Infos { get { return m_Infos; } }
    }
}