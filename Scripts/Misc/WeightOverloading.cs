using System;
using Server;
using Server.Mobiles;

namespace Server.Misc
{
	public enum DFAlgorithm
	{
		Standard,
		PainSpike
	}

	public class WeightOverloading
	{
		public static void Initialize()
		{
			EventSink.Movement += new MovementEventHandler( EventSink_Movement );
		}

		private static DFAlgorithm m_DFA;

		public static DFAlgorithm DFA
		{
			get{ return m_DFA; }
			set{ m_DFA = value; }
		}

		public static void FatigueOnDamage( Mobile m, int damage )
		{
			double fatigue = 0.0;

			switch ( m_DFA )
			{
				case DFAlgorithm.Standard:
				{
					fatigue = (damage * (100.0 / m.Hits) * ((double)m.Stam / 100)) - 5.0;
					break;
				}
				case DFAlgorithm.PainSpike:
				{
					fatigue = (damage * ((100.0 / m.Hits) + ((50.0 + m.Stam) / 100) - 1.0)) - 5.0;
					break;
				}
			}

			if ( fatigue > 0 )
				m.Stam -= (int)fatigue;
		}

		public const int OverloadAllowance = 4; // We can be four stones overweight without getting fatigued

		public static int GetMaxWeight( Mobile m )
		{
			//return ((( Core.ML && m.Race == Race.Human) ? 100 : 40 ) + (int)(3.5 * m.Str));
			//Moved to core virtual method for use there

			return m.MaxWeight;
		}

		public static void EventSink_Movement( MovementEventArgs e )
		{
			Mobile from = e.Mobile;

			if ( !from.Alive || from.AccessLevel > AccessLevel.Player  )
				return;

            #region New UOSA Code based on demo decompilation by Batlin

            //byte DL = e.Mobile.WalkPulseIndex;
            //int PulseDifference = e.Mobile.MovementPulseRecord[DL] - Pulser.Pulses;

            BaseMount mount = from.Mount as BaseMount;

            if( from.Stam == 0 || ( mount != null && mount.Stam == 0 ) )
            {
                if( mount != null )
                {
                    from.SendLocalizedMessage( 500108 ); // Your mount is too fatigued to move
                    e.Blocked = true;
                }
                else
                {
                    from.SendLocalizedMessage( 500110 ); // You are too fatigued to move.
                    e.Blocked = from.StamMax > 10 || !from.Pushing;
                }

                if( e.Blocked )
                    return;
            }

            if( from is BaseCreature )
            {
                BaseCreature bc = (BaseCreature)from;
                Mobile rider = ( bc is IMount ) ? ( (IMount)bc ).Rider : null;

                int amt = ( rider != null ) ? 32 : 96;

                if( bc.StamMax <= 0 )
                {
                    Utility.Log( "spawnErrors.log", string.Format( "{0} - {1} - {2}", bc.Serial, bc.GetType().Name, bc.Location ));
                    bc.RawDex = 100;
                }

                int origStam = ( bc.Stam / bc.StamMax ) * 100;

                if( ( ++bc.StepsTaken % amt ) == 0 )
                    --bc.Stam;

                int newStam = ( bc.Stam * 100 ) / bc.StamMax;

                if( origStam > 10 && newStam <= 10 && rider != null && rider.Player )
                {
                    if( bc.Loyalty > 10 )
                        bc.Loyalty -= 10;
                    else
                        bc.Loyalty = 0;

                    if( bc.Hunger > 0 )
                        bc.Hunger--;

                    rider.LocalOverheadMessage( Network.MessageType.Regular, 0x3B2, true, "Your mount is very fatigued." );
                }
            }

            //if (e.Mobile.Alive && from.Player && from.AccessLevel == AccessLevel.Player)
            //{
            //    int ModifiedPulseDifference = 500;
            //    if (PulseDifference != 0)
            //        ModifiedPulseDifference /= PulseDifference;

            //    int WalkIntensity = ModifiedPulseDifference <= 125 ? 100 : 400;

            //    LoseFatigueWithMoving(WalkIntensity, e.Mobile);
            //}

            //DL = e.Mobile.WalkPulseIndex;
            //e.Mobile.MovementPulseRecord[DL] = Pulser.Pulses;
            //e.Mobile.WalkPulseIndex = (byte)((e.Mobile.WalkPulseIndex + 1) % 5);

            #endregion

			if ( !from.Player )
			{
				// Else it won't work on monsters.
				Spells.Ninjitsu.DeathStrike.AddStep( from );
				return;
			}

			int maxWeight = GetMaxWeight( from ) + OverloadAllowance;
			int overWeight = (Mobile.BodyWeight + from.TotalWeight) - maxWeight;

            if ( overWeight > 0 ) // mod varie by Dies Irae
            {
				from.Stam -= GetStamLoss( from, Math.Max( overWeight, 0 ), (e.Direction & Direction.Running) != 0 );

				if ( from.Stam == 0 )
				{
					from.SendLocalizedMessage( 500109 ); // You are too fatigued to move, because you are carrying too much weight!
					e.Blocked = true;
					return;
				}
			}

			if ( ((from.Stam * 100) / Math.Max( from.StamMax, 1 )) < 10 )
				--from.Stam;

			if ( from.Stam == 0 )
			{
				from.SendLocalizedMessage( 500110 ); // You are too fatigued to move.
				e.Blocked = true;
				return;
			}

			if ( from is PlayerMobile )
			{
				int amt = ( from.Mounted ? StepsByMountToLooseStamina : StepsByFootToLooseStamina );
				PlayerMobile pm = (PlayerMobile)from;

				if ( (++pm.StepsTaken % amt) == 0 )
					--from.Stam;
			}

			Spells.Ninjitsu.DeathStrike.AddStep( from );
		}

