using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class PianoAddon : BaseAddon
    {
        [Constructable]
        public PianoAddon()
        {
            AddonComponent ac = null;

            ac = new AddonComponent( 2928 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, -1, 1, 2 );

            ac = new AddonComponent( 5981 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, -1, 1, 6 );

            ac = new AddonComponent( 5984 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, -1, 1, 8 );

            ac = new AddonComponent( 5981 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, -1, 1, 7 );

            ac = new AddonComponent( 5985 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, -1, 1, 9 );

            ac = new AddonComponent( 5431 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, -1, 1, 10 );

            ac = new AddonComponent( 7933 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, -1, 1, 7 );

            ac = new AddonComponent( 2480 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, -1, 1, 11 );

            ac = new AddonComponent( 7883 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, -1, 0, 1 );

            ac = new AddonComponent( 2480 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, -1, -1, 2 );

            ac = new AddonComponent( 2924 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, 0, -1, 0 );

            ac = new AddonComponent( 2925 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, 0, 0, 0 );

            ac = new AddonComponent( 4006 );
            ac.Name = "Piano Keys";
            AddComponent( ac, 0, 0, 7 );

            ac = new AddonComponent( 5981 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, 0, 0, 10 );

            ac = new AddonComponent( 7933 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, 0, 0, 9 );

            ac = new AddonComponent( 5991 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, 0, 0, 9 );

            ac = new AddonComponent( 5988 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, 0, 0, 10 );

            ac = new AddonComponent( 5987 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, 0, 0, 8 );

            ac = new AddonComponent( 5988 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, 0, 0, 9 );

            ac = new AddonComponent( 2252 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, 0, 0, 11 );

            ac = new AddonComponent( 2923 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, 0, 1, 0 );

            ac = new AddonComponent( 2845 );
            ac.Light = LightType.Circle225;
            ac.Name = "A Candelabra";
            AddComponent( ac, 0, 1, 17 );

            ac = new AddonComponent( 4006 );
            ac.Name = "Piano Keys";
            AddComponent( ac, 0, 1, 7 );

            ac = new AddonComponent( 7031 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, 0, 1, 12 );

            ac = new AddonComponent( 7933 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, 0, 1, 14 );

            ac = new AddonComponent( 5986 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, 0, 1, 14 );

            ac = new AddonComponent( 5986 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, 0, 1, 12 );

            ac = new AddonComponent( 5991 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, 0, 1, 8 );

            ac = new AddonComponent( 5987 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, 0, 1, 9 );

            ac = new AddonComponent( 5985 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, 0, 1, 10 );

            ac = new AddonComponent( 3774 );
            ac.Name = "Sheet Music";
            AddComponent( ac, 1, 1, 15 );

            ac = new AddonComponent( 3772 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, 1, 1, 12 );

            ac = new AddonComponent( 1114 );
            ac.Hue = 1;
            ac.Name = "Piano";
            AddComponent( ac, 1, 0, 0 );
        }

        public override BaseAddonDeed Deed
        {
            get { return new PianoAddonDeed(); }
        }

        public override void OnComponentUsed( AddonComponent ac, Mobile from )
        {
            if( !from.InRange( GetWorldLocation(), 1 ) )
                from.SendMessage( "You are too far away to use that!" );
            else if( ac.ItemID == 3774 )
                from.SendGump( new PianoGump() );
        }

        #region serialization
        public PianoAddon( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // Version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion

        public class PianoGump : Gump
        {
            public static string[] Keys = new string[]
                                              {
                                                  "do",
                                                  "do#",
                                                  "re",
                                                  "re#",
                                                  "mi",
                                                  "fa",
                                                  "fa#",
                                                  "sol",
                                                  "sol#",
                                                  "la",
                                                  "la#",
                                                  "si"
                                              };

            public static int[] XOffs = new int[] { 55, 95, 145, 185, 235, 275, 315, 365, 405, 455, 495, 545 };
            public static int[] YOffs = new int[] { 60, 80 };

            public PianoGump()
                : base( 0, 0 )
            {
                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                AddPage( 0 );

                AddBackground( 6, 15, 570, 140, 5054 );
                AddAlphaRegion( 16, 20, 550, 130 );
                AddImageTiled( 16, 20, 550, 20, 9354 );

                AddLabel( 19, 20, 200, "Piano Keys" );

                int btnIndex = 1;
                for( int j = 0; j < XOffs.Length; j++ )
                {
                    int xOff = XOffs[ j ];

                    for( int i = 0; i < YOffs.Length; i++ )
                    {
                        int yOff = YOffs[ i ];

                        AddLabel( xOff, yOff, 0, Keys[ j ] );
                        AddButton( xOff - 20, yOff + 2, 5601, 5605, btnIndex, GumpButtonType.Reply, 0 );
                        btnIndex++;
                    }
                }

                AddLabel( 55, 100, 0, "do" );
                AddButton( 35, 102, 5601, 5605, 3, GumpButtonType.Reply, 0 );
                AddButton( 425, 120, 241, 242, 26, GumpButtonType.Reply, 0 );
            }

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                Mobile m = sender.Mobile;
                if( m == null )
                    return;

                if( info.ButtonID > 0 && info.ButtonID < 20 )
                {
                    m.PlaySound( 1027 + info.ButtonID );
                    m.SendGump( new PianoGump() );
                }
                else if( info.ButtonID >= 20 && info.ButtonID < 26 )
                {
                    m.PlaySound( 1001 + info.ButtonID );
                    m.SendGump( new PianoGump() );
                }
                else
                {
                    m.SendMessage( 60, "You stop playing." );
                }
            }
        }
    }

    public class PianoAddonDeed : BaseAddonDeed
    {
        public override string DefaultName { get { return "piano"; } }

        public override BaseAddon Addon
        {
            get { return new PianoAddon(); }
        }

        [Constructable]
        public PianoAddonDeed()
        {
        }

        #region serialization
        public PianoAddonDeed( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // Version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}