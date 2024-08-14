# SpellSeeker

## スクリプト
### __デッキシャッフル__

>#### デッキのシャッフル

[case"DeckShuffle"](https://github.com/KuroYuki009/SpellSeeker/blob/develop/Assets/SpellsCard/HandCardManager.cs#L183-L185)に移行した時に[メゾット"DeckShuffle"](https://github.com/KuroYuki009/SpellSeeker/blob/develop/Assets/SpellsCard/HandCardManager.cs#L271-L286)で処理を行う。
デッキから無作為に選ばれたカード情報をデポジットListに追加していき、デッキ内のカード枚数がゼロ枚になったら [case"MixingDeckCharge"](https://github.com/KuroYuki009/SpellSeeker/blob/develop/Assets/SpellsCard/HandCardManager.cs#L187-L191)に移行させ、デポジットListのカード情報をゲーム内で使用するデッキに入れます。

<rb>

### __手札の表示情報__
  
>#### UIに表示される手札とそれらに関連する情報

UIに表示されている情報を [メゾット"HundWindowRefresh"](https://github.com/KuroYuki009/SpellSeeker/blob/develop/Assets/SpellsCard/HandCardManager.cs#L289-L326)で更新します。


### __ロックオンシステム__

>#### ロックオン開始

プレイヤーの入力で[メゾット"LockOnSearch"](https://github.com/KuroYuki009/SpellSeeker/blob/develop/Assets/SpellsCard/HandCardManager.cs#L577-L622)で処理を実行します。前方にBoxcastを飛ばし、当たった標的のオブジェクト情報を格納する。<br>
[移動・視点操作のスクリプト側の処理](https://github.com/KuroYuki009/SpellSeeker/blob/develop/Assets/Entitys/Player/Script/PlayerMoving.cs#L140-L144)でプレイヤーを格納したオブジェクトの座標方向へ回転させ続けます。

>#### ロックオンの維持

ロックオンの維持には[メゾット"LockOnProcessing"](https://github.com/KuroYuki009/SpellSeeker/blob/develop/Assets/SpellsCard/HandCardManager.cs#L626-L680)で、対象となるオブジェクトとの間にrayを飛ばし、間に障害物がないか確認させている。<rb>
もし、間に何かが遮っている状況が一定時間立つとロックオンは解除されるようになっています。

>#### ロックオンの解除

ロックオンを解除する時には[メゾット"LockOnOut"](https://github.com/KuroYuki009/SpellSeeker/blob/develop/Assets/SpellsCard/HandCardManager.cs#L682-L698)で処理を実行します。ロックオンの維持に使用していた数値等の初期化を行い、再ロックオンに必要なクールタイムを設けます。


### __カードの選択__

>#### カード選択の申請

[PlayerAdopt_Card.cs](Assets/Entitys/Player/Script/PlayerAdopt_Card.cs)では汎用選択ウィンドウ側と処理を返す事で、指定した回数分のカードを選択できるように出来ます。<rb>
シールドデッキ戦に使われる申請用の[メゾット"AddCard_Conflict_Shield"](https://github.com/KuroYuki009/SpellSeeker/blob/develop/Assets/Entitys/Player/Script/PlayerAdopt_Card.cs#L56-L69)
には引数を二つ指定する必要があり、それぞれ選択する事の出来る要素数と選択できる抽選回数を設定できます。

>#### 要素の選択

ゲーム内で要素を選択する際に使用される汎用選択ウィンドウでは表示された要素を左スティック等の入力で選択、決定ボタンを押す事で確定させることが出来ます。
また選択時、左スティック等の入力に合わせてカーソルを動かして要素に近づけると自動的にスナップされ、要素のハイライトと共に上部へ要素情報が表示されます。(参照：[選択操作の処理部分](https://github.com/KuroYuki009/SpellSeeker/blob/develop/Assets/Entitys/Player/Script/PlayerGSWInput.cs#L262-L358))

>#### 選択している情報の表示更新

汎用選択ウィンドウにて各要素を選択した際に[メゾット"PickInfoDateRefresh"](https://github.com/KuroYuki009/SpellSeeker/blob/develop/Assets/Entitys/Player/Script/PlayerGSWInput.cs#L360-L381)で表示されている情報の更新処理を行います。
