﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FC.Framework;
using FC.Framework.Domain;
using DotPay.Common;
using DotPay.MainDomain.Events;
using FC.Framework.Repository;
using DotPay.MainDomain.Repository;
using DotPay.Tools.DistributedMessageSender;
using DotPay.MainDomain.Exceptions;

namespace DotPay.MainDomain
{
    public class WithdrawVerifyMonitor : IEventHandler<CNYWithdrawCreated>
                                         // ,
                                         //IEventHandler<VirtualCoinWithdrawCreated>
    {
        public void Handle(CNYWithdrawCreated @event)
        {
            var currency = IoC.Resolve<ICurrencyRepository>().FindByCurrencyType(CurrencyType.CNY);

            if (currency.WithdrawOnceLimit < @event.CNYWithdrawEntity.Amount || currency.WithdrawOnceMin > @event.CNYWithdrawEntity.Amount)
                throw new WithdrawAmountOutOfRangeException();

        }

        //public void Handle(VirtualCoinWithdrawCreated @event)
        //{
        //    var currency = IoC.Resolve<ICurrencyRepository>().FindById<Currency>((int)@event.Currency);
        //    if (currency.WithdrawOnceLimit < @event.WithdrawEntity.Amount || currency.WithdrawOnceMin > @event.WithdrawEntity.Amount)
        //        throw new WithdrawAmountOutOfRangeException();
        //}

    }
}