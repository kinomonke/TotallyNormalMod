using HarmonyLib;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace TotallyNormalMod.Patches
{
    [HarmonyPatch(typeof(HalloweenGhostChaser), "LateUpdate")]
    public class GhostKiller
    {
        private static bool quitting = false;
        private static AudioSource audioSource;
        private static GameObject audioObj;

        public static void Postfix(HalloweenGhostChaser __instance)
        {
            if (quitting) return;

            if (__instance.currentState == HalloweenGhostChaser.ChaseState.Grabbing &&
                __instance.targetPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                quitting = true;
                __instance.StartCoroutine(PlayAndQuit());
            }
        }

        private static IEnumerator PlayAndQuit()
        {
            string qUrl = "https://raw.githubusercontent.com/kinomonke/TotallyNormalMod/main/TotallyNormalMod/Audio/GameCloseScream.mp3";

            audioObj = new GameObject("TNM_AudioPlayer");
            Object.DontDestroyOnLoad(audioObj);
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

                Application.Quit();
                Debug.Log("[TNM]: quitting game.");
            }
        }
    }   
}