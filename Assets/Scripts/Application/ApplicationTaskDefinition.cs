using UnityEngine;

namespace BirthdayJobJam.Application
{
    [CreateAssetMenu(fileName = "ApplicationTask_", menuName = "Birthday Job Jam/Application/Task Definition")]
    public sealed class ApplicationTaskDefinition : ScriptableObject
    {
        [SerializeField] private string taskId;
        [SerializeField] private string displayName;
        [SerializeField, TextArea] private string notes;
        [SerializeField, Range(1, 10)] private int maxPoints = 10;

        public string TaskId => string.IsNullOrWhiteSpace(taskId) ? name : taskId;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public string Notes => notes;
        public int MaxPoints => maxPoints;
    }
}
