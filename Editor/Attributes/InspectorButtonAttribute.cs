using UnityEngine;
using IA.Utils;

namespace IA.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class InspectorButtonAttribute : PropertyAttribute
    {
        public readonly string methodName;
        public readonly object[] parameters;
        public readonly bool isIEnumerator;
        public readonly bool isQuestionBoxEnable;
        public readonly DialogBoxParameters dialogBoxParameters;

        public InspectorButtonAttribute(string methodName, object[] parameters = null, bool isIEnumerator = false, bool enableDialogBox = false,
            string dialogTitle = "", string dialogMessage = "", string dialogOk = "", string dialogCancel = "", string dialogAlt = "")
        {
            this.methodName = methodName;
            this.parameters = parameters;
            this.isIEnumerator = isIEnumerator;
            this.isQuestionBoxEnable = enableDialogBox;

            bool hasDialogueBox = !dialogTitle.IsNullOrWhiteSpace() &&
                                  !dialogMessage.IsNullOrWhiteSpace() &&
                                  !dialogOk.IsNullOrWhiteSpace();

            if (hasDialogueBox)
            {
                this.dialogBoxParameters = new DialogBoxParameters()
                {
                    Title = dialogTitle,
                    Message = dialogMessage,
                    Ok = dialogOk,
                    Cancel = dialogCancel,
                    Alt = dialogAlt
                };
            }
            else
            {
                this.dialogBoxParameters = GetDefaultDialogBoxParameters;
            }
        }

        public static DialogBoxParameters GetDefaultDialogBoxParameters => new DialogBoxParameters()
        {
            Title = "Button Clicked",
            Message = "Do you want to an Action ?",
            Ok = "Yes",
            Cancel = "No",
            Alt = ""
        };
    }

    [System.Serializable]
    public struct DialogBoxParameters
    {
        public string Title;
        public string Message;
        public string Ok;
        public string Cancel;
        public string Alt;
    }
}