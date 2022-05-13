using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArBreakout.Splash
{
    public class Bootstrapper : MonoBehaviour
    {
        private IEnumerator Start()
        {
            var loadScene = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            while (loadScene.progress < 99)
            {
                yield return null;
            }
        }
    }
}