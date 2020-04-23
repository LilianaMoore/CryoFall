﻿namespace AtomicTorch.CBND.CoreMod.StaticObjects.Props.Bridge
{
    using AtomicTorch.CBND.CoreMod.Systems.Physics;
    using AtomicTorch.CBND.GameApi.Data.World;
    using AtomicTorch.CBND.GameApi.ServicesClient.Components;

    public class ObjectPropBridgeHorizontalRight : ProtoObjectPropPlatform
    {
        protected override void ClientSetupRenderer(IComponentSpriteRenderer renderer)
        {
            base.ClientSetupRenderer(renderer);
            renderer.PositionOffset = (0, -2);
        }

        protected override void CreateLayout(StaticObjectLayout layout)
        {
            layout.Setup("##",
                         "##");
        }

        protected override void SharedCreatePhysics(CreatePhysicsData data)
        {
            const double xOffset = 0.2;
            data.PhysicsBody
                .AddShapeRectangle((2 - xOffset, 1), (0, -1), CollisionGroups.Default)
                .AddShapeRectangle((2 - xOffset, 1), (0, 2),  CollisionGroups.Default);
        }
    }
}