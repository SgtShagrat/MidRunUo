/***************************************************************************
 *                               RaceAllowanceAttribute.cs
 *
 *   begin                : 04 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;

namespace Midgard.Engines.Races
{
    [AttributeUsage( AttributeTargets.Class )]
    public class RaceAllowanceAttribute : Attribute
    {
        private static Type m_RaceAllowanceType = typeof( RaceAllowanceAttribute );

        public Type RaceType { get; set; }

        /// <summary>
        /// Check if our item is allowed to our Race.
        /// </summary>
        /// <param name="item">item we want to check</param>
        /// <param name="from">mobile who wants the item</param>
        /// <param name="raceToCheck">the race which is restricted</param>
        /// <param name="message">true if a message must be sent on rejected</param>
        /// <returns>return true if this item is allowed to Race</returns>
        /// <remarks>This attribute is not inherited</remarks>
        public static bool IsAllowed( Item item, Mobile from, Race raceToCheck, bool message )
        {
            // if we are not of checked race we can use the item
            if( from.Race != raceToCheck )
                return true;

            // if our class does not have the attribute... we are not allowed
            object[] attrs = item.GetType().GetCustomAttributes( m_RaceAllowanceType, false );

            // we have a race attribute... so check if they have the same race
            bool allowed = attrs.Length > 0 && ( (RaceAllowanceAttribute)attrs[ 0 ] ).RaceType == raceToCheck.GetType();

            if( !allowed && message )
                from.SendMessage( "Sei troppo goffo per indossarlo!" );

            return allowed;
        }

        public RaceAllowanceAttribute( Type raceType )
        {
            RaceType = raceType;
        }
    }
}
