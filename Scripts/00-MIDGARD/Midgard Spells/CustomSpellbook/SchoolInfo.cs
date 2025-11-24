using System;

using Midgard.Engines.Classes;

using Server;

namespace Midgard.Engines.SpellSystem
{
    public class SchoolInfo : IComparable<SchoolInfo>
    {
        public static readonly SchoolInfo None = new EmptySchool();
        public static readonly SchoolInfo Options = new OptionSchool();

        public static readonly SchoolInfo PaladinSchoolInfo = new PaladinSchool();
        public static readonly SchoolInfo DruidSchoolInfo = new DruidSchool();
        public static readonly SchoolInfo NecromancerSchoolInfo = new NecromancerSchool();

        public static readonly SchoolInfo[] SchoolList = new SchoolInfo[]
		{
			None,
            Options,
            DruidSchoolInfo,
            PaladinSchoolInfo,
            NecromancerSchoolInfo
        };

        public static int FindBackgroundInfo( SchoolFlag flag )
        {
            for( int i = 0; i < SchoolList.Length; i++ )
            {
                SchoolInfo si = SchoolList[ i ];
                if( si != null && si.School == flag )
                    return si.Background;
            }

            return -1;
        }

        public static int GetIndex( SchoolInfo info )
        {
            for( int i = 0; i < SchoolList.Length; i++ )
            {
                if( info == SchoolList[ i ] )
                    return i;
            }

            return -1;
        }

        public virtual string Name
        {
            get { return "None"; }
        }

        public virtual SchoolFlag School
        {
            get { return SchoolFlag.None; }
        }

        public virtual ClassSystem System
        {
            get { return null; }
        }

        public virtual bool ReqScrolls
        {
            get { return false; }
        }

        public virtual int Background
        {
            get { return 0; }
        }

        private int[] m_Range;

        public int[] Range
        {
            get
            {
                if( m_Range == null || m_Range.Length == 0 )
                    m_Range = RPGSpellsSystem.GetIdsFromSystem( System );

                return m_Range;
            }
        }


        #region serialization
        public static void WriteReference( GenericWriter writer, SchoolInfo school )
        {
            int idx = Array.IndexOf( SchoolList, school );

            writer.WriteEncodedInt( idx + 1 );
        }

        public static SchoolInfo ReadReference( GenericReader reader )
        {
            int idx = reader.ReadEncodedInt() - 1;

            if( idx >= 0 && idx < SchoolList.Length )
                return SchoolList[ idx ];

            return null;
        }
        #endregion

        #region IComparable methods
        public int CompareTo( SchoolInfo other )
        {
            if( other == null )
                return -1;

            return other.School.CompareTo( School );
        }
        #endregion
    }

    public sealed class OptionSchool : SchoolInfo
    {
        public override string Name
        {
            get { return "Options"; }
        }
    }

    public sealed class EmptySchool : SchoolInfo
    {
        public override string Name
        {
            get { return "None"; }
        }
    }
}