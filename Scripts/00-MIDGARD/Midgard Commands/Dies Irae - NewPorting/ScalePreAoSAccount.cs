/***************************************************************************
 *                               ScalePreAoSAccount.cs
 *                            ---------------------------
 *   begin                : 30 novembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Server;
using Server.Accounting;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.ThirdCrownPorting
{
    public class PreAoSPorting
    {
        private static string LogPath = "Logs/NewPorting.log";

        private static int m_MaxPgPerAccount = 3;

        public static void Initialize()
        {
            if( !Core.AOS )
                EventSink.Login += new LoginEventHandler( OnLogin );
        }

        private static void OnLogin( LoginEventArgs e )
        {
            Mobile mobile = e.Mobile;

            if( mobile != null )
            {
                Account a = mobile.Account as Account;
                if( a == null )
                    return;

                if( a.GetTag( "PortedAccount" ) == null || a.GetTag( "PortedAccount" ) == "2" )
                    return;

                if( a.AccessLevel > AccessLevel.Player )
                    return;

                if( !mobile.CantWalk )
                    mobile.CantWalk = true;

                if( a.GetTag( "PortedAccount" ) == "0" )
                {
                    if( a.Count <= m_MaxPgPerAccount )
                        a.SetTag( "PortedAccount", "1" );
                    else
                    {
                        mobile.SendGump( new DeletePgsGump( mobile ) );
                        return;
                    }
                }

                //if( endPorting && a.GetTag( "PortedAccount" ) == "1" )
                //{
                //    mobile.SendGump( new ScalePreAoSAccountGump( mobile ) );
                //    endPorting = false;
                //}

                if( a.GetTag( "PortedAccount" ) == "1" )
                    ScaleAccountSkills( a );

                mobile.SendMessage( "Verrai disconnesso tra 5 secondi." );
                // Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), OnForceDisconnect_Callback, mobile );

                if( mobile.CantWalk )
                    mobile.CantWalk = false;
            }
        }

        private class DeletePgsGump : Gump
        {
            #region constants
            private const int m_HueTit = 662;
            private const int HuePrim = 92;
            private const int DisabledHue = 999; // grigio
            private const int m_DeltaBut = 2;
            private const int m_FieldsDist = 25;
            #endregion

            #region fields
            private readonly Mobile m_Owner;
            private readonly Account m_Account;
            private readonly List<Mobile> m_ToSave;
            #endregion

            #region constructors
            public DeletePgsGump( Mobile owner )
                : this( owner, null )
            {
            }

            private DeletePgsGump( Mobile owner, List<Mobile> toSave )
                : base( 100, 100 )
            {
                Closable = false;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                m_Owner = owner;
                if( m_Owner == null )
                    return;

                m_Owner.CantWalk = true;

                m_Account = m_Owner.Account as Account;
                if( m_Account == null )
                    return;

                m_ToSave = toSave ?? new List<Mobile>();

                Design();
            }
            #endregion

            #region members
            private void Design()
            {
                AddPage( 0 );

                AddBackground( 0, 0, 275, 325, 9200 );

                AddImageTiled( 10, 10, 255, 25, 2624 );
                AddImageTiled( 10, 45, 255, 240, 2624 );
                AddImageTiled( 40, 295, 225, 20, 2624 );

                AddButton( 10, 295, 4017, 4018, 0, GumpButtonType.Reply, 0 );
                AddHtmlLocalized( 45, 295, 75, 20, 1011012, 32767, false, false ); // CANCEL

                AddAlphaRegion( 10, 10, 255, 285 );
                AddAlphaRegion( 40, 295, 225, 20 );

                AddLabelCropped( 14, 12, 255, 25, m_HueTit, "Scegli i personaggi da salvare:" );

                for( int i = 0; i < m_Account.Length; i++ )
                {
                    Mobile m = m_Account[ i ];
                    if( m == null )
                        continue;

                    int hue = m_ToSave.Contains( m ) ? DisabledHue : HuePrim;

                    AddLabelCropped( 35, 52 + i * m_FieldsDist, 225, 20, hue, m.Name );

                    if( !m_ToSave.Contains( m ) )
                        AddButton( 15, 52 + m_DeltaBut + ( i * m_FieldsDist ), 1209, 1210, i + 1, GumpButtonType.Reply, 0 );
                }
            }

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                Mobile from = sender.Mobile;

                Log( from, string.Format( "" ) );

                if( info.ButtonID <= 0 || info.ButtonID > m_Account.Length )
                {
                    from.SendMessage( "Verrai disconnesso tra 3 secondi." );
                    Timer.DelayCall( TimeSpan.FromSeconds( 3.0 ), new TimerStateCallback( OnForceDisconnect ), from );
                }
                else
                {
                    Mobile toDelete = m_Account[ info.ButtonID - 1 ];
                    m_ToSave.Add( toDelete );

                    if( m_ToSave.Count < m_MaxPgPerAccount && m_ToSave.Count < m_Account.Count )
                        from.SendGump( new DeletePgsGump( from, m_ToSave ) );
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append( "Hai scelto di salvare i seguenti personaggi:<br>" );
                        foreach( Mobile t in m_ToSave )
                            sb.AppendFormat( "   <br><em><basefont color=red>{0}</basefont></em><br>", t.Name );
                        sb.Append( "Tutti gli altri personaggi dell'account verranno cancellati <em><basefont color=red>permanente</basefont></em>.<br>" );
                        sb.Append( "Sei sicuro di voler procedere?<br>" );

                        from.SendGump( new WarningGump( 1060635, 30720, sb.ToString(), 0xFFC000, 420, 280,
                                    new WarningGumpCallback( ConfirmCallBack ),
                                    new object[] { m_ToSave, m_Account } ) );
                    }
                }
            }

            private static void ConfirmCallBack( Mobile from, bool okay, object state )
            {
                object[] states = (object[])state;

                List<Mobile> toSave = (List<Mobile>)states[ 0 ];
                Account account = (Account)states[ 1 ];

                string netStateName = from.NetState.ToString();
                NetState netstate = from.NetState;

                if( okay )
                {
                    account.SetTag( "PortedAccount", "1" );

                    for( int i = 0; i < account.Length; i++ )
                    {
                        Mobile toDelete = account[ i ];

                        if( toDelete == null )
                            continue;

                        if( toSave.Contains( toDelete ) )
                            continue;

                        if( from == toDelete && from.NetState != null )
                            from.NetState.Dispose();

                        int index = GetIndex( toDelete, account );

                        if( index >= 0 && index < account.Length )
                        {
                            Console.WriteLine( "Client: {0}: Deleting character {1} (0x{2:X})", netStateName, toDelete.Name, toDelete.Serial.Value );

                            account.Comments.Add( new AccountComment( "System", String.Format( "Character {0} deleted by {1}", toDelete.Name, netStateName ) ) );

                            toDelete.Delete();
                            account[ index ] = null;
                            netstate.Send( new CharacterListUpdate( account ) );
                        }
                    }
                }

                Timer.DelayCall( TimeSpan.FromSeconds( 3.0 ), new TimerStateCallback( OnForceDisconnect ), from );
            }

            private static int GetIndex( Mobile m, IAccount a )
            {
                for( int i = 0; i < a.Length; i++ )
                {
                    if( a[ i ] == m )
                        return i;
                }

                return -1;
            }
            #endregion
        }

#if false
        private class ScalePreAoSAccountGump : Gump
        {
        #region constants
            private const int m_HueTit = 662;
            private const int HuePrim = 92;
            private const int HueSec = 87;
            private const int m_DeltaBut = 2;
            private const int m_FieldsDist = 25;

            private const double m_PercScaleNotMajor = 30.0;
            private const double m_LowerBoundNotMajor = 30.0;
            private const double m_UpperBoundNotMajor = 100.0;

            private const double m_PercScaleMajor = 90.0;
            private const double m_LowerBoundMajor = 30.0;
            private const double m_UpperBoundMajor = 100.0;
        #endregion

        #region fields
            private Mobile m_Owner;
            private Account m_Account;
        #endregion

        #region constructors
            public ScalePreAoSAccountGump( Mobile owner )
                : base( 100, 100 )
            {
                Closable = false;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                m_Owner = owner;
                if( m_Owner == null )
                    return;

                m_Owner.CantWalk = true;

                m_Account = m_Owner.Account as Account;
                if( m_Account == null )
                    return;

                Design();
            }
        #endregion

        #region members
            private void Design()
            {
                AddPage( 0 );

                AddBackground( 0, 0, 275, 325, 9200 );

                AddImageTiled( 10, 10, 255, 25, 2624 );
                AddImageTiled( 10, 45, 255, 240, 2624 );
                AddImageTiled( 40, 295, 225, 20, 2624 );

                AddButton( 10, 295, 4017, 4018, 0, GumpButtonType.Reply, 0 );
                AddHtmlLocalized( 45, 295, 75, 20, 1011012, 32767, false, false ); // CANCEL

                AddAlphaRegion( 10, 10, 255, 285 );
                AddAlphaRegion( 40, 295, 225, 20 );

                AddLabelCropped( 14, 12, 255, 25, m_HueTit, "Scegli il giocatore da trasferire:" );

                int hue = HuePrim;

                for( int i = 0; i < m_Account.Length; i++ )
                {
                    Mobile m = m_Account[ i ];
                    if( m == null )
                        continue;

                    hue = GetHueFor( hue );
                    AddLabelCropped( 35, 52 + i * m_FieldsDist, 225, 20, hue, m_Account[ i ].Name );
                    AddButton( 15, 52 + m_DeltaBut + ( i * m_FieldsDist ), 1209, 1210, i + 1, GumpButtonType.Reply, 0 );
                }
            }

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                Mobile from = sender.Mobile;

                Log( from, string.Format( "ScalePreAoSAccountGump response: {0}", info.ButtonID ) );

                if( info.ButtonID <= 0 || info.ButtonID > m_Account.Length )
                {
                    from.SendMessage( "Verrai disconnesso tra 30 secondi." );
                    Timer.DelayCall( TimeSpan.FromSeconds( 30.0 ), OnForceDisconnect_Callback, from );
                }
                else
                {
                    Mobile toSave = m_Account[ info.ButtonID - 1 ];

                    Log( from, string.Format( "Mobile toSave chosen {0}:", toSave.Name ) );

                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat( "Hai scelto il personaggio <em><basefont color=red>{0}</basefont></em>.<br>", from.Name );
                    sb.AppendFormat( "Le skills di tale personaggio verranno scalate al {0:F2} percento.<br>", m_PercScaleMajor );
                    sb.AppendFormat( "Le skill degli altri personaggi dell'account verranno scalate al {0:F2} percento.<br>", m_PercScaleNotMajor );
                    sb.Append( "Tale modifica sara' <em><basefont color=red>permanente</basefont></em>.<br>" );
                    sb.Append( "Sei sicuro di voler procedere?<br>" );

                    from.SendGump( new WarningGump( 1060635, 30720, sb.ToString(), 0xFFC000, 420, 280,
                                new WarningGumpCallback( ConfirmCallBack ),
                                new object[] { toSave, m_Account } ) );
                }
            }

            private static void ConfirmCallBack( Mobile from, bool okay, object state )
            {
                object[] states = (object[])state;

                Mobile toSave = (Mobile)states[ 0 ];
                Account account = (Account)states[ 1 ];

                if( okay )
                {
                    account.SetTag( "PortedAccount", "2" );

                    for( int i = 0; i < account.Length; i++ )
                    {
                        Mobile m = account[ i ];

                        if( m == null )
                            continue;

                        if( m == toSave )
                            Scale( m, m_PercScaleMajor, m_LowerBoundMajor, m_UpperBoundMajor );
                        else
                            Scale( m, m_PercScaleNotMajor, m_LowerBoundNotMajor, m_UpperBoundNotMajor );
                    }
                }

                Timer.DelayCall( TimeSpan.FromSeconds( 3.0 ), OnForceDisconnect_Callback, from );
            }

            private static int GetHueFor( int hue )
            {
                return ( hue == HuePrim ? HueSec : HuePrim );
            }
        #endregion
        }
#endif

        private const double ScaleFactor = 70.0;
        private const double LowerBound = 30.0;
        private const double UpperBound = 100.0;

        private static void ScaleAccountSkills( Account account )
        {
            for( int i = 0; i < account.Length; i++ )
            {
                Mobile m = account[ i ];
                if( m == null )
                    continue;

                Scale( m, ScaleFactor, LowerBound, UpperBound );
                ScaleStat( m, StatType.Str, ScaleFactor, LowerBound, UpperBound );
                ScaleStat( m, StatType.Dex, ScaleFactor, LowerBound, UpperBound );
                ScaleStat( m, StatType.Int, ScaleFactor, LowerBound, UpperBound );
            }

            account.SetTag( "PortedAccount", "2" );
        }

        private static void ScaleStat( Mobile toScale, StatType type, double percScale, double lowerBound, double upperBound )
        {
            double oldValue = 0;
            switch( type )
            {
                case StatType.Str:
                    oldValue = toScale.RawStr;
                    break;
                case StatType.Dex:
                    oldValue = toScale.RawDex;
                    break;
                case StatType.Int:
                    oldValue = toScale.RawInt;
                    break;
            }

            if( oldValue > upperBound )
                oldValue = upperBound;

            double newValue = oldValue * percScale * 0.01;

            if( oldValue > lowerBound && newValue < lowerBound )
                newValue = lowerBound;

            switch( type )
            {
                case StatType.Str:
                    toScale.RawStr = (int)newValue;
                    break;
                case StatType.Dex:
                    toScale.RawDex = (int)newValue;
                    break;
                case StatType.Int:
                    toScale.RawInt = (int)newValue;
                    break;
            }

            Log( string.Format( "{0} - oldValue: {1} - newValue {2}", type, oldValue.ToString( "F2" ), newValue.ToString( "F2" ) ) );
        }

        private static void Scale( Mobile toScale, double percScale, double lowerBound, double upperBound )
        {
            if( toScale == null )
                return;

            Log( "" );
            Log( string.Format( "{0}:", toScale.Name ) );

            foreach( int i in Enum.GetValues( typeof( SkillName ) ) )
            {
                double oldValue = toScale.Skills[ (SkillName)i ].Base;

                if( oldValue > upperBound )
                    oldValue = upperBound;

                double newValue = oldValue * percScale * 0.01;

                if( oldValue > lowerBound && newValue < lowerBound )
                    newValue = lowerBound;

                toScale.Skills[ (SkillName)i ].Base = newValue;
                toScale.Skills[ (SkillName)i ].Cap = 100.0;

                Log( string.Format( "{0} - oldValue: {1} - newValue {2}", (SkillName)i, oldValue.ToString( "F2" ), newValue.ToString( "F2" ) ) );
            }

            DoSpecialScale( toScale );
        }

        private static void DoSpecialScale( Mobile toScale )
        {
            toScale.Skills[ SkillName.Ninjitsu ].Base = 0.0;
            toScale.Skills[ SkillName.Ninjitsu ].Cap = 0.0;

            toScale.Skills[ SkillName.Bushido ].Base = 0.0;
            toScale.Skills[ SkillName.Bushido ].Cap = 0.0;

            toScale.Skills[ SkillName.Focus ].Base = 0.0;
            toScale.Skills[ SkillName.Focus ].Cap = 0.0;

            toScale.Skills[ SkillName.Chivalry ].Base = 0.0;
            toScale.Skills[ SkillName.Necromancy ].Base = 0.0;
            toScale.Skills[ SkillName.Spellweaving ].Base = 0.0;
        }

        private static void OnForceDisconnect( object state )
        {
            if( state is Mobile )
            {
                Mobile m = (Mobile)state;

                if( m.NetState != null && m.NetState.Running )
                    m.NetState.Dispose();

                Log( m, "Disconnected." );

                m.Map = Map.Internal;
            }
        }

        #region log
        private static void Log( string toLog )
        {
            Log( null, toLog, LogPath );
        }

        private static void Log( Mobile logger, string toLog )
        {
            Log( logger, toLog, LogPath );
        }

        private static void Log( Mobile logger, string toLog, string path )
        {
            try
            {
                using( StreamWriter op = new StreamWriter( path, true ) )
                {
                    if( logger != null )
                        op.WriteLine( "{0}: {1}: {2}", DateTime.Now, logger.NetState, toLog );
                    else
                        op.WriteLine( "{0}: {1}", DateTime.Now, toLog );
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }
        #endregion
    }
}