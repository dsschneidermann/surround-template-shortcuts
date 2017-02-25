using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace SurroundTemplateShortcuts.Navigation
{
    public class NodesFromSelectionFinder : IRecursiveElementProcessor
    {
        private readonly int _endOffset;

        private readonly List<ITreeNode> _selectionNodes = new List<ITreeNode>();
        private readonly int _startOffset;
        private ITreeNode _previousElement;
        private bool _processingStart;

        public NodesFromSelectionFinder(int startOffset, int endOffset)
        {
            _startOffset = startOffset;
            _endOffset = endOffset;
        }

        public IEnumerable<ITreeNode> SelectionNodes
        {
            get { return _selectionNodes.Where(x => x != null); }
        }

        public bool InteriorShouldBeProcessed(ITreeNode element)
        {
            return DoesSubtreeSpanOffset(element, _startOffset);
            //element.Dump($"Children of element at {element.GetNavigationRange().StartOffset.Offset}", false, WriteOutputHelper.Write);
        }

        public void ProcessBeforeInterior(ITreeNode element)
        {
            var range = element.GetNavigationRange();
            if (!_processingStart &&
                range.StartOffset.Offset > _startOffset)
            {
                _processingStart = true;
                _selectionNodes.Add(_previousElement);
            }

            if (_processingStart)
            {
                _selectionNodes.Add(element);
                if (range.StartOffset.Offset > _endOffset)
                {
                    ProcessingIsFinished = true;
                    return;
                }
            }

            // Update while looping through elements
            _previousElement = element;
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