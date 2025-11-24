using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Targeting;

namespace Midgard.Engines.TownHouses
{
    public enum HammerJob { Flip, Swap }

    public class SignHammer : Item
    {
        private static Dictionary<int, int> m_Table = new Dictionary<int, int>();
        private static List<int> m_List = new List<int>();

        public static void InitTable()
        {
            // Signs
            m_Table[ 0xB95 ] = 0xB96;
            m_Table[ 0xB96 ] = 0xB95;
            m_Table[ 0xBA3 ] = 0xBA4;
            m_Table[ 0xBA4 ] = 0xBA3;
            m_Table[ 0xBA5 ] = 0xBA6;
            m_Table[ 0xBA6 ] = 0xBA5;
            m_Table[ 0xBA7 ] = 0xBA8;
            m_Table[ 0xBA8 ] = 0xBA7;
            m_Table[ 0xBA9 ] = 0xBAA;
            m_Table[ 0xBAA ] = 0xBA9;
            m_Table[ 0xBAB ] = 0xBAC;
            m_Table[ 0xBAC ] = 0xBAB;
            m_Table[ 0xBAD ] = 0xBAE;
            m_Table[ 0xBAE ] = 0xBAD;
            m_Table[ 0xBAF ] = 0xBB0;
            m_Table[ 0xBB0 ] = 0xBAF;
            m_Table[ 0xBB1 ] = 0xBB2;
            m_Table[ 0xBB2 ] = 0xBB1;
            m_Table[ 0xBB3 ] = 0xBB4;
            m_Table[ 0xBB4 ] = 0xBB3;
            m_Table[ 0xBB5 ] = 0xBB6;
            m_Table[ 0xBB6 ] = 0xBB5;
            m_Table[ 0xBB7 ] = 0xBB8;
            m_Table[ 0xBB8 ] = 0xBB7;
            m_Table[ 0xBB9 ] = 0xBBA;
            m_Table[ 0xBBA ] = 0xBB9;
            m_Table[ 0xBBB ] = 0xBBC;
            m_Table[ 0xBBC ] = 0xBBB;
            m_Table[ 0xBBD ] = 0xBBE;
            m_Table[ 0xBBE ] = 0xBBD;
            m_Table[ 0xBBF ] = 0xBC0;
            m_Table[ 0xBC0 ] = 0xBBF;
            m_Table[ 0xBC1 ] = 0xBC2;
            m_Table[ 0xBC2 ] = 0xBC1;
            m_Table[ 0xBC3 ] = 0xBC4;
            m_Table[ 0xBC4 ] = 0xBC3;
            m_Table[ 0xBC5 ] = 0xBC6;
            m_Table[ 0xBC6 ] = 0xBC5;
            m_Table[ 0xBC7 ] = 0xBC8;
            m_Table[ 0xBC8 ] = 0xBC7;
            m_Table[ 0xBC9 ] = 0xBCA;
            m_Table[ 0xBCA ] = 0xBC9;
            m_Table[ 0xBCB ] = 0xBCC;
            m_Table[ 0xBCC ] = 0xBCB;
            m_Table[ 0xBCD ] = 0xBCE;
            m_Table[ 0xBCE ] = 0xBCD;
            m_Table[ 0xBCF ] = 0xBD0;
            m_Table[ 0xBD0 ] = 0xBCF;
            m_Table[ 0xBD1 ] = 0xBD2;
            m_Table[ 0xBD2 ] = 0xBD1;
            m_Table[ 0xBD3 ] = 0xBD4;
            m_Table[ 0xBD4 ] = 0xBD3;
            m_Table[ 0xBD5 ] = 0xBD6;
            m_Table[ 0xBD6 ] = 0xBD5;
            m_Table[ 0xBD7 ] = 0xBD8;
            m_Table[ 0xBD8 ] = 0xBD7;
            m_Table[ 0xBD9 ] = 0xBDA;
            m_Table[ 0xBDA ] = 0xBD9;
            m_Table[ 0xBDB ] = 0xBDC;
            m_Table[ 0xBDC ] = 0xBDB;
            m_Table[ 0xBDD ] = 0xBDE;
            m_Table[ 0xBDE ] = 0xBDD;
            m_Table[ 0xBDF ] = 0xBE0;
            m_Table[ 0xBE0 ] = 0xBDF;
            m_Table[ 0xBE1 ] = 0xBE2;
            m_Table[ 0xBE2 ] = 0xBE1;
            m_Table[ 0xBE3 ] = 0xBE4;
            m_Table[ 0xBE4 ] = 0xBE3;
            m_Table[ 0xBE5 ] = 0xBE6;
            m_Table[ 0xBE6 ] = 0xBE5;
            m_Table[ 0xBE7 ] = 0xBE8;
            m_Table[ 0xBE8 ] = 0xBE7;
            m_Table[ 0xBE9 ] = 0xBEA;
            m_Table[ 0xBEA ] = 0xBE9;
            m_Table[ 0xBEB ] = 0xBEC;
            m_Table[ 0xBEC ] = 0xBEB;
            m_Table[ 0xBED ] = 0xBEE;
            m_Table[ 0xBEE ] = 0xBED;
            m_Table[ 0xBEF ] = 0xBF0;
            m_Table[ 0xBF0 ] = 0xBEF;
            m_Table[ 0xBF1 ] = 0xBF2;
            m_Table[ 0xBF2 ] = 0xBF1;
            m_Table[ 0xBF3 ] = 0xBF4;
            m_Table[ 0xBF4 ] = 0xBF3;
            m_Table[ 0xBF5 ] = 0xBF6;
            m_Table[ 0xBF6 ] = 0xBF5;
            m_Table[ 0xBF7 ] = 0xBF8;
            m_Table[ 0xBF8 ] = 0xBF7;
            m_Table[ 0xBF9 ] = 0xBFA;
            m_Table[ 0xBFA ] = 0xBF9;
            m_Table[ 0xBFB ] = 0xBFC;
            m_Table[ 0xBFC ] = 0xBFB;
            m_Table[ 0xBFD ] = 0xBFE;
            m_Table[ 0xBFE ] = 0xBFD;
            m_Table[ 0xBFF ] = 0xC00;
            m_Table[ 0xC00 ] = 0xBFF;
            m_Table[ 0xC01 ] = 0xC02;
            m_Table[ 0xC02 ] = 0xC01;
            m_Table[ 0xC03 ] = 0xC04;
            m_Table[ 0xC04 ] = 0xC03;
            m_Table[ 0xC05 ] = 0xC06;
            m_Table[ 0xC06 ] = 0xC05;
            m_Table[ 0xC07 ] = 0xC08;
            m_Table[ 0xC08 ] = 0xC07;
            m_Table[ 0xC09 ] = 0xC0A;
            m_Table[ 0xC0A ] = 0xC09;
            m_Table[ 0xC0B ] = 0xC0C;
            m_Table[ 0xC0C ] = 0xC0B;
            m_Table[ 0xC0D ] = 0xC0E;
            m_Table[ 0xC0E ] = 0xC0D;

            // Hangers
            m_Table[ 0xB97 ] = 0xB98;
            m_Table[ 0xB98 ] = 0xB97;
            m_Table[ 0xB99 ] = 0xB9A;
            m_Table[ 0xB9A ] = 0xB99;
            m_Table[ 0xB9B ] = 0xB9C;
            m_Table[ 0xB9C ] = 0xB9B;
            m_Table[ 0xB9D ] = 0xB9E;
            m_Table[ 0xB9E ] = 0xB9D;
            m_Table[ 0xB9F ] = 0xBA0;
            m_Table[ 0xBA0 ] = 0xB9F;
            m_Table[ 0xBA1 ] = 0xBA2;
            m_Table[ 0xBA2 ] = 0xBA1;

            // Hangers for swapping
            m_List.Add( 0xB97 );
            m_List.Add( 0xB98 );
            m_List.Add( 0xB99 );
            m_List.Add( 0xB9A );
            m_List.Add( 0xB9B );
            m_List.Add( 0xB9C );
            m_List.Add( 0xB9D );
            m_List.Add( 0xB9E );
            m_List.Add( 0xB9F );
            m_List.Add( 0xBA0 );
            m_List.Add( 0xBA1 );
            m_List.Add( 0xBA2 );
        }

