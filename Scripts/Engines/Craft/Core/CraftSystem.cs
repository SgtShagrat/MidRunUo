using System;
using System.Collections.Generic;

using Midgard.Engines.JailSystem;
using Midgard.Engines.OldCraftSystem;
using Server.Items;
using Midgard.Engines.MidgardTownSystem;

using Server.Mobiles;

namespace Server.Engines.Craft
{
	public enum CraftECA
	{
		ChanceMinusSixty,
		FiftyPercentChanceMinusTenPercent,
		ChanceMinusSixtyToFourtyFive,
		ZeroToFourPercentPlusBonus,
		ZeroToTenPercentPlusBonus,
	}

	public abstract class CraftSystem
	{
		private int m_MinCraftEffect;
		private int m_MaxCraftEffect;
		private double m_Delay;
		private bool m_Resmelt;
		private bool m_Repair;
		private bool m_MarkOption;
		private bool m_CanEnhance;

		private CraftItemCol m_CraftItems;
		private CraftGroupCol m_CraftGroups;
		private CraftSubResCol m_CraftSubRes;
		private CraftSubResCol m_CraftSubRes2;

		public int MinCraftEffect { get { return m_MinCraftEffect; } }
		public int MaxCraftEffect { get { return m_MaxCraftEffect; } }
		public double Delay { get { return m_Delay; } }

		public CraftItemCol CraftItems { get { return m_CraftItems; } }
		public CraftGroupCol CraftGroups { get { return m_CraftGroups; } }
		public CraftSubResCol CraftSubRes { get { return m_CraftSubRes; } }
		public CraftSubResCol CraftSubRes2 { get { return m_CraftSubRes2; } }

		public abstract SkillName MainSkill { get; }

		#region mod by Dies Irae
		public void GeneratePriceAnalysis()
		{
			try
			{
				foreach( CraftGroup group in CraftGroups )
				{
					foreach( CraftItem craftItem in group.CraftItems )
					{
						craftItem.PriceAnalysis( "Logs/" + Name + "-prices.log" );
					}
				}
			}
			catch( Exception e )
			{
				Console.WriteLine( e );
			}
		}

		public List<Type> GetResourcesList()
		{
			List<Type> list = new List<Type>();

			foreach( CraftGroup group in CraftGroups )
			{
				foreach( CraftItem craftItem in group.CraftItems )
				{
					for( int i = 0; i < craftItem.Resources.Count; i++ )
					{
						CraftRes craftResource = craftItem.Resources.GetAt( i );
						if( !list.Contains( craftResource.ItemType ) )
							list.Add( craftResource.ItemType );
					}
				}
			}

			return list;
		}

		public virtual CraftDefinitionTree DefinitionTree{ get { return null; } }

		public virtual bool SupportOldMenu { get { return false; } }

		public virtual int MakeLastID { get { return 0xB95; } } // library

		public virtual CraftSubRes GetResourceFromType( Type t, bool usePrimary, bool useSecondary )
		{
			CraftSubRes res = null;

			if( usePrimary && CraftSubRes != null )
			{
				res = CraftSubRes.SearchFor( t );
				if( res != null )
				return res;
			}

			if( useSecondary && CraftSubRes2 != null )
			{
				res = CraftSubRes2.SearchFor( t );
				if( res != null )
				return res;
			}

			return null;
		}

		public virtual bool CheckResourceSkill( Mobile from, CraftSubRes subResource )
		{
			if( subResource == null )
				return false;

			if( from.Skills[ MainSkill ].Base < subResource.RequiredSkill )
			{
				if( subResource.Message is int )
					from.SendLocalizedMessage( (int)( subResource.Message ) );
				else if( subResource.Message is string )
					from.SendMessage( (string)( subResource.Message ) );

				return false;
			}

			return true;
		}

		/// <summary>
		/// Skill that gives some bonus to craft chance and to exc craft chance
		/// </summary>
		public virtual SkillName BonusSkill { get { return MainSkill; } }

