﻿namespace AtomicTorch.CBND.CoreMod.Systems.ItemFreshnessSystem
{
    using System;
    using AtomicTorch.CBND.CoreMod.ItemContainers;
    using AtomicTorch.CBND.CoreMod.Items;
    using AtomicTorch.CBND.CoreMod.Items.Generic;
    using AtomicTorch.CBND.CoreMod.StaticObjects.Vegetation.Plants;
    using AtomicTorch.CBND.GameApi.Data.Items;
    using AtomicTorch.CBND.GameApi.Scripting;
    using AtomicTorch.CBND.GameApi.ServicesServer;
    using AtomicTorch.GameEngine.Common.Helpers;
    using static ItemFreshnessConstants;

    public class ItemFreshnessSystem : ProtoSystem<ItemFreshnessSystem>
    {
        private const uint FreshnessFractionsPerSecond = 1000;

        private static readonly IItemsServerService ServerItemsService = IsServer ? Server.Items : null;

        private static IProtoItem protoItemRottenFood;

        public override string Name => "Food spoilage system";

        public static void ServerInitializeItem(IItemWithFreshnessPrivateState privateState, bool isFirstTimeInit)
        {
            var item = (IItem)privateState.GameObject;
            var protoItem = item.ProtoItem as IProtoItemWithFreshness
                            ?? throw new Exception(
                                $"The item {item} proto class doesn't implement {nameof(IProtoItem)}");

            privateState.FreshnessCurrent = isFirstTimeInit
                                                ? protoItem.FreshnessMaxValue
                                                : Math.Min(privateState.FreshnessCurrent, protoItem.FreshnessMaxValue);
        }

        public static void ServerUpdateFreshness(IItem item, double deltaTime)
        {
            var protoItem = (IProtoItemWithFreshness)item.ProtoItem;
            if (protoItem.FreshnessMaxValue == 0)
            {
                // unlimited freshness
                return;
            }

            var privateState = item.GetPrivateState<IItemWithFreshnessPrivateState>();
            var freshness = (long)privateState.FreshnessCurrent;
            var freshnessDecrease = (uint)(deltaTime
                                           * FreshnessFractionsPerSecond
                                           * ServerFreshnessDecaySpeedMultiplier);
            if (freshnessDecrease == 0)
            {
                // no freshness decrease
                return;
            }

            var container = item.Container;
            if (container == null)
            {
                // should be impossible - abandoned item
                return;
            }

            if (container.ProtoItemsContainer is IProtoItemsContainerFridge protoFridge)
            {
                var freshnessDecreaseCoefficient =
                    protoFridge.SharedGetCurrentFoodFreshnessDecreaseCoefficient(container);
                if (freshnessDecreaseCoefficient <= 0)
                {
                    // this fridge container stops spoilage
                    return;
                }

                freshnessDecreaseCoefficient = Math.Min(freshnessDecreaseCoefficient, 1);
                if (freshnessDecreaseCoefficient < 1.0)
                {
                    // calculate freshness decrease value 
                    freshnessDecrease = (uint)Math.Round(freshnessDecrease * freshnessDecreaseCoefficient,
                                                         MidpointRounding.AwayFromZero);

                    freshnessDecrease = Math.Max(freshnessDecrease, 1);
                }
            }

            freshness -= freshnessDecrease;

            if (freshness > 0)
            {
                privateState.FreshnessCurrent = (uint)freshness;
                return;
            }

            // food spoiled
            var containerSlotId = item.ContainerSlotId;
            var count = Math.Min(item.Count, protoItemRottenFood.MaxItemsPerStack);

            // destroy food item
            ServerItemsService.DestroyItem(item);

            // spawn a rotten item in place of the destroyed spoiled item
            ServerItemsService.CreateItem(protoItemRottenFood, container, count, containerSlotId);
        }

        public static uint SharedCalculateFreshnessMaxValue(IProtoItemWithFreshness protoItem)
        {
            var freshnessDuration = protoItem.FreshnessDuration;
            var freshnessMaxValue = FreshnessFractionsPerSecond * (ulong)freshnessDuration.TotalSeconds;
            if (freshnessMaxValue > uint.MaxValue)
            {
                throw new Exception(
                    $"Freshness duration exceeded max value of {TimeSpan.FromSeconds(uint.MaxValue / (double)FreshnessFractionsPerSecond)}."
                    + $" Provided value is {freshnessDuration}");
            }

            return (uint)freshnessMaxValue;
        }

        public static double SharedCalculateTimeToSpoilRemains(IItem item)
        {
            var protoItem = (IProtoItemWithFreshness)item.ProtoItem;
            if (protoItem.FreshnessMaxValue == 0)
            {
                // unlimited freshness
                return double.NaN;
            }

            var privateState = item.GetPrivateState<IItemWithFreshnessPrivateState>();

            var container = item.Container;
            if (container == null)
            {
                // should be impossible - abandoned item
                return double.NaN;
            }

            var result = privateState.FreshnessCurrent
                         / (FreshnessFractionsPerSecond
                            * (IsServer
                                   ? ServerFreshnessDecaySpeedMultiplier
                                   : ClientFreshnessDecaySpeedMultiplier));

            if (container.ProtoItemsContainer is IProtoItemsContainerFridge protoFridge)
            {
                var freshnessDecreaseCoefficient =
                    protoFridge.SharedGetCurrentFoodFreshnessDecreaseCoefficient(container);
                if (freshnessDecreaseCoefficient <= 0)
                {
                    // this fridge container stops spoilage
                    return double.NaN;
                }

                freshnessDecreaseCoefficient = Math.Min(freshnessDecreaseCoefficient, 1);
                if (freshnessDecreaseCoefficient < 1.0)
                {
                    // basically, it extends the freshness
                    result /= freshnessDecreaseCoefficient;
                }
            }

            return result;
        }

        public static int SharedCompareFreshness(IItem itemA, IItem itemB)
        {
            var freshnessA = itemA.GetPrivateState<IItemWithFreshnessPrivateState>().FreshnessCurrent;
            var freshnessB = itemB.GetPrivateState<IItemWithFreshnessPrivateState>().FreshnessCurrent;
            return freshnessA.CompareTo(freshnessB);
        }

        public static int SharedCompareFreshnessReverse(IItem itemA, IItem itemB)
        {
            return -SharedCompareFreshness(itemA, itemB);
        }

        public static ItemFreshness SharedGetFreshnessEnum(IItem item)
        {
            var fraction = SharedGetFreshnessFraction(item);
            if (fraction > 0.5)
            {
                return ItemFreshness.Green;
            }

            if (fraction > 0.2)
            {
                return ItemFreshness.Yellow;
            }

            return ItemFreshness.Red;
        }

        public static double SharedGetFreshnessFraction(IItem item)
        {
            var protoItem = item.ProtoItem as IProtoItemWithFreshness
                            ?? throw new Exception(
                                item + " prototype doesn't implement " + typeof(IProtoItemWithFreshness));
            var freshnessMaxValue = protoItem.FreshnessMaxValue;
            if (freshnessMaxValue == 0)
            {
                // always fresh
                return 1.0;
            }

            var freshnessCurrent = item.GetPrivateState<IItemWithFreshnessPrivateState>().FreshnessCurrent;
            freshnessCurrent = MathHelper.Clamp(freshnessCurrent, 0, freshnessMaxValue);
            return freshnessCurrent / (double)freshnessMaxValue;
        }

        /// <summary>
        /// Gets the food positive effects coefficient depending on the food freshness.
        /// </summary>
        public static float SharedGetFreshnessPositiveEffectsCoef(ItemFreshness freshness)
        {
            switch (freshness)
            {
                case ItemFreshness.Green:
                    return 1.0f;

                case ItemFreshness.Yellow:
                    return 0.7f;

                case ItemFreshness.Red:
                    return 0.4f;

                default:
                    throw new ArgumentOutOfRangeException(nameof(freshness), freshness, null);
            }
        }

        public static bool SharedIsRefrigerated(IItem item)
        {
            var container = item.Container;
            if (container == null)
            {
                // should be impossible - abandoned item
                return false;
            }

            if (!(container.ProtoItemsContainer is IProtoItemsContainerFridge protoFridge))
            {
                return false;
            }

            var freshnessDecreaseCoefficient =
                protoFridge.SharedGetCurrentFoodFreshnessDecreaseCoefficient(container);
            if (freshnessDecreaseCoefficient <= 0)
            {
                // this fridge container stops spoilage
                return true;
            }

            freshnessDecreaseCoefficient = Math.Min(freshnessDecreaseCoefficient, 1);
            return freshnessDecreaseCoefficient < 1.0;
        }

        // TODO: move this to separate system in A28
        public double ServerRemote_RequestFarmPlantsSpoilSpeedMultiplier()
        {
            return FarmingConstants.SharedFarmPlantsSpoilSpeedMultiplier;
        }

        public double ServerRemote_RequestFreshnessDecaySpeedMultiplier()
        {
            return ServerFreshnessDecaySpeedMultiplier;
        }

        protected override void PrepareSystem()
        {
            if (IsServer)
            {
                protoItemRottenFood = Api.GetProtoEntity<ItemRot>();
            }
        }
    }
}