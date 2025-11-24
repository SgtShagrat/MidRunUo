/***************************************************************************
 *                                 Poison.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: Poison.cs 511 2010-04-25 06:09:43Z mark $
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;

namespace Server
{
    [Parsable]
    public abstract class Poison
    {
        /*public abstract TimeSpan Interval{ get; }
        public abstract TimeSpan Duration{ get; }*/
        public abstract string Name { get; }
        public abstract string NameIt { get; }
        public abstract int Level { get; }
        public abstract Timer ConstructTimer( Mobile m );
        /*public abstract void OnDamage( Mobile m, ref object state );*/

        public override string ToString()
        {
            return this.Name;
        }


        private static List<Poison> m_Poisons = new List<Poison>();

        public static void Register( Poison reg )
        {
            string regName = reg.Name.ToLower();

            for ( int i = 0; i < m_Poisons.Count; i++ )
            {
                if ( reg.Level == m_Poisons[i].Level )
                    throw new Exception( "A poison with that level already exists." );
                else if ( regName == m_Poisons[i].Name.ToLower() )
                    throw new Exception( "A poison with that name already exists." );
            }

            m_Poisons.Add( reg );
        }

        public static Poison Lesser { get { return GetPoison( "Lesser" ); } }
        public static Poison Regular { get { return GetPoison( "Regular" ); } }
        public static Poison Greater { get { return GetPoison( "Greater" ); } }
        public static Poison Deadly { get { return GetPoison( "Deadly" ); } }
        public static Poison Lethal { get { return GetPoison( "Lethal" ); } }

		#region Mondain's Legacy
		public abstract int RealLevel{ get; }
		public abstract int LabelNumber{ get; }

		public static Poison Parasitic{ get{ return GetPoison( "DeadlyParasitic" ); } }
		public static Poison Darkglow{ get{ return GetPoison( "GreaterDarkglow" ); } }
		#endregion
		#region Poison Engine [Arlas]
		public static Poison MagiaLesser { get { return GetPoison( "MagiaLesser" ); } }
		public static Poison MagiaRegular { get { return GetPoison( "MagiaRegular" ); } }
		public static Poison MagiaGreater { get { return GetPoison( "MagiaGreater" ); } }
		public static Poison MagiaDeadly { get { return GetPoison( "MagiaDeadly" ); } }
		public static Poison MagiaLethal { get { return GetPoison( "MagiaLethal" ); } }

		public static Poison StanchezzaLesser { get { return GetPoison( "StanchezzaLesser" ); } }
		public static Poison StanchezzaRegular { get { return GetPoison( "StanchezzaRegular" ); } }
		public static Poison StanchezzaGreater { get { return GetPoison( "StanchezzaGreater" ); } }
		public static Poison StanchezzaDeadly { get { return GetPoison( "StanchezzaDeadly" ); } }
		public static Poison StanchezzaLethal { get { return GetPoison( "StanchezzaLethal" ); } }

		public static Poison ParalisiLesser { get { return GetPoison( "ParalisiLesser" ); } }
		public static Poison ParalisiRegular { get { return GetPoison( "ParalisiRegular" ); } }
		public static Poison ParalisiGreater { get { return GetPoison( "ParalisiGreater" ); } }
		public static Poison ParalisiDeadly { get { return GetPoison( "ParalisiDeadly" ); } }
		public static Poison ParalisiLethal { get { return GetPoison( "ParalisiLethal" ); } }

		public static Poison BloccoLesser { get { return GetPoison( "BloccoLesser" ); } }
		public static Poison BloccoRegular { get { return GetPoison( "BloccoRegular" ); } }
		public static Poison BloccoGreater { get { return GetPoison( "BloccoGreater" ); } }
		public static Poison BloccoDeadly { get { return GetPoison( "BloccoDeadly" ); } }
		public static Poison BloccoLethal { get { return GetPoison( "BloccoLethal" ); } }

		public static Poison LentezzaLesser { get { return GetPoison( "LentezzaLesser" ); } }
		public static Poison LentezzaRegular { get { return GetPoison( "LentezzaRegular" ); } }
		public static Poison LentezzaGreater { get { return GetPoison( "LentezzaGreater" ); } }
		public static Poison LentezzaDeadly { get { return GetPoison( "LentezzaDeadly" ); } }
		public static Poison LentezzaLethal { get { return GetPoison( "LentezzaLethal" ); } }
		#endregion

        public static List<Poison> Poisons
        {
            get
            {
                return m_Poisons;
            }
        }

        public static Poison Parse( string value )
        {
            Poison p = null;

            int plevel;

            if ( int.TryParse( value, out plevel ) )
                p = GetPoison( plevel );

            if ( p == null )
                p = GetPoison( value );

            return p;
        }

        public static Poison GetPoison( int level )
        {
            for ( int i = 0; i < m_Poisons.Count; ++i )
            {
                Poison p = m_Poisons[i];

                if ( p.Level == level )
                    return p;
            }

            return null;
        }

        public static Poison GetPoison( string name )
        {
            for ( int i = 0; i < m_Poisons.Count; ++i )
            {
                Poison p = m_Poisons[i];

                if ( Utility.InsensitiveCompare( p.Name, name ) == 0 )
                    return p;
            }

            return null;
        }

        public static void Serialize( Poison p, GenericWriter writer )
        {
            if ( p == null )
            {
                writer.Write( (byte)0 );
            }
            else
            {
                writer.Write( (byte)1 );
                writer.Write( (byte)p.Level );
            }
        }

        public static Poison Deserialize( GenericReader reader )
        {
            switch ( reader.ReadByte() )
            {
                case 1: return GetPoison( reader.ReadByte() );
                case 2:
                    //no longer used, safe to remove?
                    reader.ReadInt();
                    reader.ReadDouble();
                    reader.ReadInt();
                    reader.ReadTimeSpan();
                    break;
            }
            return null;
        }

        public abstract int GetResistDifficulty(); // mod by Dies Irae
    }
}
