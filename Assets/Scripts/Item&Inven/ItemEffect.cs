using UnityEngine;

public abstract class ItemEffect : ScriptableObject //추상클래스 ScriptableObject 상속 받음
{
    public abstract bool ExecuteRole(); //추상 메소드
}
