using System;
using System.Collections.Generic;
using BirthdayJobJam.Core;
using UnityEngine;

namespace BirthdayJobJam.Application
{
    //I turn the selected clues into stable location assignments and leave the actual objects to another system.
    [DisallowMultipleComponent]
    public sealed class RandomizedPlacementManager : MonoBehaviour
    {
        private const string ComputerFeatureGroup = "computer_feature";
        private const string DeskItemGroup = "desk_item";

        [Header("Session")]
        [SerializeField] private ApplicationSessionManager sessionManager;

        [Header("Locations")]
        [SerializeField]
        private List<PlacementLocationDefinition> locations =
            CreateDefaultLocations();

        [Header("Placeable Items")]
        [SerializeField]
        private List<PlaceableItemDefinition> items =
            CreateDefaultItems();

        [Header("Runtime State (Inspector Only)")]
        [SerializeField] private string currentSessionId;
        [SerializeField] private int currentPlacementSeed;
        [SerializeField] private List<PlacementAssignment> currentAssignments = new();
        [SerializeField] private List<string> unassignedRequestIds = new();

        private ApplicationSessionManager subscribedSessionManager;

        public event Action<IReadOnlyList<PlacementAssignment>> AssignmentsGenerated;

        public IReadOnlyList<PlacementAssignment> Assignments =>
            currentAssignments != null
                ? currentAssignments
                : Array.Empty<PlacementAssignment>();
        public IReadOnlyList<string> UnassignedRequestIds =>
            unassignedRequestIds != null
                ? unassignedRequestIds
                : Array.Empty<string>();
        public bool HasAssignments =>
            currentAssignments != null && currentAssignments.Count > 0;
        public string CurrentSessionId => currentSessionId;
        public int CurrentPlacementSeed => currentPlacementSeed;

        private void Awake()
        {
            ResolveSessionManager();
        }

        private void Reset()
        {
            UseDefaultLogicalLayout();
            ResolveSessionManager();
        }

        private void OnEnable()
        {
            BindSessionManager();
        }

        private void Start()
        {
            BindSessionManager();

            if (sessionManager != null && sessionManager.Current != null)
                GenerateAssignments(sessionManager.Current, force: false);
        }

        private void OnDisable()
        {
            UnbindSessionManager();
        }

        [ContextMenu("Placement/Generate From Current Session")]
        public void GenerateFromCurrentSession()
        {
            BindSessionManager();

            if (sessionManager == null || sessionManager.Current == null)
            {
                Debug.LogWarning(
                    "RandomizedPlacementManager: no application session is available.",
                    this);
                return;
            }

            GenerateAssignments(sessionManager.Current, force: true);
        }

