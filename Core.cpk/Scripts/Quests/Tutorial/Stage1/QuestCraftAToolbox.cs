﻿namespace AtomicTorch.CBND.CoreMod.Quests.Tutorial
{
    using System.Collections.Generic;
    using AtomicTorch.CBND.CoreMod.CraftRecipes;
    using AtomicTorch.CBND.CoreMod.PlayerTasks;

    public class QuestCraftAToolbox : ProtoQuest
    {
        public override string Description =>
            "To start building structures (such as your house), you need a toolbox. Toolboxes serve as the main building tool in CryoFall.";

        public override string Hints =>
            @"[*] Use your handcrafting menu and craft a toolbox.
              [*] You may need to craft some rope first if you don't have any.";

        public override string Name => "Craft a toolbox";

        public override ushort RewardLearningPoints => QuestConstants.TutorialRewardStage1;

        protected override void PrepareQuest(QuestsList prerequisites, TasksList tasks, HintsList hints)
        {
            tasks
                .Add(TaskCraftRecipe.RequireHandRecipe<RecipeToolboxT1>());

            prerequisites
                .Add<QuestChopDownATree>();
        }
    }
}