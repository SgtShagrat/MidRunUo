/***************************************************************************
 *                                  SetRace.cs
 *                            		----------
 *  begin                	: Ottobre, 2006
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 *			Con il comando [Setrace si puo' settare la razza a un mobile.
 * 			Le razze disponibili sono quelle definite in RaceDefinitions.
 *  
 ***************************************************************************/

using Midgard.Items;

using Server;
using Server.Accounting;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

using Midgard.Engines.MidgardTownSystem;

using MidgardClasses = Midgard.Engines.Classes.Classes;

namespace Midgard.Engines.Races
{
    public class SetRace
    {
        public static void RegisterCommands()
        {
            CommandSystem.Register( "SetRace", AccessLevel.Seer, new CommandEventHandler( SetRace_OnCommand ) );
        }

        [Usage( "SetRace" )]
        [Description( "Apre un gump per la scelta della razza" )]
        public static void SetRace_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null )
                return;

            if( e.Length == 0 )
                from.SendGump( new RaceGump( from ) );
            else
                from.SendMessage( "Uso del comando: [SetRace (senza parametri)" );
        }

        public static void HandleRaceChange( Mobile m, Race oldRace, Race newRace )
        {
            MidgardRace midRace = newRace as MidgardRace;

            if( midRace != null && midRace.SupportCustomBody )
                midRace.UnDressCustomBody( m );

            if( !newRace.ValidateHair( m, m.HairItemID ) )
                m.HairItemID = newRace.RandomHair( m.Female );

            m.HairHue = newRace.ClipHairHue( m.HairHue );

            if( !newRace.ValidateFacialHair( m, m.FacialHairItemID ) )
                m.FacialHairItemID = newRace.RandomFacialHair( m.Female );

            m.FacialHairHue = newRace.RandomHairHue();

            // Validazione del colore della pelle
            m.Hue = 0;
            m.Hue = newRace.RandomSkinHue();

            if( midRace != null )
            {
                if( midRace.SameHairSameSkinHues )
                    m.FacialHairHue = m.Hue;

                if( midRace.SameHairSameFacialHues )
                    m.FacialHairHue = m.HairHue;

                if( midRace.UnhuedFacialhair )
                    m.FacialHairHue = 0;

                if( midRace.SupportCustomBody )
                    midRace.DressCustomBody( m );
            }

            if( m is Midgard2PlayerMobile )
                ( (Midgard2PlayerMobile)m ).AcquireLanguage( newRace );

            // not working properly, Dies
            //if( m is BaseVendor )
            //    ( (BaseVendor)m ).ValidateEquipment();

            /*
                HairItemID = Race.RandomHair( Female );
                HairHue = Race.RandomHairHue();
                FacialHairItemID = Race.RandomFacialHair( Female );
                FacialHairHue = Race.RandomHairHue();
                Hue = Race.RandomSkinHue();
             */
        }
    }

    public class RaceGump : Gump
    {
        private readonly Mobile m_From;

        public RaceGump( Mobile from )
            : base( 50, 50 )
        {
            m_From = from;
            int numRazzeMid = Race.AllRaces.Count - 1;

            m_From.CloseGump( typeof( RaceGump ) );

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage( 0 );
            AddBackground( 10, 20, 250, 58 + 20 + ( numRazzeMid + 2 ) * 20, 9260 );
            AddOldHtml( 30, 30, 100, 20, "Midgard Race Gump" );

            // Bottone Close
            AddButton( 210, 58 + ( numRazzeMid + 2 ) * 20, 4023, 4024, 0, GumpButtonType.Reply, 0 );
            AddLabel( 30, 62 + ( numRazzeMid + 2 ) * 20, 0, "Close" );

            // Razza Human
            AddButton( 210, 58, 4023, 4024, 1, GumpButtonType.Reply, 0 );
            AddLabel( 30, 62, 0, Race.Human.ToString() );

            for( int i = 1; i < numRazzeMid; i++ )
            {
                AddButton( 210, 58 + i * 20, 4023, 4024, i + 1, GumpButtonType.Reply, 0 );
                AddLabel( 30, 62 + i * 20, 0, Race.AllRaces[ i + 1 ].ToString() );
            }
        }

        public override void OnResponse( NetState state, RelayInfo info )
        {
            Mobile from = state.Mobile;
            if( info.ButtonID == 0 )
            {
                from.CloseGump( typeof( RaceGump ) );
                return;
            }

            int idRazza = info.ButtonID;
            from.Target = new InternalTarget( idRazza );
        }

        private class InternalTarget : Target
        {
            private readonly int m_IdRazza;

            public InternalTarget( int indRazza )
                : base( 10, false, TargetFlags.None )
            {
                m_IdRazza = indRazza;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                Race newRace;

                if( targeted is Mobile )
                {
                    Mobile target = targeted as Mobile;

                    Race oldRace = target.Race;
                    newRace = m_IdRazza == 1 ? Race.Human : Race.AllRaces[ m_IdRazza ];

                    if( newRace != null )
                    {
                        if( !CheckValidRaceTarget( from, target, newRace ) )
                            return;

                        target.Race = newRace;
                        from.SendMessage( "Hai settato la razza \"{0}\" al pg {1}.", newRace.ToString(), target.Name );

                        SetRace.HandleRaceChange( target, oldRace, newRace );
                    }
                }
                else if( targeted is RaceTeleporter )
                {
                    newRace = m_IdRazza == 1 ? Race.Human : Race.AllRaces[ m_IdRazza ];
                    if( newRace != null )
                        ( (RaceTeleporter)targeted ).Race = newRace;
                }
            }

            #region checks
            private static bool CheckValidRaceTarget( Mobile source, Mobile target, Race race )
            {
                if( target == null || target.Deleted )
                    return false;

                if( !( target is Midgard2PlayerMobile ) )
                    return true;

                /*
                if( HasAlreadyMaxRaced( target ) )
                {
                    source.SendMessage( "That player has reached the maximum raciable characters for his account. Only Administrators can exceed that limit." );
                    return false;
                }

                if( IsEvilAlignedRace( race ) && IsGoodTownAlignedAccount( target ) )
                {
                    source.SendMessage( "That player cannot be raced because {0} is evil aligned and his account is good aligned. Only Administrators can race that player.", race.Name );
                    return false;
                }

                if( IsGoodAlignedRace( race ) && IsEvilTownAlignedAccount( target ) )
                {
                    source.SendMessage( "That player cannot be raced because {0} is good aligned and his account is evil aligned. Only Administrators can race that player.", race.Name );
                    return false;
                }
                */

                /*
                if( IsEvilAlignedRace( race ) && HasAlreadyGoodAlignedClassedInAccount( target ) )
                {
                    source.SendMessage( "That player cannot be raced because {0} is good aligned and his account has already an evil classed pg. Only Administrators can race that player.", race.Name );
                    return false;						
                }

                if( IsGoodAlignedRace( race ) && HasAlreadyEvilAlignedClassedInAccount( target ) )
                {
                    source.SendMessage( "That player cannot be raced because {0} is evil aligned and his account has already a good classed pg. Only Administrators can race that player.", race.Name );
                    return false;						
                }
                */

                return true;
            }

            /*
                        private static bool IsEvilAlignedRace( Race race )
                        {
                            return race is MidgardRace && ( (MidgardRace)race ).IsEvilAlignedRace;
                        }
            */

            /*
                        private static bool IsGoodAlignedRace( Race race )
                        {
                            return race is MidgardRace && !( (MidgardRace)race ).IsEvilAlignedRace;
                        }
            */

            /*
                        private static bool IsEvilTown( MidgardTowns town )
                        {
                            TownSystem t = TownSystem.Find( town );

                            return t != null && t.IsEvilAlignedTown;
                        }
            */

            /*
                        private static bool IsGoodTown( MidgardTowns town )
                        {
                            TownSystem t = TownSystem.Find( town );

                            return t != null && t.IsGoodAlignedTown;
                        }
            */

            /*
                        private static bool IsEvilTownAlignedAccount( Mobile m )
                        {
                            if( m == null || m.Deleted )
                                return false;

                            Account acct = m.Account as Account;

                            if( acct == null )
                                return false;

                            bool isEvilAccount = false;
                            for( int i = 0; i < acct.Length && !isEvilAccount; i++ )
                            {
                                Midgard2PlayerMobile m2Pm = acct[ i ] as Midgard2PlayerMobile;
                                if( m2Pm == null || m2Pm.Deleted )
                                    continue;

                                if( IsEvilTown( m2Pm.Town ) )
                                    isEvilAccount = true;
                            }

                            return isEvilAccount;
                        }
            */

            /*
                        private static bool IsGoodTownAlignedAccount( Mobile m )
                        {
                            if( m == null || m.Deleted )
                                return false;

                            Account acct = m.Account as Account;

                            if( acct == null )
                                return false;

                            bool isGoodAccount = false;
                            for( int i = 0; i < acct.Length && !isGoodAccount; i++ )
                            {
                                Midgard2PlayerMobile m2Pm = acct[ i ] as Midgard2PlayerMobile;
                                if( m2Pm != null && !m2Pm.Deleted )
                                {
                                    if( IsGoodTown( m2Pm.Town ) )
                                        isGoodAccount = true;
                                }
                            }

                            return isGoodAccount;
                        }
            */

            /*
            private static bool IsGoodAlignedClass( Mobile m )
            {
                Midgard2PlayerMobile m2Pm = m as Midgard2PlayerMobile;
                if( m2Pm != null && !m2Pm.Deleted )
                    return m2Pm.Class == MidgardClasses.Druid || m2Pm.Class == MidgardClasses.Paladin;

                return false;
            }
            */

            /*
            private static bool IsEvilAlignedClass( Mobile m )
            {
                Midgard2PlayerMobile m2Pm = m as Midgard2PlayerMobile;
                if( m2Pm != null && !m2Pm.Deleted )
                    return m2Pm.Class == MidgardClasses.Necromancer;

                return false;
            }
            */

            /*
                        private const int MaxRacedPgsInAccount = 2;
            */

            /*
            private static bool HasAlreadyMaxRaced( Mobile from )
            {
                if( from == null || from.Deleted )
                    return false;

                Account acct = from.Account as Account;

                if( acct == null )
                    return true; // null accounts cannot have other raced pgs

                int charsRaced = 0;
                for( int i = 0; i < acct.Length; i++ )
                {
                    Mobile m = acct[ i ];
                    if( m != null && !m.Deleted && m.Race != Race.Human )
                        charsRaced++;
                }

                return charsRaced >= MaxRacedPgsInAccount;
            }
            */

            /*
                private const int MaxClassedPgsInAccount = 2;
            */

            /*
                private static bool HasAlreadyMaxClassed( Mobile from )
                {
                    if( from == null || from.Deleted )
                        return false;

                    Account acct = from.Account as Account;

                    if( acct == null )
                        return true; // null accounts cannot have other raced pgs

                    int charsClassed = 0;
                    for( int i = 0; i < acct.Length; i++ )
                    {
                        Midgard2PlayerMobile m2pm = acct[ i ] as Midgard2PlayerMobile;
                        if( m2pm != null && !m2pm.Deleted && m2pm.Class != MidgardClasses.None )
                            charsClassed++;
                    }

                    return charsClassed >= MaxClassedPgsInAccount;
                }
            */

            /*
                private static bool HasAlreadyGoodAlignedClassedInAccount( Mobile from )
                {
                    if( from == null || from.Deleted )
                        return false;

                    Account acct = from.Account as Account;

                    if( acct == null )
                        return true; // null accounts cannot have other raced pgs

                    bool hasClassed = false;
                    for( int i = 0; i < acct.Length && !hasClassed; i++ )
                    {
                        Mobile m = acct[ i ] as Mobile;
                        if( m != null && !m.Deleted && IsGoodAlignedClass( m ) )
                            hasClassed = true;
                    }

                    return hasClassed;
                }
            */

            /*
                private static bool HasAlreadyEvilAlignedClassedInAccount( Mobile from )
                {
                    if( from == null || from.Deleted )
                        return false;

                    Account acct = from.Account as Account;

                    if( acct == null )
                        return true; // null accounts cannot have other raced pgs

                    bool hasClassed = false;
                    for( int i = 0; i < acct.Length && !hasClassed; i++ )
                    {
                        Mobile m = acct[ i ];
                        if( m != null && !m.Deleted && IsEvilAlignedClass( m ) )
                            hasClassed = true;
                    }

                    return hasClassed;
                }
            */
            #endregion
        }
    }
}