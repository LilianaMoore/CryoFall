﻿namespace AtomicTorch.CBND.CoreMod.CharacterSkeletons
{
    using AtomicTorch.CBND.CoreMod.Systems.Physics;
    using AtomicTorch.CBND.GameApi.Data.Physics;
    using AtomicTorch.CBND.GameApi.Resources;
    using AtomicTorch.CBND.GameApi.ServicesClient.Components;

    public class SkeletonTurtle : ProtoCharacterSkeletonAnimal
    {
        public override double DefaultMoveSpeed => 0.3;

        public override double IconScale => 0.6;

        public override float OrientationDownExtraAngle => 5;

        public override SkeletonResource SkeletonResourceBack { get; }
            = new SkeletonResource("Turtle/Back");

        public override SkeletonResource SkeletonResourceFront { get; }
            = new SkeletonResource("Turtle/Front");

        public override double WorldScale => 0.4;

        protected override string SoundsFolderPath => "Skeletons/Turtle";

        public override void ClientSetupShadowRenderer(IComponentSpriteRenderer shadowRenderer, double scaleMultiplier)
        {
            shadowRenderer.PositionOffset = (0, 0);
            shadowRenderer.Scale = 1.02 * scaleMultiplier;
        }

        public override void CreatePhysics(IPhysicsBody physicsBody)
        {
            physicsBody
                .AddShapeRectangle(size: (0.6, 0.25),
                                   offset: (-0.3, -0.05))
                .AddShapeCircle(radius: 0.35,
                                center: (0, 0.25),
                                group: CollisionGroups.HitboxMelee)
                .AddShapeCircle(radius: 0.35,
                                center: (0, 0.25),
                                group: CollisionGroups.HitboxRanged);
        }
    }
}