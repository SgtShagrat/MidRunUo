using System;
using System.Collections.Generic;
using System.Text;

using Midgard.Misc;
using Server;
using Server.Engines.XmlSpawner2;
using Server.Items;
using Server.Mobiles;

using Midgard.Engines.MyXmlRPC;
using Core = Midgard.Engines.MyXmlRPC.Core;

namespace Midgard.Items
{
    public interface ITreasureOfMidgard
    {
        void Doc( StringBuilder s );
    }

    public class TreasuresOfMidgard
    {
        private static readonly bool m_Enabled = Midgard2Persistance.ToMEnabled;

        public static bool Enabled
        {
            get { return m_Enabled; }
        }

        public static Type[] Artifacts
        {
            get { return m_Artifacts; }
        }

        private static readonly Type[] m_Artifacts = new Type[]
                                                {
                                                    typeof( MidgardRobe ),

                                                    typeof( FemalePlateOfSagiptar ),
                                                    typeof( PlateHelmOfSagiptar ),
                                                    typeof( PlateArmsOfSagiptar ),
                                                    typeof( PlateLegsOfSagiptar ),
                                                    typeof( PlateGlovesOfSagiptar ),
                                                    typeof( PlateGorgetOfSagiptar ),
                                                    typeof( PlateChestOfSagiptar ),

                                                    typeof( LeatherGorgetOfWater ),
                                                    typeof( LeatherArmsOfWater ),
                                                    typeof( LeatherGlovesOfWater ),
                                                    typeof( LeatherCapOfWater ),
                                                    typeof( LeatherLegsOfWater ),
                                                    typeof( LeatherTunicOfWater ),

                                                    typeof( PlateHelmOfWater ),
                                                    typeof( PlateArmsOfWater ),
                                                    typeof( PlateLegsOfWater ),
                                                    typeof( PlateGlovesOfWater ),
                                                    typeof( PlateGorgetOfWater ),
                                                    typeof( PlateChestOfWater ),

                                                    typeof( FemalePlateOfRyous ),
                                                    typeof( PlateHelmOfRyous ),
                                                    typeof( PlateArmsOfRyous ),
                                                    typeof( PlateLegsOfRyous ),
                                                    typeof( PlateGlovesOfRyous ),
                                                    typeof( PlateGorgetOfRyous ),
                                                    typeof( PlateChestOfRyous ),

                                                    typeof( FemalePlateOfDarkness ),
                                                    typeof( PlateHelmOfDarkness ),
                                                    typeof( PlateArmsOfDarkness ),
                                                    typeof( PlateLegsOfDarkness ),
                                                    typeof( PlateGlovesOfDarkness ),
                                                    typeof( PlateGorgetOfDarkness ),
                                                    typeof( PlateChestOfDarkness ),

                                                    typeof( MidgardVikingSword ),
                                                    typeof( MidgardHalberd ),
                                                    typeof( MidgardKryss ),
                                                    typeof( MidgardStaff ),
                                                    typeof( MidgardKatana ),
                                                    typeof( MidgardWarMace ),
                                                    typeof( MidgardWarAxe ),
                                                    typeof( WarForkOfWater ),
                                                    typeof( DaggerOfExtraStrike ),

                                                    typeof( GnarledStaffOfPoison ),
                                                    typeof( ClubOfPoison ),
                                                    typeof( MidgardLongSword ),
                                                };

        public static bool IsMajorArtifact( Type t )
        {
            return Array.LastIndexOf( m_Artifacts, t ) > -1;
        }

        public static void Initialize()
        {
            // http://89.97.211.236/xmlrpc?user=XmlRpcUser&pass=securexmlpass&xcmd=getArtifactsStatus
            Core.Register( "getArtifactStatus", new MyXmlEventHandler( GetArtifactStatusOnCommand ), null );

            PreAoSDocHelper.Register( new TreasuresOfMidgardDocHandler() );
        }

        public static void GetArtifactStatusOnCommand( MyXmlEventArgs e )
        {
            if( Core.Debug )
                Core.Pkg.LogInfoLine( "GetArtifactStatus command called..." );

            e.Exitcode = -1;

            try
            {
                //e.CustomResultTree.Add( new XElement( "status", from state in BuildList()
                //                                                where state != null
                //                                                select state.ToXElement() ) );

                e.Exitcode = 0;
            }
            catch( Exception warning )
            {
                e.Warnings.Add( warning );
            }
        }

