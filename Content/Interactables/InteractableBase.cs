﻿using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace WellRoundedBalance.Interactables
{
    public abstract class InteractableBase
    {
        public abstract string Name { get; }
        public virtual bool isEnabled { get; } = true;

        public abstract void Hooks();

        public virtual void Init()
        {
            ConfigManager.HandleConfigAttributes(this.GetType(), Name, Main.WRBInteractableConfig);
            Hooks();
        }
    }
}