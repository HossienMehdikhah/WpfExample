using AutoMapper;
using CommonLibrary.Managers;
using WpfExample.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfExample.Model.Dto;
using System.Threading.Tasks;
using System.Threading;

namespace WpfExample.WpfClient.DialogForm
{
    /// <summary>
    /// Interaction logic for OrderCreate_DialogForm.xaml
    /// </summary>
    public partial class OrderCreate_DialogForm : Window
    {
        readonly IMapper _mapper;
        readonly CommonEntityManager<Personal> _personalManager;
        readonly CommonEntityManager<Order> _orderManager;
        readonly CommonEntityManager<OrderDetail> _orderDetailManager;
        readonly CommonEntityManager<Produce> _produceManager;
        readonly WindowsNavigatore _windowsNavigatore;
        readonly ICollection<OrderDetail> _deleteOrderDetails = new List<OrderDetail>();
        readonly ICollection<OrderDetail> _updateOrderDetails = new List<OrderDetail>();
        readonly ICollection<OrderDetail> _createdOrderDetails = new List<OrderDetail>();
        readonly IList<OrderDetailDto> _orderDetailDtoList = new List<OrderDetailDto>();

        bool _isNewOrderComplate;
        bool _isUpdate;
        Order _newOrder = new Order();
        ushort _ordernumber = 0;

        public OrderCreate_DialogForm(IMapper mapper,
           CommonEntityManager<Personal> personalManager,
           CommonEntityManager<Order> orderManager,
           CommonEntityManager<OrderDetail> orderDetailManager,
           CommonEntityManager<Produce> produceManager,
           WindowsNavigatore windowsNavigatore)
        {
            InitializeComponent();

            _mapper = mapper;
            _personalManager = personalManager;
            _windowsNavigatore = windowsNavigatore;
            _orderManager = orderManager;
            _orderDetailManager = orderDetailManager;
            _produceManager = produceManager;
        }

        /// <summary>
        /// OrderDto For Trasfer its object Between Windows. Retrun OrderDto When Save it To Database        
        /// or Modified It. Return Null When Process Cancelled.
        /// Set Object For Give Object For Updating.
        /// </summary>        
        public OrderDto OrderDto
        {
            get
            {
                if (_isNewOrderComplate)
                {
                    var createSumPrice = _orderDetailDtoList.Sum(x => Convert.ToDecimal(x.SumPrice));
                    return _mapper.Map(_newOrder, new OrderDto() { SumPrice = Convert.ToUInt64(Math.Abs(createSumPrice)).ToString() });
                }
                else return null;
            }
            set
            {
                _isUpdate = true;
                _newOrder.ID = value.OrderID;
            }
        }

        #region Event


