#define DebugRecipeSystem

using System;
using Server;
using Server.Engines.Craft;
using Server.Mobiles;
using Server.Network;
using System.Collections.Generic;

namespace Server.Items
{
	public class RecipeScroll : Item, IIdentificable
	{
		public override int LabelNumber { get { return 1074560; } } // recipe scroll

		private int m_RecipeID;

		[CommandProperty( AccessLevel.GameMaster )]
		public int RecipeID
		{
			get { return m_RecipeID; }
			set { m_RecipeID = value; InvalidateProperties(); }
		}

		public Recipe Recipe
		{
			get
			{
				if( Recipe.Recipes.ContainsKey( m_RecipeID ) )
				{
#if DebugRecipeSystem
					Console.WriteLine( "Recipe.Recipes contain id {0}", m_RecipeID );
#endif
					return Recipe.Recipes[m_RecipeID];
				}
#if DebugRecipeSystem
				else
					Console.WriteLine( "Warning: Recipe.Recipes does NOT contain id {0}", m_RecipeID );
#endif
				return null;
			}
		}

        #region mod by Dies Irae : pre-Aos stuff
        #region IIdentificable members
        private List<Mobile> m_IdentifiersList;

        private const int MaxIdentifiers = 50;

        public void CopyIdentifiersTo( IIdentificable identificable )
        {
            if( m_IdentifiersList != null && m_IdentifiersList.Count > 0 )
            {
                foreach( Mobile mobile in m_IdentifiersList )
                {
                    identificable.AddIdentifier( mobile );
                }
            }
        }

        public void ClearIdentifiers()
        {
            if( m_IdentifiersList != null && m_IdentifiersList.Count > 0 )
                m_IdentifiersList.Clear();
        }

        public void AddIdentifier( Mobile from )
        {
            if( m_IdentifiersList == null )
                m_IdentifiersList = new List<Mobile>();

            if( !m_IdentifiersList.Contains( from ) )
                m_IdentifiersList.Add( from );

            if( m_IdentifiersList.Count > MaxIdentifiers )
                m_IdentifiersList.RemoveAt( 0 );
        }

        public bool IsIdentifiedFor( Mobile from )
        {
            if( m_IdentifiersList == null )
                return false;

            return m_IdentifiersList.Contains( from );
        }

        public void DisplayItemInfo( Mobile from )
        {
        }

        public void InvalidateSecondAgeNames()
        {
        }
        #endregion
        #endregion

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			Recipe r = this.Recipe;

#if DebugRecipeSystem
			if( r == null )
				Console.WriteLine( "Warning: null recipe on recipe scroll in GetProperties." );
#endif
			
			if( r != null )
				list.Add( 1049644, r.TextDefinition.ToString() ); // [~1_stuff~]
		}

		public RecipeScroll( Recipe r )
			: this( r.ID )
		{
		}

		[Constructable]
		public RecipeScroll( int recipeID )
			: base( 0x2831 )
		{
			m_RecipeID = recipeID;
		}

		public RecipeScroll( Serial serial )
			: base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
#if DebugRecipeSystem
			if( Recipe.Recipes == null )
				Console.WriteLine( "Warning: Recipe.Recipes is null!" );
			else
			{
				Console.WriteLine( "Maximum id of recipes {0}", Recipe.LargestRecipeID );
                try
                {
                    foreach( Recipe re in Recipe.Recipes.Values )
                    {
                        Console.WriteLine( "\t{0} - {1} - {2} - {3}", re.CraftSystem.GetType().Name, re.ID, re.CraftItem.ItemType.Name, re.TextDefinition.String );
                    }
                }
                catch( Exception e )
                {
                    Console.WriteLine( e );
                }
			}
#endif
			if( !from.InRange( this.GetWorldLocation(), 2 ) )
			{
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
				return;
			}

			Recipe r = this.Recipe;

#if DebugRecipeSystem
			if( r == null )
			{
				Console.WriteLine( "Warning: null recipe on recipe scroll in OnDoubleClick." );
				return;
			}
#endif		
			if( r != null && from is PlayerMobile )
			{
				PlayerMobile pm = from as PlayerMobile;

				if( !pm.HasRecipe( r ) )
				{
					bool allRequiredSkills = true;
					double chance = r.CraftItem.GetSuccessChance( from, null, r.CraftSystem, false, ref allRequiredSkills );

					if ( allRequiredSkills && chance >= 0.0 )
					{
						pm.SendLocalizedMessage( 1073451, r.TextDefinition.ToString() ); // You have learned a new recipe: ~1_RECIPE~
						pm.AcquireRecipe( r );
						this.Delete();
					}
					else
					{
						pm.SendLocalizedMessage( 1044153 ); // You don't have the required skills to attempt this item.
					}
				}
				else
				{
					pm.SendLocalizedMessage( 1073427 ); // You already know this recipe.
				}
				
			}
		}

        #region mod by Dies Irae : pre-aos stuff
        public override void OnSingleClick( Mobile from )
        {
            if( !IsIdentifiedFor( from ) )
                LabelTo( from, "a magic item" );
            else
                base.OnSingleClick( from );
        }
        #endregion

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)2 ); // version
			writer.Write( (int)m_RecipeID );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch( version )
			{
                case 2:
                    goto case 1;
                case 1:
                    if( version < 2 )
                        reader.ReadBool();
                    goto case 0;
				case 0:
					{
						m_RecipeID = reader.ReadInt();

						break;
					}
			}
		}
	}
}
