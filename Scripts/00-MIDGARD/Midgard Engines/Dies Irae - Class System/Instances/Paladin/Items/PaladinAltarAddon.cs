/***************************************************************************
 *                               PaladinAltarAddon.cs
 *
 *   begin                : 10 January, 2010
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Items;

namespace Midgard.Engines.Classes
{
    public class PaladinAltarAddon : BaseAddon
    {
        private static int[ , ] m_Components = new int[ , ]
        {
                {7187, 0, 0, 6}, {5736, 0, 0, 0}, {7188, 0, 1, 6}   // 1	 2	 3	 
            ,   {5735, 0, 1, 0}, {7186, 0, -1, 6}, {5737, 0, -1, 0} // 4	 5	 6	 
        };

        [Constructable]
        public PaladinAltarAddon()
        {
            for( int i = 0; i < m_Components.Length / 4; i++ )
                AddComponent( new AddonComponent( m_Components[ i, 0 ] ), m_Components[ i, 1 ], m_Components[ i, 2 ], m_Components[ i, 3 ] );

            AddComponent( new PaladinAltarActiveComponent(), 0, 0, 6 );
        }

        #region serialization
        public PaladinAltarAddon( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // Version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}