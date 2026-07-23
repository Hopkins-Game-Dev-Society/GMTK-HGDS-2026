using BirthdayJobJam.Application;
using BirthdayJobJam.Views;
using UnityEngine;

namespace BirthdayJobJam.Core
{
    public sealed class GameContext : MonoBehaviour
    {
        public static GameContext Instance => Game.Ctx;

        [Header("Lifetime")]
        [SerializeField] private bool persistAcrossScenes;

        [Header("Scene Systems")]
        [SerializeField] private GameplayTimer timer;
        [SerializeField] private GameplayViewStateMachine views;
        [SerializeField] private ApplicationScoreManager score;

        public GameplayTimer Timer => timer;
        public GameplayViewStateMachine Views => views;
        public ApplicationScoreManager Score => score;

        private void Awake()
        {
            if (Game.IsReady && Game.Ctx != this)
            {
                Debug.LogWarning("Duplicate GameContext found. Destroying the newer instance.", this);
                Destroy(gameObject);
                return;
            }

            if (persistAcrossScenes)
                DontDestroyOnLoad(gameObject);

            ResolveSceneSystems();
            Game.SetContext(this);
        }

        private void OnDestroy()
        {
            Game.ClearContext(this);
        }

        private void Reset()
        {
            ResolveSceneSystems();
        }

        private void ResolveSceneSystems()
        {
            if (timer == null)
                timer = GetComponentInChildren<GameplayTimer>(includeInactive: true);

            if (views == null)
                views = GetComponentInChildren<GameplayViewStateMachine>(includeInactive: true);

            if (score == null)
                score = GetComponentInChildren<ApplicationScoreManager>(includeInactive: true);
        }
    }
}
