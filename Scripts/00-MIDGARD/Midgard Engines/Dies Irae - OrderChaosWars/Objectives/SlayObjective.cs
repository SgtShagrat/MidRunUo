/***************************************************************************
 *                               SlayObjective.cs
 *                            -------------------
 *   begin                : 01 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using Server;

namespace Midgard.Engines.OrderChaosWars
{
    public class SlayObjective : BaseObjective
    {
        public Type Creature { get; private set; }

        public Region Region { get; private set; }

        public SlayObjective( Type creature, string name, int amount, Virtue virtue )
            : this( creature, name, amount, null, virtue )
        {
        }

        public SlayObjective( Type creature, string name, int amount, string region, Virtue virtue )
            : this( creature, name, amount, region, 0, virtue )
        {
        }

        public SlayObjective( Type creature, string name, int amount, int seconds, Virtue virtue )
            : this( creature, name, amount, null, seconds, virtue )
        {
        }

        public SlayObjective( Type creature, string name, int amount, string region, int seconds, Virtue virtue )
            : base( amount, seconds, name, virtue )
        {
            Creature = creature;

            if( region != null )
                Region = Core.GetRegion( region );
        }

        public virtual void OnKill( Mobile killed )
        {
            War.GiveExtraPoints( OwnerVirtue, 1 );
        }

        public virtual bool IsObjective( Mobile mob )
        {
            if( Creature == null )
                return false;

            if( Creature.IsAssignableFrom( mob.GetType() ) )
            {
                return Region == null || Region.Contains( mob.Location );
            }

            return false;
        }

        public void Update( object obj )
        {
            if( !( obj is Mobile ) )
                return;

            Mobile mob = (Mobile)obj;

            if( !IsObjective( mob ) )
                return;

            if( !Completed )
                CurProgress += 1;

            OnKill( mob );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();
        }
    }
}