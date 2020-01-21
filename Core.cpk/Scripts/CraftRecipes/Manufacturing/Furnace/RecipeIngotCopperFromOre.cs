﻿namespace AtomicTorch.CBND.CoreMod.CraftRecipes
{
    using System;
    using AtomicTorch.CBND.CoreMod.Items;
    using AtomicTorch.CBND.CoreMod.Items.Generic;
    using AtomicTorch.CBND.CoreMod.StaticObjects.Structures.Manufacturers;
    using AtomicTorch.CBND.CoreMod.Systems;
    using AtomicTorch.CBND.CoreMod.Systems.Crafting;

    public class RecipeIngotCopperFromOre : Recipe.RecipeForManufacturing
    {
        protected override void SetupRecipe(
            StationsList stations,
            out TimeSpan duration,
            InputItems inputItems,
            OutputItems outputItems)
        {
            stations.Add<ObjectFurnace>()
                    .Add<ObjectFurnaceElectric>();

            duration = CraftingDuration.Medium;

            inputItems.Add<ItemOreCopper>(count: 5);

            outputItems.Add<ItemIngotCopper>();

            this.Icon = ClientItemIconHelper.CreateComposedIcon(
                name: this.Id + "Icon",
                primaryIcon: GetItem<ItemIngotCopper>().Icon,
                secondaryIcon: GetItem<ItemOreCopper>().Icon);
        }
    }
}