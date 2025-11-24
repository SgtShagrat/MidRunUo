/***************************************************************************
 *                                  ShrinkItem.cs
 *                            		-------------------
 *  begin                	: Agosto, 2006
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 *  L'oggetto permette lo Shrink e l'Un-Shrink dei pet di Midgard.
 *  
 ***************************************************************************/

using System;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using System.Collections;
using Server.ContextMenus;
using Server.Targeting;
using System.IO;
using System.Text;

namespace Server.Items
{
   	public class PetPorting : Item
   	{
   		#region Campi
   		private bool m_Filled;
   		private int m_PetHue;
   		private string m_PetName;
   		private bool m_PetControled;
   		private string m_PetControlMasterName;
   		private bool m_PetIsBonded;
   		private string m_PetTypeString;
   		#endregion
   		
   		#region Proprietà
		[CommandProperty( AccessLevel.Administrator )]
		public bool Filled
		{
			get{ return m_Filled; }
			set{ m_Filled = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.Administrator )]
		public int PetHue
		{
			get{ return m_PetHue; }
			set{ m_PetHue = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.Administrator )]
		public string PetName
		{
			get{ return m_PetName; }
			set{ m_PetName = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.Administrator )]
		public bool PetControled
		{
			get{ return m_PetControled; }
			set{ m_PetControled = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.Administrator )]
		public string PetControlMasterName
		{
			get{ return m_PetControlMasterName; }
			set{ m_PetControlMasterName = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.Administrator )]
		public bool PetBonded
		{
			get{ return m_PetIsBonded; }
			set{ m_PetIsBonded = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.Administrator )]
		public string PetTypeString
		{
			get{ return m_PetTypeString; }
			set{ m_PetTypeString = value; InvalidateProperties(); }
		}
   		#endregion
   		
   		#region Cotruttori
   		[Constructable]
      	public PetPorting() : base()
      	{
      		this.m_Filled = false;
			this.m_PetControled = false;
			this.m_PetControlMasterName = string.Empty;
			this.m_PetHue = 0;
			this.m_PetIsBonded = false;
			this.m_PetName = string.Empty;
			this.m_PetTypeString = string.Empty;
			
			ItemID = 0x1870;
			Name = "a pet gem";
 			Movable = true;
 			Hue = 1152;
 			LootType=LootType.Blessed;
			Weight = 25.0;
      	}
      	
      	public PetPorting( Serial serial ) : base( serial )
		{
		}
      	#endregion
 
      	#region Metodi
  		public override void OnDoubleClick( Mobile from )
  		{
  			PlayerMobile From = from as PlayerMobile;
  			
  			if( From == null )
  				return;
  			
  			if( !IsChildOf( From.Backpack ) )
			{
				From.SendMessage( "La gemma deve essere nel tuo zaino per essere usata." );
				return;
			}
  			
  			if( m_Filled == true )
  			{
  				from.SendMessage( "Stai tentando di evocare l'animale nella gemma." );
  				if( m_PetControlMasterName == from.Name )
  				{
  					from.SendMessage( "L'animale nella gemma ti riconosce come suo Padrone." );
					if( m_PetTypeString != string.Empty )
					{				
						Type type = SpawnerType.GetType( m_PetTypeString );
						object o = Activator.CreateInstance( type );
    					BaseCreature NewPet = o as BaseCreature;
				   		NewPet.Hue	= this.m_PetHue;
				   		NewPet.Name = this.m_PetName;
				   		NewPet.Controlled = this.m_PetControled;
				   		NewPet.ControlMaster = from;
				   		NewPet.IsBonded = this.m_PetIsBonded;
				   		NewPet.MoveToWorld( from.Location, from.Map );
				   		this.Delete();
				   		return;
					}
  				}
  				else
  				{
  					from.SendMessage( "L'animale nella gemma si rifiuta di uscire perchè non ti appartiene." );
  				}
  				return;
  			}

			From.BeginTarget( 2, false, TargetFlags.None, new TargetCallback( OnTarget ) );
			From.SendMessage( "Seleziona l'animale che vuoi far entrare nella gemma." );
  		}
  		
  		public void OnTarget( Mobile from, object obj )
		{
  			if( this.Deleted )
  				return;
  			
  			BaseCreature pet = obj as BaseCreature;
  			if( pet == null )
  				return;	
  			
  			if( from != pet.ControlMaster | !pet.Controlled )
  			{
  				from.SendMessage( "La creatura si rifiuta di entrare nella gemma perchè non " +
  				                  "sei il suo Padrone.");
  				return;
  			}
  			
  			// Setta le props della statuetta
	   		this.m_Filled = true;
	   		this.m_PetHue = pet.Hue;
	   		this.m_PetName = pet.Name;
	   		this.m_PetControled = pet.Controlled;
	   		this.m_PetControlMasterName = from.Name;
	   		this.m_PetIsBonded = pet.IsBonded;
	   		this.m_PetTypeString = pet.GetType().Name;
	   		
	   		string NomeFileLog = @"monoaccount/" + "LogUsiShrinkItem.txt";
	   		TextWriter tw = File.AppendText(NomeFileLog);
	   		try
			{
				tw.WriteLine( "L'utente: " + from.Name + " (Account: " + from.Account + " ) ha usato uno ShrinkItem" +
				           	  " alle ore: " + DateTime.Now.ToShortTimeString() + " del " +
				              DateTime.Now.Date.ToShortDateString() + " per shrinkare un " + this.m_PetTypeString + 
				              " di nome -" + this.m_PetName + "- e di colore -" + this.m_PetHue.ToString() + "-." );
			}
			finally
			{
				tw.Close();
			}
			
	   		pet.Delete();
	   		InvalidateProperties();
  		}
  		
  		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			 
			if ( this.m_Filled )
			{
				list.Add( 1060747 );
				if( m_PetName != string.Empty )
					list.Add( 1060658, "Pet Name\t{0}" , m_PetName );
				if( m_PetTypeString != string.Empty )
					list.Add( 1060659, "Pet Type\t{0}" , m_PetTypeString );
				if( m_PetHue != 0 )
					list.Add( 1060660, "Pet Hue\t{0}" , m_PetHue.ToString() );
			}
			else
			{
				list.Add( 1074036 );
			}
		}
  		#endregion
  		
  		#region Serial-Deserial
  		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
			writer.Write( (bool) m_Filled ); 
			writer.Write( (int) m_PetHue );
			writer.Write( (string) m_PetName );
			writer.Write( (bool) m_PetControled );
			writer.Write( (string) m_PetControlMasterName );                         
			writer.Write( (bool) m_PetIsBonded );                          
			writer.Write( (string) m_PetTypeString );                         		
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			m_Filled = (bool)reader.ReadBool();
			m_PetHue = (int)reader.ReadInt();
			m_PetName = (string)reader.ReadString();
			m_PetControled = (bool)reader.ReadBool();
			m_PetControlMasterName = (string)reader.ReadString();
			m_PetIsBonded = (bool)reader.ReadBool();
			m_PetTypeString = (string)reader.ReadString();
		}
		#endregion
	}
}
