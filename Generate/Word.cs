namespace SweetFields.Generate;

class Word
{
    public int ID { get; set; }
    public int Number { get; set; }
    public string Answer { get; set; }
    public string Question { get; set; }
    public int Level { get; set; }
    public bool IsUsed { get; set; }
    public Point Location { get; set; }
    public bool IsVertical { get; set; }
    public bool IsHorizontal { get; set; }
}

