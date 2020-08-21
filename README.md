# AddressSplitter（住所分割）
住所を分割します。
都道府県、政令市・郡、市区町村のふりがなを返します。

## 分割例
福井県あわら市○○1-1-1 ××× → 福井県 , あわら市 , ○○1-1-1 ××× , , ふくいけん , あわらし

分割されたデータを取得する方法は2つあります。

### Addressクラスのプロパティをgetします。
- **都道府県**
- **政令市郡_市区町村**
- **政令市_市区町村**
- **住所1_2**
- **住所1**
- **住所2**
- **都道府県ふりがな**
- **政令市郡_市区町村ふりがな**
- **政令市_市区町村ふりがな**
- **都道府県フリガナ**
- **政令市郡_市区町村フリガナ**
- **政令市_市区町村フリガナ**

### 配列で返します。
- 0 **都道府県**
- 1 **政令市・郡 + 市区町村**
- 2 **住所1**　改行は維持されます。
- 3 **住所2**　改行は維持されます。
- 4 **都道府県ふりがな**
- 5 **政令市・郡 + 市区町村 のふりがな**

## 利用データ
統計で見る日本＞市区町村名・コード＞市区町村を探す でダウンロードできるデータを利用します。

https://www.e-stat.go.jp/municipalities/cities/areacode

データは次の点のみ加工しています。

北海道の行政区分が振興局になっているものは、
- 政令市･郡
- 政令市･郡（ふりがな）
を追加しています。

## インストール
- Ganges.AddressSplitter.dll を配置したフォルダ
- あるいは、Ganges.AddressSplitter.dll を配置したフォルダの中に作成した、AddressSplitterフォルダ
に、AddressSource.csv を配置して下さい。

## 留意点
- 檮原町→略式表記「梼原町」では分割できない
- 七ヶ宿町など、「ヶ」が「ケ」では分割できない
- 支庁は住所表記とは無関係。例：三宅村は「東京都三宅村」が住所。「東京都三宅支庁三宅村」という表記は誤り。

## 必要環境
.NET Framework 4.7

## メンテナンス者
太子堂（ガンジス） https://ganges.pro/

## License
MIT