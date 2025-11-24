using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Engines.Harvest;
using Server.Targeting;

namespace Midgard.Commands
{
    public class GetHarvestInfo
    {
        public static void Initialize()
        {
            CommandSystem.Register( "GetHarvestInfo", AccessLevel.GameMaster, new CommandEventHandler( GetHarvestInfo_OnCommand ) );
        }

        [Usage( "GetHarvestInfo" )]
        [Description( "Get harvesting info for a target." )]
        private static void GetHarvestInfo_OnCommand( CommandEventArgs e )
        {
            e.Mobile.SendMessage( "Seleziona qualcosa" );
            e.Mobile.Target = new InternalTarget();
        }

        private class InternalTarget : Target
        {
            public InternalTarget()
                : base( 30, true, TargetFlags.None )
            {
            }

            protected override void OnTarget( Mobile from, object targ )
            {
                IPoint3D p = targ as IPoint3D;

                if( p != null )
                {
                    int tileID;
                    Map map;
                    Point3D loc;

                    if( !Lumberjacking.System.GetHarvestDetails( from, null, targ, out tileID, out map, out loc ) ||
                        !Mining.System.GetHarvestDetails( from, null, targ, out tileID, out map, out loc ) ||
                        !Fishing.System.GetHarvestDetails( from, null, targ, out tileID, out map, out loc ) )
                    {
                        from.SendMessage( "There is no details available for that tile." );
                        return;
                    }

                    HarvestDefinition def = Lumberjacking.System.GetDefinition( tileID );
                    if( def == null )
                        def = Mining.System.GetDefinition( tileID );
                    else
                        def = Fishing.System.GetDefinition( tileID );

                    if( def == null )
                    {
                        from.SendMessage( "There is no harvest definition for that tile." );
                    }
                    else if( !Lumberjacking.System.CheckResources( from, null, def, map, loc, false, targ ) ||
                                !Mining.System.CheckResources( from, null, def, map, loc, false, targ ) ||
                                !Fishing.System.CheckResources( from, null, def, map, loc, false, targ ) )
                    {
                        from.SendMessage( "There is no resource available for that tile." );
                    }
                    else
                    {
                        Dictionary<Point2D, HarvestBank> banks = null;
                        def.Banks.TryGetValue( map, out banks );

                        if( banks == null )
                            def.Banks[ map ] = banks = new Dictionary<Point2D, HarvestBank>();

                        Point2D key = new Point2D( loc.X, loc.Y );

                        HarvestBank bank = null;
                        banks.TryGetValue( key, out bank );

                        if( bank != null )
                        {
                            from.SendMessage( string.Format( "Harvest Definition: {0}", def ) );
                            from.SendMessage( string.Format( "Vein: {0}", bank.Vein.PrimaryResource ) );
                            from.SendMessage( string.Format( "Amount {0}/{1}", bank.Current, def.MaxTotal ) );
                        }
                    }
                }
            }
        }
    }
}