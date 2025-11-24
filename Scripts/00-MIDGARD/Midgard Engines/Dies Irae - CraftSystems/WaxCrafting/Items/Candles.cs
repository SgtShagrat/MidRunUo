using System;

using Server;
using Server.Items;

namespace Midgard.Items
{
    public class CandleLongColor : BaseLight
    {
        public override int LabelNumber { get { return 1065250; } } 		// long candle
        public override int LitItemID { get { return 0x1430; } }
        public override int UnlitItemID { get { return 0x1433; } }

        [Constructable]
        public CandleLongColor()
            : base( 0x1433 )
        {
            if( Burnout )
                Duration = TimeSpan.FromMinutes( 30 );
            else
                Duration = TimeSpan.Zero;

            Burning = false;
            Light = LightType.Circle150;
            Weight = 1.0;
            Hue = GetRandomHue();
        }

        protected static int GetRandomHue()
        {
            switch( Utility.Random( 5 ) )
            {
                default:
                case 0: return Utility.RandomBlueHue();
                case 1: return Utility.RandomNeutralHue();
                case 2: return Utility.RandomGreenHue();
                case 3: return Utility.RandomYellowHue();
                case 4: return Utility.RandomRedHue();
            }
        }

        public CandleLongColor( Serial serial )
            : base( serial )
        {
        }

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
    }

    public class CandleOfLove : BaseLight
    {
        //		public override int LabelNumber{ get{ return 1065251; } } 		// Candle Of Love
        public override int LitItemID { get { return 0x1C14; } }
        public override int UnlitItemID { get { return 0x1C16; } }

        [Constructable]
        public CandleOfLove()
            : base( 0x1C16 )
        {
            Name = "Copy of a Candle of Love";
            if( Burnout )
                Duration = TimeSpan.FromMinutes( 25 );
            else
                Duration = TimeSpan.Zero;

            Burning = false;
            Light = LightType.Circle150;
            Weight = 1.0;
        }

        public CandleOfLove( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();

            if( String.IsNullOrEmpty( Name ) )
                Name = "Copy of a Candle of Love";
        }
    }

    public class CandleShortColor : BaseLight
    {
        public override int LabelNumber { get { return 1065252; } } 		// short candle
        public override int LitItemID { get { return 0x142C; } }
        public override int UnlitItemID { get { return 0x142F; } }

        [Constructable]
        public CandleShortColor()
            : base( 0x142F )
        {
            if( Burnout )
                Duration = TimeSpan.FromMinutes( 25 );
            else
                Duration = TimeSpan.Zero;

            Burning = false;
            Light = LightType.Circle150;
            Weight = 1.0;
            Hue = GetRandomHue();
        }

        protected static int GetRandomHue()
        {
            switch( Utility.Random( 5 ) )
            {
                default:
                case 0: return Utility.RandomBlueHue();
                case 1: return Utility.RandomNeutralHue();
                case 2: return Utility.RandomGreenHue();
                case 3: return Utility.RandomYellowHue();
                case 4: return Utility.RandomRedHue();
            }
        }

        public CandleShortColor( Serial serial )
            : base( serial )
        {
        }

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
    }
}