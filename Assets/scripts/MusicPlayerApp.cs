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
    public Button playPauseButton;
    public Button nextButton;
    public Button previousButton;
    public Image playPauseButtonImage;
    public Sprite playIconSprite;
    public Sprite pauseIconSprite;

    private List<Track> activePlaylist = new List<Track>();
    private List<Outline> rowOutlines = new List<Outline>();
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
        if (playPauseButton != null)
        {
            playPauseButton.onClick.RemoveAllListeners();
            playPauseButton.onClick.AddListener(TogglePlayPause);
        }

        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(NextTrack);
        }

        if (previousButton != null)
        {
            previousButton.onClick.RemoveAllListeners();
            previousButton.onClick.AddListener(PreviousTrack);
        }

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

        rowOutlines.Clear();

        for (int i = 0; i < activePlaylist.Count; i++)
        {
            int index = i;
            GameObject rowObj = Instantiate(trackButtonPrefab, trackListContent);
            TextMeshProUGUI label = rowObj.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                label.text = $"{(index + 1):00}. {activePlaylist[index].trackName}";
            }

            Outline outline = rowObj.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = (i == currentTrackIndex);
                rowOutlines.Add(outline);
            }

            Button btn = rowObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => LoadTrack(index, true));
            }
        }
    }

    private void UpdateTrackOutlines()
    {
        for (int i = 0; i < rowOutlines.Count; i++)
        {
            if (rowOutlines[i] != null)
            {
                rowOutlines[i].enabled = (i == currentTrackIndex);
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
            audioSource.time = 0f;
            if (autoPlay && selected.clip != null) audioSource.Play();
        }

        UpdatePlayPauseUI();
        UpdateTrackOutlines();
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