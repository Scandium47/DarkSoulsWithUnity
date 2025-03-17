using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SG
{
    public class WorldItemDataBase : MonoBehaviour
    {
        public static WorldItemDataBase Instance;

        public List <WeaponItem> weaponItems = new List<WeaponItem>();

        public List <EquipmentItem> equipmentItems = new List<EquipmentItem>();

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public WeaponItem GetWeaponItemByID(int weaponID)
        {
            //搜寻列表找到第一个有weaponID的，如果没有返回null
            return weaponItems.FirstOrDefault(weapon => weapon.itemID == weaponID);
        }

        public EquipmentItem GetEquipmentItemByID(int equipmentID)
        {
            //搜寻列表找到第一个有equipmentID的，如果没有返回null
            return equipmentItems.FirstOrDefault(equipment => equipment.itemID ==equipmentID);
        }
    }
}