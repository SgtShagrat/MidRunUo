/***************************************************************************
 *                                  .cs
 *                            		-------------------
 *  begin                	: Mese, 2000
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info
 * 
 ***************************************************************************/

namespace Server.Items
{
    public abstract class BaseWellAddon : BaseAddon
    {
        #region properties
        public abstract Point3D SatchelLocation { get; }
        #endregion

        #region fields
        //		private WellSatchel m_Satchel;
        #endregion

        #region constructors
        public BaseWellAddon()
        {
            //			m_Satchel = new WellSatchel( this );
        }

        public BaseWellAddon( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region members
        //		public override void OnAfterSpawn()
        //		{
        ////			m_Satchel = new WellSatchel( this );
        //			
        //			base.OnAfterSpawn();
        //		}
        //		
        //		public override void OnLocationChange( Point3D oldLocation )
        //		{
        //			if( m_Satchel != null )
        //				m_Satchel.Location = new Point3D( X + SatchelLocation.X, Y + SatchelLocation.Y, Z + SatchelLocation.Z );
        //		}
        //
        //		public override void OnMapChange()
        //		{
        //			if( m_Satchel != null )
        //				m_Satchel.Map = Map;
        //		}
        //
        //		public override void OnAfterDelete()
        //		{
        //			base.OnAfterDelete();
        //
        //			if( m_Satchel != null && !m_Satchel.Deleted )
        //				m_Satchel.Delete();
        //		}
        #endregion

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            //			writer.Write( m_Satchel );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            //			m_Satchel = reader.ReadItem() as WellSatchel;
        }
        #endregion

        #region WellSatchel
        //		internal class WellSatchel : BaseBeverage
        //		{
        //			#region fields
        //			private BaseWellAddon m_Well;
        //			#endregion
        //			
        //			#region properties
        //			public override  int EmptyLabelNumber{ get{ return 1065771; } } // empty watering can
        //			public override  int BaseLabelNumber{ get{ return 1065770; } } 	// watering can
        //			public override int MaxQuantity{ get{ return 10; } }
        //			
        //			public override int LabelNumber
        //			{
        //				get
        //				{
        //					int num = BaseLabelNumber;
        //		
        //					if( IsEmpty )
        //						return EmptyLabelNumber;
        //		
        //					return BaseLabelNumber;
        //				}
        //			}
        //			
        //			public BaseWellAddon Well
        //			{
        //				get{ return m_Well; }
        //			}
        //			#endregion
        //			
        //			#region members
        //			public override int ComputeItemID()
        //			{
        //				return 0xFFA;
        //			}
        //			
        //			public override void Pour_OnTarget( Mobile from, object targ )
        //			{
        //				if( from == null || targ == null )
        //					return;
        //				
        //				if ( !ValidateUse( from, true ) )
        //					return;
        //					
        //				if( targ is BaseWellAddon && (BaseWellAddon)targ == targ )
        //				{
        //					if( IsFull )
        //					{
        //						from.SendMessage( "This satchel is already full of water." );
        //					}
        //					else
        //					{
        //						Quantity = MaxQuantity;
        //						from.SendMessage( "This satchel is now full of water." );
        //					}
        //				}
        //				else if ( from == targ && from.Thirst < 20 )
        //				{
        //					string msg = null;
        //					
        //					switch( Utility.Random( 5 ) )
        //					{
        //						case 0:  
        //							msg = "You drink your fill of the cool well water. The quiet sounds of splashing water are softly musical."; break;
        //						case 1:  
        //							msg = "The well's invigorating water refreshes you and sets your mind at ease. You drink your fill."; break;
        //						case 2:  
        //							msg = "You drink deeply of the clean well water. The shimmering reflections on the surface stir your thoughts."; break;
        //						case 3:  
        //							msg = "As you drink from the water, an tantalizing scent reminds you of memories long forgotten."; break;
        //						case 4:  
        //							msg = "You drink from the pure well and quiet dreams of sylvan delight pass through your mind."; break;
        //					}
        //						
        //					from.SendMessage( msg );
        //					from.Thirst = 20;
        //				}
        //				else
        //				{
        //					base.Pour_OnTarget( from, targ );
        //				}
        //			}
        //			
        //			public override void OnLocationChange( Point3D oldLocation )
        //			{
        //				if( m_Well != null )
        //					m_Well.Location = new Point3D( X - m_Well.SatchelLocation.X, Y - m_Well.SatchelLocation.Y, Z - m_Well.SatchelLocation.Z );
        //			}
        //		
        //			public override void OnMapChange()
        //			{
        //				if( m_Well != null )
        //					m_Well.Map = Map;
        //			}
        //		
        //			public override void OnAfterDelete()
        //			{
        //				base.OnAfterDelete();
        //		
        //				if( m_Well != null )
        //					m_Well.Delete();
        //			}
        //			#endregion
        //			
        //			#region costruttori
        //			[Constructable]
        //			public WellSatchel( BaseWellAddon well ) : base( BeverageType.Water )
        //			{
        //				Weight = 2.0;
        //				
        //				m_Well = well;
        //				
        //	            if( m_Well != null )
        //	            {
        //	            	if( m_Well.Map != Map.Internal )
        //	            		MoveToWorld( new Point3D( X + m_Well.SatchelLocation.X, Y + m_Well.SatchelLocation.Y, Z + m_Well.SatchelLocation.Z ), m_Well.Map );
        //	            }
        //			}
        //		
        //			public WellSatchel( Serial serial ) : base( serial )
        //			{
        //			}
        //			#endregion
        //			
        //			#region serial-deserial
        //			public override void Serialize( GenericWriter writer )
        //			{
        //				base.Serialize( writer );
        //		
        //				writer.Write( (int) 0 ); // version
        //				
        //				writer.Write( m_Well );
        //			}
        //		
        //			public override void Deserialize( GenericReader reader )
        //			{
        //				base.Deserialize( reader );
        //		
        //				int version = reader.ReadInt();
        //				
        //				m_Well = reader.ReadItem() as BaseWellAddon;
        //			}
        //			#endregion
        //		}
        #endregion
    }

