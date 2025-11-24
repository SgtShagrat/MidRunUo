/***************************************************************************
 *                               SmeltingRecipeScroll.cs
 *                            -----------------------------
 *   begin                : 28 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Midgard.Items;
using Server;
using Server.Engines.Craft;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.AdvancedSmelting
{
    public class SmeltingRecipeScroll : PersonalRecipeScroll
    {
        public static void Initialize()
        {
            foreach( int i in Enum.GetValues( typeof( SmeltingRecipes ) ) )
            {
                new Recipe( i, null, null );
            }
        }

        public static Item RandomMiningRecipe()
        {
            List<int> list = new List<int>();
            foreach( int i in Enum.GetValues( typeof( SmeltingRecipes ) ) )
                list.Add( i );

            return new SmeltingRecipeScroll( list[ Utility.Random( list.Count ) ] );
        }

        public static string GetSmeltingName( int index )
        {
            if( !Enum.IsDefined( typeof( SmeltingRecipes ), index ) )
                return String.Empty;

            switch( (SmeltingRecipes)index )
            {
                case SmeltingRecipes.OldGraphite:
                    return "graphite";
                case SmeltingRecipes.OldPyrite:
                    return "pyrite";
                case SmeltingRecipes.OldAzurite:
                    return "azurite";
                case SmeltingRecipes.OldVanadium:
                    return "vanadium";
                case SmeltingRecipes.OldSilver:
                    return "silver";
                case SmeltingRecipes.OldPlatinum:
                    return "platinum";
                case SmeltingRecipes.OldAmethyst:
                    return "amethyst";
                case SmeltingRecipes.OldTitanium:
                    return "titanium";
                case SmeltingRecipes.OldXenian:
                    return "xenian";
                case SmeltingRecipes.OldRubidian:
                    return "rubidian";
                case SmeltingRecipes.OldObsidian:
                    return "obsidian";
                case SmeltingRecipes.OldEbonSapphire:
                    return "ebon twilight sapphire";
                case SmeltingRecipes.OldDarkRuby:
                    return "dark sable ruby";
                case SmeltingRecipes.OldRadiantDiamond:
                    return "radiant nimbus diamond";

                case SmeltingRecipes.OldVerite:
                    return "verite";
                case SmeltingRecipes.OldValorite:
                    return "valorite";
                default:
                    return String.Empty;
            }
        }

        [Constructable]
        public SmeltingRecipeScroll()
            : this( (int)SmeltingRecipes.OldGraphite, null )
        {
        }

        [Constructable]
        public SmeltingRecipeScroll( int recipe, Mobile owner )
            : base( recipe, owner )
        {
        }

        [Constructable]
        public SmeltingRecipeScroll( int recipe )
            : base( recipe )
        {
        }

        private string m_SecondAgeFullName;

        public override void InvalidateSecondAgeName()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append( "smelting recipe scroll" );

            Recipe r = Recipe;
            if( r != null )
                sb.AppendFormat( " for {0}", GetSmeltingName( r.ID ) );

            if( Owner != null && !String.IsNullOrEmpty( Owner.Name ) )
                sb.AppendFormat( ", owned by {0}", Owner.Name );

            m_SecondAgeFullName = sb.ToString();
        }

        public override void OnSingleClick( Mobile from )
        {
            if( m_SecondAgeFullName == null )
                InvalidateSecondAgeName();

            LabelTo( from, m_SecondAgeFullName );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !from.InRange( GetWorldLocation(), 2 ) )
            {
                from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
                return;
            }

            Recipe r = Recipe;

            if( r != null && from is PlayerMobile )
            {
                PlayerMobile pm = from as PlayerMobile;

                if( !pm.HasRecipe( r ) )
                {
                    pm.SendMessage( "You have learned how to produce: {0}", GetSmeltingName( r.ID ) );
                    pm.AcquireRecipe( r );
                    Delete();
                }
                else
                {
                    pm.SendLocalizedMessage( 1073427 ); // You already know this recipe.
                }

            }
        }

        #region serialization
        public SmeltingRecipeScroll( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}