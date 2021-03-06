﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NET.Popo
{
    public class PopoObjectPool
    {
        private static ConcurrentDictionary<long, PopoObject> objects = new ConcurrentDictionary<long, PopoObject>();
        private static ConcurrentDictionary<Type, Queue<PopoObject>> typeStorage = new ConcurrentDictionary<Type, Queue<PopoObject>>();

        public static PopoObject Fetch(Type type)
        {
            if (!typeStorage.TryGetValue(type, out Queue<PopoObject> queue))
            {
                typeStorage[type] = new Queue<PopoObject>();
            }
            PopoObject value;
            if (queue.Count > 0)
            {
                value = queue.Dequeue();
            }
            else
            {
                value = (PopoObject)Activator.CreateInstance(type);
            }
            value.ObjectId = GlobalId.CreateId();
            objects[value.ObjectId] = value;
            return value;
        }

        public static PopoObject Fetch(Type type, params object[] parameters)
        {
            // var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Static |
            //BindingFlags.NonPublic | BindingFlags.Instance);
            // foreach (var constructor in constructors)
            // {
            //     var parameters = constructor.GetParameters();

            // }

            if (!typeStorage.TryGetValue(type, out Queue<PopoObject> queue))
            {
                typeStorage[type] = new Queue<PopoObject>();
            }
            PopoObject value;
            if (queue.Count > 0)
            {
                value = queue.Dequeue();
            }
            else
            {
                value = (PopoObject)Activator.CreateInstance(type, parameters);
            }
            value.ObjectId = GlobalId.CreateId();
            objects[value.ObjectId] = value;
            return value;
        }


        public static void Push(PopoObject popoObject)
        {
            objects.TryRemove(popoObject.ObjectId, out popoObject);
            if (!typeStorage.TryGetValue(popoObject.GetType(), out Queue<PopoObject> queue))
            {
                queue = new Queue<PopoObject>();
            }
            popoObject.ObjectId = 0;
            queue.Enqueue(popoObject);
        }


        public static void Load()
        {
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.dll");
            var assemblys = files.ToList().Select((f) => Assembly.LoadFrom(f));
            var typesList = assemblys.Select((a) => a.GetTypes().Distinct());

            foreach(var types in typesList)
            {
                foreach(var type in types)
                {
                    if (IsPopoType(type))
                    {
                        typeStorage[type] = new Queue<PopoObject>();
                    }
                }
            }
        }


        public static bool IsPopoType(Type type)
        {
            Type currentType = type;
            var baseType = typeof(PopoObject);
            while (true)
            {
                if(currentType.BaseType == baseType)
                {
                    return true;
                }
                else if(currentType.BaseType == null)
                {
                    return false;
                }
                currentType = currentType.BaseType;
            }
        }
    }
}
