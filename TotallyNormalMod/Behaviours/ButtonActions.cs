using GorillaLocomotion;
using GorillaTag.Rendering;
using HarmonyLib;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable CS0618

namespace TotallyNormalMod.Behaviours
{
    public class ButtonActions : GTDoorTrigger
    {
        private bool Debouncing = false;
        public static event Action onPressed;

        private static AudioSource audioSource;
        private static GameObject audioObj;

        private readonly float _groundFogDepthFadeSize = 10f;
        private readonly float _groundFogHeightFadeSize = 100f;
        private readonly float groundFogHeight = 200f;

        private bool fogRequested = false;

        Vector3 pTPos = new Vector3(-66.0787f, 21.8672f, -81.6381f); Quaternion pTPRot = Quaternion.Euler(0f, 0f, 0f);

        GTZone zones;

        private void OnTriggerExit(Collider col) 
        {
            if (Debouncing) return;

            MethodInfo baseMethod = typeof(GTDoorTrigger).GetMethod("OnTriggerExit",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            baseMethod?.Invoke(this, new object[] { col });

            if (!NetworkSystem.Instance.InRoom) return;

            Debug.Log("[TNM]: Button Pressed, Starting Actions.");

            ZoneManagement.SetActiveZone(zones);

            BetterDayNightManager.instance.SetTimeOfDay(0);

            fogRequested = true;

            GameObject managerObj = new GameObject("LucyManager");
            DontDestroyOnLoad(managerObj);

            managerObj.AddComponent<LucyManager>();

            GTPlayer.Instance.StartCoroutine(TimedActions());
            GTPlayer.Instance.StartCoroutine(DelayedFogApply());
            GTPlayer.Instance.StartCoroutine(HorrorAudio());

            onPressed?.Invoke();
        }

        private static IEnumerator HorrorAudio()
        {
            string qUrl = "https://raw.githubusercontent.com/kinomonke/TotallyNormalMod/main/TotallyNormalMod/Audio/ghostChaseBG.mp3";

            audioObj = new GameObject("TNM_AudioPlayer_LucyChase");
            DontDestroyOnLoad(audioObj);
            audioSource = audioObj.AddComponent<AudioSource>();

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(qUrl, AudioType.MPEG))
            {
                yield return www.SendWebRequest();
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);

                audioSource.clip = clip;
                audioSource.playOnAwake = false;
                audioSource.loop = false;

                audioSource.Play();

                yield return new WaitForSeconds(0.25f);

                Debug.Log("[TNM]: quitting game.");
            }
        }

        IEnumerator TimedActions()
        {
            GTPlayer.Instance.disableMovement = true;
            GTPlayer.Instance.TeleportTo(pTPos, pTPRot);

            Destroy(GameObject.Find("DungeonHandBlock_Prefab_Outside(Clone)"));

            yield return new WaitForSeconds(20.35f);

            Debug.Log("[TNM]: Finished Timed Actions");
            GTPlayer.Instance.disableMovement = false;
        }

        private IEnumerator DelayedFogApply()
        {
            // Wait a bit for ZoneShaderSettings to load before applying
            yield return new WaitForSeconds(1f);
            ApplyFogToAllZones();
        }

        private void ApplyFogToAllZones()
        {
            ZoneShaderSettings[] allZones = UnityEngine.Object.FindObjectsOfType<ZoneShaderSettings>();

            if (allZones.Length == 0)
            {
                Debug.LogWarning("[TNM]: No ZoneShaderSettings found. Will retry shortly...");
                StartCoroutine(DelayedFogApply());
                return;
            }

            foreach (var zone in allZones)
            {
                try
                {
                    zone.isDefaultValues = true;

                    var traverse = Traverse.Create(zone);
                    traverse.Field("groundFogHeight").SetValue(groundFogHeight);
                    traverse.Field("_groundFogDepthFadeSize").SetValue(_groundFogDepthFadeSize);
                    traverse.Field("_groundFogHeightFadeSize").SetValue(_groundFogHeightFadeSize);

                    typeof(ZoneShaderSettings)
                        .GetMethod("ApplyValues", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static)
                        .Invoke(zone, new object[] { });

                    Debug.Log($"[TNM]: Applied fog to {zone.gameObject.name}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"[TNM]: Failed to apply fog to {zone.gameObject.name}: {e.Message}");
                }
            }

            Debug.Log("[TNM]: Fog successfully applied to all maps!");
        }
    }
}