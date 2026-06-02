using HarmonyLib;
using ResoniteHotReloadLib;
using ResoniteModLoader;

namespace WikiLinks;

public partial class WikiLinks : ResoniteMod
{
	public override string Name => ModName;
	public override string Author => ModAuthor;
	public const string HarmonyId = $"com.{ModAuthor}.{ModName}";
	public static ModConfiguration? Config;
	private static Harmony? _harmony;

	public static void Init()
	{
		_harmony ??= new Harmony(HarmonyId);
		_harmony.PatchAll();

		Msg($"{ModName} initialized");
	}

	public static void Unload()
	{
		_harmony?.UnpatchAll(HarmonyId);
		Msg($"{ModName} unloaded");
	}

	public override void OnEngineInit()
	{
		Msg($"{ModName} loaded!");

		Config = GetConfiguration();
#if DEBUG
		HotReloader.RegisterForHotReload(this);
#endif
		Init();
	}

#if DEBUG
	public static void BeforeHotReload()
	{
		Unload();
	}

	public static void OnHotReload(ResoniteMod modInstance)
	{
		Config = modInstance.GetConfiguration();
		Init();
	}
#endif
}