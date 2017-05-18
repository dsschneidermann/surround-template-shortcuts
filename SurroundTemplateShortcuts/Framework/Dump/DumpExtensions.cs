using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SurroundTemplateShortcuts.Framework.Dump
{
    public static class DumpExtensions
    {
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public static void Dump<T>(this T obj, Action<DumpOutput> stringOut = null, [CallerFilePath] string filepath = null, [CallerLineNumber] int linenumber = -1,
                                   [CallerMemberName] string caller = null)
        {
            DumpInternal.Dump(obj, null, null, stringOut, filepath, linenumber, caller);
        }
        
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public static void Dump<T>(this T obj, string description, Action<DumpOutput> stringOut = null, [CallerFilePath] string filepath = null,
                                   [CallerLineNumber] int linenumber = -1, [CallerMemberName] string caller = null)
        {
            DumpInternal.Dump(obj, description, null, stringOut, filepath, linenumber, caller);
        }
        
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public static void Dump<T>(this T obj, string description, Action<string> stringOut, [CallerFilePath] string filepath = null, [CallerLineNumber] int linenumber = -1,
                                   [CallerMemberName] string caller = null)
        {
            DumpInternal.Dump(obj, description, null, output => stringOut(output.Value), filepath, linenumber, caller);
        }
        
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public static void Dump<T>(this T obj, Action<string> stringOut, [CallerFilePath] string filepath = null, [CallerLineNumber] int linenumber = -1,
                                   [CallerMemberName] string caller = null)
        {
            DumpInternal.Dump(obj, null, null, output => stringOut(output.Value), filepath, linenumber, caller);
        }
        
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public static void Dump<T>(this T obj, string description, bool makeDump, Action<string> stringOut, [CallerFilePath] string filepath = null,
                                   [CallerLineNumber] int linenumber = -1, [CallerMemberName] string caller = null)
        {
            DumpInternal.Dump(obj, description, makeDump, output => stringOut(output.Value), filepath, linenumber, caller);
        }
        
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public static void Dump<T>(this T obj, string description, bool makeDump, Action<DumpOutput> stringOut = null, [CallerFilePath] string filepath = null,
                                   [CallerLineNumber] int linenumber = -1, [CallerMemberName] string caller = null)
        {
            DumpInternal.Dump(obj, description, makeDump, stringOut, filepath, linenumber, caller);
        }
    }
}