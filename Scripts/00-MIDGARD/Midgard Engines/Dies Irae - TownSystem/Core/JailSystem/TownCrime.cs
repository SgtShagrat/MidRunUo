using System;
using Server;
using Server.Items;

namespace Midgard.Engines.MidgardTownSystem
{
    public class TownCrime
    {
        public Mobile Criminal
        {
            get { return OwnerProfile != null ? OwnerProfile.Criminal : null; }
        }

        public CriminalProfile OwnerProfile { get; set; }
        public Mobile Target { get; set; }
        public DateTime DateOfCrime { get; set; }
        public CrimeType CrimeType { get; set; }

        public TimeSpan DefaultDuration
        {
            get
            {
                switch( CrimeType )
                {
                    case CrimeType.StealAction:
                        return TimeSpan.FromMinutes( Clock.MinutesPerUODay );
                    case CrimeType.ReportedAsMurderer:
                        return TimeSpan.FromMinutes( Clock.MinutesPerUODay * 7.0 );
                }

                return TimeSpan.FromMinutes( Clock.MinutesPerUODay );
            }
        }

        public int DefaultUolTimeDuration
        {
            get
            {
                switch( CrimeType )
                {
                    case CrimeType.StealAction:
                        return 1;
                    case CrimeType.ReportedAsMurderer:
                        return 7;
                }

                return 1;
            }
        }

        public string DefaultName
        {
            get
            {
                switch( CrimeType )
                {
                    case CrimeType.StealAction:
                        return "stealing from citizen";
                    case CrimeType.ReportedAsMurderer:
                        return "killing the civilians";
                }

                return "a generic crime";
            }
        }

        public bool Expired
        {
            get { return DateTime.Now > ExpirationTime; }
        }

        public DateTime ExpirationTime
        {
            get { return DateOfCrime + DefaultDuration; }
        }

        public bool Condamned { get; set; }

        private Condemn m_Condemn;

        public Condemn Condemn
        {
            get
            {
                if( m_Condemn == null )
                    m_Condemn = TownJailSystem.Instance.FindCondemnByCrime( this );

                return m_Condemn;
            }
        }

        public TownCrime( CriminalProfile ownerProfile, Mobile target, DateTime dateOfCrime, CrimeType crimeType )
        {
            OwnerProfile = ownerProfile;
            Target = target;
            DateOfCrime = dateOfCrime;
            CrimeType = crimeType;

            Condamned = false;
        }

        public override string ToString()
        {
            return string.Format( "The town of {0} has found {1} violating its laws. The crime is {2}." +
                                  "The crime has been reported in date and time {3}.",
                                  OwnerProfile.OwnerSystem.Definition.TownName, Criminal.Name, DefaultName, DateOfCrime );
        }

        #region serialization
        public void Serialize( GenericWriter writer )
        {
            if( Config.Debug )
            Config.Pkg.LogInfoLine( "Saving town crime..." );

            writer.Write( 0 );

            writer.Write( Target );
            writer.Write( DateOfCrime );
            writer.Write( (int)CrimeType );
        }

        public TownCrime( CriminalProfile ownerProfile, GenericReader reader )
        {
            if( Config.Debug )
                Config.Pkg.LogInfoLine( "Loading town crime..." );

            OwnerProfile = ownerProfile;

            reader.ReadInt();

            Target = reader.ReadMobile();
            DateOfCrime = reader.ReadDateTime();
            CrimeType = (CrimeType)reader.ReadInt();

            if( Condemn != null )
            {
                Config.Pkg.LogWarning( "TownCrime: Condemn.Profile == null " + ( Condemn.Profile == null ) );

                if( Condemn.Profile == null )
                    Timer.DelayCall( TimeSpan.Zero, new TimerCallback( LinkProfileToCondemn ) );
            }

            if( Config.Debug )
                Config.Pkg.LogInfoLine( "done." );
        }

        private void LinkProfileToCondemn()
        {
            if( Config.Debug )
                Console.WriteLine( "LinkProfileToCondemn: OwnerProfile != null -> " + OwnerProfile != null );

            Condemn.Profile = OwnerProfile;

            if( Config.Debug )
                Console.WriteLine( "LinkProfileToCondemn: Condemn.Profile != null -> " + Condemn.Profile != null );
        }

        #endregion
    }
}