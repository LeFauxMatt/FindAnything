using LeFauxMods.Common.Integrations.FindAnything;
using LeFauxMods.FindAnything.Models;
using StardewValley.Objects;

namespace LeFauxMods.FindAnything.Modules;

/// <summary>Searches for items in storages.</summary>
internal sealed class FindInStorage
{
    public FindInStorage(IFindAnythingApi api) => api.Subscribe(OnSearchSubmitted);

    private static void OnSearchSubmitted(ISearchSubmitted e)
    {
        // Search chests
        foreach (var chest in e.Location.Objects.Values.OfType<Chest>())
        {
            var items = chest.GetItemsForPlayer();
            if (items.Count == 0)
            {
                continue;
            }

            var item = items.FirstOrDefault(item =>
                item?.DisplayName.Contains(e.Text, StringComparison.OrdinalIgnoreCase) == true);

            if (item is not null)
            {
                var itemData = ItemRegistry.GetDataOrErrorItem(item.QualifiedItemId);
                e.AddResult(new FoundObject(chest, itemData));
            }
        }

        // Search furniture with storage
        foreach (var furniture in e.Location.furniture.OfType<StorageFurniture>())
        {
            if (furniture.heldItems.Count == 0)
            {
                continue;
            }

            var item = furniture.heldItems.FirstOrDefault(item =>
                item?.DisplayName.Contains(e.Text, StringComparison.OrdinalIgnoreCase) == true);

            if (item is not null)
            {
                var itemData = ItemRegistry.GetDataOrErrorItem(item.QualifiedItemId);
                e.AddResult(new FoundFurniture(furniture, itemData));
            }
        }

        // Search buildings with storage
        foreach (var building in e.Location.buildings)
        {
            if (building.GetBuildingChest("Output") is { } chest)
            {
                var items = chest.GetItemsForPlayer();
                if (items.Count == 0)
                {
                    continue;
                }

                var item = items.FirstOrDefault(item =>
                    item?.DisplayName.Contains(e.Text, StringComparison.OrdinalIgnoreCase) == true);

                if (item is not null)
                {
                    var itemData = ItemRegistry.GetDataOrErrorItem(item.QualifiedItemId);
                    e.AddResult(new FoundBuilding(building, itemData));
                }
            }
        }
    }
}
