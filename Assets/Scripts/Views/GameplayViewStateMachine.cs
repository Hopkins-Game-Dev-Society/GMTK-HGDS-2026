using System;
using System.Collections.Generic;
using BirthdayJobJam.Events;
using UnityEngine;

namespace BirthdayJobJam.Views
{
    public sealed class GameplayViewStateMachine : MonoBehaviour
    {
        [SerializeField] private GameViewId initialView = GameViewId.Computer;
        [SerializeField] private List<GameplayView> views = new List<GameplayView>();

        [Header("Events")]
        [SerializeField] private StringGameEvent viewChanged;

        private readonly Dictionary<GameViewId, GameplayView> viewMap = new Dictionary<GameViewId, GameplayView>();

        public event Action<GameViewId, GameViewId> ViewChanged;

        public GameViewId CurrentView { get; private set; } = GameViewId.None;

        private void Awake()
        {
            if (views.Count == 0)
                GetComponentsInChildren(includeInactive: true, views);

            BuildViewMap();
            HideAllViews();
        }

        private void Start()
        {
            if (initialView != GameViewId.None)
                SwitchTo(initialView);
        }

        private void Reset()
        {
            views.Clear();
            GetComponentsInChildren(includeInactive: true, views);
        }

        public void SwitchToComputer() => SwitchTo(GameViewId.Computer);
        public void SwitchToKeyboard() => SwitchTo(GameViewId.Keyboard);
        public void SwitchToDeskDrawer() => SwitchTo(GameViewId.DeskDrawer);
        public void SwitchToStickyNotes() => SwitchTo(GameViewId.StickyNotes);
        public void SwitchToDoor() => SwitchTo(GameViewId.Door);
        public void SwitchToApplication() => SwitchTo(GameViewId.Application);

        public void SwitchTo(GameViewId targetView)
        {
            if (targetView == GameViewId.None || targetView == CurrentView)
                return;

            if (!viewMap.TryGetValue(targetView, out GameplayView nextView) || nextView == null)
            {
                Debug.LogWarning($"GameplayViewStateMachine: no view registered for {targetView}.", this);
                return;
            }

            GameViewId previousView = CurrentView;

            if (viewMap.TryGetValue(CurrentView, out GameplayView currentView) && currentView != null)
                currentView.Hide();

            CurrentView = targetView;
            nextView.Show();

            ViewChanged?.Invoke(previousView, CurrentView);
            viewChanged?.Raise(CurrentView.ToString());
        }

        public void RegisterView(GameplayView view)
        {
            if (view == null)
                return;

            if (!views.Contains(view))
                views.Add(view);

            viewMap[view.ViewId] = view;
        }

        private void BuildViewMap()
        {
            viewMap.Clear();

            foreach (GameplayView view in views)
            {
                if (view == null || view.ViewId == GameViewId.None)
                    continue;

                if (viewMap.ContainsKey(view.ViewId))
                    Debug.LogWarning($"Duplicate gameplay view id registered: {view.ViewId}", view);

                viewMap[view.ViewId] = view;
            }
        }

        private void HideAllViews()
        {
            foreach (GameplayView view in views)
            {
                if (view != null)
                    view.SetVisibleSilently(false);
            }
        }
    }
}
