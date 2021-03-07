# Unity-Barracuda-Reversi-WebGL-Sample
Unity Barracudaを用いてリバーシAI(簡易なMLP)をWebGL上で推論するサンプルです。<br>
現時点(2021/03/07)でUnityのWebGLはCPU推論のみのサポートです。<br>
<img src="https://user-images.githubusercontent.com/37477845/110242874-5654f980-7f9b-11eb-911a-899375c100e9.gif" width="80%">

# Demo
動作確認用ページは以下。<br>
[https://kazuhito00.github.io/Unity-Barracuda-Reversi-WebGL-Sample/WebGL-Build](https://kazuhito00.github.io/Unity-Barracuda-Reversi-WebGL-Sample/WebGL-Build/index.html)

# Requirement (Unity)
* Unity 2021.1.0b6 or later
* Barracuda 1.3.0 or later

# Requrement (Python) ※モデル学習、ONNX変換をする場合のみ
* Tensorflow 2.4.0 or later
* tf2onnx 1.8.2 or later

# ONNXモデル
「03_ReversiAI_TrainModel.ipynb」にて、Tensorflowによるモデル学習とONNX変換を実施しています。
今回作成したモデルの構造は以下です。

# Reference
* [Barracuda 1.3.0 preview](https://docs.unity3d.com/Packages/com.unity.barracuda@1.3/manual/index.html)

# Author
高橋かずひと(https://twitter.com/KzhtTkhs)

# License 
Unity-Barracuda-Reversi-WebGL-Sample is under [MIT License](LICENSE).
