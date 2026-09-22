using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class ObjectPoolModule
{
    PoolSetting _setting;
    public PoolSetting Setting => _setting;
    Transform rootTransform;
    Queue<GameObject> prepareQueue = new();
    public ObjectPoolModule(PoolSetting newSetting)
    { 
        _setting = newSetting;
    }

    public void Initialize()
    {
        rootTransform = new GameObject(Setting.poolName).transform;
        Setting.target?.TryAddComponent<PooledObject>();
        PrepareObjects(Setting.countInitial);
    }

    GameObject PrepareObject()
    {
        if (!Setting.target) return null;
        GameObject result = CreateFromPrefab();
        EnqueueObject(result);
        return result;
    }

    void PrepareObjects(uint count)
    {
        if (!Setting.target) return;
        for (uint i = 0; i < count; i++)
        {
            GameObject result = ObjectManager.CreateObject(Setting.target, rootTransform);
            EnqueueObject(result);
        }
    }

    void PrepareObjects(uint count, out GameObject activeObject)
    {
        if (!Setting.target)
        {
            activeObject = null;
            return;
        }
        activeObject = CreateFromPrefab();
        for (uint i = 1; i < count; i++)
        {
            GameObject result = CreateFromPrefab();
            EnqueueObject(result);
        }
    }

    public GameObject CreateFromPrefab()
    {
        GameObject result = ObjectManager.CreateObject(Setting.target, rootTransform);
        if (result)
        {
            result.name = Setting.poolName;
            if (result.TryGetComponent(out PooledObject pool))
            {
                pool.OnEnqueueEvent -= DestroyObject;
                pool.OnEnqueueEvent += DestroyObject;
            }
        }
        
        return result;
    }

    public GameObject CreateObject(Transform parent = null)
    {
        GameObject result;
        if (!prepareQueue.TryDequeue(out result))
        {
            PrepareObjects(Setting.countAdditional, out result);
        }
        if (result)
        {
            if (result.TryGetComponent(out PooledObject pool))
            {
                pool.OnDequeue();
            }
            result.SetActive(true);
            Transform currentTransform = result.transform;
            Transform originTransform = Setting.target.transform;

            currentTransform.SetParent(parent);

            if (currentTransform is RectTransform asRectTransform
                && originTransform is RectTransform originRectTransform)
            {
                asRectTransform.anchorMin = originRectTransform.anchorMin;
                asRectTransform.anchorMax = originRectTransform.anchorMax;

                asRectTransform.pivot = originRectTransform.pivot;

                if (parent)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(parent.transform as RectTransform);
                }
                

                bool stretchX = asRectTransform.anchorMin.x != asRectTransform.anchorMax.x;
                bool stretchY = asRectTransform.anchorMin.y != asRectTransform.anchorMax.y;

                if (stretchX || stretchY)
                {
                    asRectTransform.offsetMax = originRectTransform.offsetMax;
                    asRectTransform.offsetMin = originRectTransform.offsetMin;

                    /*if (stretchX)
                    {
                        asRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, originRectTransform.offsetMin.x, 0);
                        asRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, originRectTransform.offsetMax.x, 0);
                    }
                    if (stretchY)
                    {
                        asRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, originRectTransform.offsetMin.y, 0);
                        asRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, originRectTransform.offsetMax.y, 0);
                    }*/
                }
                else
                {
                    asRectTransform.anchoredPosition = originRectTransform.anchoredPosition;
                    asRectTransform.sizeDelta = originRectTransform.sizeDelta;
                }
            }
            else
            { 
                currentTransform.localPosition = originTransform.localPosition;
            }
            currentTransform.localRotation = originTransform.localRotation;
            currentTransform.localScale = originTransform.localScale;

            
        }
        return result;
    }

    public void DestroyObject(GameObject target)
    {
        EnqueueObject(target);
        if (target)
        {
            target.transform.SetParent(rootTransform);
        }
    }

    public void EnqueueObject(GameObject target)
    {
        if (target)
        { 
            target.SetActive(false);
            
            prepareQueue.Enqueue(target);
        }
    }
}
