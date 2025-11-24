using System;
using Server;
using Server.Items;
using Midgard.Engines.BrewCrafing;

namespace Midgard.Engines.WineCrafting
{
    public class BaseGrapeVine : Item
    {
        private const int Max = 5;
        private DateTime m_Lastpicked;
        private int m_Yield;
        private BrewVariety m_Variety;
        private Mobile m_Placer;

        public Timer RegrowTimer;

        [CommandProperty( AccessLevel.GameMaster )]
        public int Yield { get { return m_Yield; } set { m_Yield = value; } }

        public int Capacity { get { return Max; } }
        public DateTime LastPick { get { return m_Lastpicked; } set { m_Lastpicked = value; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public BrewVariety Variety
        {
            get { return m_Variety; }
            set { m_Variety = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Placer
        {
            get { return m_Placer; }
            set { m_Placer = value; }
        }

        public virtual BrewVariety DefaultVariety { get { return BrewVariety.CabernetSauvignon; } }

        [Constructable]
        public BaseGrapeVine( int itemID )
            : base( itemID )
        {
            Movable = false;
            m_Variety = DefaultVariety;
            m_Placer = null;

            Init( this, false );
        }

        public override void AddNameProperty( ObjectPropertyList list )
        {
            list.Add( 1060658, "Variety\t{0}", BrewingResources.GetName( m_Variety ) );

            base.AddNameProperty( list );
        }

        public override void OnSingleClick( Mobile from )
        {
            LabelTo( from, "Variety : {0} ", BrewingResources.GetName( m_Variety ) );
        }

        public static void Init( BaseGrapeVine plant, bool full )
        {
            plant.LastPick = DateTime.Now;
            plant.RegrowTimer = new FruitTimer( plant );

            if( full )
            {
                plant.Yield = plant.Capacity;
            }
            else
            {
                plant.Yield = 5;
                plant.RegrowTimer.Start();
            }
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( from.Mounted )
            {
                from.SendMessage( "You cannot pick fruit while mounted." );
                return;
            }

            if( DateTime.Now > m_Lastpicked.AddSeconds( 5 ) )
            {
                m_Lastpicked = DateTime.Now;

                int lumberValue = (int)from.Skills[ SkillName.Cooking ].Value / 20;

                if( lumberValue < 0 )
                {
                    from.SendMessage( "You have no idea how to pick this fruit." );
                    return;
                }

                if( from.InRange( GetWorldLocation(), 2 ) )
                {
                    if( m_Placer == null || from == m_Placer || from.AccessLevel >= AccessLevel.GameMaster )
                    {
                        if( m_Yield < 1 )
                        {
                            from.SendMessage( "There is nothing here to harvest." );
                        }
                        else
                        {
                            from.Direction = from.GetDirectionTo( this );

                            from.Animate( 17, 7, 1, true, false, 0 );

                            if( lumberValue < m_Yield )
                                lumberValue = m_Yield + 1;

                            int pick = Utility.Random( lumberValue );
                            if( pick == 0 )
                            {
                                from.SendMessage( "You do not manage to gather any fruit." );
                                return;
                            }

                            m_Yield -= pick;
                            from.SendMessage( "You pick {0} grape bunch{1}!", pick, ( pick == 1 ? "" : "es" ) );

                            //PublicOverheadMessage( MessageType.Regular, 0x3BD, false, string.Format( "{0}", m_Yield ));

                            GiveGrapes( from, pick, m_Variety );

                            if( !RegrowTimer.Running )
                            {
                                RegrowTimer.Start();
                            }
                        }
                    }
                    else
                    {
                        from.SendMessage( "You do not own these vines." );
                    }
                }
                else
                {
                    from.SendLocalizedMessage( 500446 ); // That is too far away.
                }
            }
            else
            {
                from.SendMessage( "You must wait a few moments before you can pick more fruit." );
            }
        }

        public static void GiveGrapes( Mobile m, int pick, BrewVariety variety )
        {
            switch( variety )
            {
                case BrewVariety.CabernetSauvignon:
                    {
                        CabernetSauvignonGrapes cscrop = new CabernetSauvignonGrapes( pick );
                        m.AddToBackpack( cscrop );
                        break;
                    }
                case BrewVariety.Chardonnay:
                    {
                        ChardonnayGrapes ccrop = new ChardonnayGrapes( pick );
                        m.AddToBackpack( ccrop );
                        break;
                    }
                case BrewVariety.CheninBlanc:
                    {
                        CheninBlancGrapes cbcrop = new CheninBlancGrapes( pick );
                        m.AddToBackpack( cbcrop );
                        break;
                    }
                case BrewVariety.Merlot:
                    {
                        MerlotGrapes mcrop = new MerlotGrapes( pick );
                        m.AddToBackpack( mcrop );
                        break;
                    }
                case BrewVariety.PinotNoir:
                    {
                        PinotNoirGrapes pncrop = new PinotNoirGrapes( pick );
                        m.AddToBackpack( pncrop );
                        break;
                    }
                case BrewVariety.Riesling:
                    {
                        RieslingGrapes rcrop = new RieslingGrapes( pick );
                        m.AddToBackpack( rcrop );
                        break; //Riesling
                    }
                case BrewVariety.Sangiovese:
                    {
                        SangioveseGrapes scrop = new SangioveseGrapes( pick );
                        m.AddToBackpack( scrop );
                        break; //Sangiovese
                    }
                case BrewVariety.SauvignonBlanc:
                    {
                        SauvignonBlancGrapes sbcrop = new SauvignonBlancGrapes( pick );
                        m.AddToBackpack( sbcrop );
                        break; //Sauvignon Blanc
                    }
                case BrewVariety.Shiraz:
                    {
                        ShirazGrapes shcrop = new ShirazGrapes( pick );
                        m.AddToBackpack( shcrop );
                        break; //Shiraz
                    }
                case BrewVariety.Viognier:
                    {
                        ViognierGrapes vcrop = new ViognierGrapes( pick );
                        m.AddToBackpack( vcrop );
                        break; //Viognier
                    }
                case BrewVariety.Zinfandel:
                    {
                        ZinfandelGrapes zcrop = new ZinfandelGrapes( pick );
                        m.AddToBackpack( zcrop );
                        break; //Zinfandel
                    }
                default:
                    {
                        Grapes crop = new Grapes( pick );
                        m.AddToBackpack( crop );
                        break;
                    }
            }
        }

        private class FruitTimer : Timer
        {
            private BaseGrapeVine m_Plant;

            public FruitTimer( BaseGrapeVine plant )
                : base( TimeSpan.FromSeconds( 60 ), TimeSpan.FromSeconds( 30 ) )
            {
                Priority = TimerPriority.OneSecond;
                m_Plant = plant;
            }

            protected override void OnTick()
            {
                if( ( m_Plant != null ) && ( !m_Plant.Deleted ) )
                {
                    int current = m_Plant.Yield;

                    if( ++current >= m_Plant.Capacity )
                    {
                        current = m_Plant.Capacity;
                        Stop();
                    }
                    else if( current <= 0 )
                        current = 1;

                    m_Plant.Yield = current;

                    //m_Plant.PublicOverheadMessage( MessageType.Regular, 0x22, false, string.Format( "{0}", current ));
                }
                else
                    Stop();
            }
        }

        public BaseGrapeVine( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)2 );

            //Version 2
            writer.Write( (Mobile)m_Placer );

            //Version 1
            writer.Write( (int)m_Variety );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();

            switch( version )
            {
                case 2:
                    {
                        m_Placer = reader.ReadMobile();
                        goto case 1;
                    }
                case 1:
                    {
                        m_Variety = (BrewVariety)reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        Init( this, true );
                        break;
                    }
            }
        }
    }
}