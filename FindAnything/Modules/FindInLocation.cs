using LeFauxMods.Common.Integrations.FindAnything;
using LeFauxMods.FindAnything.Models;
using StardewValley.Characters;
using StardewValley.TokenizableStrings;

namespace LeFauxMods.FindAnything.Modules;

internal sealed class FindInLocation
{
    public FindInLocation(IFindAnythingApi api) => api.Subscribe(OnSearchSubmitted);

    private static void OnSearchSubmitted(ISearchSubmitted e)
    {
        // Search for objects by name
        foreach (var @object in e.Location.Objects.Values.Where(@object =>
                     @object.DisplayName.Contains(e.Text, StringComparison.OrdinalIgnoreCase)))
        {
            e.AddResult(new FoundObject(@object));
        }

        // Search for buildings by name
        foreach (var building in from building in e.Location.buildings
                 let buildingData = building.GetData()
                 where TokenParser.ParseText(buildingData.Name).Contains(e.Text, StringComparison.OrdinalIgnoreCase)
                 select building)
        {
            e.AddResult(new FoundBuilding(building));
        }

        // Search for NPCs by name
        foreach (var character in e.Location.characters)
        {
            if (character.displayName.Contains(e.Text, StringComparison.OrdinalIgnoreCase))
            {
                e.AddResult(new FoundCharacter(character));
                continue;
            }

            if (character is Pet pet)
            {
                var petData = pet.GetPetData();
                if (TokenParser.ParseText(petData.DisplayName).Contains(e.Text, StringComparison.OrdinalIgnoreCase))
                {
                    e.AddResult(new FoundCharacter(character));
                }
            }
        }

        // Search for animals by name
        foreach (var animal in e.Location.animals.Values)
        {
            if (animal.displayName.Contains(e.Text, StringComparison.OrdinalIgnoreCase) ||
                animal.displayType.Contains(e.Text, StringComparison.OrdinalIgnoreCase))
            {
                e.AddResult(new FoundCharacter(animal));
            }
        }

        // Search for debris
        foreach (var debris in e.Location.debris)
        {
            if (debris.item?.DisplayName.Contains(e.Text, StringComparison.OrdinalIgnoreCase) == true)
            {
                var itemData = ItemRegistry.GetDataOrErrorItem(debris.item.QualifiedItemId);
                e.AddResult(new FoundDebris(debris, e.Location, itemData));
            }
        }
    }
}
