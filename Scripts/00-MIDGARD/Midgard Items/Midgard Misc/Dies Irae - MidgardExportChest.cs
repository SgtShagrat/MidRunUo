/***************************************************************************
 *                                  MidgardExportChest2.cs
 *                            		---------------------
 *  begin               	: Agosto-Settembte, 2006
 * 	version					: 2.01
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
  * 
  *		Info:
  *  	La chest in questione salva in un file xml le proprietà degli oggetti 
  *  	contenuti in essa.
  * 
  *  	History:
  *  	- 1.0 	Aggiunto Context Menu.
  *  	- 1.1 	Cambiato nome, tipo base , colore.
  *  	- 1.2 	Aggiuntti BaseWeapon, BaseCloth, BaseArmor, BaseRunicTool, 
  * 			BaseJewel.
  * 	- 1.3	Aggiunto controllo Privs: solo Admin possono usare la cassa.
  * 	- 1.4   Aggiunti vari campi xml a tutti gli oggetti.
  *    	- 1.5   Aggiunto il contorllo artefatti, Maggiori e Minori.
  *     - 1.6   Adattata alla distro rc0 di runuo.
  * 	- 1.7   Aggiunta lista TypesProibiti, aggiunto check sui contenitori,
  * 			aggiunti override dei metodi OnDragDrop e OnDragDropInto.
  * 	- 1.8   Fix per aggiungere anche SoP e SoS.
  *     - 1.9   Cambiato l'accesso a GameMaster e aggiunta MenuEntry per listare
  * 			gli item nn inseribili.
  * 	- 1.10  Aggiunto Log di chi usa la cassa.
  * 	- 1.11  Corretto valore fantasma per gli artefatti.
  * 	- 1.12	Aggiunto Hue e Name per gli artefatti.
  * 			Aggiunti vari oggetti nella lista di vietati.
  * 			Aggiunto controllo anti Deed (permette cmq i BanCheck).
  * 			Aggiunti BankCheck e ChampionSkull agli oggetti esportabili.
  * 	- 1.13  Aggiunte Sop, Sos, e vietati nuovi (rune runebook gold).
  * 	- 1.14  Aggiunti altri items. Tolto il check sull'AccessLevel.
  * 	- 1.15	Il peso della cassa ora e' 0 perennemente.
  * 	- 1.16	Aggiunti altri oggetti.
  *		- 1.17	Aggiunti altri oggetti. Fix vari.
  * 	- 2.00 	Passata a versione 2.0 per sicurezza.
  * 	- 2.01	Fixato blessed per i vestiti.
  * 	- 2.02  Aggiunto il seriale ad ogni oggetto esportato.
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using Server.ContextMenus;
using Server.Network;

namespace Server.Items
{ 
	[FlipableAttribute( 0xE41, 0xE40 )]
   	public class MidgardExportChest2 : Container 
   	{ 
   		#region campi
   		public static readonly double m_Redux = 100;	// Percentuale di riduzione del peso
   		public static readonly double m_Version = 2.0;	// Percentuale di riduzione del peso
   		#endregion
   		
   		#region proprietà
   		public override int DefaultDropSound{ get{ return 0x42; } }
   		public override int DefaultGumpID{ get{ return 66; } }			// default della metal box
   		public override int DefaultMaxWeight{ get{ return int.MaxValue; } }
   		#endregion
   		
   		#region costruttori
		[Constructable] 
		public MidgardExportChest2() : base( 0xE41 ) 
      	{ 
			Name = "a Midgard Special Transfert Chest (versione " + m_Version.ToString() + " )";
		 	Hue = 1150;
			MaxItems = 350;
       	}
   	
	    public MidgardExportChest2( Serial serial ) : base( serial ) 
	    { 
	    } 
	    #endregion
	    
	    #region metodi
		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );
		
			if ( from.Alive )
			{
				list.Add( new Export( this, true ) );
				list.Add( new ListaVietati( this, true ) );
			}
		}
	    
	    public override bool OnDragDrop( Mobile from, Item dropped )
		{
			Item m_item = dropped as Item;

			if ( m_item != null && base.OnDragDrop( from, dropped ) )
			{
				if( m_item is BaseContainer)
				{
				   	from.SendMessage( "Non puoi inserire contenitori nella cassa." );
				   	return false;					
				}
							
				if( m_item.ItemID == 0x14F0 && 
				    m_item.GetType() != typeof(BankCheck) && 
				    m_item.GetType() != typeof(PowerScroll) &&
				    m_item.GetType() != typeof(StatCapScroll) )
				{
				   	from.SendMessage( "Non puoi inserire Deeds nella cassa." );
				   	return false;						
				}
				
				for( int ind=0; ind<TypesProibiti.Length; ind++ )
				{
					if(m_item.GetType() == TypesProibiti[ind])
					{
						from.SendMessage( "Non puoi inserire questo tipo di oggetti nella cassa: " + m_item.GetType().Name );
					   	return false;
					}
				}
				from.SendMessage( "Hai inserito : " + m_item.GetType().Name );
				return true;
			}
			else
			{
				return false;
			}
		}
	    
	    public override bool OnDragDropInto( Mobile from, Item dropped, Point3D p )
		{
			Item m_item = dropped as Item;

			if ( m_item != null && base.OnDragDropInto( from, dropped, p )  )
			{
				if( m_item is BaseContainer )
				{
				   	from.SendMessage( "Non puoi inserire contenitori nella cassa." );
				   	return false;					
				}
				
				if( m_item.ItemID == 0x14F0 && 
				    m_item.GetType() != typeof(BankCheck) && 
				    m_item.GetType() != typeof(PowerScroll) &&
				    m_item.GetType() != typeof(StatCapScroll) )
				{
				   	from.SendMessage( "Non puoi inserire Deeds nella cassa." );
				   	return false;						
				}
				
				for( int ind=0; ind<TypesProibiti.Length; ind++ )
				{
					if(m_item.GetType() == TypesProibiti[ind])
					{
					   	from.SendMessage( "Non puoi inserire questo tipo di oggetti nella cassa: " + m_item.GetType().Name );
					   	return false;
					}
				}
				from.SendMessage( "Hai inserito : " + m_item.GetType().Name );
				return true;
			}
			else
			{
				return false;
			}
		}
	    #endregion
	    
	    #region serial-deserial
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
      	
      	#region TypesProibiti
      	private static Type[] TypesProibiti = new Type[]
      	{
      		// Bulk Orders e Libri
//      		typeof(SmallCarpenterBOD),
//      		typeof(SmallFishingBOD),
//      		typeof(SmallSmithBOD),
//      		typeof(SmallTailorBOD),
//      		typeof(LargeCarpenterBOD),
//      		typeof(LargeFishingBOD),
//      		typeof(LargeSmithBOD),
//      		typeof(LargeTailorBOD),    		                            
//       		typeof(BulkOrderBook),
//      		typeof(CarpentryBulkOrderBook),
//       		typeof(FishingBulkOrderBook),
       		
       		// GraniteBox e SeedBox
//      		typeof(GraniteBox),     		                            
//       		typeof(SeedBox),
       		
       		// Runebook
//       		typeof(Runebook),
       		
       		// Oro non in Bankcheck
//       		typeof(Gold),
       		
       		// Navi
//       		typeof(ShipwreckedItem),
       		
       		// Pozze Pagane
//       		typeof(PetResurrectPotion),
//       		typeof(TamlaPotion),
//       		typeof(InvisibilityPotion),
//       		typeof(MightHomericPotion),
//       		typeof(GreaterMightHomericPotion),
       		
       		// Regs Pagani
//       		typeof(TerathanBlood),
//       		typeof(DragonsBlood),
//       		typeof(ExecutionersCap),
//       		typeof(ZoogiFungus),

       		// Powder of traslocation
//       		typeof(TranslocationPowder),
       		
       		// Pietre da paladino
//       		typeof(HonorStone),
//       		typeof(ValorStone),
//       		typeof(SacrificeStone),
//       		typeof(HumiltyStone),
//        		typeof(JusticeStone),
//       		typeof(SpiritualityStone),    		                            
//        		typeof(HonestyStone),
//       		typeof(CompassionStone),
       		
       		// Vari
//			 typeof(KeyRing),
//       		typeof(Seed),
//			typeof(TreasureMap),
//       		typeof(DivineAmulet),
       		
       		// Libri di fede
//       		typeof(BookOfChivalry),
//       		typeof(NecromancerSpellbook),
      	};
      	#endregion
      	     	
      	private class Export : ContextMenuEntry
		{
      		#region campi
      		private MidgardExportChest2 m_Chest;
      		#endregion
      		
      		#region costruttori
      		public Export( MidgardExportChest2 Chest, bool enabled ) : base( 6132, 2 )
			{
				m_Chest = Chest;

				if ( !enabled )
					Flags |= CMEFlags.Disabled;
			}
      		#endregion
      		
      		#region metodi
      		public override void OnClick()
			{
				if ( m_Chest.Deleted )
					return;

				Mobile from = Owner.From;
				
				string NomeFileLog = @"porting/" + "LogUsiPorting.txt";
				string NomeFile = @"porting/" + from.Account.ToString() + ".xml";
				
				if ( from.CheckAlive() )
				{
			    	List<Item> items = m_Chest.Items;
					if( items.Count == 0 )
					{
			    		from.SendMessage( "Non hai messo nessun item nella cassa." );
						return;
					}
			    	XmlTextWriter xmltw = new XmlTextWriter(NomeFile, null);
					xmltw.Formatting = System.Xml.Formatting.Indented;
					xmltw.WriteStartElement("bagaglio", null);
			    	foreach( Item i in items)
			    	{
			    		xmltw.WriteStartElement("item", null);
			    			xmltw.WriteElementString("type", i.GetType().Name);
			    			
			    			#region Artefatti
			    			if( IsMajorArtifact( i ) | IsMinorArtifact( i ) )
			    			{
			    				xmltw.WriteStartElement("Attributi", null);
		    						xmltw.WriteElementString("Amount" , i.Amount.ToString());
		    						if (i.Name != null)
		    							xmltw.WriteElementString("Name" , i.Name);	
			    					if (i.Hue != 0)
			    						xmltw.WriteElementString("Hue" , i.Hue.ToString());
			    					xmltw.WriteElementString("Serial" , i.Serial.ToString());
			    				xmltw.WriteEndElement();
			    			}
			    			#endregion

							#region Statuette
							else if( i is MonsterStatuette )
							{
								MonsterStatuette ms = i as MonsterStatuette;
								xmltw.WriteStartElement("Attributi", null);
									xmltw.WriteElementString("Type" , ms.Type.ToString());
									xmltw.WriteElementString("Serial" , ms.Serial.ToString());
								xmltw.WriteEndElement();			    				
							}
							#endregion
							
							#region BaseGlovesOfMining
			    			else if( i is BaseGlovesOfMining )
			    			{
			    				BaseGlovesOfMining bgm = i as BaseGlovesOfMining ;
			    				xmltw.WriteStartElement("Attributi", null);
			    					xmltw.WriteElementString("Bonus" , bgm.Bonus.ToString());
			    				xmltw.WriteEndElement();
			    			}			    			
			    			#endregion
			    			
			    			#region BaseArmor
			    			else if( i is BaseArmor )
			    			{
			    				BaseArmor ba = i as BaseArmor;		    				
			    				xmltw.WriteStartElement("Attributi", null);			    				
									foreach( int j in Enum.GetValues(typeof( AosArmorAttribute )) )
									{
										if ( ba.ArmorAttributes[(AosArmorAttribute)j] > 0 ) 
										{
											xmltw.WriteElementString( ((AosArmorAttribute)j).ToString(), ba.ArmorAttributes[(AosArmorAttribute)j].ToString());
										}
				    				}

			    					foreach( int j in Enum.GetValues(typeof( AosAttribute )) )
									{
										if ( ba.Attributes[(AosAttribute)j] > 0 ) 
										{
											xmltw.WriteElementString( ((AosAttribute)j).ToString(), ba.Attributes[(AosAttribute)j].ToString());
										}					
									}
		
				    				if (ba.ColdBonus != 0)
				    					xmltw.WriteElementString("ColdBonus", ba.ColdBonus.ToString());
				    				
			    					if (ba.Crafter != null)
			    						xmltw.WriteElementString("Crafter" , ba.Crafter.Name);
			    					
			    					if (ba.DexBonus != 0)
			    						xmltw.WriteElementString("DexBonus" , ba.DexBonus.ToString());
			    					
			    					if (ba.DexRequirement != 0)
			    						xmltw.WriteElementString("DexRequirement" , ba.DexRequirement.ToString());
			    					
			    					if (ba.EnergyBonus != 0)
			    						xmltw.WriteElementString("EnergyBonus", ba.EnergyBonus.ToString());
			    					
			    					if (ba.FireBonus != 0)
			    						xmltw.WriteElementString("FireBonus", ba.FireBonus.ToString());	
			    					
			    					xmltw.WriteElementString("HitPoints", ba.HitPoints.ToString());
			    					
			    					if (ba.IntBonus != 0)
			    						xmltw.WriteElementString("IntBonus", ba.IntBonus.ToString());
			    					
			    					if (ba.IntRequirement != 0)
			    						xmltw.WriteElementString("IntRequirement" , ba.IntRequirement.ToString());
			    					
			    					xmltw.WriteElementString("MaxHitPoints", ba.MaxHitPoints.ToString());
			    					
			    					if (ba.PhysicalBonus != 0)
				    					xmltw.WriteElementString("PhysicalBonus" , ba.PhysicalBonus.ToString());
			    					
				    				if (ba.PlayerConstructed == true)
				    					xmltw.WriteElementString("PlayerConstructed" , ba.PlayerConstructed.ToString());
				    				
				    				if (ba.PoisonBonus != 0)
				    					xmltw.WriteElementString("PoisonBonus", ba.PoisonBonus.ToString());
				    				
			    					xmltw.WriteElementString("Quality" , ba.Quality.ToString());		
			    					
				    				xmltw.WriteElementString("Resource" , ba.Resource.ToString());
				    				
				    				xmltw.WriteElementString("Serial" , ba.Serial.ToString());
				    				
									SkillName skill;
									double bonus;
									
									for( int j=0; j<5; j++ )
									{
										ba.SkillBonuses.GetValues( j, out skill, out bonus );
										if ( bonus > 0 ) 
										{
											xmltw.WriteElementString( "Skill_" + (j+1) + "_Name" , skill.ToString() );
											xmltw.WriteElementString( "Skill_" + (j+1) + "_Value" , bonus.ToString() );
										}					
									}	
				    				
				    				if (ba.StrBonus != 0)
			    						xmltw.WriteElementString("StrBonus" , ba.StrBonus.ToString());
				    				
			    					if (ba.StrRequirement != 0)
			    						xmltw.WriteElementString("StrRequirement" , ba.StrRequirement.ToString());
			    								    			
				    				if (ba.Name != null)
			    						xmltw.WriteElementString("Name" , ba.Name);
				    				
			    					if (ba.Hue != 0)
			    						xmltw.WriteElementString("Hue" , ba.Hue.ToString());
				    			xmltw.WriteEndElement();
			    			}
			    			#endregion
			    			
			    			#region BaseClothing
			    			else if( i is BaseClothing )
			    			{ 
			    				BaseClothing bc = i as BaseClothing;		    				
				    			xmltw.WriteStartElement("Attributi", null);
				    				
				    				if (bc.Crafter != null)
				    					xmltw.WriteElementString("Crafter" , bc.Crafter.Name );
				    				
			    					if (bc.PlayerConstructed == true)
			    						xmltw.WriteElementString("PlayerConstructed" , bc.PlayerConstructed.ToString());
	
			    					xmltw.WriteElementString("Quality" , bc.Quality.ToString());
				    				
			    					if (bc.LootType == LootType.Blessed)
				    					xmltw.WriteElementString("LootType" , bc.LootType.ToString() );
			    									    				
				    				if (bc.Name != null)
				    					xmltw.WriteElementString("Name" , bc.Name);
				    				
				    				if (bc.Hue != 0)
				    					xmltw.WriteElementString("Hue" , bc.Hue.ToString());
				    				
				    				xmltw.WriteElementString("Serial" , bc.Serial.ToString());
				    				
			    				xmltw.WriteEndElement();
			    			}
			    			#endregion
			    			
			    			#region BaseWeapon
			    			else if( i is BaseWeapon)
			    			{
			    				BaseWeapon bw = i as BaseWeapon;		    				
			    				xmltw.WriteStartElement("Attributi", null);
			    				
			    					foreach( int j in Enum.GetValues(typeof( AosAttribute )) )
									{
										if ( bw.Attributes[(AosAttribute)j] > 0 ) 
										{
											xmltw.WriteElementString( ((AosAttribute)j).ToString(), bw.Attributes[(AosAttribute)j].ToString());
										}					
									}
				    				
				    				if (bw.Crafter != null)
			    						xmltw.WriteElementString("Crafter" , bw.Crafter.Name);
				    				
					    			if (bw.DexRequirement != 0)
			    						xmltw.WriteElementString("DexRequirement" , bw.DexRequirement.ToString());
					    			
									xmltw.WriteElementString("Hits" , bw.HitPoints.ToString());
					    			
					   				if (bw.IntRequirement != 0)
			    						xmltw.WriteElementString("IntRequirement" , bw.IntRequirement.ToString());
					   				
									xmltw.WriteElementString("MaxDamage" , bw.MaxDamage.ToString());
					   				
									xmltw.WriteElementString("Hits" , bw.MaxHitPoints.ToString());
					 				
									xmltw.WriteElementString("MinDamage" , bw.MinDamage.ToString());
					   				
					   				if (bw.PlayerConstructed == true)
				    					xmltw.WriteElementString("PlayerConstructed" , bw.PlayerConstructed.ToString());
					   				
			    					xmltw.WriteElementString("Quality" , bw.Quality.ToString());
					   				
				    				xmltw.WriteElementString("Resource" , bw.Resource.ToString());
				    				
				    				xmltw.WriteElementString("Serial" , bw.Serial.ToString());
				    				
				    				if (bw.Slayer > SlayerName.None)
				    					xmltw.WriteElementString("Slayer" , bw.Slayer.ToString());
	
				    				xmltw.WriteElementString("Speed" , bw.Speed.ToString());
				    				
				    				xmltw.WriteElementString("StrRequirement" , bw.StrRequirement.ToString());
	
									foreach( int j in Enum.GetValues(typeof( AosWeaponAttribute )) )
									{
										if ( bw.WeaponAttributes[(AosWeaponAttribute)j] > 0 ) 
										{
											xmltw.WriteElementString( ((AosWeaponAttribute)j).ToString(), bw.WeaponAttributes[(AosWeaponAttribute)j].ToString());
										}
				    				}
				    				if (bw.Name != null)
			    						xmltw.WriteElementString("Name" , bw.Name);	
				    				
			    					if (bw.Hue != 0)
			    						xmltw.WriteElementString("Hue" , bw.Hue.ToString());		    					
				    			xmltw.WriteEndElement();
			    			}
			    			#endregion
			    			
			    			#region BaseRunicTool
			    			else if( i is BaseRunicTool)
			    			{
			    				BaseRunicTool brt = i as BaseRunicTool;
			    				xmltw.WriteStartElement("Attributi", null);
				    				xmltw.WriteElementString("Resource" , brt.Resource.ToString());
				    				xmltw.WriteElementString("Serial" , brt.Serial.ToString());
				    				xmltw.WriteElementString("UsesRemaining" , brt.UsesRemaining.ToString());
				    			xmltw.WriteEndElement();
			    			}
			    			#endregion
			    			
			    			#region BaseJewel
				    		else if( i is BaseJewel)
			    			{
			    				BaseJewel bj = i as BaseJewel;			    				
			    				xmltw.WriteStartElement("Attributi", null);
			    				
			    					foreach( int j in Enum.GetValues(typeof( AosAttribute )) )
									{
										if ( bj.Attributes[(AosAttribute)j] > 0 ) 
										{
											xmltw.WriteElementString( ((AosAttribute)j).ToString(), bj.Attributes[(AosAttribute)j].ToString());
										}					
									}	
			    					
			    					if( bj.GemType > GemType.None )
			    						xmltw.WriteElementString("GemType" , bj.GemType.ToString());
			    					
				    				xmltw.WriteElementString("PhysicalResistance" , bj.Resistances.Physical.ToString());
				    				
				    				xmltw.WriteElementString("FireResistance", bj.Resistances.Fire.ToString());
				    				
				    				xmltw.WriteElementString("ColdResistance", bj.Resistances.Cold.ToString());
				    				
				    				xmltw.WriteElementString("PoisonResistance", bj.Resistances.Poison.ToString());
				    				
				    				xmltw.WriteElementString("EnergyResistance", bj.Resistances.Energy.ToString());
				    				
			    					xmltw.WriteElementString("Resource" , bj.Resource.ToString());
			    					
			    					xmltw.WriteElementString("Serial" , bj.Serial.ToString());
			    					
									SkillName skill;
									double bonus;
									
									for( int j=0; j<5; j++ )
									{
										bj.SkillBonuses.GetValues( j, out skill, out bonus );
										if ( bonus > 0 ) 
										{
											xmltw.WriteElementString( "Skill_" + (j+1) + "_Name" , skill.ToString() );
											xmltw.WriteElementString( "Skill_" + (j+1) + "_Value" , bonus.ToString() );
										}					
									}	
									
									if (bj.Name != null)
			    						xmltw.WriteElementString("Name" , bj.Name);	
				    				
			    					if (bj.Hue != 0)
			    						xmltw.WriteElementString("Hue" , bj.Hue.ToString());
			    					
		    					xmltw.WriteEndElement();
				    		}
			    			#endregion
			    			
			    			#region Sop e Sos
			    			else if( i is PowerScroll)
			    			{
			    				PowerScroll ps = i as PowerScroll;			    				
			    				xmltw.WriteStartElement("Attributi", null);
			    					xmltw.WriteElementString("Serial" , ps.Serial.ToString());
			    					xmltw.WriteElementString("Skill" , ps.Skill.ToString());
				    				xmltw.WriteElementString("Value" , ps.Value.ToString());
			    				xmltw.WriteEndElement();
			    			}
			    			
			    			else if( i is StatCapScroll)
			    			{
			    				StatCapScroll scs = i as StatCapScroll;		    				
			    				xmltw.WriteStartElement("Attributi", null);
			    					xmltw.WriteElementString("Serial" , scs.Serial.ToString());
				    				xmltw.WriteElementString("Value" , scs.Value.ToString());
			    				xmltw.WriteEndElement();
			    			}
							#endregion
							
							#region AncientSmithyHammer
							else if( i is AncientSmithyHammer)
							{
								AncientSmithyHammer ash = i as AncientSmithyHammer;		
								xmltw.WriteStartElement("Attributi", null);
									xmltw.WriteElementString("Bonus" , ash.Bonus.ToString());
									xmltw.WriteElementString("Serial" , ash.Serial.ToString());
									xmltw.WriteElementString("UsesRemaining" , ash.UsesRemaining.ToString());
								xmltw.WriteEndElement();
							}
			    			#endregion
			    			
			    			#region BankCheck
			    			else if( i is BankCheck)
			    			{
			    				BankCheck bc = i as BankCheck;
			    				xmltw.WriteStartElement("Attributi", null);
			    					xmltw.WriteElementString("Serial" , bc.Serial.ToString());
			    					xmltw.WriteElementString("Worth" , bc.Worth.ToString());
			    				xmltw.WriteEndElement();
			    			}
			    			#endregion
			    			
							#region PetPorting
							else if( i is PetPorting)
							{
								PetPorting pp = i as PetPorting;
								xmltw.WriteStartElement("Attributi", null);
									xmltw.WriteElementString("Filled" , pp.Filled.ToString());
									xmltw.WriteElementString("PetBonded" , pp.PetBonded.ToString());
									xmltw.WriteElementString("PetControled" , pp.PetControled.ToString());
									xmltw.WriteElementString("PetControlMasterName" , pp.PetControlMasterName.ToString());
									xmltw.WriteElementString("PetHue" , pp.PetHue.ToString());
									xmltw.WriteElementString("PetName" , pp.PetName.ToString());
									xmltw.WriteElementString("PetTypeString" , pp.PetTypeString.ToString());
									xmltw.WriteElementString("Serial" , pp.Serial.ToString());
								xmltw.WriteEndElement();
							}
							#endregion
							
							#region RecallRune
							else if( i is RecallRune)
							{
								RecallRune rr = i as RecallRune;
								xmltw.WriteStartElement("Attributi", null);
									if (rr.Description != null)
										xmltw.WriteElementString("Description" , rr.Description.ToString());
									if (rr.House != null)
										xmltw.WriteElementString("House" , rr.House.ToString());
									xmltw.WriteElementString("Marked" , rr.Marked.ToString());
									xmltw.WriteElementString("Target" , rr.Target.ToString());
									if (rr.TargetMap != null)
										xmltw.WriteElementString("TargetMap" , rr.TargetMap.ToString());
									xmltw.WriteElementString("Serial" , rr.Serial.ToString());
								xmltw.WriteEndElement();
							}
							#endregion
							
			    			#region ChampionSkull
							else if( i is ChampionSkull)
							{
								ChampionSkull cs = i as ChampionSkull;
								xmltw.WriteStartElement("Attributi", null);
									xmltw.WriteElementString("Serial" , cs.Serial.ToString());
									xmltw.WriteElementString("Type" , cs.Type.ToString());
								xmltw.WriteEndElement();
							}			    			
			    			#endregion
			    				    			
			    			#region Item
			    			else
			    			{
				    			xmltw.WriteStartElement("Attributi", null);		
				    				if (i.Amount > 1)
			    						xmltw.WriteElementString("Amount" , i.Amount.ToString());	
				    				if (i.Hue != 0)
			    						xmltw.WriteElementString("Hue" , i.Hue.ToString());	
				    				xmltw.WriteElementString("itemID" , i.ItemID.ToString());
			    					if (i.LootType == LootType.Blessed)
			    						xmltw.WriteElementString("LootType" , i.LootType.ToString()); 
				    				if (i.Name != null)
			    						xmltw.WriteElementString("Name" , i.Name);	
				    				xmltw.WriteElementString("Serial" , i.Serial.ToString());    					
				    			xmltw.WriteEndElement();
			    			}
			    			#endregion
						xmltw.WriteEndElement();    
			    	}
			    	xmltw.WriteEndElement();
			    	xmltw.Flush();
					xmltw.Close();
					from.SendMessage( "Il file " + from.Account.ToString() + ".xml" + " è stato creato con successo nella cartella Porting." );
					
					TextWriter tw = File.AppendText(NomeFileLog);
					try
					{
						tw.WriteLine( "L'utente: " + from.Name + " (Account: " + from.Account + " ) ha creato il file: " + 
						             from.Account.ToString() + ".xml alle ore: " + DateTime.Now.ToShortTimeString() + " del " +
						              DateTime.Now.Date.ToShortDateString() + "." );
					}
					finally
					{
						tw.Close();
					}
				}
			}
      		
      		private bool IsMajorArtifact( object i)
      		{
      			Item m_oggetto = i as Item;
      			
      			for( int ind=0; ind<MidgardMajorArtefatti.Length; ind++)
      			{
      				if(m_oggetto.GetType() == MidgardMajorArtefatti[ind])
      					return true;		
      			}
      			return false;
      		}
      		
      		private bool IsMinorArtifact( object i)
      		{
      			Item m_oggetto = i as Item;
      			
      			for( int ind=0; ind<MidgardMinorArtefatti.Length; ind++)
      			{
      				if(m_oggetto.GetType() == MidgardMinorArtefatti[ind])
      					return true;		
      			}
      			return false;
      		}
      		#endregion
      		
      		#region MidgardArtefatti
			private static Type[] MidgardMajorArtefatti = new Type[]
			{		
	    		// Armors
	    		typeof(ArmorOfFortune),
	    		typeof(GauntletsOfNobility),
				typeof(HatOfTheMagi),
				typeof(HelmOfInsight),
				typeof(HolyKnightsBreastplate),
				typeof(InquisitorsResolution),
				typeof(JackalsCollar),
				typeof(LeggingsOfBane),
				typeof(MidnightBracers),
				typeof(OrnateCrownOfTheHarrower),
				typeof(ShadowDancerLeggings),
				typeof(TunicOfFire),
				typeof(VoiceOfTheFallenKing),
				// Shields
				typeof(Aegis),
				typeof(ArcaneShield),
				// Weapons
				typeof(AxeOfTheHeavens),
				typeof(BladeOfInsanity),
				typeof(BladeOfTheRighteous),
				typeof(BoneCrusher),
				typeof(BreathOfTheDead),
				typeof(Frostbringer),
				typeof(LegacyOfTheDreadLord),
				typeof(SerpentsFang),
				typeof(StaffOfTheMagi),
				typeof(TheBeserkersMaul),
				typeof(TheDragonSlayer),
				//typeof(TheDryadBow),
				typeof(TheTaskmaster),
				typeof(TitansHammer),
				typeof(ZyronicClaw),
				// Jewels
				typeof(BraceletOfHealth),
				typeof(OrnamentOfTheMagician),
				typeof(RingOfTheElements),				
				typeof(RingOfTheVile),	
			};
			
			private static Type[] MidgardMinorArtefatti = new Type[]
			{
				// Armors
				typeof(GlovesOfThePugilist),
				typeof(HeartOfTheLion), 
				typeof(VioletCourage), 
				// Shields
				// Weapons
				typeof(ArcticDeathDealer), 
				typeof(BlazeOfDeath),
				typeof(BowOfTheJukaKing),
				typeof(CavortingClub),		
				typeof(EnchantedTitanLegBone), 
				typeof(NightsKiss),
				typeof(NoxRangersHeavyCrossbow), 			   
				typeof(StaffOfPower),
				typeof(WrathOfTheDryad),
				// Jewels
				typeof(AlchemistsBauble),
				// Deco
				typeof(GoldBricks),
			};
			#endregion
      	}
      	
      	private class ListaVietati : ContextMenuEntry
      	{
      		#region campi
      		private MidgardExportChest2 m_Chest;
      		#endregion	
      		
      		#region costruttori
      		public ListaVietati( MidgardExportChest2 Chest, bool enabled ) : base( 6134, 2 )
			{
				m_Chest = Chest;

				if ( !enabled )
					Flags |= CMEFlags.Disabled;
			}
      		#endregion
      		
      		#region metodi
      		public override void OnClick()
			{
				if ( m_Chest.Deleted )
					return;
				
				Mobile from = Owner.From;
				
				from.SendMessage( "Non puoi inserire i seguenti tipi di oggetti nella cassa:" );
				from.SendMessage( "Ogni tipo di Deed che non sia un BankCheck" );
				for( int ind=0; ind<TypesProibiti.Length; ind++ )
				{
					from.SendMessage( TypesProibiti[ind].Name );
				}
      		}
      		#endregion
      	}
   	} 
} 
