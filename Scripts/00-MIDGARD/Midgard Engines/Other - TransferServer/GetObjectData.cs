using System;
using System.Collections.Generic;
using Server.Items;
using Server.Misc;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2.Xts
{
    [Serializable]
    public class ObjectData
    {
        public short X;
        public short Y;
        public byte Map;
        public string Name;

        public ObjectData( short x, short y, byte map, string name )
        {
            X = x;
            Y = y;
            Map = map;
            Name = name;
        }

        public ObjectData()
        {
        }

        public override string ToString()
        {
            return Name;
        }
    }

    // this is the return message for object data information
    [Serializable]
    public class ReturnObjectData : TransferMessage
    {
        public ObjectData[] Data { get; set; }

        public ReturnObjectData( ObjectData[] data )
        {
            Data = data;
        }

        public ReturnObjectData()
        {
        }
    }

    // this message will collect and return a list of Object locations filtered by map and type
    [Serializable]
    public class GetObjectData : TransferMessage
    {
        public int SelectedMap;
        public string ObjectType;
        public int ItemID;
        public short Statics;
        public short Visible;
        public short Movable;
        public short InContainers;
        public short Carried;
        public short Blessed;
        public short Innocent;
        public short Controlled;
        public short Access;
        public short Criminal;

        public GetObjectData()
        {
        }

        public GetObjectData( int map )
        {
            SelectedMap = map;
        }

        // create a return message that contains the list of object locations
        // access can be restricted with the TransferAccess attribute

        [TransferAccess( AccessLevel.Administrator )]
        public override TransferMessage ProcessMessage()
        {
            List<IEntity> tmparray = new List<IEntity>();
            Type type = null;

            // is there a object type restriction?
            if( !string.IsNullOrEmpty( ObjectType ) )
            {
                //type = Type.GetType(ObjectType);
                type = SpawnerType.GetType( ObjectType );
            }

            if( type != null )
            {
                if( type == typeof( Mobile ) || type.IsSubclassOf( typeof( Mobile ) ) )
                {
                    // because this is going to run in a separate thread, need to worry about the world lists 
                    // being modified while loooping, so make a copy

                    List<Mobile> mobilearray = new List<Mobile>( World.Mobiles.Values );

                    foreach( var m in mobilearray )
                    {
                        if( m.Map == null || m.Map == Map.Internal )
                            continue;

                        bool passed = true;

                        // match map
                        if( m.Map.MapID != SelectedMap && SelectedMap != -1 )
                        {
                            passed = false;
                        }

                        // match type
                        if( passed && type != m.GetType() && !m.GetType().IsSubclassOf( type ) )
                        {
                            passed = false;
                        }

                        // check filters

                        // apply playermobile filters
                        if( m is PlayerMobile )
                        {
                            PlayerMobile b = (PlayerMobile)m;

                            // AccessLevel restricted?  1=playeronly 2=staffonly 3=counseloronly 4=gmonly 5=seeronly 6=adminonly
                            if( passed && Access > 0 )
                            {
                                if( Access == 1 && b.AccessLevel != AccessLevel.Player )
                                    passed = false;
                                else if( Access == 2 && b.AccessLevel == AccessLevel.Player )
                                    passed = false;
                                else if( Access == 3 && b.AccessLevel != AccessLevel.Counselor )
                                    passed = false;
                                else if( Access == 4 && b.AccessLevel != AccessLevel.GameMaster )
                                    passed = false;
                                else if( Access == 5 && b.AccessLevel != AccessLevel.Seer )
                                    passed = false;
                                else if( Access == 6 && b.AccessLevel != AccessLevel.Administrator )
                                    passed = false;
                                else if( Access == 7 && b.AccessLevel != AccessLevel.Developer )
                                    passed = false;
                                else if( Access == 8 && b.AccessLevel != AccessLevel.Owner )
                                    passed = false;
                            }

                            // criminal restricted?  1=innocentonly 2=criminalonly 3=murdereronly
                            if( passed && Criminal > 0 )
                            {
                                int noto = NotorietyHandlers.MobileNotoriety( b, b );

                                if( Criminal == 1 && noto != Notoriety.Innocent )
                                    passed = false;
                                else if( Criminal == 2 && noto != Notoriety.Criminal && noto != Notoriety.Murderer )
                                    passed = false;
                                else if( Criminal == 3 && noto != Notoriety.Murderer )
                                    passed = false;
                            }
                        }

                        // apply basecreature filters
                        if( m is BaseCreature )
                        {
                            BaseCreature b = (BaseCreature)m;

                            // controlled restricted?  1=controlledonly 2=notcontrolled
                            if( passed && Controlled > 0 )
                            {
                                if( Controlled == 1 && !b.Controlled )
                                    passed = false;
                                else if( Controlled == 2 && b.Controlled )
                                    passed = false;
                            }

                            // innocent restricted?  1=innocentonly 2=invulnerable 3=attackable
                            if( passed && Innocent > 0 )
                            {
                                int noto = NotorietyHandlers.MobileNotoriety( b, b );

                                if( Innocent == 1 && noto != Notoriety.Innocent )
                                {
                                    passed = false;
                                }
                                else if( Innocent == 2 && noto != Notoriety.Invulnerable )
                                {
                                    passed = false;
                                }
                                else if( Innocent == 3 && noto != Notoriety.CanBeAttacked )
                                {
                                    passed = false;
                                }
                            }
                        }

                        // add it to the array if it passed all the tests
                        if( passed )
                        {
                            tmparray.Add( m );
                        }
                    }
                }
                else if( type == typeof( Item ) || type.IsSubclassOf( typeof( Item ) ) )
                {
                    List<Item> itemarray = new List<Item>( World.Items.Values );

                    foreach( var m in itemarray )
                    {
                        if( m.Map == null || m.Map == Map.Internal )
                            continue;

                        bool passed = true;

                        if( m.Map.MapID != SelectedMap && SelectedMap != -1 )
                        {
                            passed = false;
                        }

                        if( passed && type != m.GetType() && !m.GetType().IsSubclassOf( type ) )
                        {
                            passed = false;
                        }

                        // movable restricted?  1=movableonly 2=not movable only
                        if( passed && Movable > 0 )
                        {
                            if( Movable == 1 && !m.Movable )
                            {
                                passed = false;
                            }
                            else if( Movable == 2 && m.Movable )
                            {
                                passed = false;
                            }
                        }

                        // visible restricted?  1=visibleonly 2=not visible only
                        if( passed && Visible > 0 )
                        {
                            if( Visible == 1 && !m.Visible )
                            {
                                passed = false;
                            }
                            else if( Visible == 2 && m.Visible )
                            {
                                passed = false;
                            }
                        }

                        // blessed restricted?  1=blessedonly 2=notblessed
                        if( passed && Blessed > 0 )
                        {
                            if( Blessed == 1 && m.LootType != LootType.Blessed )
                            {
                                passed = false;
                            }
                            else if( Blessed == 2 && m.LootType == LootType.Blessed )
                            {
                                passed = false;
                            }
                        }

                        // static restricted?  1=staticsonly 2=no statics
                        if( passed && Statics > 0 )
                        {
                            if( Statics == 1 && !( m is Static ) )
                            {
                                passed = false;
                            }
                            else if( Statics == 2 && m is Static )
                            {
                                passed = false;
                            }
                        }

                        // container restricted?  1=incontaineronly 2=not in containers
                        if( passed && InContainers > 0 )
                        {
                            if( InContainers == 1 && !( m.Parent is Container ) )
                            {
                                passed = false;
                            }
                            else if( InContainers == 2 && m.Parent is Container )
                            {
                                passed = false;
                            }
                        }

                        // carried by mobiles restricted?  1=carriedonly 2=not carried
                        if( passed && Carried > 0 )
                        {
                            if( Carried == 1 && !( m.RootParent is Mobile ) )
                            {
                                passed = false;
                            }
                            else if( Carried == 2 && m.RootParent is Mobile )
                            {
                                passed = false;
                            }
                        }

                        // itemid restricted? 
                        if( passed && ItemID >= 0 && ItemID != m.ItemID )
                        {
                            passed = false;
                        }

                        if( passed )
                        {
                            tmparray.Add( m );
                        }
                    }
                }
            }

            ObjectData[] objectlist = new ObjectData[ tmparray.Count ];

            for( int i = 0; i < tmparray.Count; i++ )
            {
                if( tmparray[ i ] is Mobile )
                {
                    Mobile m = tmparray[ i ] as Mobile;

                    if( m == null )
                        continue;

                    string name = m.Name ?? m.GetType().Name;

                    // append a title if they have one
                    if( m.Title != null )
                        name += " " + m.Title;

                    // fill the return data array
                    objectlist[ i ] = new ObjectData( (short)m.Location.X, (short)m.Location.Y, (byte)m.Map.MapID, name );
                }
                else if( tmparray[ i ] is Item )
                {
                    Item m = tmparray[ i ] as Item;

                    // fill the return data array
                    if( m == null )
                        continue;

                    string name = m.Name ?? m.GetType().Name;

                    short x = (short)m.Location.X;
                    short y = (short)m.Location.Y;
                    if( m.RootParent is Mobile )
                    {
                        x = (short)( (Mobile)m.RootParent ).Location.X;
                        y = (short)( (Mobile)m.RootParent ).Location.Y;
                    }
                    else if( m.RootParent is Container )
                    {
                        x = (short)( (Container)m.RootParent ).Location.X;
                        y = (short)( (Container)m.RootParent ).Location.Y;
                    }


                    objectlist[ i ] = new ObjectData( x, y, (byte)m.Map.MapID, name );
                }
            }

            return new ReturnObjectData( objectlist );
        }
    }
}