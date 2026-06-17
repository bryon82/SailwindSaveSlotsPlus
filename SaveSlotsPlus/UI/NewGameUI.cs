using UnityEngine;

namespace SaveSlotsPlus
{
    internal class NewGameUI
    {
        internal static void InitializeUI(Transform chooseIslandUI)
        {
            // selection background scroll
            var bigBG = chooseIslandUI.GetChild(0);
            bigBG.localPosition = new Vector3(0f, -0.449f, 0.07f);
            bigBG.localScale = new Vector3(1.32f, 1.32f, 1.38f);

            // button confirm
            chooseIslandUI.GetChild(2).localPosition = new Vector3(-0.005f, -0.361f, 0.037f);

            // buttons left and right
            chooseIslandUI.GetChild(4).localPosition = new Vector3(-0.7f, -0.361f, 0.038f);
            chooseIslandUI.GetChild(6).localPosition = new Vector3(0.7f, -0.361f, 0.038f);

            // button back
            chooseIslandUI.GetChild(5).localPosition = new Vector3(-0.005f, -0.716f, 0.037f);

            // selection text
            var textTemplate = chooseIslandUI.GetChild(3);
            textTemplate.localPosition = new Vector3(-0.022f, -0.119f, 0.037f);

            var scroll = chooseIslandUI.GetChild(7);

            var label = GameObject.Instantiate(textTemplate, scroll);
            label.name = "label";
            label.transform.localPosition = new Vector3(0f, 0f, 0.1f);
            label.transform.localEulerAngles = new Vector3(0f, 270f, 270f);
            label.transform.localScale = new Vector3(0.0194f, 0.0354f, 0.0194f);
            var labelMesh = label.GetComponent<TextMesh>();
            labelMesh.text = "name your save:";
            labelMesh.anchor = TextAnchor.MiddleCenter;
            labelMesh.alignment = TextAlignment.Center;

            var input = GameObject.Instantiate(textTemplate, scroll);
            input.name = "input text";
            input.transform.localPosition = new Vector3(0f, 0f, -0.1f);
            input.transform.localEulerAngles = new Vector3(0f, 270f, 270f);
            input.transform.localScale = new Vector3(0.0194f, 0.0354f, 0.0194f);
            var inputMesh = input.GetComponent<TextMesh>();
            inputMesh.text = "";
            inputMesh.anchor = TextAnchor.MiddleCenter;
            inputMesh.alignment = TextAlignment.Center;
            inputMesh.fontStyle = FontStyle.Normal;

            NewGamePatches.SaveGameInputText = scroll.gameObject.AddComponent<SaveNameInput>();
            NewGamePatches.SaveGameInputText.displayMesh = inputMesh;
        }
    }
}
