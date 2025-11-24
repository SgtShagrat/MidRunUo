/***************************************************************************
 *                                      SantaClaus.cs
 *                            		--------------------
 *  begin                	: Dicembre, 2008
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 *  		Babbo Natale... che c'e' altro da dire?!
 *  
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Mobiles
{
    public class SantaClaus : BaseVendor
    {
        #region constants
        private static readonly bool Enabled = true;
        private static readonly int MinDelayMessage = 10;
        private static readonly int MaxDelayMessage = 20;
        private static readonly int MessageRange = 4;
        private static readonly int ChanceToShout = 101;
        #endregion

        public override bool IsInvulnerable { get { return true; } }
        public override bool IsActiveBuyer { get { return false; } }

        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }
        private DateTime m_NextMessage;

        public DateTime NextMessage
        {
            get { return m_NextMessage; }
            set { m_NextMessage = value; }
        }

        #region constructors
        [Constructable]
        public SantaClaus()
            : base( ", custode del Natale di Midgard" )
        {
            // Stats
            SetStr( 304, 400 );
            SetDex( 102, 150 );
            SetInt( 204, 300 );

            SetHits( 66, 125 );
            SetDamage( 30, 50 );

            // Resistances
            SetResistance( ResistanceType.Physical, 90, 100 );
            SetResistance( ResistanceType.Fire, 90, 100 );
            SetResistance( ResistanceType.Cold, 90, 100 );
            SetResistance( ResistanceType.Poison, 90, 100 );
            SetResistance( ResistanceType.Energy, 90, 100 );

            // Fama e Karma
            Fame = 10000;
            Karma = 10000;

            // Hair
            HairItemID = 0x203B;
            HairHue = Utility.RandomHairHue();

            // Speech & Skin hues
            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();

            // Genre
            Female = false;
            Name = "Santa Claus";
            Body = 0x190;

            CantWalk = true;
        }

        public SantaClaus( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region Init
        public override void InitSBInfo()
        {
            m_SBInfos.Add( new SBSantaClaus() );
        }

        public override void InitOutfit()
        {
            FacialHairItemID = 1599;
            FacialHairHue = 1154;

            HairItemID = 8252;
            HairHue = 1154;

            AddItem( new Cloak( 2444 ) );
            AddItem( new ShortPants( 2444 ) );
            AddItem( new FancyShirt( 1154 ) );
            AddItem( new Boots( 2444 ) );
            AddItem( new Christmas2008.CappellinoNatalizio2008( 0 ) );

            WildStaff staff = new WildStaff();
            staff.Name = "Santa Clauss staff";
            staff.Hue = 1154;
            AddItem( staff );


            Container pack = Backpack;
            if( pack == null )
            {
                pack = new Backpack();
                pack.Movable = false;
                AddItem( pack );
            }
            pack.Hue = 2444;
            pack.Name = "Gift's bag";
        }
        #endregion

        #region Speech
        public override void OnDoubleClick( Mobile m )
        {
            if( m.Alive && m is PlayerMobile )
                Shout();
        }

        public override bool HandlesOnSpeech( Mobile from )
        {
            return ( from.Alive && InRange( from, 12 ) );
        }

        public override void OnSpeech( SpeechEventArgs e )
        {
            if( !e.Handled && Utility.Random( 10 ) == 1 )
            {
                HandleShout( e.Mobile, e.Mobile.Location );
                e.Handled = true;
            }

            base.OnSpeech( e );
        }

        public override void OnMovement( Mobile m, Point3D oldLocation )
        {
            HandleShout( m, oldLocation );
        }

        public void HandleShout( Mobile listener, Point3D oldLocation )
        {
            if( Enabled )
            {
                if( Map != null && Map != Map.Internal )
                {
                    if( DateTime.Now >= NextMessage )
                    {
                        if( listener.Player && listener.Alive && !listener.Hidden )
                        {
                            if( InRange( listener, MessageRange ) && !InRange( oldLocation, MessageRange ) && InLOS( listener ) )
                            {
                                if( ChanceToShout > Utility.Random( 100 ) )
                                {
                                    Server.Spells.SpellHelper.Turn( this, listener );
                                    Shout();
                                    NextMessage = DateTime.Now + TimeSpan.FromSeconds( Utility.RandomMinMax( MinDelayMessage, MaxDelayMessage ) );
                                }
                            }
                        }
                    }
                }
            }
        }

        private static string[] m_Shouts = new string[]
		{
			"Ho ho ho! Buon Natale a tutti!",
			"Buon Natale!",
			"Un felice Natale su Midgard",
			"Hey, hai fatto il bravo quest'anno?",
			"Quest'anno niente scherzetti nel camino eh!?"
		};

        private void Shout()
        {
            if( Deleted || Map == Map.Internal )
                return;

            Say( true, m_Shouts[ Utility.Random( m_Shouts.Length ) ] );
        }
        #endregion

        #region other members
        public override bool CheckMidgardTown()
        {
            return false;
            // don't make any town cloth
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( "Buon Natale Midgard!" );
        }
        #endregion

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            if( CantWalk )
                Frozen = true;
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            if( CantWalk )
                Frozen = true;
        }
        #endregion
    }
}
