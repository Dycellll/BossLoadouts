using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace BossLoadouts.Common.Systems
{
    public static class ModCompatibility
    {
        // CALAMITY MOD
        public static Mod calamityMod = null;
        public static bool IsCalamityLoaded => calamityMod != null;

        private static Type calamityPlayerType;
        private static PropertyInfo calamityPlayerProperty;

        /// <summary>
        /// Returns the CalamityMod ModPlayer instance attached to the given Player,
        /// or null if CalamityMod is not loaded or something fails.
        /// </summary>
        public static dynamic GetCalamityModPlayer(Player player)
        {
            if (!IsCalamityLoaded)
                return null;

            if (calamityPlayerType == null)
            {
                // Find CalamityModPlayer type by name
                calamityPlayerType = calamityMod.GetType().Assembly.GetType("CalamityMod.CalPlayer");
                if (calamityPlayerType == null)
                {
                    // Could not find CalPlayer class in CalamityMod
                    return null;
                }
            }

            // Get the ModPlayer instance of this type from the player
            // Player.GetModPlayer<T>() requires the type at compile time, so we use Player.GetModPlayer(Type) overload via reflection
            var getModPlayerMethod = typeof(Player).GetMethod("GetModPlayer", new Type[] { typeof(Type) });
            if (getModPlayerMethod == null)
            {
                // Fallback if method missing
                return null;
            }

            var calamityPlayerInstance = getModPlayerMethod.Invoke(player, new object[] { calamityPlayerType });
            return calamityPlayerInstance;
        }

        /// <summary>
        /// Helper to get boolean fields/properties from CalamityModPlayer dynamically by name.
        /// </summary>
        public static bool GetBool(dynamic calamityPlayerInstance, string fieldName)
        {
            if (calamityPlayerInstance == null)
                return false;

            var type = calamityPlayerInstance.GetType();
            // First try property
            var prop = type.GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
            if (prop != null && prop.PropertyType == typeof(bool))
            {
                return (bool)prop.GetValue(calamityPlayerInstance);
            }
            // Then try field
            var field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
            if (field != null && field.FieldType == typeof(bool))
            {
                return (bool)field.GetValue(calamityPlayerInstance);
            }

            return false;
        }

        /// <summary>
        /// Helper to set boolean fields/properties on CalamityModPlayer dynamically by name.
        /// </summary>
        public static void SetBool(dynamic calamityPlayerInstance, string fieldName, bool value)
        {
            if (calamityPlayerInstance == null)
                return;

            var type = calamityPlayerInstance.GetType();
            var prop = type.GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
            if (prop != null && prop.PropertyType == typeof(bool) && prop.CanWrite)
            {
                prop.SetValue(calamityPlayerInstance, value);
                return;
            }

            var field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
            if (field != null && field.FieldType == typeof(bool))
            {
                field.SetValue(calamityPlayerInstance, value);
            }
        }

        // FARGO'S SOULS MOD
        public static Mod fargoMod = null;
        public static bool IsFargoLoaded => fargoMod != null;
    }
}
