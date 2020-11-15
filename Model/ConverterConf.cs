using System;
using System.Collections.Generic;
using System.Text;
using WpfExample.Model.Dto;
using WpfExample.Model.Entity;
using AutoMapper;
using WpfExample.Model.Converter;

namespace WpfExample.Model
{
    public class ConverterConf : Profile
    {
        public ConverterConf()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(x => x.PersonalName, y => y.MapFrom(z => z.Personal.Name))
                .ForMember(x => x.OrderID, y => y.MapFrom(z => z.ID))
                .ForMember(x => x.OrderDate, y => y.MapFrom(z => z.Date.ToString("d")))
                ;



            var orderDetailConverter = new OrderDetailDtoConverter();
            CreateMap<OrderDetail, OrderDetailDto>().ConvertUsing(orderDetailConverter);
            CreateMap<OrderDetailDto, OrderDetail>().ConvertUsing(orderDetailConverter);
        }
    }
}
