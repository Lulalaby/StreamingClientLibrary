using System;
using System.Collections.Generic;

namespace StreamingClient.Base.Util
{
    /// <summary>
    /// Helper class for interacting with enums.
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Gets the public-facing name of the specified enum from the Name Attribute.
        /// </summary>
        /// <typeparam name="T">The type of enum value</typeparam>
        /// <param name="value">The enum value</param>
        /// <returns>The public-facing name of the specified enum</returns>
        public static string GetEnumName<T>(T value)
        {
            string name = Enum.GetName(typeof(T), value);
            if (!string.IsNullOrEmpty(name))
            {
                NameAttribute[] nameAttributes = (NameAttribute[])typeof(T).GetField(name).GetCustomAttributes(typeof(NameAttribute), false);
                return nameAttributes != null && nameAttributes.Length > 0 ? nameAttributes[0].Name : name;
            }
            return null;
        }

        /// <summary>
        /// Gets the public-facing name of the specified enum from the Name Attribute.
        /// </summary>
        /// <param name="type">The enum type</param>
        /// <param name="value">The enum value</param>
        /// <returns>The public-facing name of the specified enum</returns>
        public static string GetEnumName(Type type, object value)
        {
            string name = Enum.GetName(type, value);
            if (!string.IsNullOrEmpty(name))
            {
                NameAttribute[] nameAttributes = (NameAttribute[])type.GetField(name).GetCustomAttributes(typeof(NameAttribute), false);
                return nameAttributes != null && nameAttributes.Length > 0 ? nameAttributes[0].Name : name;
            }
            return null;
        }

        /// <summary>
        /// Gets the public-facing name of the specified enums from the Name Attribute.
        /// </summary>
        /// <typeparam name="T">The type of enum value</typeparam>
        /// <param name="list">The enum values</param>
        /// <returns>The public-facing name of the specified enums</returns>
        public static IEnumerable<string> GetEnumNames<T>(IEnumerable<T> list)
        {
            List<string> results = new();
            foreach (T value in list)
            {
                string name = GetEnumName(value);
                if (!string.IsNullOrEmpty(name))
                {
                    results.Add(name);
                }
            }
            return results;
        }

        /// <summary>
        /// Gets the public-facing name of the specified enums from the Name Attribute.
        /// </summary>
        /// <typeparam name="T">The type of enum value</typeparam>
        /// <param name="includeObsoletes">Whether to include obsolete enums</param>
        /// <returns>The public-facing name of the specified enums</returns>
        public static IEnumerable<string> GetEnumNames<T>(bool includeObsoletes = false) => GetEnumNames(GetEnumList<T>(includeObsoletes));

        /// <summary>
        /// Gets a list of all available values for an enum type.
        /// </summary>
        /// <typeparam name="T">The type of enum value</typeparam>
        /// <param name="includeObsoletes">Whether to include obsolete enums</param>
        /// <returns>The list of all available values for the enum type</returns>
        public static IEnumerable<T> GetEnumList<T>(bool includeObsoletes = false)
        {
            List<T> values = new();
            foreach (T value in Enum.GetValues(typeof(T)))
            {
                if (!includeObsoletes)
                {
                    if (IsObsolete(value))
                    {
                        continue;
                    }
                }
                values.Add(value);
            }
            return values;
        }

        /// <summary>
        /// Gets the enum value that matches the specified name.
        /// </summary>
        /// <param name="type">The type to search</param>
        /// <param name="str">The name to search for</param>
        /// <returns>The enum value that matches the specified name</returns>
        public static object GetEnumValueFromString(Type type, string str)
        {
            foreach (object value in Enum.GetValues(type))
            {
                if (string.Equals(str, GetEnumName(type, value), StringComparison.CurrentCultureIgnoreCase))
                {
                    return value;
                }
            }
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        /// Gets the enum value that matches the specified name.
        /// </summary>
        /// <typeparam name="T">The type of enum value</typeparam>
        /// <param name="str">The name to search for</param>
        /// <returns>The enum value that matches the specified name</returns>
        public static T GetEnumValueFromString<T>(string str)
        {
            foreach (T value in Enum.GetValues(typeof(T)))
            {
                if (string.Equals(str, GetEnumName(value), StringComparison.CurrentCultureIgnoreCase))
                {
                    return value;
                }
            }
            return default;
        }

        /// <summary>
        /// Indicates whether an enum value is obsolete.
        /// </summary>
        /// <typeparam name="T">The type of enum value</typeparam>
        /// <param name="value">The enum value</param>
        /// <returns>Whether it is obsolete</returns>
        public static bool IsObsolete<T>(T value)
        {
            ObsoleteAttribute[] attributes = (ObsoleteAttribute[])value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(ObsoleteAttribute), false);
            return attributes != null && attributes.Length > 0;
        }
    }
}
