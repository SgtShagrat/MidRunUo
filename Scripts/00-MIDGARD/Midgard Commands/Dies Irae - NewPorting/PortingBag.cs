using System;

using Server;
using Server.Engines.BulkOrders;
using Server.Engines.Plants;
using Server.Items;

namespace Midgard.Items
{
    public class PortingBag : Bag
    {
        private readonly string m_User;

        [Constructable]
        public PortingBag( string user )
        {
            m_User = user;

            Name = "Sacca Porting da Midgard 2 a Midgard 3rd Crown";
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            if( m_User != null )
                list.Add( "username: " + m_User );
        }

        public override bool OnDragDrop( Mobile from, Item dropped )
        {
            Item item = dropped;

            if( item != null && base.OnDragDrop( from, dropped ) )
            {
                if( item is BaseContainer )
                {
                    from.SendMessage( "Non puoi inserire contenitori nella cassa." );
                    return false;
                }

                if( item.ItemID == 0x14F0 &&
                    item.GetType() != typeof( BankCheck ) &&
                    item.GetType() != typeof( PowerScroll ) &&
                    item.GetType() != typeof( StatCapScroll ) )
                {
                    from.SendMessage( "Non puoi inserire Deeds nella cassa." );
                    return false;
                }

                if( item.Stackable )
                {
                    from.SendMessage( "Non puoi inserire oggetti impilabili." );
                    return false;
                }

                foreach( Type t in m_TypeList )
                {
                    if( item.GetType() == t )
                    {
                        from.SendMessage( "Non puoi inserire questo tipo di oggetti nella cassa: " + item.GetType().Name );
                        return false;
                    }
                }

                from.SendMessage( "Hai inserito : " + item.GetType().Name );
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool OnDragDropInto( Mobile from, Item dropped, Point3D p )
        {
            Item item = dropped;

            if( item != null && base.OnDragDropInto( from, dropped, p ) )
            {
                if( item is BaseContainer )
                {
                    from.SendMessage( "Non puoi inserire contenitori nella cassa." );
                    return false;
                }

                if( item.ItemID == 0x14F0 &&
                    item.GetType() != typeof( BankCheck ) &&
                    item.GetType() != typeof( PowerScroll ) &&
                    item.GetType() != typeof( StatCapScroll ) )
                {
                    from.SendMessage( "Non puoi inserire Deeds nella cassa." );
                    return false;
                }

                if( item.Stackable )
                {
                    from.SendMessage( "Non puoi inserire oggetti impilabili." );
                    return false;
                }

                foreach( Type t in m_TypeList )
                {
                    if( item.GetType() == t )
                    {
                        from.SendMessage( "Non puoi inserire questo tipo di oggetti nella cassa: " + item.GetType().Name );
                        return false;
                    }
                }

                from.SendMessage( "Hai inserito : " + item.GetType().Name );
                return true;
            }
            else
            {
                return false;
            }
        }

        private static readonly Type[] m_TypeList = new Type[]
                                              {
            // Bulk Orders e Libri
      		typeof(SmallSmithBOD),
      		typeof(SmallTailorBOD),
      		typeof(LargeSmithBOD),
      		typeof(LargeTailorBOD),    		                            
       		typeof(BulkOrderBook),
       		
       		// GraniteBox e SeedBox
      		typeof(GraniteBox),     		                            
       		typeof(SeedBox),
       		
       		// Runebook
       		typeof(Runebook),
       		
       		// Oro non in Bankcheck
       		typeof(Gold),
       		
       		// Navi
       		typeof(ShipwreckedItem),
       		
            typeof(BasePotion),
            typeof(BaseIngot),
            typeof(BaseLog),
            typeof(BaseOre),

       		// Regs Pagani
       		typeof(ExecutionersCap),
       		typeof(ZoogiFungus),
    		
       		// Vari
			 typeof(KeyRing),
       		typeof(Seed),
			typeof(TreasureMap),
       		
       		// Libri di fede
       		typeof(BookOfChivalry),
       		typeof(NecromancerSpellbook),
                                              };

        public PortingBag( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
}