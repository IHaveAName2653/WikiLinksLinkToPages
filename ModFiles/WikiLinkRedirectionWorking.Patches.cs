using HarmonyLib;
using Elements.Core;
using System;
using System.Collections.Generic;
using FrooxEngine;
using Elements.Assets;

namespace WikiLinks;

[HarmonyPatch]
public static partial class ComplexTypes
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(ReflectionExtensions), "GetBareName")]
	public static void Patch(
		Type type, ref string __result)
	{
		// Disable the mod
		if (!WikiLinks.remap_types) return;
		// This runs the normal check first

		WikiLinks.Msg($"PrePatch {type.Name} -> {__result}");

		if (string.IsNullOrEmpty(__result)) return;

		// Split into its component parts
		__result = __result.Replace("Legacy", "");
		var parts = __result.Split('_', StringSplitOptions.RemoveEmptyEntries);
		int genericCount = 0;
		int end = parts.Length;

		// Handles a case for more then one pudo-generic type
		for (int i = parts.Length - 1; i >= 0; i--)
		{
			if (IsValidGenericType(parts[i]))
			{
				genericCount++;
			}
			else if (genericCount > 1)
			{
				end = i + 1;
				break;
			}
		}

		// Handles the general case
		if (genericCount > 1)
		{
			if (end < parts.Length)
				__result = string.Join("_", parts, 0, end);
		}
		else
		{
			switch (parts.Length)
			{
				case 2:
					// Split Case: Or_Bool -> OR
					__result = parts[0];
					break;

				case 3:
					// Swop case: Example OR_Multi_Bool -> MultiOR
					__result = parts[1] + parts[0];
					break;

				default:
					// If I missed something
					__result = string.Join("_", parts);
					WikiLinks.Msg($"broken {__result}");
					break;
			}
		}

		// Edge Case
		// Object casts already covered as generic type
		if (__result.StartsWith("Cast")) __result = "ValueCast";

		WikiLinks.Msg($"PostPatch {type.Name} -> {__result}");
	}

	private static readonly HashSet<string> GenericTypes = new()
	{
		// Prob a better way to do this
		"bool", "bool2", "bool3", "bool4",
		"byte", "ushort", "ulong", "sbyte", "short",
		"int", "int2","int3","int4",
		"uint", "uint2","uint3","uint4",
		"long","long2","long3","long4",
		"float","float2","float3","float4","floatq","float2x2","float3x3","float4x4",
		"double","double2","double3","double4","doubleq","double2x2","double3x3","double4x4",
		"color","color32",
		"string", "colorx", "char"
	};
	static bool IsValidGenericType(string part) => GenericTypes.Contains(part.ToLower());

	[HarmonyPostfix]
	[HarmonyPatch(typeof(Hyperlink), "AttachForWikiPage")]
	public static void AttachForWikiPagePatch(Slot slot, Type type, Hyperlink __result)
	{
		if (__result == null || slot == null) return;
		slot.Tag = "WikiLink";
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(Hyperlink), "Open")]
	public static bool OpenPatch(Hyperlink __instance, ref bool __result)
	{
		if (!WikiLinks.document_style) return true;
		if (__instance?.Slot?.Tag != "WikiLink") return true;

		Uri uri = new Uri(__instance.URL.Value.ToString());
		string searchUrl = $"{uri.Scheme}://{uri.Host}/api/pdf{uri.AbsolutePath}";

		WikiLinks.Msg($"[WikiLink intercepted] {searchUrl}");

		HelperFunctions.OpenUrl(searchUrl, __instance.Engine);

		return false;
	}
}

public class HelperFunctions
{
	public static void OpenUrl(string path, Engine engine)
	{
		HandleOpenWorldURL([path], engine);
	}
	public static async void HandleOpenWorldURL(List<string> openUrl, Engine engine)
	{
		World targetWorld = engine.WorldManager.FocusedWorld;
		targetWorld.LocalUser.GetPointInFrontOfUser(out var point, out var rotation, float3.Backward);
		UniversalImporter.Import(AssetClass.Document, openUrl, targetWorld, point, rotation);
	}
}