        #region Loading
        private void PersonalName_ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ICollection<string> pepeol = _personalManager.Entity.Select(x => x.Name).ToList();
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
                pepeol.Append("همه");
                PersonalName_ComboBox.ItemsSource = pepeol;
                PersonalName_ComboBox.SelectedIndex = 0;
                Serch_GroupBox.IsEnabled = true;
            }
        }

        private async void Order_DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //I Do this Becuase OrderDetail.Produce Is Null But OrderDetail.ProduceID Has Value
            //Even OrderDetail.Order and OrderDetail.OrderID has Value
            //TODO:Bug

            if (_isUpdate)
            {
                _newOrder = await _orderManager.FindByIdAsync(_newOrder.ID).ConfigureAwait(false);
                _ordernumber = _newOrder.Number;
                CreateDate_DatePicker.SelectedDate = _newOrder.Date;
                PersonalName_ComboBox.SelectedItem = PersonalName_ComboBox.ItemsSource
                    .Cast<string>().Single(x => x.Equals(_newOrder.Personal.Name));

                var temp = _orderDetailManager.Entity.Where(x => x.OrderID.Equals(_newOrder.ID)).ToList();////////
                foreach (var item in temp)
                {
                    item.Produce = await _produceManager.FindByIdAsync(item.ProduceID);
                    _orderDetailDtoList.Add(_mapper.Map<OrderDetailDto>(item));
                }
                Order_DataGrid.ItemsSource = _orderDetailDtoList.ToList();
            }
        }
        #endregion

        private void Create_OrderDetail_Click(object sender, RoutedEventArgs e)
        {
            var orderDetailDialog = _windowsNavigatore.Create_OrderDetailCreate_DialogForm(this);
            orderDetailDialog.ShowDialog();
            var orderDetailDto = (orderDetailDialog as OrderDetailCreate_DialogForm).OrderDetail;

            if (orderDetailDto != null)
            {
                CreateOrderDetail(orderDetailDto);
                Order_DataGrid.ItemsSource = _orderDetailDtoList.ToList();
            }
        }

        private void Update_OrderDetail_Click(object sender, RoutedEventArgs e)
        {
            if (Order_DataGrid.SelectedIndex > -1)
            {
                var orderDetailDialog = _windowsNavigatore.Create_OrderDetailCreate_DialogForm(this);
                var orderDetailDtoSelected = Order_DataGrid.SelectedItem as OrderDetailDto;
                var produce = _produceManager.Entity.FirstOrDefault(x => x.Name.Equals(orderDetailDtoSelected.Name));
                var orderDetail = _mapper.Map<OrderDetail>(orderDetailDtoSelected);
                orderDetail.Produce = produce;
                orderDetail.ProduceID = produce.ID;
                (orderDetailDialog as OrderDetailCreate_DialogForm).OrderDetail = orderDetail;

                orderDetailDialog.ShowDialog();
                var orderDetailDto = (orderDetailDialog as OrderDetailCreate_DialogForm).OrderDetail;

                if (orderDetailDto != null)
                {
                    UpdateOrderDetail(orderDetailDto);
                    Order_DataGrid.ItemsSource = _orderDetailDtoList.ToList();
                }
            }
            else MessageBox.Show("موردی انتخاب نشده است");
        }

        private void Delete_Order_Click(object sender, RoutedEventArgs e)
        {
            if (Order_DataGrid.SelectedIndex > -1)
            {
                var orderDetailDto = Order_DataGrid.SelectedItem as OrderDetailDto;
                DeleteOrderDetailAsync(orderDetailDto);
                _orderDetailDtoList.Remove(orderDetailDto);
                Order_DataGrid.ItemsSource = _orderDetailDtoList.ToList();
                Order_DataGrid.SelectedIndex = -1;
            }
            else MessageBox.Show("موردی انتخاب نشده است");
        }

        private async void Confirm_StausBarItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_createdOrderDetails.Count == 0 && !_isUpdate) MessageBox.Show("موردی ثبت نشده است");
            else
            {
                if (CreateDate_DatePicker.SelectedDate == null) MessageBox.Show("تاریخ ثبت انتخاب نشده");
                else
                {
                    _newOrder.Date = CreateDate_DatePicker.SelectedDate.Value;
                    _newOrder.Number = _ordernumber;
                    _newOrder.PersonalID = _personalManager.Entity.FirstOrDefault(x => x.Name.Equals(PersonalName_ComboBox.SelectedItem as string)).ID;

                    await ConfirmOrderAsync();
                    _isNewOrderComplate = true;
                    Close();
                }
            }
        }

        private void CancelStatuesBarItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        #endregion

        #region Process
        private async Task ConfirmOrderAsync()
        {
            if (_isUpdate)
                await _orderManager.UpdateAsync(_newOrder).ConfigureAwait(false);

            else
                await _orderManager.CreateAsync(_newOrder).ConfigureAwait(false);

            if (_createdOrderDetails.Count != 0)
                await _orderDetailManager.CreateAsync(_createdOrderDetails).ConfigureAwait(false);

            if (_updateOrderDetails.Count != 0)
            {
                foreach (var item in _updateOrderDetails)
                {
                    await _orderDetailManager.UpdateAsync(item).ConfigureAwait(false);
                }
            }

            if (_deleteOrderDetails.Count != 0)
                await _orderDetailManager.DeleteAsync(_createdOrderDetails).ConfigureAwait(false);
        }

        private void CreateOrderDetail(OrderDetail orderDetail)
        {
            orderDetail.OrderID = _newOrder.ID;
            _createdOrderDetails.Add(orderDetail);            
            _orderDetailDtoList.Add(_mapper.Map<OrderDetailDto>(orderDetail));
            _ordernumber++;
        }

        private void UpdateOrderDetail(OrderDetail orderDetail)
        {
            orderDetail.OrderID = _newOrder.ID;
            orderDetail.Order = _newOrder;

            //Check Item Exist in Database
            OrderDetail result = _createdOrderDetails.FirstOrDefault(x => x.ID.Equals(orderDetail.ID));
            if (result == null) _updateOrderDetails.Add(orderDetail);
            else
            {
                _createdOrderDetails.Remove(result);
                _createdOrderDetails.Add(result);
            }

            int elementIndex = _orderDetailDtoList.ToList().FindIndex(x => x.ID.Equals(orderDetail.ID));
            _orderDetailDtoList.RemoveAt(elementIndex);
            _orderDetailDtoList.Insert(elementIndex, _mapper.Map<OrderDetailDto>(orderDetail));
        }

        private async void DeleteOrderDetailAsync(OrderDetailDto orderDetailDto)
        {
            if (Order_DataGrid.SelectedIndex < -1)
            {
                //Check Item Exist in Database
                OrderDetail result = _createdOrderDetails.FirstOrDefault(x => x.ID.Equals(orderDetailDto.ID));
                if (result == null)
                {
                    result = await _orderDetailManager.FindByIdAsync(orderDetailDto.ID).ConfigureAwait(false);
                    _deleteOrderDetails.Add(result);
                }
                else _createdOrderDetails.Remove(result);
                _ordernumber--;
            }
        }
        #endregion

    }
}
