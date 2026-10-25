using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // SINGLETON
    public static AudioManager Instance { get; private set; }

    // AUDIO SOURCE - SFX
    [Header("Audio Source - SFX")]
    [Tooltip("AudioSource yang digunakan untuk memainkan sound effect.")]
    [SerializeField] private AudioSource sfxSource;

    // BACKGROUND MUSIC
    [Header("Background Music")]
    [Tooltip("AudioSource khusus background music. Jika kosong, akan dibuat otomatis.")]
    [SerializeField] private AudioSource bgmSource;

    [Tooltip("Musik latar default yang dimainkan ketika AudioManager aktif.")]
    [SerializeField] private AudioClip backgroundMusic;

    [Tooltip("Aktifkan agar musik langsung dimainkan ketika AudioManager aktif.")]
    [SerializeField] private bool playBGMOnAwake = true;

    [Tooltip("Aktifkan agar background music diputar berulang.")]
    [SerializeField] private bool loopBackgroundMusic = true;

    [Tooltip("Volume background music dari 0 sampai 1.")]
    [Range(0f, 1f)]
    [SerializeField] private float backgroundMusicVolume = 0.5f;

    // AUDIO CLIPS - ITEM
    [Header("Sound Effect - Item")]
    [Tooltip("Suara ketika player mulai mengambil item.")]
    [SerializeField] private AudioClip dragSound;
    [Tooltip("Suara ketika item berhasil masuk ke kotak jawaban.")]
    [SerializeField] private AudioClip dropSound;

    // AUDIO CLIPS - JAWABAN
    [Header("Sound Effect - Jawaban")]
    [Tooltip("Suara ketika jawaban benar.")]
    [SerializeField] private AudioClip correctSound;
    [Tooltip("Suara ketika jawaban salah.")]
    [SerializeField] private AudioClip wrongSound;

    // AUDIO CLIPS - TOMBOL
    [Header("Sound Effect - Tombol")]
    [Tooltip("Suara ketika tombol Undo ditekan.")]
    [SerializeField] private AudioClip undoSound;
    [Tooltip("Suara ketika tombol Reset ditekan.")]
    [SerializeField] private AudioClip resetSound;
    [Tooltip("Suara umum ketika tombol UI ditekan.")]
    [SerializeField] private AudioClip buttonClickSound;

    // BATTLE SFX
    [Header("Battle SFX")]
    [SerializeField] private AudioClip mantraSuccessSound;
    [SerializeField] private AudioClip mantraFailedSound;
    [SerializeField] private AudioClip basicCastSound;
    [SerializeField] private AudioClip mediumCastSound;
    [SerializeField] private AudioClip advancedCastSound;
    [SerializeField] private AudioClip magicImpactSound;
    [SerializeField] private AudioClip enemyAttackSound;
    [SerializeField] private AudioClip shieldBlockSound;
    [SerializeField] private AudioClip playerHitSound;
    [SerializeField] private AudioClip enemyHitSound;
    [SerializeField] private AudioClip victorySound;
    [SerializeField] private AudioClip defeatSound;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeAudioSources();
    }

    private void InitializeAudioSources()
    {
        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();

        if (sfxSource == null)
            Debug.LogError("AudioManager: AudioSource untuk SFX tidak ditemukan!");

        // BGM dan SFX harus menggunakan AudioSource yang berbeda.
        if (bgmSource == null || bgmSource == sfxSource)
        {
            if (bgmSource != null && bgmSource == sfxSource)
                Debug.LogWarning("AudioManager: SFX Source dan BGM Source sama. AudioSource BGM baru dibuat.");

            bgmSource = gameObject.AddComponent<AudioSource>();
        }

        bgmSource.playOnAwake = false;
        bgmSource.loop = loopBackgroundMusic;
        bgmSource.volume = Mathf.Clamp01(backgroundMusicVolume);
        bgmSource.spatialBlend = 0f; // Musik 2D

        if (playBGMOnAwake)
            PlayBackgroundMusic();
    }

    // PLAY DEFAULT BACKGROUND MUSIC
    public void PlayBackgroundMusic()
    {
        if (bgmSource == null)
        {
            Debug.LogWarning("AudioManager: BGM Source belum tersedia.");
            return;
        }

        if (backgroundMusic == null)
        {
            Debug.LogWarning("AudioManager: Background Music belum dihubungkan di Inspector.");
            return;
        }

        if (bgmSource.clip != backgroundMusic)
            bgmSource.clip = backgroundMusic;

        bgmSource.loop = loopBackgroundMusic;
        bgmSource.volume = Mathf.Clamp01(backgroundMusicVolume);

        if (!bgmSource.isPlaying)
            bgmSource.Play();
    }

    // PLAY OR CHANGE MUSIC CLIP (misalnya untuk musik Battle)
    public void PlayBackgroundMusic(AudioClip clip, bool restart = false)
    {
        if (bgmSource == null)
        {
            Debug.LogWarning("AudioManager: BGM Source belum tersedia.");
            return;
        }

        if (clip == null)
        {
            Debug.LogWarning("AudioManager: Clip background music yang diminta kosong.");
            return;
        }

        bool sameClip = bgmSource.clip == clip;
        backgroundMusic = clip;

        if (sameClip && bgmSource.isPlaying && !restart)
            return;

        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.loop = loopBackgroundMusic;
        bgmSource.volume = Mathf.Clamp01(backgroundMusicVolume);
        bgmSource.Play();
    }

    public void StopBackgroundMusic()
    {
        if (bgmSource != null && bgmSource.isPlaying)
            bgmSource.Stop();
    }

    public void PauseBackgroundMusic()
    {
        if (bgmSource != null && bgmSource.isPlaying)
            bgmSource.Pause();
    }

    public void ResumeBackgroundMusic()
    {
        if (bgmSource == null)
        {
            Debug.LogWarning("AudioManager: BGM Source belum tersedia.");
            return;
        }

        if (bgmSource.clip == null)
        {
            PlayBackgroundMusic();
            return;
        }

        if (!bgmSource.isPlaying)
        {
            bgmSource.UnPause();
            if (!bgmSource.isPlaying)
                bgmSource.Play();
        }
    }

    public void SetBackgroundMusicVolume(float volume)
    {
        backgroundMusicVolume = Mathf.Clamp01(volume);
        if (bgmSource != null)
            bgmSource.volume = backgroundMusicVolume;
    }

    // METHOD UTAMA SFX
    private void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null)
        {
            Debug.LogWarning("AudioManager: SFX Source belum tersedia.");
            return;
        }

        if (clip == null)
        {
            Debug.LogWarning("AudioManager: AudioClip SFX belum dihubungkan.");
            return;
        }

        sfxSource.PlayOneShot(clip);
    }

    public void PlayDragSound() { PlaySFX(dragSound); }
    public void PlayDropSound() { PlaySFX(dropSound); }
    public void PlayCorrectSound() { PlaySFX(correctSound); }
    public void PlayWrongSound() { PlaySFX(wrongSound); }
    public void PlayUndoSound() { PlaySFX(undoSound); }
    public void PlayResetSound() { PlaySFX(resetSound); }
    public void PlayButtonClickSound() { PlaySFX(buttonClickSound); }
    public void PlayMantraSuccessSound() { PlaySFX(mantraSuccessSound); }
    public void PlayMantraFailedSound() { PlaySFX(mantraFailedSound); }
    public void PlayBasicCastSound() { PlaySFX(basicCastSound); }
    public void PlayMediumCastSound() { PlaySFX(mediumCastSound); }
    public void PlayAdvancedCastSound() { PlaySFX(advancedCastSound); }
    public void PlayMagicImpactSound() { PlaySFX(magicImpactSound); }
    public void PlayEnemyAttackSound() { PlaySFX(enemyAttackSound); }
    public void PlayShieldBlockSound() { PlaySFX(shieldBlockSound); }
    public void PlayPlayerHitSound() { PlaySFX(playerHitSound); }
    public void PlayEnemyHitSound() { PlaySFX(enemyHitSound); }
    public void PlayVictorySound() { PlaySFX(victorySound); }
    public void PlayDefeatSound() { PlaySFX(defeatSound); }
}
