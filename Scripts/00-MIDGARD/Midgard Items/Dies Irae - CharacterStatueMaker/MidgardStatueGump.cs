/***************************************************************************
 *                               MidgardStatueGump.cs
 *
 *   begin                : 07 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Midgard.Items.StatueSystem
{
    public class MidgardStatueGump : Gump
    {
        private readonly Item m_Maker;
        private readonly MidgardStatue m_Statue;
        private readonly Timer m_Timer;
        private readonly Mobile m_Owner;

        private enum Buttons
        {
            Close,
            Sculpt,
            PosePrev,
            PoseNext,
            DirPrev,
            DirNext,
            MatPrev,
            MatNext,
            Restore
        }

        private const int XLabel = 20;

        public MidgardStatueGump( Item maker, MidgardStatue statue, Mobile owner )
            : base( 60, 36 )
        {
            m_Maker = maker;
            m_Statue = statue;
            m_Owner = owner;

            if( m_Statue == null )
                return;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage( 0 );

            const int width = 327;

            AddBackground( 0, 0, width, 324, 0x13BE );
            AddImageTiled( 10, 10, width - 20, 20, 0xA40 );
            AddImageTiled( 10, 40, width - 20, 244, 0xA40 );
            AddImageTiled( 10, 294, width - 20, 20, 0xA40 );
            AddAlphaRegion( 10, 10, width - 20, 304 );
            AddLabelCropped( XLabel, 12, width, 20, 0x296, "Midgard statue maker:" );

            // pose
            AddHtmlLocalized( XLabel, 41, 120, 20, 1076168, 0x7FFF, false, false ); // Choose Pose
            AddHtmlLocalized( XLabel, 61, 120, 20, 1076208 + (int)m_Statue.Pose, 0x77E, false, false );
            AddButton( XLabel + 30, 81, 0xFA5, 0xFA7, (int)Buttons.PoseNext, GumpButtonType.Reply, 0 );
            AddButton( XLabel, 81, 0xFAE, 0xFB0, (int)Buttons.PosePrev, GumpButtonType.Reply, 0 );

            // direction
            AddHtmlLocalized( XLabel, 126, 120, 20, 1076170, 0x7FFF, false, false ); // Choose Direction
            AddHtmlLocalized( XLabel, 146, 120, 20, GetDirectionNumber( m_Statue.Direction ), 0x77E, false, false );
            AddButton( XLabel + 30, 167, 0xFA5, 0xFA7, (int)Buttons.DirNext, GumpButtonType.Reply, 0 );
            AddButton( XLabel, 167, 0xFAE, 0xFB0, (int)Buttons.DirPrev, GumpButtonType.Reply, 0 );

            // material
            if( m_Owner.AccessLevel > AccessLevel.Player )
            {
                AddHtmlLocalized( XLabel, 211, 120, 20, 1076171, 0x7FFF, false, false ); // Choose Material
                AddHtmlLocalized( XLabel, 231, 120, 20, GetMaterialNumber( m_Statue.Material ), 0x77E, false, false );
                AddButton( XLabel + 30, 253, 0xFA5, 0xFA7, (int)Buttons.MatNext, GumpButtonType.Reply, 0 );
                AddButton( XLabel, 253, 0xFAE, 0xFB0, (int)Buttons.MatPrev, GumpButtonType.Reply, 0 );
            }
            else
            {
                AddLabelCropped( XLabel, 211, 120, 20, 0x834, "Defined material:" );
                AddHtmlLocalized( XLabel, 231, 120, 20, GetMaterialNumber( m_Statue.Material ), 0x77E, false, false );
            }

            // cancel			
            AddButton( 10, 294, 0xFB1, 0xFB2, (int)Buttons.Close, GumpButtonType.Reply, 0 );
            AddHtmlLocalized( 45, 294, 80, 20, 1006045, 0x7FFF, false, false );	// Cancel

            // sculpt			
            AddButton( 234, 294, 0xFB7, 0xFB9, (int)Buttons.Sculpt, GumpButtonType.Reply, 0 );
            AddHtmlLocalized( 269, 294, 80, 20, 1076174, 0x7FFF, false, false ); // Sculpt	

            // restore			
            if( m_Maker is MidgardStatueDeed || owner.AccessLevel == AccessLevel.Developer )
            {
                AddButton( 107, 294, 0xFAB, 0xFAD, (int)Buttons.Restore, GumpButtonType.Reply, 0 );
                AddHtmlLocalized( 142, 294, 80, 20, 1076193, 0x7FFF, false, false ); // Restore	
            }

            m_Timer = Timer.DelayCall( TimeSpan.FromSeconds( 2.5 ), TimeSpan.FromSeconds( 2.5 ), new TimerCallback( CheckOnline ) );
        }

        private static CraftResource GetPrevious( CraftResource resource )
        {
            CraftResource previous = resource;

            if( resource == CraftResource.Iron )
                return CraftResource.Enchanted;

            if( resource == CraftResource.Enchanted )
                return CraftResource.Crystal;

            if( resource == CraftResource.RegularWood )
                return CraftResource.OldRadiantDiamond;

            if( resource == CraftResource.OldDullCopper )
                return CraftResource.Iron;

            previous--;

            return previous;
        }

        private static CraftResource GetNext( CraftResource resource )
        {
            CraftResource next = resource;

            if( resource == CraftResource.Iron )
                return CraftResource.OldDullCopper;

            if( resource == CraftResource.OldRadiantDiamond )
                return CraftResource.RegularWood;

            if( resource == CraftResource.Crystal )
                return CraftResource.Enchanted;

            if( resource == CraftResource.Enchanted )
                return CraftResource.Iron;

            next++;

            return next;
        }

        private void CheckOnline()
        {
            if( m_Owner == null || m_Owner.NetState != null )
                return;

            if( m_Timer != null )
                m_Timer.Stop();

            if( m_Statue != null && !m_Statue.Deleted )
                m_Statue.Delete();
        }

        private static int GetMaterialNumber( CraftResource material )
        {
            CraftResourceInfo info = CraftResources.GetInfo( material );
            return info != null ? info.Number : 0;
        }

        private static int GetDirectionNumber( Direction direction )
        {
            switch( direction )
            {
                case Direction.North: return 1075389;
                case Direction.Right: return 1075388;
                case Direction.East: return 1075387;
                case Direction.Down: return 1076204;
                case Direction.South: return 1075386;
                case Direction.Left: return 1075391;
                case Direction.West: return 1075390;
                case Direction.Up: return 1076205;
                default: return 1075386;
            }
        }

        public override void OnResponse( NetState state, RelayInfo info )
        {
            if( m_Statue == null || m_Statue.Deleted )
                return;

            bool sendGump = false;

            if( info.ButtonID == (int)Buttons.Sculpt )
            {
                if( m_Maker is MidgardStatueDeed )
                {
                    MidgardStatue backup = ( (MidgardStatueDeed)m_Maker ).Statue;

                    if( backup != null )
                        backup.Delete();
                }

                if( m_Maker != null )
                    m_Maker.Delete();

                m_Statue.Sculpt( state.Mobile );
            }
            else if( info.ButtonID == (int)Buttons.PosePrev )
            {
                m_Statue.Pose = (MidgardStatuePose)( ( (int)m_Statue.Pose + 5 ) % 6 );
                sendGump = true;
            }
            else if( info.ButtonID == (int)Buttons.PoseNext )
            {
                m_Statue.Pose = (MidgardStatuePose)( ( (int)m_Statue.Pose + 1 ) % 6 );
                sendGump = true;
            }
            else if( info.ButtonID == (int)Buttons.DirPrev )
            {
                m_Statue.Direction = (Direction)( ( (int)m_Statue.Direction + 7 ) % 8 );
                m_Statue.InvalidatePose();
                sendGump = true;
            }
            else if( info.ButtonID == (int)Buttons.DirNext )
            {
                m_Statue.Direction = (Direction)( ( (int)m_Statue.Direction + 1 ) % 8 );
                m_Statue.InvalidatePose();
                sendGump = true;
            }
            else if( info.ButtonID == (int)Buttons.MatPrev )
            {
                m_Statue.Material = GetPrevious( m_Statue.Material );
                sendGump = true;
            }
            else if( info.ButtonID == (int)Buttons.MatNext )
            {
                m_Statue.Material = GetNext( m_Statue.Material );
                sendGump = true;
            }
            else if( info.ButtonID == (int)Buttons.Restore )
            {
                if( m_Maker is MidgardStatueDeed )
                {
                    MidgardStatue backup = ( (MidgardStatueDeed)m_Maker ).Statue;

                    if( backup != null )
                        m_Statue.Restore( backup );
                }

                sendGump = true;
            }
            else
            {
                m_Statue.Delete();
            }
            if( sendGump )
                state.Mobile.SendGump( new MidgardStatueGump( m_Maker, m_Statue, m_Owner ) );

            if( m_Timer != null )
                m_Timer.Stop();
        }

        public override void OnServerClose( NetState owner )
        {
            if( m_Timer != null )
                m_Timer.Stop();

            if( m_Statue != null && !m_Statue.Deleted )
                m_Statue.Delete();
        }
    }
}