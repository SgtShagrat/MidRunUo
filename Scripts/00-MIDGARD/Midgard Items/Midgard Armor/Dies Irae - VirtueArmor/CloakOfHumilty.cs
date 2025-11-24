/***************************************************************************
 *                                   CloakOfHumilty.cs
 *                            		---------------------
 *  begin               	: Dicembre, 2006
 * 	version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 * 			Armatura per la fazione good.
 * 			Mantello.
 * 
 * 	
 * 
 ***************************************************************************/

/*
using Server.Factions;

namespace Server.Items
{
    [FlipableAttribute( 0x2B04, 0x2B05 )]
    public class LegionaryCloakOfHumilty : BaseCloak
    {
        #region proprietà
        public override int LabelNumber { get { return 1064315; } }
        #endregion

        #region costruttori
        [Constructable]
        public LegionaryCloakOfHumilty()
            : this( 0 )
        {
        }

        [Constructable]
        public LegionaryCloakOfHumilty( int hue )
            : base( 0x2B04, hue )
        {
            Weight = 4.0;
            //			Name = "Cloak Of Humilty";
        }

        public LegionaryCloakOfHumilty( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override bool OnEquip( Mobile from )
        {
            return Validate( from ) && base.OnEquip( from );
        }

        public bool Validate( Mobile m )
        {
            if( !Core.AOS || m == null || !m.Player )
                return true;

            Faction f = Faction.Find( m );
            if( f is LegioImperialis )
            {
                return true;
            }
            else
            {
                m.SendLocalizedMessage( 1064336 ); // Only an good Legionary can wear this!
                return false;
            }
        }
        #endregion

        #region serialize-deserialize
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
        #endregion
    }
}
*/