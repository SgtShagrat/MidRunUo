/***************************************************************************
 *                               NecromancerAltarAddon.cs
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
    public class NecromancerAltarAddon : BaseAddon
    {
        private static int[ , ] m_Components = new int[ , ]
        {
			  {10906, 1, 0, 0}, {10907, 0, 0, 0}    // 1	2	
		};

        [Constructable]
        public NecromancerAltarAddon()
        {
            for( int i = 0; i < m_Components.Length / 4; i++ )
                AddComponent( new AddonComponent( m_Components[ i, 0 ] ), m_Components[ i, 1 ], m_Components[ i, 2 ], m_Components[ i, 3 ] );

            AddComponent( new NecromancerAltarActiveComponent(), 0, 0, 10 );
        }

        #region serialization
        public NecromancerAltarAddon( Serial serial )
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