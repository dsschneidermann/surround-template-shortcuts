using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Psi.Tree;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SurroundTemplateShortcuts.Framework.Stack;
using SurroundTemplateShortcuts.Navigation;

namespace SurroundTemplateShortcuts.Tests
{
    [TestClass]
    public class NodeFromCaretFinderTests
    {
        [TestMethod]
        public void CanRecurseParents()
        {
            var treeNode =
                CreateTreeNode("root", 10, 200)
                    .CreateChild("first", 10, 20)
                    .CreateSibling("second", 21, 200)
                    .CreateChild("third", 21, 41)
                    .CreateSibling("fourth", 42, 200);

            var recursiveList = treeNode.GetRecursive(x => x.Parent).ToList();

            Assert.AreEqual(3, recursiveList.Count);
            Assert.AreEqual("fourth", recursiveList[0].NodeType.ToString());
            Assert.AreEqual("second", recursiveList[1].NodeType.ToString());
            Assert.AreEqual("root", recursiveList[2].NodeType.ToString());
        }

        [TestMethod]
        public void CanRecurseChildren()
        {
            var treeNode =
                CreateTreeNode("root", 10, 200)
                    .CreateChild("first", 10, 20)
                    .CreateSibling("second", 21, 200)
                    .CreateChild("third", 21, 41)
                    .CreateSibling("fourth", 42, 200);

            var rootNode = treeNode.GetRecursive(x => x.Parent).Last();
            var recursiveList = rootNode.GetRecursive(x => x.Children()).ToList();

            Assert.AreEqual(5, recursiveList.Count);
            Assert.AreEqual("root", recursiveList[0].NodeType.ToString());
            Assert.AreEqual("first", recursiveList[1].NodeType.ToString());
            Assert.AreEqual("second", recursiveList[2].NodeType.ToString());
            Assert.AreEqual("third", recursiveList[3].NodeType.ToString());
            Assert.AreEqual("fourth", recursiveList[4].NodeType.ToString());
        }

        [TestMethod]
        public void SelectsNodeWhenMiddleAtCaret()
        {
            var treeNode =
                CreateTreeNode("root", 10, 200)
                    .CreateChild("first", 10, 20)
                    .CreateSibling("second", 21, 200)
                    .CreateChild("third", 21, 41)
                    .CreateSibling("fourth", 42, 200);

            var result = FindFromCaret(15, treeNode);

            Assert.IsNotNull(result, "NodeAtCaret was null");
            Assert.AreEqual("first", result.NodeType.ToString());
        }

        [TestMethod]
        public void SelectsNodeWhenEndAtCaret()
        {
            var treeNode =
                CreateTreeNode("root", 10, 200)
                    .CreateChild("first", 10, 20)
                    .CreateSibling("second", 21, 200)
                    .CreateChild("third", 21, 41)
                    .CreateSibling("fourth", 42, 200);

            var result = FindFromCaret(20, treeNode);

            Assert.IsNotNull(result, "NodeAtCaret was null");
            Assert.AreEqual("first", result.NodeType.ToString());
        }

        [TestMethod]
        public void SelectsNodeWhenPastStartAtCaret()
        {
            var treeNode =
                CreateTreeNode("root", 10, 200)
                    .CreateChild("first", 10, 20)
                    .CreateSibling("second", 21, 200)
                    .CreateChild("third", 21, 41)
                    .CreateSibling("fourth", 42, 200);

            var result = FindFromCaret(22, treeNode);

            Assert.IsNotNull(result, "NodeAtCaret was null");
            Assert.AreEqual("third", result.NodeType.ToString());
        }

        [TestMethod]
        public void SelectsEndingNode()
        {
            var treeNode =
                CreateTreeNode("root", 10, 200)
                    .CreateChild("first", 10, 20)
                    .CreateSibling("second", 21, 200)
                    .CreateChild("third", 21, 41)
                    .CreateSibling("fourth", 42, 200);

            var result = FindFromCaret(100, treeNode);

            Assert.IsNotNull(result, "NodeAtCaret was null");
            Assert.AreEqual("fourth", result.NodeType.ToString());
        }

        [TestMethod]
        public void SelectsNothingWhenCaretOutOfBounds()
        {
            var treeNode =
                CreateTreeNode("root", 10, 200)
                    .CreateChild("first", 10, 20)
                    .CreateSibling("second", 21, 200)
                    .CreateChild("third", 21, 41)
                    .CreateSibling("fourth", 42, 200);

            var result = FindFromCaret(210, treeNode);

            Assert.IsNull(result, "NodeAtCaret was not null");
        }

        [TestMethod]
        public void SkipsProcessInteriorWhenOutside()
        {
            var treeNode =
                CreateTreeNode("root", 10, 200)
                    .CreateChild("first", 10, 20)
                    .CreateSibling("second", 21, 200)
                    .CreateChild("third", 21, 41)
                    .CreateSibling("fourth", 42, 200);

            var result = GetInteriorProcessed(30, treeNode);

            CollectionAssert.DoesNotContain(result, "first", "Node children were processed");
        }

        [TestMethod]
        public void ShouldProcessInteriorWhenAtEdge()
        {
            var treeNode =
                CreateTreeNode("root", 10, 200)
                    .CreateChild("first", 10, 20)
                    .CreateSibling("second", 21, 200)
                    .CreateChild("third", 21, 41)
                    .CreateSibling("fourth", 42, 200);

            var result = GetInteriorProcessed(20, treeNode);

            CollectionAssert.Contains(result, "first", "Node first children were not processed");
            CollectionAssert.Contains(result, "second", "Node second children were not processed");
        }

        private ITreeNode FindFromCaret(int caretOffset, ITreeNode treeNode)
        {
            var nodeFromCaretFinder = new NodeFromCaretFinder(caretOffset);

            var rootNode = treeNode.GetRecursive(x => x.Parent).Last();

            foreach (var node in rootNode.GetRecursive(x => x.Children()))
            {
                nodeFromCaretFinder.ProcessBeforeInterior(node);

                if (nodeFromCaretFinder.ProcessingIsFinished)
                    break;
            }

            return nodeFromCaretFinder.NodeAtCaret;
        }

        private List<string> GetInteriorProcessed(int caretOffset, ITreeNode treeNode)
        {
            var nodeFromCaretFinder = new NodeFromCaretFinder(caretOffset);

            var rootNode = treeNode.GetRecursive(x => x.Parent).Last();
            return rootNode.GetRecursive(x => x.Children())
                .Where(x => nodeFromCaretFinder.InteriorShouldBeProcessed(x))
                .Select(x => x.NodeType.ToString())
                .ToList();
        }

        private ITreeNode CreateTreeNode(string nodeTypeString, int startOffset, int endOffset)
        {
            return TreeNodeMockExtensions.CreateTreeNode(null, nodeTypeString, startOffset, endOffset);
        }
    }
}