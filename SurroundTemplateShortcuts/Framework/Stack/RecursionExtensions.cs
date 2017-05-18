using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SurroundTemplateShortcuts.Framework.Stack
{
    public static class RecursionExtensions
    {
        public static IEnumerable<T> GetRecursive<T>(this T obj, Func<T, T> selector)
        {
            return new[] { obj }.GetRecursive<T>(element => new[] { selector(element) });
        }

        public static IEnumerable<T> GetRecursive<T>(this T obj, Func<T, IEnumerable> selector)
        {
            return new[] { obj }.GetRecursive<T>(selector);
        }

        public static IEnumerable<T> GetRecursive<T>(this IEnumerable<T> collection, Func<T, T> selector)
        {
            return collection.GetRecursive<T>(x => collection.Select(element => selector(element)));
        }

        public static IEnumerable<T> GetRecursive<T>(this IEnumerable collection, Func<T, IEnumerable> selector)
        {
            var stack = new Stack<IEnumerable<T>>();
            stack.Push(collection.OfType<T>());

            while (stack.Count > 0)
            {
                var items = stack.Pop();
                foreach (var item in items)
                {
                    yield return item;

                    var children = selector(item)?.OfType<T>();
                    if (children != null)
                    {
                        stack.Push(children);
                    }
                }
            }
        }
    }
}