        public PlacementAssignment FindAssignment(string assignmentId)
        {
            if (string.IsNullOrWhiteSpace(assignmentId)
                || currentAssignments == null)
            {
                return null;
            }

            for (int i = 0; i < currentAssignments.Count; i++)
            {
                PlacementAssignment assignment = currentAssignments[i];
                if (assignment != null
                    && string.Equals(
                        assignment.AssignmentId,
                        assignmentId,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return assignment;
                }
            }

            return null;
        }

        public PlacementAssignment FindAssignmentForItem(string itemId)
        {
            return FindAssignment(itemId);
        }

        public PlacementAssignment FindAssignmentForClue(string clueId)
        {
            if (string.IsNullOrWhiteSpace(clueId)
                || currentAssignments == null)
            {
                return null;
            }

            for (int i = 0; i < currentAssignments.Count; i++)
            {
                PlacementAssignment assignment = currentAssignments[i];
                if (assignment != null && assignment.ContainsClue(clueId))
                    return assignment;
            }

            return null;
        }

        private void HandleSessionGenerated(ApplicationSessionData session)
        {
            GenerateAssignments(session, force: false);
        }

        private void GenerateAssignments(
            ApplicationSessionData session,
            bool force)
        {
            if (session == null)
                return;

            //I only build this once for a session, so changing views cannot reroll the locations.
            if (!force
                && string.Equals(
                    currentSessionId,
                    session.SessionId,
                    StringComparison.Ordinal))
            {
                return;
            }

            currentSessionId = session.SessionId;
            currentPlacementSeed = CreatePlacementSeed(session.Seed);

            List<PlacementRequest> requests =
                BuildPlacementRequests(session.Clues);
            PlacementRandomizationResult result =
                PlacementAssignmentRandomizer.Generate(
                    currentPlacementSeed,
                    requests,
                    locations);

            currentAssignments = result.Assignments;
            unassignedRequestIds = result.UnassignedRequestIds;

            //The environment teammate can read the list or subscribe here without this script spawning anything.
            AssignmentsGenerated?.Invoke(currentAssignments);
        }

        private List<PlacementRequest> BuildPlacementRequests(
            IReadOnlyList<ApplicationClueRuntimeData> clues)
        {
            List<PlacementRequest> result = new();
            Dictionary<string, PlaceableItemDefinition> itemsById =
                new(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, List<string>> requestedClueIdsByItem =
                new(StringComparer.OrdinalIgnoreCase);

            if (items == null)
                return result;

            for (int i = 0; i < items.Count; i++)
            {
                PlaceableItemDefinition item = items[i];
                if (item == null
                    || !item.IsValid
                    || itemsById.ContainsKey(item.ItemId))
                {
                    continue;
                }

                itemsById.Add(item.ItemId, item);

                if (item.PlaceEveryRun)
                    requestedClueIdsByItem.Add(item.ItemId, new List<string>());
            }

            if (clues != null)
            {
                for (int i = 0; i < clues.Count; i++)
                {
                    ApplicationClueRuntimeData clue = clues[i];
                    if (clue == null)
                        continue;

                    PlaceableItemDefinition item =
                        FindItemForClue(clue, itemsById);
                    if (item == null)
                        continue;

                    if (!requestedClueIdsByItem.TryGetValue(
                        item.ItemId,
                        out List<string> clueIds))
                    {
                        clueIds = new List<string>();
                        requestedClueIdsByItem.Add(item.ItemId, clueIds);
                    }

                    if (!string.IsNullOrWhiteSpace(clue.ClueId)
                        && !ContainsIgnoringCase(clueIds, clue.ClueId))
                    {
                        clueIds.Add(clue.ClueId);
                    }
                }
            }

            List<string> requestedItemIds =
                new(requestedClueIdsByItem.Keys);
            requestedItemIds.Sort(StringComparer.Ordinal);

            for (int i = 0; i < requestedItemIds.Count; i++)
            {
                string itemId = requestedItemIds[i];
                List<string> clueIds = requestedClueIdsByItem[itemId];
                clueIds.Sort(StringComparer.Ordinal);
                result.Add(new PlacementRequest(
                    itemsById[itemId],
                    clueIds));
            }

            return result;
        }

        private static PlaceableItemDefinition FindItemForClue(
            ApplicationClueRuntimeData clue,
            IReadOnlyDictionary<string, PlaceableItemDefinition> itemsById)
        {
            if (!string.IsNullOrWhiteSpace(clue.VariantId)
                && itemsById.TryGetValue(
                    clue.VariantId.Trim(),
                    out PlaceableItemDefinition variantItem))
            {
                return variantItem;
            }

            if (!string.IsNullOrWhiteSpace(clue.ClueId)
                && itemsById.TryGetValue(
                    clue.ClueId.Trim(),
                    out PlaceableItemDefinition clueItem))
            {
                return clueItem;
            }

            return null;
        }

        private static bool ContainsIgnoringCase(
            IReadOnlyList<string> values,
            string candidate)
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (string.Equals(
                    values[i],
                    candidate,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        [ContextMenu("Placement/Use Default Logical Layout")]
        private void UseDefaultLogicalLayout()
        {
            locations = CreateDefaultLocations();
            items = CreateDefaultItems();
        }

        private static List<PlacementLocationDefinition> CreateDefaultLocations()
        {
            List<PlacementLocationDefinition> result = new()
            {
                new PlacementLocationDefinition(
                    "computer_1",
                    Views.GameViewId.Computer,
                    new[] { ComputerFeatureGroup })
            };

            AddNumberedLocations(
                result,
                "left_desk",
                Views.GameViewId.LeftDesk);
            AddNumberedLocations(
                result,
                "right_desk",
                Views.GameViewId.RightDesk);
            AddNumberedLocations(
                result,
                "drawer",
                Views.GameViewId.DeskDrawer);

            return result;
        }

        private static void AddNumberedLocations(
            List<PlacementLocationDefinition> destination,
            string idPrefix,
            Views.GameViewId viewId)
        {
            for (int i = 1; i <= 3; i++)
            {
                destination.Add(new PlacementLocationDefinition(
                    $"{idPrefix}_{i}",
                    viewId,
                    new[] { DeskItemGroup }));
            }
        }

        private static List<PlaceableItemDefinition> CreateDefaultItems()
        {
            return new List<PlaceableItemDefinition>
            {
                new("window", ComputerFeatureGroup, placeEveryRun: false),
                new("clocks", ComputerFeatureGroup, placeEveryRun: false),
                new("post_it_notes", DeskItemGroup, placeEveryRun: true),
                new("family_picture", DeskItemGroup, placeEveryRun: true),
                new("junk", DeskItemGroup, placeEveryRun: true),
                new("phone", DeskItemGroup, placeEveryRun: true),
                new("wallet", DeskItemGroup, placeEveryRun: true)
            };
        }

        private void ResolveSessionManager()
        {
            if (sessionManager == null)
                sessionManager = GetComponent<ApplicationSessionManager>();

            if (sessionManager == null)
                sessionManager = GetComponentInChildren<ApplicationSessionManager>(includeInactive: true);

            if (sessionManager == null && Game.Ctx != null)
                sessionManager = Game.Ctx.ApplicationSession;

            if (sessionManager == null)
                sessionManager = FindAnyObjectByType<ApplicationSessionManager>();
        }

        private void BindSessionManager()
        {
            ResolveSessionManager();

            if (subscribedSessionManager == sessionManager)
                return;

            UnbindSessionManager();
            subscribedSessionManager = sessionManager;

            if (subscribedSessionManager != null)
                subscribedSessionManager.SessionGenerated += HandleSessionGenerated;
        }

        private void UnbindSessionManager()
        {
            if (subscribedSessionManager != null)
            {
                subscribedSessionManager.SessionGenerated -=
                    HandleSessionGenerated;
            }

            subscribedSessionManager = null;
        }

        private static int CreatePlacementSeed(int sessionSeed)
        {
            //I use a separate seed so changing the location pool cannot change the applicant or questions.
            return unchecked((sessionSeed * 486187739) ^ 0x5F3759DF);
        }
    }
}
