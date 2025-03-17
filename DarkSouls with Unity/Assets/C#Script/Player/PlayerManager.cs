using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlayerManager : CharacterManager
    {
        [Header("Camera")]
        public CameraHandler cameraHandler;

        [Header("Input")]
        public InputHandler inputHandler;

        [Header("UI")]
        public UIManager uiManager;

        [Header("Player")]
        public PlayerStatsManager playerStatsManager;
        public PlayerWeaponSlotManager playerWeaponSlotManager;
        public PlayerEquipmentManager playerEquipmentManager;
        public PlayerCombatManager playerCombatManager;
        public PlayerInventoryManager playerInventoryManager;
        public PlayerEffectsManager playerEffectsManager;
        public PlayerAnimatorManager playerAnimatorManager;
        public PlayerLocomotionManager playerLocomotionManager;

        [Header("Interactables")]
        InteractableUI interactableUI;
        public GameObject interactableUIGameObject;
        public GameObject itemInteractableGameObject;

        [Header("World Event Manager")]
        public WorldEventManager worldEventManager;

        protected override void Awake()
        {
            base.Awake();
            cameraHandler = FindObjectOfType<CameraHandler>();
            uiManager = FindObjectOfType<UIManager>();
            interactableUI = FindObjectOfType<InteractableUI>();
            inputHandler = GetComponent<InputHandler>();
            animator = GetComponent<Animator>();

            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerWeaponSlotManager = GetComponent<PlayerWeaponSlotManager>();
            playerCombatManager = GetComponent<PlayerCombatManager>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerEffectsManager = GetComponent<PlayerEffectsManager>();
            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();

            worldEventManager = FindObjectOfType<WorldEventManager>();
        }

        protected override void Start()
        {
            base.Start();
            WorldSaveGameManager.instance.player = this;
        }

        protected override void Update()
        {
            base.Update();
            float delta = Time.deltaTime;

            isInteracting = animator.GetBool("isInteracting");
            canDoCombo = animator.GetBool("canDoCombo");
            canRotate = animator.GetBool("canRotate");
            isInvulnerable = animator.GetBool("isInvulnerable");
            isFiringSpell = animator.GetBool("isFiringSpell");
            isHoldingArrow = animator.GetBool("isHoldingArrow");
            isPerformingFullyChargedAttack = animator.GetBool("isPerformingFullyChargedAttack");
            animator.SetBool("isTwoHandingWeapon", isTwoHandingWeapon);
            animator.SetBool("isBlocking", isBlocking);
            animator.SetBool("isDead", isDead);

            inputHandler.TickInput();
            playerLocomotionManager.HandleRollingAndSprinting();
            playerLocomotionManager.HandleJumping();
            playerStatsManager.RegenerateStamina();
            playerLocomotionManager.HandleGroundedMovement();
            playerLocomotionManager.HandleRotation();
            CheckForInteractableObject();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        private void LateUpdate()
        {
            inputHandler.d_Pad_Up = false;
            inputHandler.d_Pad_Down = false;
            inputHandler.d_Pad_Left = false;
            inputHandler.d_Pad_Right = false;
            inputHandler.a_Input = false;
            inputHandler.inventory_Input = false;


            if (cameraHandler != null)
            {
                cameraHandler.FollowTarget();
                cameraHandler.HandleCameraRotation();
                cameraHandler.CheckForDeadTarget();
            }
        }

        #region Player Interactions

        public void CheckForInteractableObject()
        {
            // 定义检测球体的半径
            float detectionRadius = 1f;
            // 使用 Physics.OverlapSphere 检测球体范围内的所有碰撞体
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, cameraHandler.ignoreLayers);

            bool interactableFound = false;
            foreach (Collider collider in hitColliders)
            {
                if (collider.tag == "Interactable")
                {
                    Interactable interactableObject = collider.GetComponent<Interactable>();

                    if (interactableObject != null)
                    {
                        string interactableText = interactableObject.interactableText;
                        // 发送 UI TEXT 给 InteractableObject TEXT
                        // SET THE TEXT POP UP TO TRUE
                        interactableUI.interactableText.text = interactableText;
                        interactableUIGameObject.SetActive(true);
                        interactableFound = true;

                        if (inputHandler.a_Input)
                        {
                            collider.GetComponent<Interactable>().Interact(this);
                        }
                    }
                }
            }

            if (!interactableFound)
            {
                if (interactableUIGameObject != null)
                {
                    interactableUIGameObject.SetActive(false);
                }

                if (itemInteractableGameObject != null && inputHandler.a_Input)
                {
                    itemInteractableGameObject.SetActive(false);
                }
            }
        }

        private void OnDrawGizmos()
        {
            // 定义检测球体的半径，要和 CheckForInteractableObject 中的保持一致
            float detectionRadius = 1f;
            // 设置绘制的颜色，这里设置为蓝色
            Gizmos.color = Color.blue;
            // 绘制一个线框球体，代表探测球的范围
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }

        public void OpenChestInteraction(Transform playerStandsHereWhenOpeningChest)
        {
            Rigidbody playerRigidbody = playerLocomotionManager.GetComponent<Rigidbody>();
            if (playerRigidbody.isKinematic)
            {
                // 如果是运动学刚体，通过 MovePosition 停止移动
                playerRigidbody.MovePosition(playerRigidbody.position);
            }
            else
            {
                playerRigidbody.velocity = Vector3.zero;
            }

            transform.position = playerStandsHereWhenOpeningChest.transform.position;
            playerAnimatorManager.PlayTargetAnimation("Open Chest", true);
        }

        public void PassThroughFogWallInteraction(Transform fogWallEntrace)
        {
            //先确保朝向雾门 
            Rigidbody playerRigidbody = playerLocomotionManager.GetComponent<Rigidbody>();
            if (playerRigidbody.isKinematic)
            {
                // 如果是运动学刚体，通过 MovePosition 停止移动
                playerRigidbody.MovePosition(playerRigidbody.position);
            }
            else
            {
                playerRigidbody.velocity = Vector3.zero;
            }

            Vector3 rotationDirection = fogWallEntrace.transform.forward;
            Quaternion turnRotation = Quaternion.LookRotation(rotationDirection);
            transform.rotation = turnRotation;
            //Rotate over time so it does not look as rigid

            playerAnimatorManager.PlayTargetAnimation("Pass Through Fog", true);
        }

        #endregion

        public void SaveCharacterDataToCurrentSaveData(ref CharacterSaveData currentCharacterSaveData)
        {
            //保存角色名称，等级
            currentCharacterSaveData.characterName = playerStatsManager.characterName;
            currentCharacterSaveData.characterLevel = playerStatsManager.playerLevel;
            //保存角色位置
            currentCharacterSaveData.xPosition = transform.position.x;
            currentCharacterSaveData.yPosition = transform.position.y;
            currentCharacterSaveData.zPosition = transform.position.z;
            //保存角色左右手武器ID
            currentCharacterSaveData.currentRightHandWeaponID = playerInventoryManager.rightWeapon.itemID;
            currentCharacterSaveData.currentLeftHandWeaponID = playerInventoryManager.leftWeapon.itemID;

            if (playerInventoryManager.currentHelmetEquipment != null)
            {
                currentCharacterSaveData.currentHeadGearItemID = playerInventoryManager.currentHelmetEquipment.itemID;
            }
            else
            {
                currentCharacterSaveData.currentHeadGearItemID = -1;
            }

            if (playerInventoryManager.currentBodyEquipment != null)
            {
                currentCharacterSaveData.currentChestGearItemID = playerInventoryManager.currentBodyEquipment.itemID;
            }
            else
            {
                currentCharacterSaveData.currentChestGearItemID = -1;
            }

            if (playerInventoryManager.currentLegEquipment != null)
            {
                currentCharacterSaveData.currentLegGearItemID = playerInventoryManager.currentLegEquipment.itemID;
            }
            else
            {
                currentCharacterSaveData.currentLegGearItemID = -1;
            }

            if (playerInventoryManager.currentHandEquipment != null)
            {
                currentCharacterSaveData.currentHandGearItemID = playerInventoryManager.currentHandEquipment.itemID;
            }
            else
            {
                currentCharacterSaveData.currentHandGearItemID = -1;
            }
        }

        public void LoadCharacterDataFromCurrentCharacterSvaeData(ref CharacterSaveData currentCharacterSaveData)
        {
            //加载角色名称，等级
            playerStatsManager.characterName = currentCharacterSaveData.characterName;
            playerStatsManager.playerLevel = currentCharacterSaveData.characterLevel;
            //加载角色位置
            transform.position = new Vector3(currentCharacterSaveData.xPosition, currentCharacterSaveData.yPosition, currentCharacterSaveData.zPosition);
            //加载角色左右手武器ID并装备
            playerInventoryManager.rightWeapon = WorldItemDataBase.Instance.GetWeaponItemByID(currentCharacterSaveData.currentRightHandWeaponID);
            playerInventoryManager.leftWeapon = WorldItemDataBase.Instance.GetWeaponItemByID(currentCharacterSaveData.currentLeftHandWeaponID);
            playerWeaponSlotManager.LoadBothWeaponsOnSlots();

            //如果装备在数据库内，应用
            EquipmentItem headEquipment = WorldItemDataBase.Instance.GetEquipmentItemByID(currentCharacterSaveData.currentHeadGearItemID);
            if (headEquipment != null)
            {
                playerInventoryManager.currentHelmetEquipment = headEquipment as HelmetEquipment;
            }

            EquipmentItem bodyEquipment = WorldItemDataBase.Instance.GetEquipmentItemByID(currentCharacterSaveData.currentChestGearItemID);
            if (bodyEquipment != null)
            {
                playerInventoryManager.currentBodyEquipment = bodyEquipment as BodyEquipment;
            }

            EquipmentItem legEquipment = WorldItemDataBase.Instance.GetEquipmentItemByID(currentCharacterSaveData.currentLegGearItemID);
            if (legEquipment != null)
            {
                playerInventoryManager.currentLegEquipment = legEquipment as LegEquipment;
            }

            EquipmentItem handEquipment = WorldItemDataBase.Instance.GetEquipmentItemByID(currentCharacterSaveData.currentHandGearItemID);
            if (handEquipment != null)
            {
                playerInventoryManager.currentHandEquipment = handEquipment as HandEquipment;
            }

            playerEquipmentManager.EquipAllArmor();
        }
    }
}
