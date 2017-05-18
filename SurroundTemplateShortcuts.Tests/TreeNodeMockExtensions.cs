using System.Collections.Generic;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using Moq;

namespace SurroundTemplateShortcuts.Tests
{
    public static class TreeNodeMockExtensions
    {
        private static readonly Mock<IDocument> DocumentMock = new Mock<IDocument>();

        public static List<ITreeNode> Children(this ITreeNode parent)
        {
            return ((ITreeNodeWithChildren) parent).Children;
        }

        public static ITreeNode CreateTreeNode(ITreeNode parent, string nodeTypeString, int startOffset, int endOffset)
        {
            var treeNode = new Mock<ITreeNodeWithChildren>();
            var nodeType = new Mock<NodeType>(string.Empty, 0);

            treeNode.Setup(x => x.Parent)
                .Returns(parent);

            // Create persistent Children collection
            treeNode.SetupProperty(x => x.Children);
            treeNode.Object.Children = new List<ITreeNode>();

            treeNode.Setup(x => x.GetNavigationRange())
                .Returns(new DocumentRange(
                    DocumentMock.Object, new TextRange(startOffset, endOffset)));

            treeNode.Setup(x => x.NodeType)
                .Returns(nodeType.Object);

            nodeType.Setup(x => x.ToString()).Returns(nodeTypeString);

            return treeNode.Object;
        }

        public static ITreeNode CreateChild(this ITreeNode parent, string nodeType, int startOffset, int endOffset)
        {
            var child = CreateTreeNode(parent, nodeType, startOffset, endOffset);
            ((ITreeNodeWithChildren) parent)?.Children.Add(child);
            return child;
        }

        public static ITreeNode CreateSibling(this ITreeNode sibling, string nodeType, int startOffset, int endOffset)
        {
            var child = CreateTreeNode(sibling.Parent, nodeType, startOffset, endOffset);
            ((ITreeNodeWithChildren) sibling.Parent)?.Children.Add(child);
            return child;
        }
    }
}