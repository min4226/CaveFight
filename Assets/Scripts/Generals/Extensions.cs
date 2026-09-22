using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;


public static class Extensions
{
    public static T GetMaximum<T>(this IEnumerable targetList, System.Func<T, float> ScoreFunction)
    {
        T result = default;
        float firstScore = float.MinValue;


        foreach (T currentTarget in targetList)
        {
            float currentScore = ScoreFunction(currentTarget);

            if (currentScore > firstScore)
            {
                result = currentTarget;
                firstScore = currentScore;

            }
        }
        return result;
    }
    public static T GetMinimum<T>(this IEnumerable targetList, System.Func<T, float> ScoreFunction)
    {
        T result = default;
        float firstScore = float.MaxValue;
        

        foreach (T currentTarget in targetList)
        { 
            float currentScore = ScoreFunction(currentTarget);

            if (currentScore < firstScore)
            {
                result = currentTarget;
                firstScore = currentScore;
                
            }
        }
        return result;
    }
    public static T GetExtreme<T>(this IEnumerable targetList, float defaultScore,
        System.Func<T, float> Evalueator,
        System.Func<float, float, bool> Comparison)
    {
        T result = default;
        float firstScore = defaultScore;

        foreach (T currentTarget in targetList)
        { 
            float currentScore = Evalueator(currentTarget);

            if (Comparison(currentScore, firstScore))
            {
                result = currentTarget;
                firstScore = currentScore;
            }
        }
        return result;
    }
    public static float normalized(this float target)
    {
        if (target > 0) return 1.0f;
        else if (target < 0) return -1.0f;
        else return 0.0f;
    }

    
    public static T TryAddComponent<T>(this GameObject target) where T : Component
    {
        T result = null;
        if (target == null) return result; //RVO

        result = target.GetComponent<T>() ?? target.AddComponent<T>();

        

        return result;
    }

    public static T TryAddComponent<T>(this Component target) where T : Component
    {
        if (target == null) return null;
        else return target.gameObject.TryAddComponent<T>(); 						   TryAddComponent<T>(target.gameObject);
    }

    public static IEnumerator WaitForTask(this Task targetTask)
    {
        
        yield return new WaitUntil(() => targetTask.IsCompleted);
        
        targetTask.Dispose();
    }

    public static float GetPenetratedDistance(float aHalf, float bHalf, float aPos, float bPos)
    {
        float absAHalf = Mathf.Abs(aHalf);
        float absBHalf = Mathf.Abs(bHalf);
        //그래서 겹쳤다면, 만약에 원래 안 겹쳤을 때에 있을 수 있는 공간
        float minSpace = absAHalf + absBHalf;
        //지금 이 둘 사이의 거리가 얼마나 가까운지!
        float distance = aPos - bPos;
        //x최소 거리와 둘 사이의 거리 차이! => 예외처리!
        float penetration = minSpace - Mathf.Abs(distance);
        //어느 방향으로 묻혀있는지 확인하는 것도 중요!
        //A가 왼쪽 => +로 보여줄까 -로 보여줄까!
        //distance의 부호를 그대로 따라가게 하려면!
        //              마이너스면 -1 / 0이상이면 1
        penetration *= Mathf.Sign(distance);
        return penetration;
    }

    
    public static Vector2 AABB(this Rect A, Rect B)
    {
        Vector2 result = Vector2.zero;
        Vector2 aMin = A.min;
        Vector2 aMax = A.max;
        Vector2 aHalf = A.size * .5f;
        Vector2 bMin = B.min;
        Vector2 bMax = B.max;
        Vector2 bHalf = B.size * .5f;

        //한 쪽의 최대 위치가 다른 쪽의 최소 위치보다 높아야 함!
        if (aMax.x > bMin.x && bMax.x > aMin.x)
        {
            result.x = GetPenetratedDistance(aHalf.x, bHalf.x, A.position.x, B.position.x);
        }
        if (aMax.y > bMin.y && bMax.y > aMin.y)
        {
            result.y = GetPenetratedDistance(aHalf.y, bHalf.y, A.position.y, B.position.y);
        }
        return result;
    }

    
    public static float GetOutboundDistance(float inMin, float outMin, float inMax, float outMax)
    {
        float result = 0.0f;

        //전체 맵보다 카메라가 커요! 와이드스크린인가봐요!
        bool leftOut = inMin < outMin;
        bool rightOut = inMax > outMax;
        
        if (leftOut ^ rightOut)
        {
            if (leftOut) result = outMin - inMin;
            if (rightOut) result = outMax - inMax;
        }
        return result;
    }

    
    public static Vector2 InversedAABB(this Rect target, Rect bound)
    {
        Vector2 result;
        result.x = GetOutboundDistance(target.xMin, bound.xMin, target.xMax, bound.xMax);
        result.y = GetOutboundDistance(target.yMin, bound.yMin, target.yMax, bound.yMax);
        return result;
    }
}