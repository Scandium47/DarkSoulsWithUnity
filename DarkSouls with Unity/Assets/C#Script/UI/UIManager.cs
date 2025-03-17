using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SG
{
    public class UIManager : MonoBehaviour
    {
        public PlayerManager player;
        public ItemStatsWindowUI itemStatsWindowUI;
        public EquipmentWindowUI equipmentWindowUI;
        public QuickSlotsUI quickSlotsUI;

        [Header("HUD")]
        public GameObject crossHair;
        public TextMeshProUGUI soulCount;

        [Header("UI Windows")]
        public GameObject hudWindow;
        public GameObject selectWindow;
        public GameObject equipmentScreenWindow;
        public GameObject weaponInventoryWindow;
        public GameObject headEquipmentInventoryWindow;
        public GameObject bodyEquipmentInventoryWindow;
        public GameObject legEquipmentInventoryWindow;
        public GameObject handEquipmentInventoryWindow;
        public GameObject spellEquipmentInventoryWindow;
        public GameObject itemsEquipmentInventoryWindow;
        public GameObject allInventoryWindow;
        public GameObject SettingWindow;
        public GameObject itemStatsWindow;
        public GameObject levelUpWindow;
        public GameObject FireKeeperWindow;
        public GameObject FireKeeperDeepSeek;
        public GameObject bonfiresRestWindow;

        [Header("Equipment Window Slot Selected")]
        public bool rightHandSlot01Selected;
        public bool rightHandSlot02Selected;
        public bool leftHandSlot01Selected;
        public bool leftHandSlot02Selected;

        public bool headEquipmentSlotSelected;
        public bool bodyEquipmentSlotSelected;
        public bool legEquipmentSlotSelected;
        public bool handEquipmentSlotSelected;

        public bool upSpellSlot01Selected;
        public bool upSpellSlot02Selected;
        public bool upSpellSlot03Selected;
        public bool upSpellSlot04Selected;

        public bool downItemsSlot01Selected;
        public bool downItemsSlot02Selected;
        public bool downItemsSlot03Selected;
        public bool downItemsSlot04Selected;

        [Header("Pop Ups")]
        BonfireLitPopUPUI bonfireLitPopUPUI;

        [Header("Weapon Inventory")]
        public GameObject weaponInventorySlotPrefab;
        public Transform weaponInventorySlotsParent;
        WeaponInventorySlot[] weaponInventorySlots;

        [Header("Head Equipment Inventory")]
        public GameObject headEquipmentInventorySlotPrefab;
        public Transform headEquipmentInventorySlotParent;
        HeadEquipmentInventorySlot[] headEquipmentInventorySlots;

        [Header("Body Equipment Inventory")]
        public GameObject bodyEquipmentInventorySlotPrefab;
        public Transform bodyEquipmentInventorySlotParent;
        BodyEquipmentInventorySlot[] bodyEquipmentInventorySlots;

        [Header("Leg Equipment Inventory")]
        public GameObject legEquipmentInventorySlotPrefab;
        public Transform legEquipmentInventorySlotParent;
        LegEquipmentInventorySlot[] legEquipmentInventorySlots;

        [Header("Hand Equipment Inventory")]
        public GameObject handEquipmentInventorySlotPrefab;
        public Transform handEquipmentInventorySlotParent;
        HandEquipmentInventorySlot[] handEquipmentInventorySlots;

        [Header("Spell Inventory")]
        public GameObject spellInventorySlotPrefab;
        public Transform spellInventorySlotsParent;
        SpellEquipmentInventorySlot[] spellEquipmentInventorySlots;

        [Header("Items Inventory")]
        public GameObject itemsInventorySlotPrefab;
        public Transform itemsInventorySlotsParent;
        ItemsEquipmentInventorySlot[] itemsEquipmentInventorySlots;

        private void Awake()
        {
            player = FindObjectOfType<PlayerManager>();

            quickSlotsUI = GetComponentInChildren<QuickSlotsUI>();

            weaponInventorySlots = weaponInventorySlotsParent.GetComponentsInChildren<WeaponInventorySlot>();
            headEquipmentInventorySlots = headEquipmentInventorySlotParent.GetComponentsInChildren<HeadEquipmentInventorySlot>();
            bodyEquipmentInventorySlots = bodyEquipmentInventorySlotParent.GetComponentsInChildren<BodyEquipmentInventorySlot>();
            legEquipmentInventorySlots = legEquipmentInventorySlotParent.GetComponentsInChildren<LegEquipmentInventorySlot>();
            handEquipmentInventorySlots = handEquipmentInventorySlotParent.GetComponentsInChildren<HandEquipmentInventorySlot>();
            spellEquipmentInventorySlots = spellInventorySlotsParent.GetComponentsInChildren<SpellEquipmentInventorySlot>();
            itemsEquipmentInventorySlots = itemsInventorySlotsParent.GetComponentsInChildren<ItemsEquipmentInventorySlot>();

            bonfireLitPopUPUI = GetComponentInChildren<BonfireLitPopUPUI>();
        }
        private void Start()
        {
            equipmentWindowUI.LoadWeaponOnEquipmentScreen(player.playerInventoryManager);
            equipmentWindowUI.LoadArmorOnEquipmentScreen(player.playerInventoryManager);
            equipmentWindowUI.LoadSpellOnEquipmentScreen(player.playerInventoryManager);
            equipmentWindowUI.LoadItemsOnEquipmentScreen(player.playerInventoryManager);

            soulCount.text = player.playerStatsManager.currentSoulCount.ToString();
        }
        public void UpdateUI()
        {
            //Weapon Inventory Slots 武器
            for (int i =0; i < weaponInventorySlots.Length; i++)    //背包武器UI存储遍历
            {
                if(i < player.playerInventoryManager.weaponsInventory.Count)  //当前武器UI < 背包武器数量
                {
                    if(weaponInventorySlots.Length < player.playerInventoryManager.weaponsInventory.Count)    //背包武器UI存储长度 < 背包武器数量
                    {
                        Instantiate(weaponInventorySlotPrefab, weaponInventorySlotsParent);     //实例化背包武器槽
                        weaponInventorySlots = weaponInventorySlotsParent.GetComponentsInChildren<WeaponInventorySlot>();
                    }
                    weaponInventorySlots[i].AddItem(player.playerInventoryManager.weaponsInventory[i]);   //将背包武器添加到背包武器UI
                }
                else
                {
                    weaponInventorySlots[i].ClearInventorySlot();   //如果当前武器UI数超过背包武器实际数量，清空i的UI武器槽
                }
            }
            //Hand Equipment Inventory Slots  头盔
            for (int i = 0; i < headEquipmentInventorySlots.Length; i++)
            {
                if(i < player.playerInventoryManager.headEquipmentInventory.Count)
                {
                    if(headEquipmentInventorySlots.Length < player.playerInventoryManager.headEquipmentInventory.Count)
                    {
                        Instantiate(headEquipmentInventorySlotPrefab, headEquipmentInventorySlotParent);
                        headEquipmentInventorySlots = headEquipmentInventorySlotParent.GetComponentsInChildren<HeadEquipmentInventorySlot>();
                    }
                    headEquipmentInventorySlots[i].AddItem(player.playerInventoryManager.headEquipmentInventory[i]);
                }
                else
                {
                    headEquipmentInventorySlots[i].ClearInventorySlot();
                }
            }
            //Body Equipment Inventory Slot 胸甲
            for (int i = 0; i < bodyEquipmentInventorySlots.Length; i++)
            {
                if (i < player.playerInventoryManager.bodyEquipmentInventory.Count)
                {
                    if (bodyEquipmentInventorySlots.Length < player.playerInventoryManager.bodyEquipmentInventory.Count)
                    {
                        Instantiate(bodyEquipmentInventorySlotPrefab, bodyEquipmentInventorySlotParent);
                        bodyEquipmentInventorySlots = bodyEquipmentInventorySlotParent.GetComponentsInChildren<BodyEquipmentInventorySlot>();
                    }
                    bodyEquipmentInventorySlots[i].AddItem(player.playerInventoryManager.bodyEquipmentInventory[i]);
                }
                else
                {
                    bodyEquipmentInventorySlots[i].ClearInventorySlot();
                }
            }
            //Leg Equipment Inventory Slot 腿甲
            for (int i = 0; i < legEquipmentInventorySlots.Length; i++)
            {
                if (i < player.playerInventoryManager.legEquipmentInventory.Count)
                {
                    if (legEquipmentInventorySlots.Length < player.playerInventoryManager.legEquipmentInventory.Count)
                    {
                        Instantiate(legEquipmentInventorySlotPrefab, legEquipmentInventorySlotParent);
                        legEquipmentInventorySlots = legEquipmentInventorySlotParent.GetComponentsInChildren<LegEquipmentInventorySlot>();
                    }
                    legEquipmentInventorySlots[i].AddItem(player.playerInventoryManager.legEquipmentInventory[i]);
                }
                else
                {
                    legEquipmentInventorySlots[i].ClearInventorySlot();
                }
            }
            //Hand Equipment Inventory Slot 手甲
            for (int i = 0; i < handEquipmentInventorySlots.Length; i++)
            {
                if (i < player.playerInventoryManager.handEquipmentInventory.Count)
                {
                    if (handEquipmentInventorySlots.Length < player.playerInventoryManager.handEquipmentInventory.Count)
                    {
                        Instantiate(handEquipmentInventorySlotPrefab, handEquipmentInventorySlotParent);
                        handEquipmentInventorySlots = handEquipmentInventorySlotParent.GetComponentsInChildren<HandEquipmentInventorySlot>();
                    }
                    handEquipmentInventorySlots[i].AddItem(player.playerInventoryManager.handEquipmentInventory[i]);
                }
                else
                {
                    handEquipmentInventorySlots[i].ClearInventorySlot();
                }
            }
            //Spell Inventory Slots 法术/祷告/咒术
            for (int i = 0; i < spellEquipmentInventorySlots.Length; i++)    //背包武器UI存储遍历
            {
                if (i < player.playerInventoryManager.spellInventory.Count)  //当前武器UI < 背包武器数量
                {
                    if (spellEquipmentInventorySlots.Length < player.playerInventoryManager.spellInventory.Count)    //背包武器UI存储长度 < 背包武器数量
                    {
                        Instantiate(spellInventorySlotPrefab, spellInventorySlotsParent);     //实例化背包武器槽
                        spellEquipmentInventorySlots = spellInventorySlotsParent.GetComponentsInChildren<SpellEquipmentInventorySlot>();
                    }
                    spellEquipmentInventorySlots[i].AddItem(player.playerInventoryManager.spellInventory[i]);   //将背包武器添加到背包武器UI
                }
                else
                {
                    spellEquipmentInventorySlots[i].ClearInventorySlot();   //如果当前武器UI数超过背包武器实际数量，清空i的UI武器槽
                }
            }
            //Items Inventory Slots 物品
            for (int i = 0; i < itemsEquipmentInventorySlots.Length; i++)    //背包武器UI存储遍历
            {
                if (i < player.playerInventoryManager.consumableItemInventory.Count)  //当前武器UI < 背包武器数量
                {
                    if (itemsEquipmentInventorySlots.Length < player.playerInventoryManager.consumableItemInventory.Count)    //背包武器UI存储长度 < 背包武器数量
                    {
                        Instantiate(itemsInventorySlotPrefab, itemsInventorySlotsParent);     //实例化背包武器槽
                        itemsEquipmentInventorySlots = itemsInventorySlotsParent.GetComponentsInChildren<ItemsEquipmentInventorySlot>();
                    }
                    itemsEquipmentInventorySlots[i].AddItem(player.playerInventoryManager.consumableItemInventory[i]);   //将背包武器添加到背包武器UI
                }
                else
                {
                    itemsEquipmentInventorySlots[i].ClearInventorySlot();   //如果当前武器UI数超过背包武器实际数量，清空i的UI武器槽
                }
            }
        }

        public void OpenSelectWindow()
        {
            selectWindow.SetActive(true);
        }

        public void CloseSelectWindow()
        {
            selectWindow.SetActive(false);
        }

        public void CloseAllInventoryWindows()
        {
            ResetAllSelectedSlots();
            weaponInventoryWindow.SetActive(false);
            equipmentScreenWindow.SetActive(false);
            headEquipmentInventoryWindow.SetActive(false);
            bodyEquipmentInventoryWindow.SetActive(false);
            legEquipmentInventoryWindow.SetActive(false);
            handEquipmentInventoryWindow.SetActive(false);
            spellEquipmentInventoryWindow.SetActive(false);
            itemsEquipmentInventoryWindow.SetActive(false);
            allInventoryWindow.SetActive(false);
            SettingWindow.SetActive(false);
            itemStatsWindow.SetActive(false);
        }

        public void ResetAllSelectedSlots()
        {
            rightHandSlot01Selected = false;
            rightHandSlot02Selected = false;
            leftHandSlot01Selected=false;
            leftHandSlot02Selected=false;

            headEquipmentSlotSelected = false;
            bodyEquipmentSlotSelected = false;
            legEquipmentSlotSelected = false;
            handEquipmentSlotSelected = false;

            upSpellSlot01Selected = false;
            upSpellSlot02Selected = false;
            upSpellSlot03Selected = false;
            upSpellSlot04Selected = false;

            downItemsSlot01Selected = false;
            downItemsSlot02Selected = false;
            downItemsSlot03Selected = false;
            downItemsSlot04Selected = false;
        }

        public void ActivateBonfirePopUp()
        {
            bonfireLitPopUPUI.DisplayBonfireLitPopUp();
        }
    }
}