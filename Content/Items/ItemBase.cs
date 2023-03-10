﻿using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.Options;
using System;
using MonoMod.Cil;

namespace WellRoundedBalance.Items
{
    public abstract class ItemBase : SharedBase
    {
        public virtual string InternalPickupToken { get; }
        public abstract string PickupText { get; }
        public abstract string DescText { get; }
        public override ConfigFile Config => Main.WRBItemConfig;

        public static int GetItemLoc(ILCursor c, string item) // modify this on compat update
        {
            int ret = -1;
            if (c.TryGotoNext(x => x.MatchLdsfld(typeof(RoR2Content.Items), item), x => x.MatchCallOrCallvirt<Inventory>(nameof(Inventory.GetItemCount)), x => x.MatchStloc(out ret))) c.Index--;
            else if (c.TryGotoNext(x => x.MatchLdsfld(typeof(DLC1Content.Items), item), x => x.MatchCallOrCallvirt<Inventory>(nameof(Inventory.GetItemCount)), x => x.MatchStloc(out ret))) c.Index--;
            return ret;
        }

        public T ConfigOption<T>(T value, string name, string desc)
        {
            ConfigEntry<T> entry = Main.WRBConfig.Bind<T>(Name, name, value, desc);
            if (typeof(T) == typeof(int))
            {
                ModSettingsManager.AddOption(new IntSliderOption(entry as ConfigEntry<int>));
            }
            else if (typeof(T) == typeof(float))
            {
                ModSettingsManager.AddOption(new SliderOption(entry as ConfigEntry<float>));
            }
            else if (typeof(T) == typeof(string))
            {
                ModSettingsManager.AddOption(new StringInputFieldOption(entry as ConfigEntry<string>));
            }
            else if (typeof(T) == typeof(Enum))
            {
                ModSettingsManager.AddOption(new ChoiceOption(entry));
            }
            return entry.Value;
        }

        public override void Init()
        {
            base.Init();
            string pickupToken;
            string descriptionToken;

            pickupToken = "ITEM_" + InternalPickupToken.ToUpper() + "_PICKUP";
            descriptionToken = "ITEM_" + InternalPickupToken.ToUpper() + "_DESC";

            LanguageAPI.Add(pickupToken, PickupText);
            LanguageAPI.Add(descriptionToken, DescText);
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