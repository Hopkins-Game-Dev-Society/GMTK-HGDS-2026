#if UNITY_EDITOR
using BirthdayJobJam.Application;
using BirthdayJobJam.Core;
using BirthdayJobJam.Views;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BirthdayJobJam.Editor
{
    public static class CoreSceneRigBuilder
    {
        private const string RootName = "Game Systems";
        private const string ViewsRootName = "Views";

        [MenuItem("Birthday Job Jam/Create Core Scene Rig")]
        public static void CreateCoreSceneRig()
        {
            GameObject root = GameObject.Find(RootName);
            if (root == null)
            {
                root = new GameObject(RootName);
                Undo.RegisterCreatedObjectUndo(root, "Create Game Systems");
            }

            GameContext context = GetOrAdd<GameContext>(root);

            GameplayTimer timer = GetOrCreateChildComponent<GameplayTimer>(root.transform, "Timer");
            ApplicationScoreManager score = GetOrCreateChildComponent<ApplicationScoreManager>(root.transform, "Application Score");
            GameplayViewStateMachine views = GetOrCreateChildComponent<GameplayViewStateMachine>(root.transform, ViewsRootName);
            TimerExpiryEndingDriver endingDriver = GetOrCreateChildComponent<TimerExpiryEndingDriver>(root.transform, "Ending Driver");

            ConfigureContext(context, timer, score, views);
            ConfigureEndingDriver(endingDriver, timer, score);
            CreateDefaultViews(views.transform);

            EditorUtility.SetDirty(context);
            EditorUtility.SetDirty(timer);
            EditorUtility.SetDirty(score);
            EditorUtility.SetDirty(views);
            EditorUtility.SetDirty(endingDriver);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            Selection.activeGameObject = root;
            Debug.Log("Created Birthday Job Jam core scene rig.", root);
        }

        private static void ConfigureContext(
            GameContext context,
            GameplayTimer timer,
            ApplicationScoreManager score,
            GameplayViewStateMachine views)
        {
            SerializedObject serialized = new(context);
            serialized.FindProperty("timer").objectReferenceValue = timer;
            serialized.FindProperty("score").objectReferenceValue = score;
            serialized.FindProperty("views").objectReferenceValue = views;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConfigureEndingDriver(
            TimerExpiryEndingDriver endingDriver,
            GameplayTimer timer,
            ApplicationScoreManager score)
        {
            SerializedObject serialized = new(endingDriver);
            serialized.FindProperty("timer").objectReferenceValue = timer;
            serialized.FindProperty("scoreManager").objectReferenceValue = score;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void CreateDefaultViews(Transform viewsRoot)
        {
            CreateViewIfMissing(viewsRoot, GameViewId.Computer, "View - Computer");
            CreateViewIfMissing(viewsRoot, GameViewId.Keyboard, "View - Keyboard");
            CreateViewIfMissing(viewsRoot, GameViewId.DeskDrawer, "View - Desk Drawer");
            CreateViewIfMissing(viewsRoot, GameViewId.StickyNotes, "View - Sticky Notes");
            CreateViewIfMissing(viewsRoot, GameViewId.Application, "View - Application");
        }

        private static void CreateViewIfMissing(Transform parent, GameViewId viewId, string objectName)
        {
            Transform existing = parent.Find(objectName);
            if (existing != null)
                return;

            GameObject viewObject = new(objectName);
            Undo.RegisterCreatedObjectUndo(viewObject, $"Create {objectName}");
            viewObject.transform.SetParent(parent, worldPositionStays: false);

            GameplayView view = viewObject.AddComponent<GameplayView>();
            SerializedObject serialized = new(view);
            serialized.FindProperty("viewId").intValue = (int)viewId;
            serialized.FindProperty("root").objectReferenceValue = viewObject;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static T GetOrCreateChildComponent<T>(Transform parent, string childName) where T : Component
        {
            Transform child = parent.Find(childName);
            if (child == null)
            {
                GameObject childObject = new(childName);
                Undo.RegisterCreatedObjectUndo(childObject, $"Create {childName}");
                childObject.transform.SetParent(parent, worldPositionStays: false);
                child = childObject.transform;
            }

            return GetOrAdd<T>(child.gameObject);
        }

        private static T GetOrAdd<T>(GameObject target) where T : Component
        {
            T component = target.GetComponent<T>();
            if (component != null)
                return component;

            component = Undo.AddComponent<T>(target);
            return component;
        }
    }
}
#endif
