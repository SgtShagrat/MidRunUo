/***************************************************************************
 *                               CraftDeliverObjective.cs
 *
 *   begin                : 11 maggio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;

using Server;
using Server.Engines.Quests;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Engines.CraftQuests
{
    public class CraftDeliverObjective : DeliverObjective
    {
        public CraftResource Resource { get; set; }

        public CraftDeliverObjective( Type delivery, String deliveryName, CraftResource resource, int amount, Type destination, String destName )
            : this( delivery, deliveryName, resource, amount, destination, destName, 0 )
        {
        }

        public CraftDeliverObjective( Type delivery, String deliveryName, CraftResource resource, int amount, Type destination, String destName, int seconds )
            : base( delivery, deliveryName, amount, destination, destName, seconds )
        {
            Resource = resource;
        }

        public override bool Update( object obj )
        {
            if( Delivery == null || Destination == null )
                return false;

            if( Failed )
            {
                Quest.Owner.SendLocalizedMessage( 1074813 );  // You have failed to complete your delivery.
                return false;
            }

            if( obj is Mobile )
            {
                if( Quest.StartingItem != null )
                {
                    Complete();
                    return true;
                }
                else if( Destination.IsAssignableFrom( obj.GetType() ) )
                {
                    if( MaxProgress < CountQuestItems( Quest.Owner, Delivery, Resource ) )
                    {
                        Quest.Owner.SendLocalizedMessage( 1074813 );  // You have failed to complete your delivery.						
                        Fail();
                    }
                    else
                        Complete();

                    return true;
                }
            }

            return false;
        }

        private static int CountQuestItems( PlayerMobile from, Type type, CraftResource craftResource )
        {
            int count = 0;

            if( from.Backpack == null )
                return count;

            Item[] items = from.Backpack.FindItemsByType( type );

            foreach( Item item in items )
            {
                if( item.QuestItem && GetResource( item ) == craftResource )
                    count += item.Amount;
            }

            return count;
        }

        private static CraftResource GetResource( Item item )
        {
            if( item is BaseArmor )
                return ( (BaseArmor)item ).Resource;
            else if( item is BaseWeapon )
                return ( (BaseWeapon)item ).Resource;
            else if( item is BaseJewel )
                return ( (BaseJewel)item ).Resource;

            return CraftResource.None;
        }

        #region serialization
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.WriteEncodedInt( 0 ); // version

            writer.Write( (int)Resource );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadEncodedInt();

            Resource = (CraftResource)reader.ReadInt();
        }
        #endregion
    }
}