    public class WorkingWellAddon : BaseWellAddon
    {
        #region campi
        public override Point3D SatchelLocation { get { return new Point3D( 2, 0, 9 ); } }
        #endregion

        #region costruttori
        [Constructable]
        public WorkingWellAddon()
        {
            AddComponent( new AddonComponent( 9156 ), 2, 1, 15 );
            AddComponent( new AddonComponent( 3348 ), 0, 1, 3 );
            AddComponent( new AddonComponent( 9358 ), 0, 0, 0 );
            AddComponent( new AddonComponent( 9364 ), 0, 0, 5 );
            AddComponent( new AddonComponent( 6008 ), 2, -1, 0 );
            AddComponent( new AddonComponent( 3244 ), 2, -1, 0 );
            AddComponent( new AddonComponent( 9364 ), 0, -1, 5 );
            AddComponent( new AddonComponent( 9158 ), 1, 1, 15 );
            AddComponent( new AddonComponent( 3248 ), 1, 1, 0 );
            AddComponent( new AddonComponent( 9357 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 9364 ), 1, 0, 5 );
            AddComponent( new AddonComponent( 6039 ), 1, 0, 0 );
            AddComponent( new AddonComponent( 9158 ), 1, 0, 15 );
            AddComponent( new AddonComponent( 9156 ), 2, 0, 15 );
            AddComponent( new AddonComponent( 6007 ), 2, 0, 0 );
            //			AddComponent( new AddonComponent( 4090 ), 2, 0, 9 );
            AddComponent( new AddonComponent( 7840 ), 2, 0, 4 );
            AddComponent( new AddonComponent( 3244 ), 2, 0, 0 );
            AddComponent( new AddonComponent( 3347 ), 2, 0, 3 );
            AddComponent( new AddonComponent( 7070 ), -1, 0, 0 );
            AddComponent( new AddonComponent( 9359 ), 1, -1, 0 );
            AddComponent( new AddonComponent( 9364 ), 1, -1, 5 );
        }

        public WorkingWellAddon( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region serial-deserial
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
        #endregion
    }
}
