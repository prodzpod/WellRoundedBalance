﻿using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;
using System;
using MonoMod.Cil;

namespace WellRoundedBalance.Items
{
    public abstract class ItemBase : SharedBase
    {
        public virtual ItemDef InternalPickup { get; }
        public abstract string PickupText { get; }
        public abstract string DescText { get; }
        public override ConfigFile Config => Main.WRBItemConfig;

        public static event Action onTokenRegister;

        public static List<string> itemList = new();

        [ConfigField("Global Proc Chance", "This is a multiplier to all items that can proc something themselves, not necessarily items like Ignition Tank for example. There are special cases where this doesn't work - Sticky Bomb (as it has a Proc Coefficient of 0 by default), Shuriken (most people like it proccing even factoring Railgunner), Kjaro's Band, Runald's Band, Singularity Band, Visions of Heresy and Hooks of Heresy. This also does not include equipment.", 0.2f)]
        public static float globalProc;

        public static int GetItemLoc(ILCursor c, string item) // modify this on compat update
        {
            int ret = -1;
            if (c.TryGotoNext(x => x.MatchLdsfld(typeof(RoR2Content.Items), item), x => x.MatchCallOrCallvirt<Inventory>(nameof(Inventory.GetItemCount)), x => x.MatchStloc(out ret))) c.Index--;
            else if (c.TryGotoNext(x => x.MatchLdsfld(typeof(DLC1Content.Items), item), x => x.MatchCallOrCallvirt<Inventory>(nameof(Inventory.GetItemCount)), x => x.MatchStloc(out ret))) c.Index--;
            return ret;
        }

        public T ConfigOption<T>(T value, string name, string desc)
        {
            ConfigEntry<T> entry = Main.WRBConfig.Bind(Name, name, value, desc);
            if (typeof(T) == typeof(int)) ModSettingsManager.AddOption(new IntSliderOption(entry as ConfigEntry<int>));
            else if (typeof(T) == typeof(float)) ModSettingsManager.AddOption(new SliderOption(entry as ConfigEntry<float>));
            else if (typeof(T) == typeof(string)) ModSettingsManager.AddOption(new StringInputFieldOption(entry as ConfigEntry<string>));
            else if (typeof(T) == typeof(Enum)) ModSettingsManager.AddOption(new ChoiceOption(entry));
            return entry.Value;
        }

        public override void Init()
        {
            base.Init();
            onTokenRegister += SetToken;
            itemList.Add(Name);
        }

        [SystemInitializer(typeof(ItemCatalog))]
        public static void OnItemInitialized()
        { if (onTokenRegister != null) onTokenRegister(); }

        public void SetToken()
        {
            if (InternalPickup != null)
            {
                InternalPickup.pickupToken += "_WRB";
                InternalPickup.descriptionToken += "_WRB";
                LanguageAPI.Add(InternalPickup.pickupToken, PickupText);
                LanguageAPI.Add(InternalPickup.descriptionToken, DescText);
            };
        }

        public string GetToken(string addressablePath)
        {
            ItemDef def = Addressables.LoadAssetAsync<ItemDef>(addressablePath).WaitForCompletion();
            string token = def.nameToken;
            token = token.Replace("ITEM_", string.Empty);
            token = token.Replace("_NAME", string.Empty);
            return token;
        }
    }
}