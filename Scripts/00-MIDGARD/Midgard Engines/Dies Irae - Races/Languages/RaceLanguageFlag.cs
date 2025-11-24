/***************************************************************************
 *                                  RaceLanguageFlag.cs
 *                            		-------------------
 *  begin                	: Aprile, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using Server;

namespace Midgard.Engines.Races
{
    [Flags]
    public enum RaceLanguageFlag
    {
        None = 0x00000000,

        HighElf = 0x00000001,
        NorthernElf = 0x00000002,
        MountainDwarf = 0x00000004,
        FairyOfWood = 0x00000008,
        FairyOfFire = 0x00000010,
        FairyOfWater = 0x00000020,
        FairyOfAir = 0x00000040,
        FairyOfEarth = 0x00000080,
        HighOrc = 0x00000100,
        Sprite = 0x00000200,
        HalfElf = 0x00000400,
        HalfDrow = 0x00000800,
        Vampire = 0x00001000,
        Drow = 0x00002000,
        NorthernHuman = 0x00004000,
        Undead = 0x00008000,
        Naglor = 0x00010000,
        HalfDaemon = 0x00020000,
        Werewolf = 0x00040000
    }

    [PropertyObject]
    public class RaceLanguageAttribute
    {
        private RaceLanguageFlag m_RaceLanguageFlags;
        public Mobile Owner { get; set; }

        public RaceLanguageFlag RaceLanguageFlags
        {
            get { return m_RaceLanguageFlags; }
            set { m_RaceLanguageFlags = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool HighElf
        {
            get { return this[ RaceLanguageFlag.HighElf ]; }
            set { this[ RaceLanguageFlag.HighElf ] = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool NorthernElf
        {
            get { return this[ RaceLanguageFlag.NorthernElf ]; }
            set { this[ RaceLanguageFlag.NorthernElf ] = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool MountainDwarf
        {
            get { return this[ RaceLanguageFlag.MountainDwarf ]; }
            set { this[ RaceLanguageFlag.MountainDwarf ] = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool FairyOfWood
        {
            get { return this[ RaceLanguageFlag.FairyOfWood ]; }
            set { this[ RaceLanguageFlag.FairyOfWood ] = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool FairyOfFire
        {
            get { return this[ RaceLanguageFlag.FairyOfFire ]; }
            set { this[ RaceLanguageFlag.FairyOfFire ] = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool FairyOfWater
        {
            get { return this[ RaceLanguageFlag.FairyOfWater ]; }
            set { this[ RaceLanguageFlag.FairyOfWater ] = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool FairyOfAir
        {
            get { return this[ RaceLanguageFlag.FairyOfAir ]; }
            set { this[ RaceLanguageFlag.FairyOfAir ] = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool FairyOfEarth
        {
            get { return this[ RaceLanguageFlag.FairyOfEarth ]; }
            set { this[ RaceLanguageFlag.FairyOfEarth ] = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool HighOrc
        {
            get { return this[ RaceLanguageFlag.HighOrc ]; }
            set { this[ RaceLanguageFlag.HighOrc ] = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Sprite
        {
            get { return this[ RaceLanguageFlag.Sprite ]; }
            set { this[ RaceLanguageFlag.Sprite ] = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool HalfElf
        {
            get { return this[ RaceLanguageFlag.HalfElf ]; }
            set { this[ RaceLanguageFlag.HalfElf ] = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool HalfDrow
        {
            get { return this[ RaceLanguageFlag.HalfDrow ]; }
            set { this[ RaceLanguageFlag.HalfDrow ] = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Vampire
        {
            get { return this[ RaceLanguageFlag.Vampire ]; }
            set { this[ RaceLanguageFlag.Vampire ] = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Drow
        {
            get { return this[ RaceLanguageFlag.Drow ]; }
            set { this[ RaceLanguageFlag.Drow ] = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool NorthernHuman
        {
            get { return this[ RaceLanguageFlag.NorthernHuman ]; }
            set { this[ RaceLanguageFlag.NorthernHuman ] = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Undead
        {
            get { return this[ RaceLanguageFlag.Undead ]; }
            set { this[ RaceLanguageFlag.Undead ] = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Naglor
        {
            get { return this[ RaceLanguageFlag.Naglor ]; }
            set { this[ RaceLanguageFlag.Naglor ] = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool HalfDaemon
        {
            get { return this[ RaceLanguageFlag.HalfDaemon ]; }
            set { this[ RaceLanguageFlag.HalfDaemon ] = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Werewolf
        {
            get { return this[ RaceLanguageFlag.Werewolf ]; }
            set { this[ RaceLanguageFlag.Werewolf ] = value; }
        }

        public bool this[ RaceLanguageFlag flag ]
        {
            get { return GetFlag( flag ); }
            set { SetFlag( flag, value ); }
        }

        public bool GetFlag( RaceLanguageFlag flag )
        {
            return ( ( m_RaceLanguageFlags & flag ) != 0 );
        }

        public void SetFlag( RaceLanguageFlag flag, bool value )
        {
            if( value )
                m_RaceLanguageFlags |= flag;
            else
                m_RaceLanguageFlags &= ~flag;
        }

        public override string ToString()
        {
            return "...";
        }

        public RaceLanguageAttribute( Mobile owner )
        {
            Owner = owner;
        }

        public void AcquireLanguage( Race race )
        {
            MidgardRace midRace = race as MidgardRace;
            if( midRace == null )
                return;

            this[ midRace.LanguageFlag ] = true;
        }

        public bool KnowsLanguage( Race race )
        {
            MidgardRace midRace = race as MidgardRace;
            if( midRace == null )
                return true;

            return this[ midRace.LanguageFlag ];           
        }

        #region serialization
        public RaceLanguageAttribute( Mobile owner, GenericReader reader )
        {
            Owner = owner;

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        m_RaceLanguageFlags = (RaceLanguageFlag)reader.ReadInt();

                        break;
                    }
            }
        }

        public void Serialize( GenericWriter writer )
        {
            writer.Write( 0 ); // version

            writer.Write( (int)m_RaceLanguageFlags );
        }
        #endregion
    }
}