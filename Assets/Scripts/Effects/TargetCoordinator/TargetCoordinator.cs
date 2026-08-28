using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class TargetCoordinator
{
    [SerializeField] private bool includeSelf;
    [SerializeField] private bool hasBounds;
    [SerializeField] private bool isDynamic;
    [SerializeField] private GameObject area;

    public void Initialize(Affecter.TargetLabel targetMode, EffectContext context)
    {
        if(targetMode == Affecter.TargetLabel.self || targetMode == Affecter.TargetLabel.environment) 
            context.AddTarget(context.Owner.Body);
    }

    public async Task ValidateArea(EffectContext context)
    {
        GameObject g = Object.Instantiate(area, GetOrigin(context), GetRotation(context));
        for(int i = 0; i < g.transform.childCount; i++)
        {
            float range = GetRange(context);
            Vector2 dirNormalized = g.transform.GetChild(i).transform.localPosition.normalized;
            g.transform.GetChild(i).transform.localPosition = dirNormalized * range;
        }

        TargetAreaManager tam = g.GetComponent<TargetAreaManager>();
        List<GameObject> objsDetected = await tam.RetrieveValidTargets();
        Object.Destroy(g);
        
        if(!includeSelf) objsDetected.RemoveAll( g => g == context.Owner.Body);
        
        for(int i = 0; i < objsDetected.Count; i++)
        {
            context.AddTarget(objsDetected[i]);
        }
    }

    public Vector2 GetSpawnPosition(EffectContext context) => GetOrigin(context) + GetDirection(context) * GetRange(context);

    public Vector2 GetOrigin(EffectContext context) => !isDynamic ? context.Owner.Body.transform.position : MousePos;
    private float GetRange(EffectContext context) => context.Owner.Body.CompareTag("Player") ? 1.25f : 1.625f;
    public Vector2 GetDirection(EffectContext context)
    {
        Vector2 areaPos = Vector2.right;//use the coordinator field when implemented
        Vector2 worldPos = context.Owner.Orientation.CurrentOrientation;

        return new(areaPos.x * worldPos.x - areaPos.y * worldPos.y, areaPos.x * worldPos.y + areaPos.y * worldPos.x);
    }

    private Quaternion GetRotation(EffectContext context)
    {
        Vector2 dir = GetDirection(context);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0, angle);
    }

    private Vector2 MousePos => Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
}
