using System.Collections.Generic;
using JetBrains.ReSharper.Psi.Tree;

namespace SurroundTemplateShortcuts.Tests
{
    public interface ITreeNodeWithChildren : ITreeNode
    {
        List<ITreeNode> Children { get; set; }
    }
}