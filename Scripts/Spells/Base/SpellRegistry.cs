using System;
using System.Collections.Generic;
using Midgard.Engines.SpellSystem;
using Server.Spells.Bushido;
using Server.Spells.Chivalry;
using Server.Items;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;

namespace Server.Spells
{
	public class SpellRegistry
	{
		private static Type[] m_Types = new Type[700];
		private static int m_Count;

		public static Type[] Types
		{
			get
			{
				m_Count = -1;
				return m_Types;
			}
		}
		
		//What IS this used for anyways.
		public static int Count
		{
			get
			{
				if ( m_Count == -1 )
				{
					m_Count = 0;

					for ( int i = 0; i < m_Types.Length; ++i )
						if ( m_Types[i] != null )
							++m_Count;
				}

				return m_Count;
			}
		}

		private static Dictionary<Type, Int32> m_IDsFromTypes = new Dictionary<Type, Int32>( m_Types.Length );
		
		private static Dictionary<Int32, SpecialMove> m_SpecialMoves = new Dictionary<Int32, SpecialMove>();
		public static Dictionary<Int32, SpecialMove> SpecialMoves { get { return m_SpecialMoves; } }

		public static int GetRegistryNumber( ISpell s )
		{
			return GetRegistryNumber( s.GetType() );
		}

		public static int GetRegistryNumber( SpecialMove s )
		{
			return GetRegistryNumber( s.GetType() );
		}

		public static int GetRegistryNumber( Type type )
		{
			if( m_IDsFromTypes.ContainsKey( type ) )
				return m_IDsFromTypes[type];

			return -1;
		}

        #region mod by Dies Irae
        public static Type GetTypeFromRegNumber( int spellID )
        {
            if( spellID < 0 || spellID > m_Types.Length )
                return null;

            return Types[ spellID ];
        }

        private static Dictionary<Int32, Spell> m_SpellInstances = new Dictionary<Int32, Spell>();
        public static Dictionary<Int32, Spell> SpellInstances { get { return m_SpellInstances; } }

        private static void AddSpellInstance( int spellID, Type type )
        {
            if( !type.IsSubclassOf( typeof( SpecialMove ) ) )
            {
                Spell spell = NewSpell( spellID, null, null );

                if( spell != null )
                {
                    Console.WriteLine( "Added spell {0}", type.Name );
                    m_SpellInstances.Add( spellID, spell );
                }
                else
                {
                    Console.WriteLine( "Warning: spell {0} is null instanced.", type.Name );
                }
            }
        }

        public static Spell GetSpellByID( int spellID )
        {
            if( spellID < 0 || spellID >= m_Types.Length )
                return null;

            Type type = m_Types[ spellID ];

            if( type == null || type.IsSubclassOf( typeof( SpecialMove ) ) || !m_SpellInstances.ContainsKey( spellID ) )
                return null;

            return m_SpellInstances[ spellID ];
        }

        public static Spell GetSpellByType( Type type )
        {
            if( type == null || type.IsSubclassOf( typeof( SpecialMove ) ) )
                return null;

            int index = GetRegistryNumber( type );

            if( index > -1 )
            {
                if( !m_SpellInstances.ContainsKey( index ) )
                    AddSpellInstance( index, type );

                if( m_SpellInstances.ContainsKey( index ) )
                    return m_SpellInstances[ index ];
            }

            return null;
        }

        public static SpellInfo GetSpellInfoByType( Type type )
        {
            if( type == null || type.IsSubclassOf( typeof( SpecialMove ) ) )
                return null;

            int index = GetRegistryNumber( type );

            if( index > -1 )
            {
                if( !m_SpellInstances.ContainsKey( index ) )
                    AddSpellInstance( index, type );

                if( m_SpellInstances.ContainsKey( index ) )
                    return m_SpellInstances[ index ].Info;
            }

            return null;
        }

        public static SpellInfo GetSpellInfoByID( int spellID )
        {
            if( spellID < 0 || spellID >= m_Types.Length )
                return null;

            return m_SpellInstances[ spellID ].Info;
        }

        public static ExtendedSpellInfo GetExtendedSpellInfoByType( Type type )
        {
            if( type == null )
                return null;

            Spell spell = GetSpellByType( type );

            if( spell != null && spell is ICustomSpell )
                return ( (ICustomSpell)spell ).ExtendedInfo;
            else
                return null;
        }

