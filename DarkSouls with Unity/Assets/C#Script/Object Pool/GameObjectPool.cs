using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : MonoBehaviour
{
    //对象池
    //弓箭射箭是先创建一个箭的对象，然后射到人或者建筑上后，一定时间内或者达到一定数量销毁
    //对象池是 从集合里取出箭→销毁后再存放到集合内，节省了创建和销毁所消耗的性能

    //可以用列表，也可以用栈
    //public Stack<GameObject> stack = new Stack<GameObject>();

    public List<GameObject> list = new List<GameObject>();
    //游戏预设体
    public GameObject GoPrefab;
    public int MaxCount = 100;

    //将对象保存到对象池中
    public void Push(GameObject go)
    {
        go.SetActive(false);
        if (list.Count < MaxCount)
        {
            list.Add(go);
        }
        else
        {
            Destroy(go);
        }
    }

    //从对象池中取出一个对象
    public GameObject Pop()
    {
        if(list.Count > 0)
        {
            GameObject go = list[0];
            list.RemoveAt(0);
            go.SetActive(true);
            return go;
        }
        else
        {
            return Instantiate(GoPrefab);
        }
    }

    //清除对象池
    public void Clear()
    {
        list.Clear();
    }
}
