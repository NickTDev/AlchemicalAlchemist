using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioEngineManager : MonoBehaviour

{
    [SerializeField] string[] InitialBanks;
    private static GameObject _instance;
    private static AudioEngineManager _engineInstance;
    /*public static AudioEngineManager EngineInstance
    {
        get { return _engineInstance; }
    }*/
    private static List<FMOD.Studio.EventInstance> Events = new List<FMOD.Studio.EventInstance>();
    private static FMOD.Studio.System studioSystem;
    private static FMOD.System coreSystem;
    private static List<FMOD.Studio.Bank> loadedBanks = new List<FMOD.Studio.Bank>();

    void Start()
    {
        //Singleton instance
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this.gameObject;
            _engineInstance = this;
            DontDestroyOnLoad(_instance.gameObject);
            studioSystem = FMODUnity.RuntimeManager.StudioSystem;
            foreach (string x in InitialBanks)
            {
                studioSystem.loadBankFile(x, FMOD.Studio.LOAD_BANK_FLAGS.NORMAL, out FMOD.Studio.Bank bank);
                bank.loadSampleData();
                loadedBanks.Add(bank);
            }
            studioSystem.getCoreSystem(out coreSystem);
        }
    }

    void Update()
    {
        if (Events != null)
        {
            foreach (FMOD.Studio.EventInstance x in Events)
            {
                FMOD.Studio.PLAYBACK_STATE state;
                x.getPlaybackState(out state);
                switch (state)
                {
                    case FMOD.Studio.PLAYBACK_STATE.STARTING:
                        break;

                    case FMOD.Studio.PLAYBACK_STATE.PLAYING:
                        break;

                    case FMOD.Studio.PLAYBACK_STATE.STOPPING:
                        break;

                    case FMOD.Studio.PLAYBACK_STATE.STOPPED:
                        x.getPaused(out bool pauseCheck);
                        if (!pauseCheck)
                            x.release();
                        break;
                }
            }
            Events.RemoveAll(removeReleasedIntances);
        }
    }

    private bool removeReleasedIntances(FMOD.Studio.EventInstance eventInstance)
    {
        return !eventInstance.isValid();
    }

    /// <summary>
    /// Creates given Event without position (assumes 2D)
    /// </summary>
    public static FMOD.Studio.EventInstance CreateSound(FMODUnity.EventReference eventReference, float volume)
    {
        if (!eventReference.IsNull )
        {
            FMOD.Studio.EventInstance instance;
            instance = FMODUnity.RuntimeManager.CreateInstance(eventReference);
            instance.setVolume(volume);
            Events.Add(instance);
            return instance;
        }
        else
        {
            FMOD.Studio.EventInstance x = new FMOD.Studio.EventInstance();
            Events.Add(x);
            return x;
        }
    }

    /// <summary>
    /// Creates given Event at position
    /// </summary>
    public static FMOD.Studio.EventInstance CreateSound(FMODUnity.EventReference eventReference, float volume, Vector3 position)
    {
        if (!eventReference.IsNull)
        {
            FMOD.Studio.EventInstance instance;
            instance = FMODUnity.RuntimeManager.CreateInstance(eventReference);
            instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(position));
            instance.setVolume(volume);
            Events.Add(instance);
            return instance;
        }
        else
        {
            FMOD.Studio.EventInstance x = new FMOD.Studio.EventInstance();
            Events.Add(x);
            return x;
        }
    }

    /// <summary>
    /// Creates given Event tethered to an object's transform
    /// </summary>
    public static FMOD.Studio.EventInstance CreateSound(FMODUnity.EventReference eventReference, float volume, GameObject tether)
    {
        if (!eventReference.IsNull)
        {
            FMOD.Studio.EventInstance instance;
            instance = FMODUnity.RuntimeManager.CreateInstance(eventReference);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, tether.transform);
            instance.setVolume(volume);
            Events.Add(instance);
            return instance;
        }
        else
        {
            FMOD.Studio.EventInstance x = new FMOD.Studio.EventInstance();
            Events.Add(x);
            return x;
        }
    }

    /// <summary>
    /// Creates and plays given Event without position (assumes 2D)
    /// </summary>
    public static FMOD.Studio.EventInstance PlaySound(FMODUnity.EventReference eventReference, float volume)
    {
        if (!eventReference.IsNull)
        {
            FMOD.Studio.EventInstance instance;
            instance = FMODUnity.RuntimeManager.CreateInstance(eventReference);
            instance.setVolume(volume);
            Events.Add(instance);
            instance.start();
            return instance;
        }
        else
        {
            FMOD.Studio.EventInstance x = new FMOD.Studio.EventInstance();
            Events.Add(x);
            return x;
        }
    }

    /// <summary>
    /// Creates and plays given Event at given position
    /// </summary>
    public static FMOD.Studio.EventInstance PlaySound(FMODUnity.EventReference eventReference, float volume, Vector3 position)
    {
        if (!eventReference.IsNull)
        {
            FMOD.Studio.EventInstance instance;
            instance = FMODUnity.RuntimeManager.CreateInstance(eventReference);
            instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(position));
            instance.setVolume(volume);
            Events.Add(instance);
            instance.start();
            return instance;
        }
        else
        {
            FMOD.Studio.EventInstance x = new FMOD.Studio.EventInstance();
            Events.Add(x);
            return x;
        }
    }

    /// <summary>
    /// Creates given Event tethered to an object's transform
    /// </summary>
    public static FMOD.Studio.EventInstance PlaySound(FMODUnity.EventReference eventReference, float volume, GameObject tether)
    {
        if (!eventReference.IsNull)
        {
            FMOD.Studio.EventInstance instance;
            instance = FMODUnity.RuntimeManager.CreateInstance(eventReference);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, tether.transform);
            instance.setVolume(volume);
            Events.Add(instance);
            instance.start();
            return instance;
        }
        else
        {
            FMOD.Studio.EventInstance x = new FMOD.Studio.EventInstance();
            Events.Add(x);
            return x;
        }

    }

}