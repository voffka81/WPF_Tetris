using System;

namespace Tetris
{
    public class BlockQueue
    {
        private readonly Block[] _blocks = new Block[]
            {
                new I_Block(),
                new J_Block(),
                new L_Block(),
                new O_Block(),
                new S_Block(),
                new T_Block(),
                new Z_Block(),
            };

        private readonly Random _random = new Random();

        public Block NextBlock { get; private set; }

        public BlockQueue()
        {
            NextBlock = RandomBlock();
        }

        public Block GetAndUpdate()
        {
            var block = NextBlock;
            do
            {
                NextBlock = RandomBlock();
            } while (block.Id == NextBlock.Id);
            return block;
        }

        private Block RandomBlock()
        {
            return _blocks[_random.Next(_blocks.Length)];
        }
    }
}
