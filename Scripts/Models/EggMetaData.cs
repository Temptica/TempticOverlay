namespace Temptica.Overlay.Scripts.Models;

public class EggMetaData
{
    public string Name { get; set; }
    public EggData Data { get; set; }
    public string TwitchId { get; set; }
    public string UserName { get; set; }
    public string UpdatedAt { get; set; }
    public string CreatedAt { get; set; }

    public class EggData
    {
        public string Image { get; set; }
    }
}

