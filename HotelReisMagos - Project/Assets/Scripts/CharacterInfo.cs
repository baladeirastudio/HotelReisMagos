using System;
using UnityEngine;


[CreateAssetMenu(fileName = "New Character Info", menuName = "Caracter Info", order = 0)]
public class CharacterInfo : ScriptableObject
{
    [SerializeField] private new string name;
    
    [SerializeField, TextArea(3, 6)] private string description;
    [SerializeField, TextArea(3, 6)] private string decription2;
    [SerializeField, TextArea(3, 6)] private string description3;

    [Serializable]
    public class ActBonus
    {
        public int mediaBonus, economicBonus, socialBonus, politicBonus;
        [TextArea(3, 6)] public string onReceiveBonusText;
    }

    [SerializeField] private int mediaGoal, economicGoal, socialGoal, politicGoal;

    [SerializeField] private ActBonus secondActBonus, thirdActBonus;

    public int MediaGoal => mediaGoal;

    public int EconomicGoal => economicGoal;

    public int SocialGoal => socialGoal;

    public int PoliticGoal => politicGoal;

    public ActBonus SecondActBonus => secondActBonus;

    public ActBonus ThirdActBonus => thirdActBonus;

    public string Name => name;

    public string Description => description;
    
}