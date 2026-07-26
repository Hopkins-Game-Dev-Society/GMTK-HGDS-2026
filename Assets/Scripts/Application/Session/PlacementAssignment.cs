using System;
using System.Collections.Generic;
using BirthdayJobJam.Views;
using UnityEngine;

namespace BirthdayJobJam.Application
{
    [Serializable]
    public sealed class PlaceableItemDefinition
    {
        [SerializeField] private string itemId;
        [SerializeField] private string placementGroupId;
        [SerializeField] private GameObject prefab;
        [SerializeField] private bool placeEveryRun;

        public PlaceableItemDefinition(
            string itemId,
            string placementGroupId,
            bool placeEveryRun,
            GameObject prefab = null)
        {
            this.itemId = itemId;
            this.placementGroupId = placementGroupId;
            this.placeEveryRun = placeEveryRun;
            this.prefab = prefab;
        }

        public string ItemId => itemId?.Trim();
        public string PlacementGroupId => placementGroupId?.Trim();
        public GameObject Prefab => prefab;
        public bool PlaceEveryRun => placeEveryRun;
        public bool IsValid =>
            !string.IsNullOrWhiteSpace(itemId)
            && !string.IsNullOrWhiteSpace(placementGroupId);
    }

    [Serializable]
    public sealed class PlacementLocationDefinition
    {
        [SerializeField] private string locationId;
        [SerializeField] private GameViewId viewId;
        [SerializeField] private List<string> acceptedPlacementGroupIds = new();

        public PlacementLocationDefinition(
            string locationId,
            GameViewId viewId,
            IEnumerable<string> acceptedPlacementGroupIds)
        {
            this.locationId = locationId;
            this.viewId = viewId;
            this.acceptedPlacementGroupIds = new List<string>();

            if (acceptedPlacementGroupIds == null)
                return;

            foreach (string placementGroupId in acceptedPlacementGroupIds)
            {
                if (string.IsNullOrWhiteSpace(placementGroupId)
                    || ContainsIgnoringCase(
                        this.acceptedPlacementGroupIds,
                        placementGroupId))
                {
                    continue;
                }

                this.acceptedPlacementGroupIds.Add(placementGroupId.Trim());
            }
        }

        public string LocationId => locationId?.Trim();
        public GameViewId ViewId => viewId;
        public IReadOnlyList<string> AcceptedPlacementGroupIds =>
            acceptedPlacementGroupIds != null
                ? (IReadOnlyList<string>)acceptedPlacementGroupIds
                : Array.Empty<string>();
        public bool IsValid => !string.IsNullOrWhiteSpace(locationId);

