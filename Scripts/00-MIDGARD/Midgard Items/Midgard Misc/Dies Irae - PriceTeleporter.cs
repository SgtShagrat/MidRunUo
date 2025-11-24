/***************************************************************************
 *                               PriceTeleporter.cs
 *
 *   begin                : 07 November, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Midgard.Gumps;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class PriceTeleporter : Teleporter
    {
        private static readonly int DefaultPrice = 500;
        private static readonly string Defaultmessage = "Will you travel for {0} golds?";

        public override bool OnMoveOver( Mobile m )
        {
            if( Active )
            {
                if( !Creatures && !m.Player )
                    return true;

                if( !m.Alive )
                    return true;

                if( m.BeginAction( this ) )
                {
                    StartTeleport( m );
                    Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerStateCallback( EndActionLock ), m );
                }
            }

            return true;
        }

        private void EndActionLock( object state )
        {
            ( (Mobile)state ).EndAction( this );
        }

        public virtual void OnTraveConfirmed( Mobile from, bool okay, object state )
        {
            if( !okay )
            {
                from.SendMessage( "You decided to rest." );
                return;
            }

            Container cont = from.Backpack;
            bool payed = false;

            if( cont.ConsumeTotal( typeof( Gold ), Price ) )
                payed = true;

            if( !payed )
            {
                cont = from.FindBankNoCreate();
                if( cont != null && cont.ConsumeTotal( typeof( Gold ), Price ) )
                    payed = true;
            }

            if( payed )
            {
                if( from.InRange( Location, 5 ) )
                {
                    from.SendMessage( "You payed the price and your travel begins..." );
                    DoTeleport( from );
                }
                else
                {
                    from.SendMessage( "You are too far away to start the travel! Your offer is lost!" );
                }
            }
            else
            {
                from.SendMessage( "Begging thy pardon, but thy bank account lacks these funds." );
            }
        }

        public override string DefaultName { get { return "Midgard Price Teleporter"; } }

        public override bool DisplayWeight { get { return false; } }

        public override void StartTeleport( Mobile m )
        {
            m.SendGump( new SmallConfirmGump( string.Format( Message, Price ), OnTraveConfirmed ) );
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Price { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public string Message { get; set; }

        [Constructable]
        public PriceTeleporter()
        {
            Price = DefaultPrice;
            Message = Defaultmessage;
        }

        public PriceTeleporter( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( Price );
            writer.Write( Message );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        Price = reader.ReadInt();
                        Message = reader.ReadString();
                        break;
                    }
            }
        }
    }
}