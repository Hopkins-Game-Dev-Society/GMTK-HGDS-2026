using BirthdayJobJam.Core;
using UnityEngine;

namespace BirthdayJobJam.Player
{
    public sealed class PhoneEventManager : MonoBehaviour
    {
        [Header("Timer")]
        [SerializeField]
        private GameplayTimer timer;


        [Header("Phone UI")]
        [SerializeField]
        private GameObject phoneRoot;


        [Header("Audio")]
        [SerializeField]
        private AudioSource audioSource;

        [SerializeField]
        private AudioClip ringtone;

        [SerializeField]
        private AudioClip message;



        [Header("Event Settings")]
        [Tooltip("Seconds remaining when phone starts ringing")]
        [SerializeField]
        private float triggerTime = 300f;


        [Tooltip("How long ringtone plays before voicemail")]
        [SerializeField]
        private float ringDuration = 10f;



        private float ringTimer;

        private bool eventStarted;

        private bool ringing;

        private bool messagePlaying;



        private void Awake()
        {
            if (timer == null)
                timer = FindAnyObjectByType<GameplayTimer>();


            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
        }



        private void OnEnable()
        {
            if (timer != null)
            {
                timer.SecondsRemainingChanged += TimerChanged;
            }
        }



        private void OnDisable()
        {
            if (timer != null)
            {
                timer.SecondsRemainingChanged -= TimerChanged;
            }
        }



        private void Update()
        {
            if (!ringing)
                return;


            ringTimer -= Time.deltaTime;


            if (ringTimer <= 0)
            {
                StartMessage();
            }
        }



        private void TimerChanged(float secondsRemaining)
        {
            if (eventStarted)
                return;


            if (secondsRemaining <= triggerTime)
            {
                StartPhoneCall();
            }
        }



        private void StartPhoneCall()
        {
            eventStarted = true;

            OpenPhone();

            PlayRingtone();


            ringing = true;
            ringTimer = ringDuration;
        }



        private void PlayRingtone()
        {
            audioSource.Stop();

            audioSource.loop = true;
            audioSource.clip = ringtone;

            audioSource.Play();
        }



        private void StartMessage()
        {
            ringing = false;
            messagePlaying = true;


            audioSource.Stop();

            audioSource.loop = true;
            audioSource.clip = message;

            audioSource.Play();
        }



        private void OpenPhone()
        {
            if (phoneRoot != null)
                phoneRoot.SetActive(true);
        }



        // Called when player closes the phone/message
        public void EndPhoneMessage()
        {
            if (!eventStarted)
                return;


            audioSource.Stop();


            ringing = false;
            messagePlaying = false;


            if (phoneRoot != null)
                phoneRoot.SetActive(false);
        }
    }
}