using TadPoleFramework.Core;

namespace _GameFiles.Scripts.EventArgs
{
    public class SceneStartedEventArgs : BaseEventArgs
    {
        public int HighScore { get; set; }
        public SceneStartedEventArgs()
        {
            
        }

        public SceneStartedEventArgs(int highScore)
        {
            HighScore = highScore;
        }
    }

    public class GridCreatedEventArgs : BaseEventArgs
    {
        
    }
    public class GridDestroyedEventArgs : BaseEventArgs
    {
        
    }

    public class PieceSelectedEventArgs : BaseEventArgs
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public PieceSelectedEventArgs(params int[] coordination)
        {
            Row = coordination[0];
            Column = coordination[1];
        }
    }

    public class PieceExplodedEventArgs : BaseEventArgs
    {
        
    }
    public class TimeIsFinishedEventArgs : BaseEventArgs
    {
        
    }

    
}