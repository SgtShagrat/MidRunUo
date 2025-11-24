/***************************************************************************
 *                                  CharactersManager.cs
 *                            		-------------------
 *  begin                	: December, 2007
 *  revision                : April, 2012
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using Server;
using Server.Accounting;
using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Midgard.Commands
{
    public class CharactersManager
    {
        #region registrazione
        public static void Initialize()
        {
            CommandSystem.Register( "CharactersManager", AccessLevel.Administrator, new CommandEventHandler( CharactersManager_OnCommand ) );
        }
        #endregion

        #region callback
        [Usage( "CharactersManager" )]
        [Description( "Brings up a gump which allows deletion of characters and swapping of characters between accounts." )]
        public static void CharactersManager_OnCommand( CommandEventArgs e )
        {
            if( e.Length == 0 )
            {
                e.Mobile.SendGump( new CharactersManagerGump() );
            }
            else
            {
                e.Mobile.SendMessage( "Command use: [CharactersManager" );
            }
        }
        #endregion

        private class CharactersManagerGump : Gump
        {
            private Account m_FirstAccount;
            private Account m_SecondAccount;
            private SwapInfo m_Info;

            public CharactersManagerGump()
                : this( null, null, null, null )
            {
            }

            private CharactersManagerGump( Account firstAccount, Account secondAccount, string errorMessage )
                : this( firstAccount, secondAccount, errorMessage, null )
            {
            }

            private CharactersManagerGump( Account firstAccount, Account secondAccount, string errorMessage, SwapInfo info )
                : base( 50, 50 )
            {
                Closable = true;
                Disposable = false;
                Dragable = true;
                Resizable = false;

                m_FirstAccount = firstAccount;
                m_SecondAccount = secondAccount;
                m_Info = info;

                AddPage( 0 );

                #region Gump Prettification
                AddBackground( 16, 12, 350, 450, 9270 );

                AddImage( 190, 22, 9273 );
                AddImage( 128, 385, 9271 );
                AddImage( 180, 22, 9275 );
                AddImage( 190, 100, 9273 );
                AddImage( 180, 100, 9275 );
                AddImage( 233, 385, 9271 );
                AddImage( 26, 385, 9271 );

                AddAlphaRegion( 15, 10, 352, 454 );
                #endregion

                if( !InSwapMode )
                {
                    AddButton( 176, 49, 4023, 4025, 1, GumpButtonType.Reply, 0 ); //Okay for acct names button

                    AddHtml( 30, 395, 325, 56, Color( Center( errorMessage ), 0xFF0000 ), false, false );

                    AddImageTiled( 33, 50, 140, 20, 0xBBC );
                    AddImageTiled( 209, 50, 140, 20, 0xBBC );

                    AddTextEntry( 33, 50, 140, 20, 1152, 2, "" );
                    AddTextEntry( 209, 50, 140, 20, 1152, 3, "" );
                }

                AddLabel( 58, 28, 1152, ( m_FirstAccount == null ) ? "1st Acct Name" : m_FirstAccount.Username );
                AddLabel( 232, 28, 1152, ( m_SecondAccount == null ) ? "2nd Acct Name" : m_SecondAccount.Username );

                #region Create Character Buttons
                int x = 50; //x is 225 for 2nd...

                for( int h = 0; h < 2; h++ )
                {
                    if( m_FirstAccount != null )
                    {
                        int y = 87;
                        for( int i = 0; i < 6; i++ )	//6 because of 6th char slot and we can handle nulls & out of bounds fine
                        {
                            Mobile m = m_FirstAccount[ i ];

                            if( m == null )
                                continue;

                            if( !( InSwapMode && m_Info.AlreadyChose( m_FirstAccount ) ) )
                                AddButton( x-20, y+3, 5601, 5605, 10*i + h*100 + 5, GumpButtonType.Reply, 0 );	//The Swap Select button

                            AddLabel( x, y, 1152, String.Format( "{0} (0x{1:X})", m.Name, m.Serial.Value ) );

                            int labelY = y+23;
                            int buttonY = y+27;

                            AddLabel( x+1, labelY, 1152, "Swap" );
                            if( m_SecondAccount != null && !InSwapMode  && HasSpace( m_SecondAccount ) != 0 )
                                AddButton( x-15, buttonY, 11400, 11402, 10*i + h*100 + 6, GumpButtonType.Reply, 0 );
                            else
                                AddImage( x-15, buttonY, 11412 );

                            AddLabel( x+54, labelY, 1152, "Del" );
                            if( !InSwapMode )
                                AddButton( x+36, buttonY, 11400, 11402, 10*i + h*100 + 7, GumpButtonType.Reply, 0 );
                            else
                                AddImage( x+36, buttonY, 11412 );

                            AddLabel( x+95, labelY, 1152, "Move" );
                            if( !InSwapMode && m_SecondAccount != null && HasSpace( m_SecondAccount ) >=0 )
                                AddButton( x+78, buttonY, 11400, 11402, 10*i + h*100 + 8, GumpButtonType.Reply, 0 );
                            else
                                AddImage( x+78, buttonY, 11412 );

                            y += 48;
                        }
                    }

                    x += 175;

                    Account temp = m_FirstAccount;
                    m_FirstAccount = m_SecondAccount;
                    m_SecondAccount = temp;
                }
                #endregion
            }

            private string Color( string text, int color )
            {
                return String.Format( "<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text );
            }

            private string Center( string text )
            {
                return String.Format( "<CENTER>{0}</CENTER>", text );
            }

            private bool InSwapMode
            {
                get { return m_Info != null; }
            }

            private bool IsDeleted( Account a )
            {
                return ( a == null || Accounts.GetAccount( a.Username ) == null );
            }

            private int HasSpace( Account a )
            {
                if( a == null || a.Count > 5 )
                    return -1;
                else
                    return a.Count;
            }

            public override void OnResponse( NetState state, RelayInfo info )
            {
                bool sendGumpAgain = true;

                Mobile m = state.Mobile;

                if( m.AccessLevel < AccessLevel.Administrator )
                    return;

                #region Sanity
                if( IsDeleted( m_FirstAccount ) )
                    m_FirstAccount = null;

                if( IsDeleted( m_SecondAccount ) )
                    m_SecondAccount = null;
                #endregion

                int id = info.ButtonID;

                if( id == 0 )
                {
                    if( InSwapMode )
                        m.SendGump( new CharactersManagerGump( m_FirstAccount, m_SecondAccount, "Character swap canceled" ) );
                }
                else if( id == 1 )
                {
                    #region Find Acct from Input
                    string firstStr = info.GetTextEntry( 2 ).Text;
                    string secondStr = info.GetTextEntry( 3 ).Text;

                    Account first = Accounts.GetAccount( firstStr ) as Account;
                    Account second = Accounts.GetAccount( secondStr ) as Account;

                    string errorMessage = "";

                    if( first == null || second == null )
                    {
                        if( first == null  && firstStr != "" && secondStr == "" )
                            errorMessage = String.Format( "Account: '{0}' NOT found", firstStr );
                        else if( second == null  && secondStr != "" && firstStr == "" )
                            errorMessage = String.Format( "Account: '{0}' NOT found", secondStr );
                        else if( firstStr == "" && secondStr == "" )
                            errorMessage = "Please enter in an Account name";
                        else if( second == null && first == null )
                            errorMessage = String.Format( "Accounts: '{0}' and '{1}' NOT found", firstStr, secondStr );
                    }

                    if( m_FirstAccount != null && first == null )
                        first = m_FirstAccount;

                    if( m_SecondAccount != null && second == null )
                        second = m_SecondAccount;

                    m.SendGump( new CharactersManagerGump( first, second, errorMessage ) );
                    #endregion
                }
                else if( id > 4 ) //left side
                {
                    #region Sanity & Declarations
                    int button = id % 10;
                    int charIndex =( ( id < 100 )? id : ( id-100 ) ) / 10;

                    string error = "Invalid Button";

                    Account acct = ( id >= 100 ) ? m_SecondAccount : m_FirstAccount;
                    Account secondAcct = ( id < 100 ) ? m_SecondAccount : m_FirstAccount;

                    if( IsDeleted( acct ) )
                        error = "Selected Account is null or Deleted";
                    else if( acct != null && acct[ charIndex ] == null )
                        error = "That character is not found";
                    #endregion
                    else
                    {
                        if( acct != null )
                        {
                            Mobile mob = acct[ charIndex ];
                            switch( button )
                            {
                                #region Swap
                                case 5: //Swap Selection And/Or Props
                                    {
                                        if( InSwapMode )
                                        {
                                            if( !m_Info.AlreadyChose( acct ) && !m_Info.AlreadyChose( secondAcct ) )
                                            {
                                                //Both Empty, even though this should NEVER happen.  Just a sanity check
                                                m_Info.FirstAccount = acct;
                                                m_Info.FirstCharIndex = charIndex;
                                                error = "Please choose a character from the Other acct to swap with";
                                            }
                                            else if( ( m_Info.AlreadyChose( m_Info.FirstAccount ) && !m_Info.AlreadyChose( m_Info.SecondAccount ) ) || 
					                                 ( m_Info.AlreadyChose( m_Info.SecondAccount ) && !m_Info.AlreadyChose( m_Info.FirstAccount ) ) )
                                            {
                                                //First is filled, second is empty
                                                if( m_Info.AlreadyChose( m_Info.FirstAccount ) )
                                                {
                                                    m_Info.SecondAccount = acct;
                                                    m_Info.SecondCharIndex = charIndex;
                                                }
                                                else
                                                {
                                                    m_Info.FirstAccount = acct;
                                                    m_Info.FirstCharIndex = charIndex;
                                                }

                                                if( m_Info.DoSwap() )
                                                {
                                                    error = String.Format( "Mobile {0} (0x{1:X}) and Mobile {2} (0x{3:X}) sucessfully swapped between Accounts {4} and {5}",
                                                                           m_Info.FirstAccount[ m_Info.FirstCharIndex ], m_Info.FirstAccount[ m_Info.FirstCharIndex ].Serial.Value,
                                                                           m_Info.SecondAccount[ m_Info.SecondCharIndex ], m_Info.SecondAccount[ m_Info.SecondCharIndex ].Serial.Value,
                                                                           m_Info.SecondAccount, m_Info.FirstAccount );
                                                    CommandLogging.WriteLine( m, error );
                                                }
                                                else
                                                    error = "Swap unsucessful";

                                                m_Info = null;
                                            }
                                        }
                                        else
                                        {
                                            m.SendGump( new PropertiesGump( m, mob ) );
                                            error = "Properties gump sent";
                                        }

                                        break;
                                    }
                                case 6: //Swap
                                    {
                                        if( IsDeleted( secondAcct ) )
                                        {
                                            error = "Both accounts must exist to swap characters";
                                        }
                                        else if( HasSpace( acct ) == 0 || HasSpace( secondAcct ) == 0 )
                                        {
                                            error = "Both accounts must have at least one character to swap.";
                                        }
                                        else
                                        {
                                            error = "Please Choose the other character to swap.";
                                            m_Info = new SwapInfo( m_FirstAccount, m_SecondAccount );

                                            if( acct == m_FirstAccount )
                                                m_Info.FirstCharIndex = charIndex;
                                            else
                                                m_Info.SecondCharIndex = charIndex;
                                        }
                                        break;
                                    }
                                #endregion
                                #region Delete Character
                                case 7: //Del
                                    {
                                        object[] o = new object[] { acct, mob, this };

                                        m.SendGump(
                                            new WarningGump( 1060635, 30720,
                                                             String.Format( "You are about to delete Mobile {0} (0x{1:X}) of Acct {2}. " +
					                                                        "This can not be reversed without a complete server revert. " +
					                                                        "Please note that this'll delete any items on that Character, " +
					                                                        "but it'll still leave their house standing. " +
					                                                        "Do you wish to continue?", mob.Name, mob.Serial.Value, acct ),
                                                             0xFFC000, 360, 260, new WarningGumpCallback( CharacterDelete_Callback ), o ) );

                                        sendGumpAgain = false;

                                        break;
                                    }
                                #endregion
                                #region Move Character
                                case 8: //Move
                                    {
                                        if( secondAcct == null )
                                        {
                                            error = String.Format( "Can't move Mobile {0} (0x{1:X} because the other account is null", mob.Name, mob.Serial.Value );
                                            break;
                                        }

                                        int newCharLocation = HasSpace( secondAcct );

                                        if( newCharLocation < 0 )
                                        {
                                            error = String.Format( "Can't move Mobile {0} (0x{1:X}) to account {2} because that account is full", mob.Name, mob.Serial.Value, secondAcct );
                                            break;
                                        }

                                        acct[ charIndex ] = null;
                                        secondAcct[ newCharLocation ] = mob;

                                        mob.Say( "I've been moved to another Account!" );

                                        if( mob.NetState != null )
                                            mob.NetState.Dispose();

                                        error = String.Format( "Mobile {0} (0x{1:X}) of Account {2} moved to Account {3}.", mob.Name, mob.Serial.Value, acct, secondAcct );

                                        CommandLogging.WriteLine( m, error );
                                        break;
                                    }
                                #endregion
                            }
                        }
                    }
                    if( sendGumpAgain )
                        m.SendGump( new CharactersManagerGump( m_FirstAccount, m_SecondAccount, error, m_Info ) );
                }
            }

            private void CharacterDelete_Callback( Mobile from, bool okay, object state )
            {
                object[] states = (object[])state;

                Account acct = (Account)states[ 0 ];
                Mobile mob = (Mobile)states[ 1 ];
                CharactersManagerGump g = (CharactersManagerGump)states[ 2 ];

                if( mob == null || acct == null )
                {
                    return;
                }
                if( okay )
                {
                    mob.Say( "I've been Deleted!" );

                    if( mob.NetState != null )
                        mob.NetState.Dispose();

                    mob.Delete();
                }

                string error = String.Format( "Mobile {0} (0x{1:X}) of Acct {2} {3} Deleted.", mob.Name, mob.Serial.Value, acct, okay ? "" : "not" );

                if( okay )
                    CommandLogging.WriteLine( from, error );

                from.SendGump( new CharactersManagerGump( g.m_FirstAccount, g.m_SecondAccount, error ) );
            }
        }

        private class SwapInfo
        {
            private Account m_FirstAccount;
            private Account m_SecondAccount;
            private int m_FirstCharIndex;
            private int m_SecondCharIndex;

            public Account SecondAccount
            {
                get { return m_SecondAccount; }
                set { m_SecondAccount = value; }
            }

            public Account FirstAccount
            {
                get { return m_FirstAccount; }
                set { m_FirstAccount = value; }
            }

            public int FirstCharIndex
            {
                get { return m_FirstCharIndex; }
                set { m_FirstCharIndex = value; }
            }

            public int SecondCharIndex
            {
                get { return m_SecondCharIndex; }
                set { m_SecondCharIndex = value; }
            }

            public SwapInfo( Account firstAccount, Account secondAccount )
            {
                m_FirstAccount = firstAccount;
                m_SecondAccount = secondAccount;

                m_FirstCharIndex = -1;
                m_SecondCharIndex = -1;
            }

            public bool AlreadyChose( Account acct )
            {
                if( acct == null )
                    return false;

                return ( acct == m_FirstAccount && m_FirstCharIndex >= 0 ) || ( acct == m_SecondAccount && m_SecondCharIndex >= 0 );
            }

            private static bool AssignMobileToAccount( Account account, Mobile mobile, int index )
            {
                if( account == null || mobile == null || mobile.Deleted )
                    return false;

                account[ index ] = mobile;
                mobile.Say( "I've been moved to another Account!" );
                if( mobile.NetState != null )
                    mobile.NetState.Dispose();

                return true;
            }

            public bool DoSwap()
            {
                Mobile swapper = m_FirstAccount[ m_FirstCharIndex ];
                if( swapper == null )
                    return false;

                bool doneFirMov = AssignMobileToAccount( m_FirstAccount, m_SecondAccount[ m_SecondCharIndex ], FirstCharIndex );
                bool doneSecMov = AssignMobileToAccount( m_SecondAccount, swapper, m_SecondCharIndex );

                return doneFirMov && doneSecMov;
            }
        }
    }
}
