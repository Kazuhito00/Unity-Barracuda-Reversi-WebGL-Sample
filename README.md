# Unity-Barracuda-Reversi-WebGL-Sample
Unity Barracudaを用いてリバーシAI(簡易なMLP)をWebGL上で推論するサンプルです。<br>
リバーシ用のモデルは後述するノートブックにて教師有り学習で作成したものを使用しています。<br>
現時点(2021/03/07)でUnityのWebGLはCPU推論のみのサポートです。<br>
<img src="https://user-images.githubusercontent.com/37477845/110242874-5654f980-7f9b-11eb-911a-899375c100e9.gif" width="80%">

# Demo
動作確認用ページは以下。<br>
[https://kazuhito00.github.io/Unity-Barracuda-Reversi-WebGL-Sample/WebGL-Build](https://kazuhito00.github.io/Unity-Barracuda-Reversi-WebGL-Sample/WebGL-Build/)

# Requirement (Unity)
* Unity 2021.1.0b6 or later
* Barracuda 1.3.0 or later

# Requrement (Python) ※ONNX変換をする場合のみ
* Tensorflow 2.4.0 or later
* tf2onnx 1.8.2 or later

# ONNX Model
「03_ReversiAI_TrainModel.ipynb」にて、Tensorflowによるモデル学習とONNX変換を実施しています。<br>
今回作成したモデルの構造は以下です。
<img src="https://user-images.githubusercontent.com/37477845/110244764-b059bd00-7fa3-11eb-9edf-75ab8f2c9983.png" width="100%">

# Model Training
ONNXモデルを学習させたい場合は以下3つのノートブックをGoogle Colaboratory上で実行してください。
### 01_ReversiAI_CreateTrainData.ipynb
学習用データを作成するノートブックです。<br>
フランスのオセロ連盟が公開している棋譜データベース[WTHOR](https://www.ffothello.org/informatique/la-base-wthor/)が公開している棋譜データを利用しています。<br>
以下の処理を含みます。
* WTHORから棋譜データをダウンロード<br>本リポジトリでは2006～2020年の15年分のデータを使用
* 棋譜データを読み込む
* 棋譜データを学習データとテストデータの形式に成形
* 学習データとテストデータをnpyの形式で保存
* npyファイルをGoogleドライブに保存

### 02_ReversiAI_DataAugmentation.ipynb
「01_ReversiAI_CreateTrainData.ipynb」で作成したデータに対し、データ拡張を行うノートブックです。<br>
以下の処理を含みます。
* 盤面データを180度回転したデータを作成し追加

### 03_ReversiAI_TrainModel.ipynb
モデル構築・学習・ONNX変換を行うノートブックです。

# Reference
* [Barracuda 1.3.0 preview](https://docs.unity3d.com/Packages/com.unity.barracuda@1.3/manual/index.html)

# Author
高橋かずひと(https://twitter.com/KzhtTkhs)

# License 
Unity-Barracuda-Reversi-WebGL-Sample is under [MIT License](LICENSE).
