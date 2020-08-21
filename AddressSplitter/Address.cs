using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Ganges.AddressSplitter
{
    public class Address
    {
        #region 分割された住所とふりがな

        public string 都道府県 { get; set; }

        /// <summary>
        /// 郡を含めた、政令市・郡 + 市区町村
        /// </summary>
        public string 政令市郡_市区町村 { get; set; }

        /// <summary>
        /// 郡を除いた、政令市 ＋ 市区町村
        /// </summary>
        public string 政令市_市区町村 { get; set; }

        /// <summary>
        /// 市区町村以降の住所
        /// </summary>
        public string 住所1_2 { get; set; }

        /// <summary>
        /// 市区町村以降の住所で、最初の空白・改行で区切った、最初の部分
        /// </summary>
        public string 住所1 { get; set; }

        /// <summary>
        /// 市区町村以降の住所で、最初の空白・改行で区切った、2番目以降の部分
        /// 文字列の中の改行は維持されます。
        /// </summary>
        public string 住所2 { get; set; }
        public string 都道府県ふりがな { get; set; }
        public string 政令市郡_市区町村ふりがな { get; set; }
        public string 政令市_市区町村ふりがな { get; set; }
        public string 都道府県フリガナ { get; set; }
        public string 政令市郡_市区町村フリガナ { get; set; }
        public string 政令市_市区町村フリガナ { get; set; }

        #endregion


        /// <summary>
        /// 住所を分割します。
        /// 分割例: 福井県あわら市○○1-1-1 ××× → 福井県 , あわら市 , ○○1-1-1 ××× , , ふくいけん , あわらし
        /// </summary>
        /// <param name="fullAddress">分割をする住所の文字列</param>
        /// <param name="isAvailableAddress2">true : 住所1の部分から、空白・改行を先頭から検索し、そこで分割して、以降を住所2として返す
        /// 例: 住所1が「○○1-1-1」、住所2が「×××」になる。</param>
        /// <param name="isOmitGun">true : fullAddressに、郡がある場合でも、省略する</param>
        /// <param name="isKatakana">true : ふりがなを、カタカナで返す</param>
        /// <returns>
        /// 配列で返す。
        /// 0 都道府県
        /// 1 政令市・郡 + 市区町村
        /// 2 住所1　改行は維持されます。
        /// 3 住所2　改行は維持されます。
        /// 4 都道府県ふりがな
        /// 5 政令市・郡 + 市区町村 のふりがな
        /// </returns>
        /// 
        public string[] SplitAddress(string fullAddress, bool isAvailableAddress2 = false, bool isOmitGun = false, bool isKatakana = false)
        {
            ClearProperty();
            ReadAddressSource();
            ExecuteSplitAddress(fullAddress);

            string[] address = new string[6]; //0～5の配列
            address[0] = 都道府県;
            address[1] = isOmitGun ? 政令市_市区町村 : 政令市郡_市区町村;
            if (isAvailableAddress2)
            {
                address[2] = 住所1;
                address[3] = 住所2;
            }
            else
            {
                address[2] = 住所1_2;
                address[3] = "";
            }

            if (isKatakana)
            {
                address[4] = 都道府県フリガナ;
                address[5] = isOmitGun ? 政令市_市区町村フリガナ : 政令市郡_市区町村フリガナ;
            }
            else
            {
                address[4] = 都道府県ふりがな;
                address[5] = isOmitGun ? 政令市_市区町村ふりがな : 政令市郡_市区町村ふりがな;
            }

            return address;

        }

        private void ClearProperty()
        {
            都道府県 = "";
            政令市郡_市区町村 = "";
            政令市_市区町村 = "";
            住所1_2 = "";
            住所1 = "";
            住所2 = "";
            都道府県ふりがな = "";
            政令市郡_市区町村ふりがな = "";
            政令市_市区町村ふりがな = "";
            都道府県フリガナ = "";
            政令市郡_市区町村フリガナ = "";
            政令市_市区町村フリガナ = "";
        }

        private void ExecuteSplitAddress(string fullAddress)
        {
            var addressTrim = fullAddress.Trim();
            if (string.IsNullOrEmpty(addressTrim)) return;

            var left4 = addressTrim.Length < 4 ? "" : addressTrim.Substring(0, 4);
            var left3 = addressTrim.Length < 3 ? "" : addressTrim.Substring(0, 3);
            string addressWithoutPrefecture = ""; //都道府県を除いた住所

            switch (left4)
            {
                case "神奈川県":
                    都道府県 = left4;
                    都道府県ふりがな = "かながわけん";
                    addressWithoutPrefecture = addressTrim.Substring(4).Trim();
                    break;

                case "和歌山県":
                    都道府県 = left4;
                    都道府県ふりがな = "わかやまけん";
                    addressWithoutPrefecture = addressTrim.Substring(4).Trim();
                    break;

                case "鹿児島県":
                    都道府県 = left4;
                    都道府県ふりがな = "かごしまけん";
                    addressWithoutPrefecture = addressTrim.Substring(4).Trim();
                    break;

                default:
                    break;
            }

            if (string.IsNullOrEmpty(都道府県))
            {

                switch (left3)
                {
                    case "北海道":
                        都道府県ふりがな = "ほっかいどう";
                        break;
                    case "青森県":
                        都道府県ふりがな = "あおもりけん";
                        break;
                    case "岩手県":
                        都道府県ふりがな = "いわてけん";
                        break;
                    case "宮城県":
                        都道府県ふりがな = "みやぎけん";
                        break;
                    case "秋田県":
                        都道府県ふりがな = "あきたけん";
                        break;
                    case "山形県":
                        都道府県ふりがな = "やまがたけん";
                        break;
                    case "福島県":
                        都道府県ふりがな = "ふくしまけん";
                        break;
                    case "茨城県":
                        都道府県ふりがな = "いばらきけん";
                        break;
                    case "栃木県":
                        都道府県ふりがな = "とちぎけん";
                        break;
                    case "群馬県":
                        都道府県ふりがな = "ぐんまけん";
                        break;
                    case "埼玉県":
                        都道府県ふりがな = "さいたまけん";
                        break;
                    case "千葉県":
                        都道府県ふりがな = "ちばけん";
                        break;
                    case "東京都":
                        都道府県ふりがな = "とうきょうと";
                        break;
                    case "新潟県":
                        都道府県ふりがな = "にいがたけん";
                        break;
                    case "富山県":
                        都道府県ふりがな = "とやまけん";
                        break;
                    case "石川県":
                        都道府県ふりがな = "いしかわけん";
                        break;
                    case "福井県":
                        都道府県ふりがな = "ふくいけん";
                        break;
                    case "山梨県":
                        都道府県ふりがな = "やまなしけん";
                        break;
                    case "長野県":
                        都道府県ふりがな = "ながのけん";
                        break;
                    case "岐阜県":
                        都道府県ふりがな = "ぎふけん";
                        break;
                    case "静岡県":
                        都道府県ふりがな = "しずおかけん";
                        break;
                    case "愛知県":
                        都道府県ふりがな = "あいちけん";
                        break;
                    case "三重県":
                        都道府県ふりがな = "みえけん";
                        break;
                    case "滋賀県":
                        都道府県ふりがな = "しがけん";
                        break;
                    case "京都府":
                        都道府県ふりがな = "きょうとふ";
                        break;
                    case "大阪府":
                        都道府県ふりがな = "おおさかふ";
                        break;
                    case "兵庫県":
                        都道府県ふりがな = "ひょうごけん";
                        break;
                    case "奈良県":
                        都道府県ふりがな = "ならけん";
                        break;
                    case "鳥取県":
                        都道府県ふりがな = "とっとりけん";
                        break;
                    case "島根県":
                        都道府県ふりがな = "しまねけん";
                        break;
                    case "岡山県":
                        都道府県ふりがな = "おかやまけん";
                        break;
                    case "広島県":
                        都道府県ふりがな = "ひろしまけん";
                        break;
                    case "山口県":
                        都道府県ふりがな = "やまぐちけん";
                        break;
                    case "徳島県":
                        都道府県ふりがな = "とくしまけん";
                        break;
                    case "香川県":
                        都道府県ふりがな = "かがわけん";
                        break;
                    case "愛媛県":
                        都道府県ふりがな = "えひめけん";
                        break;
                    case "高知県":
                        都道府県ふりがな = "こうちけん";
                        break;
                    case "福岡県":
                        都道府県ふりがな = "ふくおかけん";
                        break;
                    case "佐賀県":
                        都道府県ふりがな = "さがけん";
                        break;
                    case "長崎県":
                        都道府県ふりがな = "ながさきけん";
                        break;
                    case "熊本県":
                        都道府県ふりがな = "くまもとけん";
                        break;
                    case "大分県":
                        都道府県ふりがな = "おおいたけん";
                        break;
                    case "宮崎県":
                        都道府県ふりがな = "みやざきけん";
                        break;
                    case "沖縄県":
                        都道府県ふりがな = "おきなわけん";
                        break;

                    default:
                        break;
                }


                if (!string.IsNullOrEmpty(都道府県ふりがな))
                {
                    都道府県 = left3;
                    addressWithoutPrefecture = addressTrim.Substring(3).Trim(); // "北海道　札幌市××のように、都道府県の後に、空白がある場合、Trimする
                }
                else
                {
                    addressWithoutPrefecture = addressTrim;
                }

            }
            var lastAddress = addressWithoutPrefecture;　//この時点で、addressWithoutPrefectureは、Trimされている。


            bool isFinded = false;


            //政令市・郡 + 市区町村 が、都道府県を除いた住所の先頭にあるかをチェック
            foreach (var item in addressSourceList)
            {
                if (!string.IsNullOrEmpty(item.City) && addressWithoutPrefecture.StartsWith(item.SeireisiGunCity))
                {
                    政令市郡_市区町村 = item.SeireisiGunCity;
                    政令市_市区町村 = item.SeireisiGun.EndsWith("郡") ? item.City : 政令市郡_市区町村;
                    政令市郡_市区町村ふりがな = item.SeireisiGunKana + item.CityKana;
                    政令市_市区町村ふりがな = item.SeireisiGun.EndsWith("郡") ? item.CityKana : 政令市郡_市区町村ふりがな;

                    lastAddress = addressWithoutPrefecture.Substring(item.SeireisiGunCity.Length).Trim();
                    isFinded = true;
                    break;
                }
            }

            if (!isFinded)
            {
                //政令市・郡を含めない市区町村 が、都道府県を除いた住所の先頭にあるかをチェック

                //本来は、「東村」と「東村山市」があるので、item.Cityを文字数の多い順にソートしてから調べないといけない。
                //ただ、東村より、東村山市が前の行にあるので、以下の処理で不都合はない。
                //これは、東村以外にはないので、この処理で不都合は現時点ではない。
                foreach (var item in addressSourceList)
                {
                    if (!string.IsNullOrEmpty(item.City) && addressWithoutPrefecture.StartsWith(item.City))
                    {
                        政令市郡_市区町村 = item.City;
                        政令市_市区町村 = 政令市郡_市区町村;
                        政令市郡_市区町村ふりがな = item.CityKana;
                        政令市_市区町村ふりがな = 政令市郡_市区町村ふりがな;

                        lastAddress = addressWithoutPrefecture.Substring(item.City.Length).Trim();
                        isFinded = true;
                        break;
                    }
                }
            }


            住所1_2 = lastAddress;


            //残りの住所を分割する。
            char[] separator = new char[] { ' ', '　', '\r', '\n' }; //空白と改行で分割
            string[] splitted = lastAddress.Split(separator);
            住所1 = splitted[0]; //住所1
            住所2 = lastAddress.Substring(splitted[0].Length).Trim(); //住所2


            //カタカナに変更
            都道府県フリガナ = Microsoft.VisualBasic.Strings.StrConv(都道府県ふりがな, Microsoft.VisualBasic.VbStrConv.Katakana, 0x411);
            政令市郡_市区町村フリガナ = Microsoft.VisualBasic.Strings.StrConv(政令市郡_市区町村ふりがな, Microsoft.VisualBasic.VbStrConv.Katakana, 0x411);
            政令市_市区町村フリガナ = Microsoft.VisualBasic.Strings.StrConv(政令市_市区町村ふりがな, Microsoft.VisualBasic.VbStrConv.Katakana, 0x411);


            return;

        }
    
        /// <summary>
        /// AddressSourceItemのリスト
        /// </summary>
        private static List<AddressSourceItem> addressSourceList = new List<AddressSourceItem>();

        private void ReadAddressSource()
        {
            if (addressSourceList.Count > 0) return;

            string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string dir = System.IO.Path.GetDirectoryName(appPath);
            var path = Path.Combine(dir, "AddressSource.csv");

            if (!File.Exists(path)) path = Path.Combine(dir, "AddressSplitter", "AddressSource.csv"); //同じフォルダに無い場合は、AddressSplitterフォルダの中にあると想定
            if (!File.Exists(path)) return;

            using (var sr = new StreamReader(path))
            {
                string line1 = sr.ReadLine(); //1行目をスキップ

                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] values = line.Split(',');
                    addressSourceList.Add(new AddressSourceItem() { Prefecture = values[0], SeireisiGun = values[1], City = values[3], SeireisiGunCity = values[1] + values[3], SeireisiGunKana = values[2], CityKana = values[4] });
                }

            }

            //foreach (var item in addressSourceList)
            //{
            //    Debug.WriteLine("{0} {1} {2} {3} {4} {5}", item.Prefecture, item.SeireisiGun, item.City, item.SeireisiGunCity, item.SeireisiGunKana, item.CityKana);
            //}

        }

    }

    /// <summary>
    /// AddressSourceの1行分
    /// </summary>
    class AddressSourceItem
    {
        /// <summary>
        /// 都道府県。1列目
        /// </summary>
        public string Prefecture { get; set; }

        /// <summary>
        /// 政令市・郡。2列目
        /// </summary>
        public string SeireisiGun { get; set; }

        /// <summary>
        /// 市区町村。4列目
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 検索用に、政令市・郡と、市区町村との文字列を結合しておく。
        /// </summary>
        public string SeireisiGunCity { get; set; }

        /// <summary>
        /// 政令市・郡のふりがな。3列目
        /// </summary>
        public string SeireisiGunKana { get; set; }

        /// <summary>
        /// 市区町村のふりがな。5列目
        /// </summary>
        public string CityKana { get; set; }

    }

}
