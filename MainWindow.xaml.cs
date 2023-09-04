using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SolidColorBrush[] _tileColors = new SolidColorBrush[]
        {
            Brushes.Black,
            Brushes.Cyan,
            Brushes.Blue,
            Brushes.Orange,
            Brushes.Yellow,
            Brushes.Green,
            Brushes.Purple,
            Brushes.Red,
        };
        private readonly Tile[,] _tileControls;
        private readonly Tile[,] _tileNextBlock;
        private GameState _gameState = new GameState();
        private const int CELL_SIZE = 25;
        private const int MAX_DELAY = 1000;
        private const int MIN_DELAY = 100;
        private const int DELAY_STEP = 25;

        public MainWindow()
        {
            InitializeComponent();
            _tileControls = SetupGameCanvas(_gameState.GameGrid);
            _tileNextBlock = SetupNextBlockCanvas();

        }

        private Tile[,] SetupGameCanvas(GameGrid grid)
        {
            Tile[,] tileControls = new Tile[grid.Rows, grid.Columns];

            for (int r = 0; r < grid.Rows; r++)
                for (int c = 0; c < grid.Columns; c++)
                {
                    var tileControl = new Tile()
                    {
                        Width = CELL_SIZE,
                        Height = CELL_SIZE,
                    };
                    Canvas.SetTop(tileControl, (r - 2) * CELL_SIZE + 10);
                    Canvas.SetLeft(tileControl, c * CELL_SIZE);
                    GameCanvas.Children.Add(tileControl);
                    tileControls[r, c] = tileControl;
                }
            return tileControls;
        }

        private Tile[,] SetupNextBlockCanvas()
        {
            int hintBlockSize = 4;

            Tile[,] tileControls = new Tile[4, 4];
            int cellsize = 25;
            for (int r = 0; r < hintBlockSize; r++)
                for (int c = 0; c < hintBlockSize; c++)
                {
                    var tileControl = new Tile()
                    {
                        Width = cellsize,
                        Height = cellsize,
                    };
                    Canvas.SetTop(tileControl, r * cellsize);
                    Canvas.SetLeft(tileControl, c * cellsize);
                    NextBlockCanvas.Children.Add(tileControl);
                    tileControls[r, c] = tileControl;
                }
            return tileControls;
        }
        private void DrawGhostBlock(Block block)
        {
            int dropDistance = _gameState.BlockDropDistance();
            foreach (var pos in block.TilePositions())
            {
                _tileControls[pos.Row + dropDistance, pos.Column].Opacity = 0.15;
                _tileControls[pos.Row + dropDistance, pos.Column].ColorBrush = _tileColors[block.Id];
            }
        }

        private void DrawGrid(GameGrid grid)
        {
            for (int r = 0; r < grid.Rows; r++)
                for (int c = 0; c < grid.Columns; c++)
                {
                    int id = grid[r, c];
                    _tileControls[r, c].Opacity = 1;
                    _tileControls[r, c].ColorBrush = _tileColors[id];
                }
        }

        private void DrawBlock(Block block)
        {
            foreach (var pos in block.TilePositions())
            {
                _tileControls[pos.Row, pos.Column].Opacity = 1;
                _tileControls[pos.Row, pos.Column].ColorBrush = _tileColors[block.Id];
            }
        }

        private void DrawNextBlock(BlockQueue blockQueue)
        {
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                {
                    _tileNextBlock[r, c].ColorBrush = Brushes.Black;
                }

            var block = blockQueue.NextBlock;
            foreach (var pos in block.GetRawPositions())
            {
                _tileNextBlock[pos.Row, pos.Column].ColorBrush = _tileColors[block.Id];
            }
        }

        private void Draw(GameState gameState)
        {
            DrawGrid(gameState.GameGrid);
            DrawBlock(gameState.CurrentBlock);
            //DrawGhostBlock(gameState.CurrentBlock);
            DrawNextBlock(gameState.BlockQueue);

            ScoreText.Text = _gameState.Score.ToString();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (_gameState.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.A:
                case Key.Left:
                    _gameState.MoveBlockLeft();
                    break;
                case Key.D:
                case Key.Right:
                    _gameState.MoveBlockRight();
                    break;
                case Key.S:
                case Key.Down:
                    _gameState.MoveBlockDown();
                    break;
                case Key.Up:
                case Key.W:
                    _gameState.RotateBlockCW();
                    break;
                case Key.Q:
                    _gameState.RotateBlockCCW();
                    break;
                case Key.Space:
                    _gameState.DropBlock();
                    break;
                default:
                    return;
            }
            Draw(_gameState);
        }

        private async Task GameLoop()
        {
            Draw(_gameState);
            while (!_gameState.GameOver)
            {
                var delay = Math.Max(MIN_DELAY, MAX_DELAY - (_gameState.Score * DELAY_STEP));
                await Task.Delay(delay);
                _gameState.MoveBlockDown();
                Draw(_gameState);
            }
            GameOverMenu.Visibility = Visibility.Visible;
            FinalScoreText.Text = _gameState.Score.ToString();
        }

        private async void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            GameMenu.Visibility = Visibility.Hidden;
            _gameState = new GameState();
            GameOverMenu.Visibility = Visibility.Hidden;
            await GameLoop();
        }
    }
}
