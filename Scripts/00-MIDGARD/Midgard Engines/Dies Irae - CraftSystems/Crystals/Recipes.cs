/***************************************************************************
 *                                  Recipes.cs
 *                            		----------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Midgard.Items;
using Server;
using Server.Engines.Craft;
using Server.Items;

namespace Midgard.Engines.Craft
{
    public class CrystalBrazierRecipe : PersonalRecipeScroll
    {
        public CrystalBrazierRecipe()
            : base( (int)DefCrystalCrafting.CrystalCrafterRecipes.CrystalBrazier, null )
        {
            Name = "Crystal Brazier Recipe";
        }

        public CrystalBrazierRecipe( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class CrystalThroneEastRecipe : PersonalRecipeScroll
    {
        public CrystalThroneEastRecipe()
            : base( (int)DefCrystalCrafting.CrystalCrafterRecipes.CrystalThroneEast, null )
        {
            Name = "Crystal Throne East Recipe";
        }

        public CrystalThroneEastRecipe( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class CrystalThroneSouthRecipe : PersonalRecipeScroll
    {
        public CrystalThroneSouthRecipe()
            : base( (int)DefCrystalCrafting.CrystalCrafterRecipes.CrystalThroneSouth, null )
        {
            Name = "Crystal Throne South Recipe";
        }

        public CrystalThroneSouthRecipe( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class CrystalTableEastAddonDeedRecipe : PersonalRecipeScroll
    {
        public CrystalTableEastAddonDeedRecipe()
            : base( (int)DefCrystalCrafting.CrystalCrafterRecipes.CrystalTableEastAddonDeed, null )
        {
            Name = "Crystal Table East Recipe";
        }

        public CrystalTableEastAddonDeedRecipe( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class CrystalTableSouthAddonDeedRecipe : PersonalRecipeScroll
    {
        public CrystalTableSouthAddonDeedRecipe()
            : base( (int)DefCrystalCrafting.CrystalCrafterRecipes.CrystalTableSouthAddonDeed, null )
        {
            Name = "Crystal Table South Recipe";
        }

        public CrystalTableSouthAddonDeedRecipe( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class CrystalAltarAddonDeedRecipe : PersonalRecipeScroll
    {
        public CrystalAltarAddonDeedRecipe()
            : base( (int)DefCrystalCrafting.CrystalCrafterRecipes.CrystalAltarAddonDeed, null )
        {
            Name = "Crystal Altar Recipe";
        }

        public CrystalAltarAddonDeedRecipe( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class CrystalPedestralSmallRecipe : PersonalRecipeScroll
    {
        public CrystalPedestralSmallRecipe()
            : base( (int)DefCrystalCrafting.CrystalCrafterRecipes.CrystalPedestralSmall, null )
        {
            Name = "Crystal Pedestral Small Recipe";
        }

        public CrystalPedestralSmallRecipe( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class CrystalPedestralMediumRecipe : PersonalRecipeScroll
    {
        public CrystalPedestralMediumRecipe()
            : base( (int)DefCrystalCrafting.CrystalCrafterRecipes.CrystalPedestralMedium, null )
        {
            Name = "Crystal Pedestral Medium Recipe";
        }

        public CrystalPedestralMediumRecipe( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class CrystalPedestralLargeEmptyRecipe : PersonalRecipeScroll
    {
        public CrystalPedestralLargeEmptyRecipe()
            : base( (int)DefCrystalCrafting.CrystalCrafterRecipes.CrystalPedestralLargeEmpty, null )
        {
            Name = "Crystal Pedestral Large Empty Recipe";
        }

        public CrystalPedestralLargeEmptyRecipe( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class CrystalPedestralLargeRecipe : PersonalRecipeScroll
    {
        public CrystalPedestralLargeRecipe()
            : base( (int)DefCrystalCrafting.CrystalCrafterRecipes.CrystalPedestralLarge, null )
        {
            Name = "Crystal Pedestral Large Recipe";
        }

        public CrystalPedestralLargeRecipe( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class CrystalBullSouthAddonDeedRecipe : PersonalRecipeScroll
    {
        public CrystalBullSouthAddonDeedRecipe()
            : base( (int)DefCrystalCrafting.CrystalCrafterRecipes.CrystalBullSouthAddonDeed, null )
        {
            Name = "Crystal Bull South Deed Recipe";
        }

        public CrystalBullSouthAddonDeedRecipe( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class CrystalBullEastAddonDeedRecipe : PersonalRecipeScroll
    {
        public CrystalBullEastAddonDeedRecipe()
            : base( (int)DefCrystalCrafting.CrystalCrafterRecipes.CrystalBullEastAddonDeed, null )
        {
            Name = "Crystal Bull East Deed Recipe";
        }

        public CrystalBullEastAddonDeedRecipe( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class CrystalBeggarStatueSouthRecipe : PersonalRecipeScroll
    {
        public CrystalBeggarStatueSouthRecipe()
            : base( (int)DefCrystalCrafting.CrystalCrafterRecipes.CrystalBeggarStatueSouth, null )
        {
            Name = "Crystal Beggar Statue South Recipe";
        }

        public CrystalBeggarStatueSouthRecipe( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class CrystalBeggarStatueEastRecipe : PersonalRecipeScroll
    {
        public CrystalBeggarStatueEastRecipe()
            : base( (int)DefCrystalCrafting.CrystalCrafterRecipes.CrystalBeggarStatueEast, null )
        {
            Name = "Crystal Beggar Statue East Recipe";
        }

        public CrystalBeggarStatueEastRecipe( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class CrystalSupplicantStatueSouthRecipe : PersonalRecipeScroll
    {
        public CrystalSupplicantStatueSouthRecipe()
            : base( (int)DefCrystalCrafting.CrystalCrafterRecipes.CrystalSupplicantStatueSouth, null )
        {
            Name = "Crystal Supplicant Statue South Recipe";
        }

        public CrystalSupplicantStatueSouthRecipe( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class CrystalSupplicantStatueEastRecipe : PersonalRecipeScroll
    {
        public CrystalSupplicantStatueEastRecipe()
            : base( (int)DefCrystalCrafting.CrystalCrafterRecipes.CrystalSupplicantStatueEast, null )
        {
            Name = "Crystal Supplicant Statue East Recipe";
        }

        public CrystalSupplicantStatueEastRecipe( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class CrystalRunnerStatueSouthRecipe : PersonalRecipeScroll
    {
        public CrystalRunnerStatueSouthRecipe()
            : base( (int)DefCrystalCrafting.CrystalCrafterRecipes.CrystalRunnerStatueSouth, null )
        {
            Name = "Crystal Runner Statue South Recipe";
        }

        public CrystalRunnerStatueSouthRecipe( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class CrystalRunnerStatueEastRecipe : PersonalRecipeScroll
    {
        public CrystalRunnerStatueEastRecipe()
            : base( (int)DefCrystalCrafting.CrystalCrafterRecipes.CrystalRunnerStatueEast, null )
        {
            Name = "Crystal Runner Statue East Recipe";
        }

        public CrystalRunnerStatueEastRecipe( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class PurpleCrystalRecipe : PersonalRecipeScroll
    {
        public PurpleCrystalRecipe()
            : base( (int)DefCrystalCrafting.CrystalCrafterRecipes.PurpleCrystal, null )
        {
            Name = "Purple Crystal Recipe";
        }

        public PurpleCrystalRecipe( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class GreenCrystalRecipe : PersonalRecipeScroll
    {
        public GreenCrystalRecipe()
            : base( (int)DefCrystalCrafting.CrystalCrafterRecipes.GreenCrystal, null )
        {
        }

        public GreenCrystalRecipe( Serial serial )
            : base( serial )
        {
            Name = "Green Crystal Recipe";
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class MagicCrystalBallRecipe : PersonalRecipeScroll
    {
        public MagicCrystalBallRecipe()
            : base( (int)DefCrystalCrafting.CrystalCrafterRecipes.MagicCrystalBall, null )
        {
            Name = "Magic Crystal Ball Recipe";
        }

        public MagicCrystalBallRecipe( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }

    public class GlobeOfSosariaAddonRecipe : PersonalRecipeScroll
    {
        public GlobeOfSosariaAddonRecipe()
            : base( (int)DefCrystalCrafting.CrystalCrafterRecipes.GlobeOfSosariaAddon, null )
        {
            Name = "Globe Of Sosaria Recipe";
        }

        public GlobeOfSosariaAddonRecipe( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }
}