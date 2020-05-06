using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StatisticsMod.UI
{
    public class GraphView : MonoBehaviour
    {
        private List<float> dataset;

        public void AddPoint(float pointValue)
        {
            dataset.Add(pointValue);
        }

        public void RemovePoint(float pointValue)
        {
            dataset.Remove(pointValue);
        }

        public List<float> GetPoints()
        {
            return dataset;
        }

        public float GetMaxInDataset()
        {
            float max = Mathf.NegativeInfinity;
            for(int i = 0; i < dataset.Count; i++)
            {
                if (dataset[i] > max)
                    max = dataset[i];
            }
            return max;
        }


        void Recalculate()
        {
            RectTransform trans = transform as RectTransform;
            LineRenderer line = transform.Find("ViewContainer/Line").GetComponent<LineRenderer>();


            float max = GetMaxInDataset();
            float scaling = trans.rect.height / max;

            line.positionCount = dataset.Count;
            
            float increment = trans.rect.width / dataset.Count;
            for(int i = 0; i < dataset.Count; i++)
            {
                float pos = increment * i;
                line.SetPosition(i, new Vector3(pos,dataset[i] * scaling));
            }
        }

        void Update()
        {
            Recalculate();
        }
    }
}
