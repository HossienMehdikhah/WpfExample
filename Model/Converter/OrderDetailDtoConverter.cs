using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using WpfExample.Model.Dto;
using WpfExample.Model.Entity;

namespace WpfExample.Model.Converter
{
    public class OrderDetailDtoConverter : ITypeConverter<OrderDetail, OrderDetailDto>,
        ITypeConverter<OrderDetailDto, OrderDetail>
        
    {
        public OrderDetailDto Convert(OrderDetail source, OrderDetailDto destination, ResolutionContext context)
        {
            destination ??= new OrderDetailDto();
            destination.ID = source.ID;
            destination.ConcurrencyStamp = source.ConcurrencyStamp;
            destination.Count = source.Count.ToString();
            destination.Price = source.Price.ToString();
            destination.SumPrice = source.SumPrice.ToString();
            destination.Name = source.Produce.Name;
            destination.Code = source.Produce.Code;
            
            return destination;
        }

        public OrderDetail Convert(OrderDetailDto source, OrderDetail destination, ResolutionContext context)
        {
            destination ??= new OrderDetail();

            if (source.ID != null)
            {
                destination.ID = source.ID;
                destination.ConcurrencyStamp = source.ConcurrencyStamp;
            }
            destination.Count = uint.Parse(source.Count);
            destination.Price = ulong.Parse(source.Price);
            destination.SumPrice = ulong.Parse(source.SumPrice);

            return destination;
        }
    }
}
