using System;
using System.Collections;
using System.Threading;

namespace Midgard.Engines.RandomEncounterSystem
{
    internal delegate void RedBlackTreeModifiedHandler();

    /// <remarks>
    /// RedBlackTree is implemented using black-red binary trees. The
    /// algorithms follows the indications given in the textbook
    /// "Introduction to Algorithms" Thomas H. Cormen, Charles E. 
    /// Leiserson, Ronald L. Rivest
    /// </remarks>
    public class RedBlackTree : ICollection
    {
        /// <summary>
        /// Store the number of elements in the RedBlackTree.
        /// </summary>
        private int m_Count;

        /// <summary>
        /// Store the root node of the RedBlackTree.
        /// </summary>
        internal RedBlackTreeNode m_Root;

        /// <summary>
        /// Store the IComparer that allows to compare the node keys.
        /// </summary>
        private IComparer m_Comparer;

        /// <summary>
        /// Store the lock for multiple-reader access and single-writer access.
        /// </summary>
        private ReaderWriterLock m_RwLock;

        /// <summary>
        /// Store the RedBlackTreeEnumerator which will be called if the
        /// RedBlackTree is modified
        /// </summary>
        private event RedBlackTreeModifiedHandler RedBlackTreeModified;

        /// <summary>
        /// Initializes an new instance of Collections.System.RedBlackTree
        /// class that is empty. A default m_Comparer will be used to
        /// compare the elements added to the RedBlackTree.
        /// </summary>
        public RedBlackTree()
        {
            m_Comparer = Comparer.Default;
            Initialize();
        }

        /// <summary>
        /// Initializes an new instance of Collections.System.RedBlackTree
        /// class that is empty.
        /// </summary>
        /// <param name="comp">
        /// comp represents the IComparer elements which will be used to
        /// sort the elements in RedBlackTree.
        /// </param>
        public RedBlackTree( IComparer comp )
        {
            m_Comparer = comp;
            Initialize();
        }

        /// <summary>
        /// Perform the common initialization taks to all the constructors.
        /// </summary>
        private void Initialize()
        {
            m_Count = 0;
            m_Root = null;
            m_RwLock = new ReaderWriterLock();
        }

