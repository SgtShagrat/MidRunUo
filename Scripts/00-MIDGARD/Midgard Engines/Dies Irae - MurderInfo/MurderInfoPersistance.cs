/***************************************************************************
 *                                  MurderInfoPersistance.cs
 *                            		-----------------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server;

namespace Midgard.Engines.MurderInfo
{
    public class MurderInfoPersistance : Item
    {
        private static Dictionary<Mobile, List<MurderInfo>> m_Table = new Dictionary<Mobile, List<MurderInfo>>();

        public static MurderInfoPersistance Instance { get; private set; }
        public override string DefaultName { get { return "MurderInfo Persistance - Internal"; } }

        [Constructable]
        public MurderInfoPersistance()
            : base( 1 )
        {
            Movable = false;

            if( Instance == null || Instance.Deleted )
            {
                Instance = this;
            }
            else
            {
                base.Delete();
            }
        }

        public MurderInfoPersistance( Serial serial )
            : base( serial )
        {
            Instance = this;
        }

        public static void EnsureExistence()
        {
            if( Instance == null )
                Instance = new MurderInfoPersistance();
        }

        public static void RegisterInfo( Mobile killer, MurderInfo info )
        {
            if( killer == null )
                return;

            List<MurderInfo> list;
            m_Table.TryGetValue( killer, out list );

            if( list == null )
                m_Table[ killer ] = list = new List<MurderInfo>();

            list.Add( info );
        }

        public static List<MurderInfo> GetMurderInfoForKiller( Mobile killer )
        {
            if( killer == null )
                return null;

            List<MurderInfo> list = null;
            m_Table.TryGetValue( killer, out list );

            return list;
        }

        public static List<MurderInfo> GetMurderInfoForVictim( Mobile victim )
        {
            if( victim == null )
                return null;

            List<MurderInfo> list = new List<MurderInfo>();

            foreach( KeyValuePair<Mobile, List<MurderInfo>> kvp in m_Table )
            {
                if( kvp.Value != null )
                {
                    foreach( MurderInfo info in kvp.Value )
                    {
                        if( info.Victim != null && info.Victim == victim )
                        {
                            list.Add( info );
                        }
                    }
                }
            }

            if( list.Count == 0 )
                return null;
            else
                return list;
        }

        public static List<Mobile> GetKillers()
        {
            if( m_Table == null || m_Table.Count == 0 )
                return null;

            List<Mobile> keys = new List<Mobile>( m_Table.Keys.Count );
            foreach( Mobile m in m_Table.Keys )
                keys.Add( m );
            // m_Table.Keys.CopyTo( keys, 0 );

            return keys;
        }

        public static void UnregisterInfoForKiller( Mobile killer, MurderInfo info )
        {
            if( m_Table != null && killer != null && m_Table.ContainsKey( killer ) )
            {
                List<MurderInfo> list;
                m_Table.TryGetValue( killer, out list );

                if( list != null && list.Contains( info ) )
                    list.Remove( info );

                if( list != null )
                    if( list.Count == 0 )
                        m_Table.Remove( killer );
            }
        }

        public static void UnregisterKiller( Mobile killer )
        {
            if( m_Table != null && killer != null && m_Table.ContainsKey( killer ) )
                m_Table.Remove( killer );
        }

        public static void UnRegisterAll()
        {
            if( m_Table != null )
                m_Table.Clear();
        }

        public override void Delete()
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( m_Table.Count );
            foreach( KeyValuePair<Mobile, List<MurderInfo>> kvp in m_Table )
            {
                writer.WriteMobile( kvp.Key );

                if( kvp.Value == null )
                    writer.Write( (int)0 );
                else
                {
                    writer.Write( kvp.Value.Count );
                    foreach( MurderInfo info in kvp.Value )
                        info.Serialize( writer );
                }
            }

            //			List<Mobile> keys = new List<Mobile>( m_Table.Keys.Count );
            //			m_Table.Keys.CopyTo( keys.ToArray(), 0 );
            //
            //			writer.Write( keys.Count );
            //
            //			foreach( Mobile key in keys )
            //			{
            //				writer.WriteMobile( key );
            //
            //				List<MurderInfo> list = GetMurderInfoFromMobile( key );
            //				if( list == null )
            //					writer.Write( (int) 0 );
            //				else
            //				{
            //					writer.Write( (int) list.Count );
            //
            //					foreach( MurderInfo info in list )
            //						info.Serialize( writer );
            //				}
            //			}

            //			writer.Write( keys.Count );
            //			for( int i = 0; i < keys.Count; i++ )
            //			{
            //				Mobile killer = keys[ i ];
            //				writer.WriteMobile( killer );
            //
            //				List<MurderInfo> list = GetMurderInfoFromMobile( killer );
            //				if( list == null )
            //					writer.Write( (int) 0 );
            //				else
            //				{
            //					writer.Write( (int) list.Count );
            //					for( int j = 0; j < list.Count; j++ )
            //						list[ j ].Serialize( writer );
            //				}
            //			}
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            if( m_Table == null )
                m_Table = new Dictionary<Mobile, List<MurderInfo>>();

            int killerCounter = reader.ReadInt();
            for( int i = 0; i < killerCounter; i++ )
            {
                Mobile killer = reader.ReadMobile();

                int infoCounter = reader.ReadInt();
                for( int j = 0; j < infoCounter; j++ )
                {
                    MurderInfo info = new MurderInfo( reader );
                    RegisterInfo( killer, info );
                }
            }
        }
    }
}
