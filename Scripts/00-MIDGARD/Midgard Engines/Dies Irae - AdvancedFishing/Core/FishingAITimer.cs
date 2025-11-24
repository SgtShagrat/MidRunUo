/***************************************************************************
 *                               FishingAITimer.cs
 *
 *   begin                : 20 settembre 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Engines.Harvest;
using Server.Misc;
using Server.Network;

namespace Midgard.Engines.AdvancedFishing
{
    public class FishingAITimer : Timer
    {
        private readonly Mobile m_From;
        private readonly SpecialFishingPole m_Pole;

        private int m_ActionTicksDelay;

        private Stages m_Stage;
        private Actions m_FishAction;

        private readonly int m_FishWeight;
        private int m_FishStr;
        private double m_Difficulty;
        private readonly Point3D m_StartLocation;

        public FishingAITimer( Mobile from, SpecialFishingPole pole, Point3D p, int tile )
            : base( TimeSpan.FromSeconds( Config.DefaultDelayInSeconds ), TimeSpan.FromSeconds( Config.OneTickInSeconds ) )
        {
            Priority = TimerPriority.FiftyMS;

            m_From = from;
            m_Pole = pole;

            m_Stage = Stages.Quiet;
            m_FishAction = Actions.None;
            m_Pole.FisherAction = Actions.None;
            m_Difficulty = 0.0;
            m_StartLocation = new Point3D( m_From.X, m_From.Y, m_From.Z );

            Fishing harvestSystem = Fishing.System;
            HarvestDefinition def = harvestSystem.Definition;
            HarvestBank bank = def.GetBank( from.Map, p.X, p.Y, p.Z, tile );
            bank.Consume( 1, from );

            if( bank.Current <= 1 )
            {
                EndFish( EndFishResults.NoMoreFishes, true, true );
                return;
            }

            bool deepWater = Core.CheckDeepWater( m_StartLocation, m_From.Map );
            Core.InitFishContest( from, deepWater, out m_FishStr, out m_FishWeight );

            m_ActionTicksDelay = Utility.RandomMinMax( 15, ( 25 * def.MaxTotal ) / bank.Current );
            if( m_From.PlayerDebug )
                m_ActionTicksDelay = 5;

            Core.SendDebugMessage( m_From, string.Format( "Timer vars: deep: {0} - str: {1} - size {2}gr - delay {3}s/10",
               deepWater, m_FishStr, m_FishWeight, m_ActionTicksDelay ) );
        }

        protected override void OnTick()
        {
            // Core.SendDebugMessage( m_From, String.Format( "stage: {0} - countdown: {1}", m_Stage, m_ActionTicksDelay )) ;

            m_ActionTicksDelay--;

            if( m_Stage == Stages.FishCatched || m_Stage == Stages.FisherReaction )
            {
                if( m_Pole.FisherAction != Actions.None )
                    m_ActionTicksDelay = 0;
            }

            if( m_Stage >= Stages.FisherReaction && m_FishStr == 0 )
            {
                EndFish( EndFishResults.Fished, true, true );
                return;
            }

            switch( m_Stage )
            {
                case Stages.Quiet:
                    {
                        if( m_FishStr == 0 )
                        {
                            EndFish( EndFishResults.NoMoreFishes, true, true );
                            return;
                        }

                        if( m_ActionTicksDelay <= 0 )
                            OnQuietEnded();
                        break;
                    }
                case Stages.FishCatched:
                    {
                        if( m_ActionTicksDelay <= 0 )
                            OnFishCatched();
                        break;
                    }
                case Stages.FishAction:
                    {
                        if( m_ActionTicksDelay <= 0 )
                            OnFishActed();
                        break;
                    }
                case Stages.FisherReaction:
                    {
                        if( m_ActionTicksDelay <= 0 )
                            OnFisherReacted();
                        break;
                    }
            }
        }

        private void OnFishStrengthChanged( int delta )
        {
            m_FishStr += delta;

            Core.SendDebugMessage( m_From, "Fish strength changed by '{0}'. Now it is '{1}'.", delta, m_FishStr );
        }

        private void OnFishCatched()
        {
            bool isFromGM = m_From.AccessLevel > AccessLevel.Counselor;

            Core.SendDebugMessage( m_From, "Called event OnFishCatched. pole: {0}, fish: {1}.", m_Pole.FisherAction, m_FishAction );

            if( m_Pole.FisherAction == Actions.None )
            {
                Core.SendDebugMessage( m_From, "m_Pole.FisherAction == Actions.None" );
                EndFish( EndFishResults.BadReflex, true, true );
            }
            else if( m_Pole.FisherAction != Config.FisherActionOnFishCatched )
            {
                Core.SendDebugMessage( m_From, "m_Pole.FisherAction != Config.FisherActionOnFishCatched" );
                EndFish( EndFishResults.WrongMove, true, true );
            }
            else
            {
                if( Core.CheckReflex( m_From, m_Pole, Config.DefaultReflexDelay ) || isFromGM )
                {
                    SkillCheck.CheckSkill( m_From, m_From.Skills[ SkillName.Fishing ], m_Pole, 0.5 );

                    m_From.SendLangMessage( 10020009 ); // "Well done! You fish a while... and successed!"

                    m_Stage = Stages.FishAction;
                    m_ActionTicksDelay = Config.GetFishActionDelayInTicks();
                    m_Pole.LastFishAction = DateTime.Now;
                    m_Pole.FisherAction = Actions.None;
                    m_FishAction = Actions.None;

                    OnFishStrengthChanged( -1 );

                    if( m_FishWeight > Config.IncredibleWeight )
                    {
                        string incredibleText = TextHelper.Text( 10020010, m_From.TrueLanguage ); // "*You see an incredible fish!!*"
                        m_From.PublicOverheadMessage( MessageType.Regular, 0x29, true, String.Format( incredibleText ) );
                    }
                }
                else
                    EndFish( EndFishResults.BadReflex, true, true );
            }
        }

        private void OnQuietEnded()
        {
            Core.SendDebugMessage( m_From, "Called event OnQuietEnded. Stage is now FishCatched." );

            m_Stage = Stages.FishCatched;
            m_ActionTicksDelay = Config.GetAfterQuietDelayInTicks();
            m_Pole.LastFishAction = DateTime.Now;
            m_Pole.FisherAction = Actions.None;

            m_From.SendLangMessage( 10020011 ); // Your war starts now!
        }

        private void OnFisherReacted()
        {
            bool isFromGM = m_From.AccessLevel > AccessLevel.Counselor;

            Core.SendDebugMessage( m_From, "Called event OnFisherReacted. pole: {0}, fish: {1}.", m_Pole.FisherAction, m_FishAction );

            if( !isFromGM && m_Pole.FisherAction != m_FishAction && m_Pole.FisherAction != Actions.None )
            {
                Core.SendDebugMessage( m_From, "m_Pole.FisherAction != m_FishAction && m_Pole.FisherAction != Actions.None" );
                if( Utility.RandomDouble() > 0.001 )
                    EndFish( EndFishResults.WrongMove, true, true );
                else
                    EndFish( EndFishResults.MoreStrength, true, false );
            }
            else if( m_Pole.FisherAction == Actions.None )
            {
                Core.SendDebugMessage( m_From, "m_Pole.FisherAction == Actions.None" );
                EndFish( EndFishResults.BadReflex, true, true );
            }
            else if( Core.CheckLocation( m_From, m_StartLocation ) )
            {
                Core.SendDebugMessage( m_From, "Core.CheckLocation( m_From, m_StartLocation )" );
                EndFish( EndFishResults.FisherMoved, true, true );
            }
            else if( !Core.CheckIsUnluckyContest( m_From ) )
            {
                Core.SendDebugMessage( m_From, "Core.CheckUnluckyContest( m_From )" );
                EndFish( EndFishResults.UnluckyContest, true, true );
            }
            else if( !Core.CheckReflex( m_From, m_Pole, Config.DefaultReflexDelay ) )
            {
                Core.SendDebugMessage( m_From, "!Core.CheckReflex( m_From, m_Pole, Config.DefaultReflexDelay )" );
                EndFish( EndFishResults.BadReflex, true, true );
            }
            else
            {
                if( Utility.RandomDouble() > 0.3 )
                    SkillCheck.CheckSkill( m_From, m_From.Skills[ SkillName.Fishing ], m_Pole, m_Difficulty );

                m_From.SendLangMessage( Core.RandomGoodReflexMessage() );

                OnFishStrengthChanged( -1 );
                m_Stage = Stages.FishAction;
                m_ActionTicksDelay = Config.GetFishActionDelayInTicks();
                m_Pole.LastFishAction = DateTime.Now;
                m_Pole.FisherAction = Actions.None;
                m_Difficulty += Config.DifficultyIncreaseOnRightMove;
            }
        }

        private void OnFishActed()
        {
            m_FishAction = (Actions)( Utility.RandomMinMax( 1, 3 ) );
            m_ActionTicksDelay = Config.GetFishActionDelayInTicks();
            m_Pole.LastFishAction = DateTime.Now;
            m_Pole.FisherAction = Actions.None;
            m_Stage = Stages.FisherReaction;

            int message = 0;

            switch( m_FishAction )
            {
                case Actions.Down:
                    message = Core.RandomDownMex();
                    break;
                case Actions.War:
                    message = Core.RandomWarMex();
                    break;
                case Actions.Jump:
                    message = Core.RandomJumpMex();
                    break;
            }

            if( message > 0 )
                m_From.SendLangMessage( message );

            Core.SendDebugMessage( m_From, "Called event OnFishActed. Fish action: {0} ({1})", m_FishAction, (int)m_FishAction );
        }

        private void EndFish( EndFishResults result, bool message, bool endAction )
        {
            switch( result )
            {
                case EndFishResults.Fished:
                    {
                        Item item = Core.CreateFish( m_From, m_FishWeight );
                        if( item != null )
                            item = Core.Mutate( item, m_From, m_Pole, m_From.Map, m_From.Location );

                        if( item != null )
                        {
                            if( Core.Give( m_From, item, true ) && message )
                                Core.SendSuccessTo( m_From, item );
                            else
                                item.Delete();

                            if( item is BaseAdvancedFish )
                            {
                                BaseAdvancedFish fish = (BaseAdvancedFish)item;
                                FishRanks.RegisterNewState( new FishRankState( m_From, fish ) );
                            }
                        }
                        break;
                    }
                case EndFishResults.NoMoreFishes:
                    {
                        m_From.SendLangMessage( 10020006 ); // "The fish don't seem to be biting here."
                        break;
                    }
                case EndFishResults.BadReflex:
                    {
                        m_From.SendLangMessage( Core.RandomBadReflexMessage() );
                        break;
                    }
                case EndFishResults.WrongMove:
                    {
                        m_From.SendLangMessage( Core.RandomWrongMoveMessage() );
                        break;
                    }
                case EndFishResults.MoreStrength:
                    {
                        OnFishStrengthChanged( 3 );
                        m_From.SendLangMessage( 10020008 ); // "Bad move! Your fish will fight with more strenght!"
                        break;
                    }
                case EndFishResults.FisherMoved:
                    {
                        m_From.SendLangMessage( 10020012 ); // "Art of fighing requires patience. You moved!"
                        break;
                    }
                case EndFishResults.UnluckyContest:
                    {
                        m_From.SendLangMessage( Core.RandomUnluckyMessage() );
                        break;
                    }
                case EndFishResults.WrongMoveOnCatched:
                    {
                        m_From.SendLangMessage( 10020101 ); // Try moving up your pole after catching a fish.
                        break;
                    }
            }

            if( endAction )
                m_Pole.Reset( m_From );
        }
    }
}