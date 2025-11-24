/***************************************************************************
 *                                  MidgardFoodDecaySystem.cs
 *                            		-------------------
 *  begin                	: Settembre, 2006
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using Server;
using Server.Network;

namespace Midgard.Engines.FoodDecaySystem
{
    public class FoodDecayTimer : Timer
    {
        internal static void StartTimer()
        {
            new FoodDecayTimer().Start();
        }

        public FoodDecayTimer()
            : base( TimeSpan.FromMinutes( 1.0 ), TimeSpan.FromMinutes( 20.0 ) )
        {
            Priority = TimerPriority.OneMinute;
        }

        protected override void OnTick()
        {
            if( Config.Debug )
                Config.Pkg.LogInfo( "Food decay check." );

            foreach( NetState state in NetState.Instances )
            {
                ComputeFoodStatLoss( state.Mobile );
                StatLossDecay( state.Mobile );
            }
        }

        public static void LoadMidgardLocalization()
        {
            TextHelper.LoadLocalization( "FoodDecaySystem.cfg" );

            /*
            Eng	10010001	You are almost dieing...
            Eng	10010002	You are starving.
            Eng	10010003	Your stomash hurts and you feel dizzy.
            Eng	10010004	You are enough hungry.
            Eng	10010005	You are somewhat hungry.
            Eng	10010006	You are with little hungry.
            Eng	10010007	You are enough full.
            Eng	10010008	You are quite full.
            Eng	10010009	You are full.
             */
        }

        public static int GetHungerMessage( Mobile mobile )
        {
            int status = mobile.Hunger + mobile.Thirst;

            if( status <= 0 )
                return 10010001;
            else if( status < 5 )
                return 10010002;
            else if( status < 10 )
                return 10010003;
            else if( status < 15 )
                return 10010004;
            else if( status < 20 )
                return 10010005;
            else if( status < 25 )
                return 10010006;
            else if( status < 30 )
                return 10010007;
            else if( status < 35 )
                return 10010008;
            else
                return 10010009;
        }

        public static void StatLossDecay( Mobile m )
        {
            if( m == null || !m.Alive || m.AccessLevel > AccessLevel.Player )
                return;

            if( m.Hunger > 0 )
                m.Hunger -= 1;

            if( m.Thirst > 0 )
                m.Thirst -= 1;

            if( m.Hunger + m.Thirst < 20 )
                m.SendLangMessage( GetHungerMessage( m ) );
        }

        public static void ComputeFoodStatLoss( Mobile m )
        {
            if( !Config.Enabled )
                return;

            if( m == null || !m.Alive || m.AccessLevel > AccessLevel.Player )
                return;

            // Rimozione degli statloss da fame
            m.RemoveStatMod( "FoodStatLossStr" );
            m.RemoveStatMod( "FoodStatLossDex" );
            m.RemoveStatMod( "FoodStatLossInt" );

            // Per ora la formula fa variare bi-lineramente da 60% per t = 0 a 0% per t = 30.
            int sum = m.Hunger + m.Thirst;
            int percStatLoss = ( -2 * sum + 60 < 0 ) ? 0 : ( -2 * sum + 60 );

            // Applicazione dello statloss
            m.AddStatMod( new StatMod( StatType.Str, "FoodStatLossStr", -( percStatLoss * m.Str ) / 100, TimeSpan.Zero ) );
            m.AddStatMod( new StatMod( StatType.Dex, "FoodStatLossDex", -( percStatLoss * m.Dex ) / 100, TimeSpan.Zero ) );
            m.AddStatMod( new StatMod( StatType.Int, "FoodStatLossInt", -( percStatLoss * m.Int ) / 100, TimeSpan.Zero ) );
        }
    }
}