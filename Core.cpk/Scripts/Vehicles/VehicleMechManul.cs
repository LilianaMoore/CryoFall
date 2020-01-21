﻿namespace AtomicTorch.CBND.CoreMod.Vehicles
{
    using System.Collections.Generic;
    using AtomicTorch.CBND.CoreMod.CharacterSkeletons;
    using AtomicTorch.CBND.CoreMod.ClientComponents.Rendering.Lighting;
    using AtomicTorch.CBND.CoreMod.ItemContainers.Vehicles;
    using AtomicTorch.CBND.CoreMod.Items;
    using AtomicTorch.CBND.CoreMod.Items.Generic;
    using AtomicTorch.CBND.CoreMod.Systems;
    using AtomicTorch.CBND.CoreMod.Systems.Physics;
    using AtomicTorch.CBND.GameApi.Data.World;
    using AtomicTorch.CBND.GameApi.Scripting;
    using AtomicTorch.GameEngine.Common.Primitives;

    public class VehicleMechManul : ProtoVehicleMech
    {
        public override byte CargoItemsSlotsCount => 48;

        public override string Description =>
            "Medium design for mechanized battle armor. Boasts substantial defensive capabilities and massive cargo holds without sacrificing in other areas.";

        public override ushort EnergyUsePerSecondIdle => 85;

        public override ushort EnergyUsePerSecondMoving => 300;

        public override BaseItemsContainerMechEquipment EquipmentItemsContainerType
            => Api.GetProtoEntity<ContainerMechEquipmentSkipper>();

        public override bool IsPlayersHotbarAndEquipmentItemsAllowed => false;

        public override double MaxDistanceToInteract => 1;

        public override string Name => "Manul";

        public override double PhysicsBodyAccelerationCoef => 3;

        public override double PhysicsBodyFriction => 10;

        public override double StatMoveSpeed => 2.0;

        public override double StatMoveSpeedRunMultiplier => 1.0; // no run mode

        public override float StructurePointsMax => 600;

        public override double VehicleWorldHeight => 2.0;

        protected override void PrepareDefense(DefenseDescription defense)
        {
            defense.Set(
                impact: 0.60,
                kinetic: 0.60,
                heat: 0.50,
                cold: 0.50,
                chemical: 0.70,
                electrical: 0.40,
                radiation: 0.0,
                psi: 0.0);
        }

        protected override void PrepareDismountPoints(List<Vector2D> dismountPoints)
        {
            dismountPoints.Add((0, -0.36));     // down
            dismountPoints.Add((-0.45, -0.36)); // down-left
            dismountPoints.Add((0.45, -0.36));  // down-right
            dismountPoints.Add((0, 0.36));      // up
            dismountPoints.Add((-0.7, 0));      // left
            dismountPoints.Add((0.7, 0));       // right
            dismountPoints.Add((-0.45, 0.36));  // up-left
            dismountPoints.Add((0.45, 0.36));   // up-right
        }

        protected override void PrepareProtoVehicle(
            InputItems buildRequiredItems,
            InputItems repairStageRequiredItems,
            out int repairStagesCount)
        {
            buildRequiredItems
                .Add<ItemStructuralPlating>(15)
                .Add<ItemUniversalActuator>(8)
                .Add<ItemImpulseEngine>(5)
                .Add<ItemComponentsHighTech>(15);

            repairStageRequiredItems
                .Add<ItemIngotSteel>(10)
                .Add<ItemStructuralPlating>(1)
                .Add<ItemUniversalActuator>(1)
                .Add<ItemComponentsHighTech>(1);

            repairStagesCount = 5;
        }

        protected override void PrepareProtoVehicleLightConfig(ItemLightConfig lightConfig)
        {
            lightConfig.Color = LightColors.Flashlight;
            lightConfig.ScreenOffset = (3, -2);
            lightConfig.WorldOffset = (0, -0.5);
            lightConfig.Size = 18;
        }

        protected override void SharedCreatePhysics(CreatePhysicsData data)
        {
            base.SharedCreatePhysics(data);

            var physicsBody = data.PhysicsBody;
            if (data.PublicState.PilotCharacter == null)
            {
                // no pilot
                physicsBody.AddShapeRectangle(size: (0.9, 1.5),
                                              offset: (-0.45, -0.4),
                                              @group: CollisionGroups.ClickArea);
            }
        }

        protected override void SharedGetSkeletonProto(
            IDynamicWorldObject gameObject,
            out ProtoCharacterSkeleton protoSkeleton,
            ref double scale)
        {
            protoSkeleton = Api.GetProtoEntity<SkeletonMechManul>();
        }
    }
}