/***************************************************************************
 *                               BaseClassAttributes.cs
 *
 *   begin                : 10 ottobre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;

namespace Midgard.Engines.Classes
{
    [PropertyObject]
    public abstract class BaseClassAttributes
    {
        public ClassPlayerState State { get; private set; }

        protected BaseClassAttributes( ClassPlayerState state )
        {
            State = state;
        }

        public override string ToString()
        {
            return "...";
        }

        protected void SetLevel( Type t, int value )
        {
            if( State != null )
                State.SetLevel( t, value );
        }

        protected int GetLevel( Type t )
        {
            return State != null ? State.GetLevel( t ) : 0;
        }
    }
}