		/// <summary>
		/// Difficulty in value/100 for a given material
		/// </summary>
		public virtual double GetMaterialDifficulty( Mobile from, CraftItem item )
		{
			CraftContext context = GetContext( from );

			if( context != null )
			{
				CraftSubResCol res = ( item.UseSubRes2 ? CraftSubRes2 : CraftSubRes );
				int resIndex = ( item.UseSubRes2 ? context.LastResourceIndex2 : context.LastResourceIndex );

				if( resIndex >= 0 && resIndex < res.Count )
				{
					if( from.AccessLevel == AccessLevel.Developer )
						from.SendMessage( "Your material difficulty is now {0}", res.GetAt( resIndex ).MaterialDifficulty.ToString( "F3" ) );

					return res.GetAt( resIndex ).MaterialDifficulty;
				}
			}

			return 1.0;
		}

		public virtual CraftSubRes GetResourceFromCraftResource( CraftResource resource, bool usePrimary, bool useSecondary )
		{
			CraftSubRes res = null;

			if( usePrimary && CraftSubRes != null )
			{
				res = CraftSubRes.SearchFor( resource );
				if( res != null )
					return res;
			}

			if( useSecondary && CraftSubRes2 != null )
			{
				res = CraftSubRes2.SearchFor( resource );
				if( res != null )
					return res;
			}

			return null;
		}

		public double GetMaterialDifficulty( CraftResource craftResource )
		{
			CraftSubRes res = GetResourceFromCraftResource( craftResource, CraftSubRes != null, CraftSubRes2 != null );
			if( res != null )
			{
                if( Core.Debug )
				    Console.WriteLine( "Debug: material difficulty for {0} is {1:F2}", craftResource, res.MaterialDifficulty );
				return res.MaterialDifficulty;
			}

			return 1.0;
		}

		/// <summary>
		/// Exceptional chance at 100.0 main skill
		/// </summary>
		public virtual double GetExcChanceAtMaxSkill( Mobile from, CraftItem item )
		{
			return 0.05;
		}

		/// <summary>
		/// Maximum exceptional chance
		/// </summary>
		public virtual double GetMaxExcChance( Mobile from, CraftItem item )
		{
			return 0.10;
		}

		/// <summary>
		/// Exceptional bonus chance for bonus skill
		/// </summary>
		public virtual double GetExcBonusAtCraft( Mobile from, CraftItem item )
		{
			double bonus = 0.0;

			SkillName bonusSkill = BonusSkill;
			if( item.ItemType.IsSubclassOf( typeof(BaseWeapon) ) || item.ItemType.IsAssignableFrom( typeof(BaseArmor) ) )
				bonusSkill = SkillName.ArmsLore;

			if( bonusSkill != MainSkill )
				bonus = from.Skills[ BonusSkill ].Value / 10000.0;

			if( from.PlayerDebug )
			{
				from.SendMessage( "GetExcBonusAtCraft: bonus {0}", bonus.ToString( "F3" ) );
			}

			return bonus;
		}

		public virtual void SetCustomQuality( Mobile from, CraftItem craftItem, Item item, bool exceptional, Type typeRes, double bonus )
		{
			if( exceptional )
			{
				item.CustomQuality = Quality.Exceptional;
				return;
			}

			SkillName bonusSkill = BonusSkill;
			if( item is BaseWeapon || item is BaseArmor )
				bonusSkill = SkillName.ArmsLore;

			// mean from 0 to 100.0
			int mean = (int)( ( from.Skills[ MainSkill ].Base + from.Skills[ bonusSkill ].Base ) / 2.0 );

			// chance from 0 to 100
			double chance = Utility.Random( mean );

			double quality = 0.4 + bonus + ( chance / 100.0 ) + ( mean / 200.0 );

			if( quality > 1.25 )
				quality = 1.25;

			if( from.AccessLevel == AccessLevel.Developer )
			{
				from.SendMessage( "SetCustomQuality: mean {0}, chance {1}, quality {2}", mean, chance.ToString( "F3" ), quality.ToString( "F3" ) );
			}

			item.CustomQuality = quality;
		}

		public virtual bool SupportsAutoLoop( Type type )
		{
			return true;
		}

	    public virtual bool ForceConsumeOnFailure
	    {
	        get { return false; }
	    }

	    public virtual ConsumeType ConsumeTypeOnFailure
	    {
	        get { return ConsumeType.Half; }
	    }

		public static bool IsCrafting( Mobile m )
		{
			return !m.CanBeginAction( typeof( CraftSystem ) );
		}

