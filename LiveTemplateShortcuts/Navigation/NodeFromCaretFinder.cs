using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using LiveTemplateShortcuts.Framework.Dump;

namespace LiveTemplateShortcuts.Navigation
{
    public class NodeFromCaretFinder : IRecursiveElementProcessor
    {
        private readonly int _caretOffset;
        private ITreeNode _previousElement;

        public NodeFromCaretFinder(int caretOffset)
        {
            _caretOffset = caretOffset;
        }

        public ITreeNode CaretNode { get; private set; }

        public bool InteriorShouldBeProcessed(ITreeNode element)
        {
            return DoesSubtreeSpanOffset(element, _caretOffset);
        }

        public void ProcessBeforeInterior(ITreeNode element)
        {
            var range = element.GetNavigationRange();

            if (range.StartOffset.Offset > _caretOffset)
            {
                // This is the first element with startoffset after the caret, so the previous one
                // is our target.
                CaretNode = _previousElement;
                ProcessingIsFinished = true;
                return;
            }

            _previousElement = element;
        }

        public void ProcessAfterInterior(ITreeNode element) { }

        public bool ProcessingIsFinished { get; private set; }

        private static bool DoesSubtreeSpanOffset(ITreeNode element, int offset)
        {
            while (true)
            {
                if (element.LastChild != null)
                {
                    element = element.LastChild;
                    continue;
                }

                var endOffset = element.GetNavigationRange().EndOffset.Offset;
                return endOffset > offset;
            }
        }
    }
}