/***************************************************************************
 *                               BaseScoutTentRoll.cs
 *
 *   begin                : 08 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Midgard.Engines.Classes;

using Server;
using Server.Multis.Deeds;

namespace Midgard.Items
{
    public abstract class BaseScoutTentRoll : HouseDeed
    {
        protected BaseScoutTentRoll( int id, Point3D offset )
            : base( id, offset )
        {
            ItemID = 0xA57;
        }

        public override Rectangle2D[] Area
        {
            get { return ScoutTent.AreaArray; }
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !IsChildOf( from.Backpack ) )
            {
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
            }
            else if( from.AccessLevel < AccessLevel.GameMaster && !ClassSystem.IsScout( from ) )
            {
                from.SendMessage( "Only scouts can open this rolled tent." );
            }
            else
            {
                from.Target = new HousePlacementTarget( this );
            }
        }

        #region serialization
        public BaseScoutTentRoll( Serial serial )
            : base( serial )
        {
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
        #endregion
    }
}