using BirthdayJobJam.Core;
using BirthdayJobJam.Views;
using UnityEngine;
using UnityEngine.EventSystems;

/*
* Attached to the virtual keyboard object itself, only attached once to the overall object not to each key. 
*/

/*
namespace BirthdayJobJam.UI
{
    public sealed class VirtualKeyboardController : MonoBehaviour
    {
        public static VirtualKeyboardController Instance { get; private set; }

        [SerializeField]
        private GameplayViewStateMachine stateMachine;

        [SerializeField]
        private GameViewId keyboardView = GameViewId.Keyboard;

        private VirtualKeyboardTarget currentTarget;

        private GameViewId previousView;

        public bool Shift { get; private set; }

        public bool CapsLock { get; private set; }

        public VirtualKeyboardTarget CurrentTarget => currentTarget;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (stateMachine == null)
                stateMachine = Game.Ctx?.Views;
        }

        public void SetTarget(VirtualKeyboardTarget target)
        {
            currentTarget = target;

            if (stateMachine != null)
            {
                previousView = stateMachine.CurrentView;
                stateMachine.SwitchTo(keyboardView);
            }
        }

        public void InsertCharacter(string value)
        {
            if (currentTarget == null)
                return;

            if (Shift ^ CapsLock)
                value = value.ToUpper();
            else
                value = value.ToLower();

            currentTarget.Insert(value);

            if (Shift)
                Shift = false;
        }

        public void InsertSpace()
        {
            currentTarget?.Insert(" ");
        }

        public void Backspace()
        {
            currentTarget?.Backspace();
        }

        public void ToggleShift()
        {
            Shift = !Shift;
        }

        public void ToggleCapsLock()
        {
            CapsLock = !CapsLock;
        }

        public void Finish()
        {
            if (currentTarget == null)
                return;

            currentTarget.EndEditing();
            currentTarget = null;

            if (stateMachine != null)
                stateMachine.SwitchTo(previousView);
        }
    }
} */

/*

namespace BirthdayJobJam.UI
{
    public sealed class VirtualKeyboardController : MonoBehaviour
    {
        public static VirtualKeyboardController Instance { get; private set; }

        [SerializeField] private GameplayViewStateMachine stateMachine;
        [SerializeField] private GameViewId keyboardView = GameViewId.Keyboard;

        private VirtualKeyboardTarget currentTarget;
        private GameViewId previousView;

        public bool Shift { get; private set; }
        public bool CapsLock { get; private set; }

        public VirtualKeyboardTarget CurrentTarget => currentTarget;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (stateMachine == null)
                stateMachine = Game.Ctx?.Views;
        }

        public void SetTarget(VirtualKeyboardTarget target)
        {
            currentTarget = target;

            if (stateMachine != null)
            {
                previousView = stateMachine.CurrentView;
            }
        }

        public void InsertCharacter(string value)
        {
            if (currentTarget == null)
                return;

            if (Shift ^ CapsLock)
                value = value.ToUpper();
            else
                value = value.ToLower();

            currentTarget.Insert(value);

            if (Shift)
                Shift = false;
        }

        public void InsertSpace()
        {
            currentTarget?.Insert(" ");
        }

        public void Backspace()
        {
            currentTarget?.Backspace();
        }

        public void ToggleShift()
        {
            Shift = !Shift;
        }

        public void ToggleCapsLock()
        {
            CapsLock = !CapsLock;
        }

        public void Finish()
        {
            if (currentTarget == null)
                return;

            currentTarget.EndEditing();
            currentTarget = null;

            if (stateMachine != null)
                stateMachine.SwitchTo(previousView);
        }
    }
} */
using BirthdayJobJam.Core;
using BirthdayJobJam.Views;
using UnityEngine;

namespace BirthdayJobJam.UI
{
    public sealed class VirtualKeyboardController : MonoBehaviour
    {
        public static VirtualKeyboardController Instance { get; private set; }


        [SerializeField]
        private GameplayViewStateMachine stateMachine;


        [SerializeField]
        private GameViewId keyboardView = GameViewId.Keyboard;

        public bool Shift { get; private set; }
        public bool CapsLock { get; private set; }




        private VirtualKeyboardTarget currentTarget;


        private GameViewId previousView;



        public bool HasActiveTarget =>
            currentTarget != null;



        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }


            Instance = this;


            if (stateMachine == null)
                stateMachine = Game.Ctx?.Views;
        }



        public void SetTarget(VirtualKeyboardTarget target)
        {
            if (currentTarget != null &&
                currentTarget != target)
            {
                currentTarget.Deselect();
            }


            currentTarget = target;


            if (stateMachine != null)
            {
                previousView = stateMachine.CurrentView;
            }
        }

/*
        public void NotifyKeyboardClicked()
        {
            // Intentionally empty.
            // This exists so clicking keyboard keys does not clear the active input target.
        } */

        public void InsertSpace()
        {
            currentTarget?.Insert(" ");
        }

        public void Backspace()
        {
            currentTarget?.Backspace();
        }

        public void ToggleShift()
        {
            Shift = !Shift;
        }

        public void ToggleCapsLock()
        {
            CapsLock = !CapsLock;
        }




        public void ClearTarget()
        {
            if (currentTarget != null)
            {
                currentTarget.Deselect();
            }


            currentTarget = null;
        }

        private void OnDisable()
{
    Debug.Log("VirtualKeyboardController disabled");
}


private void OnEnable()
{
    Debug.Log("VirtualKeyboardController enabled");
}



        public void InsertCharacter(string value)
        {
            if (currentTarget == null)
                return;


            currentTarget.Insert(value);
        }

        public void Finish()
        {
            if (currentTarget == null)
                return;


            ClearTarget();


            if (stateMachine != null)
                stateMachine.SwitchTo(previousView);
        }
    }
}