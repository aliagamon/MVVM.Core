using System.Threading.Tasks;

namespace MVVM.Core.Services
{
    public interface IDialogueService
    {
         Task<DialogueResult> ShowDialogue(DialogueType type, string tile, string message, string ok = "Ok", string yes = "Yes", string no = "No");
    }

    public enum DialogueType
    {
        YesNo,
        Ok
    }
    

    public enum DialogueResult
    {
        Yes,
        No
    }
}
