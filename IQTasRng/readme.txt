This directory contains a command line tool to calculate and output the 10 best puzzle sets for a given selection of waves. It was mainly created to find the highest scoring puzzle sets for my TAS (https://www.youtube.com/watch?v=X_7VUEGHTk4) given a range of possible RNG seeds.

To use
---------

As this was written for a one-shot TAS (that I ended up doing twice), everything is hardcoded into the app. To change what it generates:

Go to the Main function in Program.cs
change the startPoint and endPoint variables to the desired RNG range
Change all four GetPuzzleSetPoints calls to represent the stage waves desired (they're currently set to the Final stage, so 7x9 puzzles with 1 puzzle per wave)

Then compile and run. Depending on defines, it'll either output the best 10 seeds that either generate the most capturable cubes, the most points (as determined by the CalcPuzzlePoints function) or require the fewest turns as defined by their TRNs (the number that appears on the bottom of the move count while you're playing)