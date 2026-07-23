using UnityEngine;
using UnityEngine.UI;

namespace BirthdayJobJam.Views
{
    public sealed class ViewNavigationUI : MonoBehaviour
    {
        [SerializeField]
        private GameplayViewStateMachine stateMachine;


        [Header("Arrow Buttons")]
        [SerializeField]
        private GameObject upArrow;

        [SerializeField]
        private GameObject downArrow;

        [SerializeField]
        private GameObject leftArrow;

        [SerializeField]
        private GameObject rightArrow;



        private void Start()
        {
            Refresh(
                GameViewId.None,
                stateMachine.CurrentView
            );
        }



        private void OnEnable()
        {
            if (stateMachine != null)
            {
                stateMachine.ViewChanged += Refresh;
            }
        }



        private void OnDisable()
        {
            if (stateMachine != null)
            {
                stateMachine.ViewChanged -= Refresh;
            }
        }




        private void Refresh(
            GameViewId previous,
            GameViewId current)
        {
            GameplayView view =
                stateMachine.GetCurrentView();



            if (view == null)
                return;



            ViewNavigation navigation =
                view.GetComponent<ViewNavigation>();



            if (navigation == null)
            {
                HideAll();
                return;
            }



            SetArrow(
                upArrow,
                navigation,
                ViewDirection.Up
            );


            SetArrow(
                downArrow,
                navigation,
                ViewDirection.Down
            );


            SetArrow(
                leftArrow,
                navigation,
                ViewDirection.Left
            );


            SetArrow(
                rightArrow,
                navigation,
                ViewDirection.Right
            );
        }



        private void SetArrow(
            GameObject arrow,
            ViewNavigation navigation,
            ViewDirection direction)
        {
            if (arrow == null)
                return;


            arrow.SetActive(
                navigation.TryGetDestination(
                    direction,
                    out _
                )
            );
        }



        private void HideAll()
        {
            upArrow?.SetActive(false);
            downArrow?.SetActive(false);
            leftArrow?.SetActive(false);
            rightArrow?.SetActive(false);
        }
    }
}