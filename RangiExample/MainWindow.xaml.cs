using AutoMapper;
using CommonLibrary.Managers;
using WpfExample.Model.Entity;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfExample.WpfClient.DialogForm;
using WpfExample.Model.Dto;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace WpfExample.WpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly IMapper _mapper;
        readonly CommonEntityManager<Order> _orderManager;
        readonly CommonEntityManager<Personal> _personalManager;
        readonly CommonEntityManager<OrderDetail> _orderDetailManager;
        readonly WindowsNavigatore _windowsNavigatore;
        IList<OrderDto> _orderDtoList = new List<OrderDto>();


        public MainWindow(CommonEntityManager<Order> orderManager,
            CommonEntityManager<Personal> personalManager,
            CommonEntityManager<OrderDetail> orderDetailManager,
            IMapper mapper,
            WindowsNavigatore windowsNavigatore)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("fa-IR");
            InitializeComponent();

            _mapper = mapper;
            _orderManager = orderManager;
            _personalManager = personalManager;
            _orderDetailManager = orderDetailManager;
            _windowsNavigatore = windowsNavigatore;
        }

        #region Event

        #region Load
        private void Order_DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            Order_DataGridLoading();
            Order_DataGrid.ItemsSource = _orderDtoList.ToList();
        }

        private void Name_ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            IList<string> pepeol = _personalManager.Entity.Select(x => x.Name).ToList();
            if (pepeol.Count() == 0)
            {
                pepeol.Add("موردی سافت نشد");
                PersonalName_ComboBox.ItemsSource = pepeol;
                PersonalName_ComboBox.SelectedIndex = 0;
                Serch_GroupBox.IsEnabled = false;
            }
            else
            {
                PersonalName_ComboBox.IsEnabled = true;
                pepeol.Insert(0, "همه");
                PersonalName_ComboBox.ItemsSource = pepeol;
                PersonalName_ComboBox.SelectedIndex = 0;
                Serch_GroupBox.IsEnabled = true;
            }
        }
        #endregion

        #region StatusBarItem

        private void CreateStatusBarItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var orderCreateWindow = _windowsNavigatore.Create_OrderCreate_DialogForm(this);
            orderCreateWindow.ShowDialog();
            var orderDto = (orderCreateWindow as OrderCreate_DialogForm).OrderDto;
            if (orderDto == null) return;
            _orderDtoList.Add(orderDto);
            Order_DataGrid.ItemsSource = _orderDtoList.ToList();
        }

        private void UpdateStatusBarItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Order_DataGrid.SelectedIndex > -1)
            {
                var orderCreateWindow = _windowsNavigatore.Create_OrderCreate_DialogForm(this);
                (orderCreateWindow as OrderCreate_DialogForm).OrderDto = Order_DataGrid.SelectedItem as OrderDto;
                orderCreateWindow.ShowDialog();
                var orderDto = (orderCreateWindow as OrderCreate_DialogForm).OrderDto;
                if (orderDto == null) return;

                var elementIndex = _orderDtoList.ToList().FindIndex(x => x.OrderID.Equals(orderDto.OrderID));
                _orderDtoList.RemoveAt(elementIndex);
                _orderDtoList.Insert(elementIndex, orderDto);
                Order_DataGrid.ItemsSource = _orderDtoList.ToList();
            }
            else MessageBox.Show("موردی انتخاب نشده است");
        }

        private async void DeleteStatusBarItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Order_DataGrid.SelectedIndex > -1)
            {
                var result = MessageBox.Show(this, "مطمئنی؟", "آیا اطمینان دارید", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.OK)
                {
                    var orderDto = Order_DataGrid.SelectedItem as OrderDto;
                    await DeleteOrder(orderDto);
                    Order_DataGrid.ItemsSource = _orderDtoList.ToList();
                }
            }
            else MessageBox.Show("موردی انتخاب نشده است");
        }

        private void SerchStatusBarItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (StartDate_DatePicker.SelectedDate.Value <= EndDate_DatePicker.SelectedDate.Value)
            {
                var selectedComboItem = (PersonalName_ComboBox.SelectedItem as string);
                var selectedName = selectedComboItem.Equals("همه") ? null : selectedComboItem;

                var filterList = _orderDtoList.AsQueryable();
                if (selectedName != null)
                    filterList = filterList.Where(x => x.PersonalName.Equals(selectedName));

                if (StartDate_DatePicker.SelectedDate != null)
                    filterList = filterList.Where(x => DateTimeOffset.Parse(x.OrderDate) >= StartDate_DatePicker.SelectedDate.Value);


                if (EndDate_DatePicker.SelectedDate != null)
                    filterList = filterList.Where(x => DateTimeOffset.Parse(x.OrderDate) <= EndDate_DatePicker.SelectedDate.Value);

                Order_DataGrid.ItemsSource = filterList.ToList();
            }
            else MessageBox.Show("تاریخ انتخابی اشتباه است");
        }

        private void ExiteStatusBarItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
        #endregion
        #endregion



        #region Process
        private void Order_DataGridLoading()
        {
            _orderDtoList = _personalManager.Entity
                .Join(_orderManager.Entity,
                personal => personal.ID,
                order => order.PersonalID,
                (personal, order) => new { personal, order }
                )
                .Join(_orderDetailManager.Entity,
                personalOrder => personalOrder.order.ID,
                detail => detail.OrderID,
                (personalOrder, detail) => new { personalOrder, detail }
                ).ToList()
                .GroupBy(x => x.personalOrder.order,
                y => y.detail,
                (order, orderDetail) => new { order, orderDetail }).ToList()
                .Select(x => _mapper.Map(x.order, new OrderDto() { SumPrice = x.orderDetail.Sum(x => Convert.ToInt64(x.SumPrice)).ToString() })).ToList();
        }

        private async Task DeleteOrder(OrderDto orderDto)
        {
            var order = await _orderManager.FindByIdAsync(orderDto.OrderID).ConfigureAwait(false);
            await _orderManager.DeleteAsync(order).ConfigureAwait(false);
            _orderDtoList.Remove(orderDto);
        }



        #endregion


    }
}
