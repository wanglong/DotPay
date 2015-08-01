﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FC.Framework.Domain;
using FC.Framework;
using DotPay.MainDomain.Events;
using DotPay.MainDomain.Exceptions;
using FC.Framework.Repository;
using DotPay.Common;
using DotPay.MainDomain.Repository;

namespace DotPay.MainDomain
{
    public class AccountMonitor : IEventHandler<CNYDepositCompleted>,                 //cny充值完成
                                  IEventHandler<CNYDepositUndoComplete>,              //撤销cny充值
                                  IEventHandler<VirtualCoinDepositCompleted>,         //虚拟币充值完成 
                                  IEventHandler<CNYWithdrawCreated>,                  //cny提现创建 
                                  IEventHandler<CNYWithdrawSetFee>,                   //cny提现计入提现费
                                  IEventHandler<InsideTransferTransactionCreated>,
                                  IEventHandler<InsideTransferTransactionComplete>,
                                  IEventHandler<OutboundTransferTransactionCreated>,
                                  IEventHandler<OutboundTransferTransactionConfirmed>,
                                  IEventHandler<OutboundTransferTransactionFailed>,
        //IEventHandler<CNYWithdrawCompleted>,                //cny提现完成
                                  IEventHandler<CNYWithdrawCanceled>
    /*,                 //cny提现撤销
    IEventHandler<VirtualCoinWithdrawCreated>,          //虚拟币提现创建
    IEventHandler<VirtualCoinWithdrawSetFee>,           //虚拟币计入提现费
    IEventHandler<VirtualCoinWithdrawCanceled>          //cny提现撤销 */
    {
        private IRepository repos = IoC.Resolve<IRepository>();

        public void Handle(CNYDepositCompleted @event)
        {
            var account = repos.FindById<CNYAccount>(@event.AccountID);

            account.BalanceIncrease(@event.DepositAmount);

            this.Apply(new AccountChangedByDeposit(@event.DepositUserID, @event.AccountID, @event.DepositAmount,
                                                   @event.DepositID, CurrencyType.CNY));
        }

        public void Handle(CNYDepositUndoComplete @event)
        {
            var account = repos.FindById<CNYAccount>(@event.AccountID);

            if (account.Balance < @event.DepositAmount)
                throw new DepositUndoCompleteForCNYException();

            account.BalanceDecrease(@event.DepositAmount);

            this.Apply(new AccountChangedByCancelDeposit(account.UserID, @event.AccountID, @event.DepositAmount,
                                                         @event.DepositID, CurrencyType.CNY));
        }

        public void Handle(VirtualCoinDepositCompleted @event)
        {
            var account = IoC.Resolve<IAccountRepository>().FindByIDAndCurrency(@event.AccountID, @event.Currency);

            account.BalanceIncrease(@event.DepositAmount);

            this.Apply(new AccountChangedByDeposit(account.UserID, @event.AccountID, @event.DepositAmount,
                                                   @event.DepositID, @event.Currency));
        }


        public void Handle(CNYWithdrawCreated @event)
        {
            var account = repos.FindById<CNYAccount>(@event.AccountID);

            account.BalanceDecrease(@event.Amount);

            this.Apply(new AccountChangedByWithdrawCreated(account.UserID, @event.AccountID, @event.Amount,
                                                           @event.CNYWithdrawEntity.UniqueID, CurrencyType.CNY));
        }

        public void Handle(CNYWithdrawSetFee @event)
        {
            var account = repos.FindById<CNYAccount>(@event.CNYWithdraw.AccountID);

            account.BalanceDecrease(@event.CNYWithdraw.Fee);
            //account.LockedIncrease(@event.CNYWithdraw.Fee);
        }

        //public void Handle(VirtualCoinWithdrawSetFee @event)
        //{
        //    var account = IoC.Resolve<IAccountRepository>().FindByIDAndCurrency(@event.AccountID, @event.Currency);

        //    account.BalanceDecrease(@event.VirtualCoinWithdraw.Fee);
        //}

        //public void Handle(CNYWithdrawCompleted @event)
        //{
        //    var cnyWithdraw = repos.FindById<CNYWithdraw>(@event.WithdrawID);
        //    var account = repos.FindById<Account>(cnyWithdraw.AccountID);

        //    var withdrawSum = cnyWithdraw.Amount + cnyWithdraw.Fee;
        //    account.LockedDecrease(withdrawSum);

        //    this.Apply(new AccountChangedByWithdrawComplete(cnyWithdraw.AccountID, withdrawSum, @event.WithdrawID, CurrencyType.CNY));
        //} 

        public void Handle(CNYWithdrawCanceled @event)
        {
            var cnyWithdraw = IoC.Resolve<IWithdrawRepository>().FindByUniqueIdAndCurrency(@event.WithdrawUniqueID, CurrencyType.CNY);
            var account = IoC.Resolve<IAccountRepository>().FindByIDAndCurrency(@event.CNYAccountID, CurrencyType.CNY);

            var withdrawSum = cnyWithdraw.Amount + cnyWithdraw.Fee;
            //account.LockedDecrease(withdrawSum);
            account.BalanceIncrease(withdrawSum);

            this.Apply(new AccountChangedByWithdrawCancel(cnyWithdraw.UserID, cnyWithdraw.AccountID, withdrawSum, @event.WithdrawUniqueID, CurrencyType.CNY));
        }

        //public void Handle(VirtualCoinWithdrawCanceled @event)
        //{
        //    var withdraw = IoC.Resolve<IWithdrawRepository>().FindByUniqueIdAndCurrency(@event.WithdrawUniqueID, @event.Currency);
        //    var account = IoC.Resolve<IAccountRepository>().FindByIDAndCurrency(@event.AccountID, @event.Currency);

        //    var withdrawSum = withdraw.Amount + withdraw.Fee;
        //    //account.LockedDecrease(withdrawSum);
        //    account.BalanceIncrease(withdrawSum);

        //    this.Apply(new AccountChangedByWithdrawCancel(withdraw.UserID, withdraw.AccountID, withdrawSum, @event.WithdrawUniqueID, @event.Currency));
        //}

        //public void Handle(VirtualCoinWithdrawCreated @event)
        //{
        //    var account = IoC.Resolve<IAccountRepository>().FindByIDAndCurrency(@event.AccountID, @event.Currency);

        //    account.BalanceDecrease(@event.Amount);
        //    //account.LockedIncrease(@event.Amount); 

        //    this.Apply(new AccountChangedByWithdrawCreated(account.UserID, @event.AccountID, @event.Amount,
        //                                                   @event.WithdrawEntity.UniqueID, @event.Currency));
        //}


        public void Handle(InsideTransferTransactionComplete @event)
        {
            var transferTransaction = IoC.Resolve<IInsideTransferTransactionRepository>().FindTransferTxByID(@event.InternalTransferID, @event.Currency);
            var toAccount = IoC.Resolve<IAccountRepository>().FindByUserIDAndCurrency(transferTransaction.ToUserID, @event.Currency);
            var fromAccount = IoC.Resolve<IAccountRepository>().FindByUserIDAndCurrency(transferTransaction.FromUserID, @event.Currency);

            if (toAccount == null)
            {
                toAccount = AccountFactory.CreateAccount(transferTransaction.ToUserID, @event.Currency);
                IoC.Resolve<IRepository>().Add(toAccount);
            }

            fromAccount.BalanceDecrease(transferTransaction.Amount);
            toAccount.BalanceIncrease(transferTransaction.Amount);


            //触发用户账户变更版本记录事件
            this.Apply(new AccountChangedByInsideTransferCompleted(fromAccount.ID, 0, transferTransaction.Amount, @event.InternalTransferID, @event.Currency));
            this.Apply(new AccountChangedByInsideTransferCompleted(toAccount.ID, transferTransaction.Amount, 0, @event.InternalTransferID, @event.Currency));
        }

        public void Handle(InsideTransferTransactionCreated @event)
        {
            var fromAccount = IoC.Resolve<IAccountRepository>().FindByUserIDAndCurrency(@event.FromUserID, @event.Currency);

            if (fromAccount == null)
            {
                fromAccount = AccountFactory.CreateAccount(@event.FromUserID, @event.Currency);
                IoC.Resolve<IRepository>().Add(fromAccount);
            }

        }

        public void Handle(OutboundTransferTransactionCreated @event)
        {
            var currency = CurrencyType.CNY; //目前只有CNY一种货币用户可以持有，所以暂时指定为CNY
            var fromAccount = IoC.Resolve<IAccountRepository>().FindByUserIDAndCurrency(@event.FromUserID, currency);

            if (fromAccount == null)
            {
                fromAccount = AccountFactory.CreateAccount(@event.FromUserID, currency);
                IoC.Resolve<IRepository>().Add(fromAccount);
            }
        }

        public void Handle(OutboundTransferTransactionConfirmed @event)
        {
            var currency = CurrencyType.CNY; //目前只有CNY一种货币用户可以持有，所以暂时指定为CNY
            var transfer = IoC.Resolve<IRepository>().FindById<OutboundTransferTransaction>(@event.OutboundTransferID);
            var fromAccount = IoC.Resolve<IAccountRepository>().FindByUserIDAndCurrency(transfer.FromUserID, currency); 

            fromAccount.BalanceDecrease(transfer.SourceAmount);

            //触发用户账户变更版本记录事件
            this.Apply(new AccountChangedByOutboundTransferConfirm(fromAccount.ID, 0, transfer.SourceAmount, transfer.SequenceNo, currency));
        }

        public void Handle(OutboundTransferTransactionFailed @event)
        {
            var currency = CurrencyType.CNY; //目前只有CNY一种货币用户可以持有，所以暂时指定为CNY
            var transfer = IoC.Resolve<IRepository>().FindById<OutboundTransferTransaction>(@event.OutboundTransferID);
            var fromAccount = IoC.Resolve<IAccountRepository>().FindByUserIDAndCurrency(transfer.FromUserID, currency);


            fromAccount.BalanceIncrease(transfer.SourceAmount);
            //触发用户账户变更版本记录事件
            this.Apply(new AccountChangedByOutboundTransferConfirm(fromAccount.ID, transfer.SourceAmount, 0, transfer.SequenceNo, currency));
        }
    }
}