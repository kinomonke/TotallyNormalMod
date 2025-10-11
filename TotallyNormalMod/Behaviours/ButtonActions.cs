using GorillaLocomotion;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

namespace TotallyNormalMod.Behaviours
{
    public class ButtonActions : GTDoorTrigger
    {
        private bool Debouncing = false;
        private AudioSource audioSource;
        public static event Action onPressed;

        Vector3 pTPos = new Vector3(-66.0787f, 21.8672f, -81.6381f);
        Quaternion pTPRot = Quaternion.Euler(0f, 0f, 0f);

        GTZone zones;

        private void OnTriggerExit(Collider col)
        {
            if (Debouncing) return;

            MethodInfo baseMethod = typeof(GTDoorTrigger).GetMethod("OnTriggerExit",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (baseMethod != null) { baseMethod.Invoke(this, new object[] { col }); }
            else { Debug.LogError("[TNM]: Base trigger method not found."); }
           
            #region Button Actions

            if (!NetworkSystem.Instance.InRoom)
            {
                Debug.Log("[TNM]: Button Pressed, Starting Actions.");

                ZoneManagement.SetActiveZone(zones);
                
                BetterDayNightManager.instance.SetTimeOfDay(0);

                GameObject managerObj = new GameObject("LucyManager");
                GameObject.DontDestroyOnLoad(managerObj);
                managerObj.AddComponent<LucyManager>();
                
                GTPlayer.Instance.StartCoroutine(TimedActions());

                onPressed?.Invoke();
            }

            #endregion
        }

        IEnumerator TimedActions()
        {
            GTPlayer.Instance.disableMovement = true;
            GTPlayer.Instance.TeleportTo(pTPos, pTPRot);
            GameObject.Find("DungeonHandBlock_Prefab_Outside(Clone)").Destroy();

            yield return new WaitForSeconds(20.35f);

            Debug.Log("[TNM]: Finished Timed Actions");

            GTPlayer.Instance.disableMovement = false;

        }
        IEnumerator PlayAudio()
        { string hUrl = "";

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(hUrl, AudioType.MPEG))
            {
                yield return www.SendWebRequest();
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"[TNM]: Failed to load audio from web: {www.error}"); yield break;
                }

                if (clip == null)
                {
                    Debug.LogError("[TNM]: AudioClip is null"); yield break;
                }

                if (audioSource == null)
                    audioSource = gameObject.AddComponent<AudioSource>();

                audioSource.clip = clip;
                audioSource.playOnAwake = false;
                audioSource.loop = false;

                audioSource.Play();
                audioSource.loop = true;
                Debug.Log("[TNM]: Playing Audio.");
            }
        }
    }
}