		public virtual void AutoMacroCheck( Mobile toCheck )
		{
			if( toCheck is Midgard2PlayerMobile )
				( (Midgard2PlayerMobile)toCheck ).AutoMacroCheck();
		}
		#endregion

		public virtual int GumpTitleNumber { get { return 0; } }
		public virtual string GumpTitleString { get { return ""; } }

		public virtual CraftECA ECA { get { return CraftECA.ChanceMinusSixty; } }

		private Dictionary<Mobile, CraftContext> m_ContextTable = new Dictionary<Mobile, CraftContext>();

	    public abstract double GetChanceAtMin( CraftItem item );

		public virtual bool RetainsColorFrom( CraftItem item, Type type )
		{
			return false;
		}

		public CraftContext GetContext( Mobile m )
		{
			if( m == null )
				return null;

			if( m.Deleted )
			{
				m_ContextTable.Remove( m );
				return null;
			}

			CraftContext c = null;
			m_ContextTable.TryGetValue( m, out c );

			if( c == null )
				m_ContextTable[ m ] = c = new CraftContext();

			return c;
		}

		#region mod by Dies Irae
		public void ClearContext( Mobile m )
		{
			if( m_ContextTable != null && m_ContextTable.ContainsKey( m ) )
				m_ContextTable.Remove( m );

			CraftContext c = null;
		    if( m_ContextTable != null )
		        m_ContextTable.TryGetValue( m, out c );

		    if( m.PlayerDebug )
				m.SendMessage( "Craft context is {0}null", c == null ? "" : "NOT " );
		}

		public CraftContext ResetContext( Mobile m )
		{
			if( m.PlayerDebug )
				m.SendMessage( "Resetting craft context." );

			ClearContext( m );
			return GetContext( m );
		}
		#endregion

		public void OnMade( Mobile m, CraftItem item )
		{
			CraftContext c = GetContext( m );

			if( c != null )
				c.OnMade( item );
		}

		public bool Resmelt
		{
			get { return m_Resmelt; }
			set { m_Resmelt = value; }
		}

		public bool Repair
		{
			get { return m_Repair; }
			set { m_Repair = value; }
		}

		public bool MarkOption
		{
			get { return m_MarkOption; }
			set { m_MarkOption = value; }
		}

		public bool CanEnhance
		{
			get { return m_CanEnhance; }
			set { m_CanEnhance = value; }
		}

		public CraftSystem( int minCraftEffect, int maxCraftEffect, double delay )
		{
			m_MinCraftEffect = minCraftEffect;
			m_MaxCraftEffect = maxCraftEffect;
			m_Delay = delay;

			m_CraftItems = new CraftItemCol();
			m_CraftGroups = new CraftGroupCol();
			m_CraftSubRes = new CraftSubResCol();
			m_CraftSubRes2 = new CraftSubResCol();

			InitCraftList();
		}

		public virtual bool ConsumeOnFailure( Mobile from, Type resourceType, CraftItem craftItem )
		{
			return true;
		}

		public void CreateItem( Mobile from, Type type, Type typeRes, BaseTool tool, CraftItem realCraftItem )
		{
			// Verify if the type is in the list of the craftable item
			CraftItem craftItem = m_CraftItems.SearchFor( type );
			if( craftItem != null )
			{
				// The item is in the list, try to create it
				// Test code: items like sextant parts can be crafted either directly from ingots, or from different parts
				realCraftItem.Craft( from, this, typeRes, tool );
				//craftItem.Craft( from, this, typeRes, tool );
			}
		}


		public int AddCraft( Type typeItem, TextDefinition group, TextDefinition name, double minSkill, double maxSkill, Type typeRes, TextDefinition nameRes, int amount )
		{
			return AddCraft( typeItem, group, name, MainSkill, minSkill, maxSkill, typeRes, nameRes, amount, "" );
		}

		public int AddCraft( Type typeItem, TextDefinition group, TextDefinition name, double minSkill, double maxSkill, Type typeRes, TextDefinition nameRes, int amount, TextDefinition message )
		{
			return AddCraft( typeItem, group, name, MainSkill, minSkill, maxSkill, typeRes, nameRes, amount, message );
		}

