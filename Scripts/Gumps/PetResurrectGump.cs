using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;
using System.IO;

namespace Server.Gumps
{
	public class PetResurrectGump : Gump
	{
		private BaseCreature m_Pet;
		private double m_HitsScalar;

		public PetResurrectGump( Mobile from, BaseCreature pet )
			: this( from, pet, 0.0 )
		{
		}

		public PetResurrectGump( Mobile from, BaseCreature pet, double hitsScalar ) : base( 50, 50 )
		{
			from.CloseGump( typeof( PetResurrectGump ) );

			m_Pet = pet;
			m_HitsScalar = hitsScalar;

			AddPage( 0 );

			AddBackground( 10, 10, 265, 140, 0x242C );

			AddItem( 205, 40, 0x4 );
			AddItem( 227, 40, 0x5 );

			AddItem( 180, 78, 0xCAE );
			AddItem( 195, 90, 0xCAD );
			AddItem( 218, 95, 0xCB0 );

			AddHtmlLocalized( 30, 30, 150, 75, 1049665, false, false ); // <div align=center>Wilt thou sanctify the resurrection of:</div>
			AddHtml( 30, 70, 150, 25, String.Format( "<div align=CENTER>{0}</div>", pet.Name ), true, false );

			AddButton( 40, 105, 0x81A, 0x81B, 0x1, GumpButtonType.Reply, 0 ); // Okay
			AddButton( 110, 105, 0x819, 0x818, 0x2, GumpButtonType.Reply, 0 ); // Cancel
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			if ( m_Pet.Deleted || !m_Pet.IsBonded || !m_Pet.IsDeadPet )
				return;

            #region mod by Dies Irae
            if( !m_Pet.Deleted && m_Pet.IsDeadPet && !m_Pet.IsBonded )
            {
                try
                {
                    state.Mobile.SendMessage( "Warning il pet e' morto ma non bondato. Contatta SUBITO dies irae e descrivi la situazione accuratamente." );    
                
                    using( StreamWriter op = new StreamWriter( "Logs/pet-resurrection-errors.log", true ) )
                    {
                        op.WriteLine( "{0}\t{1}", DateTime.Now, string.Format( "pet type {0}, name {1}, serial {2}, is dead but not bonded.",
                            m_Pet.GetType().Name, string.IsNullOrEmpty( m_Pet.Name ) ? "null" : m_Pet.Name, m_Pet.Serial.ToString() ) );
                        op.WriteLine();
                    }
                }
                catch
                {
                }
            }
            #endregion

			Mobile from = state.Mobile;

			if ( info.ButtonID == 1 )
			{
				if ( m_Pet.Map == null || !m_Pet.Map.CanFit( m_Pet.Location, 16, false, false ) )
				{
					from.SendLocalizedMessage( 503256 ); // You fail to resurrect the creature.
					return;
				}
				else if( m_Pet.Region != null && m_Pet.Region.IsPartOf( "Khaldun" ) )	//TODO: Confirm for pets, as per Bandage's script.
				{
					from.SendLocalizedMessage( 1010395 ); // The veil of death in this area is too strong and resists thy efforts to restore life.
					return;
				}

				m_Pet.PlaySound( 0x214 );
				m_Pet.FixedEffect( 0x376A, 10, 16 );
				m_Pet.ResurrectPet();

				double decreaseAmount;

				if( from == m_Pet.ControlMaster )
					decreaseAmount = 0.1;
				else
					decreaseAmount = 0.2;

				for ( int i = 0; i < m_Pet.Skills.Length; ++i )	//Decrease all skills on pet.
					m_Pet.Skills[i].Base -= decreaseAmount;

				if( !m_Pet.IsDeadPet && m_HitsScalar > 0 )
					m_Pet.Hits = (int)(m_Pet.HitsMax * m_HitsScalar);
			}

		}
	}
}