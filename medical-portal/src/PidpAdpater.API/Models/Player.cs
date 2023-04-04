namespace pdipadapter.Models;

[Table("Players")]
public class Player : BaseAuditable
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int ShirtNo { get; set; }
    public int Apperance { get; set; } 

    public int Goals { get; set; }
}
