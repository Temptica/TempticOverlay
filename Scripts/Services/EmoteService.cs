using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using Godot.NativeInterop;
using HttpClient = System.Net.Http.HttpClient;

namespace Temptica.Overlay.Scripts.Services;

public class EmoteService
{
    /*const string EmoteUrl = "https://static-cdn.jtvnw.net/emoticons/v2/{id}/static/light/3.0"; //replace {id} with the emote id
    const string EmotePath = "user://emotes/"; //replace {id} with the emote id
    
    public SpriteFrames GetEmote(int emoteId)
    {
        //check if the emote exist in the file system. if not, download it from the url and save it to the file system. Then return the path to the emote
        if (!EmoteExists(emoteId))
        {
            var emoteData = DownloadEmote(emoteId, out var isGif);
            var path = $"{EmotePath}{emoteId}{(isGif?".gif":".png")}";
            SaveEmote(path, emoteData);
            
        }
        
        var emoteDataGet = GetEmoteImage(emoteId);
        var image = new Image();
        image.LoadPngFromBuffer(emoteDataGet);
        var texture = new ImageTexture();
        texture.SetImage(image);
        
        var frames = new SpriteFrames();
        frames.AddFrame("png",texture);
        return frames;
    }
    
    private static void SaveEmote(string fileName, byte[] emoteData )
    {
        //save the emote to the file system
        var access = FileAccess.Open(fileName,FileAccess.ModeFlags.Write);
        access.StoreBuffer(emoteData);
        access.Close();
    }
    
    private bool EmoteExists(int emoteId)
    {
        //check if the emote exists in the file system
        return FileAccess.FileExists($"{EmotePath}{emoteId}.png") || FileAccess.FileExists($"{EmotePath}{emoteId}.gif");
    }
    
    private byte[] GetEmoteImage(int emoteId)
    {
        return FileAccess.GetFileAsBytes(FileAccess.FileExists($"{EmotePath}{emoteId}.png") 
            ? $"{EmotePath}{emoteId}.png" 
            : $"{EmotePath}{emoteId}.gif");
    }
    
    private byte[] DownloadEmote(int emoteId, out bool isGif)
    {
        //download the emote from the url
        isGif = false;
        
        using var client = new HttpClient();
        var response = client.GetAsync(EmoteUrl.Replace("{id}", emoteId.ToString()));
        var data = response.Result.Content.ReadAsByteArrayAsync().Result;
        if (response.Result.Content.Headers.ContentType != null)
        {
            isGif = response.Result.Content.Headers.ContentType.MediaType == "image/gif";
        }
        
        return data;
    }*/
}