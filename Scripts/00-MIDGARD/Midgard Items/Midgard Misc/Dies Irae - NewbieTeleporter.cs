/***************************************************************************
 *                               Dies Irae - NewbieTeleporter.cs
 *
 *   begin                : 07 November, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class NewbieTeleporter : Teleporter
    {
        private static readonly int DefaultMaxSumSkills = 400;
        private static readonly int DefaultMaxSumStats = 400;

        private int m_MaxSumSkills;
        private int m_MaxSumStats;

        [CommandProperty( AccessLevel.GameMaster )]
        public int MaxSumSkills { get { return m_MaxSumSkills; } set { m_MaxSumSkills = value; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public int MaxSumStats { get { return m_MaxSumStats; } set { m_MaxSumStats = value; } }

        private void EndMessageLock( object state )
        {
            ( (Mobile)state ).EndAction( this );
        }

        public override bool OnMoveOver( Mobile m )
        {
            if( Active )
            {
                if( !Creatures && !m.Player )
                    return true;

                if( m.AccessLevel == AccessLevel.Player && ( m.RawStatTotal > m_MaxSumStats || m.SkillsTotal > m_MaxSumSkills ) )
                {
                    if( m.BeginAction( this ) )
                    {
                        m.SendMessage( 0x22, "You cannot pass. Only inexperienced players can do." );

                        Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerStateCallback( EndMessageLock ), m );
                    }

                    return false;
                }

                StartTeleport( m );
                return false;
            }

            return true;
        }

        public override string DefaultName { get { return "Midgard Newbie Teleporter"; } }

        public override bool DisplayWeight { get { return false; } }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( "Newbie limits: {0} sum of skills - {1} sum of stats", m_MaxSumSkills, m_MaxSumStats );
        }

        [Constructable]
        public NewbieTeleporter()
        {
            m_MaxSumSkills = DefaultMaxSumSkills;
            m_MaxSumStats = DefaultMaxSumStats;
        }

        public NewbieTeleporter( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
            writer.Write( m_MaxSumSkills );
            writer.Write( m_MaxSumStats );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        m_MaxSumSkills = reader.ReadInt();
                        m_MaxSumStats = reader.ReadInt();
                        break;
                    }
            }
        }
    }
}