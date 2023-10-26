using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    [Header("Layers")]
    public bool enableLayer1;
    public bool layer1InFade;
    public bool enableLayer2;
    bool layer2InFade;
    public float layerFadeTime;
    public float pauseFadeTime;
    public float targetCutoffFrequency = 500f;
    float currentCutoffFrequency;
    float currentHighPassFrequency;
    public bool Indoor = false;
    int JumpScareIndex = 0;

    int randIndex;
    int prevIndex;

    [Header("Volumes")]
    public float mainThemeVolume;
    public float layer1Volume;
    public float layer2Volume;
    public float ambVolume;

    [Header("Audio Sources")]
    public AudioSource mainThemeAudioSource;
    public AudioSource layer1AudioSource;
    public AudioSource layer2AudioSource;
    public AudioSource oneShotSource;
    public AudioSource intermittentSource;
    public AudioSource outdoorAmbSource;
    public AudioSource heartbeatSource;

    [Header("Heartbeat")]
    public int minPlayerHealth;

    [Header("Audio Effects")]
    public AudioLowPassFilter AmbientLPF;
    public AudioHighPassFilter AmbientHPF;
    public AudioReverbZone IndoorReverbZone;
    public AudioReverbZone OutdoorReverbZone;
    public AudioReverbZone BasementReverbZone;
    public AudioReverbZone TunnelReverbZone;

    [Header("One Shot Clips")]
    public AudioClip StartGameStinger;
    public AudioClip DeathStinger;
    public AudioClip PauseStinger;
    public AudioClip[] Instruments = new AudioClip[3];
    public AudioClip[] JumpScares = new AudioClip[2];

    public GameObject player;

    private void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Music");

        if(objs.Length > 1) 
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.Find("Player");
    }

    // Start is called before the first frame update
    void Start()
    {
        HandleFadeInMainMusic(100f);

        if (IndoorReverbZone != null)
            IndoorReverbZone.enabled = false;
        if (OutdoorReverbZone != null)
            OutdoorReverbZone.enabled = true;
        if (BasementReverbZone != null)
            BasementReverbZone.enabled = false;
        if (TunnelReverbZone != null)
            TunnelReverbZone.enabled = false;
        
        layer1AudioSource.volume = 0f;
        layer2AudioSource.volume = 0f;
        outdoorAmbSource.volume = 0f;
        heartbeatSource.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        HandleReverbZonePosition();

        if (enableLayer1 && layer1AudioSource.volume < (layer1Volume - .02f))
        {
            if (!layer1InFade)
            {
                layer1InFade = true;
                StartCoroutine(FadeInMusic(
                    layer1AudioSource,
                    layer1Volume,
                    layerFadeTime,
                    () => { layer1InFade = false; }));
            }
        }
        else if (!enableLayer1 && layer1AudioSource.volume > 0.05f)
        {
            if (!layer1InFade)
            {
                layer1InFade = true;
                StartCoroutine(FadeInMusic(
                    layer1AudioSource,
                    0f,
                    200f,
                    () => { layer1InFade = false; }));
            }
        }

        if (enableLayer2 && layer2AudioSource.volume < (layer2Volume - .02f))
        {
            if (!layer2InFade)
            {
                layer2InFade = true;
                StartCoroutine(FadeInMusic(
                    layer2AudioSource,
                    layer2Volume,
                    layerFadeTime,
                    () => { layer2InFade = false; }));
            }
        }
        else if (!enableLayer2 && layer2AudioSource.volume > 0.05f)
        {
            if (!layer2InFade)
            {
                layer2InFade = true;
                StartCoroutine(FadeInMusic(
                    layer2AudioSource,
                    0f,
                    50f,
                    () => { layer2InFade = false; }));
            }
        }

        //Audio Effects for Ambience
        if (Indoor)
        {
            currentCutoffFrequency = AmbientLPF.cutoffFrequency;
            AmbientLPF.cutoffFrequency = Mathf.Lerp(currentCutoffFrequency, targetCutoffFrequency, 3f * Time.deltaTime);

            currentHighPassFrequency = AmbientHPF.cutoffFrequency;
            AmbientHPF.cutoffFrequency = Mathf.Lerp(currentHighPassFrequency, 120f, 3f * Time.deltaTime);
        }
        else
        {
            currentCutoffFrequency = AmbientLPF.cutoffFrequency;
            AmbientLPF.cutoffFrequency = Mathf.Lerp(currentCutoffFrequency, 20000f, .2f * Time.deltaTime);

            currentHighPassFrequency = AmbientHPF.cutoffFrequency;
            AmbientHPF.cutoffFrequency = Mathf.Lerp(currentHighPassFrequency, 10f, 1f * Time.deltaTime);
        }
    }

    void HandleReverbZonePosition()
    {
        if(player != null)
        {
            if (IndoorReverbZone != null)
                IndoorReverbZone.transform.position = player.transform.position;
            if (OutdoorReverbZone != null)
                OutdoorReverbZone.transform.position = player.transform.position;
            if (BasementReverbZone != null)
                BasementReverbZone.transform.position = player.transform.position;
            if (TunnelReverbZone != null)
                TunnelReverbZone.transform.position = player.transform.position;
        }
    }

    [ContextMenu("Play Paused Snapshots")]
    public void PlayPauseMusicSnapshot()
    {
        layer1InFade = false;
        layer2InFade = false;

        oneShotSource.PlayOneShot(PauseStinger, 0.5f);
        var mixer = GameManager.Instance.settings.audioMixer;
        float[] weights = { 0f, 1f };
        HandleMusicSnapshots(mixer, weights, pauseFadeTime);
    }

    [ContextMenu("Play Resumed Snapshots")]
    public void PlayResumedMusicSnapshot()
    {
        var mixer = GameManager.Instance.settings.audioMixer;
        float[] weights = { 1f, 0f };
        HandleMusicSnapshots(mixer, weights, pauseFadeTime);
    }

    void HandleMusicSnapshots(
        AudioMixer mixer,
        float[] weights,
        float duration)
    {
        AudioMixerSnapshot[] snapshots = new AudioMixerSnapshot[2];
        snapshots[0] = mixer.FindSnapshot("BasicSnapshot");
        snapshots[1] = mixer.FindSnapshot("PausedSnapshot");
        mixer.TransitionToSnapshots(
            snapshots,
            weights,
            duration);
    }

    void HandleFadeInMainMusic(float fadeTime)
    {
        mainThemeAudioSource.Play();

        mainThemeAudioSource.volume = 0f;
        StartCoroutine(FadeInMusic(
            mainThemeAudioSource,
            mainThemeVolume,
            fadeTime,
            () => { }));
    }

    [ContextMenu("Start Game")]
    public void StartGameMusicEvent()
    {
        StopAllCoroutines();

        oneShotSource.PlayOneShot(StartGameStinger, 0.3f);
        
        enableLayer1 = true;

        StartCoroutine(FadeInMusic(
            mainThemeAudioSource,
            0f,
            layerFadeTime/5,
            () => { StartCoroutine(Intermittents()); }));

        StartCoroutine(FadeInMusic(
            outdoorAmbSource,
            ambVolume,
            layerFadeTime,
            () => { }));
    }

    [ContextMenu("Intensify")]
    public void IntensifyMusicEvent()
    {
        enableLayer2 = true;
    }

    [ContextMenu("Exit to Main Menu")]
    public void ResetMusicEvent()
    {
        Debug.Log("Music Manager Reset Music Event");
        //StopAllCoroutines();
 
        PlayResumedMusicSnapshot();
        StopCoroutine(Intermittents());

        HandleFadeInMainMusic(2000f);

        Indoor = false;

        enableLayer1 = false;
        enableLayer2 = false;

        IndoorReverbZone.enabled = false;
        OutdoorReverbZone.enabled = true;
        BasementReverbZone.enabled = false;
        TunnelReverbZone.enabled = false;

        heartbeatSource.enabled = false;

        layer1AudioSource.volume = 0f;
        layer2AudioSource.volume = 0f;
        outdoorAmbSource.volume = 0f;
    }

    [ContextMenu("Death")]
    public void DeathMusicEvent()
    {        
        ResetMusicEvent();

        oneShotSource.PlayOneShot(DeathStinger, 1f);

    }

    [ContextMenu("Indoor")]
    public void EnterAudioEvent()
    {
        Indoor = true;

        //Reverb Zone switch
        IndoorReverbZone.enabled = true;
        OutdoorReverbZone.enabled = false;
        BasementReverbZone.enabled = false;
        TunnelReverbZone.enabled = false;
    }

    [ContextMenu("Outdoor")]
    public void ExitAudioEvent()
    {
        Indoor = false;

        //Reverb Zone switch
        IndoorReverbZone.enabled = false;
        OutdoorReverbZone.enabled = true;
        BasementReverbZone.enabled = false;
        TunnelReverbZone.enabled = false;

    }

    [ContextMenu("Basement")]
    public void BasementAudioEvent()
    {
        Indoor = true;

        //Reverb switch
        IndoorReverbZone.enabled = false;
        OutdoorReverbZone.enabled = false;
        BasementReverbZone.enabled = true;
        TunnelReverbZone.enabled = false;

    }

    [ContextMenu("Tunnel")]
    public void TunnelAudioEvent()
    {
        Indoor = true;

        //Reverb switch
        IndoorReverbZone.enabled = false;
        OutdoorReverbZone.enabled = false;
        BasementReverbZone.enabled = false;
        TunnelReverbZone.enabled = true;

    }

    public void HandlePlayerHealthChange(int health)
    {
        if(health <= minPlayerHealth && !heartbeatSource.enabled)
        {
            LowHealthEvent();
        }
        else if (health > minPlayerHealth && heartbeatSource.enabled)
        {
            RegainedHealthEvent();
        }
    }

    [ContextMenu("HeartBeat")]
    public void LowHealthEvent()
    {
        heartbeatSource.enabled = true;
    }

    [ContextMenu("Regained")]
    public void RegainedHealthEvent()
    {
        heartbeatSource.enabled = false;
    }

    [ContextMenu("JumpScare")]
    public void PlayJumpScare()
    {
        JumpScareIndex = (JumpScareIndex + 1) % 2;

        oneShotSource.PlayOneShot(JumpScares[JumpScareIndex], .6f);
    }

    IEnumerator FadeInMusic(
        AudioSource src,
        float targetVol,
        float dur,
        System.Action callback)
    {
        // Setup
        float t = 0f;

        // Loop
        while (true)
        {
            // Loop logic
            src.volume = Mathf.Lerp(
                src.volume,
                targetVol,
                t / dur);
            t += Time.deltaTime;

            // Check to break out of loop
            if (Mathf.Abs(src.volume - targetVol) <= 0.02f)
            {
                src.volume = targetVol;
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        // Cleanup here
        callback();
    }

    IEnumerator Intermittents()
    {

        float randInterval = Random.Range(4f, 6f);

        yield return new WaitForSeconds(randInterval);

        do
        {
            randIndex = Random.Range(0, 2);
        }
        while (randIndex == prevIndex);

        //int randIndex = Random.Range(0, 2);
        float randPitch = Random.Range(0.6f, 1.2f);
        float randVol = Random.Range(0.3f, 0.6f);
        float randPan = Random.Range(-.5f, .5f);

        prevIndex = randIndex;
        
        intermittentSource.pitch = randPitch;
        intermittentSource.panStereo = randPan;
        intermittentSource.PlayOneShot(Instruments[randIndex], randVol);

        yield return new WaitForSeconds(Instruments[randIndex].length);


        StartCoroutine(Intermittents());
    }

    /*
    public void EnableMusicLayer(int layer, bool enable)
    {
        if(layer == 1)
        {
            enableLayer1 = enable;
        }
        else if(layer == 2)
        {
            enableLayer2 = true;
        }
    }
    */
}
