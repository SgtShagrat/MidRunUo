/***************************************************************************
 *                                  DisguiseEntry.cs
 *                            		-------------------
 *  begin                	: Mese, 2000
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using Server;
using Server.Mobiles;

namespace Midgard.Engines.AdvancedDisguise
{
    public class DisguiseEntry
    {
        public enum InfoTypes
        {
            Physical,
            Reputation,
            Town,
            Other
        }

        public Mobile AliasSource { get; set; }
        public int AliasSkinHue { get; set; }
        public int AliasFacialHairHue { get; set; }
        public int AliasFacialHairID { get; set; }
        public int AliasHairHue { get; set; }
        public int AliasHairID { get; set; }
        public string AliasName { get; set; }
        public int AliasFame { get; set; }
        public int AliasKarma { get; set; }
        public int AliasBodyMod { get; set; }
        public MidgardTowns AliasTown { get; set; }

        public bool IsFullEntry
        {
            get
            {
                return AliasSkinHue != -1 &&
                        AliasFacialHairHue != -1 &&
                        AliasFacialHairID != -1 &&
                        AliasHairHue != -1 &&
                        AliasHairID != -1 &&
                        !String.IsNullOrEmpty( AliasName ) &&
                        AliasFame != -1 &&
                        AliasKarma != -1 &&
                        AliasBodyMod != -1 &&
                        AliasTown != MidgardTowns.None;
            }
        }

        public DisguiseEntry()
        {
            InitializeEntry();
        }

        private void InitializeEntry()
        {
            AliasSource = null;
            AliasSkinHue = -1;
            AliasFacialHairHue = -1;
            AliasFacialHairID = -1;
            AliasHairHue = -1;
            AliasHairID = -1;
            AliasName = String.Empty;
            AliasFame = -1;
            AliasKarma = -1;
            AliasBodyMod = -1;
            AliasTown = MidgardTowns.None;
        }

        public InfoTypes NextTypeRequired()
        {
            if( !HasInfoType( this, InfoTypes.Physical ) )
                return InfoTypes.Reputation;
            else if( !HasInfoType( this, InfoTypes.Reputation ) )
                return InfoTypes.Town;
            else
                return InfoTypes.Physical;
        }

        public bool HasInfoType( DisguiseEntry entry, InfoTypes type )
        {
            if( entry == null || entry.AliasSource == null )
                return false;

            switch( type )
            {
                case InfoTypes.Physical:
                    {
                        return ( entry.AliasBodyMod != -1 &&
                                 entry.AliasSkinHue != -1 &&
                                 entry.AliasFacialHairHue != -1 &&
                                 entry.AliasFacialHairID != -1 &&
                                 entry.AliasHairHue != -1 &&
                                 entry.AliasHairID != -1 &&
                                 !String.IsNullOrEmpty( entry.AliasName ) );
                    }
                case InfoTypes.Reputation:
                    {
                        return ( entry.AliasFame != -1 && entry.AliasKarma != -1 );
                    }
                case InfoTypes.Town:
                    {
                        return entry.AliasTown != MidgardTowns.None;
                    }
                default:
                    return false;
            }
        }

        public static void AdvanceEntry( DisguiseEntry entry, Mobile mobile )
        {
            if( entry == null || mobile == null )
                return;

            if( !( mobile is Midgard2PlayerMobile ) )
                return;

            if( !entry.HasInfoType( entry, InfoTypes.Physical ) )
            {
                entry.AliasBodyMod = ( mobile.Female ? 1888 : 1889 );
                entry.AliasSkinHue = mobile.Hue;

                if( mobile.Race == Race.Human )
                    entry.AliasSkinHue -= 32768;

                entry.AliasFacialHairHue = mobile.FacialHairHue;
                entry.AliasFacialHairID = mobile.FacialHairItemID;
                entry.AliasHairHue = mobile.HairHue;
                entry.AliasHairID = mobile.HairItemID;
                entry.AliasName = !String.IsNullOrEmpty( mobile.Name ) ? mobile.Name : "unknown";
            }
            else if( !entry.HasInfoType( entry, InfoTypes.Reputation ) )
            {
                entry.AliasFame = mobile.Fame;
                entry.AliasKarma = mobile.Karma;
            }
            else if( !entry.HasInfoType( entry, InfoTypes.Town ) )
            {
                entry.AliasTown = ( (Midgard2PlayerMobile)mobile ).Town;
            }
        }

        public bool IsValid()
        {
            return AliasSource != null && AliasSource.Player && AliasSource.AccessLevel == AccessLevel.Player;
        }

        #region serial-deserial
        public void Serialize( GenericWriter writer )
        {
            writer.Write( 0 ); // version

            writer.WriteMobile( AliasSource );
            writer.Write( AliasSkinHue );
            writer.Write( AliasFacialHairHue );
            writer.Write( AliasFacialHairID );
            writer.Write( AliasHairHue );
            writer.Write( AliasHairID );
            writer.Write( AliasName );
            writer.Write( AliasFame );
            writer.Write( AliasKarma );
            writer.Write( AliasBodyMod );
            writer.Write( (int) AliasTown );
        }

        public DisguiseEntry( GenericReader reader )
        {
            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        AliasSource = reader.ReadMobile();
                        AliasSkinHue = reader.ReadInt();
                        AliasFacialHairHue = reader.ReadInt();
                        AliasFacialHairID = reader.ReadInt();
                        AliasHairHue = reader.ReadInt();
                        AliasHairID = reader.ReadInt();
                        AliasName = reader.ReadString();
                        AliasFame = reader.ReadInt();
                        AliasKarma = reader.ReadInt();
                        AliasBodyMod = reader.ReadInt();

                        int town = reader.ReadInt();
                        if( Enum.IsDefined( typeof( MidgardTowns ), town ) )
                            AliasTown = (MidgardTowns)town;
                        else
                            AliasTown = MidgardTowns.None;
 
                        break;
                    }
            }
        }
        #endregion
    }
}