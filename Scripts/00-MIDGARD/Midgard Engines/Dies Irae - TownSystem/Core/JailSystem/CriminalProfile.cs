using System;
using System.Collections.Generic;
using System.Text;
using Server;
using Server.Mobiles;

namespace Midgard.Engines.MidgardTownSystem
{
    public class CriminalProfile
    {
        /// <summary>
        /// The player who owns this profile
        /// </summary>
        public Mobile Criminal { get; private set; }

        /// <summary>
        /// The date of creation of this profile
        /// </summary>
        public DateTime CreationTime { get; private set; }

        /// <summary>
        /// The town system which this profile belongs ti
        /// </summary>
        public TownSystem OwnerSystem { get; private set; }

        /// <summary>
        /// The list of crimes which are contained in this profile
        /// </summary>
        public List<TownCrime> Crimes { get; private set; }

        /// <summary>
        /// Useful comments for this profile
        /// </summary>
        public List<string> Comments { get; private set; }

        /// <summary>
        /// true if the criminal has pending arrestable crimes
        /// </summary>
        public bool IsCatchable
        {
            get
            {
                if( Crimes == null || Crimes.Count == 0 )
                    return false;

                foreach( TownCrime crime in Crimes )
                {
                    if( crime != null && !crime.Expired && !crime.Condamned )
                        return true;
                }

                return false;
            }
        }

        public DateTime CatchableUntil
        {
            get
            {
                DateTime max = DateTime.MinValue;

                if( !IsCatchable )
                    return max;

                foreach( TownCrime crime in Crimes )
                {
                    if( crime != null && crime.ExpirationTime > max && !crime.Condamned )
                        max = crime.ExpirationTime;
                }

                return max;
            }
        }

        public TownCrime LastCrime
        {
            get
            {
                if( Crimes == null )
                    return null;

                return Crimes[ Crimes.Count - 1 ];
            }
        }

        public bool IsUnderCondamn
        {
            get { return TownJailSystem.Instance.IsActuallyCondemned( Criminal ); }
        }

        public CriminalProfile( Mobile criminal, TownSystem ownerSystem )
        {
            CreationTime = DateTime.Now;
            Criminal = criminal;
            OwnerSystem = ownerSystem;

            ownerSystem.CriminalProfiles.Add( this );
        }

        public void AddComment( string comment, Mobile from )
        {
            if( Comments == null )
                Comments = new List<string>();

            if( from != null )
                Comments.Add( string.Format( "{0}: {1}", from.Name, comment ) );
            else
                Comments.Add( string.Format( "The jailors: {0}", comment ) );
        }

        public void AddCrime( Mobile victim, CrimeType type )
        {
            if( Crimes == null )
                Crimes = new List<TownCrime>();

            Crimes.Add( new TownCrime( this, victim, DateTime.Now, type ) );

            Crimes.Sort( AgeCrimeComparer.Instance );
        }

        public int GetCrimesByType( CrimeType type )
        {
            int count = 0;

            foreach( TownCrime crime in Crimes )
            {
                if( crime.CrimeType == type )
                    count++;
            }

            return count;
        }

        public void Serialize( GenericWriter writer )
        {
            if( Config.Debug )
                Config.Pkg.LogInfoLine( "Saving criminal profile..." );

            writer.Write( 0 );

            writer.Write( Criminal );
            writer.Write( CreationTime );

            if( Crimes != null )
            {
                writer.Write( Crimes.Count );
                foreach( TownCrime crime in Crimes )
                    crime.Serialize( writer );
            }
            else
                writer.Write( 0 );

            if( Comments != null )
            {
                writer.Write( Comments.Count );
                foreach( string comment in Comments )
                    writer.Write( comment );
            }
            else
                writer.Write( 0 );
        }

        public CriminalProfile( TownSystem ownerSystem, GenericReader reader )
        {
            if( Config.Debug )
                Config.Pkg.LogInfoLine( "Loading criminal profile..." );

            OwnerSystem = ownerSystem;

            int version = reader.ReadInt();

            Criminal = reader.ReadMobile();
            CreationTime = reader.ReadDateTime();

            Crimes = new List<TownCrime>();
            int crimeCount = reader.ReadInt();
            if( crimeCount > 0 )
            {
                for( int i = 0; i < crimeCount; i++ )
                    Crimes.Add( new TownCrime( this, reader ) );
            }

            Comments = new List<string>();
            int commentsCount = reader.ReadInt();
            if( commentsCount > 0 )
            {
                for( int i = 0; i < commentsCount; i++ )
                    Comments.Add( reader.ReadString() );
            }

            if( Config.Debug )
                Config.Pkg.LogInfoLine( "done." );
        }

        public List<TownCrime> GetCondamnableCrimes()
        {
            List<TownCrime> crimes = new List<TownCrime>();

            foreach( TownCrime crime in Crimes )
            {
                if( crime != null && !crime.Expired )
                    crimes.Add( crime );
            }

            return crimes;
        }

        public void SendCondemnMessage()
        {
            List<TownCrime> crimes = GetCondamnableCrimes();

            StringBuilder sb = new StringBuilder();
            foreach( TownCrime crime in crimes )
            {
                sb.Append( string.Format( "  {0} (detenction: {1:} day{2})\n", crime.DefaultName,
                                          crime.DefaultUolTimeDuration,
                                          crime.DefaultUolTimeDuration > 1 ? "s" : "" ) );
            }

            string message = string.Format( TownJailSystem.JailMessage, OwnerSystem.Definition.TownName, sb );

            message = message.Replace( Char.ConvertFromUtf32( 0x0A ), Char.ConvertFromUtf32( 0x0D ) );
            message = message.Replace( (char)0x0A, (char)0x0D );

            if( Criminal is Midgard2PlayerMobile )
                ( (Midgard2PlayerMobile)Criminal ).SendCustomScrollMessage( string.Format( message, OwnerSystem.Definition.TownName ) );
        }

        public void UpdateForCondemn( Condemn condemn )
        {
            foreach( TownCrime crime in Crimes )
            {
                if( !crime.Condamned )
                    crime.Condamned = true;

                AddComment( string.Format( "Executed condemn on {0}", DateTime.Now ), condemn.Jailor );
            }
        }
    }
}