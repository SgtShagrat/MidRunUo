/***************************************************************************
 *                               Ritual.cs
 *
 *   revision             : 03 January, 2010
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Engines.Classes
{
    public abstract class Ritual
    {
        protected Ritual( PowerDefinition definition, Mobile ritualist )
        {
            Ritualist = ritualist;
            Definition = definition;
        }

        public Mobile Ritualist { get; private set; }
        public PowerDefinition Definition { get; private set; }

        /// <summary>
        /// Invoked first. In this method all preliminary checks are run
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Invoked after Start. Starts the anim timer and setup the main
        /// ritual callback
        /// </summary>
        public abstract void BeginRitual();

        /// <summary>
        /// The main ritual effect.
        /// </summary>
        /// <param name="state">state of our callback</param>
        public abstract void MakeRitualCallback( object state );

        /// <summary>
        /// Effects that are passed to AnimTimer and executed every time
        /// that timer ticks
        /// </summary>
        /// <param name="from">the target of the effects</param>
        /// <param name="success">true if our ritual is running successfully</param>
        public abstract void DoEffects( Mobile from, bool success );

        /// <summary>
        /// Called at the end. Release locks and finalize the ritual
        /// </summary>
        /// <param name="from"></param>
        public abstract void EndRitual( Mobile from );

        public virtual double GetRitualDuration( int level )
        {
            return level;
        }

        public bool CheckCombat( Mobile toCheck )
        {
            for ( int i = 0; i < toCheck.Aggressed.Count; ++i )
            {
                AggressorInfo info = toCheck.Aggressed[ i ];

                if( info.Defender.Player && ( DateTime.Now - info.LastCombatTime ) < TimeSpan.FromSeconds( 30.0 ) )
                    return true;
            }

            return false;
        }

        public bool CheckResources( Container container, Midgard2PlayerMobile from )
        {
            if( from == null )
                return false;

            int level = from.ClassState.GetLevel( Definition ) + 1;

            bool success = true;
            foreach ( RequirementDefinition definition in Definition.Requirements )
            {
                if( container.GetAmount( definition.ItemType, true ) < definition.Quantity * level )
                    success = false;
            }

            return success;
        }
    }
}