using BirthdayJobJam.Core;
using UnityEngine;

namespace BirthdayJobJam.Views
{
    public sealed class SwitchViewButton : MonoBehaviour
    {
        [SerializeField] private GameplayViewStateMachine stateMachine;
        [SerializeField] private GameViewId targetView;

        public void Switch()
        {
            GameplayViewStateMachine machine = stateMachine != null
                ? stateMachine
                : Game.Ctx != null
                    ? Game.Ctx.Views
                    : null;

            if (machine == null)
            {
                Debug.LogWarning("SwitchViewButton: no GameplayViewStateMachine found.", this);
                return;
            }

            machine.SwitchTo(targetView);
        }
    }
}
