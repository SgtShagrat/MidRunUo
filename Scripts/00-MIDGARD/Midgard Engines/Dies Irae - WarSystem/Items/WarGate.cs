/***************************************************************************
 *                               WarGate.cs
 *
 *   begin                : 12 marzo 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System.Collections.Generic;

using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.WarSystem
{
    public class WarGate : Item
    {
        public override string DefaultName
        {
            get { return "a war gate"; }
        }

        [Constructable]
        public WarGate()
            : base( 0xF6C )
        {
            Movable = false;
            Hue = 0x2D1;
            Light = LightType.Circle300;
        }

        public override bool OnMoveOver( Mobile m )
        {
            if( m != null && m.Player && m.Map != null )
            {
                if( Core.Instance.CurrentBattle != null && TravelDefinitions != null && TravelDefinitions.Length > 0 )
                {
                    m.CloseGump( typeof( WarGateGump ) );
                    m.SendGump( new WarGateGump( TravelDefinitions ) );
                    return true;
                }
                else
                {
                    m.SendMessage( "There is no battle in progress." );
                }
            }

            return false;
        }

        public WarGateTravelDefinition[] TravelDefinitions
        {
            get
            {
                return Core.Instance.CurrentBattle != null ? Core.Instance.CurrentBattle.TravelDefinitions : null;
            }
        }

        #region serialization
        public WarGate( Serial serial )
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

        internal class WarGateGump : Gump
        {
            private readonly List<WarGateTravelDefinition> m_List;

            private const int Fields = 9;
            private const int HueTit = 662;
            private const int DeltaBut = 2;
            private const int FieldsDist = 25;
            private const int HuePrim = 92;

            public WarGateGump( IEnumerable<WarGateTravelDefinition> list )
                : base( 50, 50 )
            {
                Closable = false;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                m_List = new List<WarGateTravelDefinition>( list );

                Design();
            }

            public virtual void Design()
            {
                AddPage( 0 );

                AddBackground( 0, 0, 275, 325, 9200 );

                AddImageTiled( 10, 10, 255, 25, 2624 );
                AddImageTiled( 10, 45, 255, 240, 2624 );
                AddImageTiled( 40, 295, 225, 20, 2624 );

                AddButton( 10, 295, 4017, 4018, 0, GumpButtonType.Reply, 0 );
                AddHtmlLocalized( 45, 295, 75, 20, 1011012, HueTit, false, false ); // CANCEL

                AddAlphaRegion( 10, 10, 255, 285 );
                AddAlphaRegion( 40, 295, 225, 20 );

                AddLabelCropped( 14, 12, 255, 25, HueTit, "Where will you go?" );

                for( int i = 0; i < m_List.Count; ++i )
                {
                    if( ( i % Fields ) == 0 )
                    {
                        if( i != 0 )
                            AddButton( 245, 297, 5601, 5605, 300, GumpButtonType.Page, ( i / Fields ) + 1 ); // Next page

                        AddPage( ( i / Fields ) + 1 );

                        if( i != 0 )
                            AddButton( 225, 297, 5603, 5607, 200, GumpButtonType.Page, ( i / Fields ) ); // Previous page
                    }

                    WarGateTravelDefinition m = m_List[ i ];

                    if( m != null )
                    {
                        AddLabelCropped( 50, 52 + ( ( i % 11 ) * FieldsDist ), 225, 20, HuePrim, m.Name );
                        AddButton( 15, 52 + DeltaBut + ( ( i % 11 ) * FieldsDist ), BtnNormal, BtnPressed, i + 1, GumpButtonType.Reply, 0 );
                    }
                }
            }

            private const int BtnNormal = 0x4b9;
            private const int BtnPressed = 0x4ba;

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                int index = info.ButtonID;
                if( index <= 0 )
                    return;

                index--;

                if( index > -1 && index < m_List.Count )
                    DoTeleport( sender.Mobile, m_List[ index ] );
            }

            public virtual void DoTeleport( Mobile m, WarGateTravelDefinition definition )
            {
                Map map = definition.Map;
                if( map == null || map == Map.Internal )
                    map = m.Map;

                Point3D p = definition.Location;
                if( p == Point3D.Zero )
                    p = m.Location;

                Server.Mobiles.BaseCreature.TeleportPets( m, p, map );

                bool sendEffect = ( !m.Hidden || m.AccessLevel == AccessLevel.Player );

                if( sendEffect )
                    Effects.SendLocationEffect( m.Location, m.Map, 0x3728, 10, 10 );

                m.MoveToWorld( p, map );

                if( sendEffect )
                    Effects.SendLocationEffect( m.Location, m.Map, 0x3728, 10, 10 );
            }
        }
    }

    public class WarGateTravelDefinition
    {
        public WarGateTravelDefinition( string name, Point3D location, Map map )
        {
            Name = name;
            Location = location;
            Map = map;
        }

        public string Name { get; private set; }

        public Point3D Location { get; private set; }

        public Map Map { get; private set; }
    }
}