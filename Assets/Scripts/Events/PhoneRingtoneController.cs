using UnityEngine;

namespace BirthdayJobJam.Player
{
    public sealed class PhoneRingtoneController : MonoBehaviour
    {
        public static PhoneRingtoneController Instance { get; private set; }


        [SerializeField]
        private AudioSource ringtoneSource;


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;


            if (ringtoneSource == null)
            {
                ringtoneSource = GetComponent<AudioSource>();
            }
        }


        public void StartRinging()
        {
            if (ringtoneSource == null)
                return;


            if (!ringtoneSource.isPlaying)
            {
                ringtoneSource.loop = true;
                ringtoneSource.Play();
            }
        }


        public void StopRinging()
        {
            if (ringtoneSource == null)
                return;


            if (ringtoneSource.isPlaying)
            {
                ringtoneSource.Stop();
            }
        }
    }
}