﻿using Jint;
using Jint.Native;
using Jint.Native.Array;
using Jint.Native.Object;
using Jint.Runtime.Descriptors;
using Oxide.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Oxide.Core.JavaScript
{
    /// <summary>
    /// Contains extension and utility methods
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Copies and translates the contents of the specified table into the specified config file
        /// </summary>
        /// <param name="config"></param>
        /// <param name="objectInstance"></param>
        public static void SetConfigFromObject(DynamicConfigFile config, ObjectInstance objectInstance)
        {
            config.Clear();
            foreach (KeyValuePair<string, PropertyDescriptor> property in objectInstance.GetOwnProperties())
            {
                object value = property.Value.Value?.ToObject();
                if (value != null)
                {
                    config[property.Key] = value;
                }
            }
        }

        /// <summary>
        /// Copies and translates the contents of the specified config file into the specified object
        /// </summary>
        /// <param name="config"></param>
        /// <param name="engine"></param>
        /// <returns></returns>
        public static ObjectInstance ObjectFromConfig(DynamicConfigFile config, Engine engine)
        {
            ObjectInstance objInst = new ObjectInstance(engine) { Extensible = true };
            foreach (KeyValuePair<string, object> pair in config)
            {
                objInst.FastAddProperty(pair.Key, JsValueFromObject(pair.Value, engine), true, true, true);
            }

            return objInst;
        }

        public static JsValue JsValueFromObject(object obj, Engine engine)
        {
            List<object> values = obj as List<object>;
            if (values != null)
            {
                ArrayInstance array = (ArrayInstance)engine.Array.Construct(values.Select(v => JsValueFromObject(v, engine)).ToArray());
                array.Extensible = true;
                return array;
            }

            Dictionary<string, object> dict = obj as Dictionary<string, object>;
            if (dict != null)
            {
                ObjectInstance objInst = new ObjectInstance(engine) { Extensible = true };
                foreach (KeyValuePair<string, object> pair in dict)
                {
                    objInst.FastAddProperty(pair.Key, JsValueFromObject(pair.Value, engine), true, true, true);
                }

                return objInst;
            }

            return JsValue.FromObject(engine, obj);
        }

        /// <summary>
        /// Gets the namespace of the specified type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetNamespace(Type type) => type.Namespace ?? string.Empty;
    }
}
