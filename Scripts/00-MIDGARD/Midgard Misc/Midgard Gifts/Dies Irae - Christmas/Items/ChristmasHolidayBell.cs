using System;

using Server;
using Server.Network;

namespace Midgard.Engines.Events.Items
{
    public class ChristmasHolidayBell : Item, IEventItem
    {
        #region IEventItem
        [CommandProperty( AccessLevel.GameMaster )]
        public int Year { get; set; }

        public EventType Event
        {
            get { return EventType.Christmas; }
        }
        #endregion

        public enum ChristmasHolidayBellSound
        {
            Sound1 = 0,
            Sound2,
            Sound3,
            Sound4,
            Sound5,
            Sound6
        }

        private ChristmasHelper.MidgardStaff m_Type;

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public ChristmasHelper.MidgardStaff Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public ChristmasHolidayBellSound Sound { get; set; }

        [Constructable]
        public ChristmasHolidayBell()
            : base( 0x1C12 )
        {
            m_Type = ChristmasHelper.GetRandomStaffMember();

            Sound = (ChristmasHolidayBellSound)Utility.Random( Enum.GetNames( typeof( ChristmasHolidayBellSound ) ).Length );

            Hue = Utility.RandomList( 1150, 55, 65, 75, 85, 95, 105, 115, 125, 135, 145, 30, 35, 37 );
            LootType = LootType.Blessed;

            Name = "Midgard Christmas Bell";

            Year = ChristmasHelper.GetCurrentYear;
        }

        public ChristmasHolidayBell( Serial serial )
            : base( serial )
        {
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( Utility.InsensitiveCompare( Name, "Midgard Holiday Bell" ) == 0 )
            {
                string name = ChristmasHelper.GetStaffMember( m_Type );

                if( !String.IsNullOrEmpty( name ) )
                {
                    from.SendMessage( "Hey, you notice some words on the bell! " );
                    from.PlaySound( 0x1FA );
                    Effects.SendLocationEffect( this, Map, 14201, 16 );
                    Name = String.Format( "an holiday bell gifted by {0}", name );
                    InvalidateProperties();
                }
                return;
            }

            int sound;

            switch( Sound )
            {
                case ( ChristmasHolidayBellSound.Sound1 ):
                    sound = 0x100;
                    break;
                case ( ChristmasHolidayBellSound.Sound2 ):
                    sound = 0x101;
                    break;
                case ( ChristmasHolidayBellSound.Sound3 ):
                    sound = 0x103;
                    break;
                case ( ChristmasHolidayBellSound.Sound4 ):
                    sound = 0x104;
                    break;
                case ( ChristmasHolidayBellSound.Sound5 ):
                    sound = 0x16;
                    break;
                case ( ChristmasHolidayBellSound.Sound6 ):
                    sound = 0x428;
                    break;
                default:
                    sound = 0x100;
                    break;
            }

            from.PlaySound( sound );
            PublicOverheadMessage( MessageType.Regular, Utility.RandomRedHue(), true, ChristmasHelper.GetMidgardGreetings() );

            base.OnDoubleClick( from );
        }

        public override bool DisplayLootType { get { return false; } }
        public override bool DisplayWeight { get { return false; } }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.WriteEncodedInt( (int)m_Type );
            writer.WriteEncodedInt( (int)Sound );

            writer.WriteEncodedInt( Year );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_Type = (ChristmasHelper.MidgardStaff)reader.ReadEncodedInt();
            Sound = (ChristmasHolidayBellSound)reader.ReadEncodedInt();

            Year = reader.ReadEncodedInt();

            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( ChristmasHelper.VerifyEndPeriod_Callback ), this );
        }
        #endregion
    }
}