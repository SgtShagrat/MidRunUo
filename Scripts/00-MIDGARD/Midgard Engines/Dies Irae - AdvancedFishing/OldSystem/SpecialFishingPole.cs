// ToDo:
// + Influenzare Forza pesce in base ai riflessi
// + gestione grafica dei salti dei pesci 
// + gestione pasturazione (basta incrementare nell'harvest resources) Utility.RandomMinMax( fish.MinTotal, fish.MaxTotal );
// + oggetti speciali ?
// + Gestione della frequenza abboccate in base alla pasturazione..
// + ondeath del pg che succede ?
//
// ToTest:
// + random sul colpo ok
// + Salita della skill
// + creazione dei pesci
// + Spostamento pg durante fishing
// + gestione harvestsystem
// + con il fish str 10% che non si slami e riprenda forza su azione sbagliata.. +2
// + croce alto mare
// 	+ THE BIG FISH  
// + randomize dei messaggi
// + ATTESA ABBOCCATA
// + get props del pesce
// + carve del pesce

/*
using System;
using System.Collections;
using Server.Targeting;
using Server.Items;
using Server.Engines.Harvest;
using Server.Gumps;
using Server.Network;
using Server.Spells;
using Server.Mobiles;
using System.IO;
using Server.Scripts.Commands;


namespace Server.Items
{
	public class SpecialFishingPole : Item
	{
		public int action_timer;
		public int action_gump;
		public InternalFishingAITimer AI_Timer=null;
		public DateTime m_LastAction;
		public DateTime m_LastFishAction;
		private int m_uses=0;
		// controllo se puo' equippare ?
		
		[CommandProperty( AccessLevel.Seer )] 
		public int uses
		{ 
		 	get{ return m_uses; } 
		 	set{ m_uses = value; } 
		}

		public override bool CanEquip( Mobile from )
		{
			if ( from.Dex < 25 )
			{
				from.SendMessage( "You are not nimble enough to equip that." );
				return false;
			} 
			else if ( from.Str < 50 )
			{
				from.SendMessage( "You are not strong enough to equip that." );
				return false;
			}
			else if ( !from.CanBeginAction( typeof( SpecialFishingPole ) ) )
			{
				return false;
			}
			else
			{
				return base.CanEquip( from );
			}
		}
		public override void OnRemoved( object parent )
		{
			base.OnRemoved( parent );
			if (parent is PlayerMobile)
			{
				PlayerMobile pm= (PlayerMobile)parent;
				pm.CloseGump( typeof( FishingGump ) );
			}
		}
						
		// inizio a pescare...
		public override void OnDoubleClick( Mobile from )
		{
			if (m_uses==0)
				this.Delete();			
			else if(m_uses>0)
				m_uses--;
				
			if (AI_Timer!=null)
			{
				from.SendMessage( "Azione di pesca interrotta." );
				AI_Timer.Stop();
				AI_Timer=null;
				return;
			}
			else if (from.Skills[SkillName.Fishing].Value<50)
			{
				from.SendMessage( "Non sei abbastanza bravo nell'arte della pesca!" );
				return;
			}			
			else if (from.Mounted)
			{
				from.SendMessage( "You must dismount!" );
				return;
			}
			else
			{			
				from.BeginTarget( -1, true, TargetFlags.None, new TargetCallback( SpecialFishing_OnTarget ) );
				from.SendMessage( "Dove vuoi tirare ?" );
			}
		}

		// GESTIONE TARGET DI FISHING******************************************************************************************
		private static void SpecialFishing_OnTarget( Mobile from, object targeted)
		{
			try{
				// Attenzione crasha in circostanze strane..su target di item
				int tileID = 0;
				Point3D target_point = Point3D.Zero;
				
				//Console.WriteLine("TARGET: {0}",targeted);
				
				if (targeted is StaticTarget)				
				{
					tileID = ((((StaticTarget)targeted).ItemID & 0x3FFF) | 0x4000);
					target_point= ((StaticTarget)targeted).Location;
				}
				else if ((targeted is LandTarget))	
				{
					target_point= ((LandTarget)targeted).Location;
					tileID = ((LandTarget)targeted).TileID;
				}
				
				//Console.WriteLine("tileID: {0}",tileID);
				
				Item therod = from.FindItemOnLayer( Layer.OneHanded );
				
				if (!(therod is SpecialFishingPole))
				{
					from.SendMessage( "You must equip the rod during fishing!!" );
					return;
				}
				SpecialFishingPole TheSpecialRod = (SpecialFishingPole) therod;
				
				if ( targeted is StaticTarget || (targeted is LandTarget) )
				{
					// fishing avanzata !											
					if (from.GetDistanceToSqrt(target_point)>9)
					{
						// troppo lontano	
						from.SendMessage( "Troppo Lontano!" );
						return;
					}
					
					if (!ValidateDeepWater( tileID ))
					{
						from.SendMessage( "Non e' acqua!" );
						return;	
					}
					Map map=from.Map;
					// gira verso il punto targettato..
					SpellHelper.Turn( from, target_point );
					
					InternalTimer t=new InternalTimer(map,target_point);
					t.Start();
					
					from.Animate( 12, 5, 1, true, false, 0 );
					from.CloseGump( typeof( FishingGump ) );
					from.SendGump(new FishingGump( 1060635, 30720, 1049583, 32512, 220, 220, TheSpecialRod , new FishingGumpCallback( Fishing_Callback)) );
					// tutto ok e' acqua e ho mandato il gump
					//
					// inizio randomizzazione della pesca..
					// AI FISH timer start.
					//
					// check if ai timer != null
					TheSpecialRod.m_LastAction=DateTime.Now;
					TheSpecialRod.m_LastFishAction=DateTime.Now;
	
					TheSpecialRod.AI_Timer=new InternalFishingAITimer(from,TheSpecialRod,target_point,tileID);
					TheSpecialRod.AI_Timer.Start();
				}
				else
				{
					// target non valido	
					from.SendMessage( "Target non valido !" );
					TheSpecialRod.AI_Timer=null;
					return;
				}			
			}catch ( Exception ex )
			{				
				Console.WriteLine("SPECIALFISHING TARGET: {0}",ex.Message);
			}

		}// FINE GESTIONE TARGET DI FISHING******************************************************************************************
		
		
		// GESTIONE CALL BACK DAL GUMP ******************************************************************************************
		private static void Fishing_Callback( Mobile from, int action,SpecialFishingPole TheSpecialRod )
		{
			//Console.WriteLine("ACTION {0}",action);
			// questa funzione viene chiamata dal fishing gump
			// setta l'action sull'item canna usata per il dialogo tra pesce / pg / timer ai
			if ( from.CheckAlive() )
			{
				TheSpecialRod.action_gump=action;
				TheSpecialRod.m_LastAction=DateTime.Now;
				if (action == 1) 
				{
					from.Animate( 12, 5, 1, true, false, 0 );
					//Console.WriteLine("ACTION ALZA");
					from.CloseGump( typeof( FishingGump ) );
					from.SendGump(new FishingGump( 1060635, 30720, 1049583, 32512, 420, 280, TheSpecialRod , new FishingGumpCallback( Fishing_Callback)) );
				}
				else if (action == 2) 
				{
					from.Animate( 12, 5, 1, true, false, 0 );
					//Console.WriteLine("ACTION COMBATTI");
					from.CloseGump( typeof( FishingGump ) );
					from.SendGump(new FishingGump( 1060635, 30720, 1049583, 32512, 420, 280, TheSpecialRod , new FishingGumpCallback( Fishing_Callback)) );
				}
				else if (action == 3) 
				{
					from.Animate( 12, 5, 1, true, false, 0 );
					//Console.WriteLine("ACTION ABBASSA");
					from.CloseGump( typeof( FishingGump ) );
					from.SendGump(new FishingGump( 1060635, 30720, 1049583, 32512, 420, 280, TheSpecialRod , new FishingGumpCallback( Fishing_Callback)) );
				}
				else
				{
					if (TheSpecialRod.AI_Timer!=null)
						from.SendGump(new FishingGump( 1060635, 30720, 1049583, 32512, 420, 280, TheSpecialRod , new FishingGumpCallback( Fishing_Callback)) );
				}
				
			}				
		}// FINE GESTIONE CALL BACK DAL GUMP ******************************************************************************************

		// GESTIONE SPLASH ******************************************************************************************
		private class InternalTimer : Timer
		{
			private Point3D m_loc;
			private Map m_map;
			
			public InternalTimer( Map Mappa,Point3D Location ) : base( TimeSpan.FromSeconds( 0.9 ) )
			{
				m_loc = (Point3D)Location;
				m_map = (Map)Mappa;
			}

			protected override void OnTick()
			{
				Effects.SendLocationEffect( m_loc, m_map, 0x352D, 16, 4 );
				Effects.PlaySound( m_loc, m_map, 0x364 );
			}
		}// FINE GESTIONE SPLASH ******************************************************************************************

		// GESTIONE FISHING AI ******************************************************************************************
		public class InternalFishingAITimer : Timer
		{
			private Mobile m_from;
			private SpecialFishingPole m_pole=null;			
			private int m_fish_quiet=0;
			private int m_pg_bonus=0;
			private int m_war_stage=0;
			private int m_fish_action=0;			
			private int m_fish_size=0;
			private int m_fish_str=0;
			private int DEFAULT_WAIT_ACTION=30;
			private int MAX_FISH_SIZE=15000;
			private Point3D Start_location;
			private int HUE_BAD=26;
			private int HUE_GOOD=41;
			private int HUE_FISH=12;
			
			public InternalFishingAITimer( Mobile from,SpecialFishingPole pole,Point3D Location,int tileID) : base( TimeSpan.FromSeconds( 0.4 ),TimeSpan.FromSeconds( 0.1 ) )
			{
				m_from = (Mobile) from;
				m_pole = (SpecialFishingPole) pole;				
				m_pg_bonus=0;
				m_war_stage=0;
				m_pole.action_gump=0;
				m_fish_action=0;
				
				Start_location=new Point3D(m_from.X,m_from.Y,m_from.Z);
				
				Fishing HarvestSystem=Fishing.System;
				HarvestDefinition def = HarvestSystem.Definition;
				HarvestBank bank = def.GetBank( from.Map, Location.X, Location.Y,Location.Z,tileID);				
				//Console.WriteLine("Pesci presenti : {0}",bank.Current);
				bank.Consume(def,1);
				
				//Console.WriteLine("Pesci presenti : {0}",bank.Current);
				if (bank.Current<=1)
				{
					m_from.SendMessage( HUE_FISH,"Qui sembra non ci sono piu' pesci !" );
					m_pole.action_gump=0;
					Stop();
					m_pole.AI_Timer=null;
					return;						
				}
				//double skillBase = from.Skills[SkillName.Fishing].Base;				
				//LA DIMENSIONE E' RANDOM IN BASE ALLA SKILL
				double skillValue = from.Skills[SkillName.Fishing].Value/100;
				skillValue = skillValue * Utility.RandomDouble();
				//Console.WriteLine("FishSizer {0}",skillValue);
				m_fish_size=(int)(MAX_FISH_SIZE * skillValue * Utility.RandomDouble());
				m_fish_size=(int)(m_fish_size * Utility.RandomDouble());
								
				// croce deep
				int croce_size=30;
				double deep_sea_moltiplier=2.1;
				
				if (!ValidateDeepWaterPoint(from.X , from.Y+croce_size,from.Map ))
						deep_sea_moltiplier=0.5;
				if (!ValidateDeepWaterPoint(from.X+croce_size , from.Y,from.Map ))
						deep_sea_moltiplier=0.5;
				if (!ValidateDeepWaterPoint(from.X , from.Y-croce_size,from.Map ))
						deep_sea_moltiplier=0.5;
				if (!ValidateDeepWaterPoint(from.X-croce_size , from.Y,from.Map ))
						deep_sea_moltiplier=0.5;																		
				
				//Console.WriteLine("Size multi: {0}",deep_sea_moltiplier);
				//Console.WriteLine("Fish Size {0}",m_fish_size);
				if (deep_sea_moltiplier==2.1)
				{
					// check skill alto mare.	
					if (from.Skills[SkillName.Fishing].Value<75)
					{
						m_from.SendMessage( HUE_FISH,"Non sei ancora pronto per questa pesca in alto mare !" );
						m_pole.action_gump=0;
						Stop();
						m_pole.AI_Timer=null;
						return;						
					}
				}
				m_fish_size=(int)(m_fish_size*deep_sea_moltiplier);
				if (m_fish_size<100)
					m_fish_size=100+Utility.Random(100);
				
				m_fish_str=(int)(m_fish_size / 1000);
				
				if (m_fish_str<=3)
					m_fish_str+=Utility.Random(3);

				//Console.WriteLine("Fish Size {0}",m_fish_size);
				//Console.WriteLine("Fish Str {0}",m_fish_str);								
							
				// MAX_FISH_SIZE=15000
				// 1*1*1 * MAX_SIZE * (1.2) * (2.1)=2.52*MAX_SIZE
				if (m_fish_size>25000)
				{
					if (Utility.RandomDouble()<0.2)
					{
						m_fish_size=(int)33000+Utility.Random(8000);
						m_fish_str=(int)((25000+Utility.Random(4000)) / 1000);						
						//m_from.PublicOverheadMessage( MessageType.Regular, 41, false, String.Format( "* You see an Incredible Fish !! *" ) );
					}
				}

				// QUESTO DELAY e' PROPORZIONALE AL NUMERO DI PESCI CHE CI SONO
				// TRA MINIMO E MAX A CHE PUNTO STO DEL BANCO ?
				//def.MinTotal = 5;
				//def.MaxTotal = 15;
				//Console.WriteLine("Pesci presenti : {0}-{1}-{2}",def.MinTotal,bank.Current,def.MaxTotal);
				int wait_moltiplier=(50*def.MaxTotal)/bank.Current;				
				m_fish_quiet=Utility.RandomMinMax( 30, wait_moltiplier );
				//Console.WriteLine("Wait for : {0}-{1}",m_fish_quiet,wait_moltiplier);
			}

			protected override void OnTick()
			{
				// GESTIONE FISH AI
				if ((m_war_stage>=1) && (m_pole.action_gump!=0) && (m_war_stage!=2))
				{
					m_fish_quiet=0;
				}
				m_fish_quiet--;
				//Console.WriteLine("tick AI TImer {0}",m_fish_quiet);
				if ((m_fish_str==0) && (m_war_stage<=1))
				{
					m_from.SendMessage( HUE_FISH,"Non sembra abboccare nulla qui." );
					m_pole.action_gump=0;
					Stop();
					m_pole.AI_Timer=null;
					return;
				}
				if ((m_fish_str==0) && (m_war_stage>=1))
				{					
					
					AdvFish TheFish=null;
					
					if ((m_fish_size>=0) && (m_fish_size<=600))
					{						
						switch ( Utility.Random( 5 ) )
						{
							case 0: TheFish=(AdvFish)new PiperFish(m_fish_size);
								break;
							case 1: TheFish=(AdvFish)new BleakFish(m_fish_size);
								break;
							case 2: TheFish=(AdvFish)new TropicalFish(m_fish_size);
								break;			
							case 3: TheFish=(AdvFish)new TenchFish(m_fish_size);
								break;
							case 4: TheFish=(AdvFish)new GranchioFish(m_fish_size);
								break;
						}
					}					
					else if ((m_fish_size>600) && (m_fish_size<=1500))
					{						
						switch ( Utility.Random( 6 ) )
						{
							case 0: TheFish=(AdvFish)new PiperFish(m_fish_size);
								break;
							case 1: TheFish=(AdvFish)new SalmonFish(m_fish_size);
								break;
							case 2: TheFish=(AdvFish)new TropicalFish(m_fish_size);
								break;			
							case 3: TheFish=(AdvFish)new TenchFish(m_fish_size);
								break;
							case 4: TheFish=(AdvFish)new GranchioFish(m_fish_size);
								break;
							case 5: TheFish=(AdvFish)new ChubFish(m_fish_size);
								break;					
						}
					}
					else if ((m_fish_size>1500) && (m_fish_size<=7000))
					{
						switch ( Utility.Random( 8 ) )
						{
							case 0: TheFish=(AdvFish)new SalmonFish(m_fish_size);
								break;
							case 1: TheFish=(AdvFish)new TropicalFish(m_fish_size);
								break;			
							case 2: TheFish=(AdvFish)new TenchFish(m_fish_size);
								break;
							case 3: TheFish=(AdvFish)new CatFish(m_fish_size);
								break;
							case 4: TheFish=(AdvFish)new SmallMouthFish(m_fish_size);
								break;				
							case 5: TheFish=(AdvFish)new ChubFish(m_fish_size);
								break;
							case 6: TheFish=(AdvFish)new MantaFish(m_fish_size);
								break;
							case 7: TheFish=(AdvFish)new CarpFish(m_fish_size);
								break;								
						}
					}
					else if ((m_fish_size>1500) && (m_fish_size<=15000))
					{
						switch ( Utility.Random( 8 ) )
						{
							case 0: TheFish=(AdvFish)new SalmonFish(m_fish_size);
								break;
							case 1: TheFish=(AdvFish)new TropicalFish(m_fish_size);
								break;			
							case 2: TheFish=(AdvFish)new TenchFish(m_fish_size);
								break;
							case 3: TheFish=(AdvFish)new CatFish(m_fish_size);
								break;
							case 4: TheFish=(AdvFish)new SmallMouthFish(m_fish_size);
								break;				
							case 5: TheFish=(AdvFish)new ChubFish(m_fish_size);
								break;
							case 6: TheFish=(AdvFish)new MantaFish(m_fish_size);
								break;
							case 7: TheFish=(AdvFish)new CarpFish(m_fish_size);
								break;								
						}
					}
					else if ((m_fish_size>15000) && (m_fish_size<=33000))
					{
						switch ( Utility.Random( 5 ) )
						{
							case 0: TheFish=(AdvFish)new SalmonFish(m_fish_size);
								break;
							case 1: TheFish=(AdvFish)new CatFish(m_fish_size);
								break;
							case 2: TheFish=(AdvFish)new SmallMouthFish(m_fish_size);
								break;				
							case 3: TheFish=(AdvFish)new MantaFish(m_fish_size);
								break;
							//case 4: TheFish=(AdvFish)new MarlinFish(m_fish_size);
							//	break;
							case 4: TheFish=(AdvFish)new CarpFish(m_fish_size);
								break;					
						}
					}
					
					else if ((m_fish_size>30000) && (m_fish_size<=70000))
					{
						switch ( Utility.Random( 4 ) )
						{
							case 0: TheFish=(AdvFish)new SalmonFish(m_fish_size);
								break;
							case 1: TheFish=(AdvFish)new MantaFish(m_fish_size);
								break;
							case 2: TheFish=(AdvFish)new MarlinFish(m_fish_size);
								break;								
							case 3: TheFish=(AdvFish)new MarlinFish(m_fish_size);
								break;			
						}
						TheFish=(AdvFish)new MarlinFish(m_fish_size);
					}
					
					m_from.PublicOverheadMessage( MessageType.Regular, HUE_GOOD, false, String.Format( "* Ha catturato un {0} di {1} grammi! *",TheFish.Name,m_fish_size) );
					
					if (!( m_from.Backpack != null && m_from.Backpack.TryDropItem( m_from, TheFish, true ) ))
						TheFish.MoveToWorld( Start_location,m_from.Map);

					m_pole.action_gump=0;
					Stop();
					m_pole.AI_Timer=null;
					return;
				}
				
				if ((m_war_stage==0) && (m_fish_quiet>0))
				{
					// ATTESA ABBOCCATA
					// bonus abboccata derivanti da piccoli spostamenti dell'esca
					// nella fase di attesa del pesce.
					// eventuale bonus pasturazione.
					// QUESTE AZIONI AGISCONO SUL TEMPO DI ATTESA E
					// IN PICCOLISSIMA PARTE SULLE DIMENSIONI
					m_pg_bonus=7;					
				}					
				else if ((m_war_stage==0) && (m_fish_quiet<=0) )
				{
					// ABBOCCATA			
					m_war_stage=1;				
					// TEMPO MASSIMO PER NON PERDERE IL PESCE
					m_fish_quiet=DEFAULT_WAIT_ACTION;
					m_pole.m_LastFishAction=DateTime.Now;
					m_pole.action_gump=0;
					//Console.WriteLine("tick AI TImer {0}<0",m_fish_quiet);
					m_from.SendMessage( HUE_FISH,"ABBOCCATA !" );					
				}
				else if ((m_war_stage==1) && (m_fish_quiet<=0))
				{
					// PRIMO COLPO ALLAMA IL PESCE
					// bonus abboccata derivanti da piccoli spostamenti dell'esca
					// nella fase di attesa del pesce.
					// eventuale bonus pasturazione.
					if (m_pole.action_gump!=1 && m_pole.action_gump!=0)
					{
						//m_from.SendMessage( "Pessima mossa ! Il pesce e' scappato." );
						m_from.SendMessage( HUE_BAD,m_pole.random_bad_mex() );						
						m_pole.action_gump=0;
						Stop();
						m_pole.AI_Timer=null;
					}
					else if (m_pole.action_gump==0)
					{
						//m_from.SendMessage( "Pessimi riflessi ! Il pesce e' scappato." );
						m_from.SendMessage( HUE_BAD,m_pole.random_bad_reflex_mex() );
						m_pole.action_gump=0;
						Stop();
						m_pole.AI_Timer=null;
					}
					else
					{
						if (m_pole.Check_Riflesso(m_from,m_pole,2.0))
						{
							m_from.SendMessage(HUE_GOOD, "Ottimo ! Il pesce e' agganciato!" );
							
							//m_from.CheckSkill( SkillName.Fishing, m_from.Skills[SkillName.Fishing].Value-10, m_from.Skills[SkillName.Fishing].Value+10 );
							//Console.WriteLine("checkskill:{0}",Server.Misc.SkillCheck.CheckSkill( m_from, m_from.Skills[SkillName.Fishing], m_pole, .5));
							Server.Misc.SkillCheck.CheckSkill( m_from, m_from.Skills[SkillName.Fishing], m_pole, .5);
							m_war_stage=2;							
							m_fish_quiet=Utility.RandomMinMax( 15, 35 );
							m_pole.m_LastFishAction=DateTime.Now;
							m_pole.action_gump=0;
							m_fish_action=0;
							m_fish_str--;
							if (m_fish_size>25000 && Utility.RandomDouble()<0.4)
							{
								//Console.WriteLine("incredibile fish size : {0}",m_fish_size);
								m_from.PublicOverheadMessage( MessageType.Regular, 41, false, String.Format( "* You see an Incredible Fish !! *" ) );
							}
						}
						else
						{
							//m_from.SendMessage( "Tardi ! Il pesce e' scappato." );
							m_from.SendMessage( HUE_BAD,m_pole.random_bad_reflex_mex() );
							Stop();
							m_pole.AI_Timer=null;
						}
					}
				}
				else if ((m_war_stage==2) && (m_fish_quiet<=0))
				{
					// AZIONE DEL PESCE
					m_fish_action=1+Utility.Random( 3 );
					m_fish_quiet=DEFAULT_WAIT_ACTION;
					m_pole.m_LastFishAction=DateTime.Now;
					m_pole.action_gump=0;
					m_war_stage=3;
					switch ( m_fish_action )
					{
						case 1: m_from.SendMessage( HUE_FISH, m_pole.RandomSpecials( m_pole.random_down_mex() ) );
							break;
						case 2: m_from.SendMessage( HUE_FISH, m_pole.RandomSpecials( m_pole.random_war_mex() ) );
							break;
						case 3: m_from.SendMessage( HUE_FISH, m_pole.RandomSpecials( m_pole.random_jump_mex() ) );
							break;							
					}
				}
				else if ((m_war_stage==3) && (m_fish_quiet<=0))
				{
					// REAZIONE DEL PESCATORE
					//Console.WriteLine("Forza restante al pesce {0}",m_fish_str);
					if (m_pole.action_gump==1 && m_fish_action!=1)
					{
						//Randomize mex						
						if (Utility.RandomDouble()>0.01)
						{
							//m_from.SendMessage( "Pessima mossa ! Il pesce e' scappato." );
							m_from.SendMessage( HUE_BAD, m_pole.random_bad_mex() );
							m_pole.action_gump=0;
							Stop();
							m_pole.AI_Timer=null;
							return;
						}
						else
						{
							m_fish_str+=3;
							m_from.SendMessage( HUE_BAD, "Pessima mossa ! Il pesce non e' scappato ma lottera' con piu' forza!" );
							return;
						}
					}
					if (m_pole.action_gump==2 && m_fish_action!=2)
					{
						if (Utility.RandomDouble()>0.01)
						{						
							//Randomize mex
							//m_from.SendMessage( "Pessima mossa ! Il pesce e' scappato." );
							m_from.SendMessage( HUE_BAD,m_pole.random_bad_mex() );
							m_pole.action_gump=0;
							Stop();
							m_pole.AI_Timer=null;
							return;
						}
						else
						{
							m_fish_str+=3;
							m_from.SendMessage( HUE_BAD, "Pessima mossa ! Il pesce non e' scappato ma lottera' con piu' forza!" );
							return;
						}
					}
					if (m_pole.action_gump==3 && m_fish_action!=3)
					{
						if (Utility.RandomDouble()>0.01)
						{
							//Randomize mex
							//m_from.SendMessage( "Pessima mossa ! Il pesce e' scappato." );
							m_from.SendMessage( HUE_BAD,m_pole.random_bad_mex() );
							m_pole.action_gump=0;
							Stop();
							m_pole.AI_Timer=null;
							return;
						}
						else
						{
							m_fish_str+=3;
							m_from.SendMessage( HUE_BAD,"Pessima mossa ! Il pesce non e' scappato ma lottera' con piu' forza!" );
							return;
						}
					}
					else if (m_pole.action_gump==0)
					{
						//Randomize mex
						//m_from.SendMessage( "Pessimi riflessi ! Il pesce e' scappato." );
						m_from.SendMessage( HUE_BAD, m_pole.random_bad_reflex_mex() );
						m_pole.action_gump=0;
						Stop();
						m_pole.AI_Timer=null;
						return;
					}
					else
					{
						if (m_pole.Check_Riflesso(m_from,m_pole,2.0))
						{
							if (Utility.RandomDouble()>0.01)
							{
								// CONTROLLO POSIZIONE PG
								// CANNA IN MANO ETC.
								if ( (Start_location.X!=m_from.X) || (Start_location.Y!=m_from.Y) || (Start_location.Z!=m_from.Z))
								{
									m_from.SendMessage( HUE_BAD,"L'arte della pesca richiede pazienza!" );
									Stop();
									m_pole.AI_Timer=null;
									return;
								}
								// il pesce perde forza...								
								if (Utility.RandomDouble()>0.3)
								{								
									// salita della skill
									Server.Misc.SkillCheck.CheckSkill( m_from, m_from.Skills[SkillName.Fishing], m_pole, .8);
								}								
								m_from.SendMessage( HUE_GOOD, m_pole.random_good_reflex_mex() );							
								m_fish_str--;							
								m_war_stage=2;
								m_fish_quiet=Utility.RandomMinMax( 15, 35 );
								m_pole.m_LastFishAction=DateTime.Now;
								m_pole.action_gump=0;					
							}
							else
							{
								//m_from.SendMessage( "Che sfortuna! Il pesce e' scappato." );
								m_from.SendMessage( HUE_BAD, m_pole.random_sfiga_mex() );
								Stop();
								m_pole.AI_Timer=null;
								return;
							}
						}
						else
						{
							//m_from.SendMessage( "Tardi ! Il pesce e' scappato." );
							m_from.SendMessage( HUE_BAD,m_pole.random_bad_reflex_mex() );
							Stop();
							m_pole.AI_Timer=null;
							return;
						}
					}
				}
			}
		}// FINE GESTIONE FISHING AI ******************************************************************************************

		// MAIN ITEM METHODS ************************************************************************
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );			
			if (m_uses>0)
				list.Add( 1060658, "{0}\t{1}", string.Format( "uses"), m_uses ); // ~1_val~: ~2_val~
		}

		public override void GetContextMenuEntries( Mobile from, ArrayList list )
		{
			base.GetContextMenuEntries( from, list );
			BaseHarvestTool.AddContextMenuEntries( from, this, list, Fishing.System );
		}

		[Constructable]
		public SpecialFishingPole() : base( 0x0DC0 )
		{
			Name = "a Special Fishing Rod";
			Layer = Layer.OneHanded;
			Weight = 8.0;
			m_uses=-1;
		}

		public SpecialFishingPole( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version			
			writer.Write( (int) m_uses );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			if (version==2)
				m_uses = reader.ReadInt();
		}// END MAIN METHODS ************************************************************************
		
		// Utility functions ********************************************************************
		
		public bool Check_Riflesso(Mobile from, SpecialFishingPole pole,double Difficulty)
		{
			bool Reflex = false;
			TimeSpan Ritardo_Azione=(TimeSpan)((pole.m_LastAction)-(pole.m_LastFishAction));
			
			using ( StreamWriter op = new StreamWriter( "fishing.log",true ) )
			{
				op.WriteLine( "RIFLESSO :{0},{1}",CommandLogging.Format( from ),Ritardo_Azione );
			}
			//Console.WriteLine("Riflesso {0}",Ritardo_Azione);
			
			if (Ritardo_Azione<TimeSpan.Zero)
				Reflex = false;
			if (Ritardo_Azione<=TimeSpan.FromSeconds( Difficulty ) && Ritardo_Azione>=TimeSpan.FromSeconds( 0.1 ))
				Reflex = true;
							
			//Console.WriteLine("Riflesso {0}",Reflex);			
			return Reflex;
		}
		public string RandomSpecials(string message)
		{
			/*string spacer=" ";
			switch ( Utility.Random( 4 ) )
			{
				case 0: spacer=" "; break;
				case 1: spacer="  "; break;
				case 2: spacer="   "; break;
				case 3: spacer="    "; break;	
			}
			message=message.Replace(" ",spacer);
			switch ( Utility.Random( 11 ) )
			{
				case 0: spacer="+"; break;
				case 1: spacer="-"; break;
				case 2: spacer="*"; break;
				case 3: spacer="%"; break;
				case 4: spacer="#"; break;
				case 5: spacer="@"; break;
				case 6: spacer="|"; break;
				case 7: spacer="&"; break;
				case 8: spacer="%"; break;
				case 9: spacer="("; break;
				case 10: spacer=")"; break;
			}
			message=message.Replace("*",spacer);
			switch ( Utility.Random( 11 ) )
			{
				case 0: spacer="+"; break;
				case 1: spacer="-"; break;
				case 2: spacer="*"; break;
				case 3: spacer="%"; break;
				case 4: spacer="#"; break;
				case 5: spacer="@"; break;
				case 6: spacer="|"; break;
				case 7: spacer="&"; break;
				case 8: spacer="%"; break;
				case 9: spacer="("; break;
				case 10: spacer=")"; break;
			}
			message=message.Replace("+",spacer);
			return message;
		}
		
		public string random_war_mex()
		{			
			return "Il pesce combatte !";
		}
		public string random_down_mex()
		{	
			return "Il pesce si inabissa !";		
			/*switch ( Utility.Random( 10 ) )
			{
				case 0: return "* Il pesce si inabissa ! +";
				case 1: return "* Il  pesce  si  inabissa  ! +";
				case 2: return "* Il pesce ti viene incontro ! +";
				case 3: return "* Sale in superficie! +";
				case 4: return "* Sale  in  superficie! +";
				case 5: return "* E' al pelo dell'acqua! +";
				case 6: return "* Mostra segni di stanchezza ! +";
				case 7: return "* Il pesce non combatte ! +";
				case 8: return "* Sta cedendo ! +";
				case 9: return "* Il pesce punta sul fondo ! +";
			}
			return "Il pesce si inabissa !";
		}
		public string random_jump_mex()
		{			
			return "Il pesce salta !";
			/*switch ( Utility.Random( 6 ) )
			{
				case 0: return "* Il pesce salta ! +";
				case 1: return "* Il pesce  salta  ! +";
				case 2: return "* Il pesce Tira con molta forza ! +";
				case 3: return "* Combatte con troppa forza ! +";
				case 4: return "* Non vuole cedere ! +";
				case 5: return "* Il pesce strattona con forza ! +";				
			}
			return "Il pesce salta !";
		}
		public string random_good_mex()
		{			
			switch ( Utility.Random( 6 ) )
			{
				case 0: return "Per un pelo! Stava per scappare.";
				case 1: return "Ottima Mossa!";
				case 2: return "Combattera' ancora molto!";
				case 3: return "Vendera' cara la sua pelle!";
				case 4: return "Ben Fatto!";
				case 5: return "Deve essere grosso!";
			}
			return "Deve essere grosso!";
		}
		public string random_bad_mex()
		{			
			switch ( Utility.Random( 6 ) )
			{
				case 0: return "L'hai fatto scappare.";
				case 1: return "Pessima Mossa!";
				case 2: return "Era un gran bel pesce!";
				case 3: return "Accidenti!Si e' sgaciato.";
				case 4: return "Non e' la tua giornata.";
				case 5: return "Dovevi pensarci meglio.";
			}
			return "Dovevi pensarci meglio.";
		}
		public string random_bad_reflex_mex()
		{			
			switch ( Utility.Random( 6 ) )
			{
				case 0: return "Buona notte!";
				case 1: return "Pessimi riflessi.";
				case 2: return "Chi Dorme non piglia pesci!";
				case 3: return "Serve maggior attenzione!";
				case 4: return "Il pesce e' stato piu' lesto di te.";
				case 5: return "Tardi ! Il pesce e' scappato.";
			}
			return "Pessimi riflessi.";
		}

		public string random_good_reflex_mex()
		{						
			switch ( Utility.Random( 7 ) )
			{
				case 0: return "Ottima mossa!";
				case 1: return "Ottimi riflessi.";
				case 2: return "Questo pesce non ha scampo!";
				case 3: return "Perfetto!";
				case 4: return "Sara' una dura battaglia!";
				case 5: return "Che tempismo!";
				case 6: return "Non ha scampo!";
			}		
			return "Ottima mossa!";
		}
		
		public string random_sfiga_mex()
		{						
			switch ( Utility.Random( 7 ) )
			{
				case 0: return "Accidenti!Si deve essere rotto l'amo.";
				case 1: return "Accidenti!Si e' sganciato.";
				case 2: return "Il pesce si e' liberato!";
				case 3: return "Il pesce, e' stato piu' forte.";
				case 4: return "Era troppo grosso! Si e' liberato.";
				case 5: return "Il filo della lenza ha ceduto!";
				case 6: return "Che sfortuna...e' scappato.";
			}		
			return "Si e' liberato!";
		}

		public static bool ValidateDeepWater( int tileID )
		{
			//Console.WriteLine("Tile : {0}",tileID);
			bool water = false;
			for ( int i = 0; !water && i < m_WaterTiles.Length; i += 2 )			
				water = ( tileID >= m_WaterTiles[i] && tileID <= m_WaterTiles[i + 1] );

			return water;
		}

		public static bool ValidateDeepWaterPoint( int X, int Y , Map mappa)
		{
			bool water = false;
			Point2D p = new Point2D( X , Y );
			ArrayList w_items=mappa.GetTilesAt( p,false, true, false);				
			for ( int i = 0; i < w_items.Count; ++i )
			{
				Tile cur_tile=(Tile)w_items[i];				
				//if (ValidateDeepWater( ((cur_tile.ID & 0x3FFF) | 0x4000) ) )
				if (ValidateDeepWater( cur_tile.ID ) )
				{
					//Console.WriteLine("Tile is DEEP: {0}",cur_tile.ID);
					water=true;					
				}
				//else
				//	Console.WriteLine("Tile is NOOT DEEP: {0}",cur_tile.ID);
			}
			return water;
		}
				
		public static int[] m_WaterTiles = new int[]
			{
				0x00A8, 0x00AB,
				0x0136, 0x0137,
				0x5797, 0x579C,
				0x746E, 0x7485,
				0x7490, 0x74AB,
				0x74B5, 0x75D5
			};
	}
}
*/