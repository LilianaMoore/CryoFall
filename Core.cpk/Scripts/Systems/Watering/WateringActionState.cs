﻿namespace AtomicTorch.CBND.CoreMod.Systems.Watering
{
    using AtomicTorch.CBND.CoreMod.Characters.Player;
    using AtomicTorch.CBND.CoreMod.Items.Tools;
    using AtomicTorch.CBND.CoreMod.Items.Tools.WateringCans;
    using AtomicTorch.CBND.CoreMod.SoundPresets;
    using AtomicTorch.CBND.GameApi.Data.Characters;
    using AtomicTorch.CBND.GameApi.Data.Items;
    using AtomicTorch.CBND.GameApi.Data.World;
    using AtomicTorch.GameEngine.Common.Helpers;

    public class WateringActionState
        : ActionSystemState<
            WateringSystem,
            ItemWorldActionRequest,
            WateringActionState,
            WateringActionState.PublicState>
    {
        public readonly IItem ItemWateringCan;

        public WateringActionState(
            ICharacter character,
            IWorldObject targetWorldObject,
            double durationSeconds,
            IItem itemWateringCan)
            : base(character, targetWorldObject, durationSeconds)
        {
            this.ItemWateringCan = itemWateringCan;
        }

        public class PublicState : BasePublicActionState
        {
            protected override void ClientOnCompleted()
            {
                if (this.IsCancelled)
                {
                    return;
                }

                if (this.Character.SharedGetPlayerSelectedHotbarItemProto()
                        is IProtoItemToolWateringCan protoWateringCan)
                {
                    protoWateringCan.SharedGetItemSoundPreset()
                                    .PlaySound(ItemSound.Use,
                                               this.Character,
                                               pitch: RandomHelper.Range(0.95f, 1.05f));
                }
            }

            protected override void ClientOnStart()
            {
                // TODO: play animation
            }
        }
    }
}