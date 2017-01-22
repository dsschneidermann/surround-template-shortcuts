using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace LiveTemplateShortcuts.Framework.Dump
{
    public static class DumpExtensions
    {
        [Conditional("DEBUG")]
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public static void Dump<T>(this T obj, Action<DumpOutput> stringOut = null, [CallerFilePath] string filepath = null, [CallerLineNumber] int linenumber = -1,
                                   [CallerMemberName] string caller = null)
        {
            DumpInternal.Dump(obj, null, null, stringOut, filepath, linenumber, caller);
        }

        [Conditional("DEBUG")]
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public static void Dump<T>(this T obj, string description, Action<DumpOutput> stringOut = null, [CallerFilePath] string filepath = null,
                                   [CallerLineNumber] int linenumber = -1, [CallerMemberName] string caller = null)
        {
            DumpInternal.Dump(obj, description, null, stringOut, filepath, linenumber, caller);
        }

        [Conditional("DEBUG")]
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public static void Dump<T>(this T obj, string description, Action<string> stringOut, [CallerFilePath] string filepath = null, [CallerLineNumber] int linenumber = -1,
                                   [CallerMemberName] string caller = null)
        {
            DumpInternal.Dump(obj, description, null, output => stringOut(output.Value), filepath, linenumber, caller);
        }

        [Conditional("DEBUG")]
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public static void Dump<T>(this T obj, Action<string> stringOut, [CallerFilePath] string filepath = null, [CallerLineNumber] int linenumber = -1,
                                   [CallerMemberName] string caller = null)
        {
            DumpInternal.Dump(obj, null, null, output => stringOut(output.Value), filepath, linenumber, caller);
        }

        [Conditional("DEBUG")]
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public static void Dump<T>(this T obj, string description, bool makeDump, Action<string> stringOut, [CallerFilePath] string filepath = null,
                                   [CallerLineNumber] int linenumber = -1, [CallerMemberName] string caller = null)
        {
            DumpInternal.Dump(obj, description, makeDump, output => stringOut(output.Value), filepath, linenumber, caller);
        }

        [Conditional("DEBUG")]
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public static void Dump<T>(this T obj, string description, bool makeDump, Action<DumpOutput> stringOut = null, [CallerFilePath] string filepath = null,
                                   [CallerLineNumber] int linenumber = -1, [CallerMemberName] string caller = null)
        {
            DumpInternal.Dump(obj, description, makeDump, stringOut, filepath, linenumber, caller);
        }
    }
}