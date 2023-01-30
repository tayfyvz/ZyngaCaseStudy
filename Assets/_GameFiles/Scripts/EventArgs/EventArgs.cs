using _GameFiles.Scripts.Controllers;
using TadPoleFramework.Core;

//Contains event classes.
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
        public PieceController PieceController { get; set; }

        public PieceSelectedEventArgs(PieceController pieceController)
        {
            PieceController = pieceController;
        }
    }

    public class PieceExplodedEventArgs : BaseEventArgs
    {
        
    }

    public class SwipedEventArgs : BaseEventArgs
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public SwipedEventArgs(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
    public class TimeIsFinishedEventArgs : BaseEventArgs
    {
        
    }
}