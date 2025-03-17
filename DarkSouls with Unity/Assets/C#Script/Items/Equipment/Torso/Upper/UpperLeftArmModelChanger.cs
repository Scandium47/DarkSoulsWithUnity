using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class UpperLeftArmModelChanger : MonoBehaviour
    {
        public List<GameObject> upperModels;

        private void Awake()
        {
            GetAllUpperModels();
        }

        private void GetAllUpperModels()
        {
            int childrenGameObjects = transform.childCount;

            for (int i = 0; i < childrenGameObjects; i++)
            {
                upperModels.Add(transform.GetChild(i).gameObject);
            }
        }

        public void UnEquipAllUpperModles()
        {
            foreach (GameObject upperModel in upperModels)
            {
                upperModel.SetActive(false);
            }
        }

        public void EquipUpperModelByName(string upperModelName)
        {
            for (int i = 0; i < upperModels.Count; i++)
            {
                if (upperModels[i].name == upperModelName)
                {
                    upperModels[i].SetActive(true);
                }
            }
        }
    }
}