using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class MusicPlayerApp : MonoBehaviour
{
    [System.Serializable]
    public struct Track
    {
        public string trackName;
        public AudioClip clip;
    }

    [Header("Audio & Playlist")]
    public AudioSource audioSource;
    public List<Track> builtInPlaylist = new List<Track>();

    [Header("Scroll List References")]
    public RectTransform trackListContent;
    public GameObject trackButtonPrefab;

    [Header("Playback UI References")]
    public Image playPauseButtonImage;
    public Sprite playIconSprite;
    public Sprite pauseIconSprite;

    private List<Track> activePlaylist = new List<Track>();
    private int currentTrackIndex = -1;
    private string importedFolderPath;

    private void Awake()
    {
        importedFolderPath = Path.Combine(Application.persistentDataPath, "ImportedMusic");
        if (!Directory.Exists(importedFolderPath))
        {
            Directory.CreateDirectory(importedFolderPath);
        }
    }

    private void Start()
    {
        activePlaylist = new List<Track>(builtInPlaylist);
        RebuildTrackListUI();

        if (activePlaylist.Count > 0)
        {
            LoadTrack(0, false);
        }

        LoadSavedImportedTracks();
    }

    private void LoadSavedImportedTracks()
    {
        if (!Directory.Exists(importedFolderPath)) return;

        string[] savedFiles = Directory.GetFiles(importedFolderPath);
        List<string> validAudioFiles = new List<string>();

        foreach (string file in savedFiles)
        {
            string ext = Path.GetExtension(file).ToLower();
            if (ext == ".mp3" || ext == ".wav" || ext == ".ogg")
            {
                validAudioFiles.Add(file);
            }
        }

        if (validAudioFiles.Count > 0)
        {
            StartCoroutine(LoadAudioFilesCoroutine(validAudioFiles.ToArray(), false));
        }
    }

    public void ImportMusicFiles()
    {
#if UNITY_EDITOR
        string path = UnityEditor.EditorUtility.OpenFilePanelWithFilters(
            "Import Music Track",
            "",
            new string[] { "Audio Files", "mp3,wav,ogg" }
        );

        if (!string.IsNullOrEmpty(path))
        {
            string fileName = Path.GetFileName(path);
            string destPath = Path.Combine(importedFolderPath, fileName);

            File.Copy(path, destPath, true);

            StartCoroutine(LoadAudioFilesCoroutine(new string[] { destPath }, true));
        }
#else
        Debug.LogWarning("Runtime file dialog requires a standalone file browser plugin in builds. Drop files into: " + importedFolderPath);
#endif
    }

    private IEnumerator LoadAudioFilesCoroutine(string[] filePaths, bool autoPlayLast)
    {
        foreach (string filePath in filePaths)
        {
            string ext = Path.GetExtension(filePath).ToLower();
            AudioType audioType = AudioType.UNKNOWN;

            if (ext == ".mp3") audioType = AudioType.MPEG;
            else if (ext == ".wav") audioType = AudioType.WAV;
            else if (ext == ".ogg") audioType = AudioType.OGGVORBIS;

            if (audioType == AudioType.UNKNOWN) continue;

            string uri = "file:///" + filePath.Replace("\\", "/");
            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(uri, audioType))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(uwr);
                    if (clip != null)
                    {
                        string title = Path.GetFileNameWithoutExtension(filePath);
                        clip.name = title;

                        activePlaylist.Add(new Track
                        {
                            trackName = title,
                            clip = clip
                        });

                        RebuildTrackListUI();

                        if (autoPlayLast)
                        {
                            LoadTrack(activePlaylist.Count - 1, true);
                        }
                    }
                }
                else
                {
                    Debug.LogError("Failed to load audio: " + uwr.error);
                }
            }
        }
    }

    public void RebuildTrackListUI()
    {
        foreach (Transform child in trackListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < activePlaylist.Count; i++)
        {
            int index = i;
            GameObject rowObj = Instantiate(trackButtonPrefab, trackListContent);
            TextMeshProUGUI label = rowObj.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                label.text = $"{(index + 1):00}. {activePlaylist[index].trackName}";
            }

            Button btn = rowObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => LoadTrack(index, true));
            }
        }
    }

    public void LoadTrack(int index, bool autoPlay = true)
    {
        if (index < 0 || index >= activePlaylist.Count) return;

        currentTrackIndex = index;
        Track selected = activePlaylist[currentTrackIndex];

        if (audioSource != null)
        {
            audioSource.clip = selected.clip;
            if (autoPlay) audioSource.Play();
        }

        UpdatePlayPauseUI();
    }

    public void TogglePlayPause()
    {
        if (audioSource == null || audioSource.clip == null) return;

        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.Play();
        }

        UpdatePlayPauseUI();
    }

    private void UpdatePlayPauseUI()
    {
        bool isPlaying = audioSource != null && audioSource.isPlaying;

        if (playPauseButtonImage != null)
        {
            if (isPlaying && pauseIconSprite != null)
            {
                playPauseButtonImage.sprite = pauseIconSprite;
            }
            else if (!isPlaying && playIconSprite != null)
            {
                playPauseButtonImage.sprite = playIconSprite;
            }
        }
    }

    public void NextTrack()
    {
        if (activePlaylist.Count == 0) return;
        LoadTrack((currentTrackIndex + 1) % activePlaylist.Count, true);
    }

    public void PreviousTrack()
    {
        if (activePlaylist.Count == 0) return;
        LoadTrack((currentTrackIndex - 1 + activePlaylist.Count) % activePlaylist.Count, true);
    }

    private void Update()
    {
        if (audioSource != null && audioSource.clip != null && !audioSource.isPlaying)
        {
            if (audioSource.time >= audioSource.clip.length - 0.1f && audioSource.time > 0f)
            {
                NextTrack();
            }
        }
    }
}