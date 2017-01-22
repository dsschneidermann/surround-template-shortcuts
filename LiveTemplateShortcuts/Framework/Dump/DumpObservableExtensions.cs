//using System;
//using System.Diagnostics.CodeAnalysis;
//using System.Reactive.Linq;
//using System.Runtime.CompilerServices;

//namespace WpfRandomFile.Framework.Dump
//{
//    public static class DumpObservableExtensions
//    {
//        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
//        internal static IObservable<T> Dump_Internal<T>(this IObservable<T> obs, string description, bool? makeDump, Action<DumpOutput> stringOut,
//                                                        [CallerFilePath] string filepath = null, [CallerLineNumber] int linenumber = -1, [CallerMemberName] string caller = null)
//        {
//            return obs.Do(x => DumpInternal.Dump(x, description, makeDump, stringOut, filepath, linenumber, caller));
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
//        public static IObservable<T> DumpObs<T>(this IObservable<T> obs, string description, bool makeDump, Action<DumpOutput> stringOut, [CallerFilePath] string filepath = null,
//                                                 [CallerLineNumber] int linenumber = -1, [CallerMemberName] string caller = null)
//        {
//#if DEBUG
//            return obs.Dump_Internal(description, makeDump, stringOut, filepath, linenumber, caller);
//#else
//            return obs;
//#endif
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
//        public static IObservable<T> DumpObs<T>(this IObservable<T> obs, string description = null, [CallerFilePath] string filepath = null, [CallerLineNumber] int linenumber = -1,
//                                             [CallerMemberName] string caller = null)
//        {
//#if DEBUG
//            return obs.Dump_Internal(description, null, null, filepath, linenumber, caller);
//#else
//            return obs;
//#endif
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
//        public static IObservable<T> DumpObs<T>(this IObservable<T> obs, string description, Action<DumpOutput> stringOut, [CallerFilePath] string filepath = null,
//                                             [CallerLineNumber] int linenumber = -1, [CallerMemberName] string caller = null)
//        {
//#if DEBUG
//            return obs.Dump_Internal(description, null, stringOut, filepath, linenumber, caller);
//#else
//            return obs;
//#endif
//        }

//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
//        public static IObservable<T> DumpObs<T>(this IObservable<T> obs, string description, bool makeDump, [CallerFilePath] string filepath = null,
//                                             [CallerLineNumber] int linenumber = -1, [CallerMemberName] string caller = null)
//        {
//#if DEBUG
//            return obs.Dump_Internal(description, makeDump, null, filepath, linenumber, caller);
//#else
//            return obs;
//#endif
//        }
//    }
//}

