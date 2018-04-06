﻿using Jint;
using Jint.Native;
using Jint.Native.Object;
using Jint.Runtime;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Interop;
using Oxide.Core.Libraries;
using System;
using System.Reflection;

namespace Oxide.Core.JavaScript
{
    /// <summary>
    /// Wraps a CLR instance
    /// </summary>
    public sealed class LibraryWrapper : ObjectInstance, IObjectWrapper
    {
        public Object Target { get; set; }

        public LibraryWrapper(Engine engine, Object obj) : base(engine)
        {
            Target = obj;
        }

        public override void Put(string propertyName, JsValue value, bool throwOnError)
        {
            if (!CanPut(propertyName))
            {
                if (throwOnError)
                {
                    throw new JavaScriptException(Engine.TypeError);
                }

                return;
            }

            PropertyDescriptor ownDesc = GetOwnProperty(propertyName);

            if (ownDesc == null)
            {
                if (throwOnError)
                {
                    throw new JavaScriptException(Engine.TypeError, "Unknown member: " + propertyName);
                }

                return;
            }

            ownDesc.Value = value;
        }

        public override PropertyDescriptor GetOwnProperty(string propertyName)
        {
            PropertyDescriptor x;
            if (Properties.TryGetValue(propertyName, out x))
            {
                return x;
            }

            Library library = (Library)Target;
            MethodInfo method = library.GetFunction(propertyName);

            if (method != null)
            {
                PropertyDescriptor descriptor = new PropertyDescriptor(new MethodInfoFunctionInstance(Engine, new[] { method }), false, true, false);
                Properties.Add(propertyName, descriptor);
                return descriptor;
            }

            return PropertyDescriptor.Undefined;
        }
    }
}
