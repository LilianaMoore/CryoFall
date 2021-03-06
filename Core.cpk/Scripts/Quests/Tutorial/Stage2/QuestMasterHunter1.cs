﻿namespace AtomicTorch.CBND.CoreMod.Quests.Tutorial
{
    using System.Collections.Generic;
    using AtomicTorch.CBND.CoreMod.Characters.Mobs;
    using AtomicTorch.CBND.CoreMod.PlayerTasks;

    public class QuestMasterHunter1 : ProtoQuest
    {
        public override string Description =>
            "Time to go hunting! For starters it may be better to avoid more dangerous animals, but there are plenty of other options out there.";

        public override string Hints =>
            @"[*] Each animal has different loot you can expect to find. Your hunting skill also plays a significant role in this.
              [*] Hunting is a great way to gain more experience and learning points.
              [*] Please don't do this in real life :(";

        public override string Name => "Master hunter—part one";

        public override ushort RewardLearningPoints => QuestConstants.TutorialRewardStage2;

        protected override void PrepareQuest(QuestsList prerequisites, TasksList tasks, HintsList hints)
        {
            tasks
                .Add(TaskKill.Require<MobChicken>(count: 1))
                .Add(TaskKill.Require<MobCrab>(count: 1))
                .Add(TaskKill.Require<MobRiverSnail>(count: 1))
                .Add(TaskKill.Require<MobStarfish>(count: 1));

            prerequisites
                .Add<QuestCraftRangedWeapon>()
                .Add<QuestCraftAndEquipBetterArmor>();
        }
    }
}