# TrainCrewMotorSound

TrainCrewMotorSoundは、溝月レイル/Acty様製作の列車運転ゲーム「TRAIN CREW」で動作する、BVE Trainsim用モータ音連動ソフトです。  
#### ※車両データは「BVE5」専用となります。BVE4以前およびBVE6以降には対応していません。

![1](https://github.com/user-attachments/assets/57d7b18f-0b49-4ce4-b8a2-6ed587c1dca5)

# 使い方
1、TRAIN CREWの操作設定→外部デバイス入出力を「有効」に設定してください。

2、TRAIN CREWのサウンド設定→モータ音音量を「0」に設定してください。

3、Zipファイルを解凍後、TrainCrewMoniter.exeをダブルクリックして起動してください。ファイルの解凍先は自由です。

4、[ BVE車両ファイル読込 ] ボタンをクリックして、ダイアログからVehicleファイルを選択します。

5、[ 情報 ] 欄の"モータ音読込"項目が [ 完了 ] になっていれば成功です。

# 操作説明

☑最前面表示  
　ウィンドウを最前面にするかどうかを設定します。  
　チェックを入れると最前面になります  

☑ノッチ連動  
　ノッチ操作と連動して音声を再生します。  
　チェックを入れるとノッチ操作に連動し、外すと速度情報のみで再生処理を行います。 

 ☑非常ブレーキ時回生オフ  
　非常ブレーキ操作時に回生モータ音を再生するかどうかを設定します。  
　チェックを入れると非常ブレーキ操作時に回生モータ音をオフにします。 

 ○回生失効速度  
　減速時にモータ音を停止する速度を指定します。  
　0km/h～120km/hまで選択できます。  

○走行音選択  
　Runファイルが用意されている場合に再生したいRunファイルを指定できます。  

# 作成情報

* 作成者:Suine97

# License
"TrainCrewMotorSound" is under [MIT license](https://en.wikipedia.org/wiki/MIT_License).
