using System;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Midgard.Engines.MidgardTownSystem
{
    public class Condemn
    {
        public Condemn( CriminalProfile profile, Mobile jailor, DateTime expirationTime, JailCell assignedCell )
        {
            Profile = profile;

            AssignedCell = assignedCell;
            ExpirationTime = expirationTime;
            Jailor = jailor;

            TownJailSystem.RegisterCondemn( this );
        }

        public Condemn( CriminalProfile profile, DateTime expirationTime, JailCell assignedCell )
            : this( profile, null, expirationTime, assignedCell )
        {
            Jailor = null;
        }

        /// <summary>
        /// The owner of this condemn
        /// </summary>
        public Mobile Prisoner
        {
            get { return Profile != null ? Profile.Criminal : null; }
        }

        /// <summary>
        /// True if this condemn is ended
        /// </summary>
        public bool Expired
        {
            get { return DateTime.Now > ExpirationTime; }
        }

        /// <summary>
        /// True if the crime, the prisoner, the cell are valid and if the Prisoner is already in the cell
        /// </summary>
        public bool IsValid
        {
            get
            {
                return Profile != null && Prisoner != null && AssignedCell != null &&
                       AssignedCell.IsPrisonerInCell( Prisoner );
            }
        }

        /// <summary>
        /// The time when this condemn will end
        /// </summary>
        public DateTime ExpirationTime { get; private set; }

        /// <summary>
        /// The mobile who condemned our prisoner. May be null
        /// </summary>
        public Mobile Jailor { get; private set; }

        /// <summary>
        /// The jail where the prisoner must stay and think about his crimes
        /// </summary>
        public JailCell AssignedCell { get; private set; }

        public CriminalProfile Profile { get; set; }

        public object SourceXElement { get; set; }

        #region serialization
        public void Serialize( GenericWriter writer )
        {
            writer.Write( 0 );

            writer.Write( ExpirationTime );
            writer.Write( Jailor );
            writer.Write( AssignedCell.Name ?? "" );
        }

        public Condemn( GenericReader reader )
        {
            int version = reader.ReadInt();

            ExpirationTime = reader.ReadDateTime();
            Jailor = reader.ReadMobile();

            string cellName = reader.ReadString();
            if( !string.IsNullOrEmpty( cellName ) )
                Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( FindJailCellByName ), cellName );
        }

        private void FindJailCellByName( object state )
        {
            string name = state as string;
            AssignedCell = TownJailSystem.Instance.FindCellByName( name );
        }
        #endregion

        /// <summary>
        /// Verify if this condemn is expired. If so the prisoner must be released.
        /// </summary>
        public void CheckRelease()
        {
            if( IsValid && Expired && TownJailSystem.Instance.HasNoMoreCondemns( Prisoner ) )
                Release();
        }

        /// <summary>
        /// Release the prisoner. If dead he is resurrected. The release message is sent.
        /// </summary>
        public void Release()
        {
            if( Prisoner == null )
                return;

            if( !Prisoner.Alive )
                Prisoner.Resurrect();

            Prisoner.MoveToWorld( TownJailSystem.JailReleaseLocation, TownJailSystem.JailsMap );

            JailHandcuffs cuffs = Prisoner.FindItemOnLayer( Layer.Bracelet ) as JailHandcuffs;
            if( cuffs != null )
                cuffs.CheckDelete();

            if( Prisoner.NetState == null || !( Prisoner is Midgard2PlayerMobile ) )
                return;

            string message = string.Format( TownJailSystem.ReleaseMessage, Prisoner.Name, Profile.OwnerSystem.Definition.TownName );

            ( (Midgard2PlayerMobile)Prisoner ).SendCustomScrollMessage( message );
        }

        /// <summary>
        /// This method apply the condemn to our criminal
        /// </summary>
        public void Execute()
        {
            Mobile prisoner = Prisoner;

            #region effetti visivi e trasporto alla colonia penitenziaria
            if( prisoner.Map != Map.Internal )
                TownJailSystem.EffectCircle( prisoner.Location, prisoner.Map, 3 );

            // Trasferisce il condannato nella colonia penitenziaria
            prisoner.Map = TownJailSystem.JailsMap;
            prisoner.Location = AssignedCell.GoLocation;
            if( AssignedCell.Door != null )
                AssignedCell.Door.Locked = true;

            if( prisoner.Map != Map.Internal )
            {
                TownJailSystem.EffectCircle( prisoner.Location, prisoner.Map, 3 );
                prisoner.PlaySound( 0x228 );
            }
            #endregion

            #region confisca dei beni
            // Confisco i beni del codannato e li pongo in banca in uno zaino
            Backpack prisonerBag = new Backpack();
            prisonerBag.Hue = 666;
            prisonerBag.Name = "A Prisoner Bag";
            prisoner.BankBox.DropItem( prisonerBag );

            List<Item> equipitems = new List<Item>( prisoner.Items );
            for( int i = 0; i < equipitems.Count; i++ )
            {
                if( !equipitems[ i ].Movable )
                    continue;

                if( ( equipitems[ i ].Layer != Layer.Bank ) && ( equipitems[ i ].Layer != Layer.Mount ) &&
                    ( equipitems[ i ].Layer != Layer.Backpack ) )
                    prisonerBag.DropItem( equipitems[ i ] );
            }

            List<Item> packitems = new List<Item>( prisoner.Backpack.Items );
            for( int i = 0; i < packitems.Count; i++ )
            {
                if( packitems[ i ].Movable )
                    prisonerBag.DropItem( packitems[ i ] );
            }
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

                    if( pet != null )
                    {
                        pet.ControlTarget = null;
                        pet.ControlOrder = OrderType.Stay;
                        pet.Internalize();

                        pet.SetControlMaster( null );
                        pet.SummonMaster = null;

                        pet.IsStabled = true;

                        pet.Loyalty = BaseCreature.MaxLoyalty;

                        prisoner.Stabled.Add( pet );
                    }
                }
            }
            catch( Exception ex )
            {
                Config.Pkg.LogWarning( "Warning: TownJail System - Pet non stabled correctly: {0}", ex.ToString() );
            }
            #endregion

            #region vestizione
            prisoner.AddItem( new Robe( 0 ) );

            JailHandcuffs cuffs = new JailHandcuffs();
            prisoner.AddItem( cuffs );
            cuffs.Movable = false;
            #endregion
        }
    }
}