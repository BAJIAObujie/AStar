using System;
using UnityEngine;

namespace PathFinding
{
    public class GridInfoMono : MonoBehaviour
    {
        public TextMesh F;
        public TextMesh G;
        public TextMesh H;
        public TextMesh Step;

        public void Set(GridInfo info)
        {
            F.text = info.F.ToString();
            G.text = info.G.ToString();
            H.text = info.H.ToString();
            Step.text = info.Step.ToString();
        }
        
        public void Hide()
        {
            F.text = "";
            G.text = "";
            H.text = "";
            Step.text = "";
        }
        
    }
}