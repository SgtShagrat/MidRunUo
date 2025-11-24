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

using System;
using System.IO;
using System.Xml;

using Server;
using Server.Commands;
using Server.Spells;
using Server.Spells.Chivalry;

namespace Midgard.Commands
{
	public class ListSpells
	{
		#region registrazione
		public static void Initialize()
		{
			CommandSystem.Register( "ListSpells" , AccessLevel.Developer, new CommandEventHandler( ListSpells_OnCommand ) );
		}
		#endregion
	
		#region callback
		[Usage( "ListSpells" )]
		[Description( "Serialize SpellDef" )]
		public static void ListSpells_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			if( from == null  )
				return;
			
			string FileName = "DefaultSpellsAvailable.xml";
			
  			XmlDocument Doc = new XmlDocument();
  			
  			if ( !File.Exists(FileName) )
  			{
  				XmlTextWriter XmlTw = new XmlTextWriter(FileName, null);
				XmlTw.Formatting = System.Xml.Formatting.Indented;
				XmlTw.WriteStartElement( "SpellListData" );
					XmlTw.WriteStartElement( "Spells" );
						XmlTw.WriteElementString("EoF", "----------");
					XmlTw.WriteEndElement();
				XmlTw.WriteEndElement();
				XmlTw.Flush();
				XmlTw.Close();
  			}
  			
  			FileStream FsLoadXml = new FileStream( FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite );
  			try
  			{
				Doc.Load( FsLoadXml );
  			}
  			catch
  			{
  				from.SendMessage( "Operazione non riuscita: file gia' in uso." );
  			}
  			
			XmlElement Root = Doc.DocumentElement;
			FsLoadXml.Close();
			
			int NumSpells = Root.GetElementsByTagName( "EoF" ).Count;
			Type[] spellArray = Server.Spells.SpellRegistry.Types;
			
			for( int i = 0; i < spellArray.Length; i++ )
			{
				if( spellArray[i] != null )
				{
					Spell spell = SpellRegistry.NewSpell( ((Type)spellArray[i]).Name, from, null );
					if( spell != null )
					{
						Console.WriteLine( "{0} {1} {2}", spell.Name, spell.Mantra, spell.CastSkill.ToString() );
					
						XmlElement newSpellDef = Doc.CreateElement( "SpellDef" );
						
					    XmlElement name = Doc.CreateElement( "Name" );
			      		name.InnerText = spell.Name;
			      		newSpellDef.AppendChild( name );
			      		
					    XmlElement mantra = Doc.CreateElement( "Mantra" );
			      		mantra.InnerText = spell.Mantra;
			      		newSpellDef.AppendChild( mantra );
			      		
					    XmlElement manaReq = Doc.CreateElement( "ManaReq" );
					    if( spell is PaladinSpell )
					    	manaReq.InnerText = (((PaladinSpell)spell).RequiredMana).ToString();
					    else
					    	manaReq.InnerText = spell.GetMana().ToString();
			      		newSpellDef.AppendChild( manaReq );
			      		
					    XmlElement skillReq = Doc.CreateElement( "SkillReq" );
					    if( spell is PaladinSpell )
					    	skillReq.InnerText = string.Format( "{0:F0}", ((PaladinSpell)spell).RequiredSkill );
					    else
					    {
					    	double tmpMin = 0; 
					    	double tmpMax = 0;
					    	spell.GetCastSkills( out tmpMin, out tmpMax );
					    	if( tmpMin < 0.0 )
					    		tmpMin = 0.0;
					    	
					    	skillReq.InnerText = string.Format( "{0:F0}", tmpMin );
					    }
			      		newSpellDef.AppendChild( skillReq );
			      		
					    XmlElement regs = Doc.CreateElement( "Reagents" );
					    SpellInfo si = spell.Info;
					    for( int j = 0; j < si.Reagents.Length; j++ )
					    {
					    	Console.WriteLine( si.Reagents.Length );
					    	if( si.Reagents.Length > 0 )
					    	{
					    		if( si.Reagents[j] != null )
					    		{
							    	XmlElement reg = Doc.CreateElement( "Reagent" );
							    	reg.SetAttribute( "Name", ((Type)si.Reagents[j]).Name );
							    	reg.SetAttribute( "Type", ((Type)si.Reagents[j]).Name );
							    	reg.SetAttribute( "Quantity", "1" );
							    	regs.AppendChild( reg );
					    		}
						    }
					    }
			      		newSpellDef.AppendChild( regs );
			      		
					    XmlElement iconName = Doc.CreateElement( "IconName" );
			      		iconName.InnerText = ((Type)spellArray[i]).Name + ".bmp";
			      		newSpellDef.AppendChild( iconName );
			      		
					    XmlElement tableId = Doc.CreateElement( "TableId" );
					    tableId.InnerText = i.ToString();
			      		newSpellDef.AppendChild( tableId );
			      		
						Doc.DocumentElement.InsertAfter( newSpellDef , Root.LastChild );
      		
      					FileStream FsSaveXml = new FileStream( FileName, FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);
      					Doc.Save(FsSaveXml);
      					FsSaveXml.Close();
					}
				}
			}
		}
		#endregion
	}
}
