using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class AmmoPoolManager : MonoBehaviour
    {
        //����ģʽ
        public static AmmoPoolManager Instance;

        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        public List<Pool> pools;
        public Dictionary<string, Queue<GameObject>> poolDictionary;

        void Awake()
        {
            Instance = this;
            InitializePools();
        }

        void InitializePools()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (Pool pool in pools)
            {
                //�������гأ�����ÿ���ش���һ������
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    //ʵ��������Ԥ���С��Ԥ���壬����Ϊ�Ǽ���������
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                //�����Ӵ����ֵ䣬ͨ��tag������
                poolDictionary.Add(pool.tag, objectPool);
            }
        }

        public GameObject GetPooledObject(string tag, Vector3 position, Quaternion rotation)
        {
            //���û���ҵ���Ӧtag�ĳ��ӣ����ؿ�
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
                return null;
            }

            // ����ؿ��˾���չ��
            if (poolDictionary[tag].Count == 0)
            {
                ExpandPool(tag, 5); // ÿ����չ5��
            }

            //����ҵ���Ӧtag�ĳ��ӣ��Ӷ�����ȡ��ʵ���������岢��������
            GameObject obj = poolDictionary[tag].Dequeue();

            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);

            // ���ü�ʸ״̬
            var damageCollider = obj.GetComponent<RangedProjectileDamageCollider>();
            if (damageCollider != null)
            {
                damageCollider.ResetArrowState();
            }

            return obj;
        }

        public void ReturnToPool(string tag, GameObject obj)
        {
            //�������ز��ص�����
            obj.SetActive(false);
            poolDictionary[tag].Enqueue(obj);
        }

        void ExpandPool(string tag, int count)
        {
            Pool targetPool = pools.Find(p => p.tag == tag);
            if (targetPool == null) return;

            for (int i = 0; i < count; i++)
            {
                //ʵ����һ�����������岢���뵽������
                GameObject newObj = Instantiate(targetPool.prefab);
                newObj.SetActive(false);
                poolDictionary[tag].Enqueue(newObj);
            }
        }
    }
}