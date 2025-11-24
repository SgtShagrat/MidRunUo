using Server;
using Server.Mobiles;

namespace Midgard.Items
{
    public class KarmaLocker : Item
    {
        public enum ShrineVirtueNames
        {
            Compassion,
            Honesty,
            Honor,
            Humility,
            Justice,
            Sacrifice,
            Spirituality,
            Valor,
            Chaos
        }

        private readonly string[] m_Mantra = new string[]
                                             {
                                                 "mu",      // Compassion
                                                 "ahm",     // Honesty
                                                 "summ",    // Honor
                                                 "lum",     // Humility
                                                 "beh",     // Justice
                                                 "cah",     // Sacrifice
                                                 "om",      // Spirituality
                                                 "ra",      // Valor
                                                 "bal"      // Chaos
                                             };

        [Constructable]
        public KarmaLocker()
            : base( 0x1BC3 )
        {
            Range = 3;
            Visible = false;
            Movable = false;
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public ShrineVirtueNames ShrineVirtue { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public string Mantra
        {
            get { return m_Mantra[ (int)ShrineVirtue ]; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Range { get; set; }

        public override bool HandlesOnSpeech
        {
            get { return true; }
        }

        public override void OnSpeech( SpeechEventArgs e )
        {
            if( e.Handled )
            {
                return;
            }

            var m = e.Mobile as PlayerMobile;
            if( m == null || !m.InRange( GetWorldLocation(), Range ) )
            {
                return;
            }

            if( !Insensitive.Contains( e.Speech, Mantra ) )
            {
                return;
            }

            e.Handled = true;

            if( ShrineVirtue == ShrineVirtueNames.Chaos && !m.KarmaLocked )
            {
                m.KarmaLocked = true;
                m.SendLocalizedMessage( 1060192 ); // Your karma has been locked. Your karma can no longer be raised.
            }
            else if( ShrineVirtue != ShrineVirtueNames.Chaos && m.KarmaLocked )
            {
                m.KarmaLocked = false;
                m.SendLocalizedMessage( 1060191 ); // Your karma has been unlocked. Your karma can be raised again.
            }
        }

        #region serialization
        public KarmaLocker( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( (int)ShrineVirtue );
            writer.Write( Range );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        ShrineVirtue = (ShrineVirtueNames)reader.ReadInt();
                        Range = reader.ReadInt();
                        break;
                    }
            }

            if( Visible )
                Visible = false;
            if( Movable )
                Movable = false;
        }
        #endregion
    }
}