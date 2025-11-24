using System;
using System.Reflection;
using Server;
using Server.Commands;
using Server.Engines.XmlSpawner2;
using CPA = Server.CommandPropertyAttribute;

namespace Midgard.Engines.RandomEncounterSystem
{
    public class XmlSpawnerStub
    {
        public static object CreateObject( Type type, string[] typewordargs, bool requireconstructable,
                                          bool requireattachable )
        {
            if( type == null )
                return null;

            object o = null;

            int typearglen = 0;
            if( typewordargs != null )
                typearglen = typewordargs.Length;

            // ok, there are args in the typename, so we need to invoke the proper constructor
            ConstructorInfo[] ctors = type.GetConstructors();

            if( ctors == null )
                return null;

            // go through all the constructors for this type
            for( int i = 0; i < ctors.Length; ++i )
            {
                ConstructorInfo ctor = ctors[ i ];

                if( ctor == null )
                    continue;

                // if both requireconstructable and requireattachable are true, then allow either condition
                if( !( requireconstructable && Add.IsConstructable( ctor, AccessLevel.Player ) ) &&
                    !( requireattachable && XmlAttach.IsAttachable( ctor ) ) )
                    continue;

                // check the parameter list of the constructor
                ParameterInfo[] paramList = ctor.GetParameters();

                // and compare with the argument list provided
                if( paramList != null && typearglen == paramList.Length )
                {
                    // this is a constructor that takes args and matches the number of args passed in to CreateObject
                    if( paramList.Length > 0 )
                    {
                        object[] paramValues = null;

                        try
                        {
                            paramValues = Add.ParseValues( paramList, typewordargs );
                        }
                        catch
                        {
                        }

                        if( paramValues == null )
                            continue;

                        // ok, have a match on args, so try to construct it
                        try
                        {
                            o = Activator.CreateInstance( type, paramValues );
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        // zero argument constructor
                        try
                        {
                            o = Activator.CreateInstance( type );
                        }
                        catch
                        {
                        }
                    }

                    // successfully constructed the object, otherwise try another matching constructor
                    if( o != null )
                        break;
                }
            }

            return o;
        }
    }
}