using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Feature.Services.Menu;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl.DataContext;
using JetBrains.UI.ActionsRevised;
using JetBrains.UI.Controls.GotoByName;

namespace LiveTemplateShortcuts.GotoWordSample
{
    [Action(Id, "GotoWordIndex")]
    public class GotoWordIndexAction : IExecutableAction, IInsertLast<ToolsMenu>
    {
        public const string Id = "GotoWordIndex";

        //// File searching stuff
        ////var finder = _provider.PsiServices.Finder;
        ////var references = finder.FindReferences(
        ////  propDecl.DeclaredElement,
        ////  SearchDomainFactory.Instance.CreateSearchDomain(_provider.SourceFile.ToProjectFile()),
        ////  NullProgressIndicator.Instance);
        ////Also interesting is: sharpFile.ProcessThisAndDescendants(new StructuralFinder());

        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            var solution = context.GetData(ProjectModelDataConstants.SOLUTION);
            var isUpdate = solution != null;

            presentation.Visible = isUpdate;

            return isUpdate;
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            var solution = context.GetData(ProjectModelDataConstants.SOLUTION);
            if (solution == null)
            {
                return;
            }

            var projectFile = context.GetData(ProjectModelDataConstants.PROJECT_MODEL_ELEMENT) as IProjectFile;
            var textControl = context.GetData(TextControlDataConstants.TEXT_CONTROL);
            var initialText = context.GetData(GotoByNameDataConstants.CurrentSearchText);

            var projectElement = (IProjectModelElement) projectFile ?? solution;
            var factory = Shell.Instance.GetComponent<GotoWordControllerFactory>();

            factory.ShowMenu(projectElement, textControl, initialText);
        }
    }
}