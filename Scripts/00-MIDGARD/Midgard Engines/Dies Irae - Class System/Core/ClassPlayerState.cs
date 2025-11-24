/***************************************************************************
 *                               ClassPlayerState.cs
 *
 *   revision             : 03 January, 2010
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using Midgard.Engines.SpellSystem;
using Server;
using Server.Accounting;
using Server.Mobiles;

using Utility = Midgard.Engines.MyMidgard.Utility;

namespace Midgard.Engines.Classes
{
    [PropertyObject]
    public class ClassPlayerStateCollection : Collection<ClassPlayerState>
    {
        public override string ToString()
        {
            return "...";
        }
    }

    [PropertyObject]
    public class ClassPlayerState : IComparable
    {
        private string m_CustomClassTitle;
        private BaseClassAttributes m_PowerAttributes;

        public ClassPlayerState( ClassSystem system, Mobile mobile )
        {
            ClassSystem = system;
            Mobile = mobile;

            Init();

            if( ClassSystem.Definition.PowersDefinitions != null )
            {
                Powers = new int[ ClassSystem.Definition.PowersDefinitions.Length ];
                for( int i = 0; i < Powers.Length; i++ )
                    Powers[ i ] = 0;

                foreach( PowerDefinition definition in ClassSystem.Definition.PowersDefinitions )
                {
                    if( definition.FirstLevelGranted )
                        SetLevel( definition, 1 );
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster, true )]
        public ClassSystem ClassSystem { get; private set; }

        [CommandProperty( AccessLevel.GameMaster, true )]
        public Mobile Mobile { get; private set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Seer )]
        public string CustomClassTitle
        {
            get { return m_CustomClassTitle; }
            set
            {
                m_CustomClassTitle = value;
                Invalidate();
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsInactive
        {
            get
            {
                if( Mobile != null )
                {
                    Account acct = Mobile.Account as Account;
                    if( acct != null )
                    {
                        if( ( acct.LastLogin + TimeSpan.FromDays( 60.0 ) ) > DateTime.Now )
                            return false;
                    }
                }

                return true;
            }
        }

        private int[] Powers { get; set; }

        public bool ShowsTitle
        {
            get { return ClassSystem.ShowsTitle; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool DisplayClassStatus
        {
            get { return Mobile is Midgard2PlayerMobile && ( (Midgard2PlayerMobile)Mobile ).DisplayClassStatus; }
            set
            {
                if( Mobile is Midgard2PlayerMobile )
                    ( (Midgard2PlayerMobile)Mobile ).DisplayClassStatus = value;
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public int TotalPowers
        {
            get
            {
                int total = 0;
                foreach( int i in Powers )
                    total += i;

                return total;
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public int TotalPowersPercent
        {
            get { return (int)( 100 * ( TotalPowers / ( Powers.Length * 5.0 ) ) ); }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public BaseClassAttributes PowerAttributes
        {
            get { return m_PowerAttributes; }
            set { }
        }

        #region IComparable Members
        public int CompareTo( object obj )
        {
            return GetStatePower() - ( (ClassPlayerState)obj ).GetStatePower();
        }
        #endregion

        #region serialization
        public ClassPlayerState()
        {
        }

        public virtual void Deserialize( ClassSystem system, GenericReader reader )
        {
            ClassSystem = system;
            Init();

            int version = reader.ReadEncodedInt();

            switch( version )
            {
                case 0:
                    Mobile = reader.ReadMobile();
                    m_CustomClassTitle = reader.ReadString();

                    int powersCount = reader.ReadEncodedInt();
                    if( powersCount > 0 )
                    {
                        int[] array = new int[ powersCount ];
                        for( int i = 0; i < powersCount; ++i )
                            array[ i ] = reader.ReadEncodedInt();

                        Powers = array;
                    }
                    break;
            }

            if( Mobile == null )
            {
                Config.Pkg.LogInfoLine( "Warning: ClassPlayerState with null mobile detected. Removing..." );
                Timer.DelayCall( TimeSpan.Zero, Detach );
            }
            /*
            else
                Timer.DelayCall( TimeSpan.Zero, new TimerCallback( CheckAttach ) );
            */
        }

        public virtual void Serialize( GenericWriter writer )
        {
            writer.WriteEncodedInt( 0 ); // version

            // Version 0
            writer.Write( Mobile );
            writer.Write( m_CustomClassTitle );

            if( Powers != null )
            {
                writer.WriteEncodedInt( Powers.Length );
                foreach( int t in Powers )
                    writer.WriteEncodedInt( t );
            }
            else
                writer.WriteEncodedInt( 0 );
        }
        #endregion

        public static ClassPlayerState Find( Mobile mob )
        {
            return Find( mob, false );
        }

        public static ClassPlayerState Find( Mobile mob, bool inherit )
        {
            if( mob == null )
                return null;

            Midgard2PlayerMobile player = mob as Midgard2PlayerMobile;

            if( player == null )
            {
                if( inherit && mob is BaseCreature )
                {
                    BaseCreature bc = mob as BaseCreature;

                    if( bc.Controlled )
                        player = bc.ControlMaster as Midgard2PlayerMobile;
                    else if( bc.Summoned )
                        player = bc.SummonMaster as Midgard2PlayerMobile;
                }

                if( player == null )
                    return null;
            }

            ClassPlayerState state = player.ClassState;

            if( state != null && !state.ClassSystem.IsEligible( state.Mobile ) )
                player.ClassState = state = null;

            return state;
        }

        public void Invalidate()
        {
            if( Mobile is PlayerMobile )
                Mobile.InvalidateProperties();
        }

        public void Init()
        {
            m_CustomClassTitle = String.Empty;
            m_PowerAttributes = ClassSystem.GetNewPowerAttributes( this );
        }

        #region paladin
        [CommandProperty( AccessLevel.Seer )]
        public bool HasPrayed
        {
            get { return ( DateTime.Now - LastPrayed ) < PaladinSystem.GetPrayDuration( Mobile ); }
        }

        [CommandProperty( AccessLevel.Seer )]
        public DateTime LastPrayed { get; set; }

        [CommandProperty( AccessLevel.Seer )]
        public bool IsWaitingCriticalShot { get; set; }

        [CommandProperty( AccessLevel.Seer )]
        public HolyMount HolyMount
        {
            get { return HolyMountSpell.GetMount( Mobile ); }
            set { HolyMountSpell.SetMount( Mobile, value ); }
        }
        #endregion

        public bool IncreasePowerLevel( PowerDefinition def )
        {
            if( Powers == null )
                Powers = new int[ ClassSystem.Definition.PowersDefinitions.Length ];

            int index = GetDefIndex( def );

            if( index > -1 && index < Powers.Length )
            {
                int level = Powers[ index ];

                if( level < def.MaxRituals )
                {
                    Powers[ index ] = Powers[ index ] + 1;
                    return true;
                }
            }

            return false;
        }

        public PowerDefinition GetDefinitionByIndex( int index )
        {
            return ClassSystem.Definition.PowersDefinitions[ index ];
        }

        /*
        public int GetLevel( int index )
        {
            return ( Powers != null && index < Powers.Length && index >= 0 ) ? Powers[ index ] : 0;
        }
        */

        public void RandomizePowers()
        {
            foreach( PowerDefinition def in ClassSystem.Definition.PowersDefinitions )
            {
                SetLevel( def, Server.Utility.RandomMinMax( 0, def.MaxRituals ) );
            }
        }

        public void ClassPowers( PowerDefinition def, int level )
        {
            SetLevel( def, level );
        }

        public int GetLevel( Type t )
        {
            if( Powers == null )
                return 0;

            PowerDefinition def = GetDefBySpellType( t ) ?? GetDefByDefType( t );
            return def != null ? GetLevel( def ) : 0;
        }

        public int GetLevel( PowerDefinition def )
        {
            if( Powers == null )
                return 0;

            if( def != null && def.IsGranted )
                return def.MaxRituals;

            int index = GetDefIndex( def );
            if( index > -1 && index < Powers.Length )
                return Powers[ index ];

            return 0;
        }

        public void SetLevel( Type t, int level )
        {
            PowerDefinition def = GetDefBySpellType( t ) ?? GetDefByDefType( t );
            if( def != null )
                SetLevel( def, level );
        }

        public void SetLevel( PowerDefinition def, int level )
        {
            if( Powers == null )
                Powers = new int[ ClassSystem.Definition.PowersDefinitions.Length ];

            int index = GetDefIndex( def );

            if( index > -1 && index < Powers.Length )
            {
                if( level <= def.MaxRituals )
                    Powers[ index ] = level;
            }
        }

        private int GetDefIndex( PowerDefinition def )
        {
            return Array.IndexOf( ClassSystem.Definition.PowersDefinitions, def );
        }

        public PowerDefinition GetDefByName( string name )
        {
            if( string.IsNullOrEmpty( name ) )
                return null;

            foreach( PowerDefinition def in ClassSystem.Definition.PowersDefinitions )
            {
                if( def.PowerType.Name == name )
                    return def;
            }

            return null;
        }

        public PowerDefinition GetDefBySpellType( Type type )
        {
            foreach( PowerDefinition def in ClassSystem.Definition.PowersDefinitions )
            {
                if( def.PowerType == type )
                    return def;
            }

            return null;
        }

        public PowerDefinition GetDefByDefType( Type type )
        {
            foreach( PowerDefinition def in ClassSystem.Definition.PowersDefinitions )
            {
                if( def.GetType() == type )
                    return def;
            }

            return null;
        }

        public int GetDefIndex( Type type )
        {
            foreach( PowerDefinition def in ClassSystem.Definition.PowersDefinitions )
            {
                if( def.PowerType == type )
                    return GetDefIndex( def );
            }

            return -1;
        }

        public bool HasPower( Type t )
        {
            PowerDefinition def = GetDefBySpellType( t ) ?? GetDefByDefType( t );
            if( def != null && def.IsGranted )
                return true;

            return def != null && GetLevel( def ) > 0;
        }

        public void CheckAttach()
        {
            if( ClassSystem != null && ClassSystem.IsEligible( Mobile ) )
                Attach( false );
        }

        public void Attach( bool force )
        {
            if( ClassSystem == null )
            {
                Config.Pkg.LogInfoLine( "Warning: class player state with null class system. Failed attach..." );
                return;
            }
            else if( Mobile == null )
            {
                Config.Pkg.LogInfoLine( "Warning: class player state with null mobile. Failed attach..." );
                return;
            }
            else if( ClassSystem.GetMobilesFromStates().Contains( Mobile ) )
            {
                Config.Pkg.LogInfoLine( "Warning: playerstate (mobile {0}) is already in {1} class system. Failed attach, trying to remove.", Mobile.Name, ClassSystem.Definition.ClassName );
                ClassSystem.RemoveMobileFromState( Mobile );
                Mobile.SendMessage( "Bug, probably you were already classed." );
            }

            if( Mobile is Midgard2PlayerMobile )
            {
                ( (Midgard2PlayerMobile)Mobile ).ClassState = this;
                ClassSystem.Players.Add( this );
                ClassSystem.OnClassSystemJoined( this );
            }
        }

        public int GetStatePower()
        {
            return 0;
        }

        public void Detach()
        {
            if( Mobile is Midgard2PlayerMobile )
                ( (Midgard2PlayerMobile)Mobile ).ClassState = null;

            ClassSystem.Players.Remove( this );
        }

        public override string ToString()
        {
            return "...";
        }

        public XElement ToXElement()
        {
            return new XElement( "state", new XAttribute( "owner", Utility.SafeString( Mobile.Name ?? "" ) ),
                                new XAttribute( "account", Utility.SafeString( Mobile.Account.Username ?? "" ) ),
                                new XAttribute( "creation", Mobile.CreationTime.ToString() ),
                                new XAttribute( "title", Utility.SafeString( m_CustomClassTitle ?? "" ) ),
                                new XElement( "powers",
                                             from d in ClassSystem.Definition.PowersDefinitions
                                             let l = GetLevel( d )
                                             where l > 0
                                             select new XElement( "power",
                                                                 new XAttribute( "name", Utility.SafeString( d.Name ?? "" ) ),
                                                                 new XAttribute( "level", l ) ) ) );
        }
    }
}