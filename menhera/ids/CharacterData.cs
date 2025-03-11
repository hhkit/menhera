namespace menhera
{
    [Serializable]
    public struct CharacterData
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public int MaxHp { get; set; }
        public int[] StaggerThresholds { get; set; }
        public SkillData[] Skills { get; set; }
    }
}