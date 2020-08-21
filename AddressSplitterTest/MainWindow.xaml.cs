using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ganges.AddressSplitterTestDrive
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            SourceTextBox.Text = testData;
            Source2TextBox.Text = testData2;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {

            //■1行毎に分割を実行
            var cls = new Ganges.AddressSplitter.Address();

            var isKatakana = (bool)isKatakanaCheckBox.IsChecked;
            var isOmitGun = (bool)isOmitGunCheckBox.IsChecked;
            bool isAvailableAddress2 = (bool)isAvailableAddress2CheckBox.IsChecked;

            var splittedList = new List<SplittedItem>();

            using (var sr = new StringReader(SourceTextBox.Text))
            {
                string s = sr.ReadLine();
                while (s != null)
                {
                    var address = cls.SplitAddress(s, isAvailableAddress2, isOmitGun, isKatakana);
                    var item = new SplittedItem()
                    {
                        都道府県 = address[0],
                        政令市_郡_市区町村 = address[1],
                        住所1 = address[2],
                        住所2 = address[3],
                        都道府県ふりがな = address[4],
                        政令市_郡_市区町村ふりがな = address[5]
                    };
                    splittedList.Add(item);
                    s = sr.ReadLine();
                }
            }
            SplittedDataGrid.ItemsSource = splittedList;


            //■改行を含めた住所を分割
            var address2 = cls.SplitAddress(Source2TextBox.Text, isAvailableAddress2, isOmitGun, isKatakana);
            Splitted2TextBox.Text = string.Join(",", address2);

        }


        private readonly string testData = @"北海道札幌市中央区北1条西2丁目
函館市東雲町4-13
北海道北見市大通西2丁目1番地 まちきた大通ビル
夕張郡栗山町松風3-252
東京都新宿区西新宿2-8-1
京都府京都市上京区下立売通新町西入薮ノ内町
京都市中京区寺町通御池上る上本能寺前町488番地
石川県金沢市広坂1-2-1
福井県あわら市宮谷57-2-19
東京都東村山市本町1丁目2番地3
沖縄県国頭郡東村字平良804番地";

        private readonly string testData2 = @"福井県勝山市村岡町寺尾51-11
かつやま恐竜の森内";

    }

    /// <summary>
    /// DataGridでの表示に利用
    /// </summary>
    class SplittedItem
    {
        public string 都道府県 { get; set; }
        public string 政令市_郡_市区町村 { get; set; }
        public string 住所1 { get; set; }
        public string 住所2 { get; set; }
        public string 都道府県ふりがな { get; set; }
        public string 政令市_郡_市区町村ふりがな { get; set; }
    }

}
