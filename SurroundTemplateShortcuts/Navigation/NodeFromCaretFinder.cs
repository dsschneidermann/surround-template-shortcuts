using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using SurroundTemplateShortcuts.Framework.Dump;
using SurroundTemplateShortcuts.Framework.Stack;

namespace SurroundTemplateShortcuts.Navigation
{
    public class NodeFromCaretFinder : IRecursiveElementProcessor
    {
        private readonly int _caretOffset;

        private static readonly List<string> ParentLanguageElements = new List<string>()
        {
            "OBJECT_CREATION_EXPRESSION",
            "REFERENCE_EXPRESSION"
        };

        public NodeFromCaretFinder(int caretOffset)
        {
            _caretOffset = caretOffset;
        }

        public ITreeNode NodeAtCaret { get; private set; }

        public bool InteriorShouldBeProcessed(ITreeNode element)
        {
            return DoesSubtreeSpanOffset(element, _caretOffset);
        }

        public void ProcessBeforeInterior(ITreeNode element)
        {
            var range = element.GetNavigationRange();

            if (range.StartOffset.Offset >= _caretOffset)
            {
                // This is the first element with startoffset after the caret, so the previous one
                // is our target.
                ProcessingIsFinished = true;
                return;
            }

            NodeAtCaret = element;
            if (range.EndOffset.Offset < _caretOffset)
            {
                // Reset to null if caret is outside element
                NodeAtCaret = null;
            }
        }

        public void ProcessAfterInterior(ITreeNode element)
        {
        }

        public bool ProcessingIsFinished { get; private set; }

        public static ITreeNode GetParentLanguageElementFromNode(ITreeNode element)
        {
            foreach (var output in element.GetRecursive(x => x.Parent))
            {
                output.NodeType.ToString().Dump("NodeType", WriteOutputHelper.Write);
            }

            return element.GetRecursive(x => x.Parent)
                       .FirstOrDefault(node =>
                           ParentLanguageElements.Contains(node.NodeType.ToString()))
                   ?? element;
        }

        private static bool DoesSubtreeSpanOffset(ITreeNode element, int offset)
        {
            return element.GetRecursive(x => x.LastChild).Last()
                       .GetNavigationRange().EndOffset.Offset >= offset;
        }
    }
}