        public override string DefaultName { get { return "Sign Hammer"; } }

        public HammerJob Job { get; set; }

        [Constructable]
        public SignHammer()
            : base( 0x13E3 )
        {
        }

        public int GetFlipFor( int id )
        {
            if( m_Table != null )
            {
                if( m_Table.ContainsKey( id ) )
                    return m_Table[ id ];
            }

            return id;
        }

        public int GetNextSign( int id )
        {
            if( !m_List.Contains( id ) )
                return id;

            int idx = m_List.IndexOf( id );

            if( idx + 2 < m_List.Count )
                return m_List[ idx + 2 ];

            if( idx % 2 == 0 )
                return m_List[ 0 ];

            return m_List[ 1 ];
        }

        public override void OnDoubleClick( Mobile m )
        {
            if( RootParent != m )
            {
                m.SendMessage( "That item must be in your backpack to use." );
                return;
            }

            BaseHouse house = BaseHouse.FindHouseAt( m );

            if( m.AccessLevel == AccessLevel.Player && ( house == null || house.Owner != m ) )
            {
                m.SendMessage( "You have to be inside your house to use this." );
                return;
            }

            m.BeginTarget( 3, false, TargetFlags.None, new TargetCallback( OnTarget ) );
        }

        protected void OnTarget( Mobile m, object obj )
        {
            Item item = obj as Item;

            if( item == null )
            {
                m.SendMessage( "You cannot change that with this." );
                return;
            }

            if( item == this )
            {
                new SignHammerGump( m, this );
                return;
            }

            if( Job == HammerJob.Flip )
            {
                int id = GetFlipFor( item.ItemID );

                if( id == item.ItemID )
                    m.SendMessage( "You cannot change that with this." );
                else
                    item.ItemID = id;
            }
            else
            {
                int id = GetNextSign( item.ItemID );

                if( id == item.ItemID )
                    m.SendMessage( "You cannot change that with this." );
                else
                    item.ItemID = id;
            }
        }

        #region serialization
        public SignHammer( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion

        internal class SignHammerGump : GumpPlusLight
        {
            private SignHammer m_Hammer;

            public SignHammerGump( Mobile m, SignHammer hammer )
                : base( m, 100, 100 )
            {
                m_Hammer = hammer;

                NewGump();
            }

            protected override void BuildGump()
            {
                AddBackground( 0, 0, 200, 200, 2600 );

                AddButton( 50, 45, 2152, 2154, "Swap", new GumpCallback( Swap ) );
                AddHtml( 90, 50, 70, "Swap Hanger" );

                AddButton( 50, 95, 2152, 2154, "Flip", new GumpCallback( Flip ) );
                AddHtml( 90, 100, 70, "Flip Sign or Hanger" );
            }

            private void Swap()
            {
                m_Hammer.Job = HammerJob.Swap;
            }

            private void Flip()
            {
                m_Hammer.Job = HammerJob.Flip;
            }
        }
    }
}