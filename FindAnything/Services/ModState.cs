using LeFauxMods.Common.Services;
using StardewModdingAPI.Utilities;

namespace LeFauxMods.FindAnything.Services;

/// <summary>Responsible for managing state.</summary>
internal sealed class ModState
{
    private static ModState? Instance;

    private readonly ConfigHelper<ModConfig> configHelper;
    private readonly IModHelper helper;
    private readonly PerScreen<Pointers?> pointers = new();
    private readonly PerScreen<string> searchText = new(static () => string.Empty);

    private ModState(IModHelper helper)
    {
        this.helper = helper;
        this.configHelper = new ConfigHelper<ModConfig>(helper);
    }

    public static HashSet<string> CachedKeywords { get; } = new(StringComparer.OrdinalIgnoreCase);

    public static ModConfig Config => Instance!.configHelper.Config;

    public static ConfigHelper<ModConfig> ConfigHelper => Instance!.configHelper;

    public static Pointers? Pointers
    {
        get => Instance!.pointers.Value;
        set => Instance!.pointers.Value = value;
    }

    public static string SearchText
    {
        get => Instance!.searchText.Value;
        set => Instance!.searchText.Value = value;
    }

    public static void Init(IModHelper helper) => Instance ??= new ModState(helper);
}