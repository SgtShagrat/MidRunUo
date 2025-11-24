/***************************************************************************
 *                               SlayObjective.cs
 *
 *   begin                : 20 febbraio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using Server;

namespace Midgard.Engines.WarSystem
{
    public class SlayObjective : BaseObjective
    {
        public Type Creature { get; private set; }

        public Region Region { get; private set; }

        public SlayObjective( Type creature, string name, int amount, WarTeam warTeam )
            : this( creature, name, amount, null, warTeam )
        {
        }

        public SlayObjective( Type creature, string name, int amount, string region, WarTeam warTeam )
            : this( creature, name, amount, region, 0, warTeam )
        {
        }

        public SlayObjective( Type creature, string name, int amount, int seconds, WarTeam warTeam )
            : this( creature, name, amount, null, seconds, warTeam )
        {
        }

        public SlayObjective( Type creature, string name, int amount, string region, int seconds, WarTeam warTeam )
            : base( amount, seconds, name, warTeam )
        {
            Creature = creature;

            if( region != null )
                Region = Core.GetRegion( region );
        }

        public virtual void OnKill( Mobile killed )
        {
            War.GiveExtraPoints( OwnerTeam, 1 );
        }

        public override string StatusDescription()
        {
            return "Kills: " + CurProgress;
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