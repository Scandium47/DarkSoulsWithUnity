using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class AmmoPoolManager : MonoBehaviour
    {
        //单例模式
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
                //遍历所有池，并给每个池创建一个队列
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    //实例化池子预设大小个预制体，设置为非激活，加入队列
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                //将池子存入字典，通过tag来查找
                poolDictionary.Add(pool.tag, objectPool);
            }
        }

        public GameObject GetPooledObject(string tag, Vector3 position, Quaternion rotation)
        {
            //如果没有找到对应tag的池子，返回空
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
                return null;
            }

            // 如果池空了就扩展池
            if (poolDictionary[tag].Count == 0)
            {
                ExpandPool(tag, 5); // 每次扩展5个
            }

            //如果找到对应tag的池子，从队列中取出实例化的物体并设置属性
            GameObject obj = poolDictionary[tag].Dequeue();

            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);

            // 重置箭矢状态
            var damageCollider = obj.GetComponent<RangedProjectileDamageCollider>();
            if (damageCollider != null)
            {
                damageCollider.ResetArrowState();
            }

            return obj;
        }

        public void ReturnToPool(string tag, GameObject obj)
        {
            //物体隐藏并回到队列
            obj.SetActive(false);
            poolDictionary[tag].Enqueue(obj);
        }

        void ExpandPool(string tag, int count)
        {
            Pool targetPool = pools.Find(p => p.tag == tag);
            if (targetPool == null) return;

            for (int i = 0; i < count; i++)
            {
                //实例化一定数量的物体并加入到队列中
                GameObject newObj = Instantiate(targetPool.prefab);
                newObj.SetActive(false);
                poolDictionary[tag].Enqueue(newObj);
            }
        }
    }
}