﻿namespace AtomicTorch.CBND.CoreMod.CraftRecipes
{
    using System;
    using AtomicTorch.CBND.CoreMod.Items.Generic;
    using AtomicTorch.CBND.CoreMod.StaticObjects.Structures.CraftingStations;
    using AtomicTorch.CBND.CoreMod.Systems;
    using AtomicTorch.CBND.CoreMod.Systems.Crafting;

    public class RecipeCanisterEmpty : Recipe.RecipeForStationCrafting
    {
        protected override void SetupRecipe(
            StationsList stations,
            out TimeSpan duration,
            InputItems inputItems,
            OutputItems outputItems)
        {
            stations.Add<ObjectWorkbench>();

            duration = CraftingDuration.VeryShort;

            inputItems.Add<ItemIngotCopper>(count: 2);
            inputItems.Add<ItemIngotIron>(count: 2);

            outputItems.Add<ItemCanisterEmpty>(count: 1);
        }
    }
}