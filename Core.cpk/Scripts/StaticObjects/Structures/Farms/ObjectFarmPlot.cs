﻿namespace AtomicTorch.CBND.CoreMod.StaticObjects.Structures.Farms
{
    using System;
    using AtomicTorch.CBND.CoreMod.Items.Generic;
    using AtomicTorch.CBND.CoreMod.SoundPresets;
    using AtomicTorch.CBND.CoreMod.Systems.Construction;
    using AtomicTorch.CBND.GameApi.Resources;

    public class ObjectFarmPlot : ProtoObjectFarmPlot
    {
        public override string Description =>
            "Can be used to grow a variety of crops and other plants. Farm plot can only be constructed over fertile land.";

        public override bool IsDrawingPlantShadow => true;

        public override string Name => "Farm plot";

        public override ObjectMaterial ObjectMaterial => ObjectMaterial.SolidGround;

        public override float StructurePointsMax => 400;

        public override ITextureResource Texture { get; }
            = new TextureResource("Terrain/Field/TileField1.jpg",
                                  isTransparent: false);

        protected override TextureResource TextureFieldFertilized { get; }
            = new TextureResource("Terrain/Field/FertilizedField.png");

        protected override TextureResource TextureFieldWatered { get; }
            = new TextureResource("Terrain/Field/WetField.png");

        protected override ITextureResource PrepareDefaultTexture(Type thisType)
            => this.Texture;

        protected override void PrepareFarmPlotConstructionConfig(
            ConstructionTileRequirements tileRequirements,
            ConstructionStageConfig build,
            ConstructionStageConfig repair)
        {
            build.StagesCount = 1;
            build.StageDurationSeconds = BuildDuration.Short;
            build.AddStageRequiredItem<ItemPlanks>(count: 20);
            build.AddStageRequiredItem<ItemSand>(count: 5);

            repair.StagesCount = 1;
            repair.StageDurationSeconds = BuildDuration.Short;
            repair.AddStageRequiredItem<ItemPlanks>(count: 10);
        }

        protected sealed override void SharedCreatePhysics(CreatePhysicsData data)
        {
        }
    }
}