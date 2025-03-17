using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace SG
{ 
    public class InputHandler : MonoBehaviour
    {
        public InputActionAsset inputActionss;
        PlayerControls inputActions;
        PlayerManager player;

        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        public bool b_Input;
        public bool a_Input;
        public bool x_Input;
        public bool y_Input;

        public bool tap_rb_Input;
        public bool critical_Attack_Input;
        public bool hold_rb_Input;
        public bool tap_rt_Input;
        public bool hold_rt_Input;

        public bool lb_Input;
        public bool tap_lb_Input;
        public bool tap_lt_Input;

        public bool jump_Input;
        public bool inventory_Input;
        public bool lockOnInput;
        public bool right_Stick_Right_Input;
        public bool right_Stick_Left_Input;

        public bool d_Pad_Up;
        public bool d_Pad_Down;
        public bool d_Pad_Left;
        public bool d_Pad_Right;

        public bool rollFlag;
        public bool twoHandFlag;
        public bool comboFlag;
        public bool lockOnFlag;
        public bool fireFlag;
        public bool inventoryFlag;
        public bool fireKeeperInventoryFlag;
        public bool bonforesRestFlag;
        public float rollInputTimer;

        public bool input_Has_Been_Qued;
        public float current_Qued_Input_Timer;
        public float default_Qued_Input_Time;
        public bool qued_RB_Input;

        Vector2 movementInput;
        Vector2 cameraInput;

        private void Awake()
        {
            player = GetComponent<PlayerManager>();
        }

        private void Start()
        {
            inputActionss.Enable();
        }

        public void OnEnable()
        {
            if(inputActions == null)
            {
                inputActions = new PlayerControls();
                inputActions.PlayerMovement.Movement.performed += inputActions => movementInput = inputActions.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

                inputActions.PlayerActions.RB.performed += inputActions => tap_rb_Input = true;
                inputActions.PlayerActions.CriticalAttack.performed += i => critical_Attack_Input = true;

                //拉弓，放手的时候射箭
                inputActions.PlayerActions.HoldRB.performed += inputActions => hold_rb_Input = true;
                inputActions.PlayerActions.HoldRB.canceled += inputActions => hold_rb_Input = false;

                //RB是鼠标左键，RT是Shift+鼠标左键
                inputActions.PlayerActions.RT.performed += inputActions => tap_rt_Input = true;
                //蓄力重击
                inputActions.PlayerActions.HoldRT.performed += inputActions => hold_rt_Input = true;
                inputActions.PlayerActions.HoldRT.canceled += inputActions => hold_rt_Input = false;

                //防御，松开按键取消防御
                inputActions.PlayerActions.TapLB.performed += inputActions => tap_lb_Input = true;
                inputActions.PlayerActions.LB.performed += inputActions => lb_Input = true;
                inputActions.PlayerActions.LB.canceled += inputActions => lb_Input = false;

                inputActions.PlayerActions.LT.performed += inputActions => tap_lt_Input = true;

                //上下左右切换键（上：法术，下：消耗品，左：左手武器，右：右手武器）
                inputActions.PlayerQuickSlots.DPadUp.performed += i => d_Pad_Up = true;
                inputActions.PlayerQuickSlots.DPadDown.performed += i => d_Pad_Down = true;
                inputActions.PlayerQuickSlots.DPadRight.performed += i => d_Pad_Right = true;
                inputActions.PlayerQuickSlots.DPadLeft.performed += i => d_Pad_Left = true;

                //互动键
                inputActions.PlayerActions.A.performed += i => a_Input = true;

                //使用道具键
                inputActions.PlayerActions.X.performed += i => x_Input = true;

                //翻滚，冲刺通过rollFlag和sprintFlag控制，按住开始冲刺，松开停止冲刺
                inputActions.PlayerActions.Roll.performed += i => b_Input = true;
                inputActions.PlayerActions.Roll.canceled += i => b_Input = false;

                //跳
                inputActions.PlayerActions.Jump.performed += i => jump_Input = true;

                //打开选单
                inputActions.PlayerActions.Inventory.performed += i => inventory_Input = true;

                //锁定
                inputActions.PlayerActions.LockOn.performed += i => lockOnInput = true;
                //锁定左右切换目标
                inputActions.PlayerMovement.LockOnTargetRight.performed += i => right_Stick_Right_Input = true;
                inputActions.PlayerMovement.LockOnTargetLeft.performed += i => right_Stick_Left_Input = true;

                //双持
                inputActions.PlayerActions.Y.performed += i => y_Input = true;

                inputActions.PlayerActions.QuedRB.performed += i => QueInput(ref qued_RB_Input);
            }

            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void TickInput()
        {
            if (player.isDead)
                return;

            HandleInventoryInput();

            //升级界面或DeepSeek时禁止其他输入
            HandleFireKeeperInventory();

            //坐火时禁止其他输入
            HandlebonforesRest();

            HandleMoveInput();
            HandleRollInput();

            if (inventoryFlag == true)
            {
                SetAllInputsToFalse();
                return;
            }

            HandleHoldRBInput();
            HandleHoldLBInput();
            HandleHoldRTInput();

            HandleTapLBInput();
            HandleTapRBInput();
            HandleTapRTInput();
            HandleTapLTInput();

            HandleQuickSlotsInput();

            HandleLockOnInput();
            HandleTwoHandInput();
            HandleCriticalAttackInput();
            HandleUseConsumableInput();
        }

        private void SetAllInputsToFalse()
        {
            tap_rb_Input = false;
            critical_Attack_Input = false;
            hold_rb_Input = false;
            tap_rt_Input = false;
            hold_rt_Input = false;
            tap_lb_Input = false;
            lb_Input = false;
            tap_lt_Input = false;
            d_Pad_Up = false;
            d_Pad_Down = false;
            d_Pad_Right = false;
            d_Pad_Left = false;
            a_Input = false;
            x_Input = false;
            lockOnInput = false;
        }

        private void HandleMoveInput()
        {
            if(player.isHoldingArrow || player.playerStatsManager.encumbranceLevel == EncumbranceLevel.Overloaded)
            {
                horizontal = movementInput.x;
                vertical = movementInput.y;
                moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

                if(moveAmount > 0.5f)
                {
                    moveAmount = 0.5f;
                }

                mouseX = cameraInput.x;
                mouseY = cameraInput.y;
            }
            else
            {
                horizontal = movementInput.x;
                vertical = movementInput.y;
                moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
                mouseX = cameraInput.x;
                mouseY = cameraInput.y;
            }
        }

        private void HandleRollInput()
        {
            if (b_Input)
            {
                rollInputTimer += Time.deltaTime;

                if(player.playerStatsManager.currentStamina <= 0)
                {
                    b_Input = false;
                    player.isSprinting = false;
                }    

                if(moveAmount > 0.5f && player.playerStatsManager.currentStamina > 0)
                {
                    player.isSprinting = true;
                }
            }
            else
            {
                player.isSprinting = false;

                if (rollInputTimer > 0 && rollInputTimer < 0.5f)
                {
                    rollFlag = true;
                }

                rollInputTimer = 0;
            }
        }

        private void HandleTapRBInput()
        {
            //Rb Input 控制右手武器轻攻击
            if (tap_rb_Input)
            {
                if(player.playerInventoryManager.rightWeapon.oh_tap_RB_Action != null)
                {
                    player.UpdateWhitchHandCharacterIsUsing(true);
                    player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                    player.playerInventoryManager.rightWeapon.oh_tap_RB_Action.PerformAction(player);
                }

                tap_rb_Input = false;
            }
        }

        private void HandleHoldRBInput()
        {
            if (hold_rb_Input)
            {
                if(player.playerInventoryManager.rightWeapon.oh_hold_RB_Action != null)
                {
                    player.UpdateWhitchHandCharacterIsUsing(true);
                    player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                    player.playerInventoryManager.rightWeapon.oh_hold_RB_Action.PerformAction(player);
                }
            }
        }

        private void HandleTapRTInput()
        {
            //Rt Input 控制右手武器重攻击
            if (tap_rt_Input)
            {
                if (player.playerInventoryManager.rightWeapon.oh_tap_RT_Action != null)
                {
                    player.UpdateWhitchHandCharacterIsUsing(true);
                    player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                    player.playerInventoryManager.rightWeapon.oh_tap_RT_Action.PerformAction(player);
                }

                tap_rt_Input = false;
            }
        }

        private void HandleHoldRTInput()
        {
            player.animator.SetBool("isChargingAttack", hold_rt_Input);
            if (hold_rt_Input)
            {
                Debug.Log("蓄力");
                player.UpdateWhitchHandCharacterIsUsing(true);
                player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;

                if(player.isTwoHandingWeapon)
                {
                    if(player.playerInventoryManager.rightWeapon.th_hold_RT_Action != null)
                    {
                        player.playerInventoryManager.rightWeapon.th_hold_RT_Action.PerformAction(player);
                    }
                }
                else
                {
                    if(player.playerInventoryManager.rightWeapon.oh_hold_RT_Action != null)
                    {
                        player.playerInventoryManager.rightWeapon.oh_hold_RT_Action.PerformAction(player);
                    }
                }
            }
        }

        private void HandleTapLTInput()
        {
            if (tap_lt_Input)
            {
                tap_lt_Input = false;

                if(player.isTwoHandingWeapon)
                {
                    if(player.playerInventoryManager.rightWeapon.oh_tap_LT_Action != null)
                    {
                        //双手就是用右手
                        player.UpdateWhitchHandCharacterIsUsing(true);
                        player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                        player.playerInventoryManager.rightWeapon.oh_tap_LT_Action.PerformAction(player);
                    }
                }
                else
                {
                    if(player.playerInventoryManager.leftWeapon.oh_tap_LT_Action != null)
                    {
                        player.UpdateWhitchHandCharacterIsUsing(false);
                        player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.leftWeapon;
                        player.playerInventoryManager.leftWeapon.oh_tap_LT_Action.PerformAction(player);
                    }
                }
            }
        }

        private void HandleHoldLBInput()
        {
            if (!player.isGrounded ||
                player.isSprinting ||
                player.isFiringSpell)
            {
                lb_Input = false;
                return;
            }
            if (lb_Input)
            {
                if(player.isTwoHandingWeapon)
                {
                    if(player.playerInventoryManager.rightWeapon.oh_hold_LB_Action != null)
                    {
                        player.UpdateWhitchHandCharacterIsUsing(true);
                        player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                        player.playerInventoryManager.rightWeapon.oh_hold_LB_Action.PerformAction(player);
                    }
                }
                else
                {
                    if(player.playerInventoryManager.leftWeapon.oh_hold_LB_Action != null)
                    {
                        player.UpdateWhitchHandCharacterIsUsing(false);
                        player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.leftWeapon;
                        player.playerInventoryManager.leftWeapon.oh_hold_LB_Action.PerformAction(player);
                    }
                }
            }
            else if(lb_Input == false)
            {
                if(player.isAiming)
                {
                    player.isAiming = false;
                    player.uiManager.crossHair.SetActive(false);
                    //重置相机旋转
                    player.cameraHandler.ResetAimCameraRotations();
                }

                if (player.isBlocking)
                {
                    player.isBlocking = false;
                }
            }
        }

        private void HandleTapLBInput()
        {
            if (tap_lb_Input)
            {
                tap_lb_Input = false;

                if(player.isTwoHandingWeapon)
                {
                    if(player.playerInventoryManager.rightWeapon.oh_tap_LB_Action != null)
                    {
                        player.UpdateWhitchHandCharacterIsUsing(true);
                        player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                        player.playerInventoryManager.rightWeapon.oh_tap_LB_Action.PerformAction(player);
                    }
                }
                else
                {
                    if(player.playerInventoryManager.leftWeapon.oh_tap_LB_Action != null)
                    {
                        player.UpdateWhitchHandCharacterIsUsing(false);
                        player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.leftWeapon;
                        player.playerInventoryManager.leftWeapon.oh_tap_LB_Action.PerformAction(player);
                    }
                }
            }
        }

        private void HandleQuickSlotsInput()
        {
            if (d_Pad_Right)
            {
                player.playerInventoryManager.ChangeRightWeapon();
            }
            else if(d_Pad_Left)
            {
                player.playerInventoryManager.ChangeLeftWeapon();
            }
            else if(d_Pad_Up)
            {
                player.playerInventoryManager.ChangeSpell();
            }
            else if(d_Pad_Down)
            {
                player.playerInventoryManager.ChangeConsumableItem();
            }
        }

        private void HandleInventoryInput()
        {
            if(inventoryFlag)
            {
                player.uiManager.UpdateUI();
            }
            if(inventory_Input)
            {
                if(fireKeeperInventoryFlag)
                {
                    player.uiManager.levelUpWindow.SetActive(false);
                    player.uiManager.FireKeeperDeepSeek.SetActive(false);
                    player.uiManager.FireKeeperWindow.SetActive(false);
                    fireKeeperInventoryFlag = false;
                    return;
                }

                if (bonforesRestFlag)
                {
                    player.uiManager.bonfiresRestWindow.SetActive(false);
                    bonforesRestFlag = false;
                    player.animator.SetBool("isRestByBornfires", false);
                    return;
                }

                inventoryFlag = !inventoryFlag;

                if (inventoryFlag)
                {
                    player.uiManager.OpenSelectWindow();   //打开选择菜单
                    player.uiManager.hudWindow.SetActive(false);   //关闭HUD显示
                }
                else
                {
                    player.uiManager.CloseSelectWindow();
                    player.uiManager.CloseAllInventoryWindows();
                    player.uiManager.hudWindow.SetActive(true);
                }
            }
        }

        private void HandleLockOnInput()
        {
            if (lockOnInput && lockOnFlag == false) //按下锁定键（没有锁定时）
            {
                lockOnInput = false;
                player.cameraHandler.HandleLockOn();
                if(player.cameraHandler.nearestLockOnTarget != null)
                {
                    player.cameraHandler.currentLockOnTarget = player.cameraHandler.nearestLockOnTarget;
                    lockOnFlag = true;
                }
            }
            else if(lockOnInput && lockOnFlag)  //按下锁定键（已经锁定）→取消锁定，清空所有可选目标
            {
                lockOnInput = false;
                lockOnFlag = false;
                player.cameraHandler.ClearLockOnTargets();
            }

            if(lockOnFlag && right_Stick_Left_Input)    //锁定切换左侧对象（已经锁定）
            {
                right_Stick_Left_Input = false;
                player.cameraHandler.HandleLockOn();
                if(player.cameraHandler.leftLockTarget != null)
                {
                    player.cameraHandler.currentLockOnTarget = player.cameraHandler.leftLockTarget;
                }
            }

            if(lockOnFlag && right_Stick_Right_Input)       //锁定切换右侧对象（已经锁定）
            {
                right_Stick_Right_Input = false;
                player.cameraHandler.HandleLockOn();
                if(player.cameraHandler.rightLockTarget != null)
                {
                    player.cameraHandler.currentLockOnTarget = player.cameraHandler.rightLockTarget;
                }
            }

            player.cameraHandler.SetCameraHeight();     //设置相机高度
        }

        private void HandleTwoHandInput()
        {
            if(y_Input)
            {
                y_Input = false;

                twoHandFlag = !twoHandFlag;

                if(twoHandFlag)
                {
                    //ENABLE TWO HANDING
                    player.isTwoHandingWeapon = true;
                    player.playerWeaponSlotManager.LoadWeaponOnSlot(player.playerInventoryManager.rightWeapon, false);
                    player.playerWeaponSlotManager.LoadTwoHandIKTargets(true);
                }
                else
                {
                    //DISABLE TWO HANDING
                    player.isTwoHandingWeapon = false;
                    player.playerWeaponSlotManager.LoadWeaponOnSlot(player.playerInventoryManager.rightWeapon, false);
                    player.playerWeaponSlotManager.LoadWeaponOnSlot(player.playerInventoryManager.leftWeapon, true);
                    player.playerWeaponSlotManager.LoadTwoHandIKTargets(false);
                }
            }
        }

        private void HandleCriticalAttackInput()
        {
            if (critical_Attack_Input)
            {
                critical_Attack_Input = false;
                if (player.playerInventoryManager.rightWeapon.oh_tap_RB_Critical_Action != null)
                {
                    player.UpdateWhitchHandCharacterIsUsing(true);
                    player.playerInventoryManager.currentItemBeingUsed = player.playerInventoryManager.rightWeapon;
                    player.playerInventoryManager.rightWeapon.oh_tap_RB_Critical_Action.PerformAction(player);
                }
            }
        }

        private void HandleUseConsumableInput()
        {
            if (x_Input)
            {
                x_Input = false;
                //Use current consumable
                player.playerInventoryManager.currentConsumable.AttemtToConsumeItem(player);
            }
        }

        private void QueInput(ref bool quedInput)
        {
            //同一时间只能有一个预输入

            //在正在做动作的时候才可以预输入，不做动作的时候直接操作
            if(player.isInteracting)
            {
                quedInput = true;
                current_Qued_Input_Timer = default_Qued_Input_Time;
                input_Has_Been_Qued = true;
            }
        }

        private void HandleQuedInput()
        {
            if(input_Has_Been_Qued)
            {
                if(current_Qued_Input_Timer > 0)
                {
                    current_Qued_Input_Timer = current_Qued_Input_Timer - Time.deltaTime;
                    ProcessQuedInput();
                }
                else
                {
                    input_Has_Been_Qued = false;
                    current_Qued_Input_Timer = 0;
                }
            }
        }

        private void ProcessQuedInput()
        {
            if(qued_RB_Input)
            {
                tap_rb_Input = true;
            }
            //if Qued_LB_Input => tap_lb_Input = true;
        }

        //升级界面或DeepSeek时禁止其他输入
        private void HandleFireKeeperInventory()
        {
            if (fireKeeperInventoryFlag == true)
            {
                SetAllInputsToFalse();
                movementInput = Vector2.zero;
                cameraInput = Vector2.zero;
                b_Input = false;
                jump_Input = false;
                y_Input = false;
                return;
            }
        }

        //做火时禁止其他输入
        private void HandlebonforesRest()
        {
            if (bonforesRestFlag == true)
            {
                player.animator.SetBool("isRestByBornfires", true);
                SetAllInputsToFalse();
                b_Input = false;
                jump_Input = false;
                y_Input = false;
                return;
            }
        }
    }

}