using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using LINQPad;

namespace SurroundTemplateShortcuts.Framework.Dump
{
    internal static class DumpInternal
    {
        private static int _makeObjectIdCounter;
        private static readonly ConditionalWeakTable<object, string> ConditionalWeakTable = new ConditionalWeakTable<object, string>();
        private static readonly Regex RegexFilenameIllegalChars = new Regex($"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()))}]", RegexOptions.Compiled);

        private static readonly Dictionary<Type, string> PrimitiveTypeNames = new Dictionary<Type, string>
            {
                { typeof(bool), "bool" },
                { typeof(byte), "byte" },
                { typeof(char), "char" },
                { typeof(decimal), "decimal" },
                { typeof(double), "double" },
                { typeof(int), "int" },
                { typeof(long), "long" },
                { typeof(object), "object" },
                { typeof(sbyte), "sbyte" },
                { typeof(short), "short" },
                { typeof(string), "string" },
                { typeof(uint), "uint" },
                { typeof(ulong), "ulong" },
                { typeof(ushort), "ushort" },
                { typeof(void), "void" }
            };

        [Conditional("DEBUG")]
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        internal static void Dump<T>(T obj, string description, bool? makeDump, Action<DumpOutput> stringOut, [CallerFilePath] string filepath = null,
                                     [CallerLineNumber] int linenumber = -1, [CallerMemberName] string caller = null)
        {
            var objType = obj?.GetType() ?? typeof(T);

            if (stringOut == null)
            {
                stringOut = output => Debug.Write(output.Value);
            }

            var linePrefix = $"{Path.GetFileName(filepath)}({linenumber}):";

            if (obj != null &&
                objType.IsEnum)
            {
                var enumDump = $"{objType.Name}.{Enum.GetName(objType, obj)}";
                stringOut(new DumpOutput(linePrefix, description, string.Empty, objType.Name, enumDump));
                return;
            }

            var objectIsSimple = obj == null || obj is string || objType.IsPrimitive;
            if (objectIsSimple)
            {
                var simpleDump = obj?.ToString();
                stringOut(new DumpOutput(linePrefix, description, string.Empty, PrimitiveTypeNames[objType], simpleDump));
                return;
            }
            
            var objectId = GetObjectId(obj, true);
            var objectType = GetTypeNameExpanded(objType);

            var filename = string.Empty;
            try
            {
                if (makeDump != false)
                {
                    string strHtml;
                    using (var writer = Util.CreateXhtmlWriter(true))
                    {
                        writer.Write(obj);
                        strHtml = writer.ToString();
                    }
                    filename = GetFilenameForType(objType);
                    File.WriteAllText(filename, strHtml);
                }
            }
            catch (Exception ex)
            {
                var descriptionLine = !string.IsNullOrEmpty(description) ? $" '{description.Trim()}'" : string.Empty;
                stringOut(new DumpOutput(linePrefix, $"Exception in Dump{descriptionLine}. {ex.GetType().Name}: {ex.Message}", null, objectType, null));
                return;
            }

            stringOut(new DumpOutput(linePrefix, description, objectId, objectType, string.IsNullOrEmpty(filename) ? string.Empty : $"file:///{filename}"));
        }

        private static string GetFilenameForType(Type type)
        {
            var directory = Path.GetTempPath();
            var random = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            var filename = RegexFilenameIllegalChars.Replace($"{type.Name}_{random}.html", string.Empty);
            return $"{directory}{filename}";
        }

        private static string GetObjectId(object obj, bool makeObjectId)
        {
            if (ConditionalWeakTable.TryGetValue(obj, out string foundObjectId))
            {
                return foundObjectId;
            }
            if (makeObjectId)
            {
                var objectId = $"${Interlocked.Add(ref _makeObjectIdCounter, 1)}";
                ConditionalWeakTable.Add(obj, objectId);
                return objectId;
            }
            return string.Empty;
        }

        private static string GetTypeNameExpanded(Type type)
        {
            if (!type.IsGenericType)
            {
                return type.Name;
            }
            var tArgs = type.GetGenericArguments().Select(GetTypeNameExpanded);
            return $"{type.Name.Split('`')[0]}<{string.Join(",", tArgs)}>";
        }
    }
}