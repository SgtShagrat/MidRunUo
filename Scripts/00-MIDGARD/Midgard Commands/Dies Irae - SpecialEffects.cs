/***************************************************************************
 *                                    SpecialEffects.cs
 *                            		---------------------
 *  begin                	: Aprile, 2007
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *
 * 	Info:
 *  		Con il comando [SpecialEffects si possono ottenere splendidi 
 * 			effetti visivi di vario genere.
 * 			Uso del comando:
 * 				[SpecialEffect <Tipo di effetto> <Parametri aggiuntivi>
 * 			Le tipologie di effetto con i rispettivi argomenti sono:
 * 			Flame 		h	|	u	|	k	|	t	|	a
 *			Explosion	h	|	u	|	k	|	t	|	a
 *			Sparkle1	h	|	u	|	k	|	t	|	a
 *			Sparkle2	h	|	u	|	k	|	t	|	a
 *			Sparkle3	h	|	u	|	k	|	t	|	a
 *			Sparkle4	h	|	u	|	k	|	t	|	a
 *			Sparkle5	h	|	u	|	k	|	t	|	a
 *			Sparkle6	h	|	u	|	k	|	t	|	a
 *			Sparkle7	h	|	u	|	k	|	t	|	a
 *			Sparkle8	h	|	u	|	k	|	t	|	a
 *			Sparkle9	h	|	u	|	k	|	t	|	a
 *			Gate		x		y		z 		Map
 * 			
 * 			Gli argomenti sono:
 * 			h:		Hidda il pg che usa il comando con l'effetto desiderato.
 * 			u:		Unhidda il pg che usa il comando con l'effetto desiderato.	
 * 			k: 		Killa il target con l'effetto desiderato.
 * 			t: 		Spawna l'effetto sul target.
 * 			a: 		Spawna nel raggio di +/- 5 tiles l'effetto desiderato.
 * 
 ***************************************************************************/
 
using System;
using Server;
using Server.Commands;
using Server.Items;
using Server.Misc;
using Server.Targeting;

namespace Midgard.Commands
{
	public class SpecialEffects
	{
		#region registrazione
		public static void Initialize()
		{
			CommandSystem.Register( "SpecialEffects", AccessLevel.GameMaster, new CommandEventHandler( SpecialEffects_OnCommand ) );
			CommandSystem.Register( "SE"			, AccessLevel.GameMaster, new CommandEventHandler( SpecialEffects_OnCommand ) );
		}
		#endregion
		
		#region enumerazioni
		/// <summary>
		/// Tipi di Effetti: in particolare Area presuppone un secondo effetto e Gate presuppone i parametri per aprire il gate.
		/// </summary>
		public enum EffectType
		{
			None,
			Flame,
			Explosion,
			Sparkle1,
			Sparkle2,
			Sparkle3,
			Sparkle4,
			Sparkle5,
			Sparkle6,
			Sparkle7,
			Sparkle8,
			Sparkle9,
			Gate
		}
		
		/// <summary>
		/// Sottotipi di effetti: c hidda, u rivela, k killa, t aggiunge un effetto via target.
		/// In particolare se come effetto principale e' stato usato Sparkle s1-s9 sono i tipi di sparkle addabili.
		/// </summary>
		private enum EffectSubType
		{
			None,
			h,		// Argomento per hiddare
			u,		// Argomento per un-hiddare
			k,		// Argomento per killare
			t,		// Argomento per targettare un punto\mobile dove far apparire l'effetto
			a		// Argomento per castare l'effetto ad area
		}
		#endregion
		
		#region metodi
		/// <summary>
		/// Crea un effetto in locazione (x,y) sul target from.
		/// </summary>
		private static void DrawFirework( Mobile from, int x, int y, EffectType type ) 
		{ 
			int intEffectID = EffectInfo.GetInfo(type).ItemID;
			Effects.SendLocationEffect( new Point3D( x, y, from.Z + 20 ), from.Map, intEffectID, 90 ); 
		} 
		
		/// <summary>
		///  Apre un gate in locazione (x, y, z) sulla mappa map.
		/// </summary>
		private static void OpenGate( Mobile from, int x, int y, int z, Map map )
		{
			Map MapGateIn = from.Map;
			Map MapGateOut = map;
			
			Point3D LocGateIn = from.Location;
			Point3D LocGateOut = new Point3D( x, y, z );
			
			Effects.PlaySound( LocGateIn, MapGateIn, 0x20E );

			// Crea il primo gate su LocGateIn con destinazione LocGateOut
			InternalItem FirstGate = new InternalItem( LocGateOut, MapGateOut );
			FirstGate.MoveToWorld( LocGateIn, MapGateIn );

			// Crea il secondo gate su LocGateOut con destinazione LocGateIn
			InternalItem SecondGate = new InternalItem( LocGateIn, MapGateIn );
			SecondGate.MoveToWorld( LocGateOut, MapGateOut );
		}
		#endregion
		
