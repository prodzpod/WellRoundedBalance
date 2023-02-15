using System.Reflection;
using System.Collections;
using System;
using System.Linq;
using BepInEx.Configuration;
using HG.Reflection;

namespace WellRoundedBalance.Attributes {
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ConfigFieldAttribute : Attribute {
        public string name;
        public string desc;
        public object defaultValue;

        public ConfigFieldAttribute(string name, string desc, object defaultValue) {
            this.name = name;
            this.desc = desc;
            this.defaultValue = defaultValue;
        }
    }

    public class ConfigManager {
        public static void HandleConfigAttributes(Type type, string section, ConfigFile config) {
            TypeInfo info = type.GetTypeInfo();

            foreach (FieldInfo field in info.GetFields()) {
                if (!field.IsStatic) {
                    continue;
                }

                Type t = field.FieldType;

                ConfigFieldAttribute configattr = field.GetCustomAttribute<ConfigFieldAttribute>();
                if (configattr == null) {
                    continue;
                }

                MethodInfo method = typeof(ConfigFile).GetMethods().Where(x => x.Name == nameof(ConfigFile.Bind)).First();
                method = method.MakeGenericMethod(t);
                ConfigEntryBase val = (ConfigEntryBase)method.Invoke(config, new object[] { new ConfigDefinition(section, configattr.name), configattr.defaultValue, new ConfigDescription(configattr.desc)});

                field.SetValue(null, val.BoxedValue);
            }
        }
    }
}