using UnityEngine;
using UnityEngine.UI;

namespace EventDrivenFramework.Utility
{
    [RequireComponent(typeof(Text))]
    public class AppVersionText : MonoBehaviour
    {
        void Start()
        {
            GetComponent<Text>().text = $"V {Application.version}";
        }
    }
}