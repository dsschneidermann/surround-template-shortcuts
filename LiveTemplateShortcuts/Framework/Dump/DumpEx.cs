using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace LiveTemplateShortcuts.Framework.Dump
{
    public static class DumpEx
    {
        [Conditional("DEBUG")]
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public static void Dump(string description, [CallerFilePath] string filepath = null, [CallerLineNumber] int linenumber = -1, [CallerMemberName] string caller = null)
        {
            DumpInternal.Dump(string.Empty, description, null, null, filepath, linenumber, caller);
        }

        [Conditional("DEBUG")]
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public static void Dump(string description, Action<DumpOutput> stringOut, [CallerFilePath] string filepath = null, [CallerLineNumber] int linenumber = -1,
                                [CallerMemberName] string caller = null)
        {
            DumpInternal.Dump(string.Empty, description, null, stringOut, filepath, linenumber, caller);
        }

        [Conditional("DEBUG")]
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public static void Dump(string description, Action<string> stringOut, [CallerFilePath] string filepath = null, [CallerLineNumber] int linenumber = -1,
                                [CallerMemberName] string caller = null)
        {
            DumpInternal.Dump(string.Empty, description, null, output => stringOut(output.Value), filepath, linenumber, caller);
        }
    }
}