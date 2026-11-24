using Timer = System.Windows.Forms.Timer;

namespace BasicSnakeGame;

public partial class SnakeGame : Form
{
    private const int CellSize = 20;
    private const int Cols = 30;          
    private const int Rows = 20;              
    private const int HeaderHeight = 40;         

    private const int BoardWidth = Cols * CellSize;
    private const int BoardHeight = Rows * CellSize + HeaderHeight;
    private const int TickMs = 120;         
    private const int InitialLength = 3;

    private readonly List<Point> snake = new();
    private Point dir;                           
    private Point food;
    private int score;
    private bool isGameOver;
    private readonly Random rng = new();
    private readonly Timer timer = new();

    public SnakeGame()
    {
        InitializeComponent();
        ClientSize = new Size(BoardWidth, BoardHeight);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        DoubleBuffered = true;

        timer.Interval = TickMs;
    }
}
