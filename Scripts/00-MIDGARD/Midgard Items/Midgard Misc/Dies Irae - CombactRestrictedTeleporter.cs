/***************************************************************************
 *                               Dies Irae - CombactRestrictedTeleporter.cs
 *
 *   begin                : 25 aprile 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using Server;
using Server.Items;
using Server.Spells;

namespace Midgard.Items
{
    public class NotorietyTeleporter : Teleporter
    {
        private static readonly string DefaultNoCombatantsMessageString = "Wouldst thou flee during the heat of battle??";
        private static readonly int DefaultNoCombatantsMessageLocalized = 1005561;

        private static readonly string DefaultNoCriminalsMessageString = "Thou'rt a criminal and cannot escape so easily.";
        private static readonly int DefaultNoCriminalsMessageLocalized = 1005564;

        private string m_NoCombatantsMessageString;
        private int m_NoCombatantsMessageNumber;

        private string m_NoCriminalsMessageString;
        private int m_NoCriminalsMessageNumber;

        private bool m_AllowCriminals;
        private bool m_AllowCombatants;

        [CommandProperty( AccessLevel.GameMaster )]
        public bool AllowCriminals
        {
            get { return m_AllowCriminals; }
            set { m_AllowCriminals = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool AllowCombatants
        {
            get { return m_AllowCombatants; }
            set { m_AllowCombatants = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public string NoCombatantsMessageString
        {
            get { return m_NoCombatantsMessageString; }
            set { m_NoCombatantsMessageString = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int NoCombatantsMessageNumber
        {
            get { return m_NoCombatantsMessageNumber; }
            set { m_NoCombatantsMessageNumber = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public string NoCriminalsMessageString
        {
            get { return m_NoCriminalsMessageString; }
            set { m_NoCriminalsMessageString = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int NoCriminalsMessageNumber
        {
            get { return m_NoCriminalsMessageNumber; }
            set { m_NoCriminalsMessageNumber = value; InvalidateProperties(); }
        }

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

                if( !m_AllowCriminals && m.Criminal )
                {
                    if( m.BeginAction( this ) )
                    {
                        if( m_NoCriminalsMessageNumber != 0 )
                            m.SendLocalizedMessage( m_NoCriminalsMessageNumber, "", 0x22 );
                        else if( !String.IsNullOrEmpty( m_NoCriminalsMessageString ) )
                            m.SendMessage( 0x22, m_NoCriminalsMessageString );

                        Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerStateCallback( EndMessageLock ), m );
                    }

                    return false;
                }

                if( !m_AllowCombatants && SpellHelper.CheckCombat( m ) )
                {
                    if( m.BeginAction( this ) )
                    {
                        if( m_NoCombatantsMessageNumber != 0 )
                            m.SendLocalizedMessage( m_NoCombatantsMessageNumber, "", 0x22 );
                        else if( !String.IsNullOrEmpty( m_NoCombatantsMessageString ) )
                            m.SendMessage( 0x22, m_NoCombatantsMessageString );

                        Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerStateCallback( EndMessageLock ), m );
                    }

                    return false;
                }

                StartTeleport( m );
                return false;
            }

            return true;
        }

        public override string DefaultName
        {
            get { return "Midgard Notoriety Teleporter"; }
        }

        public override bool DisplayWeight
        {
            get { return false; }
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            list.Add( "Criminals teleport: {0}", ( m_AllowCriminals ? "allowed" : "not allowed" ) );
            list.Add( "Combatants teleport: {0}", ( m_AllowCombatants ? "allowed" : "not allowed" ) );
        }

        [Constructable]
        public NotorietyTeleporter()
        {
            m_AllowCriminals = false;
            m_AllowCombatants = false;

            m_NoCombatantsMessageNumber = DefaultNoCombatantsMessageLocalized;
            m_NoCombatantsMessageString = DefaultNoCombatantsMessageString;

            m_NoCriminalsMessageNumber = DefaultNoCriminalsMessageLocalized;
            m_NoCriminalsMessageString = DefaultNoCriminalsMessageString;
        }

        public NotorietyTeleporter( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( m_AllowCriminals );
            writer.Write( m_AllowCombatants );

            writer.Write( m_NoCombatantsMessageString );
            writer.Write( m_NoCombatantsMessageNumber );

            writer.Write( m_NoCriminalsMessageString );
            writer.Write( m_NoCriminalsMessageNumber );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        m_AllowCriminals = reader.ReadBool();
                        m_AllowCombatants = reader.ReadBool();

                        m_NoCombatantsMessageString = reader.ReadString();
                        m_NoCombatantsMessageNumber = reader.ReadInt();

                        m_NoCriminalsMessageString = reader.ReadString();
                        m_NoCriminalsMessageNumber = reader.ReadInt();

                        break;
                    }
            }
        }
    }
}