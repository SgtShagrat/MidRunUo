/***************************************************************************
 *                               RitualItem.cs
 *
 *   revision             : 03 January, 2010
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Mobiles;

using MidgardClasses = Midgard.Engines.Classes.Classes;

namespace Midgard.Engines.Classes
{
    public abstract class RitualItem : Item
    {
        public RitualItem( int itemID )
            : base( itemID )
        {
        }

        public RitualItem( Serial serial )
            : base( serial )
        {
        }

        public abstract MidgardClasses Class { get; }

        public virtual bool Accepted
        {
            get { return Deleted; }
        }

        public abstract bool CanDrop( PlayerMobile pm );

        /*
        public override bool DropToWorld( Mobile from, Point3D p )
        {
            bool ret = base.DropToWorld( from, p );

            if( ret && !Accepted && Parent != from.Backpack )
            {
                if( from.AccessLevel > AccessLevel.Player )
                {
                    return true;
                }
                else if( !( from is PlayerMobile ) || CanDrop( (PlayerMobile)from ) )
                {
                    return true;
                }
                else
                {
                    from.SendMessage( "You can only drop ritual items into the top-most level of your backpack or in a bag inside it." );
                    return false;
                }
            }
            else
            {
                return ret;
            }
        }
        */

        /*
        public override bool DropToMobile( Mobile from, Mobile target, Point3D p )
        {
            bool ret = base.DropToMobile( from, target, p );

            if( ret && !Accepted && Parent != from.Backpack )
            {
                if( from.AccessLevel > AccessLevel.Player )
                {
                    return true;
                }
                else if( !( from is PlayerMobile ) || CanDrop( (PlayerMobile)from ) )
                {
                    return true;
                }
                else
                {
                    from.SendMessage( "You decide against trading the sacred item or in a bag inside it." );
                    return false;
                }
            }
            else
            {
                return ret;
            }
        }
        */

        /*
        public override bool DropToItem( Mobile from, Item target, Point3D p )
        {
            bool ret = base.DropToItem( from, target, p );

            if( ret && !Accepted && Parent != from.Backpack )
            {
                if( from.AccessLevel > AccessLevel.Player )
                {
                    return true;
                }
                else if( !( from is PlayerMobile ) || CanDrop( (PlayerMobile)from ) )
                {
                    return true;
                }
                else if( target.IsChildOf( from.Backpack ) )
                {
                    return true;
                }
                else
                {
                    from.SendMessage( "You can only drop ritual items into the top-most level of your backpack or in a bag inside it." );
                    return false;
                }
            }
            else
            {
                return ret;
            }
        }
        */

        public override DeathMoveResult OnParentDeath( Mobile parent )
        {
            if( parent is PlayerMobile && !CanDrop( (PlayerMobile)parent ) )
                return DeathMoveResult.MoveToBackpack;
            else
                return base.OnParentDeath( parent );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}