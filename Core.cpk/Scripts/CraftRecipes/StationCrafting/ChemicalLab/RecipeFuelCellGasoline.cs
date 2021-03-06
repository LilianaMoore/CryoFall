﻿namespace AtomicTorch.CBND.CoreMod.CraftRecipes
{
    using System;
    using AtomicTorch.CBND.CoreMod.Items.Generic;
    using AtomicTorch.CBND.CoreMod.StaticObjects.Structures.CraftingStations;
    using AtomicTorch.CBND.CoreMod.Systems;
    using AtomicTorch.CBND.CoreMod.Systems.Crafting;

    public class RecipeFuelCellGasoline : Recipe.RecipeForStationCrafting
    {
        protected override void SetupRecipe(
            StationsList stations,
            out TimeSpan duration,
            InputItems inputItems,
            OutputItems outputItems)
        {
            stations.Add<ObjectChemicalLab>();

            duration = CraftingDuration.Medium;

            inputItems.Add<ItemFuelCellEmpty>(count: 1);
            inputItems.Add<ItemCanisterGasoline>(count: 20);

            outputItems.Add<ItemFuelCellGasoline>(count: 1);
            outputItems.Add<ItemCanisterEmpty>(count: 20);
        }
    }
}