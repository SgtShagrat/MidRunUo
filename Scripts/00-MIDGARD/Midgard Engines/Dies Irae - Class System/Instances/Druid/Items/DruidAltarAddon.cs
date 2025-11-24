/***************************************************************************
 *                               DruidAltarAddon.cs
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
    public class DruidAltarAddon : BaseAddon
    {
        private static int[ , ] m_Components = new int[ , ]
        {
                { 9017, 0, -1, 4 }, { 6944, -1, 1, 4 }, { 3346, -1, 1, 4 }  // 1	2	3	
            ,   { 6008, 1, -1, 4 }, { 2278, -1, 0, 4 }, { 6944, 0, 0, 4 }   // 4	5	6	
            ,   { 6944, 0, -1, 4 }, { 3309, 1, 0, 0 }                       // 7	8	
        };

        [Constructable]
        public DruidAltarAddon()
        {
            for( int i = 0; i < m_Components.Length / 4; i++ )
                AddComponent( new AddonComponent( m_Components[ i, 0 ] ), m_Components[ i, 1 ], m_Components[ i, 2 ], m_Components[ i, 3 ] );

            AddComponent( new DruidAltarActiveComponent(), 0, 0, 4 );
        }

        #region serialization
        public DruidAltarAddon( Serial serial )
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