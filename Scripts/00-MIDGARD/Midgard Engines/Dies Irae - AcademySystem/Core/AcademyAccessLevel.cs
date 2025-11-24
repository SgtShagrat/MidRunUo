/***************************************************************************
 *                               AcademyAccessLevel.cs
 *
 *   begin                : 05 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using Server;

namespace Midgard.Engines.Academies
{
    [Flags]
    public enum AcademyAccessFlags
    {
        None = 0,

        RemoveAcademic = 1 << 0,
        BanAcademic = 1 << 1,
        SetAcademyOffice = 1 << 2,
        SetInfo = 1 << 3,
        ClearNews = 1 << 4,
        RemoveInactiveStates = 1 << 5,
        ViewPrivateInfo = 1 << 6,
        SetAcademyAccess = 1 << 7,
        AccessDefaultMenu = 1 << 8,

        Academic = AccessDefaultMenu,

        Staff = Academic | RemoveAcademic | BanAcademic | ClearNews | 
                RemoveInactiveStates | ViewPrivateInfo | SetAcademyAccess
    }

    [PropertyObject]
    public class AcademyAccessLevel
    {
        public static AcademyAccessLevel[] Levels = new AcademyAccessLevel[]
                                                    {
                                                        new AcademyAccessLevel( "None", 0, AcademyAccessFlags.None ),
                                                        new AcademyAccessLevel( "Academic", 1, AcademyAccessFlags.Academic ),
                                                        new AcademyAccessLevel( "Staff", 2, AcademyAccessFlags.Staff ),
                                                    };

        public static AcademyAccessLevel None { get { return Levels[ 0 ]; } }
        public static AcademyAccessLevel Academic { get { return Levels[ 1 ]; } }
        public static AcademyAccessLevel Staff { get { return Levels[ 2 ]; } }

        public TextDefinition Name { get; private set; }
        public int Level { get; private set; }
        public AcademyAccessFlags Flags { get; private set; }

        public AcademyAccessLevel( TextDefinition name, int level, AcademyAccessFlags flags )
        {
            Name = name;
            Level = level;
            Flags = flags;
        }

        public bool GetFlag( AcademyAccessFlags flag )
        {
            return ( ( Flags & flag ) != 0 );
        }

        public void SetFlag( AcademyAccessFlags flag, bool value )
        {
            if( value )
                Flags |= flag;
            else
                Flags &= ~flag;
        }

        public override string ToString()
        {
            return Name;
        }

        public void ResetFlags()
        {
            Flags = AcademyAccessFlags.None;
        }

        public void SetAcademic()
        {
            Flags = AcademyAccessFlags.Academic;
        }

        public void SetStaff()
        {
            Flags = AcademyAccessFlags.Staff;
        }

        public static int GetAcademyFlags()
        {
            return 12;
        }

        public static int GetAcademyAccessLevel()
        {
            return 2;
        }

        public static int GetMaxFlagValue()
        {
            return (int)AcademyAccessFlags.AccessDefaultMenu;
        }

        #region serialization
        public AcademyAccessLevel( GenericReader reader )
        {
            Flags = (AcademyAccessFlags)reader.ReadInt();
        }

        public void Serialize( GenericWriter writer )
        {
            writer.Write( (int)Flags );
        }
        #endregion
    }
}