        public bool Accepts(string placementGroupId)
        {
            if (string.IsNullOrWhiteSpace(placementGroupId)
                || acceptedPlacementGroupIds == null)
            {
                return false;
            }

            for (int i = 0; i < acceptedPlacementGroupIds.Count; i++)
            {
                string acceptedGroup = acceptedPlacementGroupIds[i]?.Trim();
                if (acceptedGroup == "*"
                    || string.Equals(
                        acceptedGroup,
                        placementGroupId,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
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
    }

    //This is only the result of the roll. It never spawns or displays anything.
    [Serializable]
    public sealed class PlacementAssignment
    {
        [SerializeField] private string itemId;
        [SerializeField] private string placementGroupId;
        [SerializeField] private GameObject prefab;
        [SerializeField] private string locationId;
        [SerializeField] private GameViewId viewId;
        [SerializeField] private List<string> clueIds = new();

        internal PlacementAssignment(
            PlacementRequest request,
            PlacementLocationDefinition location,
            IReadOnlyList<string> clueIds)
        {
            itemId = request.ItemId;
            placementGroupId = request.PlacementGroupId;
            prefab = request.Prefab;
            locationId = location.LocationId;
            viewId = location.ViewId;
            this.clueIds = clueIds != null
                ? new List<string>(clueIds)
                : new List<string>();
        }

        public string AssignmentId => itemId;
        public string ItemId => itemId;
        public string PlacementGroupId => placementGroupId;
        public GameObject Prefab => prefab;
        public string LocationId => locationId;
        public GameViewId ViewId => viewId;
        public IReadOnlyList<string> ClueIds =>
            clueIds != null
                ? (IReadOnlyList<string>)clueIds
                : Array.Empty<string>();

        public bool ContainsClue(string clueId)
        {
            if (string.IsNullOrWhiteSpace(clueId) || clueIds == null)
                return false;

            for (int i = 0; i < clueIds.Count; i++)
            {
                if (string.Equals(
                    clueIds[i],
                    clueId,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }

    internal sealed class PlacementRequest
    {
        internal PlacementRequest(
            PlaceableItemDefinition item,
            IReadOnlyList<string> clueIds)
        {
            ItemId = item.ItemId;
            PlacementGroupId = item.PlacementGroupId;
            Prefab = item.Prefab;
            ClueIds = clueIds != null
                ? new List<string>(clueIds)
                : new List<string>();
        }

        internal string AssignmentId => ItemId;
        internal string ItemId { get; }
        internal string PlacementGroupId { get; }
        internal GameObject Prefab { get; }
        internal IReadOnlyList<string> ClueIds { get; }
        internal bool IsValid =>
            !string.IsNullOrWhiteSpace(AssignmentId)
            && !string.IsNullOrWhiteSpace(ItemId)
            && !string.IsNullOrWhiteSpace(PlacementGroupId);
    }

    internal sealed class PlacementRandomizationResult
    {
        internal PlacementRandomizationResult(
            List<PlacementAssignment> assignments,
            List<string> unassignedRequestIds)
        {
            Assignments = assignments;
            UnassignedRequestIds = unassignedRequestIds;
        }

        internal List<PlacementAssignment> Assignments { get; }
        internal List<string> UnassignedRequestIds { get; }
    }

    internal static class PlacementAssignmentRandomizer
    {
        internal static PlacementRandomizationResult Generate(
            int seed,
            IReadOnlyList<PlacementRequest> requests,
            IReadOnlyList<PlacementLocationDefinition> locations)
        {
            List<PlacementLocationDefinition> validLocations =
                GetUniqueValidLocations(locations);
            List<PlacementRequest> validRequests = GetUniqueValidRequests(
                requests,
                out List<string> unassignedRequestIds);

            //I assign the most restricted groups first so flexible groups cannot take their only valid spot.
            validRequests.Sort((left, right) =>
            {
                int comparison = CountCompatibleLocations(left, validLocations)
                    .CompareTo(CountCompatibleLocations(right, validLocations));

                return comparison != 0
                    ? comparison
                    : string.Compare(
                        left.AssignmentId,
                        right.AssignmentId,
                        StringComparison.Ordinal);
            });

            System.Random random = new(seed);
            HashSet<string> usedLocationIds =
                new(StringComparer.OrdinalIgnoreCase);
            List<PlacementAssignment> assignments = new();

            for (int i = 0; i < validRequests.Count; i++)
            {
                PlacementRequest request = validRequests[i];
                List<PlacementLocationDefinition> candidates =
                    GetAvailableCandidates(
                        request,
                        validLocations,
                        usedLocationIds);

                if (candidates.Count == 0)
                {
                    unassignedRequestIds.Add(request.AssignmentId);
                    continue;
                }

                PlacementLocationDefinition selected =
                    candidates[random.Next(candidates.Count)];
                usedLocationIds.Add(selected.LocationId);
                assignments.Add(new PlacementAssignment(
                    request,
                    selected,
                    request.ClueIds));
            }

            assignments.Sort((left, right) =>
                string.Compare(
                    left.AssignmentId,
                    right.AssignmentId,
                    StringComparison.Ordinal));
            unassignedRequestIds.Sort(StringComparer.Ordinal);

            return new PlacementRandomizationResult(
                assignments,
                unassignedRequestIds);
        }

        private static List<PlacementLocationDefinition> GetUniqueValidLocations(
            IReadOnlyList<PlacementLocationDefinition> locations)
        {
            List<PlacementLocationDefinition> result = new();
            HashSet<string> seenLocationIds =
                new(StringComparer.OrdinalIgnoreCase);

            if (locations == null)
                return result;

            for (int i = 0; i < locations.Count; i++)
            {
                PlacementLocationDefinition location = locations[i];
                if (location == null
                    || !location.IsValid
                    || !seenLocationIds.Add(location.LocationId))
                {
                    continue;
                }

                result.Add(location);
            }

            result.Sort((left, right) =>
                string.Compare(
                    left.LocationId,
                    right.LocationId,
                    StringComparison.Ordinal));
            return result;
        }

        private static List<PlacementRequest> GetUniqueValidRequests(
            IReadOnlyList<PlacementRequest> requests,
            out List<string> invalidRequestIds)
        {
            List<PlacementRequest> result = new();
            invalidRequestIds = new List<string>();
            HashSet<string> seenAssignmentIds =
                new(StringComparer.OrdinalIgnoreCase);

            if (requests == null)
                return result;

            for (int i = 0; i < requests.Count; i++)
            {
                PlacementRequest request = requests[i];
                string requestId = request?.AssignmentId;

                if (request == null
                    || !request.IsValid
                    || !seenAssignmentIds.Add(requestId))
                {
                    invalidRequestIds.Add(
                        string.IsNullOrWhiteSpace(requestId)
                            ? $"invalid_request_{i}"
                            : requestId);
                    continue;
                }

                result.Add(request);
            }

            return result;
        }

        private static int CountCompatibleLocations(
            PlacementRequest request,
            IReadOnlyList<PlacementLocationDefinition> locations)
        {
            int count = 0;

            for (int i = 0; i < locations.Count; i++)
            {
                if (locations[i].Accepts(request.PlacementGroupId))
                    count++;
            }

            return count;
        }

        private static List<PlacementLocationDefinition> GetAvailableCandidates(
            PlacementRequest request,
            IReadOnlyList<PlacementLocationDefinition> locations,
            HashSet<string> usedLocationIds)
        {
            List<PlacementLocationDefinition> result = new();

            for (int i = 0; i < locations.Count; i++)
            {
                PlacementLocationDefinition location = locations[i];
                if (!usedLocationIds.Contains(location.LocationId)
                    && location.Accepts(request.PlacementGroupId))
                {
                    result.Add(location);
                }
            }

            return result;
        }
    }
}
