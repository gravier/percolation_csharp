using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using Percolation.Properties;

namespace Percolation
{
    public partial class PercolationVisualizerForm : Form
    {
        private SolidBrush _brush;
        private Graphics _g;
        private BackgroundWorker _bw;

        public PercolationVisualizerForm()
        {
            InitializeComponent();

            _brush = new SolidBrush(Color.Black);
            _g = pnlCanvas.CreateGraphics();
            _bw = new BackgroundWorker();
            _bw.WorkerReportsProgress = true;
            _bw.WorkerSupportsCancellation = true;
            _bw.DoWork += bw_DoWork;
            _bw.ProgressChanged += bw_ProgressChanged;
        }

        /// <summary>
        ///  draw N-by-N percolation system
        /// </summary>
        /// <param name="perc">Percolation state manager</param>
        private void draw(Percolation perc)
        {
            int openedCount = 0;
            int N = perc.size;
            float rSizeW = pnlCanvas.ClientRectangle.Width / N;
            float rSizeH = pnlCanvas.ClientRectangle.Height / N;
            
            // draw N-by-N grid
            for (int row = 1; row <= N; row++)
            {
                for (int col = 1; col <= N; col++)
                {
                    if (perc.isFull(row, col))
                    {
                        _brush.Color = Color.LightBlue;
                        openedCount++;
                    }
                    else if (perc.isOpen(row, col))
                    {
                        _brush.Color = Color.White;
                        openedCount++;
                    }
                    else
                        _brush.Color = Color.Black;

                    _g.FillRectangle(_brush, (col - 1) * rSizeW, (row - 1) * rSizeH, rSizeW, rSizeH);
                }
            }
            writeStatus(openedCount, perc.doesPercolate());
        }

        /// <summary>
        /// write status text
        /// </summary>
        private void writeStatus(int openedCount, bool doesPercolate)
        {
            string statusText = string.Format(Resources.STATUS_TEXT_OPEN_SITES, openedCount, doesPercolate ? Resources.STATUS_TEXT_PERCOLATES : Resources.STATUS_TEXT_DOES_NOT_PERCOLATE);
            lblStatus.Text = statusText;
        }

        private void runPercolation(int size, int delay)
        {
            Percolation perc = new Percolation(size);
            int i, j;
            Random r = new Random();
            while (!perc.doesPercolate() && !_bw.CancellationPending)
            {
                i = r.Next(1, size + 1);
                j = r.Next(1, size + 1);
                if (perc.isOpen(i, j))
                    continue;

                perc.open(i, j);
                _bw.ReportProgress(1, perc);
                Thread.Sleep(delay);
            }
        }

        #region UI Handlers

        private void btnStart_Click(object sender, EventArgs e)
        {
            int size = 0;
            int delay = 0;
            if (!int.TryParse(edtSize.Text, out size))
            {
                MessageBox.Show(Resources.INVALID_SIZE_VALUE);
                return;
            }
            if (!int.TryParse(edtDelay.Text, out delay))
            {
                MessageBox.Show(Resources.INVALID_DELAY_VALUE);
                return;
            }
            _bw.RunWorkerAsync(new[]{size, delay});

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            this._bw.CancelAsync();
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            int[] args = (int[])e.Argument;
            int size = args[0];
            int delay = args[1];
            runPercolation(size, delay);
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            draw((Percolation)e.UserState);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        public void Dispose()
        {
            Dispose(true);
            _g.Dispose();
            _brush.Dispose();
            _bw.Dispose();
            _bw = null;
            _g = null;
            _brush = null;
        }

        private void PercolationVisualizerForm_SizeChanged(object sender, EventArgs e)
        {
            _g = pnlCanvas.CreateGraphics();
        }

    }
}
