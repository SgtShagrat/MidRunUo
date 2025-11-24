/***************************************************************************
 *								  EnchantStone.cs
 *									---------------
 *  begin					: Ottobre, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright				: Midgard Uo Shard - Matteo Visintin
 *  email					: tocasia@alice.it
 *
 ***************************************************************************/

#define DebugEnchantStone

using System;
using System.IO;

using Server;
using Server.Items;
using Server.Network;
using Server.Targeting;

namespace Midgard.Engines.StoneEnchantSystem
{
	public class EnchantStone : Item
	{
		public static readonly int MaxStoneLevel = 5;
		private const double EnchantDurationPerLevel = 2.0;
		private const double AnimateDelay = 1.5;

		private bool m_Identified;
		private StoneTypes m_StoneType;

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
		public bool Identified
		{
			get { return m_Identified; }
			set
			{
				bool oldValue = m_Identified;
				if( oldValue != value )
				{
					m_Identified = value;
					OnIdentifiedChanged( oldValue );
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
		public StoneTypes StoneType
		{
			get { return m_StoneType; }
			set
			{
				StoneTypes oldValue = m_StoneType;
				if( m_StoneType != value )
				{
					m_StoneType = value;
					OnStoneTypeChanged( oldValue );
				}
			}
		}

		public StoneDefinition Definition{get { return StoneDefinition.GetFromIndex( (int)StoneType - 1 ); }}
		public override bool DisplayWeight{get { return false; }}

		[Constructable]
		public EnchantStone() : this( StoneEnchantHelper.RandomStoneType() )
		{
		}

		[Constructable]
		public EnchantStone( StoneTypes stoneType ) : base( 0x1EA7 )
		{
			Weight = 1.0;

			m_Identified = false;
			m_StoneType = stoneType;
		}

		public EnchantStone( Serial serial ) : base( serial )
		{
		}

		private static readonly int[] AnimIds = new int[]{245, 266};

		public virtual void OnIdentification( Mobile from, bool successed, bool destroy )
		{
			if( Deleted )
				return;

			if( successed )
			{
				from.CheckSkill( SkillName.ItemID, 0.01 );
				from.CheckSkill( SkillName.ItemID, 0.01 );
				from.CheckSkill( SkillName.ItemID, 0.01 );//bonus rawpoints
				from.SendMessage( from.Language == "ITA" ? "{0}, grazie alla tua conoscenza e ad un pizzico di fortuna identifichi con successo questa gemma!" : "{0}, with your knowledge and some luck you have successfully identified this powerfull gem!", from.Name );
				Identified = true;
#if DebugEnchantStone
				Config.SendDebug( "DebugEnchantStone: OnIdentification called. if( successed ) == true." );
#endif
			}
			else
			{
				if( destroy )
				{
#if DebugEnchantStone
					Config.SendDebug( "DebugEnchantStone: OnIdentification called. if( destroy ) == true." );
#endif
					from.SendMessage( from.Language == "ITA" ? "{0}, hai danneggiato la gemma. Si rompe e dissolve in una nube di polvere!" : "{0}, you damaged this gem. It crasks and dissolves in a pile of dust!", from.Name );
					Delete();
				}
				else
				{
#if DebugEnchantStone
					Config.SendDebug( "DebugEnchantStone: OnIdentification called. if( destroy ) == false." );
#endif
					from.SendMessage( from.Language == "ITA" ? "{0}, non sei riuscito ad identificare la gemma. Fortunatamente non sembra essersi danneggiata!" : "{0}, you failed to get this gem identified. Luckly it seems not damaged from your identification process!", from.Name );
				}
			}
		}

		public virtual void OnStoneTypeChanged( StoneTypes oldValue )
		{
			if( m_StoneType != oldValue )
			{
				Hue = StoneEnchantHelper.GetHueFromStoneType( m_StoneType );
				InvalidateProperties();
				Delta( ItemDelta.Update );
			}
		}

		public virtual void OnIdentifiedChanged( bool oldValue )
		{
			Hue = !oldValue ? StoneEnchantHelper.GetHueFromStoneType( m_StoneType ) : 0;

			InvalidateProperties();
			Delta( ItemDelta.Update );
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			string identName = String.Format( "a powerfull {0} gem", m_StoneType );
			list.Add( ( Identified ) ? identName : "a strange gem" );
		}

		public override void OnSingleClick( Mobile from )
		{
			if( Identified )
			{
				string effect = StoneEnchantHelper.GetPrefixFromStoneType( m_StoneType );

				if( effect.Length > 0 )
					LabelTo( from, String.Format( from.Language == "ITA" ? "gemma magica {0}" : "a {0} magic gem", effect ) );
				else
					LabelTo( from, from.Language == "ITA" ? "gemma magica" : "a magic gem" );
			}
			else
				LabelTo( from, from.Language == "ITA" ? "una strana gemma" : "a strange gem" );
		}

		public void DisplayInfo( Mobile to )
		{
			if( Definition != null && Definition.Suffix != null )
				to.SendMessage( "This is magic with {0} power on it.", Definition.Suffix );
		}

		public void LogContruct( Mobile from, Container container )
		{
			try
			{
				string message = String.Format( "Type {0} - From: {1} ({2}) - Cont: {3} - Location {4}", StoneType, from.Name ?? "", from.GetType().Name, container.GetType().Name, from.Location );

				using( StreamWriter op = new StreamWriter( "Logs/enchant-stones.log", true ) )
					op.WriteLine( "{0}\t{1}", DateTime.Now, message );
			}
			catch
			{
			}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if( IsChildOf( from.Backpack ) )
			{
				if( Identified )
				{
					if( m_StoneType == StoneTypes.None )
						from.SendMessage( "This strange gem appears to be magical but...seem has no other effect" );
					else
					{
						from.BeginTarget( 2, false, TargetFlags.None, new TargetCallback( OnTarget ) );
						from.SendMessage( from.Language == "ITA" ? "Seleziona un oggetto da incantare..." : "Select an item to enchant..." );
					}
				}
				else
					from.SendMessage( from.Language == "ITA" ? "Questa strana gemma sembra essere magica ma... non sembra avere un particolare effetto." : "This strange gem appears to be magical but...seem has no other effect." );
			}
			else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
		}

		public void OnTarget( Mobile from, object obj )
		{
			Config.SendDebug( "Debug Enchant: Type {0}", obj.GetType() );

			if( !IsChildOf( from.Backpack ) )
				from.SendMessage( from.Language == "ITA" ? "{0}, la gemma deve essere nel tuo zaino.." : "{0}, that gem must be in you pack to be used...", from.Name );
			else if( !( obj is Item ) )
			{
				from.SendMessage( from.Language == "ITA" ? "Questo oggetto non può essere incantato." : "This item cannot be enchanted with any stone magical power." );
				Config.SendDebug( "obj is not Item" );
			}
			else if( !( obj is IStoneEnchantItem ) )
			{
				from.SendMessage( from.Language == "ITA" ? "Questo oggetto non può essere incantato." : "This item cannot be enchanted with any stone magical power." );
				Config.SendDebug( "obj is not IStoneEnchantItem" );
			}
			else
			{
				Item item = (Item)obj;
				CraftAttributeInfo info = null;

				if( !item.IsChildOf( from.Backpack ) )
					from.SendMessage( from.Language == "ITA" ? "L'oggetto da incantare deve essere nel tuo zaino!" : "You cannot use the gem because item to be enchanted must be in your pack." );
				else
				{
					if( !StoneEnchantHelper.CanBeEnchanted( item ) )
					{
						from.SendMessage( from.Language == "ITA" ? "Questo oggetto non può essere incantato." : "This item cannot be enchanted." );
						Config.SendDebug( "!StoneEnchantHelper.CanBeEnchanted( item )" );
					}
					else if( !CanEnchant( item ) )
					{
						try
						{
							from.SendMessage( from.Language == "ITA" ? "Questo oggetto non può contenere il potere {0}." : "This item cannot be enchanted with {0} magical power.", Definition.Prefix );
						}
						catch( Exception e )
						{
							Config.SendDebug( e.ToString() );
							Config.SendDebug( "Definition.Prefix  is null for stonetype: {0}", m_StoneType );
						}

						Config.SendDebug( "!StoneEnchantHelper.CanEnchant( m_StoneType, item )" );
					}
					else if( StoneEnchantHelper.IsEnchanted( from, item, true ) )
						Config.SendDebug( "StoneEnchantHelper.IsEnchanted( from, item, true )" );
					else if( item.CustomQuality != Quality.Undefined && item.CustomQuality < Quality.Superior )
					{
						Config.SendDebug( "item.CustomQuality != Quality.Undefined && item.CustomQuality < Superior" );
						from.SendMessage( from.Language == "ITA" ? "Puoi incantare solo oggetti di qualità." : "You can enchant only quality items." );
					}
					else if( ( (int)from.Skills[ Definition.EnchantSkill ].Value ) / 20 < 1 )
					{
						Config.SendDebug( "( (int)from.Skills[ SkillName.{0} ].Value ) / 20 < 1", Definition.EnchantSkill );
						from.SendMessage( from.Language == "ITA" ? "La magia in te non è sufficiente per l'incantamento." : "Magic in you in not enough to enchant this item." );
					}
					else if( !CheckResource( item, from, true, ref info ) )
						Config.SendDebug( "!CheckResource( item, from, true, out info )" );
					else if( info == null )
					{
						Config.SendDebug( "info == null" );
						from.SendMessage( "An error occurred. Contact Dies Irae. Don't delete the stone and the item you tried to enchant." );
					}
					else
					{
						try
						{
							int level = (int)( from.Skills[ Definition.EnchantSkill ].Value / 20 );

							if( level > info.OldMaxMagicalLevel )
								level = info.OldMaxMagicalLevel;

							int malusLevel = info.OldMagicalLevelMalus;

							if( level == 0 )
							{
								from.SendMessage( from.Language == "ITA" ? "Questo oggetto non può essere incantato." : "This item cannot be enchanted with any stone magical power." );
								Config.SendDebug( "level 0" );
								return;
							}

							if( level > MaxStoneLevel )
								level = MaxStoneLevel;

							level = Utility.Random( level );//+ 1; //0-4

							if( malusLevel > 0 && ( ( 100 - malusLevel ) < Utility.Random( 100 ) ) )
								level--;

							if( from.Int > 100 && Utility.Random( 20 ) + 1 < from.Int - 100)
								level++; 

							if( level > MaxStoneLevel )
								level = MaxStoneLevel;

							/*
								POL script
							 
								var int := who.intelligence - who.intelligence_mod;

								if (int < (80 + level * 4) and !who.cmdlevel)
									SendSysMessage(who, "You aren't intelligent enought to enchant this item.");
									return;
								endif
							*/

							if( level < 1 )
								OnFailureEnchant( item, from, Utility.Random( 100 ) < level );
							else if( from.Int < ( 80 + level * 4 ) )
								from.SendMessage( from.Language == "ITA" ? "Non sei abbastanza intelligente per incantare questo oggetto." : "You aren't intelligent enought to enchant this item." );
							else
								BeginEnchant( item, from, level, malusLevel );
						}
						catch( Exception e )
						{
							Config.SendDebug( e.ToString() );
						}
					}
				}
			}
		}

		public bool CanEnchant( Item item )
		{
			StoneDefinition def = StoneDefinition.GetFromIndex( (int)m_StoneType - 1 );
			if( def == null )
				return false;

			Type t = item.GetType();

			foreach( Type supported in def.TypesSupported )
			{
				if( t == supported || t.IsSubclassOf( supported ) )
					return true;
			}

			return false;
		}

		public double GetEnchantDuration( int level )
		{
			return EnchantDurationPerLevel * level;
		}

		public virtual void BeginEnchant( Item item, Mobile from, int level, int malusLevel )
		{
			if( from.CanBeginAction( typeof( EnchantStone ) ) )
			{
				int count = (int)Math.Ceiling( GetEnchantDuration( level ) / AnimateDelay );

				if( count > 0 )
				{
					AnimTimer animTimer = new AnimTimer( from, this, count );
					animTimer.Start();

					double effectiveDuration = ( count * AnimateDelay ) + 1.0;
					from.Freeze( TimeSpan.FromSeconds( effectiveDuration ) );
					Timer.DelayCall( TimeSpan.FromSeconds( effectiveDuration ), new TimerStateCallback( Enchant_Callback ), new object[]
																													   {
																														   from, this, item, level, malusLevel
																													   } );
				}
			}
			else
				from.SendMessage( from.Language == "ITA" ? "{0}, non puoi incantare nulla in questo momento." : "{0}, you cannot start enchanting anything yet.", from.Name );
		}

		private static void Enchant_Callback( object state )
		{
			object[] states = (object[])state;

			Mobile from = (Mobile)states[ 0 ];
			EnchantStone stone = (EnchantStone)states[ 1 ];
			Item item = (Item)states[ 2 ];
			int level = (int)states[ 3 ];
			int malusLevel = (int)states[ 4 ];

			if( malusLevel > 0 )
				from.SendMessage( from.Language == "ITA" ? "Sembrava molto difficile incantare questo oggetto ma..." : "It appears very hard to enchant this item but..." );

			if( !stone.CheckSkills( from, level, malusLevel, (IStoneEnchantItem)item ) )
				stone.OnFailureEnchant( item, from, Utility.Random( 100 ) < level );
			else
			{
				StoneEnchantItem.Imbue( item, stone.StoneType, level );
				stone.OnSuccessfullEnchant( item, from, level );
			}
		}

		public virtual void OnSuccessfullEnchant( Item item, Mobile from, int level )
		{
			from.SendMessage( from.Language == "ITA" ? "Incanti l'oggetto con il potere {0} e livello {1}." : "You enchant this item with {0} power and level {1}.", Definition.Prefix, level );

			DoEffects( from, true );
			from.PublicOverheadMessage( MessageType.Regular, 37, true, from.Language == "ITA" ? "*Successo!!*" : "*Success!!*", true );
			item.InvalidateProperties();
			item.Delta( ItemDelta.Update );

			EndEnchant( item, from );
		}

		public virtual void OnFailureEnchant( Item item, Mobile from, bool isCriticalFailure )
		{
			from.SendMessage( from.Language == "ITA" ? "Hai fallito... e distrutto la gemma." : "You fail... and destroy the gem." );

			DoEffects( from, false );
			from.PublicOverheadMessage( MessageType.Regular, 37, true, from.Language == "ITA" ? "*Fallimento!!*" : "*Failed!!*", true );

			if( isCriticalFailure )
			{
				from.SendMessage( from.Language == "ITA" ? "Disastro! Il processo è fallito catastroficamente distruggendo pietrina e oggetto per sempre!" : "Disaster! The process failed in a catastrophic way. Both item and stone are lost forever!" );
				if( !item.Deleted )
					item.Delete();
			}

			EndEnchant( item, from );
		}

		public virtual void DoEffects( Mobile from, bool success )
		{
			from.FixedParticles( 0x376A, 9, 32, 5007, EffectLayer.Waist );
			from.PlaySound( 0x210 );
		}

		public virtual void EndEnchant( Item item, Mobile from )
		{
			if( !Deleted )
				Delete();

			from.EndAction( typeof( EnchantStone ) );
		}

		public bool CheckResource( Item item, Mobile from, bool message, ref CraftAttributeInfo info )
		{
			CraftResource res;

			if( item is BaseArmor )
				res = ( (BaseArmor)item ).Resource;
			else if( item is BaseWeapon )
				res = ( (BaseWeapon)item ).Resource;
			else if( item is BaseJewel )
				res = ( (BaseJewel)item ).Resource;
			else if( item is BaseClothing )
				res = CraftResource.Iron; // clothing is special...
			else
			{
				if( message )
					from.SendMessage( from.Language == "ITA" ? "Questo oggetto non può essere incantato." : "This item cannot be enchanted with any stone magical power." );
				Config.SendDebug( "No Resource defined." );
				return false;
			}

			CraftResourceInfo resInfo = CraftResources.GetInfo( res );
			if( resInfo == null )
			{
				Config.SendDebug( "resInfo == null" );
				if( message )
					from.SendMessage( from.Language == "ITA" ? "Questo oggetto non può essere incantato." : "This item cannot be enchanted with any stone magical power." );
				return false;
			}

			CraftAttributeInfo attrInfo = resInfo.AttributeInfo;
			if( attrInfo == null )
			{
				Config.SendDebug( "attrInfo == null" );
				if( message )
					from.SendMessage( from.Language == "ITA" ? "Questo oggetto non può essere incantato." : "This item cannot be enchanted with any stone magical power." );
				return false;
			}

			info = attrInfo;
			return true;
		}

		public bool CheckSkills( Mobile from, int level, int malusLevel, IStoneEnchantItem stoneItem )
		{
			SkillName[] skillsReq = stoneItem.SkillsRequiredToEnchant;
			if( skillsReq == null || skillsReq.Length < 3 )
				from.SendMessage( from.Language == "ITA" ? "Non capisci quali abilità servano per incantare questo oggetto." :"You cannot recognize what skills are required to enchant this item." );
			else
			{
				/*
					POL script:

			  		var sk1 := NewGetSkill(who, Cint(skills[2]));
					var sk2 := NewGetSkill(who, Cint(skills[3]));
					var sk3 := NewGetSkill(who, Cint(skills[4]));
					var diff := Cint ((sk1 * 0.25 + sk2 * 0.25 + sk3 * 0.50) / 2.0);

					var malus := (level * 5) - maluslevel;

					if (sk1+sk2+sk3 == 300)
						diff := diff + 10;
					endif

					if ((RandomInt(100) < (diff - malus)))
				 */

				double sk1 = from.Skills[ skillsReq[ 0 ] ].Value; // Itemid
				double sk2 = from.Skills[ skillsReq[ 1 ] ].Value;
				double sk3 = from.Skills[ skillsReq[ 2 ] ].Value;

				int diff = (int)( ( sk1 * 0.25 + sk2 * 0.25 + sk3 * 0.50 ) );// / 2.0 );//0-100
				int malus = ( level * 5 ) + malusLevel;

				if ( malus > 100 )
					malus = 100;

				if( ( sk1 + sk2 + sk3 ) >= 300.0 )
					diff += 10;

				return Utility.Random( 100 ) < ( diff - malus );
			}

			return false;
		}

		private class AnimTimer : Timer
		{
			private readonly Mobile m_From;
			private readonly EnchantStone m_Stone;

			public AnimTimer( Mobile from, EnchantStone stone, int count )
				: base( TimeSpan.Zero, TimeSpan.FromSeconds( AnimateDelay ), count )
			{
				m_From = from;
				m_Stone = stone;

				Priority = TimerPriority.FiftyMS;
			}

			protected override void OnTick()
			{
				if( !m_From.Mounted && m_From.Body.IsHuman )
					m_From.Animate( Utility.RandomList( AnimIds ), 7, 1, true, false, 0 );

				m_Stone.DoEffects( m_From, true );
				m_From.PlaySound( 0x208 );
			}
		}

		#region serial-deserial
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 );

			writer.Write( m_Identified );
			writer.Write( (int)m_StoneType );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_Identified = reader.ReadBool();
			m_StoneType = (StoneTypes)reader.ReadInt();
		}
		#endregion
	}
}