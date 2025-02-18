namespace menhera;

[Serializable]
public struct SkillData
{
    public int basePower;
    public int coinPower;
    public int coinCount;

    // coin effects?
    public Effect[] coinEffects;
}