		public int AddCraft( Type typeItem, TextDefinition group, TextDefinition name, SkillName skillToMake, double minSkill, double maxSkill, Type typeRes, TextDefinition nameRes, int amount )
		{
			return AddCraft( typeItem, group, name, skillToMake, minSkill, maxSkill, typeRes, nameRes, amount, "" );
		}

		public int AddCraft( Type typeItem, TextDefinition group, TextDefinition name, SkillName skillToMake, double minSkill, double maxSkill, Type typeRes, TextDefinition nameRes, int amount, TextDefinition message )
		{
			CraftItem craftItem = new CraftItem( typeItem, group, name );
			craftItem.AddRes( typeRes, nameRes, amount, message );
			craftItem.AddSkill( skillToMake, minSkill, maxSkill );

			DoGroup( group, craftItem );
			return m_CraftItems.Add( craftItem );
		}


		private void DoGroup( TextDefinition groupName, CraftItem craftItem )
		{
			int index = m_CraftGroups.SearchFor( groupName );

			if( index == -1 )
			{
				CraftGroup craftGroup = new CraftGroup( groupName );
				craftGroup.AddCraftItem( craftItem );
				m_CraftGroups.Add( craftGroup );
			}
			else
			{
				m_CraftGroups.GetAt( index ).AddCraftItem( craftItem );
			}
		}


		public void SetManaReq( int index, int mana )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.Mana = mana;
		}

