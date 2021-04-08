# Match3Solver

![WindowScreenshot](https://i.imgur.com/y2bp54S.jpg)
![Board](https://i.imgur.com/XKdTcJn.jpg)

Match 3 Solver for HuniePop2

Features:
- Automatically Injects to the game to be able to screenshot the board state
- Parses and Solve the board state.
- Draw Movements on top of the game
- Various sort modes for results

Usage:
1) [Download the Zip](https://github.com/mdnpascual/Match3Solver/releases)
2) Right click the Zip >> Properties >> Tick Unblock and Click Apply
3) Extract the zip BUT DO NOT PUT IT ON THE SAME FOLDER WHERE HUNIEPOP2 is located
4) Run Match3Solver.exe. (It won't run if you Click no when asking for Admin Access)
5) Run huniepop
6) Press Ctrl + Alt + C to capture board state and solve.

Video Demo on How to Use:

[<img src="https://j.gifs.com/1W2gDG.gif">](https://youtu.be/xW7D1d0Jp6c)https://www.youtube.com/watch?v=xW7D1d0Jp6c

Known Bugs:
- Random Game crash when alt-tabing

Unknown Bugs:
- Only Tested on monitors with 16:9 Aspect Ratio (3840x2160, 2560x1440, 1920x1080). Don't know how the gmae behaves on widescreen monitors

Notes when Building from Source:
Include sharpdx_direct3d11_1_effects_x64.dll in Debug or Release folder. File can be found under Match3Solver\packages\SharpDX.Direct3D11.Effects.4.2.0\runtimes\win7-x64\native
Then add a suffix of "_x64" at the end
