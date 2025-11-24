/***************************************************************************
 *                               CavePassage.cs
 *                            --------------------
 *   begin                : 12 gennaio, 2009
 *   author               :	Faxx	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Faxx - Dies Irae			
 *   revisione            : Dies Irae
 ***************************************************************************/

using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class CavePassage : MultiItem
    {
        private static int m_CaveCenter = 0x53E;
        private static int m_CaveE = 0x544;
        private static int m_CaveN = 0x54E;
        private static int m_CaveS = 0x543;
        private static int m_CaveW = 0x54A;

        [Constructable]
        public CavePassage()
            : base( m_CaveCenter )
        {
            Movable = false;
            AddSubItem( m_CaveCenter, 2, 2 );
            AddSubItem( m_CaveCenter, 2, 0 );
            AddSubItem( m_CaveCenter, 0, 2 );

            //AddSubItem(m_CaveCenter,0,0);

            AddSubItem( m_CaveE, 0, 1 );
            AddSubItem( m_CaveW, 2, 1 );
            AddSubItem( m_CaveN, 1, 2 );
            AddSubItem( m_CaveS, 1, 0 );
        }

        public virtual Type CaveType
        {
            get { return typeof( CaveBoulder ); }
        }

        public override void OnMapChange()
        {
            Point3D loc = Location;
            loc.X = loc.X + 1;
            loc.Y = loc.Y + 1;

            Hue = CaveBoulder.GetDepthHue( Z );

            // delete all items around
            IPooledEnumerable e = Map.GetItemsInRange( loc, 1 );

            List<Item> todelete = new List<Item>();
            foreach( Item i in e )
            {
                if( i.Serial != Serial && i.Z == Z )
                    todelete.Add( i );
            }

            foreach( Item i in todelete )
                i.Delete();

            Point3D loc2 = loc;
            loc2.Z = loc2.Z - CaveBoulder.LevelDepth;

            // start a new cave below
            CaveBoulder b = Activator.CreateInstance( CaveType ) as CaveBoulder;
            if( b != null )
            {
                b.MoveToWorld( loc2, Map );
                b.BlockSize = 3;
                b.Break();
            }

            base.OnMapChange();

            // deete items in at the landing point

            e = Map.GetItemsInRange( loc2, 0 );

            todelete = new List<Item>();

            foreach( Item i in e ) // delete all items around
            {
                if( i.Z == loc2.Z && !( i is CaveFloorCenter ) )
                    todelete.Add( i );
            }

            foreach( Item i in todelete )
                i.Delete();
        }

        #region serialization
        public CavePassage( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int v = reader.ReadInt();
        }
        #endregion
    }
}