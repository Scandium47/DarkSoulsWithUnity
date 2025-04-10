using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class LowerRightArmModelChanger : MonoBehaviour
    {
        public List<GameObject> models;

        private void Awake()
        {
            GetAllModels();
        }

        private void GetAllModels()
        {
            int childrenGameObjects = transform.childCount;

            for (int i = 0; i < childrenGameObjects; i++)
            {
                models.Add(transform.GetChild(i).gameObject);
            }
        }

        public void UnEquipAllModles()
        {
            foreach (GameObject model in models)
            {
                model.SetActive(false);
            }
        }

        public void EquipModelByName(string modelName)
        {
            for (int i = 0; i < models.Count; i++)
            {
                if (models[i].name == modelName)
                {
                    models[i].SetActive(true);
                }
            }
        }
    }
}