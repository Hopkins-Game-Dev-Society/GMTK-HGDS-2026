using BirthdayJobJam.Core;
using UnityEngine;

namespace BirthdayJobJam.Views
{
    public sealed class NavigationButton : MonoBehaviour
    {
        [SerializeField]
        private ViewDirection direction;


        [SerializeField]
        private GameplayViewStateMachine stateMachine;



        public void Navigate()
        {
            GameplayViewStateMachine machine =
                stateMachine != null
                ? stateMachine
                : Game.Ctx?.Views;



            if (machine == null)
            {
                Debug.LogWarning(
                    "NavigationButton: No state machine found.",
                    this
                );

                return;
            }



            GameplayView current =
                machine.GetCurrentView();



            if (current == null)
                return;



            ViewNavigation navigation =
                current.GetComponent<ViewNavigation>();



            if (navigation == null)
                return;



            if (navigation.TryGetDestination(
                direction,
                out GameViewId destination))
            {
                machine.SwitchTo(destination);
            }
        }
    }
}