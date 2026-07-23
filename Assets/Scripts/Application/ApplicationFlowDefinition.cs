using System.Collections.Generic;
using UnityEngine;

namespace BirthdayJobJam.Application
{
    [CreateAssetMenu(fileName = "ApplicationFlow_", menuName = "Birthday Job Jam/Application/Flow Definition")]
    public sealed class ApplicationFlowDefinition : ScriptableObject
    {
        [Header("Flow")]
        [SerializeField] private ApplicationSectionId initialSection = ApplicationSectionId.CreateAccountSignIn;
        [SerializeField] private List<ApplicationSectionDefinition> sections = new List<ApplicationSectionDefinition>();

        [Header("Fallback Error Behaviour")]
        [SerializeField] private string defaultErrorMessage = "An error occurred. Please refresh the page to continue.";
        [SerializeField, Min(0f)] private float fallbackRefreshCooldownSeconds = 5f;

        public ApplicationSectionId InitialSection => initialSection;
        public IReadOnlyList<ApplicationSectionDefinition> Sections => sections;
        public string DefaultErrorMessage => defaultErrorMessage;
        public float FallbackRefreshCooldownSeconds => fallbackRefreshCooldownSeconds;

        private void Reset()
        {
            UseDefaultJamSections();
        }

        [ContextMenu("Use Default Jam Sections")]
        public void UseDefaultJamSections()
        {
            initialSection = ApplicationSectionId.CreateAccountSignIn;
            sections = DefaultApplicationSections.Create();
        }
    }
}
