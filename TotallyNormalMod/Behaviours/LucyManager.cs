using System.Collections;
using UnityEngine;

namespace TotallyNormalMod.Behaviours
{
    public class LucyManager : MonoBehaviour
    {
        void OnEnable() => ButtonActions.onPressed += OnButtonPressed;

        void OnDisable() => ButtonActions.onPressed -= OnButtonPressed;

        private void OnButtonPressed() => StartCoroutine(TryEnableGhosts());

        IEnumerator TryEnableGhosts()
        {
            if (!NetworkSystem.Instance.InRoom)
            {
                yield return new WaitForSeconds(20.15f);

                GameObject envRoot = GameObject.Find("Environment Objects");
                if (envRoot == null)
                {
                    Debug.LogWarning("[TNM]: Environment Objects not found!");
                    yield break;
                }

                Transform mazePersistent = envRoot.transform.Find("05Maze_PersistentObjects");
                if (mazePersistent == null)
                {
                    Debug.LogWarning("[TNM]: Maze_PersistentObjects not found!");
                    yield break;
                }

                Transform ghosts = mazePersistent.Find("Ghosts");
                if (ghosts == null)
                {
                    Debug.LogWarning("[TNM]: Ghosts not found!");
                    yield break;
                }

                envRoot.SetActive(true);
                mazePersistent.gameObject.SetActive(true);
                ghosts.gameObject.SetActive(true);

                Transform halloweenGhost = ghosts.Find("Halloween Ghost");
                if (halloweenGhost != null)
                {
                    halloweenGhost.gameObject.SetActive(true);

                    halloweenGhost.Find("FloatingChaseSkeleton")?.gameObject.SetActive(true);
                    halloweenGhost.Find("FloatingChaseSkeleton - BAYOU only")?.gameObject.SetActive(false);
                }

                Debug.Log("[TNM]: Ghosts successfully enabled!");
            }
        }

    }
}
