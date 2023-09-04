using System.Windows.Controls;
using System.Windows.Media;

namespace Tetris
{
    /// <summary>
    /// Interaction logic for Tile.xaml
    /// </summary>
    public partial class Tile : UserControl
    {
        public Tile(Brush color)
        {
            InitializeComponent();
            singleBlock.Background = color;
        }
        public Tile()
        {
            InitializeComponent();
        }

        public Brush ColorBrush
        {
            set { singleBlock.Background = value; }
        }
    }
}
