// using System.Text.Json.Serialization;

using Snake.Common;

namespace Snake.Models
{
    public class GameStatus : BaseModel
    {
        public bool IsActive { get; set; }
        public int Score { get; set; }
        public short Level { get; set; }
        
        // public string Player { get; set; }
        // [JsonIgnore] public bool IsCurrentPlayer { get; set; }
    }
}