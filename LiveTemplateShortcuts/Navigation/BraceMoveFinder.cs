using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using LiveTemplateShortcuts.Framework.Dump;

namespace LiveTemplateShortcuts.Navigation
{
    public class BraceMoveFinder : IRecursiveElementProcessor
    {
        private readonly int _endOffset;
        private readonly int _startOffset;
        private int _bracesOpen;
        private bool _firstDebugElement;
        private bool _processingStart;

        public BraceMoveFinder(int startOffset, int endOffset)
        {
            _startOffset = startOffset;
            _endOffset = endOffset;
        }

        public ITreeNode BestMatchingBraceElement { get; private set; }
        private ITreeNode LastMatchingBraceElement { get; set; }
        public ITreeNode FoundFirstCaretElement { get; private set; }
        public ITreeNode FoundLastSelectionElement { get; private set; }

        public bool InteriorShouldBeProcessed(ITreeNode element)
        {
            if (CSharpTokenType.STRING_INTERPOLATION[element.NodeType])
            {
                return false;
            }

            var result = DoesSubtreeSpanOffset(element, _startOffset);

            if (result)
            {
                element.Dump($"Children of element at {element.GetNavigationRange().StartOffset.Offset}", false, WriteOutputHelper.Write);
            }
            return result;
        }

        public void ProcessBeforeInterior(ITreeNode element)
        {
            var range = element.GetNavigationRange();
            if (!_processingStart)
            {
                if (range.StartOffset.Offset > _startOffset)
                {
                    // This is the first element after the caret, so the previous one
                    // is our start target.
                    _processingStart = true;
                }
                else
                {
                    // Update while looping through elements
                    FoundFirstCaretElement = element;
                }
            }
            if (!_processingStart)
            {
                return;
            }

            if (!_firstDebugElement)
            {
                _firstDebugElement = true;
                FoundFirstCaretElement.Dump($"Start caret: Element at {range.StartOffset.Offset} -> {range.EndOffset.Offset}", false, WriteOutputHelper.Write);
            }

            // Find a matching brace to move
            if (BestMatchingBraceElement == null)
            {
                if (element.GetTokenType() == CSharpTokenType.LBRACE)
                {
                    element.Dump("CSharpTokenType.LBRACE", false, WriteOutputHelper.Write);
                    _bracesOpen++;
                }
                else if (element.GetTokenType() == CSharpTokenType.RBRACE)
                {
                    _bracesOpen--;
                    LastMatchingBraceElement = element;
                    if (_bracesOpen < 0)
                    {
                        BestMatchingBraceElement = element;
                        element.Dump("CSharpTokenType.RBRACE (best match)", false, WriteOutputHelper.Write);
                    }
                    else
                    {
                        element.Dump("CSharpTokenType.RBRACE", false, WriteOutputHelper.Write);
                    }
                }
            }

            if (range.StartOffset.Offset > _endOffset)
            {
                // This is the first element after the selection, so the previous one
                // is our end target.
                if (BestMatchingBraceElement == null)
                {
                    // Set brace to move to be the last one
                    BestMatchingBraceElement = LastMatchingBraceElement;
                }
                ProcessingIsFinished = true;
                element.Dump($"End selection: Element at {range.StartOffset.Offset} -> {range.EndOffset.Offset}", false, WriteOutputHelper.Write);
            }
            else
            {
                // Update while looping through elements
                FoundLastSelectionElement = element;
            }
        }

        public void ProcessAfterInterior(ITreeNode element) { }

        public bool ProcessingIsFinished { get; set; }

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