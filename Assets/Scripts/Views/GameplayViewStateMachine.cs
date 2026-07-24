using System;
using System.Collections.Generic;
using BirthdayJobJam.Events;
using UnityEngine;

//So here the plan is to make it so that the currently active view determines the camera that's active, that can also then
//impact what elements of the UI are currently active as well as what each of them does.

//States openly visible for use in other contexts such as UI (so you can ask for current state and be sent it)

namespace BirthdayJobJam.Views
{
    public sealed class GameplayViewStateMachine : MonoBehaviour
    {
        [SerializeField] private GameViewId initialView = GameViewId.Computer;

        [SerializeField]
        private List<GameplayView> views = new();


        [Header("Events")]
        [SerializeField] private StringGameEvent viewChanged;


        private readonly Dictionary<GameViewId, GameplayView> viewMap = new();


        public event Action<GameViewId, GameViewId> ViewChanged;


        public GameViewId CurrentView { get; private set; } = GameViewId.None;



        private void Awake()
        {
            if (views.Count == 0)
            {
                GetComponentsInChildren(
                    includeInactive: true,
                    views
                );
            }


            BuildViewMap();

            DeactivateAllViews();
        }



        private void Start()
        {
            if (initialView != GameViewId.None)
                SwitchTo(initialView);
        }



        private void Reset()
        {
            views.Clear();

            GetComponentsInChildren(
                includeInactive: true,
                views
            );
        }



        public void SwitchTo(GameViewId targetView)
        {
            if (targetView == GameViewId.None)
                return;


            if (targetView == CurrentView)
                return;



            if (!viewMap.TryGetValue(targetView, out GameplayView nextView))
            {
                Debug.LogWarning(
                    $"GameplayViewStateMachine: No view registered for {targetView}.",
                    this
                );

                return;
            }



            GameViewId previousView = CurrentView;



            if (viewMap.TryGetValue(CurrentView, out GameplayView currentView))
            {
                currentView.Deactivate();
            }



            CurrentView = targetView;


            nextView.Activate();



            ViewChanged?.Invoke(previousView, CurrentView);

            viewChanged?.Raise(CurrentView.ToString());
        }




        public GameplayView GetCurrentView()
        {
            if (viewMap.TryGetValue(CurrentView, out GameplayView view))
                return view;


            return null;
        }

        public Camera CurrentCamera
        {
            get
            {
                GameplayView view = GetCurrentView();

                return view != null
                    ? view.ViewCamera
                    : null;
            }
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
                if (view == null)
                    continue;


                if (view.ViewId == GameViewId.None)
                    continue;



                if (viewMap.ContainsKey(view.ViewId))
                {
                    Debug.LogWarning(
                        $"Duplicate GameplayView ID: {view.ViewId}",
                        view
                    );
                }


                viewMap[view.ViewId] = view;
            }
        }





        private void DeactivateAllViews()
        {
            foreach (GameplayView view in views)
            {
                if (view != null)
                    view.DeactivateSilently();
            }
        }
    }
}