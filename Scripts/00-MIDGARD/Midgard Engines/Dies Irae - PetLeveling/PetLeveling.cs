using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Midgard.Engines.PetSystem
{
    public class PetLeveling
    {
        #region costanti
        public static bool NewPetSystem = false;

        public static bool AosPetMovesEnabled = false;

        public static bool AosPetSystemEnabled = false;

        public static readonly int PercStatLoss = 2;
        public static readonly int EffectStatLoss = 4;
        public static readonly bool StatLossenabled = false;

        public static readonly int MinLevelForMate = 9;
        #endregion

        #region metodi
        /// <summary>
        /// Method to check if our creature should suffer a stat loss 
        /// </summary>
        /// <param name="bc">our creature</param>
        /// <param name="message">true if a message should be sent to the creature master</param>
        public static void CheckStatLoss( BaseCreature bc, bool message )
        {
            if( !AosPetSystemEnabled )
                return;

            if( bc != null && StatLossenabled )
            {
                #region Statloss
                if( Utility.Random( 100 ) < PercStatLoss )
                {
                    int strloss = bc.Str / EffectStatLoss;
                    int dexloss = bc.Dex / EffectStatLoss;
                    int intloss = bc.Int / EffectStatLoss;
                    int hitsloss = bc.Hits / EffectStatLoss;
                    int stamloss = bc.Stam / EffectStatLoss;
                    int manaloss = bc.Mana / EffectStatLoss;
                    int physloss = bc.PhysicalResistance / EffectStatLoss;
                    int fireloss = bc.FireResistance / EffectStatLoss;
                    int coldloss = bc.ColdResistance / EffectStatLoss;
                    int energyloss = bc.EnergyResistance / EffectStatLoss;
                    int poisonloss = bc.PoisonResistance / EffectStatLoss;
                    int dminloss = bc.DamageMin / EffectStatLoss;
                    int dmaxloss = bc.DamageMax / EffectStatLoss;

                    if( bc.Str > strloss )
                        bc.Str -= strloss;

                    if( bc.Str > dexloss )
                        bc.Dex -= dexloss;

                    if( bc.Str > intloss )
                        bc.Int -= intloss;

                    if( bc.HitsMaxSeed > hitsloss )
                        bc.HitsMaxSeed -= hitsloss;
                    if( bc.StamMaxSeed > stamloss )
                        bc.StamMaxSeed -= stamloss;
                    if( bc.ManaMaxSeed > manaloss )
                        bc.ManaMaxSeed -= manaloss;

                    if( bc.PhysicalResistanceSeed > physloss )
                        bc.PhysicalResistanceSeed -= physloss;
                    if( bc.FireResistSeed > fireloss )
                        bc.FireResistSeed -= fireloss;
                    if( bc.ColdResistSeed > coldloss )
                        bc.ColdResistSeed -= coldloss;
                    if( bc.EnergyResistSeed > energyloss )
                        bc.EnergyResistSeed -= energyloss;
                    if( bc.PoisonResistSeed > poisonloss )
                        bc.PoisonResistSeed -= poisonloss;

                    if( bc.DamageMin > dminloss )
                        bc.DamageMin -= dminloss;

                    if( bc.DamageMax > dmaxloss )
                        bc.DamageMax -= dmaxloss;

                    if( message )
                    {
                        Mobile master = bc.GetMaster();

                        if( master != null )
                            master.SendMessage( 38, "Your pet has suffered a {0}% of stat lose due to its death.", EffectStatLoss );
                    }
                }
                #endregion
            }
        }

        public static void GainExp( BaseCreature bc, int gain, bool message )
        {
            if( !AosPetSystemEnabled )
                return;

            if( !PetUtility.IsLevelablePet( bc ) )
                return;

            if( bc.Level <= bc.MaxLevel - 1 )
            {
                bc.Exp += gain;

                // Broadcast gain
                if( message && bc.ControlMaster != null )
                    bc.ControlMaster.SendMessage( "Your pet has gained {0} experience points.", gain );

                // Check for next level
                if( bc.Exp >= bc.NextLevel * bc.Level )
                    DoLevelIncrease( bc, true );
            }
        }

        /// <summary>
        /// Calculate the experience required for a given craeture to advance
        /// to the next level
        /// </summary>
        /// <param name="bc">our creature</param>
        /// <returns>xp points required to advance</returns>
        public static int ComputeNextLevel( BaseCreature bc )
        {
            int totalstats = bc.Str + bc.Dex + bc.Int + bc.HitsMax + bc.StamMax + bc.ManaMax +
                bc.PhysicalResistance + bc.FireResistance + bc.ColdResistance + bc.EnergyResistance + bc.PoisonResistance +
                bc.DamageMin + bc.DamageMax + bc.VirtualArmor;

            return totalstats * 10;
        }

        /// <summary>
        /// Calculate the experience value that should be distribuited
        /// for a given creature
        /// </summary>
        /// <param name="bc">our creature</param>
        /// <returns>xp points to distribute</returns>
        public static int EvalExpForCreature( BaseCreature bc )
        {
            return Utility.RandomMinMax( bc.HitsMax * 25, bc.HitsMax * 50 );
        }

        private static int ExpPointsAtOne = 40;
        private static int ExpPointsAtMax = 10;

        /// <summary>
        /// Calculate the ability points that should be for a level advance to our creature
        /// </summary>
        /// <param name="bc">our creature</param>
        /// <param name="level">level to calculate for</param>
        /// <returns>ability points gained</returns>
        public static int EvalAbilityPointsForLevel( BaseCreature bc, int level )
        {
            int linear = (int)( ExpPointsAtOne + ( ( ExpPointsAtMax - ExpPointsAtOne ) / (double)( bc.MaxLevel - 1 ) ) * ( level - 1 ) );

            if( linear < ExpPointsAtMax )
                linear = ExpPointsAtMax;
            else if( linear > ExpPointsAtOne )
                linear = ExpPointsAtOne;

            return linear + Utility.Random( 5 );
        }

        /// <summary>
        /// Event invoked when a creature advance a level.
        /// </summary>
        /// <param name="bc">our creature</param>
        /// <param name="message">true if a message should be sent to the creature master</param>
        public static void DoLevelIncrease( BaseCreature bc, bool message )
        {
            if( !AosPetSystemEnabled )
                return;

            if( bc == null || bc.Deleted )
                return;

            Mobile master = bc.ControlMaster;

            bc.Level++;
            bc.Exp = 0;

            // Effects
            bc.FixedParticles( 0x373A, 10, 15, 5012, EffectLayer.Waist );
            bc.PlaySound( 503 );

            // Experience
            if( message && master != null )
            {
                master.SendMessage( 38, "Experience level of {0} increased to {1}.",
                    !string.IsNullOrEmpty( bc.Name ) ? bc.Name : bc.GetType().Name, bc.Level );
            }

            // Ability Points
            int toGain = EvalAbilityPointsForLevel( bc, bc.Level );
            bc.AbilityPoints += toGain;
            if( message && master != null )
                master.SendMessage( 38, "Your pet has gained {0} ability points.", toGain );

            // Allow mating
            if( bc.Level == MinLevelForMate )
            {
                bc.AllowMating = true;
                if( message && master != null )
                    master.SendMessage( 1161, "Your pet is now at the level to mate." );
            }
        }

        /// <summary>
        /// Event invoked when a message should be cast to the creature master
        /// </summary>
        /// <param name="bc">our creature</param>
        public static void BroadcastDeathMessage( BaseCreature bc )
        {
            if( !AosPetSystemEnabled )
                return;

            Mobile master = bc.ControlMaster;

            if( bc.Controlled && !bc.Summoned && master != null )
            {
                master.SendMessage( 64, "Your pet has been killed!" );
            }
        }

        /// <summary>
        /// Event invoked when a creature is dead and have to distribute its experience
        /// </summary>
        /// <param name="bc">our creature</param>
        /// <param name="message">true if a message should be sent to the creature master</param>
        public static void DistributeExperience( BaseCreature bc, bool message )
        {
            if( !AosPetSystemEnabled )
                return;

            if( bc == null || bc.Summoned || bc.IsBonded || bc.Controlled )
                return;

            List<DamageEntry> rights = bc.DamageEntries;
            int expTodistribute = EvalExpForCreature( bc );

            foreach( DamageEntry entry in rights )
            {
                BaseCreature damager = Mobile.GetDamagerFrom( entry ) as BaseCreature;

                if( damager != null && damager.Controlled && !damager.Summoned && damager.ControlMaster != null )
                {
                    double perc = Math.Min( entry.DamageGiven / (double)bc.HitsMax, 1.0 );

                    int rawExp = (int)( perc * expTodistribute );

                    int maxExp = GetMaxExpForLevel( damager );

                    GainExp( damager, Math.Min( rawExp, maxExp ), message );
                }
            }
        }

        /// <summary>
        /// Return the maximum experience gainable for a given kill.
        /// </summary>
        /// <param name="creature"></param>
        /// <returns></returns>
        public static int GetMaxExpForLevel( BaseCreature creature )
        {
            if( Core.AOS )
                return (int)( ( creature.NextLevel * creature.Level ) / (double)( creature.Level + 10 ) );

            return (int)( ( creature.NextLevel * creature.Level ) / ( Math.Pow( creature.Level, 2 ) + 10 ) );
        }

        /// <summary>
        /// General event invoked by OnBeforeDeath of BaseCreature class
        /// </summary>
        /// <param name="bc">our creature</param>
        public static void HandleDeath( BaseCreature bc )
        {
            if( !AosPetSystemEnabled )
                return;

            if( bc == null || bc.Summoned )
                return;

            BroadcastDeathMessage( bc );

            DistributeExperience( bc, true );

            CheckStatLoss( bc, true );
        }
        #endregion
    }
}