        public const int StepsByFootToLooseStamina = 16;
        public const int StepsByMountToLooseStamina = /* 48 */ 40;

		public static int GetStamLoss( Mobile from, int overWeight, bool running )
		{
			int loss = 5 + (overWeight / 25);

			if ( from.Mounted )
				loss /= 3;

			if ( running )
				loss *= 2;

			return loss;
		}

		public static bool IsOverloaded( Mobile m )
		{
			if ( !m.Player || !m.Alive || m.AccessLevel > AccessLevel.Player )
				return false;

			return ( (Mobile.BodyWeight + m.TotalWeight) > (GetMaxWeight( m ) + OverloadAllowance) );
		}

        #region RunUO Interpretation of Decompilation of OSI Code by Batlin, original disassembly shown below

        static int GetHPLevel( Mobile m )
        {
            return m.HitsMax == 0 ? 0 : ( m.Hits * 100 ) / m.HitsMax;
        }

	    static int GetModifiedRealStrength( Mobile m )
        {   
            // take out the Math.Min once core has been patched to fix default Mobile.MaxWeight
            return ( 40 + (int)( 3.5 * m.Str ) ) + OverloadAllowance; //m.Str * 4 + 30;
        }

        // EIP=0x0047107C
        static int GetEncumbrance( Mobile m )
        {
            int divisor = GetModifiedRealStrength( m );
            if( divisor == 0 )
                divisor = 1;

            return ( ( Mobile.BodyWeight + m.TotalWeight ) * 100 ) / divisor;
        }

        // EIP=0x0047107C
        static int GetCarryCapacity( Mobile m )
        {
            int multiplier = GetModifiedRealStrength( m );

            int divisor = ( GetHPLevel( m ) * multiplier ) / 100;
            if( divisor == 0 )
                divisor = 1;

            return ( ( Mobile.BodyWeight + m.TotalWeight ) * 100 ) / divisor;
        }

        // EIP=0x004713C6
        static void LoseFatigueWithMoving( int walkIntensity, Mobile m ) // 100 or 400
        {
            // EIP=0x004713D0
            int walkDiv10 = walkIntensity / 10;

            // EIP=0x004713DE
            int encumbrance = GetEncumbrance( m );
            int weightIntensity;

            weightIntensity = encumbrance > 100 ? GetCarryCapacity( m ) * 10 : GetCarryCapacity( m ) / 10;

            // EIP=0x00471412
            walkDiv10 += weightIntensity;

            // EIP=0x0047141B
            if( m.Mount != null )
            {
                // EIP=0x0047142B
                BaseMount mount = m.Mount as BaseMount;
                if( mount == null )
                {
                    // EIP=0x0047143C
                    // Something odd, needs research
                }
                else
                {
                    // EIP=0x00471449
                    int origStam = ( mount.Stam * 100 ) / mount.StamMax;

                    // EIP=0x00471468
                    LoseFatigueWithMoving( walkIntensity, mount );

                    // EIP=0x0047147A
                    int newStam = ( mount.Stam * 100 ) / mount.StamMax;

                    // EIP=0x00471499
                    // NOTE: only notify if horse stamina switched changed from >10 to <=10.
                    if( origStam > 10 && newStam <= 10 && m.Player )
                    {
                        if( mount.Loyalty > 10 )
                            mount.Loyalty -= 10;
                        else
                            mount.Loyalty = 0;
                        if( mount.Hunger > 0 )
                            mount.Hunger--;
                        m.LocalOverheadMessage( Network.MessageType.Regular, 0x3B2, true, "Your mount is very fatigued." );
                    }

                    if( ( mount.Loyalty == 0 ) && Utility.RandomDouble() < 0.01 ) // buck player
                    {
                        mount.
                        Mount.Rider = null;
                        mount.DoIdleAnimation();

                        mount.Say( 1043255, mount.Name ); // ~1_NAME~ appears to have decided that is better off without a master!
                        mount.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully Happy
                        mount.IsBonded = false;
                        mount.BondingBegin = DateTime.MinValue;
                        mount.OwnerAbandonTime = DateTime.MinValue;
                        mount.ControlTarget = null;

                        mount.ControlOrder = OrderType.None; // -=Derrick=-

                        mount.AIObject.DoOrderRelease(); // this will prevent no release of creatures left alone with AI disabled (and consequent bug of Followers)
                    }

                    // EIP=0x004714C1
                    walkDiv10 = walkDiv10 / 3;
                }
            }

            // EIP=0x004714CF
            // 0x274 looks like a fatigue counter. I suspect that this player variable should be decremented by 200 when stamina regenerates -=Derrick=-
            //m.FatigueCounter += walkDiv10;

            //m.Say( "FC:{0} MFC:{1}", m.FatigueCounter, ( m.Mount is BaseMount ) ? ( (BaseMount)m.Mount ).FatigueCounter : -1 );

            //while( m.FatigueCounter > m.MoveFatigueRate ) // NOTE: > and not >=
            //{
            //    m.FatigueCounter -= m.MoveFatigueRate;
            //    m.Stam--;
            //}
        }
        #endregion
	}
}