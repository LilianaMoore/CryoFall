﻿namespace AtomicTorch.CBND.CoreMod.Quests.Tutorial
{
    using System.Collections.Generic;
    using AtomicTorch.CBND.CoreMod.CraftRecipes;
    using AtomicTorch.CBND.CoreMod.Items.Generic;
    using AtomicTorch.CBND.CoreMod.Items.Tools;
    using AtomicTorch.CBND.CoreMod.Items.Tools.WateringCans;
    using AtomicTorch.CBND.CoreMod.PlayerTasks;
    using AtomicTorch.CBND.CoreMod.StaticObjects.Structures.Manufacturers;
    using AtomicTorch.CBND.GameApi.Scripting;

    public class QuestBuildMulchboxAndCraftWateringCan : ProtoQuest
    {
        public const string TaskUseMulch = "Use mulch";

        public const string TaskUseWateringCan = "Use watering can";

        public override string Description =>
            "You can improve the growth speed of your plants if you regularly water them and use fertilizer.";

        public override string Hints =>
            @"[*] Regularly watering your plants is the best way to speed up their growth. It also costs no resources other than water.
              [*] You can refill your watering can in a water collector or a well. Just left click on it with a watering can selected in your hotbar.
              [*] To create mulch, simply drop any organic items (plants, rotten food, etc.) in and wait for them to decompose.
              [*] Mulch is easy to obtain and is all-natural fertilizer. You can use it to speed up growth of any of your plants.";

        public override string Name => "Farming efficiency";

        public override ushort RewardLearningPoints => QuestConstants.TutorialRewardStage2;

        protected override void PrepareQuest(QuestsList prerequisites, TasksList tasks, HintsList hints)
        {
            tasks
                .Add(TaskCraftRecipe.RequireStationRecipe<RecipeWateringCanWood>())
                .Add(TaskBuildStructure.Require<ObjectMulchbox>())
                .Add(TaskUseItem.Require<IProtoItemToolWateringCan>(description: TaskUseWateringCan)
                                .WithIcon(Api.GetProtoEntity<ItemWateringCanWood>().Icon))
                .Add(TaskUseItem.Require<ItemMulch>(description: TaskUseMulch));

            prerequisites
                .Add<QuestBuildAFarm>()
                .Add<QuestClaySandGlassBottlesWaterCollector>();
        }
    }
}