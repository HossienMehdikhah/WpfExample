using CommonLibrary.Managers;
using WpfExample.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.Globalization;

namespace WpfExample.WpfClient.DialogForm
{
    /// <summary>
    /// Interaction logic for OrderDetailCreate_DialogForm.xaml
    /// </summary>
    public partial class OrderDetailCreate_DialogForm : Window
    {
        readonly CommonEntityManager<Produce> _produceManager;
        readonly IEnumerable<Produce> _produces;
        readonly Regex _regex = new Regex("[^0-9]+");
        OrderDetail _orderDetail;       
        bool _isUpdate;

        public OrderDetailCreate_DialogForm(CommonEntityManager<Produce> produceManager)
        {
            InitializeComponent();            

            _produceManager = produceManager;
            _produces = _produceManager.Entity;
        }

        public OrderDetail OrderDetail
        {
            get
            {
                return _orderDetail;
            }
            set
            {
                _isUpdate = true;
                _orderDetail = value;                
                Price_TextBox.Text = value.Produce.Price.ToString("c0");
                Count_TextBox.Text = value.Count.ToString("f0");
                SumPrice_TextBox.Text = value.SumPrice.ToString("c0");
            }
        }

        #region Event
        private void Produce_ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ICollection<string> producesComboList = _produces.Select(x => x.Name).ToList();
            if (producesComboList.Count() == 0)
            {
                Produce_ComboBox.IsEnabled = false;
                producesComboList.Add("موردی یافت نشد");
                Produce_ComboBox.ItemsSource = producesComboList;
                Produce_ComboBox.SelectedIndex = 0;
            }
            else
            {
                Produce_ComboBox.IsEnabled = true;
                Produce_ComboBox.ItemsSource = producesComboList;
                if (_isUpdate) Produce_ComboBox.SelectedItem = _orderDetail.Produce.Name;
            }
        }

        private void Produce_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var temp = (sender as ComboBox).SelectedItem;

            if (temp != null)
            {
                //Maybe User Modified Defualt Price. So I must Check It.
                if (!_isUpdate)
                {
                    Price_TextBox.Text = _produces.FirstOrDefault(x => x.Name.Equals(temp)).Price.ToString("f0");
                    if (!string.IsNullOrEmpty(Count_TextBox.Text))
                        SumPriceCalculate();
                }
                Price_TextBox.IsEnabled = true;
                Count_TextBox.IsEnabled = true;
            }
        }

        private void Price_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }

        private void Price_TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            var priceText = $"{ Price_TextBox.Text:f0}";
            if (!string.IsNullOrWhiteSpace(priceText.ToString()))
            {
                Price_TextBox.Text = priceText;
                Price_TextBox.CaretIndex = priceText.Length;
                if (!string.IsNullOrEmpty(Count_TextBox.Text))
                    SumPriceCalculate();
            }
            else SumPrice_TextBox.Text = "";
        }

        private void Count_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }

        private void Count_TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            var countText = $"{ Count_TextBox.Text:f0}";

            if (!string.IsNullOrWhiteSpace(countText))
            {
                Count_TextBox.Text = countText;
                if (!string.IsNullOrEmpty(Price_TextBox.Text))
                    SumPriceCalculate();
            }
            else SumPrice_TextBox.Text = "";
        }

        private void Create_DetailOrder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var validation = ValidationInput();

            if (!string.IsNullOrWhiteSpace(validation)) MessageBox.Show(this, validation, "", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                var produce = _produces.FirstOrDefault(x => x.Name.Equals(Produce_ComboBox.SelectedItem as string));
                if (!_isUpdate) _orderDetail = new OrderDetail();

                _orderDetail.Produce = produce;
                _orderDetail.ProduceID = produce.ID;
                _orderDetail.Price = ulong.Parse(Price_TextBox.Text, NumberStyles.Currency);
                _orderDetail.Count = uint.Parse(Count_TextBox.Text, NumberStyles.Currency);
                _orderDetail.SumPrice = ulong.Parse(SumPrice_TextBox.Text, NumberStyles.Currency);
                Close();
            }
        }

        private void CancelStatusBarItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        #endregion

        private string ValidationInput()
        {
            if (Produce_ComboBox.SelectedIndex < 0) return "محصول را انتخاب کنید";
            if (string.IsNullOrEmpty(Price_TextBox.Text)) return "قیمت را وارد کنید";
            if (string.IsNullOrEmpty(Count_TextBox.Text)) return "تعداد را وارد کنید";
            if (string.IsNullOrEmpty(SumPrice_TextBox.Text)) return "جمع کل را وارد کنید";
            return "";
        }

        private void SumPriceCalculate()
        {
            var price = ulong.Parse(Price_TextBox.Text, NumberStyles.Currency);
            var count = uint.Parse(Count_TextBox.Text, NumberStyles.Currency);
            var sumPrice = price * count;
            SumPrice_TextBox.Text = sumPrice.ToString("c0");
        }
    }
}
