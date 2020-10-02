using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ToggleUINode : UINode
{
    public ToggleUINode() : base()
    {
        IsEntryPoint = true;
    }
    public enum UIAction
    {
        Show, Hide, Refresh
    };

    public UIAction ActionToDo;
}
