using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using Server;
using Server.Items;

namespace Midgard.Engines.MidgardTownSystem
{
    public class JailCell
    {
        private readonly Point3D m_GoLocation;
        private readonly Point3D m_DoorLocation;
        private readonly string m_Name;

        public bool HasDoor
        {
            get { return Door != null; }
        }

        public bool IsLocked
        {
            get { return HasDoor && Door.Locked; }
        }

        public string Name
        {
            get { return m_Name; }
        }

        public Point3D DoorLocation
        {
            get { return m_DoorLocation; }
        }

        public Point3D GoLocation
        {
            get { return m_GoLocation; }
        }

        public BaseDoor Door { get; private set; }

        public bool IsPrisonerInCell( Mobile m )
        {
            List<Mobile> mobs = FindMobilesInCell();
            return mobs != null && mobs.Contains( m );
        }

        public JailCell( XmlElement xml )
        {
            try
            {
                if( !Region.ReadPoint3D( xml[ "go" ], Map.Felucca, ref m_GoLocation, true ) )
                    throw new DataException( string.Format( "Error: the JailCell xmlElement has not {0} attribute", "go" ) );

                if( !Region.ReadPoint3D( xml[ "door" ], Map.Felucca, ref m_DoorLocation, true ) )
                    throw new DataException( string.Format( "Error: the JailCell xmlElement has not {0} attribute", "door" ) );

                if( !Region.ReadString( xml, "name", ref m_Name, true ) )
                    throw new DataException( string.Format( "Error: the JailCell xmlElement has not {0} attribute", "name" ) );
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.Message );
            }
        }

        public void FindDoor()
        {
            Door = Find( DoorLocation, Map.Felucca );
        }

        private static BaseDoor Find( Point3D loc, Map facet )
        {
            foreach( Item item in facet.GetItemsInRange( loc, 0 ) )
            {
                if( typeof( BaseDoor ).IsAssignableFrom( item.GetType() ) )
                    return item as BaseDoor;
            }

            return null;
        }

        public List<Mobile> FindMobilesInCell()
        {
            return FindMobiles( TownJailSystem.JailsMap, 3 );
        }

        public List<Mobile> FindMobiles( Map map, int range )
        {
            if( map == null )
                return null;

            List<Mobile> list = new List<Mobile>();
            if( range <= 0 )
                return null;

            IPooledEnumerable inRange = map.GetMobilesInRange( GoLocation, range );

            foreach( Mobile trg in inRange )
                list.Add( trg );

            inRange.Free();
            return list;
        }

        public void CheckRelease()
        {
            List<Mobile> prisoners = FindMobilesInCell();
            if( prisoners == null || prisoners.Count <= 0 )
            {
                if( HasDoor )
                    Door.Locked = false;
            }
        }

        public void VerifyIntegrity()
        {
            FindDoor();
            if( Door == null )
                Config.Pkg.LogError( "{0} cell without door.", Name );
        }
    }
}