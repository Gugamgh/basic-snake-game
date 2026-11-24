using Timer = System.Windows.Forms.Timer;

namespace BasicSnakeGame;

public partial class SnakeGame : Form
{
    // ---------- Board ----------
    private const int CellSize = 40;
    private const int Cols = 30;
    private const int Rows = 20;
    private const int HeaderHeight = 50;
    private const int BoardWidth = Cols * CellSize;
    private const int BoardHeight = Rows * CellSize + HeaderHeight;

    // ---------- Game ----------
    private const int TickMs = 120;
    private const int InitialLength = 3;

    // ---------- State ----------

    // snake[0] is the head of the snake
    private readonly List<Point> snake = new();

    // (1,0) right, (-1,0) left, (0,-1) up, (0,1) down
    private Point dir;
    private Point apple;
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
        BackColor = Color.Black;

        timer.Interval = TickMs;
        timer.Tick += (s, e) => { Step(); Invalidate(); };

        SetupGame();
    }

    private void SetupGame()
    {
        snake.Clear();
        for (int i = 0; i < InitialLength; i++)
        {
            snake.Add(new Point(Cols / 2 - i, Rows / 2));
        }
        dir = new Point(1, 0);
        score = 0;
        isGameOver = false;
        PlaceApple();
        timer.Start();
    }
    private void PlaceApple()
    {
        do
        {
            apple = new Point(rng.Next(Cols), rng.Next(Rows));
        } while (snake.Contains(apple));
    }
    private void Step()
    {
        if (isGameOver) return;

        Point newHead = new Point(snake[0].X + dir.X, snake[0].Y + dir.Y);

        if (IsOutOfBounds(newHead))
        {
            if (dir == new Point(0, 1)) newHead = new Point(newHead.X, 0);
            else if (dir == new Point(0, -1)) newHead = new Point(newHead.X, Rows - 1);
            else if (dir == new Point(1, 0)) newHead = new Point(0, newHead.Y);
            else if (dir == new Point(-1, 0)) newHead = new Point(Cols - 1, newHead.Y);
        }

        if (IsSelfCollision(newHead))
        {
            GameOver();
            return;
        }

        snake.Insert(0, newHead);

        if (newHead == apple)
        {
            score++;
            PlaceApple();
        }
        else
        {
            snake.RemoveAt(snake.Count - 1);
        }
    }
    private bool IsOutOfBounds(Point cell)
    {
        return cell.X < 0 || cell.X >= Cols || cell.Y < 0 || cell.Y >= Rows;
    }
    private bool IsSelfCollision(Point cell)
    {
        return cell != snake[snake.Count - 1] && snake.Contains(cell);
    }
    private void GameOver()
    {
        isGameOver = true;
        timer.Stop();
    }
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.KeyCode == Keys.Up || e.KeyCode == Keys.W)
        {
            if (dir != new Point(0, 1)) dir = new Point(0, -1);
        }
        else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.S)
        {
            if (dir != new Point(0, -1)) dir = new Point(0, 1);
        }
        else if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
        {
            if (dir != new Point(1, 0)) dir = new Point(-1, 0);
        }
        else if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D)
        {
            if (dir != new Point(-1, 0)) dir = new Point(1, 0);
        }

        if (e.KeyCode == Keys.Space && isGameOver)
        {
            SetupGame();
            Invalidate();
        }
    }
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;

        DrawHeader(g);
        DrawApple(g);
        DrawSnake(g);
        if (isGameOver) DrawGameOver(g);
    }
    private Rectangle CellToRect(Point cell)
    {
        int x = cell.X * CellSize;
        int y = cell.Y * CellSize + HeaderHeight;

        return new Rectangle(x, y, CellSize, CellSize);
    }
    private void DrawSnake(Graphics g)
    {
        for (int i = 1; i < snake.Count; i++)
        {
            g.FillRectangle(Brushes.Green, CellToRect(snake[i]));
        }

        g.FillRectangle(Brushes.LimeGreen, CellToRect(snake[0]));
    }
    private void DrawApple(Graphics g)
    {
        g.FillRectangle(Brushes.Red, CellToRect(apple));
    }
    private void DrawHeader(Graphics g)
    {
        g.FillRectangle(Brushes.DarkGreen, 0, 0, BoardWidth, HeaderHeight);
        g.DrawString("Score: " + score, new Font("Segoe UI", 16), Brushes.White, 10, 10);
    }
    private void DrawGameOver(Graphics g)
    {
        SizeF sizeGameOver = g.MeasureString("Game Over", new Font("Segoe UI", 32));
        SizeF sizeSpace = g.MeasureString("Press space to restart", new Font("Segoe UI", 16));
        int x1 = (BoardWidth - (int)sizeGameOver.Width) / 2;
        int y1 = (BoardHeight - (int)sizeGameOver.Height) / 2;
        int x2 = (BoardWidth - (int)sizeSpace.Width) / 2;
        int y2 = y1 + (int)sizeGameOver.Height + 10;

        g.DrawString("Game Over", new Font("Segoe UI", 32), Brushes.White, x1, y1);
        g.DrawString("Press space to restart", new Font("Segoe UI", 16), Brushes.White, x2, y2);
    }
}