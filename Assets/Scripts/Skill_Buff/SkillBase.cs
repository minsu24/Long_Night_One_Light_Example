using System;
using UnityEngine;

public abstract class SkillBase : MonoBehaviour, ISkill
{
    [SerializeField] float coolDown = 5f;

    float _nextReadytime;

    public bool IsReady
    {
        get { return Time.time >= _nextReadytime; }
    }
    public virtual void ExecuteSkill()
    {
        if (!IsReady)
        {
            Debug.Log("스킬 준비 안됨!");
            return;
        }

        OnCast();
        _nextReadytime = Time.time + coolDown;
    }
    protected abstract void OnCast();

    

}
