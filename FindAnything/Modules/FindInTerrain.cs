using LeFauxMods.Common.Integrations.FindAnything;
using LeFauxMods.FindAnything.Models;
using StardewValley.ItemTypeDefinitions;
using StardewValley.TerrainFeatures;
using StardewValley.TokenizableStrings;

namespace LeFauxMods.FindAnything.Modules;

/// <summary>Search through data related to terrain features</summary>
internal sealed class FindInTerrain
{
    public FindInTerrain(IFindAnythingApi api) => api.Subscribe(OnSearchSubmitted);

    private static void OnSearchSubmitted(ISearchSubmitted e)
    {
        // Search each terrain type by the crop
        foreach (var terrainFeature in e.Location.terrainFeatures.Values)
        {
            ParsedItemData? itemData;
            switch (terrainFeature)
            {
                case GiantCrop giantCrop when giantCrop.GetData() is { } giantCropData:
                    itemData = ItemRegistry.GetDataOrErrorItem(giantCropData.FromItemId);
                    if (TokenParser.ParseText(itemData.DisplayName)
                        .Contains(e.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        e.AddResult(new FoundTerrain(giantCrop, itemData));
                    }

                    continue;
                case HoeDirt { crop: { } crop } hoeDirt when crop.GetData() is { } cropData:
                    itemData = ItemRegistry.GetDataOrErrorItem(cropData.HarvestItemId);
                    if (TokenParser.ParseText(itemData.DisplayName)
                        .Contains(e.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        e.AddResult(new FoundTerrain(hoeDirt, itemData));
                    }

                    continue;
                case FruitTree fruitTree when fruitTree.GetData() is { } fruitTreeData:
                    if (TokenParser.ParseText(fruitTreeData.DisplayName)
                        .Contains(e.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        e.AddResult(new FoundTerrain(fruitTree));
                    }

                    // TBD: Search fruit

                    continue;
                case Tree tree when tree.GetData() is { } treeData:
                    itemData = ItemRegistry.GetDataOrErrorItem(treeData.SeedItemId);
                    if (TokenParser.ParseText(itemData.DisplayName)
                        .Contains(e.Text, StringComparison.OrdinalIgnoreCase))
                    {
                        e.AddResult(new FoundTerrain(tree, itemData));
                    }

                    continue;
            }
        }
    }
}