		public void SetStamReq( int index, int stam )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.Stam = stam;
		}

		public void SetHitsReq( int index, int hits )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.Hits = hits;
		}

		public void SetUseAllRes( int index, bool useAll )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.UseAllRes = useAll;
		}

		public void SetNeedHeat( int index, bool needHeat )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.NeedHeat = needHeat;
		}

		public void SetNeedOven( int index, bool needOven )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.NeedOven = needOven;
		}

		public void SetNeedMill( int index, bool needMill )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.NeedMill = needMill;
		}

		public void SetNeededExpansion( int index, Expansion expansion )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.RequiredExpansion = expansion;
		}

		#region mod by Dies Irae
		public void SetNeededTownSystem( int index, TownSystem system )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.RequiredTownSystem = system;
		}

		public void SetNeededRace( int index, Race race )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.RequiredRace = race;
		}

		public void SetNeededAlchemyTable( int index, bool needTable )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.NeedAlchemyTable = needTable;
		}

		public void SetNeededCarpenteryTable( int index, bool needTable )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.NeedCarpenteryTable = needTable;
		}
		#endregion

		public void AddRes( int index, Type type, TextDefinition name, int amount )
		{
			AddRes( index, type, name, amount, "" );
		}

		public void AddRes( int index, Type type, TextDefinition name, int amount, TextDefinition message )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.AddRes( type, name, amount, message );
		}

		public void AddSkill( int index, SkillName skillToMake, double minSkill, double maxSkill )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.AddSkill( skillToMake, minSkill, maxSkill );
		}

		public void SetUseSubRes2( int index, bool val )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.UseSubRes2 = val;
		}

		public void AddRecipe( int index, int id )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.AddRecipe( id, this );
		}

		public void ForceNonExceptional( int index )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.ForceNonExceptional = true;
		}


		public void SetSubRes( Type type, string name )
		{
			m_CraftSubRes.ResType = type;
			m_CraftSubRes.NameString = name;
			m_CraftSubRes.Init = true;
		}

		public void SetSubRes( Type type, int name )
		{
			m_CraftSubRes.ResType = type;
			m_CraftSubRes.NameNumber = name;
			m_CraftSubRes.Init = true;
		}

		public void AddSubRes( Type type, int name, double reqSkill, object message )
		{
			CraftSubRes craftSubRes = new CraftSubRes( type, name, reqSkill, message );
			m_CraftSubRes.Add( craftSubRes );
		}

		public void AddSubRes( Type type, int name, double reqSkill, int genericName, object message )
		{
			CraftSubRes craftSubRes = new CraftSubRes( type, name, reqSkill, genericName, message, 1.0 );
			m_CraftSubRes.Add( craftSubRes );
		}

		public void AddSubRes( Type type, string name, double reqSkill, object message )
		{
			CraftSubRes craftSubRes = new CraftSubRes( type, name, reqSkill, message );
			m_CraftSubRes.Add( craftSubRes );
		}

		#region mod by Dies Irae
		public void AddSubRes( Type type, int name, double reqSkill, int genericName, object message, double materialDifficulty )
		{
			CraftSubRes craftSubRes = new CraftSubRes( type, name, reqSkill, genericName, message, materialDifficulty );
			m_CraftSubRes.Add( craftSubRes );
		}

		public void AddSubRes( Type type, string name, double reqSkill, int genericName, object message, double materialDifficulty )
		{
			CraftSubRes craftSubRes = new CraftSubRes( type, name, reqSkill, genericName, message, materialDifficulty );
			m_CraftSubRes.Add( craftSubRes );
		}

		public void AddSubRes( Type type, string name, double reqSkill, int genericName, object message )
		{
			CraftSubRes craftSubRes = new CraftSubRes( type, name, reqSkill, genericName, message, 1.0 );
			m_CraftSubRes.Add( craftSubRes );
		}

		public void AddDescription( int index, string description )
		{
			CraftItem craftItem = m_CraftItems.GetAt( index );
			craftItem.AddDescription( description, this );
		}
		#endregion

		public void SetSubRes2( Type type, string name )
		{
			m_CraftSubRes2.ResType = type;
			m_CraftSubRes2.NameString = name;
			m_CraftSubRes2.Init = true;
		}

		public void SetSubRes2( Type type, int name )
		{
			m_CraftSubRes2.ResType = type;
			m_CraftSubRes2.NameNumber = name;
			m_CraftSubRes2.Init = true;
		}

		public void AddSubRes2( Type type, int name, double reqSkill, object message )
		{
			CraftSubRes craftSubRes = new CraftSubRes( type, name, reqSkill, message );
			m_CraftSubRes2.Add( craftSubRes );
		}

		public void AddSubRes2( Type type, int name, double reqSkill, int genericName, object message )
		{
			CraftSubRes craftSubRes = new CraftSubRes( type, name, reqSkill, genericName, message, 1.0 ); // mod by Dies Irae
			m_CraftSubRes2.Add( craftSubRes );
		}

		public void AddSubRes2( Type type, string name, double reqSkill, object message )
		{
			CraftSubRes craftSubRes = new CraftSubRes( type, name, reqSkill, message );
			m_CraftSubRes2.Add( craftSubRes );
		}

		#region mod by Dies Irae
		public void AddSubRes2( Type type, string name, double reqSkill, object message, double materialDifficulty )
		{
			CraftSubRes craftSubRes = new CraftSubRes( type, name, reqSkill, 0, message, materialDifficulty );
			m_CraftSubRes2.Add( craftSubRes );
		}

		public void AddSubRes2( Type type, string name, double reqSkill, int genericName, object message, double materialDifficulty )
		{
			CraftSubRes craftSubRes = new CraftSubRes( type, name, reqSkill, genericName, message, materialDifficulty );
			m_CraftSubRes2.Add( craftSubRes );
		}

		public void AddSubRes2( Type type, int name, double reqSkill, int genericName, object message, double materialDifficulty )
		{
			CraftSubRes craftSubRes = new CraftSubRes( type, name, reqSkill, genericName, message, materialDifficulty );
			m_CraftSubRes2.Add( craftSubRes );
		}
		#endregion

		public abstract void InitCraftList();

		#region mod by Dies Irae
		// public abstract void PlayCraftEffect( Mobile from );
		public virtual void PlayCraftEffect( Mobile from )
		{
		}

		public virtual void PlayCraftEffect( Mobile from, CraftItem craftItem, Type typeRes, BaseTool tool )
		{
			PlayCraftEffect( from );
		}

		public virtual string Name { get{ return ""; } }

        /// <summary>
        /// true if this system support custom quality engine
        /// </summary>
        public virtual bool SupportsQuality
        {
            get { return false; }
        }

        /// <summary>
        /// minimum skill to repair an item of this system
        /// </summary>
        public virtual double MinSkillToRepair
        {
            get { return 70.0; }
        }
        #endregion

		public abstract int PlayEndingEffect( Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item );

		public abstract int CanCraft( Mobile from, BaseTool tool, Type itemType );
	}
}