/***************************************************************************
 *                                    QuiverOfInfinity.cs
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
 * 			Faretra.
 * 
 * 	
 * 
 ***************************************************************************/

/*
using Server.Factions;

namespace Server.Items
{
    [Flipable( 0x2B02, 0x2B03 )]
    public class LegionaryQuiverOfInfinity : BaseQuiver
    {
        #region proprietà
        public override int LabelNumber { get { return 1064312; } }
        #endregion

        #region costruttori
        [Constructable]
        public LegionaryQuiverOfInfinity()
            : base()
        {
        }

        public LegionaryQuiverOfInfinity( Serial serial )
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
            if( m == null || !m.Player )
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

        #region serial-deserial
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