        public static ExtendedSpellInfo GetExtendedSpellInfoByID( int spellID )
        {
            if( spellID < 0 || spellID >= m_Types.Length )
                return null;

            Spell spell = GetSpellByID( spellID );

            if( spell != null && spell is ICustomSpell )
                return ( (ICustomSpell)spell ).ExtendedInfo;
            else
                Console.WriteLine( "Warning: trying to get extended info from spellID {0}", spellID );

            return null;
        }
        #endregion

		public static void Register( int spellID, Type type )
		{
			if ( spellID < 0 || spellID >= m_Types.Length )
				return;

			if ( m_Types[spellID] == null )
				++m_Count;

			m_Types[spellID] = type;

			if( !m_IDsFromTypes.ContainsKey( type ) )
				m_IDsFromTypes.Add( type, spellID );

			if( type.IsSubclassOf( typeof( SpecialMove ) ) )
			{
				SpecialMove spm = null;

				try
				{
					spm = Activator.CreateInstance( type ) as SpecialMove;
				}
				catch
				{
				}

				if( spm != null )
					m_SpecialMoves.Add( spellID, spm );
			}
            else
			{
                Spell spell = null;

				m_Params[0] = null;
				m_Params[1] = null;

				try
				{
					spell = Activator.CreateInstance( type, m_Params ) as Spell;
				}
                catch( Exception ex )
                {
                    Console.WriteLine( "SpellID: " + spellID + " SpellType: " + type.Name );
                    Console.WriteLine( ex.ToString() );
                    Console.ReadKey();
                }

                if( spell != null )
                    m_SpellInstances.Add( spellID, spell );

                #region mod by Dies Irae
                if( Core.Debug )
                {
                    if( spell != null )
                    {
                        SpellInfo info = spell.Info;
                        Utility.Log( "SpellsInfo.log", "{0}\taction\t{1},\tLH:\t{2}:\tRH:\t{3}",
                                     info.Name, info.Action.ToString(), info.LeftHandEffect.ToString(), info.RightHandEffect.ToString() );
                    }

                    Utility.Log( "SpellRegistry.log", "registered: {0} id->{1}", type.Name, spellID.ToString() );
                    if( spellID == -1 )
                        Utility.Log( "SpellRegistry.log", "Warning: trying to register {0} with -1 spellID", type.Name );
                }
                #endregion
            }
		}

		public static SpecialMove GetSpecialMove( int spellID )
		{
			if ( spellID < 0 || spellID >= m_Types.Length )
				return null;

			Type t = m_Types[spellID];

			if ( t == null || !t.IsSubclassOf( typeof( SpecialMove ) ) || !m_SpecialMoves.ContainsKey( spellID ) )
				return null;

			return m_SpecialMoves[spellID];
		}

		private static object[] m_Params = new object[2];

        public static Spell NewSpell( int spellID, Mobile caster, Item scroll )
        {
            if( spellID < 0 || spellID >= m_Types.Length )
            {
                Console.WriteLine( "Warning: NewSpell {0}/{1}", spellID, m_Types.Length );
                return null;
            }

            Type t = m_Types[ spellID ];

            if( t != null && !t.IsSubclassOf( typeof( SpecialMove ) ) )
            {
                m_Params[ 0 ] = caster;
                m_Params[ 1 ] = scroll;

                try
                {
                    return (Spell)Activator.CreateInstance( t, m_Params );
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                    Console.ReadKey();
                }
            }

            return null;
        }

		private static string[] m_CircleNames = new string[]
			{
				"First",
				"Second",
				"Third",
				"Fourth",
				"Fifth",
				"Sixth",
				"Seventh",
				"Eighth",
				"Necromancy",
				"Chivalry",
				"Bushido",
				"Ninjitsu",
				"Spellweaving"
			};

		public static Spell NewSpell( string name, Mobile caster, Item scroll )
		{
			for ( int i = 0; i < m_CircleNames.Length; ++i )
			{
				Type t = ScriptCompiler.FindTypeByFullName( String.Format( "Server.Spells.{0}.{1}", m_CircleNames[i], name ) );

			    #region mod by Dies Irae
			    if( t == null )
			        t = ScriptCompiler.FindTypeByFullName( String.Format( "Midgard.Engines.SpellSystem.{0}", name ) );
			    #endregion
				
                if ( t != null && !t.IsSubclassOf( typeof( SpecialMove ) ) )
				{
					m_Params[0] = caster;
					m_Params[1] = scroll;

					try
					{
						return (Spell)Activator.CreateInstance( t, m_Params );
					}
					catch
					{
					}
				}
			}

			return null;
		}
	}
}