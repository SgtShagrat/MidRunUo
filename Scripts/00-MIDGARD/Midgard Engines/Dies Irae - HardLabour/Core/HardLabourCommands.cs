// #define DebugHardLabourCommands

/***************************************************************************
 *                                	 HardLabourCommand.cs
 *
 *  begin                	: Gennaio, 2007
 *  version					: 0.1
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Server;
using Server.Accounting;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Midgard.Engines.HardLabour
{
    public class HardLabourCommands
    {
        public static Map ColonyMap = Map.Malas;
        public static Point3D ColonyLocation = new Point3D( 2152, 1682, -54 );

        public static void RegisterCommands()
        {
            if( Config.Enabled )
            {
                CommandSystem.Register( "HardLabour", AccessLevel.Counselor, new CommandEventHandler( HardLabour_OnCommand ) );
                CommandSystem.Register( "GenerateHardLabourSystem", AccessLevel.Developer, new CommandEventHandler( GenerateHardLabourSystem_OnCommand ) );
            }
        }

        [Usage( "GenerateHardLabourSystem" )]
        [Description( "Generate hard labour system." )]
        public static void GenerateHardLabourSystem_OnCommand( CommandEventArgs e )
        {
            if( e.Mobile == null )
                return;

            if( e.Length > 0 )
            {
                e.Mobile.SendMessage( "Command Use: [GenerateHardLabourSystem" );
                return;
            }

            Console.WriteLine( "Generating HardLabourSystem..." );

            // Creazione della regione penitenziaria
            if( Region.Find( ColonyLocation, Map.Malas ).GetType() == typeof( HardLabourColonyRegion ) )
            {
                Console.WriteLine( "Unregistering actual HL region." );
                Region.Find( ColonyLocation, Map.Malas ).Unregister();
            }
            Console.Write( "Creation of hard labour region..." );
            Region region = new HardLabourColonyRegion();
            region.Register();
            Console.Write( "Done.\n" );

            // Creazione della persistenza
            new HardLabourPersistance( region );

            // Creazione di Kerberos
            Console.Write( "Creating Kerberos..." );
            List<Mobile> npcs = new List<Mobile>( region.GetMobiles() );
            foreach( Mobile t in npcs )
            {
                if( t.GetType() == typeof( Kerberos ) )
                    t.Delete();
            }
            Kerberos carceriere = new Kerberos();
            carceriere.MoveToWorld( new Point3D( 2138, 1678, -74 ), Map.Malas );
            carceriere.CantWalk = true;
            carceriere.Direction = Direction.East;
            Console.Write( "Done.\n" );

            // Creazione delle porte
            Console.Write( "Creating doors..." );
            BaseDoor doors1 = CreateDoors( 2160, 1700, -54, true, 0x44E );
            doors1.Locked = true;
            doors1.KeyValue = Key.RandomValue();
            if( doors1.Link != null )
            {
                doors1.Link.Locked = true;
                doors1.Link.KeyValue = Key.RandomValue();
            }
            Console.Write( "Done.\n" );

            // Creazione di Blocker all'entrata
            Console.Write( "Creating blockers at entrance..." );
            for( int i = 0; i < 4; i++ )
            {
                Blocker bl = new Blocker();
                bl.MoveToWorld( new Point3D( 2168, 1707 + i, -90 ), Map.Malas );
            }
            Console.Write( "Done.\n" );

            Console.WriteLine( "Creation of hard labour system comleted." );
        }

        [Usage( "HardLabour <number of minerals to mine>" )]
        [Description( "Condemn a player to mine ores." )]
        public static void HardLabour_OnCommand( CommandEventArgs e )
        {
            if( e.Mobile == null )
                return;

            if( e.Length != 1 )
            {
                e.Mobile.SendMessage( "Command Use: [HardLabour <number of minerals to mine>" );
                return;
            }

            int mineralsToMine = e.GetInt32( 0 );

            if( mineralsToMine > 0 )
            {
                e.Mobile.SendMessage( "Choose a player you want do condemn" );
                e.Mobile.Target = new InternalTarget( mineralsToMine, e.Mobile );
            }
            else
            {
                e.Mobile.SendMessage( "Command Use: [HardLabour <number of minerals to mine>" );
            }
        }

        public static Region GenerateHardLabourColonyRegion()
        {
            if( Region.Find( ColonyLocation, Map.Malas ).GetType() == typeof( HardLabourColonyRegion ) )
                Region.Find( ColonyLocation, Map.Malas ).Unregister();

            Region region = new HardLabourColonyRegion();
            region.Register();

            return region;
        }

        private static void EffectCircle( IPoint3D center, Map map, int radius )
        {
            Point3D current = new Point3D( center.X + radius, center.Y, center.Z );

            for( int i = 0; i <= 360; i++ )
            {
                Point3D next = new Point3D( (int)Math.Round( Math.Cos( i ) * radius ) + center.X, (int)Math.Round( Math.Sin( i ) * radius ) + center.Y, current.Z );
                Effects.SendLocationEffect( next, map, 0x3728, 13 );
            }
        }

        public static void GetHardLabourInfo( Mobile from )
        {
            if( !from.CheckAlive() )
                return;

            Midgard2PlayerMobile player = from as Midgard2PlayerMobile;
            if( player == null )
                return;

            StringBuilder s = new StringBuilder();
            s.Append( "You are in Midgard hard labours penitentiary colony.<br>" );

            if( !string.IsNullOrEmpty( player.HardLabourCondamner ) )
                s.AppendFormat( "You have been condemned by {0}.<br>", player.HardLabourCondamner );

            if( !string.IsNullOrEmpty( player.HardLabourInfo ) )
                s.AppendFormat( "You were condamned for: {0}.<br>", player.HardLabourInfo );

            s.AppendFormat( "You have to mine {0} ores to be released.", player.Minerals2Mine );

            player.SendGump( new NoticeGump( 1060635, 30720, s.ToString(),
                           0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeGumpCallBack ), null ) );
        }

        public static bool HasOtherPendingCondemns( Mobile prisoner )
        {
            Account a = prisoner.Account as Account;
            if( a == null )
                return false;

            for( int i = 0; i < a.Count; i++ )
            {
                Midgard2PlayerMobile m2Pm = a[ i ] as Midgard2PlayerMobile;
                if( m2Pm != null && !m2Pm.Deleted && m2Pm != prisoner && m2Pm.Minerals2Mine > 0 )
                {
                    return true;
                }
            }

            return false;
        }

        private static readonly double PendingCondemnsDisconnectDelay = 30.0;

        private static readonly string PendingCondemnsMessage = "Il carceriere Kerberos ha denunciato il fatto che un tuo personaggio giocante ha ancora del lavoro " +
                                                                "da svolgere nella colonia penitenziaria di Midgard.<br>" +
                                                                "Fintanto che tali minerali non saranno consegnati al Carceriere, non potrai loggare con nessun altro personaggio.<br>" +
                                                                "Per la tua sicurezza personale sei stato nascosto alla vista degli altri giocatori e dei mostri ma appena ti muoverai tornerai visibile.<br>" +
                                                                "Verrai disconnesso tra meno di 30 secondi.";

        public static void HandleOtherPendingCondemns( Mobile m )
        {
            m.Hidden = true;
            m.Freeze( TimeSpan.FromSeconds( PendingCondemnsDisconnectDelay ) );
            m.SendGump( new NoticeGump( 1060635, 30720, PendingCondemnsMessage, 0xFFC000, 420, 280, new NoticeGumpCallback( CloseNoticeGumpCallBack ), null ) );

            Timer t = Timer.DelayCall( TimeSpan.FromSeconds( PendingCondemnsDisconnectDelay ), OnForceDisconnect_Callback, m );
            t.Start();
        }

        private static void OnForceDisconnect_Callback( object state )
        {
            if( state is Mobile )
            {
                Mobile m = (Mobile)state;

                if( m.NetState != null && m.NetState.Running )
                    m.NetState.Dispose();
            }
        }

        private static void CloseNoticeGumpCallBack( Mobile from, object state )
        {
        }

        public static void ExecuteCondemn( Mobile prisoner, bool withEffects )
        {
            #region effetti visivi e trasporto alla colonia penitenziaria
            if( withEffects && prisoner.Map != Map.Internal )
                EffectCircle( prisoner.Location, prisoner.Map, 3 );

            // Trasferisce il condannato nella colonia penitenziaria
            prisoner.Map = ColonyMap;
            prisoner.Location = ColonyLocation;

            if( withEffects && prisoner.Map != Map.Internal )
                EffectCircle( prisoner.Location, prisoner.Map, 3 );

            if( withEffects )
                prisoner.PlaySound( 0x228 );
            #endregion

            #region confisca dei beni
            // Confisco i beni del codannato e li pongo in banca in uno zaino
            Backpack prisonerBag = new Backpack();
            prisonerBag.Hue = 666;
            prisonerBag.Name = "A Prisoner Bag";
            prisoner.BankBox.DropItem( prisonerBag );

            List<Item> equipitems = new List<Item>( prisoner.Items );
            foreach( Item t in equipitems )
            {
                if( !t.Movable )
                    continue;
                if( ( t.Layer != Layer.Bank ) && ( t.Layer != Layer.Mount ) && ( t.Layer != Layer.Backpack ) )
                    prisonerBag.DropItem( t );
            }

            #region edit by Arlas: stablatura degli animali
            if( prisoner is Midgard2PlayerMobile )
                ( (Midgard2PlayerMobile)prisoner ).ShrinkAllPets();
            #endregion

            #region stablatura dell'eventuale cavalcatura
            try
            {
                if( prisoner.Mounted )
                {
                    IMount mount = prisoner.Mount;
                    if( mount != null )
                        mount.Rider = null;

                    BaseCreature pet = (BaseCreature)mount;
			pet.Shrink();
                    /*if( pet != null )
                    {
                        pet.ControlTarget = null;
                        pet.ControlOrder = OrderType.Stay;
                        pet.Internalize();

                        pet.SetControlMaster( null );
                        pet.SummonMaster = null;

                        pet.IsStabled = true;

                        pet.Loyalty = BaseCreature.MaxLoyalty;

                        prisoner.Stabled.Add( pet );
                    }*/
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( "Warning: HardLabour System - Pet non stabled correctly: {0}", ex );
            }
            #endregion

            List<Item> packitems = new List<Item>( prisoner.Backpack.Items );
            foreach( Item t in packitems )
            {
                if( t.Movable )
                    prisonerBag.DropItem( t );
            }
            #endregion
        }

        public static void EquipPrisoner( Mobile prisoner )
        {
            // Tunic
            Tunic tunic = new Tunic( 0 );
            tunic.Name = CreatePrisonerNumber( HardLabourPersistance.HardLabourCounter );
            prisoner.AddItem( tunic );

            // Pants
            LongPants pants = new LongPants( 0 );
            pants.Name = "Designed by Dies Irae";
            prisoner.AddItem( pants );

            // Shirt
            Shirt shirt = new Shirt( 0 );
            shirt.Name = "Designed by Dies Irae";
            prisoner.AddItem( shirt );

            // Skullcap
            SkullCap skullcap = new SkullCap( 0 );
            skullcap.Name = "Designed by Dies Irae";
            prisoner.AddItem( skullcap );

            // Shoes
            Shoes shoes = new Shoes( 0 );
            shoes.Name = "Designed by Dies Irae";
            prisoner.AddItem( shoes );

            // Piccone
            prisoner.AddItem( new SlavePickaxe() );

            // Brocca vuota
            prisoner.AddToBackpack( new Pitcher() );

            // Cibo avariato
            prisoner.AddToBackpack( new StaleBreadLoaf( 12 ) );
            prisoner.AddToBackpack( new SpoiledMeat( 8 ) );
        }

        public static void DoHardLabourCondemn( Mobile prisoner, int mineralsToMine, string condamner )
        {
            Midgard2PlayerMobile m2Pm = prisoner as Midgard2PlayerMobile;
            if( m2Pm == null )
                return;

            if( m2Pm.Young )
                m2Pm.Young = false;

            ExecuteCondemn( m2Pm, true );

            HardLabourPersistance.HardLabourCounter++;
            EquipPrisoner( m2Pm );

            m2Pm.SendGump( new HardLabourNotifierGump( mineralsToMine ) );
            m2Pm.Minerals2Mine = ( m2Pm.Minerals2Mine < 0 ) ? 10 : ( m2Pm.Minerals2Mine += mineralsToMine );

            if( !String.IsNullOrEmpty( condamner ) )
                m2Pm.HardLabourCondamner = condamner;

            m2Pm.SendMessage( "{0}, you has been condamned to Hard Labour. To expire your guilt you have to dig {1} ores", m2Pm.Name, m2Pm.Minerals2Mine );
        }

        private static string CreatePrisonerNumber( int num )
        {
            string s2 = num.ToString().PadLeft( 9, '0' );
            char[] s1 = s2.ToCharArray();
            char cc = '-';
            s1[ 3 ] = cc;

            string s3 = new string( s1 );

            return s3;
        }

        private static BaseDoor CreateDoors( int xDoor, int yDoor, int zDoor, bool doorEastToWest, int hue )
        {
            BaseDoor hiDoor = new MetalDoor( doorEastToWest ? DoorFacing.NorthCCW : DoorFacing.WestCW );
            BaseDoor loDoor = new MetalDoor( doorEastToWest ? DoorFacing.SouthCW : DoorFacing.EastCCW );

            hiDoor.MoveToWorld( new Point3D( xDoor, yDoor, zDoor ), Map.Malas );
            loDoor.MoveToWorld( new Point3D( xDoor + ( doorEastToWest ? 0 : 1 ), yDoor + ( doorEastToWest ? 1 : 0 ), zDoor ), Map.Malas );

            hiDoor.Link = loDoor;
            loDoor.Link = hiDoor;

            hiDoor.Hue = hue;
            loDoor.Hue = hue;

            return hiDoor;
        }

        private class InternalTarget : Target
        {
            private readonly int m_Pena;
            private readonly Mobile m_Condamner;

            public InternalTarget( int pena, Mobile condamner )
                : base( 10, false, TargetFlags.None )
            {
                m_Pena = pena;
                m_Condamner = condamner;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                Midgard2PlayerMobile m2Pm = targeted as Midgard2PlayerMobile;

                if( m2Pm == null || m2Pm.Deleted )
                    from.SendMessage( "You must target a Player!" );
                else if( from.AccessLevel < m2Pm.AccessLevel )
                    from.SendMessage( "You have NOT enought privileges to condemn this player." );
                else if( !m2Pm.Alive )
                    from.SendMessage( "Resurrect player before condamn execution." );
                else
                    DoHardLabourCondemn( m2Pm, m_Pena, m_Condamner.Name );
            }
        }

        public static void LogMinerals2MineChanged( Mobile m, int oldValue, int minerals2Mine )
        {
            try
            {
                TextWriter tw = File.AppendText( "Logs/Minerals2MineChangedLog.log" );
                tw.WriteLine( String.Format( "Player: {0} - Account {1} - Serial {2} - DateTime {3} - Minerals from: {4} - Minerals to {5}.",
                    String.IsNullOrEmpty( m.Name ) ? "null" : m.Name, m.Account.Username, m.Serial,
                    DateTime.Now, oldValue, minerals2Mine ) );
                tw.Close();
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }
    }
}