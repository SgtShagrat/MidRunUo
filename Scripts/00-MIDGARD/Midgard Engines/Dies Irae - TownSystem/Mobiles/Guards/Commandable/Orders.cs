using System.Collections.Generic;
using Server;

namespace Midgard.Engines.MidgardTownSystem
{
    public enum ReactionType
    {
        Ignore,
        Warn,
        Attack
    }

    public enum MovementType
    {
        Stand,
        Patrol,
        Follow
    }

    public class Reaction
    {
        private ReactionType m_Type;

        public TownSystem System { get; private set; }
        public ReactionType Type { get { return m_Type; } set { m_Type = value; } }

        public Reaction( TownSystem system, ReactionType type )
        {
            System = system;
            m_Type = type;
        }

        public Reaction( GenericReader reader )
        {
            int version = reader.ReadEncodedInt();

            switch( version )
            {
                case 0:
                    {
                        System = TownSystem.ReadReference( reader );
                        m_Type = (ReactionType)reader.ReadEncodedInt();

                        break;
                    }
            }
        }

        public void Serialize( GenericWriter writer )
        {
            writer.WriteEncodedInt( (int)0 ); // version

            TownSystem.WriteReference( writer, System );
            writer.WriteEncodedInt( (int)m_Type );
        }
    }

    public class Orders
    {
        private List<Reaction> m_Reactions;

        public BaseTownGuard Guard { get; private set; }

        public MovementType Movement { get; set; }
        public Mobile Follow { get; set; }

        public Reaction GetReaction( TownSystem system )
        {
            Reaction reaction;

            for( int i = 0; i < m_Reactions.Count; ++i )
            {
                reaction = m_Reactions[ i ];

                if( reaction.System == system )
                    return reaction;
            }

            reaction = new Reaction( system, ( system == null || system == Guard.System ) ? ReactionType.Ignore : ReactionType.Attack );
            m_Reactions.Add( reaction );

            return reaction;
        }

        public void SetReaction( TownSystem system, ReactionType type )
        {
            Reaction reaction = GetReaction( system );

            reaction.Type = type;
        }

        public Orders( BaseTownGuard guard )
        {
            Guard = guard;
            m_Reactions = new List<Reaction>();
            Movement = MovementType.Patrol;
        }

        public Orders( BaseTownGuard guard, GenericReader reader )
        {
            Guard = guard;

            int version = reader.ReadEncodedInt();

            switch( version )
            {
                case 1:
                    {
                        Follow = reader.ReadMobile();
                        goto case 0;
                    }
                case 0:
                    {
                        int count = reader.ReadEncodedInt();
                        m_Reactions = new List<Reaction>( count );

                        for( int i = 0; i < count; ++i )
                            m_Reactions.Add( new Reaction( reader ) );

                        Movement = (MovementType)reader.ReadEncodedInt();

                        break;
                    }
            }
        }

        public void Serialize( GenericWriter writer )
        {
            writer.WriteEncodedInt( (int)1 ); // version

            writer.Write( (Mobile)Follow );

            writer.WriteEncodedInt( (int)m_Reactions.Count );

            for( int i = 0; i < m_Reactions.Count; ++i )
                m_Reactions[ i ].Serialize( writer );

            writer.WriteEncodedInt( (int)Movement );
        }
    }
}