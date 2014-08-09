using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Percolation
{
    /// <summary>
    /// Class for calculating percolations in NxN grid
    /// </summary>
    public class Percolation
    {
        private WeightedQuickUnionUF _grid;
        private bool[] _states;
        private int _N;
        private int _top;
        private int _bottom;
        private Dictionary<Direction, Action<int, int, int>> _joinSiteDirections;
        
        private enum Direction
        {
            Left = 1,
            Right = 2,
            Down = 3,
            Up = 4
        }

        /// <summary>
        /// number of sites for N X N grid
        /// </summary>
        public int size { get { return _N; } }

        private int getInd(int i, int j)
        {
            if (isOutOfRange(i, j))
            {
                throw new IndexOutOfRangeException();
            }
            return (i - 1) * _N + (j - 1);
        }

        private bool isOutOfRange(int i, int j)
        {
            return i < 1 || j < 1 || i > _N || j > _N;
        }

        /// <summary>
        /// create N-by-N grid, with all sites blocked
        /// </summary>
        /// <param name="N">number of sites for N X N grid</param>
        public Percolation(int N) 
        {
            this._N = N;
            _grid = new WeightedQuickUnionUF(N * N + 2); // 2 extra sites for percolation testing
            _states = new bool[N * N];

            _top = _grid.count - 2; // top index
            _bottom = _grid.count - 1; // bottom index

            connectSide(0, N - 1, _top);
            connectSide(N * N - N, N * N - 1, _bottom);

            _joinSiteDirections = new Dictionary<Direction, Action<int, int, int>>{
                {Direction.Left,  joinLeft},
                {Direction.Right, joinRight},
                {Direction.Up,    joinUp},
                {Direction.Down,  joinDown}
            };
        }

        private void connectSide(int start, int end, int target)
        {
            for (int k = start; k <= end; k++)
            {
                _grid.union(target, k);
            }
        }

        private void joinSite(int idx, int i, int j){
            if (!isOutOfRange(i, j) && isOpen(i, j))
            {
                _grid.union(idx, getInd(i, j));
            }
        }

        private void joinLeft(int idx, int i, int j)
        {
            joinSite(idx, i, j - 1);
        }

        private void joinRight(int idx, int i, int j)
        {
            joinSite(idx, i, j + 1);
        }

        private void joinDown(int idx, int i, int j)
        {
            joinSite(idx, i - 1, j);
        }

        private void joinUp(int idx, int i, int j)
        {
            joinSite(idx, i + 1, j);
        }

        /// <summary>
        /// open site (row i, column j) if it is not already open
        /// </summary>
        /// <param name="i">row i</param>
        /// <param name="j">column j</param>
        public void open(int i, int j) 
        {
            if (isOpen(i, j))
                return;

            int idx = getInd(i, j);
            _states[idx] = true;

            foreach (var joinAction in _joinSiteDirections.Values)
                joinAction(idx, i, j);
        }

        /// <summary>
        /// is site (row i, column j) open?
        /// </summary>
        /// <param name="i">row index (starts from 1)</param>
        /// <param name="j">column index (starts from 1)</param>
        /// <returns>true if opened, otherwise false</returns>
        public bool isOpen(int i, int j) 
        {
            int ind = getInd(i, j);
            return _states[ind];
        }

        /// <summary>
        /// is site (row i, column j) full?
        /// </summary>
        /// <param name="i">row index (starts from 1)</param>
        /// <param name="j">column index (starts from 1)</param>
        /// <returns>true if opened, otherwise false</returns>
        public bool isFull(int i, int j) // 
        {
            return isOpen(i, j) && _grid.isConnected(getInd(i, j), _top);
        }

        /// <summary>
        /// does the system percolate?
        /// </summary>
        /// <returns>true if percolates otherwise false</returns>
        public bool doesPercolate()
        {
            return _grid.isConnected(_top, _bottom);
        }
    }

}