		[DispellableField]
		private class InternalItem : Moongate
		{
			#region campi
			public override bool ShowFeluccaWarning{ get{ return Core.AOS; } }
			#endregion
			
			#region costruttori
			public InternalItem( Point3D target, Map map ) : base( target, map )
			{
				Map = map;

				if( ShowFeluccaWarning && map == Map.Felucca )
					ItemID = 0xDDA;

				Dispellable = true;

				InternalTimer t = new InternalTimer( this );
				t.Start();
			}

			public InternalItem( Serial serial ) : base( serial )
			{
			}
			#endregion

			#region serialize-deserialize
			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );
				Delete();
			}
			#endregion
			
			private class InternalTimer : Timer
			{
				#region campi
				private Item m_Item;
				#endregion
				
				#region costruttori
				public InternalTimer( Item item ) : base( TimeSpan.FromSeconds( 10.0 ) )
				{
					Priority = TimerPriority.OneSecond;
					m_Item = item;
				}
				#endregion
				
				#region metodi
				protected override void OnTick()
				{
					m_Item.Delete();
				}
				#endregion
			}
		}
		
		/// <summary>
		///  Classe che permette dato il tipo di effetto di ritrovare sound e id
		/// </summary>
		public class EffectInfo
		{
			#region campi
			private EffectType m_Effect;
			private int m_ItemID;
			private int m_Sound;
			#endregion
			
			#region proprietà
			public EffectType Effect{ get{ return m_Effect; } }
			public int ItemID{ get{ return m_ItemID; } }
			public int Sound{ get{ return m_Sound; } }
			#endregion
			
			#region costruttori
			public EffectInfo( EffectType effect, int itemid, int sound )
			{
				m_Effect = effect;
				m_ItemID = itemid;
				m_Sound = sound;
			}
			#endregion
			
			#region metodi
			public static EffectInfo GetInfo( EffectType type )
			{
				int v = (int)type;
	
				if( v < 0 || v >= m_List.Length )
					v = 0;
	
				return m_List[v];
			}
			#endregion
			
			#region Lista di Effetti
			private static EffectInfo[] m_List = new EffectInfo[]
			{	
				new EffectInfo( EffectType.None, 		0, 		0 	  ),
				new EffectInfo( EffectType.Flame, 		0x3709, 0x208 ),
				new EffectInfo( EffectType.Explosion, 	0x36B0, 0x208 ),
				new EffectInfo( EffectType.Sparkle1, 	0x373A, 0x209 ),
				new EffectInfo( EffectType.Sparkle2, 	0x3728, 0x209 ),
				new EffectInfo( EffectType.Sparkle3, 	0x374A, 0x209 ),
				new EffectInfo( EffectType.Sparkle4, 	0x375A, 0x209 ),
				new EffectInfo( EffectType.Sparkle5, 	0x376A, 0x209 ),
				new EffectInfo( EffectType.Sparkle6, 	0x377A, 0x209 ),
				new EffectInfo( EffectType.Sparkle7, 	0x3789, 0x209 ),
				new EffectInfo( EffectType.Sparkle8, 	0x37C3, 0x209 ),
				new EffectInfo( EffectType.Sparkle9, 	0x37CC, 0x209 ),		                                                                                                                                                                    
			};
			#endregion
		}
		
		/// <summary>
		///  Target per killare un bersaglio qualsiasi vivo.
		/// </summary>
		private class KillTarget : Target 
		{
			#region campi
			private EffectType m_Type;
			#endregion
			
			#region costruttori
			public KillTarget( EffectType type ) : base( -1, true, TargetFlags.None )
			{
				m_Type = type;
			}
			#endregion
			
			#region metodi
			protected override void OnTarget( Mobile from, object o )
			{
				Mobile target = o as Mobile;

				if( target == null )
				{
					Console.WriteLine( "You must target a player or a mobile!" ); 
					return;
				}
				
				int intEffectID = EffectInfo.GetInfo(m_Type).ItemID;
				int intSoundID = EffectInfo.GetInfo(m_Type).Sound;
						
				Effects.SendLocationEffect( target.Location, from.Map, intEffectID , 30 );
				target.PlaySound( intSoundID );
				target.Kill();
			}
			#endregion
		}
		
		/// <summary>
		/// Target per addare un effetto in una data locazione (sia su mobile sia in un punto 3d).
		/// </summary>
		private class AddTarget : Target
		{
			#region campi
			private EffectType m_Type;
			#endregion

			#region costruttori
			public AddTarget( EffectType type ) : base( -1, true, TargetFlags.None )
			{
				m_Type = type;
			}
			#endregion
			
			#region metodi
			protected override void OnTarget( Mobile from, object o )
			{
				IPoint3D target = o as IPoint3D;

				if( target == null )
					return;
				
				int intEffectID = EffectInfo.GetInfo(m_Type).ItemID;
				int intSoundID = EffectInfo.GetInfo(m_Type).Sound;
				
				if( target != null )
				{
					if ( target is Item )
						target = ((Item)target).GetWorldTop();
					else if ( target is Mobile )
						target = ((Mobile)target).Location;
				}

				Effects.SendLocationEffect( new Point3D( target.X + 1, target.Y, target.Z ), from.Map,  intEffectID , 30 );
				from.PlaySound( intSoundID );
			}
			#endregion
		}
		
		[Usage( "[SpecialEffect <EffectType> <SubEffectType>" )]
		[Description( "Cast a SpecialEffect" )]
		private static void SpecialEffects_OnCommand( CommandEventArgs e )
		{
			EffectType m_Type  = EffectType.None;
			EffectSubType m_SubType = EffectSubType.None;
			
			Mobile from = e.Mobile; 
			
			string arg1 = e.GetString( 0 );
			string arg2 = e.GetString( 1 );

			if( arg1 != string.Empty && Enum.IsDefined( typeof(EffectType), arg1 ) )
				m_Type = (EffectType)Enum.Parse( typeof(EffectType), arg1 );
			
			if( arg2 != string.Empty && Enum.IsDefined( typeof(EffectSubType), arg2 ) )
				m_SubType = (EffectSubType)Enum.Parse( typeof(EffectSubType), arg2 );
			
			switch( m_Type )
			{
				case EffectType.Flame:	
				case EffectType.Explosion:
				case EffectType.Sparkle1:
				case EffectType.Sparkle2:
				case EffectType.Sparkle3:
				case EffectType.Sparkle4:
				case EffectType.Sparkle5:
				case EffectType.Sparkle6:
				case EffectType.Sparkle7:
				case EffectType.Sparkle8:
				case EffectType.Sparkle9:
					switch( m_SubType )
					{	
						case EffectSubType.h:
							Effects.SendLocationEffect( new Point3D( from.X + 1, from.Y, from.Z + 4 ), from.Map, EffectInfo.GetInfo(m_Type).ItemID, 30 );
							from.Hidden = true; from.PlaySound( EffectInfo.GetInfo(m_Type).Sound ); break;
						case EffectSubType.u:
							Effects.SendLocationEffect( new Point3D( from.X + 1, from.Y, from.Z ), from.Map,  EffectInfo.GetInfo(m_Type).ItemID, 30 );
							from.Hidden = false; from.PlaySound( EffectInfo.GetInfo(m_Type).Sound ); break;
						case EffectSubType.t:
							from.Target = new AddTarget( m_Type ); break;
						case EffectSubType.k:
							from.Target = new KillTarget( m_Type ); break;
						case EffectSubType.a:
							for( int i = 0; i < 10; i++ )
							{
								DrawFirework( from, from.X + Utility.RandomMinMax( -5, 5 ), from.Y + Utility.RandomMinMax( -5, 5 ), m_Type ); 
								TimeSpan.FromSeconds( 2.0 );
							}
							from.PlaySound( EffectInfo.GetInfo(m_Type).Sound ); break;						
						default:
							from.SendMessage( 37, "Uso del comando: [SpecialEffect <EffectType> <SubEffectType>" );
							break;
					}
					break;
					
				#region Effect: Gate
				case EffectType.Gate:
					int x = e.GetInt32( 1 );
					int y = e.GetInt32( 2 );
					int z = e.GetInt32( 3 );
					string tmp2 = e.GetString( 4 );
					Map map = Map.Parse( tmp2 );
					
					if( map != null )
					{
						OpenGate( from, x, y, z, map );
					}
					else
					{
						from.SendMessage( "You must select a map from : Felucca, Trammel, Ilshenar, Malas, Tokuno." );	
					}
					break;
				#endregion
				
				default:
					from.SendMessage( 37, "Uso del comando: [SpecialEffect <EffectType> <SubEffectType>" );
					break;
			} 			
		}
	}
}
