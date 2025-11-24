/***************************************************************************
 *                                  TownAccessFlag.cs
 *                            		-----------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using Server;

namespace Midgard.Engines.MidgardTownSystem
{
    [Flags]
    public enum TownAccessFlags
    {
        None = 0,

        RemoveCitizen = 1 << 0,
        BanCitizen = 1 << 1,
        SetTownOffice = 1 << 2,
        SetCustomTownOffice = 1 << 3,
        SetProfession = 1 << 4,
        SetCustomProfession = 1 << 5,
        SetInfo = 1 << 6,
        ClearNews = 1 << 7,
        RemoveInactiveStates = 1 << 8,
        ViewPrivateInfo = 1 << 9,
        SetTownAccess = 1 << 10,
        CanEditItemPrice = 1 << 11,
        AccessDefaultMenu = 1 << 12,
        DecorateTown = 1 << 13,
        CommandGuards = 1 << 14,
        CanEditWarFare = 1 << 15,
        PermaBanCitizen = 1 << 16,

        Citizen = AccessDefaultMenu,

        Staff = Citizen | RemoveCitizen | BanCitizen | SetCustomTownOffice |
                SetCustomProfession | ClearNews | RemoveInactiveStates |
                ViewPrivateInfo | SetTownAccess | CanEditItemPrice | PermaBanCitizen
    }

    [PropertyObject]
    public class TownAccessLevel
    {
        public static TownAccessLevel[] Levels = new TownAccessLevel[]
			{
                new TownAccessLevel( "None", 0, TownAccessFlags.None ),
                new TownAccessLevel( "Citizen", 1, TownAccessFlags.Citizen ),
                new TownAccessLevel( "Staff", 2, TownAccessFlags.Staff ),
            };

        public static TownAccessLevel None { get { return Levels[ 0 ]; } }
        public static TownAccessLevel Citizen { get { return Levels[ 1 ]; } }
        public static TownAccessLevel Staff { get { return Levels[ 2 ]; } }

        public TextDefinition Name { get; private set; }
        public int Level { get; private set; }
        public TownAccessFlags Flags { get; private set; }

        public TownAccessLevel( TextDefinition name, int level, TownAccessFlags flags )
        {
            Name = name;
            Level = level;
            Flags = flags;
        }

        public bool GetFlag( TownAccessFlags flag )
        {
            return ( ( Flags & flag ) != 0 );
        }

        public void SetFlag( TownAccessFlags flag, bool value )
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
            Flags = TownAccessFlags.None;
        }

        public void SetCitizen()
        {
            Flags = TownAccessFlags.Citizen;
        }

        public void SetStaff()
        {
            Flags = TownAccessFlags.Staff;
        }

        public static int GetTownFlags()
        {
            return 15;
        }

        public static int GetTownAccessLevel()
        {
            return 2;
        }

        public static int GetMaxFlagValue()
        {
            return (int)TownAccessFlags.CanEditWarFare;
        }

        #region serialization
        public TownAccessLevel( GenericReader reader )
        {
            Flags = (TownAccessFlags)reader.ReadInt();
        }

        public void Serialize( GenericWriter writer )
        {
            writer.Write( (int)Flags );
        }
        #endregion
    }
}