using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Temptica.Overlay.Scripts.LaserShow;

public class ShowLoader
{
    public string SongId { get; set; }
    public string SongName { get; set; }
    
    public int LaserGroupCount { get; set; }
    public int LaserCount { get; set; }
    
    public List<LaserGroup> LaserGroups { get; set; }
    public List<LaserKeyFrame> LaserKeyFrames { get; set; }
    
    public static ShowLoader Load(string path)
    {
        var json = File.ReadAllText(path);
        var showLoader = JsonConvert.DeserializeObject<ShowLoader>(json);
        showLoader.LaserKeyFrames = showLoader.LaserKeyFrames.OrderBy(x => x.Time).ToList();
        return showLoader;
    }
    
}