        private static bool CheckLocation( Mobile m )
        {
            Region r = m.Region;

            return !r.IsPartOf( typeof( Server.Regions.HouseRegion ) ) && Server.Multis.BaseBoat.FindBoatAt( m, m.Map ) == null;
        }

        public static void HandleKill( Mobile victim, Mobile killer )
        {
            Midgard2PlayerMobile m2Pm = killer as Midgard2PlayerMobile;
            BaseCreature bc = victim as BaseCreature;

            if( !Enabled || m2Pm == null || bc == null || !CheckLocation( bc ) || !CheckLocation( m2Pm ) || !killer.InRange( victim, 18 ) )
                return;

            if( bc.Controlled || bc.Owners.Count > 0 || bc.Fame <= 0 )
                return;

            //25000 for 1/100 chance, 10 hyrus
            //1500, 1/1000 chance, 20 lizard men for that chance.

            if( !Midgard2Persistance.ToMEnabled )
                return;

            double fame = bc.Fame;
            if( killer.Map == Map.Ilshenar )
                fame *= 2;
            else
                fame /= 2;

            if( fame > 32000 )
                fame = 32000;

            if( fame >= Midgard2Persistance.ToMMinFameNoGain )
                m2Pm.ToMTotalMonsterFame += (int)( fame );

            //This is the Exponentional regression with only 2 datapoints.
            //A log. func would also work, but it didn't make as much sense.
            int x = m2Pm.ToMTotalMonsterFame;

            //const double A = 8.63316841 * Math.Pow( 10, -4 );
            const double a = 0.000863316841;
            //const double B = 4.25531915 * Math.Pow( 10, -6 );
            const double b = 0.00000425531915;

            double chance = ( a * Math.Pow( 10, b * x ) ) * Midgard2Persistance.ToMDemultiplier;

            if( killer.AccessLevel == AccessLevel.Developer )
            {
                killer.SendMessage( "ToM chance: {0}\n", chance.ToString( "F4" ) );
                chance = 2.0;
            }

            if( chance > Utility.RandomDouble() )
            {
                Item artifact = null;

                try
                {
                    artifact = GetRandomArtifact();
                }
                catch
                { }

                if( artifact != null )
                {
                    XmlBlessedCursedAttach attach = new XmlBlessedCursedAttach( 5, m2Pm, XmlBlessedCursedAttach.MagicalTypes.Bless );
                    XmlAttach.AttachTo( artifact, attach );

                    Container pack = m2Pm.Backpack;

                    if( pack == null || !pack.TryDropItem( m2Pm, artifact, false ) )
                        m2Pm.BankBox.DropItem( artifact );

                    Loot.StringToLog( string.Format( "Artefatto *old midgard* {0} (seriale {1}) droppato in data {2} al pg {3} (seriale {4}).",
                                                     artifact.GetType().Name, artifact.Serial, DateTime.Now,
                                                     m2Pm.Name, m2Pm.Serial ), "Logs/MidgardArtifactsLog.txt" );

                    m2Pm.SendLocalizedMessage( 1062317 ); // For your valor in combating the fallen beast, a special artifact has been bestowed on you.
                    m2Pm.ToMTotalMonsterFame = 0;
                }
            }
        }

        private static Dictionary<Type, int> m_Table = new Dictionary<Type, int>();

        public static int MaxNumberOfArtifacts = 2;

        public static void RegisterExistance( Type t )
        {
            if( m_Table == null )
                m_Table = new Dictionary<Type, int>();

            if( m_Table.ContainsKey( t ) )
                m_Table[ t ]++;
        }

        public static bool CanBeCreated( Type t )
        {
            return GetNumberOnShard( t ) < MaxNumberOfArtifacts;
        }

        public static int GetNumberOnShard( Type t )
        {
            int number = 0;

            if( m_Table == null )
                return 0;

            if( m_Table.TryGetValue( t, out number ) )
                number++;

            return number;
        }

        public static Item GetRandomArtifact()
        {
            Item artifact = null;

            List<Type> list = new List<Type>();
            foreach( Type t in Artifacts )
            {
                if( CanBeCreated( t ) )
                    list.Add( t );
            }

            try
            {
                artifact = Activator.CreateInstance( list[ Utility.Random( list.Count ) ] ) as Item;
            }
            catch
            { }

            return artifact;
        }
    }
}