/***************************************************************************
 *                               DruidTree.cs
 *
 *   begin                : 27 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;

namespace Midgard.Engines.SpellSystem
{
    public class DruidTree : Item
    {
        private Leaves m_Leaves;

        [Constructable]
        public DruidTree( int bustID, int leavesID )
            : base( bustID )
        {
            Movable = false;
            m_Leaves = new Leaves( this, leavesID );
        }

        public DruidTree( Serial serial )
            : base( serial )
        {
        }

        public override void OnLocationChange( Point3D oldLocation )
        {
            if( m_Leaves != null )
                m_Leaves.Location = new Point3D( X, Y, Z );
        }

        public override void OnMapChange()
        {
            if( m_Leaves != null )
                m_Leaves.Map = Map;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if( m_Leaves != null && !m_Leaves.Deleted )
                m_Leaves.Delete();
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( 0 ); // version

            writer.Write( m_Leaves );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();

            m_Leaves = reader.ReadItem() as Leaves;
        }
        #endregion

        #region Leaves
        private class Leaves : Item
        {
            private DruidTree m_Tree;

            public Leaves( DruidTree tree, int leavesID )
                : base( leavesID )
            {
                Movable = false;
                m_Tree = tree;
            }

            public Leaves( Serial serial )
                : base( serial )
            {
            }

            public override void OnLocationChange( Point3D oldLocation )
            {
                if( m_Tree != null )
                    m_Tree.Location = new Point3D( X, Y, Z );
            }

            public override void OnMapChange()
            {
                if( m_Tree != null )
                    m_Tree.Map = Map;
            }

            public override void OnAfterDelete()
            {
                base.OnAfterDelete();

                if( m_Tree != null )
                    m_Tree.Delete();
            }

            #region serial-deserial
            public override void Serialize( GenericWriter writer )
            {
                base.Serialize( writer );

                writer.Write( 0 ); // version

                writer.Write( m_Tree );
            }

            public override void Deserialize( GenericReader reader )
            {
                base.Deserialize( reader );

                int version = reader.ReadInt();

                m_Tree = reader.ReadItem() as DruidTree;
            }
            #endregion
        }
        #endregion
    }
}