using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace Assets.Editor
{
    class Utils
    {
        public static void MessageBox(string text)
        {
            EditorUtility.DisplayDialog("MessageBox", text, "OK");
        }
    }
}
