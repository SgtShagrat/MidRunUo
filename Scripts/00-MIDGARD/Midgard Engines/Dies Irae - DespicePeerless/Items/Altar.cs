/***************************************************************************
 *                                  DespiceAltar.cs
 *                            		---------------
 *  begin                	: Settembre, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using Server.Mobiles;

namespace Server.Items
{
    public class DespiceAltar : PeerlessAltar
    {
        public override int KeyCount { get { return 1; } }
        public override MasterKey MasterKey { get { return new BarracoonFavouriteCheese(); } }
        public override TimeSpan TimeToSlay { get { return TimeSpan.FromMinutes( 60.0 ); } }
        public override string DefaultName { get { return "An offering altar to the Rat Lord"; } }

        public override Type[] Keys
        {
            get
            {
                return new Type[]
		        {
			        typeof( FluteOfRatmen ), typeof( RatsRattle ), typeof( IdolOfTheRats )
		        };
            }
        }

        public override BasePeerless Boss { get { return new BarracoonPeerless(); } }

        private DateTime m_NextChallenge;

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime NextChallenge
        {
            get { return m_NextChallenge; }
            set { m_NextChallenge = value; }
        }

        [Constructable]
        public DespiceAltar()
            : base( 0x990 )
        {
            BossLocation = new Point3D( 5614, 791, 60 );
            TeleportDest = new Point3D( 5558, 826, 80 );
            ExitDest = new Point3D( 5482, 825, 60 );
        }

        public DespiceAltar( Serial serial )
            : base( serial )
        {
        }

        public override bool OnDragDrop( Mobile from, Item dropped )
        {
            if( DateTime.Now < m_NextChallenge )
            {
                from.SendMessage( "The master of this realm has been slayed.  Your opportunity will come in {0} hours.",
                                 ( m_NextChallenge - DateTime.Now ).TotalHours.ToString( "F0" ) );
                return false;
            }

            return base.OnDragDrop( from, dropped );
        }

        public override void FinishSequence()
        {
            if( !World.Loading )
                m_NextChallenge = DateTime.Now + TimeSpan.FromHours( 12.0 );

            base.FinishSequence();
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)1 ); // version

            writer.Write( m_NextChallenge );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    m_NextChallenge = reader.ReadDateTime();
                    goto case 0;

                case 0:
                    break;
            }
        }
    }
}
