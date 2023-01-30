using TMPro;
using UnityEngine;

namespace EventDrivenFramework.Utility
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class AppVersionTextMeshPro : MonoBehaviour
    {
        void Start()
        {
            GetComponent<TextMeshProUGUI>().text = $"V {Application.version}";
        }
    }
}