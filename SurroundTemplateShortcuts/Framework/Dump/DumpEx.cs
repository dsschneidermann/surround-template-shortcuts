using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace SurroundTemplateShortcuts.Framework.Dump
{
    public static class DumpEx
    {
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public static void Dump(string description, [CallerFilePath] string filepath = null, [CallerLineNumber] int linenumber = -1, [CallerMemberName] string caller = null)
        {
            DumpInternal.Dump(string.Empty, description, null, null, filepath, linenumber, caller);
        }
        
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public static void Dump(string description, Action<DumpOutput> stringOut, [CallerFilePath] string filepath = null, [CallerLineNumber] int linenumber = -1,
                                [CallerMemberName] string caller = null)
        {
            DumpInternal.Dump(string.Empty, description, null, stringOut, filepath, linenumber, caller);
        }
        
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public static void Dump(string description, Action<string> stringOut, [CallerFilePath] string filepath = null, [CallerLineNumber] int linenumber = -1,
                                [CallerMemberName] string caller = null)
        {
            DumpInternal.Dump(string.Empty, description, null, output => stringOut(output.Value), filepath, linenumber, caller);
        }
    }
}