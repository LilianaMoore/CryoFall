﻿namespace AtomicTorch.CBND.CoreMod.Technologies.Tier2.Decorations
{
    using AtomicTorch.CBND.CoreMod.StaticObjects.Structures.Decorations;

    public class TechNodeDecorationSofa : TechNode<TechGroupDecorations>
    {
        protected override void PrepareTechNode(Config config)
        {
            config.Effects
                  .AddStructure<ObjectDecorationSofa>();

            config.SetRequiredNode<TechNodeDecorationBookshelf>();
        }
    }
}