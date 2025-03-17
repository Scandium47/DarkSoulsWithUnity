using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CharacterEffectsManager : MonoBehaviour
    {
        CharacterManager character;

        [Header("Static Effects")]
        [SerializeField] List<StaticCharacterEffect> staticCharacterEffects;

        [Header("Timed Effects")]
        public List<CharacterEffect> timedEffects;
        [SerializeField] float effectTickTimer = 0;

        [Header("Timed Effect Visual FX")]
        public List<GameObject> timedEffectParticles;

        [Header("Current Range FX")]
        public GameObject instantiatedFXModel;

        [Header("Damage FX")]
        public GameObject bloodSplatterFX;

        [Header("Weapon FX")]
        public WeaponManager rightWeaponManager;
        public WeaponManager leftWeaponManager;

        [Header("Right Weapon Buff")]
        public WeaponBuffEffect rightWeaponBuffEffect;

        [Header("Poison")]
        public Transform buildUpTransform;      //�ж�Ч����λ��

        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Start()
        {
            foreach (var effect in staticCharacterEffects)
            {
                effect.AddStaticEffect(character);
            }
        }

        public virtual void ProcessEffectInstantly(CharacterEffect effect)
        {
            effect.ProecssEffect(character);
        }

        public virtual void ProcessAllTimedEffects()
        {
            effectTickTimer = effectTickTimer + Time.deltaTime;

            if (effectTickTimer >= 1)
            {
                effectTickTimer = 0;
                ProcessWeaponBuffs();
                
                //��ÿ��tick�ڴ�������Ч��
                for (int i = timedEffects.Count -1; i > -1; i--)
                {
                    timedEffects[i].ProecssEffect(character);
                }

                //ÿ��tick���̶�����
                ProcessBuildUpDecay();
            }
        }

        public void ProcessWeaponBuffs()
        {
            if(rightWeaponBuffEffect != null)
            {
                rightWeaponBuffEffect.ProecssEffect(character);
            }
        }

        public void AddStaticEffect(StaticCharacterEffect effect)
        {
            //����б�ȷ��û�����ͬһ��Ч����������
            StaticCharacterEffect staticEffect;

            for (int i = staticCharacterEffects.Count - 1; i > -1; i--)
            {
                if (staticCharacterEffects[i] != null)
                {
                    if (staticCharacterEffects[i].effectID == effect.effectID)
                    {
                        staticEffect = staticCharacterEffects[i];
                        //�Ƴ���ɫ���ϵ�Ч��
                        staticEffect.RemoveStaticEffect(character);
                        //��Ч���б����Ƴ�Ч��
                        staticCharacterEffects.Remove(staticEffect);
                    }
                }
            }

            //��Ч����ӵ���ɫ
            staticCharacterEffects.Add(effect);
            //��Ч����ӵ��б���
            effect.AddStaticEffect(character);

            //����б���û�пյ�Ч�����еĻ��Ƴ�
            for (int i = staticCharacterEffects.Count - 1; i > -1; i--)
            {
                if (staticCharacterEffects[i] == null)
                {
                    staticCharacterEffects.RemoveAt(i);
                }
            }
        }

        public void RemoveStaticEffect(int effectID)
        {
            StaticCharacterEffect staticEffect;

            for (int i = staticCharacterEffects.Count - 1; i > -1; i--)
            {
                if(staticCharacterEffects[i] != null)
                {
                    if (staticCharacterEffects[i].effectID == effectID)
                    {
                        staticEffect = staticCharacterEffects[i];
                        //�Ƴ���ɫ���ϵ�Ч��
                        staticEffect.RemoveStaticEffect(character);
                        //��Ч���б����Ƴ�Ч��
                        staticCharacterEffects.Remove(staticEffect);
                    }
                }
            }

            //����б���û�пյ�Ч�����еĻ��Ƴ�
            for (int i = staticCharacterEffects.Count - 1; i > -1; i--)
            {
                if (staticCharacterEffects[i] == null)
                {
                    staticCharacterEffects.RemoveAt(i);
                }
            }
        }

        public virtual void PlayWeaponFX(bool isLeft)
        {
            if(isLeft == false)
            {
                if(rightWeaponManager != null)
                {
                    rightWeaponManager.PlayWeaponTrailFX();
                }
            }
            else
            {
                if(leftWeaponManager != null)
                {
                    leftWeaponManager.PlayWeaponTrailFX();
                }
            }
        }

        public virtual void PlayBloodSplatterFX(Vector3 bloodSplatterLocation)
        {
            GameObject blood = Instantiate(bloodSplatterFX, bloodSplatterLocation, Quaternion.identity);
        }

        public virtual void InterruptEffect()
        {
            //�����������Ч���ڵĻ����ж���Ч���ں�ҩ����ʹ�õ���ʱʹ�øú�����
            if (instantiatedFXModel != null)
            {
                Destroy(instantiatedFXModel);
            }
            //��������������Ѽ����ȥ���Ƴ���ͷ
            if (character.isHoldingArrow)
            {
                character.animator.SetBool("isHoldingArrow", false);
                Animator rangedWeaponAnimator = character.characterWeaponSlotManager.rightHandSlot.currentWeaponModel.GetComponentInChildren<Animator>();

                if(rangedWeaponAnimator != null)
                {
                    rangedWeaponAnimator.SetBool("isDrawn", false);
                    rangedWeaponAnimator.Play("Bow_TH_Fire_01");
                }
            }

            //�����ɫ������׼���˳���׼״̬
            if(character.isAiming)
            {
                character.animator.SetBool("isAiming", false);
            }
        }

        protected virtual void ProcessBuildUpDecay()
        {
            if(character.characterStatsManager.poisonBuildup > 0)
            {
                character.characterStatsManager.poisonBuildup -= 1;
            }
        }

        public virtual void AddTimedEffectParticle(GameObject effect)
        {
            GameObject effectGameObject = Instantiate(effect, buildUpTransform);
            timedEffectParticles.Add(effectGameObject);
        }

        public virtual void RemoveTimedEffectParticle(EffectParticleType effectType)
        {
            for (int i = timedEffectParticles.Count - 1; i > -1; i--)
            {
                if (timedEffectParticles[i].GetComponent<EffectParticle>().effectType == effectType)
                {
                    Destroy(timedEffectParticles[i]);
                    timedEffectParticles.RemoveAt(i);
                }
            }
        }
    }
}