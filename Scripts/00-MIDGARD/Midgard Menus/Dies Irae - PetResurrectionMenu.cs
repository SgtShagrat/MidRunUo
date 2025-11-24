using System;
using System.IO;

using Server;
using Server.Menus.Questions;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Menus
{
    public class PetResurrectionMenu : QuestionMenu
    {
        private readonly BaseCreature m_Pet;
        private readonly double m_HitsScalar;
        private readonly Item m_ObjectFrom;

        public PetResurrectionMenu( BaseCreature pet )
            : this( pet, 0.0 )
        {
        }

        public PetResurrectionMenu( BaseCreature pet, double hitsScalar )
            : this( pet, hitsScalar, null )
        {
        }

        public PetResurrectionMenu( BaseCreature pet, double hitsScalar, Item objectFrom )
            : base( string.Concat( "Wilt thou sanctify the resurrection of: ", pet.Name ), new string[] { "Yes", "No" } )
        {
            m_Pet = pet;
            m_HitsScalar = hitsScalar;
            m_ObjectFrom = objectFrom;
        }

        public override void OnCancel( NetState state )
        {
        }

        public override void OnResponse( NetState state, int index )
        {
            if( m_Pet.Deleted || !m_Pet.IsBonded || !m_Pet.IsDeadPet )
                return;

            Mobile from = state.Mobile;

            if( !m_Pet.Deleted && m_Pet.IsDeadPet && !m_Pet.IsBonded )
            {
                try
                {
                    state.Mobile.SendMessage( "Warning il pet e' morto ma non bondato. Contatta SUBITO dies irae e descrivi la situazione accuratamente." );

                    using( StreamWriter op = new StreamWriter( "Logs/pet-resurrection-errors.log", true ) )
                    {
                        op.WriteLine( "{0}\t{1}", DateTime.Now, string.Format( "pet type {0}, name {1}, serial {2}, is dead but not bonded.",
                            m_Pet.GetType().Name, string.IsNullOrEmpty( m_Pet.Name ) ? "null" : m_Pet.Name, m_Pet.Serial ) );
                        op.WriteLine();
                    }
                }
                catch
                {
                }
            }

            if( index != 0 )
                return;

            if( m_Pet.Map == null || !m_Pet.Map.CanFit( m_Pet.Location, 16, false, false ) )
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

            for( int i = 0; i < m_Pet.Skills.Length; ++i )	//Decrease all skills on pet.
                m_Pet.Skills[ i ].Base -= decreaseAmount;

            if( !m_Pet.IsDeadPet && m_HitsScalar > 0 )
                m_Pet.Hits = (int)( m_Pet.HitsMax * m_HitsScalar );

            if( m_ObjectFrom != null && !m_ObjectFrom.Deleted )
                m_ObjectFrom.Consume();
        }
    }
}