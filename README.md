Percolation (C#)
==================
##Introduction

 We model a percolation system using an N-by-N grid of sites. Each site is either open or blocked. A full site is an open site that can be connected to an open site in the top row via a chain of neighboring (left, right, up, down) open sites. We say the system percolates if there is a full site in the bottom row. In other words, a system percolates if we fill all open sites connected to the top row and that process fills some open site on the bottom row.

##Usage

* Start - Starts randomally opening sites in entered size grid and redraws it
* Stop - Stops the running percolation 
* Exit - Closes application
* Size - Size of the grid (NXN)
* Delay - Wait time between opening new site and redrawing the grid 

##Structure

* WeightedQuickUnionUF.cs - Data structure for storing relations between sites in union 
* Percolation.cs - Manages opening/closing sites and status of percolation
* PercolationVisualizerForm.cs - Visualizes percolation in Windows Form

