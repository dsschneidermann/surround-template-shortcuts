using System;

namespace SurroundTemplateShortcuts.Framework.Dump
{
    public class DumpOutput
    {
        private readonly Lazy<string> _value;
        private readonly Lazy<string> _valueNoLinebreak;
        private readonly Lazy<string> _valueNoPrefix;
        private readonly Lazy<string> _valueNoPrefixNoLinebreak;

        public DumpOutput(string linePrefix, string description, string objectId, string objectType, string dumpResult)
        {
            LinePrefix = linePrefix;
            Description = description;
            ObjectId = objectId;
            ObjectType = objectType;
            DumpResult = dumpResult;

            _value = new Lazy<string>(() => $"{_valueNoLinebreak.Value}\r\n");

            _valueNoPrefix = new Lazy<string>(() => $"{_valueNoPrefixNoLinebreak.Value}\r\n");

            _valueNoLinebreak = new Lazy<string>(() =>
                {
                    var line = $"{LinePrefix} {Description}".TrimEnd();
                    return $"{line} ({ObjectId}:{ObjectType}) {DumpResult}";
                });

            _valueNoPrefixNoLinebreak = new Lazy<string>(() =>
                {
                    var line = $"{Description}".TrimEnd();
                    return $"{line} ({ObjectId}:{ObjectType}) {DumpResult}";
                });
        }

        public string ObjectType { get; }

        public string DumpResult { get; }
        public string Description { get; }
        public string LinePrefix { get; }
        public string ObjectId { get; }
        public string Value => _value.Value;
        public string ValueNoLinebreak => _valueNoLinebreak.Value;
        public string ValueNoPrefix => _valueNoPrefix.Value;
        public string ValueNoPrefixNoLinebreak => _valueNoPrefixNoLinebreak.Value;
    }
}