/***************************************************************************
 *                               LayerComparer.cs
 *                            ----------------------
 *   begin                : 24 agosto, 2009
 *   author               :	Dies Irae - Magius(CHE)
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae - Magius(CHE)		
 *
 ***************************************************************************/

using System.Collections.Generic;
using Server;

namespace Midgard.Engines.MyMidgard
{
    public class LayerComparer : IComparer<Item>
    {
        private const Layer ChainTunic = (Layer)254;
        private const Layer LeatherShorts = (Layer)253;
        private const Layer PlateArms = (Layer)255;
        public static readonly IComparer<Item> Instance = new LayerComparer();

        #region m_DesiredLayerOrder

        private static readonly Layer[] m_DesiredLayerOrder = new Layer[]
                                                                  {
                                                                      Layer.Cloak,
                                                                      Layer.Bracelet,
                                                                      Layer.Ring,
                                                                      Layer.Shirt,
                                                                      Layer.Pants,
                                                                      Layer.InnerLegs,
                                                                      Layer.Shoes,
                                                                      LeatherShorts,
                                                                      Layer.Arms,
                                                                      Layer.InnerTorso,
                                                                      LeatherShorts,
                                                                      PlateArms,
                                                                      Layer.MiddleTorso,
                                                                      Layer.OuterLegs,
                                                                      Layer.Neck,
                                                                      Layer.Waist,
                                                                      Layer.Gloves,
                                                                      Layer.OuterTorso,
                                                                      Layer.OneHanded,
                                                                      Layer.TwoHanded,
                                                                      Layer.FacialHair,
                                                                      Layer.Hair,
                                                                      Layer.Helm,
                                                                      Layer.Talisman
                                                                  };

        #endregion

        static LayerComparer()
        {
            TranslationTable = new int[ 256 ];

            for( int i = 0; i < m_DesiredLayerOrder.Length; ++i )
                TranslationTable[ (int)m_DesiredLayerOrder[ i ] ] = m_DesiredLayerOrder.Length - i;
        }

        public static int[] TranslationTable { get; private set; }

        #region IComparer<Item> Members

        public int Compare( Item a, Item b )
        {
            Layer aLayer = a.Layer;
            Layer bLayer = b.Layer;

            aLayer = Fix( a.ItemID, aLayer );
            bLayer = Fix( b.ItemID, bLayer );

            return TranslationTable[ (int)bLayer ] - TranslationTable[ (int)aLayer ];
        }

        #endregion

        public static bool IsValid( Item item )
        {
            return ( TranslationTable[ (int)item.Layer ] > 0 );
        }

        public Layer Fix( int itemID, Layer oldLayer )
        {
            if( itemID == 0x1410 || itemID == 0x1417 ) // platemail arms
                return PlateArms;

            if( itemID == 0x13BF || itemID == 0x13C4 ) // chainmail tunic
                return ChainTunic;

            if( itemID == 0x1C08 || itemID == 0x1C09 ) // leather skirt
                return LeatherShorts;

            if( itemID == 0x1C00 || itemID == 0x1C01 ) // leather shorts
                return LeatherShorts;

            return oldLayer;
        }
    }
}