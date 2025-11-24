/***************************************************************************
 *                                  PersonalRecipeScroll.cs
 *                            		-----------------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Midgard.Engines.AdvancedSmelting;
using Server;
using Server.Engines.Craft;
using Server.Items;

namespace Midgard.Items
{
    public class PersonalRecipeScroll : RecipeScroll
    {
        private Mobile m_Owner;

        [CommandProperty( AccessLevel.Administrator )]
        public Mobile Owner
        {
            get { return m_Owner; }
            set { m_Owner = value; InvalidateSecondAgeName(); }
        }

        public virtual void InvalidateSecondAgeName()
        {
        }

        public PersonalRecipeScroll( int recipe, Mobile owner )
            : base( recipe )
        {
            m_Owner = owner;
        }

        public PersonalRecipeScroll( Recipe r )
            : base( r )
        {
            m_Owner = null;
        }

        public PersonalRecipeScroll( int ID )
            : base( ID )
        {
            m_Owner = null;
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( m_Owner != null && m_Owner != from )
            {
                from.SendMessage( "You cannot use that recipe." );
                return;
            }

            base.OnDoubleClick( from );
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            if( m_Owner != null )
                list.Add( "Owner: {0}", m_Owner.Name );
        }

        public override void OnSingleClick( Mobile from )
        {
            Recipe r = Recipe;

            if( r != null && !( this is SmeltingRecipeScroll ) ) // mining recipes do not have a CraftItem nor TextDefinition
                LabelTo( from, 1049644, r.TextDefinition.ToString() ); // [~1_stuff~]

            if( m_Owner != null )
                LabelTo( from, string.Format( "Owner: {0}", m_Owner.Name ) );

            base.OnSingleClick( from );
        }

        public static Item RandomRecipe( Type enumType, Mobile from )
        {
            List<int> list = new List<int>();
            foreach( int i in Enum.GetValues( enumType ) )
                list.Add( i );

            return new PersonalRecipeScroll( list[ Utility.Random( list.Count ) ], from );
        }

        #region serialization
        public PersonalRecipeScroll( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( m_Owner );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        m_Owner = reader.ReadMobile();

                        break;
                    }
            }
        }
        #endregion
    }
}