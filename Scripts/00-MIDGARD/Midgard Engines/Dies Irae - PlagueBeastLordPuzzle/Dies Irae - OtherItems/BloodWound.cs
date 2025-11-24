using System;
using Server;
using Server.Items;

namespace Midgard.Engines.PlagueBeastLordPuzzle
{
    public class BloodWound : Item
    {
        private DecoItem m_Bandage;
        private bool m_IsAided;
        private Timer m_BleedingTimer;

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public bool IsAided
        {
            get { return m_IsAided; }
            set
            {
                bool oldValue = m_IsAided;
                if( value != oldValue )
                {
                    m_IsAided = value;
                    OnIsAidedChanged( oldValue );
                }
            }
        }

        [CommandProperty( AccessLevel.Developer )]
        public BaseOrgan Organ { get; private set; }

        [CommandProperty( AccessLevel.Developer )]
        public Gland Gland { get; private set; }

        [CommandProperty( AccessLevel.Developer )]
        public bool IsBleeding
        {
            get { return m_BleedingTimer != null && m_BleedingTimer.Running; }
        }

        public override int LabelNumber { get { return 1066125; } } // blood
        public override bool DisplayWeight { get { return false; } }

        #region constructors
        [Constructable]
        public BloodWound( BaseOrgan organ, Gland gland )
            : base( 0x122C )
        {
            m_IsAided = false;
            Organ = organ;
            Gland = gland;

            Movable = false;
            Visible = false;
        }

        public BloodWound( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region members
        public virtual void OnIsAidedChanged( bool oldValue )
        {
            Console.WriteLine( "OnIsAidedChanged" );

            PuzzlePlagueBeastLord pbl = RootParent as PuzzlePlagueBeastLord;
            if( pbl != null && pbl.IsInDebugMode )
                pbl.Say( "OnIsAidedChanged oldValue: {0}", oldValue.ToString() );

            if( !oldValue )
            {
                m_Bandage.Visible = true;
                Gland.RevealGland( Gland );
            }
        }

        public void CreateBandage()
        {
            Container c = Parent as Container;

            m_Bandage = new DecoItem( 0x1914, 1066109, 0, false ); // bandage
            if( c != null )
                c.AddItem( m_Bandage );
            m_Bandage.Location = new Point3D( X + 9, Y, 0 );
        }

        public void DoBleed( Mobile from )
        {
            if( !Visible && m_BleedingTimer == null )
            {
                Visible = true;

                Organ.SendLocalizedMessageTo( from, 1066110 ); // * The organ starts bleeding *

                PuzzlePlagueBeastLord pbl = RootParent as PuzzlePlagueBeastLord;
                if( pbl == null || pbl.Deleted )
                    return;

                TimeSpan delay = pbl.IsInDebugMode ? TimeSpan.FromSeconds( 10 ) : TimeSpan.FromSeconds( 1 );
                m_BleedingTimer = Timer.DelayCall( delay, delay, new TimerCallback( OnBleedTimerTick ) );
            }
        }

        public void DoAid( Mobile from )
        {
            IsAided = true;

            if( Organ != null && !Organ.Deleted )
                Organ.SendLocalizedMessageTo( from, 1066111 ); // * You bandage the blood *
        }

        public void OnBleedTimerTick()
        {
            PuzzlePlagueBeastLord pbl = RootParent as PuzzlePlagueBeastLord;
            if( Deleted || !Visible || m_IsAided || pbl == null || !pbl.Alive )
                m_BleedingTimer.Stop();
            else
            {
                switch( ItemID )
                {
                    case 0x122C:
                        {
                            ItemID = 4654;
                            X += 5;
                            break;
                        }
                    case 4654:
                        ItemID = 4653;
                        break;
                    case 4653:
                        ItemID = 4650;
                        break;
                    default:
                        {
                            m_BleedingTimer.Stop();
                            pbl.Kill();
                            break;
                        }
                }
            }
        }
        #endregion

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( m_IsAided );
            writer.Write( m_Bandage );
            writer.Write( Organ );
            writer.Write( Gland );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_IsAided = reader.ReadBool();
            m_Bandage = (DecoItem)reader.ReadItem();
            Organ = (BaseOrgan)reader.ReadItem();
            Gland = (Gland)reader.ReadItem();

            if( m_Bandage == null )
                Delete();
        }
        #endregion
    }
}