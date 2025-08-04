using HarmonyLib;
using Il2CppScheduleOne.UI.Items;
using System.Reflection.Emit;

namespace Lithium.Modules.StackSizes.Patches
{
    [HarmonyPatch(typeof(ItemUIManager), nameof(ItemUIManager.UpdateCashDragAmount))]
    [HarmonyPatch(typeof(ItemUIManager), nameof(ItemUIManager.StartDragCash))]
    [HarmonyPatch(typeof(ItemUIManager), nameof(ItemUIManager.EndCashDrag))]
    public class CashDragPatch
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> TranspilerPatch(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_R4 && (float)instruction.operand == 1000f)
                    yield return new(OpCodes.Ldc_R4, (float)5000);
                else
                    yield return instruction;
            }
        }
    }
}
