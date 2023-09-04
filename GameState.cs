namespace Tetris
{
    public class GameState
    {
        private Block _currentBlock;

        public Block CurrentBlock
        {
            get => _currentBlock;
            private set
            {
                _currentBlock = value;
                _currentBlock.Reset();
                for (int row = 0; row < 2; row++)
                {
                    MoveBlockDown();
                }
            }
        }

        public GameGrid GameGrid { get; }
        public BlockQueue BlockQueue { get; }
        public bool GameOver { get; private set; }
        public int Score { get; private set; }

        public GameState()
        {
            Score = 0;
            GameGrid = new GameGrid(22, 10);
            BlockQueue = new BlockQueue();
            CurrentBlock = BlockQueue.GetAndUpdate();
        }

        private bool BlockFits()
        {
            foreach (var pos in CurrentBlock.TilePositions())
            {
                if (!GameGrid.IsEmpty(pos.Row, pos.Column))
                {
                    return false;
                }
            }
            return true;
        }

        public void RotateBlockCW()
        {
            CurrentBlock.RotateClockWise();
            if (!BlockFits()) { CurrentBlock.RotateCountClockWise(); }
        }

        public void RotateBlockCCW()
        {
            CurrentBlock.RotateCountClockWise();
            if (!BlockFits()) { CurrentBlock.RotateClockWise(); }
        }

        public void MoveBlockLeft()
        {
            CurrentBlock.Move(0, -1);
            if (!BlockFits()) { CurrentBlock.Move(0, 1); }
        }
        public void MoveBlockRight()
        {
            CurrentBlock.Move(0, 1);
            if (!BlockFits()) { CurrentBlock.Move(0, -1); }
        }
        public void MoveBlockDown()
        {
            CurrentBlock.Move(1, 0);
            if (!BlockFits())
            {
                CurrentBlock.Move(-1, 0);
                PlaceBlock();
            }
        }
        private bool IsGameOver()
        {
            return !(GameGrid.IsRowEmpty(0) && GameGrid.IsRowEmpty(1));
        }

        private void PlaceBlock()
        {
            foreach (var pos in CurrentBlock.TilePositions())
            {
                GameGrid[pos.Row, pos.Column] = CurrentBlock.Id;
            }
            Score += GameGrid.ClearFullRows();
            if (IsGameOver())
            {
                GameOver = true;
            }
            else
            {
                CurrentBlock = BlockQueue.GetAndUpdate();
            }
        }

        public void DropBlock()
        {
            CurrentBlock.Move(BlockDropDistance(), 0);
            PlaceBlock();
        }

        public int BlockDropDistance()
        {
            int drop = GameGrid.Rows;
            foreach (Position pos in CurrentBlock.TilePositions())
            {
                drop = System.Math.Min(drop, BlockDropDistance(pos));
            }
            return drop;
        }

        private int BlockDropDistance(Position pos)
        {
            int drop = 0;
            while (GameGrid.IsEmpty(pos.Row + drop + 1, pos.Column))
            {
                drop++;
            }
            return drop;
        }
    }
}
