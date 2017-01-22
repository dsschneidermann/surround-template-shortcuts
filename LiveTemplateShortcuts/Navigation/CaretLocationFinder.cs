using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using LiveTemplateShortcuts.Framework.Dump;

namespace LiveTemplateShortcuts.Navigation
{
    public class CaretLocationFinder : IRecursiveElementProcessor
    {
        private readonly int _caretDocumentOffset;

        public CaretLocationFinder(int caretDocumentOffset)
        {
            _caretDocumentOffset = caretDocumentOffset;
        }

        public ITreeNode FoundElement { get; private set; }

        public bool InteriorShouldBeProcessed(ITreeNode element)
        {
            var offset = element.GetNavigationRange().StartOffset.Offset;
            var result = DoesSubtreeSpanOffset(element, _caretDocumentOffset);

            if (result)
            {
                element.Dump($"Children of element at {offset}", false, WriteOutputHelper.Write);
            }
            return result;
        }

        public void ProcessBeforeInterior(ITreeNode element)
        {
            var range = element.GetNavigationRange();

            if (range.StartOffset.Offset > _caretDocumentOffset)
            {
                // This is the first element with startoffset after the caret, so the previous one
                // is our target.
                ProcessingIsFinished = true;
                return;
            }

            FoundElement = element;
            element.Dump($"Element at {range.StartOffset.Offset} -> {range.EndOffset.Offset}", false, WriteOutputHelper.Write);
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