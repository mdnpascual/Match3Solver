# Match3Solver

![WindowScreenshot](https://i.imgur.com/97MEtvU.jpg)
![Board](https://i.imgur.com/CLusuxr.jpg)

Match 3 Solver for HuniePop2

Features:
- Automatically Injects to the game to be able to screenshot the board state
- Parses and Solve the board state.
- Draw Movements on top of the game
- Various sort modes for results

Usage:
1) Download the Zip
2) Right click the Zip >> Properties >> Tick Unblock and Click Apply
3) Extract the zip BUT DO NOT PUT IT ON THE SAME FOLDER WHERE HUNIEPOP2 is located
4) Run the Match3Solver.exe as admin
5) Run huniepop
6) Press Ctrl + Alt + I to Inject
7) Press Ctrl + Alt + C to capture board state and solve.

Video Demo on how to Use and Read the results:

[<img src="https://j.gifs.com/lxzokr.gif">](https://youtu.be/nof1xo_q4ws)

Known Bugs:
- Random Game crash when alt-tabing

Unknown Bugs:
- Only Tested on monitors with 16:9 Aspect Ratio (3840x2160, 2560x1440, 1920x1080). Don't know how the gmae behaves on widescreen monitors

Notes when Building from Source:
Include sharpdx_direct3d11_1_effects_x64.dll in Debug or Release folder. File can be found under Match3Solver\packages\SharpDX.Direct3D11.Effects.4.2.0\runtimes\win7-x64\native
Then add a suffix of "_x64" at the end