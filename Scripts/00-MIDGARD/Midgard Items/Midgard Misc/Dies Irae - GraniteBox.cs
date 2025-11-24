/***************************************************************************
 *                                     GraniteBox.cs
 *                            		-------------------
 *  begin                	: Maj, 2010
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;

using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Targeting;

namespace Midgard.Items
{
    public class GraniteBox : Item
    {
        private const int MaxStorage = 60000;
        private readonly int[] m_Resources = new int[ 23 ];

        [Constructable]
		public GraniteBox() : base( 15810 )
        {
            Weight = 100.0;
            Hue = 0x3df;
            UpdateWeight();

            for( int i = 0; i < 23; i++ )
                m_Resources[ i ] = 0;
        }

        #region resources
        [CommandProperty( AccessLevel.GameMaster )]
        public int Granite
        {
            get { return GetAmountFor( CraftResource.Iron ); }
            set { SetAmountFor( CraftResource.Iron, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int DullCopper
        {
            get { return GetAmountFor( CraftResource.OldDullCopper ); }
            set { SetAmountFor( CraftResource.OldDullCopper, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int ShadowIron
        {
            get { return GetAmountFor( CraftResource.OldShadowIron ); }
            set { SetAmountFor( CraftResource.OldShadowIron, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Copper
        {
            get { return GetAmountFor( CraftResource.OldCopper ); }
            set { SetAmountFor( CraftResource.OldCopper, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Bronze
        {
            get { return GetAmountFor( CraftResource.OldBronze ); }
            set { SetAmountFor( CraftResource.OldBronze, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Gold
        {
            get { return GetAmountFor( CraftResource.OldGold ); }
            set { SetAmountFor( CraftResource.OldGold, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Agapite
        {
            get { return GetAmountFor( CraftResource.OldAgapite ); }
            set { SetAmountFor( CraftResource.OldAgapite, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Verite
        {
            get { return GetAmountFor( CraftResource.OldVerite ); }
            set { SetAmountFor( CraftResource.OldVerite, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Valorite
        {
            get { return GetAmountFor( CraftResource.OldValorite ); }
            set { SetAmountFor( CraftResource.OldValorite, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Graphite
        {
            get { return GetAmountFor( CraftResource.OldGraphite ); }
            set { SetAmountFor( CraftResource.OldGraphite, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Pyrite
        {
            get { return GetAmountFor( CraftResource.OldPyrite ); }
            set { SetAmountFor( CraftResource.OldPyrite, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Azurite
        {
            get { return GetAmountFor( CraftResource.OldAzurite ); }
            set { SetAmountFor( CraftResource.OldAzurite, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Vanadium
        {
            get { return GetAmountFor( CraftResource.OldVanadium ); }
            set { SetAmountFor( CraftResource.OldVanadium, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Silver
        {
            get { return GetAmountFor( CraftResource.OldSilver ); }
            set { SetAmountFor( CraftResource.OldSilver, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Platinum
        {
            get { return GetAmountFor( CraftResource.OldPlatinum ); }
            set { SetAmountFor( CraftResource.OldPlatinum, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Amethyst
        {
            get { return GetAmountFor( CraftResource.OldAmethyst ); }
            set { SetAmountFor( CraftResource.OldAmethyst, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Titanium
        {
            get { return GetAmountFor( CraftResource.OldTitanium ); }
            set { SetAmountFor( CraftResource.OldTitanium, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Xenian
        {
            get { return GetAmountFor( CraftResource.OldXenian ); }
            set { SetAmountFor( CraftResource.OldXenian, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Rubidian
        {
            get { return GetAmountFor( CraftResource.OldRubidian ); }
            set { SetAmountFor( CraftResource.OldRubidian, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Obsidian
        {
            get { return GetAmountFor( CraftResource.OldObsidian ); }
            set { SetAmountFor( CraftResource.OldObsidian, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int EbonSapphire
        {
            get { return GetAmountFor( CraftResource.OldEbonSapphire ); }
            set { SetAmountFor( CraftResource.OldEbonSapphire, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int DarkRuby
        {
            get { return GetAmountFor( CraftResource.OldDarkRuby ); }
            set { SetAmountFor( CraftResource.OldDarkRuby, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int RadiantDiamond
        {
            get { return GetAmountFor( CraftResource.OldRadiantDiamond ); }
            set { SetAmountFor( CraftResource.OldRadiantDiamond, value ); }
        }
        #endregion

        public override int LabelNumber
        {
            get { return 1064507; } // A Granite Box
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !from.InRange( GetWorldLocation(), 2 ) )
                from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
            else if( from.Alive )
                from.SendGump( new GraniteBoxGump( from, this ) );
        }

        public void BeginCombine( Mobile from )
        {
            from.Target = new InternalTarget( this );
        }

        public virtual void UpdateWeight()
        {
            int sum = 0;
            foreach( int i in m_Resources )
                sum += i;

            Weight = 20 + 10 * sum;
        }

        public int GetAmountFor( CraftResource craftResource )
        {
            if( craftResource == CraftResource.Iron )
                return m_Resources[ 0 ];

            // NB: OldDullCopper = 51,
            // so 51 must be 1
            int index = craftResource - ( CraftResource.OldDullCopper - 1 );

            if( index >= 0 && index < m_Resources.Length )
                return m_Resources[ index ];
            else
                Console.WriteLine( "Warning: {0} {1}", craftResource, index );

            return 0;
        }

        public void SetAmountFor( CraftResource craftResource, int value )
        {
            if( craftResource == CraftResource.Iron )
            {
                m_Resources[ 0 ] = value;
                return;
            }

            // NB: OldDullCopper = 51,
            // so 51 must be 1
            int index = craftResource - ( CraftResource.OldDullCopper - 1 );

            if( index >= 0 && index < m_Resources.Length )
            {
                m_Resources[ index ] = value;
                UpdateWeight();
            }
            else
                Console.WriteLine( "Warning: {0} {1}", craftResource, index );
        }

        public void EndCombine( Mobile from, object o )
        {
            if( o is Item && ( (Item)o ).IsChildOf( from.Backpack ) )
            {
                if( !( o is BaseGranite ) )
					from.SendMessage( from.Language == "ITA" ? "Non puoi metterlo qui dentro." : "That is not an item you can put in here." );
                else
                {
                    BaseGranite baseGranite = (BaseGranite)o;
                    CraftResource craftResource = baseGranite.Resource;

                    int currentAmount = GetAmountFor( craftResource );
                    int toHold = Math.Min( MaxStorage - currentAmount, baseGranite.Amount );

                    if( toHold <= 0 )
                    {
						from.SendMessage( from.Language == "ITA" ? "Questo contenitore non ha più spazio!" : "The box will not hold any more!" );
                        return;
                    }

                    baseGranite.Consume( toHold );
                    SetAmountFor( craftResource, currentAmount + toHold );

                    if( !baseGranite.Deleted )
                        baseGranite.Bounce( from );

                    from.SendGump( new GraniteBoxGump( from, this ) );
                    BeginCombine( from );
                }
            }
            else
                from.SendLocalizedMessage( 1045158 ); // You must have the item in your backpack to target it.
        }

        private void DropGranite( Mobile from, CraftResource craftResource )
        {
            int currentAmount = GetAmountFor( craftResource );

            if( currentAmount > 0 )
            {
                Item granite = BaseGranite.GetGraniteFor( craftResource );
                if( granite != null )
                {
                    from.AddToBackpack( granite );
                    SetAmountFor( craftResource, currentAmount - 1 );
                    from.SendGump( new GraniteBoxGump( from, this ) );
                }
            }
            else
				from.SendMessage( from.Language == "ITA" ? "Non possiedi quel granito!" : "You do not have any of that Granite!" );
        }

        #region serial-deserial
        private static void SetSaveFlag( ref MidgardFlag flags, MidgardFlag toSet, bool setIf )
        {
            if( setIf )
                flags |= toSet;
        }

        private static bool GetSaveFlag( MidgardFlag flags, MidgardFlag toGet )
        {
            return ( ( flags & toGet ) != 0 );
        }

		public GraniteBox( Serial serial ) : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            MidgardFlag flags = MidgardFlag.None;

            SetSaveFlag( ref flags, MidgardFlag.DullCopper, DullCopper != 0 );
            SetSaveFlag( ref flags, MidgardFlag.ShadowIron, ShadowIron != 0 );
            SetSaveFlag( ref flags, MidgardFlag.Copper, Copper != 0 );
            SetSaveFlag( ref flags, MidgardFlag.Bronze, Bronze != 0 );

            SetSaveFlag( ref flags, MidgardFlag.Gold, Gold != 0 );
            SetSaveFlag( ref flags, MidgardFlag.Agapite, Agapite != 0 );
            SetSaveFlag( ref flags, MidgardFlag.Verite, Verite != 0 );
            SetSaveFlag( ref flags, MidgardFlag.Valorite, Valorite != 0 );

            SetSaveFlag( ref flags, MidgardFlag.Graphite, Graphite != 0 );
            SetSaveFlag( ref flags, MidgardFlag.Pyrite, Pyrite != 0 );
            SetSaveFlag( ref flags, MidgardFlag.Azurite, Azurite != 0 );
            SetSaveFlag( ref flags, MidgardFlag.Vanadium, Vanadium != 0 );

            SetSaveFlag( ref flags, MidgardFlag.Silver, Silver != 0 );
            SetSaveFlag( ref flags, MidgardFlag.Platinum, Platinum != 0 );
            SetSaveFlag( ref flags, MidgardFlag.Amethyst, Amethyst != 0 );
            SetSaveFlag( ref flags, MidgardFlag.Titanium, Titanium != 0 );

            SetSaveFlag( ref flags, MidgardFlag.Xenian, Xenian != 0 );
            SetSaveFlag( ref flags, MidgardFlag.Rubidian, Rubidian != 0 );
            SetSaveFlag( ref flags, MidgardFlag.Obsidian, Obsidian != 0 );
            SetSaveFlag( ref flags, MidgardFlag.EbonSapphire, EbonSapphire != 0 );

            SetSaveFlag( ref flags, MidgardFlag.DarkRuby, DarkRuby != 0 );
            SetSaveFlag( ref flags, MidgardFlag.RadiantDiamond, RadiantDiamond != 0 );
            SetSaveFlag( ref flags, MidgardFlag.Granite, Granite != 0 );

            writer.WriteEncodedInt( (int)flags );

            if( GetSaveFlag( flags, MidgardFlag.DullCopper ) )
                writer.WriteEncodedInt( DullCopper );
            if( GetSaveFlag( flags, MidgardFlag.ShadowIron ) )
                writer.WriteEncodedInt( ShadowIron );
            if( GetSaveFlag( flags, MidgardFlag.Copper ) )
                writer.WriteEncodedInt( Copper );
            if( GetSaveFlag( flags, MidgardFlag.Bronze ) )
                writer.WriteEncodedInt( Bronze );

            if( GetSaveFlag( flags, MidgardFlag.Gold ) )
                writer.WriteEncodedInt( Gold );
            if( GetSaveFlag( flags, MidgardFlag.Agapite ) )
                writer.WriteEncodedInt( Agapite );
            if( GetSaveFlag( flags, MidgardFlag.Verite ) )
                writer.WriteEncodedInt( Verite );
            if( GetSaveFlag( flags, MidgardFlag.Valorite ) )
                writer.WriteEncodedInt( Valorite );

            if( GetSaveFlag( flags, MidgardFlag.Graphite ) )
                writer.WriteEncodedInt( Graphite );
            if( GetSaveFlag( flags, MidgardFlag.Pyrite ) )
                writer.WriteEncodedInt( Pyrite );
            if( GetSaveFlag( flags, MidgardFlag.Azurite ) )
                writer.WriteEncodedInt( Azurite );
            if( GetSaveFlag( flags, MidgardFlag.Vanadium ) )
                writer.WriteEncodedInt( Vanadium );

            if( GetSaveFlag( flags, MidgardFlag.Silver ) )
                writer.WriteEncodedInt( Silver );
            if( GetSaveFlag( flags, MidgardFlag.Platinum ) )
                writer.WriteEncodedInt( Platinum );
            if( GetSaveFlag( flags, MidgardFlag.Amethyst ) )
                writer.WriteEncodedInt( Amethyst );
            if( GetSaveFlag( flags, MidgardFlag.Titanium ) )
                writer.WriteEncodedInt( Titanium );

            if( GetSaveFlag( flags, MidgardFlag.Xenian ) )
                writer.WriteEncodedInt( Xenian );
            if( GetSaveFlag( flags, MidgardFlag.Rubidian ) )
                writer.WriteEncodedInt( Rubidian );
            if( GetSaveFlag( flags, MidgardFlag.Obsidian ) )
                writer.WriteEncodedInt( Obsidian );
            if( GetSaveFlag( flags, MidgardFlag.EbonSapphire ) )
                writer.WriteEncodedInt( EbonSapphire );

            if( GetSaveFlag( flags, MidgardFlag.DarkRuby ) )
                writer.WriteEncodedInt( DarkRuby );
            if( GetSaveFlag( flags, MidgardFlag.RadiantDiamond ) )
                writer.WriteEncodedInt( RadiantDiamond );
            if( GetSaveFlag( flags, MidgardFlag.Granite ) )
                writer.WriteEncodedInt( Granite );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            MidgardFlag flags = (MidgardFlag)reader.ReadEncodedInt();

            if( GetSaveFlag( flags, MidgardFlag.DullCopper ) )
                DullCopper = reader.ReadEncodedInt();
            if( GetSaveFlag( flags, MidgardFlag.ShadowIron ) )
                ShadowIron = reader.ReadEncodedInt();
            if( GetSaveFlag( flags, MidgardFlag.Copper ) )
                Copper = reader.ReadEncodedInt();
            if( GetSaveFlag( flags, MidgardFlag.Bronze ) )
                Bronze = reader.ReadEncodedInt();
            if( GetSaveFlag( flags, MidgardFlag.Gold ) )
                Gold = reader.ReadEncodedInt();
            if( GetSaveFlag( flags, MidgardFlag.Agapite ) )
                Agapite = reader.ReadEncodedInt();
            if( GetSaveFlag( flags, MidgardFlag.Verite ) )
                Verite = reader.ReadEncodedInt();
            if( GetSaveFlag( flags, MidgardFlag.Valorite ) )
                Valorite = reader.ReadEncodedInt();
            if( GetSaveFlag( flags, MidgardFlag.Graphite ) )
                Graphite = reader.ReadEncodedInt();
            if( GetSaveFlag( flags, MidgardFlag.Pyrite ) )
                Pyrite = reader.ReadEncodedInt();
            if( GetSaveFlag( flags, MidgardFlag.Azurite ) )
                Azurite = reader.ReadEncodedInt();
            if( GetSaveFlag( flags, MidgardFlag.Vanadium ) )
                Vanadium = reader.ReadEncodedInt();
            if( GetSaveFlag( flags, MidgardFlag.Silver ) )
                Silver = reader.ReadEncodedInt();
            if( GetSaveFlag( flags, MidgardFlag.Platinum ) )
                Platinum = reader.ReadEncodedInt();
            if( GetSaveFlag( flags, MidgardFlag.Amethyst ) )
                Amethyst = reader.ReadEncodedInt();
            if( GetSaveFlag( flags, MidgardFlag.Titanium ) )
                Titanium = reader.ReadEncodedInt();
            if( GetSaveFlag( flags, MidgardFlag.Xenian ) )
                Xenian = reader.ReadEncodedInt();
            if( GetSaveFlag( flags, MidgardFlag.Rubidian ) )
                Rubidian = reader.ReadEncodedInt();
            if( GetSaveFlag( flags, MidgardFlag.Obsidian ) )
                Obsidian = reader.ReadEncodedInt();
            if( GetSaveFlag( flags, MidgardFlag.EbonSapphire ) )
                EbonSapphire = reader.ReadEncodedInt();
            if( GetSaveFlag( flags, MidgardFlag.DarkRuby ) )
                DarkRuby = reader.ReadEncodedInt();
            if( GetSaveFlag( flags, MidgardFlag.RadiantDiamond ) )
                RadiantDiamond = reader.ReadEncodedInt();
            if( GetSaveFlag( flags, MidgardFlag.Granite ) )
                Granite = reader.ReadEncodedInt();
        }

        private enum MidgardFlag
        {
            None = 0x00000000,

            DullCopper = 0x00000001,
            ShadowIron = 0x00000002,
            Copper = 0x00000004,
            Bronze = 0x00000008,

            Gold = 0x00000010,
            Agapite = 0x00000020,
            Verite = 0x00000040,
            Valorite = 0x00000080,

            Graphite = 0x00000100,
            Pyrite = 0x00000200,
            Azurite = 0x00000400,
            Vanadium = 0x00000800,

            Silver = 0x00001000,
            Platinum = 0x00002000,
            Amethyst = 0x00004000,
            Titanium = 0x00008000,

            Xenian = 0x00010000,
            Rubidian = 0x00020000,
            Obsidian = 0x00040000,
            EbonSapphire = 0x00080000,

            DarkRuby = 0x00100000,
            RadiantDiamond = 0x00200000,
            Granite = 0x00400000,
        }
        #endregion

        private class GraniteBoxGump : Gump
        {
            private readonly GraniteBox m_Box;
            private readonly Mobile m_From;

            public GraniteBoxGump( Mobile from, GraniteBox box )
                : base( 50, 50 )
            {
                m_From = from;
                m_Box = box;

                m_From.CloseGump( typeof( GraniteBoxGump ) );

                AddPage( 0 );

                AddBackground( 0, 0, 500, 370, 2600 );
				AddFirstCharHuedHtml( 200, 30, 100, 18, from.Language == "ITA" ? "Box Graniti" : "Granite Box", Colors.Indigo, true );

				AddOldHtml( 60, 50, 100, 18, from.Language == "ITA" ? "Aggiungi gran." : "Add granite" );
                AddButton( 25, 50, 4005, 4007, (int)Buttons.AddGranite, GumpButtonType.Reply, 0 );

				AddOldHtml( 60, 70, 100, 18, from.Language == "ITA" ? "Chiudi" : "Close" );
                AddButton( 25, 70, 4005, 4007, (int)Buttons.Close, GumpButtonType.Reply, 0 );

				AddLabel( 60, 100, 0x480, from.Language == "ITA" ? "Graniti" : "Granite" );
                AddButton( 25, 100, 4005, 4007, (int)Buttons.RemoveGranite, GumpButtonType.Reply, 0 );
                AddLabel( 210, 100, 0x480, m_Box.Granite.ToString() );

                for( int i = 0; i < 11; i++ )
                {
                    // OldDullCopper = 51,
                    AddInfo( (CraftResource)( 51 + i ), i, false );
                    AddInfo( (CraftResource)( 62 + i ), i, true );
                }
            }

            private void AddInfo( CraftResource res, int index, bool offset )
            {
                int delta = offset ? 225 : 0;

                AddButton( 25 + delta, 130 + ( 20 * index ), 4005, 4007, (int)res, GumpButtonType.Reply, 0 );

                CraftResourceInfo info = CraftResources.GetInfo( res );
                if( info != null )
                    AddLabel( 60 + delta, 130 + ( 20 * index ), info.Hue - 1, info.Name );
                else
                    AddLabel( 60 + delta, 130 + ( 20 * index ), 0x480, res.ToString() );

                AddLabel( 210 + delta, 130 + ( 20 * index ), 0x480, m_Box.GetAmountFor( res ).ToString() );
            }

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                if( m_Box.Deleted )
                    return;

                int index = info.ButtonID;
                if( index <= 0 )
                    return;

                if( index == (int)Buttons.RemoveGranite )
                    m_Box.DropGranite( sender.Mobile, CraftResource.Iron );
                else if( index == (int)Buttons.AddGranite )
                    m_Box.BeginCombine( sender.Mobile );

                if( index >= (int)CraftResource.OldDullCopper && index <= (int)CraftResource.OldRadiantDiamond )
                    m_Box.DropGranite( sender.Mobile, (CraftResource)index );
            }

            private enum Buttons
            {
                Close,
                AddGranite,
                RemoveGranite
            }
        }

        private class InternalTarget : Target
        {
            private readonly GraniteBox m_Box;

            public InternalTarget( GraniteBox box )
                : base( 18, false, TargetFlags.None )
            {
                CheckLOS = true;

                m_Box = box;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if( m_Box.Deleted )
                    return;

                m_Box.EndCombine( from, targeted );
            }
        }
    }
}