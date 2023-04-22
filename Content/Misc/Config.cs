using System.Reflection;
using System;
using BepInEx.Configuration;

namespace WellRoundedBalance.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ConfigFieldAttribute : Attribute
    {
        public string name;
        public string desc;
        public object defaultValue;

        public ConfigFieldAttribute(string name, object defaultValue) => Init(name, string.Empty, defaultValue);
        public ConfigFieldAttribute(string name, string desc, object defaultValue) => Init(name, desc, defaultValue);
        public void Init(string name, string desc, object defaultValue)
        {
            this.name = name;
            this.desc = desc;
            this.defaultValue = defaultValue;
        }
    }

    public class ConfigManager
    {
        internal static bool ConfigChanged = false;
        internal static bool VersionChanged = false;
        public static void HandleConfigAttributes(Type type, string section, ConfigFile config)
        {
            TypeInfo info = type.GetTypeInfo();

            foreach (FieldInfo field in info.GetFields())
            {
                if (!field.IsStatic) continue;

                Type t = field.FieldType;
                ConfigFieldAttribute configattr = field.GetCustomAttribute<ConfigFieldAttribute>();
                if (configattr == null) continue;

                MethodInfo method = typeof(ConfigFile).GetMethods().Where(x => x.Name == nameof(ConfigFile.Bind)).First();
                method = method.MakeGenericMethod(t);
                ConfigEntryBase val = (ConfigEntryBase)method.Invoke(config, new object[] { new ConfigDefinition(section, configattr.name), configattr.defaultValue, new ConfigDescription(configattr.desc) });
                if (!ConfigEqual(val.DefaultValue, val.BoxedValue))
                {
                    if (VersionChanged) val.BoxedValue = val.DefaultValue;
                    else ConfigChanged = true;
                }
                field.SetValue(null, val.BoxedValue);
            }
        }

        private static bool ConfigEqual(object a, object b)
        {
            if (a.Equals(b)) return true;
            float fa, fb;
            if (float.TryParse(a.ToString(), out fa) && float.TryParse(b.ToString(), out fb) && Mathf.Abs(fa - fb) < 0.0001) return true;

            return false;
        }
    }
}