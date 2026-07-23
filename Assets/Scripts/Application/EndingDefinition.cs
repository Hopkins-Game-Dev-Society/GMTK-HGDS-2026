using System;
using UnityEngine;

namespace BirthdayJobJam.Application
{
    [Serializable]
    public sealed class EndingDefinition
    {
        [SerializeField] private string endingId;
        [SerializeField] private string title;
        [SerializeField, TextArea] private string description;
        [SerializeField] private int minimumScore;

        public string EndingId => endingId;
        public string Title => title;
        public string Description => description;
        public int MinimumScore => minimumScore;
    }
}
