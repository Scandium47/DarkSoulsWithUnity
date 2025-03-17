using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : MonoBehaviour
{
    //�����
    //����������ȴ���һ�����Ķ���Ȼ���䵽�˻��߽����Ϻ�һ��ʱ���ڻ��ߴﵽһ����������
    //������� �Ӽ�����ȡ���������ٺ��ٴ�ŵ������ڣ���ʡ�˴��������������ĵ�����

    //�������б�Ҳ������ջ
    //public Stack<GameObject> stack = new Stack<GameObject>();

    public List<GameObject> list = new List<GameObject>();
    //��ϷԤ����
    public GameObject GoPrefab;
    public int MaxCount = 100;

    //�����󱣴浽�������
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

    //�Ӷ������ȡ��һ������
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

    //��������
    public void Clear()
    {
        list.Clear();
    }
}
