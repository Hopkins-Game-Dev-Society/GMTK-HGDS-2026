using System.Collections.Generic;
using UnityEngine;

namespace BirthdayJobJam.Views
{
    public sealed class ViewNavigation : MonoBehaviour
    {
        [System.Serializable]
        public class NavigationLink
        {
            public ViewDirection direction;
            public GameViewId destination;
        }


        [SerializeField]
        private List<NavigationLink> links = new();


        private Dictionary<ViewDirection, GameViewId> navigationMap = new();



        private void Awake()
        {
            BuildNavigationMap();
        }



        private void BuildNavigationMap()
        {
            navigationMap.Clear();


            foreach (NavigationLink link in links)
            {
                if (link.destination == GameViewId.None)
                    continue;


                navigationMap[link.direction] = link.destination;
            }
        }



        public bool TryGetDestination(
            ViewDirection direction,
            out GameViewId destination)
        {
            return navigationMap.TryGetValue(
                direction,
                out destination
            );
        }
    }
}