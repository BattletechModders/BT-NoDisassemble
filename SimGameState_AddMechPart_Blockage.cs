using BattleTech;
using BattleTech.UI;
using Harmony;
using TMPro;

namespace NoDisassemble
{
    [HarmonyPatch(typeof(SimGameState), "AddMechPart", MethodType.Normal)]
    static class SimGameState_AddMechPart_Blockage
    {
        static bool Prefix(string id, SimGameState __instance)
        {
            __instance.AddItemStat(id, "MECHPART", false);
            return false;
        }
    }

    [HarmonyPatch(typeof(MechBayPanel), "OnReadyMech", MethodType.Normal)]
    static class DoThing4
    {
        static void Prefix(MechBayChassisUnitElement chassisElement, MechBayPanel __instance)
        {
            Logger.Debug("we readying!");
            var chassisDef = chassisElement.ChassisDef;
            var itemCount = chassisDef.MechPartCount;
            if (itemCount > 0)
            {
                
                int defaultMechPartMax = this.Constants.Story.DefaultMechPartMax;
                if (itemCount + 1 >= defaultMechPartMax)
                {
                    for (int index = 0; index < defaultMechPartMax - 1; ++index)
                        this.RemoveItemStat(id, "MECHPART", false);
                    MechDef mechDef = new MechDef(this.DataManager.MechDefs.Get(id), this.GenerateSimGameUID(), this.Constants.Salvage.EquipMechOnSalvage);
                    this.AddMech(0, mechDef, true, false, true, (string) null);
                    this.interruptQueue.DisplayIfAvailable();
                    this.MessageCenter.PublishMessage((MessageCenterMessage) new SimGameMechAddedMessage(mechDef, true));
                }
                Logger.Debug("hey this is where we put our code to do the buildout");
            }
        }
    }

    [HarmonyPatch(typeof(MechBayChassisUnitElement), "SetData", MethodType.Normal)]
    static class DoThing3
    {
        static void Postfix(ChassisDef chassisDef, int partsCount, int partsMax, int chassisQuantity, TextMeshProUGUI ___partsLabelText, TextMeshProUGUI ___partsText)
        {
            if (chassisDef == null) return;
//            Logger.Debug($"chassisDef stuff: {chassisDef.VariantName} {chassisDef.MechPartCount}");
            if (chassisDef.MechPartCount > 0)
            {
                ___partsLabelText.SetText("Parts");
                ___partsText.SetText($"{partsCount} / {partsMax}");
            }
            else if (chassisQuantity > 0)
            {
                ___partsLabelText.SetText("QTY");
                ___partsText.SetText($"{chassisQuantity}");
            }
        }
    }
}