        /// <summary>
        /// Gets the number of elements stored in RedBlackTree.
        /// </summary>
        public int Count
        {
            get { return m_Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the access to RedBlackTree is
        /// synchronized (thread-safe).
        /// </summary>
        public bool IsSynchronized
        {
            get { return true; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access
        /// to RedBlackTree
        /// </summary>
        public object SyncRoot
        {
            get { return this; }
        }

        /// <summary>
        /// Gets a value indicating whether the RedBlackTree has
        /// a fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the RedBlackTree is
        /// read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the highest element stored in the RedBlackTree. The operation
        /// is performed in a guaranteed logarithmic time of the size of RedBlackTree.
        /// </summary>
        public object Max
        {
            get
            {
                RedBlackTreeNode node;

                m_RwLock.AcquireReaderLock( Timeout.Infinite );

                try
                {
                    if( m_Root == null )
                        throw new InvalidOperationException( "Unable to return Max because the RedBlackTree is empty." );

                    node = m_Root;
                    while( node.Right != null )
                        node = node.Right;
                }
                finally
                {
                    m_RwLock.ReleaseReaderLock();
                }

                return node.Key;
            }
        }

        /// <summary>
        /// Gets the lowest element stored in the RedBlackTree. The operation
        /// is performed in a guaranteed logarithmic time of the size of RedBlackTree.
        /// </summary>
        public object Min
        {
            get
            {
                RedBlackTreeNode node;

                m_RwLock.AcquireReaderLock( Timeout.Infinite );

                try
                {
                    if( m_Root == null )
                        throw new InvalidOperationException( "Unable to return Min because the RedBlackTree is empty." );

                    node = m_Root;
                    while( node.Left != null )
                        node = node.Left;
                }
                finally
                {
                    m_RwLock.ReleaseReaderLock();
                }

                return node.Key;
            }
        }

        /// <summary>
        /// Adds an elements to the RedBlackTree. The operation is performed
        /// in a guaranteed logarithmic time of the RedBlackTree size.
        /// </summary>
        public void Add( object x )
        {
            m_RwLock.AcquireReaderLock( Timeout.Infinite );

            try
            {
                OnRedBlackTreeModified();
                //if(m_Comparer == null) 
                //    throw new ArgumentException("RedBlackTree : not able to compare the elements");

                if( m_Root == null )
                    m_Root = new RedBlackTreeNode( x, null );
                else
                {
                    // First step : a naive insertion of the element
                    RedBlackTreeNode node1 = m_Root, node2 = null;

                    while( node1 != null )
                    {
                        node2 = node1;
                        if( m_Comparer.Compare( x, node1.Key ) < 0 )
                            node1 = node1.Left;
                        else
                            node1 = node1.Right;
                    }

                    node1 = new RedBlackTreeNode( x, node2 );

                    if( m_Comparer.Compare( x, node2.Key ) < 0 )
                        node2.Left = node1;
                    else
                        node2.Right = node1;

                    node1.Color = true;

                    // Then : correct the structure of the tree
                    while( node1 != m_Root && node1.Father.Color )
                    {
                        if( node1.Father == node1.Father.Father.Left )
                        {
                            node2 = node1.Father.Father.Right;
                            if( node2 != null && node2.Color )
                            {
                                node1.Father.Color = false;
                                node2.Color = false;
                                node1.Father.Father.Color = true;
                                node1 = node1.Father.Father;
                            }
                            else
                            {
                                if( node1 == node1.Father.Right )
                                {
                                    node1 = node1.Father;
                                    RotateLeft( node1 );
                                }
                                node1.Father.Color = false;
                                node1.Father.Father.Color = true;
                                RotateRight( node1.Father.Father );
                            }
                        }
                        else
                        {
                            node2 = node1.Father.Father.Left;
                            if( node2 != null && node2.Color )
                            {
                                node1.Father.Color = false;
                                node2.Color = false;
                                node1.Father.Father.Color = true;
                                node1 = node1.Father.Father;
                            }
                            else
                            {
                                if( node1 == node1.Father.Left )
                                {
                                    node1 = node1.Father;
                                    RotateRight( node1 );
                                }
                                node1.Father.Color = false;
                                node1.Father.Father.Color = true;
                                RotateLeft( node1.Father.Father );
                            }
                        }
                    }
                }

                m_Root.Color = false;

                m_Count++;
            }
            finally
            {
                m_RwLock.ReleaseReaderLock();
            }
        }

        /// <summary>
        /// Removes of the elements from the RedBlackTree.
        /// </summary>
        public void Clear()
        {
            m_RwLock.AcquireWriterLock( Timeout.Infinite );

            try
            {
                OnRedBlackTreeModified();
                m_Root = null;
                m_Count = 0;
            }
            finally
            {
                m_RwLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Determines whether the RedBlackTree contains a specific object.
        /// The RedBlackTree could contain several identical object. The operation
        /// is performed in a guaranteed logarithmic time of the RedBlackTree size.
        /// </summary>
        public bool Contains( object x )
        {
            // null is always contained in a tree
            if( x == null )
                return true;

            bool isContained;

            m_RwLock.AcquireReaderLock( Timeout.Infinite );

            try
            {
                isContained = ( RecContains( m_Root, x ) != null );
            }
            finally
            {
                m_RwLock.ReleaseReaderLock();
            }

            return isContained;
        }

        /// <summary>
        /// Copies the elements of RedBlackTree to a one dimensional
        /// System.Array at the specified index.
        /// </summary>
        public void CopyTo( Array array, int index )
        {
            // Check the validity of the arguments
            if( array == null )
                throw new ArgumentNullException();
            if( index < 0 )
                throw new ArgumentOutOfRangeException();
            if( array.Rank > 1 || ( array.Length - index ) < m_Count )
                throw new ArgumentException();

            m_RwLock.AcquireReaderLock( Timeout.Infinite );

            try
            {
                RecCopyTo( m_Root, array, index );
            }
            finally
            {
                m_RwLock.ReleaseReaderLock();
            }
        }

        /// <summary>
        /// Returns an System.Collection.IEnumerator that can iterate
        /// through the RedBlackTree.
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            RedBlackTreeEnumerator tEnum;

            m_RwLock.AcquireReaderLock( Timeout.Infinite );

            try
            {
                tEnum = new RedBlackTreeEnumerator( this );
                RedBlackTreeModified += new RedBlackTreeModifiedHandler( tEnum.Invalidate );
            }
            finally
            {
                m_RwLock.ReleaseReaderLock();
            }

            return tEnum;
        }

        /// <summary>
        /// Removes the first occurrence of the element in the RedBlackTree.
        /// The operation is performed in a guaranteed logarithmic time
        /// of the RedBlackTree size.
        /// </summary>
        public void Remove( object x )
        {
            RedBlackTreeNode node;

            m_RwLock.AcquireWriterLock( Timeout.Infinite );

            try
            {
                node = RecContains( m_Root, x );
                if( node != null )
                    RemoveRedBlackTreeNode( node );
            }
            finally
            {
                m_RwLock.ReleaseWriterLock();
            }
        }

        public object Find( object x )
        {
            RedBlackTreeNode node;

            m_RwLock.AcquireWriterLock( Timeout.Infinite );

            try
            {
                node = RecContains( m_Root, x );
                if( node != null )
                    return node.Key;
                else
                    return null;
            }
            finally
            {
                m_RwLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Invalidates the System.Collections.IEnumerator linked
        /// with the RedBlackTree.
        /// </summary>
        private void OnRedBlackTreeModified()
        {
            if( RedBlackTreeModified != null )
            {
                RedBlackTreeModified();
                RedBlackTreeModified = null;
            }
        }

        /// <summary>
        /// Removes a specific node of the RedBlackTree.
        /// </summary>
        /// <param name="node">
        /// node must be contained by RedBlackTree.</param>
        private void RemoveRedBlackTreeNode( RedBlackTreeNode node )
        {
            RedBlackTreeNode nodeX, nodeY;

            if( node.Left == null || node.Right == null )
                nodeY = node;
            else
                nodeY = Successor( node );

            nodeX = nodeY.Left ?? nodeY.Right;

            RedBlackTreeNode fatherY = nodeY.Father;
            RedBlackTreeNode fatherX = fatherY;
            if( nodeX != null )
                nodeX.Father = nodeY.Father;

            if( fatherY == null )
                m_Root = nodeX;
            else
            {
                if( nodeY == fatherY.Left )
                    fatherY.Left = nodeX;
                else
                    fatherY.Right = nodeX;
            }

            if( nodeY != node )
                node.Key = nodeY.Key;

            // Remove Correction of the colors
            if( !nodeY.Color )
            {
                while( nodeX != m_Root && ( nodeX == null || !nodeX.Color ) )
                {
                    if( nodeX == fatherX.Left /*&& nodeX != fatherX.Right*/)
                    {
                        // fatherY = fatherX;
                        nodeY = fatherX.Right;
                        if( /*nodeY != null && */nodeY.Color )
                        {
                            nodeY.Color = false;
                            fatherX.Color = true;
                            RotateLeft( fatherX );
                            nodeY = fatherX.Right;
                        }

                        if( ( nodeY.Left == null || !nodeY.Left.Color )
                            && ( nodeY.Right == null || !nodeY.Right.Color ) )
                        {
                            nodeY.Color = true;
                            nodeX = fatherX;
                            fatherX = fatherX.Father;
                        }
                        else
                        {
                            if( nodeY.Right == null || !nodeY.Right.Color )
                            {
                                if( nodeY.Left != null )
                                    nodeY.Left.Color = false;
                                nodeY.Color = true;
                                RotateRight( nodeY );
                                nodeY = fatherX.Right;
                            }

                            nodeY.Color = fatherX.Color;
                            fatherX.Color = false;
                            nodeY.Right.Color = false;
                            RotateLeft( fatherX );
                            nodeX = m_Root;
                        }
                    }
                    else
                    {
                        fatherY = fatherX;
                        nodeY = fatherX.Left;
                        if( /*nodeY != null &&*/ nodeY.Color )
                        {
                            nodeY.Color = false;
                            fatherX.Color = true;
                            RotateRight( fatherX );
                            nodeY = fatherX.Left;
                        }

                        if( ( nodeY.Right == null || !nodeY.Right.Color )
                            && ( nodeY.Left == null || !nodeY.Left.Color ) )
                        {
                            nodeY.Color = true;
                            nodeX = fatherX;
                            fatherX = fatherX.Father;
                        }
                        else
                        {
                            if( nodeY.Left == null || !nodeY.Left.Color )
                            {
                                nodeY.Right.Color = false;
                                nodeY.Color = true;
                                RotateLeft( nodeY );
                                nodeY = fatherX.Left;
                            }

                            nodeY.Color = fatherX.Color;
                            fatherX.Color = false;
                            nodeY.Left.Color = false;
                            RotateRight( fatherX );
                            nodeX = m_Root;
                        }
                    }
                } // End While

                if( nodeX != null )
                    nodeX.Color = false;
            } // End Correction

            m_Count--;
        }

        /// <summary>
        /// Returns the node that contains the successor of node.Key.
        /// If such node does not exist then null is returned.
        /// </summary>
        /// <param name="node">
        /// node must be contained by RedBlackTree.</param>
        private static RedBlackTreeNode Successor( RedBlackTreeNode node )
        {
            RedBlackTreeNode node1, node2;

            if( node.Right != null )
            {
                // We find the Min
                node1 = node.Right;
                while( node1.Left != null )
                    node1 = node1.Left;
                return node1;
            }

            node1 = node;
            node2 = node.Father;
            while( node2 != null && node1 == node2.Right )
            {
                node1 = node2;
                node2 = node2.Father;
            }
            return node2;
        }

        /// <summary>
        /// Performs a left tree rotation.
        /// </summary>
        /// <param name="node">
        /// node is considered as the root of the tree.</param>
        private void RotateLeft( RedBlackTreeNode node )
        {
            RedBlackTreeNode nodeX = node, nodeY = node.Right;
            nodeX.Right = nodeY.Left;

            if( nodeY.Left != null )
                nodeY.Left.Father = nodeX;
            nodeY.Father = nodeX.Father;

            if( nodeX.Father == null )
                m_Root = nodeY;
            else
            {
                if( nodeX == nodeX.Father.Left )
                    nodeX.Father.Left = nodeY;
                else
                    nodeX.Father.Right = nodeY;
            }

            nodeY.Left = nodeX;
            nodeX.Father = nodeY;
        }

        /// <summary>
        /// Performs a right tree rotation.
        /// </summary>
        /// <param name="node">
        /// node is considered as the root of the tree.</param>
        private void RotateRight( RedBlackTreeNode node )
        {
            RedBlackTreeNode nodeX = node, nodeY = node.Left;
            nodeX.Left = nodeY.Right;

            if( nodeY.Right != null )
                nodeY.Right.Father = nodeX;
            nodeY.Father = nodeX.Father;

            if( nodeX.Father == null )
                m_Root = nodeY;
            else
            {
                if( nodeX == nodeX.Father.Right )
                    nodeX.Father.Right = nodeY;
                else
                    nodeX.Father.Left = nodeY;
            }

            nodeY.Right = nodeX;
            nodeX.Father = nodeY;
        }

        /// <summary>
        /// Copies the element of the tree into a one dimensional
        /// System.Array starting at index.
        /// </summary>
        /// <param name="currentRedBlackTreeNode">The root of the tree.</param>
        /// <param name="array">The System.Array where the elements will be copied.</param>
        /// <param name="index">The index where the copy will start.</param>
        /// <returns>
        /// The new index after the copy of the elements of the tree.
        /// </returns>
        private static int RecCopyTo( RedBlackTreeNode currentRedBlackTreeNode, Array array, int index )
        {
            if( currentRedBlackTreeNode != null )
            {
                array.SetValue( currentRedBlackTreeNode.Key, index );
                return RecCopyTo( currentRedBlackTreeNode.Right, array,
                                 RecCopyTo( currentRedBlackTreeNode.Left, array, index + 1 ) );
            }
            else
                return index;
        }

        /// <summary>
        /// Returns a node of the tree which contains the object
        /// as Key. If the tree does not contain such node, then
        /// null is returned.
        /// </summary>
        /// <param name="node">The root of the tree.</param>
        /// <param name="x">The researched object.</param>
        private RedBlackTreeNode RecContains( RedBlackTreeNode node, object x )
        {
            if( node == null )
                return null;

            int c = m_Comparer.Compare( x, node.Key );

            if( c == 0 )
                return node;
            if( c < 0 )
                return RecContains( node.Left, x );
            else
                return RecContains( node.Right, x );
        }

        /// <summary>
        /// For debugging only. Checks whether the RedBlackTree is conform
        /// to the definition of the a red-black tree. If not an
        /// exception is thrown.
        /// </summary>
        /// <param name="node">The root of the tree.</param>
        private int RecConform( RedBlackTreeNode node )
        {
            if( node == null )
                return 1;

            if( node.Father == null )
            {
                if( node.Color )
                    throw new ArgumentException( "RedBlackTree : the root is not black." );
            }
            else
            {
                if( node.Father.Color && node.Color )
                    throw new ArgumentException( "RedBlackTree : father and son are red." );
            }

            if( node.Left != null && m_Comparer.Compare( node.Key, node.Left.Key ) < 0 )
                throw new ArgumentException( "RedBlackTree : order not respected in tree." );
            if( node.Right != null && m_Comparer.Compare( node.Key, node.Right.Key ) > 0 )
                throw new ArgumentException( "RedBlackTree : order not respected in tree." );

            int a = RecConform( node.Left ),
                b = RecConform( node.Right );

            if( a < 0 || b < 0 )
                return -1;

            if( a != b )
                throw new ArgumentException( "RedBlackTree : the paths do have not the  same number of black nodes." );

            if( !node.Color )
                return ( a + 1 );
            else
                return a;
        }
    }

    /// <remarks>
    /// RedBlackTreeEnumerator could be instancied only through the
    /// RedBlackTree.GetEnumerator method. If the RedBlackTree is modified
    /// after the instanciation of RedBlackTreeEnumerator, then
    /// RedBlackTreeEnumerator become invalid. Any attempt to read or
    /// iterate will throw an exception. The elements contained
    /// in the RedBlackTree are iterated following the order provided
    /// to the RedBlackTree (ascending order).
    /// </remarks>
    public class RedBlackTreeEnumerator : IEnumerator
    {
        /// <summary>
        /// The m_Current node (or null if none)
        /// </summary>
        private RedBlackTreeNode m_Current;

        /// <summary>
        /// Reference to the RedBlackTree which has instanciated the
        /// RedBlackTreeEnumerator.
        /// </summary>
        private RedBlackTree m_Tree;

        /// <summary>
        /// Store the state of the RedBlackTreeEnumerator. If 
        /// <c>!m_Started</c> then the m_Current position is
        /// before the first element of the RedBlackTree.
        /// </summary>
        private bool m_Started;

        /// <summary>
        /// Store the the state of the RedBlackTreeEnumerator. If
        /// <c>!m_IsValid</c>, any attempt to read or iterate 
        /// will throw an exception.
        /// </summary>
        private bool m_IsValid;

        /// <summary>
        /// Initializes an new instance of Collections.System.RedBlackTreeEnumerator
        /// class. The m_Current position is before the first element.
        /// </summary>
        /// <param name="t">The RedBlackTree which will be enumerate.</param>
        internal RedBlackTreeEnumerator( RedBlackTree t )
        {
            m_Tree = t;
            m_Started = false;
            m_IsValid = true;
            m_Current = m_Tree.m_Root;
            if( m_Current != null )
            {
                while( m_Current.Left != null )
                    m_Current = m_Current.Left;
            }
        }

        /// <summary>
        /// Gets the m_Current element in the RedBlackTree.
        /// </summary>
        public object Current
        {
            get
            {
                if( !m_IsValid )
                    throw
                        new InvalidOperationException( "The RedBlackTree was modified after the enumerator was created" );
                if( !m_Started )
                    throw
                        new InvalidOperationException( "Before first element" );
                if( m_Current == null )
                    throw
                        new InvalidOperationException( "After last element" );
                return m_Current.Key;
            }
        }

        /// <summary>
        /// Advances the RedBlackTreeEnumerator the next element of the RedBlackTree.
        /// Returns whether the move was possible.
        /// </summary>
        public bool MoveNext()
        {
            if( !m_IsValid )
                throw
                    new InvalidOperationException( "The RedBlackTree was modified after the enumerator was created" );
            if( !m_Started )
            {
                m_Started = true;
                return m_Current != null;
            }
            if( m_Current == null )
                return false;
            if( m_Current.Right == null )
            {
                RedBlackTreeNode prev;
                do
                {
                    prev = m_Current;
                    m_Current = m_Current.Father;
                } while( ( m_Current != null ) && ( m_Current.Right == prev ) );
            }
            else
            {
                m_Current = m_Current.Right;
                while( m_Current.Left != null )
                    m_Current = m_Current.Left;
            }
            return m_Current != null;
        }

        /// <summary>
        /// Sets the enumerator the its initial position which is before
        /// the first element of the RedBlackTree.
        /// </summary>
        public void Reset()
        {
            if( !m_IsValid )
                throw
                    new InvalidOperationException( "The RedBlackTree was modified after the enumerator was created" );
            m_Started = false;
            m_Current = m_Tree.m_Root;
            if( m_Current != null )
            {
                while( m_Current.Left != null )
                    m_Current = m_Current.Left;
            }
        }

        /// <summary>
        /// Invalidates the RedBlackTreeEnumerator.
        /// </summary>
        internal void Invalidate()
        {
            m_IsValid = false;
        }
    }

    /// <remarks>
    /// RedBlackTreeNode is simple colored binary tree node which
    /// contains a m_Key.
    /// </remarks>
    internal class RedBlackTreeNode
    {
        /// <summary>
        /// References to the other elements of the RedBlackTree.
        /// </summary>
        private RedBlackTreeNode m_Father, m_Left, m_Right;

        /// <summary>
        /// Reference to the object contained by the RedBlackTreeNode.
        /// </summary>
        private object m_Key;

        /// <summary>
        /// The color of the node (red = true, black = false).
        /// </summary>
        private bool m_Color;

        /// <summary>
        /// Initializes an new instance of Collections.System.RedBlackTreeNode
        /// class. All references are set to null.
        /// </summary>
        internal RedBlackTreeNode()
        {
            m_Key = null;
            m_Father = null;
            m_Left = null;
            m_Right = null;
            m_Color = true;
        }

        /// <summary>
        /// Initializes an new instance of Collections.System.RedBlackTreeNode
        /// class and partially insert the RedBlackTreeNode into a tree.
        /// </summary>
        /// <param name="k">Key of the RedBlackTreeNode</param>
        /// <param name="fatherRedBlackTreeNode">The m_Father node of the instanciated RedBlackTreeNode.</param>
        internal RedBlackTreeNode( object k, RedBlackTreeNode fatherRedBlackTreeNode )
        {
            m_Key = k;
            m_Father = fatherRedBlackTreeNode;
            m_Left = null;
            m_Right = null;
            m_Color = true;
        }

        /// <summary>
        /// Gets or sets the m_Key of the RedBlackTreeNode.
        /// </summary>
        internal object Key
        {
            get { return m_Key; }
            set { m_Key = value; }
        }

        /// <summary>
        /// Gets or sets the m_Father of the RedBlackTreeNode.
        /// </summary>
        internal RedBlackTreeNode Father
        {
            get { return m_Father; }
            set { m_Father = value; }
        }

        /// <summary>
        /// Gets or sets the m_Left children of the RedBlackTreeNode.
        /// </summary>
        internal RedBlackTreeNode Left
        {
            get { return m_Left; }
            set { m_Left = value; }
        }

        /// <summary>
        /// Gets or sets the m_Right children of the RedBlackTreeNode.
        /// </summary>
        internal RedBlackTreeNode Right
        {
            get { return m_Right; }
            set { m_Right = value; }
        }

        /// <summary>
        /// Gets or sets the color of the RedBlackTreeNode.
        /// </summary>
        internal bool Color
        {
            get { return m_Color; }
            set { m_Color = value; }
        }
    }
}