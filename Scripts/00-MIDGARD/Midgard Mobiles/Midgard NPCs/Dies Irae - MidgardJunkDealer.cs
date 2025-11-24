/***************************************************************************
 *                                  MidgardJunkDealer.cs
 *                            		-------------------
 *  begin                	: Settembre, 2006
 *  version					: 2.2 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 * 		Il MidgardJunkDealer scambia una borsa di gioielli ( max 10 ) con
 * 		un gioiello abbastanza raro in base alla somma cumulata delle intensità 
 * 		dei bonus dei singoli gioielli.
 * 
 *  History:
 * 	-2.1 
 * 		Fixato il fatto che se il gioiello e' oscenamente potente (probabilmente	
 * 		addato in test da uno dello staff, il valore massimo del singolo gioiello 
 * 		viene cappato a 20.0 .
 * 		Fixato il fatto che possa essere presente sul gioiello anke la prop NightSight.
 *  -2.2 Scalata in maniera piu' logica la scala dei premi. Ora conviene investire.
 * 
 ***************************************************************************/
 
using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Items;
using Server.Network;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "il corpo di un malvivente" )]
	public class MidgardJunkDealer : BaseCreature
	{
		#region campi
		public static readonly int MaxJewelsQuantity = 10;
		public static readonly double SuperRarityLevel = 3.5;
		public static readonly int Sop110to115 = 10;
		public static readonly int Sop115to120 = 10;
		public static readonly int MinorToMajor = 10;
		#endregion

		#region costruttori
		[Constructable]
		public MidgardJunkDealer() : base( AIType.AI_Vendor, FightMode.Aggressor, 12, 1, 0.2, 0.8 )
		{
			// Stats
			SetStr( 304, 400 );
			SetDex( 102, 150 );
			SetInt( 204, 300 );
			
			SetHits( 66, 125 );
			SetDamage( 30, 50 );
			
			this.CantWalk = true;
			
			// Resistances
			SetResistance( ResistanceType.Physical, 40, 50 );
			SetResistance( ResistanceType.Fire, 40, 50 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 40, 50 );
			
			// Skills
			SetSkill( SkillName.EvalInt, 90.0, 100.0 );
			SetSkill( SkillName.Inscribe, 90.0, 100.0 );
			SetSkill( SkillName.Magery, 90.0, 100.0 );
			SetSkill( SkillName.Meditation, 90.0, 100.0 );
			SetSkill( SkillName.MagicResist, 90.0, 100.0 );
			SetSkill( SkillName.Wrestling, 90.0, 100.0 );
			
			// Fama e Karma
			Fame = 5000;
			Karma = -10000;

			// Hair
			HairItemID = 0x203B;
			
			// Speech & Skin hues
			SpeechHue = Utility.RandomDyedHue();
			Hue = Utility.RandomSkinHue();
			
			// Genre
			Female = Utility.RandomBool();
			if( Female ) 
         	{ 
            	Name = NameList.RandomName( "female" );
				Title = ", the pretty junk dealer"; 
				Body = 0x191;  	
		    } 
         	else 
         	{ 
	   			Name = NameList.RandomName( "male" );
				Title = ", the junk dealer"; 
				Body = 0x190;
         	} 

			// Equip:
			// 	Robe
			Item robe = new Item ( 778 );
			robe.Hue = 1234;
			robe.Layer = Layer.OuterTorso;
			robe.LootType = LootType.Blessed;
			robe.Movable = false;
			AddItem( robe );

			// 	Sandals
			Sandals sandals = new Sandals ();
			sandals.Hue = 1235;
			sandals.LootType = LootType.Blessed;
			sandals.Movable = false;
			AddItem( sandals );

			// 	Cloak
			Cloak cloak = new Cloak ();
			cloak.Hue = 1236;
			cloak.LootType = LootType.Blessed;
			cloak.Movable = false;
			AddItem( cloak );
		}

		public MidgardJunkDealer( Serial serial ) : base( serial )
		{
		}
		#endregion
		
		#region metodi			
		public override void GenerateLoot()
		{	
			AddLoot( LootPack.Rich, 2 );
		}
		
		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );
				
			if ( from.Alive )
			{
				list.Add( new GetValueEntry( this, true ) );
				list.Add( new OfferEntry( this, true ) );

                if( Midgard.Misc.Midgard2Persistance.PowerScrollsEnabled )
				    list.Add( new PowerScrollEntry( this, true ) );

				if( from.AccessLevel >= AccessLevel.Administrator )
					list.Add( new ArtifactEntry( this, true ) );

				if( from.AccessLevel >= AccessLevel.Administrator )
					list.Add( new PaladinPlateNewEntry( this, true ) );
			}
		}
		
		public double ValutaGioiello( BaseJewel bj )
  		{
			double ValoreGioiello = 0;
			
			#region Valori Massimi di intensità delle props
			int[] CapProps = {
				2, 		//  RegenHitsCap = 2;
				3, 		//  RegenStamCap = 3;
				2, 		//  RegenManaCap = 2;
				15, 	//  DefenceChanceCap = 15;
				15, 	// 	AttackChanceCap = 15;
				8, 		//  BonusStrCap = 8;
				8, 		//  BonusDexCap = 8;
				8, 		//  BonusIntCap = 8;
				5, 		//  BonusHitsCap = 5;
				8, 		//  BonusStamCap = 8;
				8, 		//  BonusManaCap = 8;
				25, 	//  WeaponDamageCap = 25 ;
				30, 	//  WeaponSpeedCap = 30 ;
				12, 	//  SpellDamageCap = 12;
				3, 		//  CastRecovery = 3;
				1, 		//  CastSpeed = 1;
				8, 		//  LowerManaCostCap = 8;
				20, 	//  LowerRegCostCap = 20;
				15, 	//  ReflectPhysicalCap = 15;
				25, 	//  EnhancePotionsCap = 25;
				100, 	//  LuckCap = 100;
				1, 		//  SpellChanneling = 1;	
				1, 		//  NightSight= 1;
			};

			int ResistancesCap = 15;
			
			int SkillBonusCap = 15;
			#endregion
			
			int i = 0;
			foreach( int j in Enum.GetValues(typeof( AosAttribute )) )
			{
				if( bj.Attributes[(AosAttribute)j] > 0 ) 
				{
					ValoreGioiello += ( bj.Attributes[(AosAttribute)j] / (double)CapProps[i] );
				}
				i++;
			}
			
			foreach( int j in Enum.GetValues(typeof( AosElementAttribute )) )
			{
				if( bj.Resistances[(AosElementAttribute)j] > 0 ) 
				{
					ValoreGioiello += ( bj.Resistances[(AosElementAttribute)j] / (double)ResistancesCap );
				}
			}
							
			for( int j=0; j<5; j++ )
			{
				if ( bj.SkillBonuses.GetBonus( j ) > 0 ) 
				{
					ValoreGioiello += ( bj.SkillBonuses.GetBonus( j ) / (double)SkillBonusCap );
				}					
			}
			
			// Se un singolo gioiello ha ValoreGioiello molto alto il valore viene incrementato.
			if( ValoreGioiello > SuperRarityLevel )
			{
				ValoreGioiello = (Math.Exp( ValoreGioiello - SuperRarityLevel)) - 1 + SuperRarityLevel;
				if( ValoreGioiello > 20.0 )
					ValoreGioiello = 20.0;
				this.Say( "*Ghigna felice guardando un gioiello in particolare*" );
			}
			
			return ValoreGioiello;
  		}
		#endregion
		
		#region serialize-deserialize
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
		#endregion
	
		private class PaladinPlateEntry : ContextMenuEntry
		{
			#region campi
			private Mobile m_Offerer;
			private MidgardJunkDealer m_Dealer;
			#endregion
			
			#region costruttori
			public PaladinPlateEntry( MidgardJunkDealer mjd, bool enabled ) : base( 5009, 2 ) // Smite
			{
				m_Dealer = mjd;				
				if ( !enabled )
					Flags |= CMEFlags.Disabled;				
			}
			#endregion
			
			#region metodi
      		public override void OnClick()
			{
				if ( m_Dealer.Deleted )
					return;
				
				m_Offerer = Owner.From;
				m_Dealer.Say( "Scegli una borsa nel tuo zaino contenente SOLO pezzi di armatura da paladino." );
				m_Offerer.BeginTarget( 0, false, TargetFlags.None, new TargetCallback( OnTarget ) );
				return;
      		}
      		
      		public void OnTarget( Mobile from, object obj )
      		{
  				Bag Borsa = obj as Bag;
  				if( Borsa == null )
  				{
  					m_Dealer.Say( "Non hai scelto una borsa." );
  					return;
  				}
  				if( !( Borsa.IsChildOf( m_Offerer.Backpack ) ) )
  				{
  					m_Dealer.Say( "Non hai scelto una borsa nel tuo zaino." );
  					return;  					
  				}
  				
  				int NumeroPezzi = 0;
  				
  				// Calcola il valore della borsa.
  				List<Item> Offerta = Borsa.Items;
  				foreach( Item i in Offerta )
  				{
  					if( PaladinPlateNewEntry.IsPalaPlate( i ) )
  					{
  						NumeroPezzi++;
  					}
  					else
  					{
  						m_Dealer.Say( "La borsa non contiene pezzi di armatura da paladino indi non mi interessa." );
  						return;
  					}
  				}
  				
  				if( NumeroPezzi < Offerta.Count )
  				{
  					m_Dealer.Say( "La borsa contiene anche oggetti che non mi interessano." );
  					return;
  				}
  				
  				if( NumeroPezzi > 0 )
  				{
	  				int Cariche = NumeroPezzi * 5;
	  				RunicHammer rt = new RunicHammer( CraftResource.Valorite, Cariche );
	  				m_Dealer.Say( "Eccoti un oggetto di valore equivalente." );
					if( from.Backpack == null || !from.Backpack.TryDropItem( from, rt, false ) )
						rt.MoveToWorld( from.Location, from.Map );
  				}
  				
				Borsa.Delete();
      		}
      		#endregion
		}
		
		private class PaladinPlateNewEntry : ContextMenuEntry
		{
			#region campi
			private Mobile m_Offerer;
			private MidgardJunkDealer m_Dealer;
			#endregion
			
			#region costruttori
			public PaladinPlateNewEntry( MidgardJunkDealer mjd, bool enabled ) : base( 5009, 2 ) // Smite
			{
				m_Dealer = mjd;				
				if ( !enabled )
					Flags |= CMEFlags.Disabled;				
			}
			#endregion
			
			#region metodi
      		public override void OnClick()
			{
				if ( m_Dealer.Deleted )
					return;
				
				m_Offerer = Owner.From;
				m_Dealer.Say( "Scegli una borsa nel tuo zaino contenente SOLO pezzi di armatura da paladino." );
				m_Offerer.BeginTarget( 0, false, TargetFlags.None, new TargetCallback( OnTarget ) );
				return;
      		}
      		
      		public void OnTarget( Mobile from, object obj )
      		{
  				Bag Borsa = obj as Bag;
  				if( Borsa == null )
  				{
  					m_Dealer.Say( "Non hai scelto una borsa." );
  					return;
  				}
  				if( !( Borsa.IsChildOf( m_Offerer.Backpack ) ) )
  				{
  					m_Dealer.Say( "Non hai scelto una borsa nel tuo zaino." );
  					return;  					
  				}

  				// Crea la lista di oggetti nella borsa
  				List<Item> Offerta = Borsa.Items;
  				
  				if( Offerta.Count != 1 )
  				{
  					m_Dealer.Say( "Hey, la borsa deve contenere solo UN pezzo di armatura da paladino e null'altro." );
  					return; 
  				}
  					
  				BaseArmor oldArmor = Offerta[0] as BaseArmor;
  				
  				if( oldArmor != null && IsPalaPlate( oldArmor ) )
				{
					Type newArmorType = ScriptCompiler.FindTypeByName( oldArmor.GetType().Name.Remove( 0, 7 ) ); // Rimuovo "Paladin"
					
					try
					{
						BaseArmor newArmor = Activator.CreateInstance( newArmorType ) as BaseArmor;
						
						ConvertIntoOSIArmor( newArmor, oldArmor, ArmorQuality.Exceptional, CraftResource.Aqua );

						if( from.Backpack == null || !from.Backpack.TryDropItem( from, newArmor, false ) )
							newArmor.MoveToWorld( from.Location, from.Map );
						
						m_Dealer.Say( "Eccoti qualcosa di prezioso in cambio." );
						
						if( from.AccessLevel < AccessLevel.Administrator )
							oldArmor.Delete();
					}
					catch( System.Exception e )
					{
						Console.WriteLine( "Errore di conversione di una palaplate: {0}", e.ToString() );
					}
  				}
  				else
  				{
  					m_Dealer.Say( "Hey, accetto solo un pezzo di armatura da paladino nella borsa." );
  				}
      		}
      
      		public static bool IsPalaPlate( Item i )
      		{
      			if( i == null )
      				return false;
      			else
	      			return ( i is PaladinPlateArms   || i is PaladinPlateChest || i is PaladinPlateGloves ||
	      					 i is PaladinPlateGorget || i is PaladinPlateHelm  || i is PaladinPlateLegs );
      		}
      					
			public static void ConvertIntoOSIArmor( BaseArmor dest, BaseArmor src, ArmorQuality destQuality, CraftResource destMaterial ) 
			{ 
				// Qualità dell'armatura (normal, exc)
				dest.Quality = destQuality;
				
				// Materiale dell'armatura. Influenza le res base da solo ed eventuali bonus del materiale stesso.
				dest.Resource = destMaterial;
				
				// Copia dei AosAttribute
				int j = -1;
				foreach( int i in Enum.GetValues( typeof( AosAttribute ) ) )
				{
					j++;
					if( src.Attributes[(AosAttribute)i] > 0 ) 
					{
						dest.Attributes[(AosAttribute)i] = Math.Min( src.Attributes[(AosAttribute)i], CapAosAttributes[j] );
					}					
				}
				
				// Copia dei AosArmorAttribute
				int k = -1;
				foreach( int i in Enum.GetValues( typeof( AosArmorAttribute ) ) )
				{
					k++;
					if ( src.ArmorAttributes[(AosArmorAttribute)i] > 0 ) 
					{
						dest.ArmorAttributes[(AosArmorAttribute)i] = Math.Min( src.ArmorAttributes[(AosArmorAttribute)i], CapAosAttributes[k] );					
					}
				}

				// Copia delle res
				dest.PhysicalBonus 	= Math.Min( src.PhysicalBonus, capResBonus);
				dest.FireBonus		= Math.Min( src.FireBonus, capResBonus);
				dest.ColdBonus		= Math.Min( src.ColdBonus, capResBonus);
				dest.PoisonBonus	= Math.Min( src.PoisonBonus, capResBonus);
				dest.EnergyBonus	= Math.Min( src.EnergyBonus, capResBonus);
			}
			#endregion
			
			#region caps AosAttributes e AosArmorAttributes Su Armors: -1 prop disabilitata, 0 senza cap, il valore altrimenti
			public static int[] CapAosAttributes = {
				2, 			//  RegenHitsCap
				3, 			//  RegenStamCap
				2, 			//  RegenManaCap
				-1, 		//  DefenceChanceCap
				-1, 		// 	AttackChanceCap
				3, 			//  BonusStrCap
				3, 			//  BonusDexCap
				3, 			//  BonusIntCap
				5, 			//  BonusHitsCap
				8, 			//  BonusStamCap
				8, 			//  BonusManaCap
				-1, 		//  WeaponDamageCap
				-1, 		//  WeaponSpeedCap
				-1, 		//  SpellDamageCap
				-1, 		//  CastRecovery
				-1, 		//  CastSpeed
				8, 			//  LowerManaCostCap
				20, 		//  LowerRegCostCap
				15, 		//  ReflectPhysicalCap
				-1, 		//  EnhancePotionsCap
				100, 		//  LuckCap
				-1, 		//  SpellChanneling	
				1, 			//  NightSight
			};
			
			public static int[] CapAosArmorAttributes = {
				100, 		//  Durability
				100, 		//  LowerStatReq
				1, 			//  MageArmor
				5, 			//  SelfRepair
			};
			
			static int capResBonus = 15;
      		#endregion
		}
		
		private class PowerScrollEntry : ContextMenuEntry
		{
			#region campi
			private Mobile m_Offerer;
			private MidgardJunkDealer m_Dealer;
			#endregion
			
			#region costruttori
			public PowerScrollEntry( MidgardJunkDealer mjd, bool enabled ) : base( 1042, 2 ) // Offer a bag of Power Scrolls to the Dealer
			{
				m_Dealer = mjd;				
				if ( !enabled )
					Flags |= CMEFlags.Disabled;				
			}
			#endregion
			
			#region metodi
      		public override void OnClick()
			{
				if ( m_Dealer.Deleted )
					return;
				
				m_Offerer = Owner.From;
				m_Dealer.Say( "Scegli una borsa nel tuo zaino contenente SOLO Power Scrolls dello stesso tipo (110 o 115)." );
				m_Offerer.BeginTarget( 0, false, TargetFlags.None, new TargetCallback( OnTarget ) );
				return;
      		}
      		
      		public void OnTarget( Mobile from, object obj )
      		{
  				Bag Borsa = obj as Bag;
  				if( Borsa == null )
  				{
  					m_Dealer.Say( "Non hai scelto una borsa." );
  					return;
  				}
  				if( !( Borsa.IsChildOf( m_Offerer.Backpack ) ) )
  				{
  					m_Dealer.Say( "Non hai scelto una borsa nel tuo zaino." );
  					return;  					
  				}
  				
  				int NumeroSop = 0;
  				double Potenza = 0;
  				
  				// Calcola il valore della borsa.
  				List<Item> Offerta = Borsa.Items;
  				foreach( Item i in Offerta )
  				{
  					if( i is PowerScroll )
  					{
  						NumeroSop++;
  						PowerScroll sop = (PowerScroll) i;
  						if( Potenza != 0 && sop.Value != Potenza )
						{
	  						m_Dealer.Say( "La borsa non contiene solo Sops della stessa potenza indi non mi interessa." );
	  						return;							
						}
  						Potenza = sop.Value;
  					}
  					else
  					{
  						m_Dealer.Say( "La borsa non contiene solo Sops indi non mi interessa." );
  						return;
  					}
  				}
  				
  				if( NumeroSop < Offerta.Count )
  				{
  					m_Dealer.Say( "La borsa contiene oggetti che non mi interessano." );
  					return;
  				}
  				
  				int NewPot = 0;
  					
  				// Se arriva qua la borsa ha solo sop e dello stesso tipo in numro di NumeroSop
  				if( Potenza == 110.0 && NumeroSop == Sop110to115 )
  				{
  					NewPot = 15;
  				}
  				else if( Potenza == 115.0 && NumeroSop == Sop115to120 )
  				{
  					NewPot = 20;
  				} 
  				else
  				{
   					m_Dealer.Say( "Il numero di sop non è corretto. Scambierò {0} sop 110 per una 115 e {1} sop 115 per una 120.", Sop110to115, Sop115to120 );
  					return; 					
  				}
  				
  				PowerScroll NewSop = PowerScroll.CreateRandomNoCraft( NewPot, NewPot );

  				m_Dealer.Say( "Ecco fatto. Ho posto una nuova pergamena del potere nel tuo zaino." );

				if( from.Backpack == null || !from.Backpack.TryDropItem( from, NewSop, false ) )
					NewSop.MoveToWorld( from.Location, from.Map );	
				Borsa.Delete();
      		}
      		#endregion
		}
		
		private class ArtifactEntry : ContextMenuEntry
		{
			#region campi
			private Mobile m_Offerer;
			private MidgardJunkDealer m_Dealer;
			#endregion
			
			#region costruttori
			public ArtifactEntry( MidgardJunkDealer mjd, bool enabled ) : base( 1043, 3 ) // Offer a bag of Minor Artifacts to the Dealer
			{
				m_Dealer = mjd;				
				if ( !enabled )
					Flags |= CMEFlags.Disabled;				
			}
			#endregion
			
			#region metodi
      		public override void OnClick()
			{
				if ( m_Dealer.Deleted )
					return;
				
				m_Offerer = Owner.From;
				m_Dealer.Say( "Scegli una borsa nel tuo zaino contenente SOLO artefatti minori." );
				m_Offerer.BeginTarget( 0, false, TargetFlags.None, new TargetCallback( OnTarget ) );
				return;
      		}
      		
      		public void OnTarget( Mobile from, object obj )
      		{
  				Bag Borsa = obj as Bag;
  				if( Borsa == null )
  				{
  					m_Dealer.Say( "Non hai scelto una borsa." );
  					return;
  				}
  				if( !( Borsa.IsChildOf( m_Offerer.Backpack ) ) )
  				{
  					m_Dealer.Say( "Non hai scelto una borsa nel tuo zaino." );
  					return;  					
  				}
  				
  				int NumeroArties = 0;
  				
  				List<Item> Offerta = Borsa.Items;
  				foreach( Item i in Offerta )
  				{
  					if( IsMinorArtifact( i ) )
  					{
  						NumeroArties++;
  					}
  					else
  					{
  						m_Dealer.Say( "La borsa non contiene solo artefatti minori indi non mi interessa." );
  						return;
  					}
  				}
  				
  				if( NumeroArties < Offerta.Count )
  				{
  					m_Dealer.Say( "La borsa contiene oggetti che non mi interessano." );
  					return;
  				}
  				
  				// Se arriva qua la borsa ha solo artefatti minori 
  				if( NumeroArties == MinorToMajor )
  				{
  					Item artifact = DemonKnight.CreateRandomArtifact();
		  			Container pack = from.Backpack;
		
					if( pack == null || !pack.TryDropItem( from, artifact, false ) )
					{
						artifact.MoveToWorld( from.Location, from.Map );	
					}
					Borsa.Delete();
  				}
  				else
  				{
   					m_Dealer.Say( "Il numero di artefatti minori non è corretto. Scambierò {0} artefatti minori in uno maggiore random.", MinorToMajor );
  					return; 					
  				}
      		}
      		#endregion
      		
	  		public static bool IsMinorArtifact( Item i )
	  		{
	  			return ( Array.IndexOf( Midgard2MinorArtifacts, i.GetType() ) >= 0 );
//	  			for( int ind=0; ind<MidgardMinorArtefatti.Length; ind++ )
//	  			{
//	  				if( i.GetType() == MidgardMinorArtefatti[ind] )
//	  				{
//	  					return true;
//	  				}
//	  			}
//	  			return false;
	  		}
	  		
	  		#region minor artifacts
	 		private static Type[] Midgard2MinorArtifacts = new Type[]
			{
				// Armors
				typeof(BurglarsBandana),
				typeof(DreadPirateHat),
				typeof(GlovesOfThePugilist),
				typeof(HeartOfTheLion), 
				typeof(OrcishVisage),
				typeof(PolarBearMask),
				typeof(VioletCourage),

				// Shields
				typeof(ShieldOfInvulnerability),
				
				// Weapons
				typeof(ArcticDeathDealer), 
				typeof(BlazeOfDeath),
				typeof(BowOfTheJukaKing),
				typeof(CaptainQuacklebushsCutlass),
				typeof(CavortingClub),	
				typeof(ColdBlood),
				typeof(EnchantedTitanLegBone),
				typeof(LunaLance),
				typeof(NightsKiss),
				typeof(NoxRangersHeavyCrossbow),
				typeof(PixieSwatter),
				typeof(StaffOfPower),
				typeof(WrathOfTheDryad),

				// Jewels
				typeof(AlchemistsBauble),
				
				// Deco
				typeof(CandelabraOfSouls),
				typeof(GhostShipAnchor),
				typeof(GoldBricks),
				typeof(PhillipsWoodenSteed),
				typeof(SeahorseStatuette),
				typeof(ShipModelOfTheHMSCape),
				typeof(AdmiralsHeartyRum),
				
				// Instruments
				typeof(GwennosHarp),
				typeof(IolosLute),	   
			};
	 		#endregion
		}		
		
		private class GetValueEntry : ContextMenuEntry
		{
			#region campi
			private Mobile m_Offerer;
			private MidgardJunkDealer m_Dealer;
			#endregion
			
			#region costruttori
			public GetValueEntry( MidgardJunkDealer mjd, bool enabled ) : base( 1040, 3 ) // Get a Evalutaion of a bag of Jewels
			{
				m_Dealer = mjd;				
				if ( !enabled )
					Flags |= CMEFlags.Disabled;				
			}
			#endregion
			
			#region metodi
      		public override void OnClick()
			{
				if ( m_Dealer.Deleted )
					return;
				
				m_Offerer = Owner.From;
				m_Dealer.Say( "Scegli una borsa nel tuo zaino." );
				m_Offerer.BeginTarget( 0, false, TargetFlags.None, new TargetCallback( OnTarget ) );
				return;
      		}
      		
      		public void OnTarget( Mobile from, object obj )
      		{
  				Bag Borsa = obj as Bag;
  				if( Borsa == null )
  				{
  					m_Dealer.Say( "Non hai scelto una borsa." );
  					return;
  				}
  				if( !( Borsa.IsChildOf( m_Offerer.Backpack ) ) )
  				{
  					m_Dealer.Say( "Non hai scelto una borsa nel tuo zaino." );
  					return;  					
  				}
  				
  				double ValoreAssolutoBorsa = 0;
  				int NumeroGioielli = 0;
  				
  				// Calcola il valore della borsa.
  				List<Item> Offerta = Borsa.Items;

                bool badItems = false;
  				foreach( Item i in Offerta )
  				{
  					if( !( i is BaseJewel ) || Midgard.Engines.XmlForceIdentify.IsUnidentified( i ) )
  					{
                        badItems = true;
  					}
  				}

                if( badItems )
                {
                    m_Dealer.Say( "Ah, vedo che nella borsa ci sono oggetti che non mi interessano oppure non sono identificati." );
                    return;
                }

  				foreach( Item i in Offerta )
  				{
  					if( i is BaseJewel )
  					{
  						NumeroGioielli++;
  						BaseJewel bj = (BaseJewel) i;
  						ValoreAssolutoBorsa += m_Dealer.ValutaGioiello( bj );
  					}
  				}
  				m_Dealer.Say( "Per me la borsa vale {0} in una scala da 0 a 60" , ((int)ValoreAssolutoBorsa).ToString() );
  				
  				if( NumeroGioielli > MaxJewelsQuantity )
  					m_Dealer.Say( "Ma attenzione che dentro ci sono troppi gioielli e non l'accetterei come offerta" );
  				
  				//if( NumeroGioielli < Offerta.Count )
  				// 	m_Dealer.Say( "Ah, vedo che nella borsa ci sono oggetti che non mi interessano perchè non sono gioielli" );
      		}
      		#endregion
		}
		
		private class OfferEntry : ContextMenuEntry
		{
			#region campi
			private Mobile m_Offerer;
			private MidgardJunkDealer m_Dealer;
			#endregion
			
			#region costruttori
			public OfferEntry( MidgardJunkDealer mjd, bool enabled ) : base( 1041, 3 ) // Offer a bag of Jewels to the Dealer
			{
				m_Dealer = mjd;				
				if ( !enabled )
					Flags |= CMEFlags.Disabled;				
			}
			#endregion
			
			#region metodi
      		public override void OnClick()
			{
				if ( m_Dealer.Deleted )
					return;
				
				m_Offerer = Owner.From;
				m_Dealer.Say( "Scegli una borsa nel tuo zaino." );
				m_Offerer.BeginTarget( 0, false, TargetFlags.None, new TargetCallback( OnTarget ) );
				return;
      		}
      		
      		public void OnTarget( Mobile from, object obj )
      		{
  				Bag Borsa = obj as Bag;
  				if( Borsa == null )
  				{
  					m_Dealer.Say( "Non hai scelto una borsa." );
  					return;
  				}
  				if( !( Borsa.IsChildOf( m_Offerer.Backpack ) ) )
  				{
  					m_Dealer.Say( "Non hai scelto una borsa nel tuo zaino." );
  					return;  					
  				}
  				
  				double ValoreAssolutoBorsa = 0;
  				int NumeroGioielli = 0;
  				
  				// Calcola il valore della borsa.
  				List<Item> Offerta = Borsa.Items;
  				foreach( Item i in Offerta )
  				{
  					if( i is BaseJewel )
  					{
  						NumeroGioielli++;
  						BaseJewel bj = (BaseJewel) i;
  						ValoreAssolutoBorsa += m_Dealer.ValutaGioiello( bj );
  					}
  				}
  				
   				if( NumeroGioielli > MaxJewelsQuantity)
   				{
  					m_Dealer.Say( "Dentro essa ci sono troppi gioielli e non tratto così tanta merce in una volta sola." );
   					return;
   				}
   				
  				if( NumeroGioielli < Offerta.Count )
  				{
  					m_Dealer.Say( "Ah, vedo che nella borsa ci sono oggetti che non mi interessano perchè non sono gioielli." );
  					return;
  				}
  				
  				m_Dealer.Say( "Per me la borsa vale {0} in una scala da 0 a 60" , ((int)ValoreAssolutoBorsa).ToString() );

  				if( (int)ValoreAssolutoBorsa < 5 )
  				{
  					m_Dealer.Say( "Uhm... davvero un bel sacchetto di pacottiglia." );
  					return;
  				}	
  				
				m_Dealer.Say( "Uhm... vediamo cosa posso darti per questi gioielli" );
  				Item GioielloContrabbandato = Loot.RandomJewelry();
  				
  				#region settaggi originari da calibrare
  				/*
  				if( (int)ValoreAssolutoBorsa < 10 )
  				{
  					BaseRunicTool.ApplyAttributesTo( (BaseJewel)GioielloContrabbandato, Utility.RandomMinMax( 1, 3 ), 30, 50);
  				}
  				else if( (int)ValoreAssolutoBorsa < 20 )
  				{
  					BaseRunicTool.ApplyAttributesTo( (BaseJewel)GioielloContrabbandato, Utility.RandomMinMax( 2, 4 ), 40, 60);
  				}
   				else if( (int)ValoreAssolutoBorsa < 30 )
  				{
  					BaseRunicTool.ApplyAttributesTo( (BaseJewel)GioielloContrabbandato, Utility.RandomMinMax( 3, 5 ), 50, 70);
  				} 				
   				else if( (int)ValoreAssolutoBorsa < 40 )
  				{
  					BaseRunicTool.ApplyAttributesTo( (BaseJewel)GioielloContrabbandato, Utility.RandomMinMax( 4, 6 ), 60, 80);
  				}				
   				else if( (int)ValoreAssolutoBorsa < 50 )
  				{
  					BaseRunicTool.ApplyAttributesTo( (BaseJewel)GioielloContrabbandato, Utility.RandomMinMax( 4, 6 ), 70, 90);
  				} 				
   				else if( (int)ValoreAssolutoBorsa < 60 )
  				{
  					BaseRunicTool.ApplyAttributesTo( (BaseJewel)GioielloContrabbandato, Utility.RandomMinMax( 5, 6 ), 80, 100);
  				} 				
  				else
   				{
  					BaseRunicTool.ApplyAttributesTo( (BaseJewel)GioielloContrabbandato, Utility.RandomMinMax( 6, 6 ), 100, 100);
  				}  	
  				*/
  				#endregion
 
   				#region settaggi dopo la prima calibrazione
  				if( (int)ValoreAssolutoBorsa < 10 ) // restituisce un gioiello di valore 0.7-1.4
  				{
  					BaseRunicTool.ApplyAttributesTo( (BaseJewel)GioielloContrabbandato, Utility.RandomMinMax( 1, 2 ), 70, 70);
  				}
  				else if( (int)ValoreAssolutoBorsa < 20 ) // restituisce un gioiello di valore 1.4-2.1
  				{
  					BaseRunicTool.ApplyAttributesTo( (BaseJewel)GioielloContrabbandato, Utility.RandomMinMax( 2, 3 ), 70, 70);
  				}
   				else if( (int)ValoreAssolutoBorsa < 30 ) // restituisce un gioiello di valore 2.4-3.2
  				{
  					BaseRunicTool.ApplyAttributesTo( (BaseJewel)GioielloContrabbandato, Utility.RandomMinMax( 3, 4 ), 80, 80);
  				} 				
   				else if( (int)ValoreAssolutoBorsa < 40 ) // restituisce un gioiello di valore 3.2-4.0
  				{
  					BaseRunicTool.ApplyAttributesTo( (BaseJewel)GioielloContrabbandato, Utility.RandomMinMax( 4, 5 ), 80, 80);
  				}				
   				else if( (int)ValoreAssolutoBorsa < 50 ) // restituisce un gioiello di valore 4.0-5.4
  				{
  					BaseRunicTool.ApplyAttributesTo( (BaseJewel)GioielloContrabbandato, Utility.RandomMinMax( 5, 6 ), 80, 90);
  				} 				
   				else if( (int)ValoreAssolutoBorsa < 60 ) // restituisce un gioiello di valore 5.4-6.0
  				{
  					BaseRunicTool.ApplyAttributesTo( (BaseJewel)GioielloContrabbandato, Utility.RandomMinMax( 6, 6 ), 90, 100);
  				} 				
  				else // restituisce un gioiello di valore 6.0
   				{
  					BaseRunicTool.ApplyAttributesTo( (BaseJewel)GioielloContrabbandato, Utility.RandomMinMax( 6, 6 ), 100, 100);
  				}  	
  				#endregion
  				
  				GioielloContrabbandato.Visible = true;
				if( from.Backpack == null || !from.Backpack.TryDropItem( from, GioielloContrabbandato, false ) )
					GioielloContrabbandato.MoveToWorld( from.Location, from.Map );	
				Borsa.Delete();
      		}
      		#endregion
		}
	}
}
