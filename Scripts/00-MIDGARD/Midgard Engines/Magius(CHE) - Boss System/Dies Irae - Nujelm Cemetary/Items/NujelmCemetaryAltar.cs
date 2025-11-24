/***************************************************************************
 *                               NujelmCemetaryAltar.cs
 *                            ----------------------------
 *   begin                : 06 September, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Engines.BossSystem.NujelmCemetary
{
    public class NujelmCemetaryAltar : PeerlessAltar
    {
        public override int KeyCount { get { return 1; } }
        public override MasterKey MasterKey { get { return new GoldenScrabble(); } }
        public override TimeSpan TimeToSlay { get { return TimeSpan.FromMinutes( 60.0 ); } }
        public override string DefaultName { get { return "An offering altar to Kalish'ten"; } }

        public override Type[] Keys
        {
            get
            {
                return new Type[]
                {
                    typeof( BloodyBandageKey )
                };
            }
        }

        public override BasePeerless Boss { get { return new KalishtenPeerless(); } }

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime NextChallenge { get; set; }

        [Constructable]
        public NujelmCemetaryAltar()
            : base( 0x990 )
        {
            BossLocation = Config.BossLocation;
            TeleportDest = Config.TeleportDest;
            ExitDest = Config.ExitDest;
        }

        public override bool OnDragDrop( Mobile from, Item dropped )
        {
            if( DateTime.Now < NextChallenge )
            {
                from.SendMessage( "The master of this realm has been slayed.  Your opportunity will come in {0} hours.",
                ( NextChallenge - DateTime.Now ).TotalHours.ToString( "F0" ) );
                return false;
            }

            return base.OnDragDrop( from, dropped );
        }

        public override void FinishSequence()
        {
            if( !World.Loading )
                NextChallenge = DateTime.Now + TimeSpan.FromHours( 12.0 );

            base.FinishSequence();
        }

        #region serialization
        public NujelmCemetaryAltar( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)1 ); // version

            writer.Write( NextChallenge );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    NextChallenge = reader.ReadDateTime();
                    goto case 0;

                case 0:
                    break;
            }
        }
        #endregion
    }
}
